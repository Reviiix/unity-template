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
| `ThemeService` | C# | Light or dark theme and its colours (see Look) | dark mode | `Changed` |
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

- Each menu (`MainMenu`, `StageSelectionMenu`, `StatisticsMenu`, `SettingsMenu`, `StoreMenu`) is a `MenuBase` component on the root canvas of its own prefab in `Prefabs/UserInterface/Menus`. Every menu prefab is a variant of `Menu.prefab`, which holds the canvas set-up they share: overlay canvas, Scale With Screen Size at 1600×900 with Expand, and a graphic raycaster. Each variant overrides only what differs from the base, such as its name and whether its canvas starts enabled.
- `UserInterfaceManager` references the menu instances in `Initialisation.unity` and initialises them in a fixed order; showing a menu enables its canvas and hides the others.
- Popups derive from `PopUpWindow` and move through Hidden, Opening, Open and Closing. They slide in from the left with DOTween (`Ease.OutExpo`, 0.87 s, from `Screen.width * 2 + 500` units off-screen), which matches the original frame-based movement at 30 fps.
- Three ways to close a popup, matching the original game: `CloseImmediately` (vanish with the menu whoosh; daily CLAIM, anniversary CLAIM), `CloseWithSlide` (slide out silently; every X button and the welcome CONTINUE), `CloseWithSlide(playSound: true)` (confirmation YES/NO).
- Every popup prefab is a variant of `Prefabs/UserInterface/PopUps/PopUp.prefab`, the shared frame: a dimmed backdrop and a white rounded panel with a title and a round close button. `ConfirmationPopUp.prefab` removes the title and close button (it shows its own question with NO and YES); like every popup, its backdrop dims and blocks taps on the menus behind it.
- Conditional popups are Addressable prefabs instantiated with `Addressables.InstantiateAsync` and released with `ReleaseInstance` after closing. The confirmation popup is an instance of `ConfirmationPopUp.prefab` in the scene and is reused.
- Canvas sort orders: background and the unused in-stage HUD 0, menus 1 (set on `Menu.prefab`), the unused pause and game-over screens 2 and 4, popups 5, confirmation, achievement toast and cheats 10, stage fade 100.
- `MenuRenderThrottle` saves battery in the menus: once nothing has been touched, moved or tweened for 2 s it renders only every 4th frame (`OnDemandRendering.renderFrameInterval`). Any input or playing tween, and every frame inside a stage, renders at the full rate. Scripts still update every frame, so timers and logic are unaffected; an animation that isn't a DOTween tween must keep the frame rate up itself.
- Android back closes the top popup like its X button (the confirmation answers NO), returns from a sub-menu to the main menu, and does nothing on the main menu or inside a stage.
- Timings preserved from the original: currency and wheel roll-ups 3.37 s, linear (101 frames at 30 fps); experience bar 3 s, OutQuad; daily wheel 7 s, InOutQuart; stage fade 1 s out and 1 s in, linear.

## Look

The UI follows one small design system with a light and a dark theme, so a game made from the template can be restyled or rebranded in a few places.

Every colour is a `ThemeColour`. `Assets/Settings/Themes/LightTheme.asset` and `DarkTheme.asset` (`ThemePalette`, assigned on `GameInitialiser`) hold each theme's value; edit them to rebrand.

| Token | Light | Dark | Used for |
|---|---|---|---|
| Background | `#F3F4F6` | `#0E1013` | Behind the content card |
| Surface | `#FFFFFF` | `#181B20` | Content card, popup panels, the main menu's credit chips |
| Raised | `#FFFFFF` | `#2C313A` | The selected tab on a tab bar |
| Muted | `#EEF0F3` | `#22262D` | Secondary buttons, rows, tab bars, stage tiles, the store's credit chips |
| Border | `#E4E7EB` | `#30353E` | Progress tracks, outlines, dividers, the wheel's lines |
| Track | `#C9CED6` | `#3A404A` | The volume slider track, switches when off |
| TextPrimary, TextSecondary, TextTertiary | `#15171C`, `#5C6370`, `#666D7A` | `#F1F2F4`, `#A3A9B4`, `#8C929D` | Primary, secondary and caption text |
| Accent | `#3E63DD` | `#5B7FF0` | Primary buttons, progress, earned stars, switches when on, the wheel pointer |
| OnAccent | `#FFFFFF` | `#FFFFFF` | Text on the accent or danger colour, the switch knob |
| AccentSoft | `#DBE5FC` | `#24305A` | Unlocked achievement rows |
| Danger | `#D63B3F` | `#EF5B5E` | RESET ALL, YES on the confirmation, the cheats label |
| Dim | `#0E1014` at 58% | `#000000` at 62% | The overlay behind popups, the pause screen and the cheat page |
| StarUnearned | `#80868F` | `#6B717C` | Unearned stars (3:1 on the stage tile) |

- Theming: `ThemeService` holds the current theme (saved as `SaveData.darkMode`; the **DARK MODE** switch in Settings flips it) and raises `Changed`. A `ThemedGraphic` on an Image, RawImage or text names its token and recolours it on every change; scripts that set colours themselves (statistics tabs, stage stars, achievement rows, the switch) hold `ThemeColour` fields and resolve them through `ThemeService`. A graphic has one or the other, never both. In the Editor, graphics show their light colour (a test checks this).
- Type: Inter Semi Bold for the game title (96), screen titles (52), popup titles (42), buttons (34) and small uppercase labels (26, letter-spaced); Inter Regular for body text (30-34) and captions (26). Sizes are canvas units on the 1600x900 reference; nothing is smaller than 26, about 11 pt on a phone.
- Shapes: white sprites in `Assets/Graphics/Shapes` (rounded rectangle and outline, circle, soft shadow, triangle, star), tinted per use. The rounded rectangle is 9-sliced, so one sprite gives any corner radius: set the Image's Pixels Per Unit Multiplier to 64 divided by the radius (buttons 24, cards 32, popups 36, rows 20).
- Kit: `Prefabs/UserInterface/Kit` holds `Button.prefab` (shape, label, press and disabled tints) with its variants, and `Switch.prefab` (an on/off `Toggle` with `ToggleSwitch`). Changing a kit prefab changes every instance.
- The daily wheel texture (`Assets/Graphics/Wheel.png`) is white lines and ring on transparent, tinted by the wheel's RawImage, with no numbers; `SpinningWheel` writes each segment's value on its label at start-up, so changing a prize is a change to its segment value.

## Addressables

| Group | Contents | Owner of the handle |
|---|---|---|
| ConditionalMenus | First-open, daily and anniversary popups | `UserInterfaceManager` (instantiate/release per popup) |
| Prefabs | Stage list item | `StageSelectionMenu` (loaded once at start-up and released in its `OnDestroy`; the stage buttons use the prefab's sprites and font until then) |
| Scenes | Stage scenes | Addressables (Single-mode scene load) |
| Achievements | Achievement icons | `AchievementItemBase` (released when the row or toast is destroyed) |

Pooled prefabs (`AudioSourceForPool`, `AchievementPopUp`) and the achievement list item are always in memory, so they are referenced directly instead of through Addressables.

## Assemblies

| Assembly | Folder | References |
|---|---|---|
| `UnityTemplate` | `Scripts/Runtime` | Addressables, ResourceManager, TextMeshPro, UnityEngine.UI, Services Core/Authentication/Remote Config (DOTween is auto-referenced) |
| `UnityTemplate.Editor` | `Scripts/Editor` | UnityTemplate (Editor only) |
| `UnityTemplate.Tests.EditMode` | `Scripts/Tests/EditMode` | UnityTemplate, UnityTemplate.Editor, Test Runner, Addressables Editor, TextMeshPro |
| `UnityTemplate.Tests.PlayMode` | `Scripts/Tests/PlayMode` | UnityTemplate, Test Runner, UnityEngine.UI |

When code needs a new package, add its assembly to the `references` list of the asmdef.

## Decisions

- Kept the menu loop exactly as it was: no in-stage HUD, pause or game-over screens (the canvases remain in the scene, unused, for future gameplay), and no way back from a stage yet.
- Menus and popups are prefab variants of two bases, `Menu.prefab` and `PopUp.prefab`, so set-up they share is edited once and the scene holds only their instances.
- The UI uses one light palette with a single accent, Inter, and shapes drawn from a few tinted sprites, so a game can rebrand by changing a handful of prefabs (see Look).
- Remote Config changes apply from the next launch, so nothing changes mid-session and boot never waits for the network.
- Animations are time-based and pinned to what devices showed at the original 30 fps, now running at 60 fps.
- Ratings are stored by stage position: append stages to the catalogue rather than inserting them.
- `[FormerlySerializedAs]` attributes stay on renamed fields until every scene and prefab has been re-saved in the Editor (`AssetDatabase.ForceReserializeAssets`).
