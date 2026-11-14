# Unity Template

A mobile game template: boot scene, main menu, stage selection, statistics, settings, store, a daily bonus wheel, first-open and anniversary popups, currencies, experience levels, achievements and a save system.

| | |
|---|---|
| Unity | 6000.3.9f1 |
| Platforms | iOS 15+ and Android 7.0+ (API 24), landscape only |
| Scripting | IL2CPP, ARM64 (Android), .NET Standard 2.1 |
| Packages | Addressables, Remote Config (Unity Gaming Services), TextMesh Pro (uGUI), Test Framework, DOTween (in `Assets/Plugins`) |

## Getting started

1. Open the project with Unity 6000.3.9f1.
2. Press **Play**. The game always starts from `Assets/Scenes/Initialisation.unity`, because that scene's `GameInitialiser` creates every service. You can switch this off under **Tools > Scenes > Always Play From Initialisation Scene**.
3. In the Editor, Addressables use the **Use Asset Database** play mode, so no content build is needed. To test real bundles, run **Window > Asset Management > Addressables > Groups > Build > New Build > Default Build Script**, then set **Play Mode Script** to **Use Existing Build**.

### Device builds

- Build Addressables content first (see above); player builds include the local bundles automatically.
- A **Development Build** turns on the cheat menu (top-right **CHEATS** button) and verbose `GameLog` messages. Release builds remove both.
- iOS: set your signing team in **Player Settings > iOS**. Android: set a keystore in **Player Settings > Android > Publishing Settings**.

### Unity Gaming Services (Remote Config)

- The project is linked to a Unity Cloud project in **Edit > Project Settings > Services**. A game made from this template should link to **its own** cloud project.
- At start-up the game reads the Remote Config values cached on the previous launch and fetches fresh ones in the background, so **dashboard changes apply from the next launch**. Offline or unlinked, the built-in defaults in `GameConfiguration` are used and a warning is logged.
- Signing in is anonymous, which creates one Unity player per install: mention this in the App Store and Play privacy declarations.
- The fetch uses the default `production` environment. If your dashboard only has other environments, the sign-in fails with "invalid environment name" (a warning, the game carries on): create a `production` environment or set one with `InitializationOptions.SetEnvironmentName` in `RemoteConfiguration.FetchForNextLaunchAsync`.
- The dashboard key is `Configuration` (type JSON). Field names must match `GameConfiguration`:

```json
{
  "Holiday": "None",
  "AmountOfExperienceLevels": 50,
  "DynamicAchievements": {
    "highScore": -1, "highScoreReward": 0,
    "consecutivePlayTimeInSeconds": -1, "consecutivePlayTimeInSecondsReward": 0,
    "totalPlayTimeInSeconds": -1, "totalPlayTimeInSecondsReward": 0,
    "experienceGained": -1, "experienceGainedReward": 0,
    "stagesComplete": -1, "stagesCompleteReward": 0
  }
}
```

A target of `-1` disables that weekly achievement. Missing or invalid values keep their defaults.

## Project layout

```
Assets/
  Audio/                    UI sound effects
  Graphics/                 Sprites and textures (mobile import settings: Tools > Apply Mobile Import Settings)
  Prefabs/                  Pooled objects, menu items and popups (the popups are Addressable)
  Scenes/                   Initialisation.unity (boot scene) and Scenes/StageGroupOne/... (stages)
  Settings/                 StageCatalogue and AchievementCatalogue (designer-editable data)
  Scripts/
    Runtime/                UnityTemplate assembly, one folder per feature (namespace UnityTemplate.<Folder>)
    Editor/                 UnityTemplate.Editor assembly: Tools menu, play-mode start scene, import settings
    Tests/EditMode/         Service, save and scene-wiring tests
    Tests/PlayMode/         Smoke tests that boot the game and walk the menu loop
  Plugins/DOTween/          Tweening library (vendor code, do not edit)
  AddressableAssetsData/    Addressables groups and settings
  Editor/RemoteConfig/      Remote Config editor data (this path is fixed by the package)
```

How the code fits together (boot order, services, saves, popups) is described in [ARCHITECTURE.md](ARCHITECTURE.md).

## Common tasks

| Task | How |
|---|---|
| Add a stage | Create the scene under `Assets/Scenes/...`, mark it Addressable (Scenes group), then drag it into a slot of `Assets/Settings/StageCatalogue.asset`. Empty slots show as locked stages. Append new stages: ratings are stored by position. |
| Add a popup | Duplicate one of the prefabs in `Prefabs/UserInterface/ConditionalMenus`, give it a script that derives from `PopUpWindow`, mark it Addressable, reference it from `UserInterfaceManager` and open it with `OpenPopUpAsync`. |
| Add a service | Write a plain C# class (implement `ISaveable` if it saves), create it and register it in `GameInitialiser.Awake`, then resolve it from MonoBehaviours with `ServiceLocator.Get<T>()` in `OnEnable`. |
| Save a new value | Add a field to `SaveData`, read it in the owning service's `Load` and write it in `Save`. Never rename or retype an existing field; bump `SaveData.CurrentVersion` and migrate if a field's meaning changes. |
| Pool a new object | Add an entry to `PoolKey` (append only) and a pool on the `ObjectPooler` in `Initialisation.unity`, then use `ObjectPooler.Get`/`Release`. |
| Change achievements | Edit `Assets/Settings/AchievementCatalogue.asset`. Switch the feature on with **Achievements Enabled** on `GameInitialiser`. |
| Tune features | Select `GlobalGameManagers` in `Initialisation.unity`: `GameInitialiser` exposes achievements, the daily bonus and the target frame rate. |
| Import new sprites or sounds | Put them in `Assets/Graphics` or `Assets/Audio` and run **Tools > Apply Mobile Import Settings**. |

## Coding conventions

- Services are plain C# classes registered in the `ServiceLocator` by `GameInitialiser`; no `static` managers and no `Singleton<T>`.
- One central initialiser: everything that needs set-up is called from `GameInitialiser.Awake`, in order.
- Object pooling for anything created repeatedly; prefabs for anything reused.
- Cache references and precompute at start-up rather than at runtime.
- Self-documenting names, a one-line `/// <summary>` on every class and method, few other comments.
- Private by default; `[SerializeField]` only for values designers tweak.
- Namespaces `UnityTemplate.<Folder>`; never name a type after its own namespace.
- `StringBuilder` or TMP `SetText` for text that changes at runtime.
- Prefer `async`/`await` (Unity `Awaitable`) or DOTween to `Update` and coroutines. The one `Update` (Android back button in `UserInterfaceManager`) exists because the old Input Manager can only be polled.
- Every `async void` entry point catches and logs its exceptions; tweens use `SetLink(gameObject)`.
- Vocabulary: a **stage** is a playable scene; a **level** is an experience level.
- Keep **Reload Domain** on in Enter Play Mode Options: DOTween 1.2 and the Unity Services SDKs keep static state between play sessions.

## Tests

- **Window > General > Test Runner**: EditMode (services, saves, scene wiring) and PlayMode (boots the game and walks first launch, the daily wheel and starting a stage).
- Command line: `unity test <project> --mode EditMode` and `unity test <project> --mode PlayMode`. The PlayMode tests back up and restore your local save.

Manual checklist before a release: cold start, first open, daily wheel (spin and X), anniversary (Tools > Save Data tools), stage fade, RESET ALL, Android back button, airplane mode.

## Troubleshooting

| Problem | Fix |
|---|---|
| `No X is registered` | You pressed Play in another scene with the start-scene option off. Play from `Initialisation.unity`. |
| `InvalidKeyException` from Addressables | The asset is not marked Addressable, or the content build is stale: rebuild Addressables. |
| Remote Config warnings | Normal offline or when the project is not linked to Unity Cloud. |
| Missing Script after renaming a file | Rename C# files together with their `.meta` (in the Editor, or with `git mv` on both). |
| DOTween's **Create ASMDEF** button | Not used: DOTween stays in `Assets/Plugins` (Assembly-CSharp-firstpass) and only its core API is used. |

## Git workflow

Git flow: branch from `develop` (`feature/...`, `bugfix/...`), one commit per change, and merge into `develop` when reviewed.
