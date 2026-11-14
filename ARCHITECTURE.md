# Architecture

## Boot sequence

`Assets/Scenes/Initialisation.unity` is the only scene in the build list. Its `GlobalGameManagers` object holds `GameInitialiser`, which runs before every other script (`[DefaultExecutionOrder(-1000)]`).

`GameInitialiser.Awake` (synchronous, in this order):

1. Sets the target frame rate and initialises DOTween.
2. Creates `SaveService` and reads the cached Remote Config values (`RemoteConfiguration.LoadCachedConfiguration`).
3. Creates the plain C# services, registers the saveable ones with `SaveService` and registers everything with `ServiceLocator`.
4. `SaveService.Load()` hands the save file (or a new game) to every service.
5. Initialises the scene services in order: `ObjectPooler` (prewarms pools), `AudioManager` (takes pooled audio sources, wires click sounds), `UserInterfaceManager` (wires menus).
6. Counts the app open and starts the play-time clock.

Because all of this happens in `Awake`, every other component can resolve services in its own `Awake`/`OnEnable`.

`GameInitialiser.Start` (asynchronous) starts the background Remote Config fetch and runs the start-up popups: the first-open popup on the first launch, otherwise the anniversary popup and then the daily bonus popup, one after another.

`OnApplicationPause(true)` and `OnApplicationQuit` save, because mobile operating systems kill backgrounded apps without warning.

`PersistObjects` keeps the `GlobalGameManagers` and `UserInterface` roots alive when a stage scene loads (stages load in Single mode).

## Services

| Service | Kind | Responsibility | Saves | Events |
|---|---|---|---|---|
| `SaveService` | C# | JSON save file, atomic writes, backup, reset | - | `DataLoaded` |
| `CreditsService` | C# | Credits and premium credits | balances | `BalanceChanged` |
| `ExperienceService` | C# | Experience and level (table precomputed once) | total experience | `ExperienceChanged`, `LevelChanged` |
| `ProgressService` | C# | Stage ratings, unlocks, CONTINUE target, high score | ratings, high score | - |
| `EngagementService` | C# | App opens, play time, daily bonus and streak, anniversaries | yes | - |
| `AchievementService` | C# | Permanent and remotely configured achievements | unlocked ids | `AchievementUnlocked` |
| `StageLoader` | C# | Fade, load a stage scene, fade back | - | - |
| `ObjectPooler` | MonoBehaviour | Pools of pre-instantiated prefabs (`PoolKey`) | - | - |
| `AudioManager` | MonoBehaviour | UI sounds, master volume and mute | volume, mute | `VolumeChanged` |
| `UserInterfaceManager` | MonoBehaviour | Menus, popups and queue, stage fade, Android back | - | - |
| `GameManager` | MonoBehaviour (stage scene) | Stage timer, score, pause | via ProgressService | - |

Rules:
- Plain C# services get their dependencies through their constructor from `GameInitialiser`; only MonoBehaviours call `ServiceLocator.Get<T>()`.
- Views subscribe to service events in `OnEnable` and unsubscribe in `OnDisable`, and refresh on `SaveService.DataLoaded` (RESET ALL).
- No mutable static state outside `ServiceLocator`; `ServiceLocator.Reset` runs before every play session.

## Saving

- `SaveData` holds primitive fields only (`JsonUtility`): no properties, dictionaries, `DateTime` or nested arrays. Dates are stored as ticks and days as day numbers.
- Each `ISaveable` service reads its fields in `Load(SaveData)` and writes them in `Save(SaveData)`.
- Writes go to `save.json.tmp` and are swapped in with `File.Replace`, which keeps the previous file as `save.json.backup`. An unreadable file falls back to the backup, then to a new game.
- Nothing is saved before the first load, so quitting during boot never overwrites progress with defaults.
- Change rules: add fields freely; never rename or retype one; for a change in meaning, bump `SaveData.CurrentVersion` and migrate in `SaveService`.
- RESET ALL (`SaveService.DeleteAll`) deletes the files, hands every service a new game and raises `DataLoaded` so views refresh.

## User interface

- Menus (`MainMenu`, `StageSelectionMenu`, `StatisticsMenu`, `SettingsMenu`, `StoreMenu`) are `[Serializable]` classes stored on `UserInterfaceManager`; showing a menu enables its canvas and hides the others.
- Popups derive from `PopUpWindow` and move through Hidden, Opening, Open and Closing. They slide in from the left with DOTween (`Ease.OutExpo`, 0.87 s, from `Screen.width * 2 + 500` units off-screen), which matches the original frame-based movement at 30 fps.
- Three ways to close a popup, matching the original game: `CloseImmediately` (vanish with the menu whoosh; daily CLAIM, anniversary CLAIM), `CloseWithSlide` (slide out silently; every X button and the welcome CONTINUE), `CloseWithSlide(playSound: true)` (confirmation YES/NO).
- Conditional popups are Addressable prefabs instantiated with `Addressables.InstantiateAsync` and released with `ReleaseInstance` after closing. The confirmation popup lives in the scene and is reused.
- Canvas sort orders: menus 0-3, popups 5, confirmation, achievement toast and cheats 10, stage fade 100.
- Android back closes the top popup like its X button (the confirmation answers NO), returns from a sub-menu to the main menu, and does nothing on the main menu or inside a stage.
- Timings preserved from the original: currency and wheel roll-ups 3.37 s, linear (101 frames at 30 fps); experience bar 3 s, OutQuad; daily wheel 7 s, InOutQuart; stage fade 1 s out and 1 s in, linear.

## Addressables

| Group | Contents | Owner of the handle |
|---|---|---|
| ConditionalMenus | First-open, daily and anniversary popups | `UserInterfaceManager` (instantiate/release per popup) |
| Prefabs | Stage list item | `StageSelectionMenu` (loaded once, released after building the list) |
| Scenes | Stage scenes | Addressables (Single-mode scene load) |
| Achievements | Achievement icons | `AchievementItemBase` (released when the row or toast is destroyed) |

Pooled prefabs (`AudioSourceForPool`, `AchievementPopUp`) and the achievement list item are always in memory, so they are referenced directly instead of through Addressables.

## Assemblies

| Assembly | Folder | References |
|---|---|---|
| `UnityTemplate` | `Scripts/Runtime` | Addressables, ResourceManager, TextMeshPro, UnityEngine.UI, Services Core/Authentication/Remote Config (DOTween is auto-referenced) |
| `UnityTemplate.Editor` | `Scripts/Editor` | UnityTemplate (Editor only) |
| `UnityTemplate.Tests.EditMode` | `Scripts/Tests/EditMode` | UnityTemplate, Test Runner, Addressables Editor |
| `UnityTemplate.Tests.PlayMode` | `Scripts/Tests/PlayMode` | UnityTemplate, Test Runner, UnityEngine.UI |

When code needs a new package, add its assembly to the `references` list of the asmdef.

## Decisions

- Kept the menu loop exactly as it was: no in-stage HUD, pause or game-over screens (the canvases remain in the scene, unused, for future gameplay), and no way back from a stage yet.
- Remote Config changes apply from the next launch, so nothing changes mid-session and boot never waits for the network.
- Animations are time-based and pinned to what devices showed at the original 30 fps, now running at 60 fps.
- Ratings are stored by stage position: append stages to the catalogue rather than inserting them.
- `[FormerlySerializedAs]` attributes stay on renamed fields until every scene and prefab has been re-saved in the Editor (`AssetDatabase.ForceReserializeAssets`).
