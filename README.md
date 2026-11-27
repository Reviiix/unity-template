# Unity Template

A mobile game template: boot scene, main menu, stage selection, statistics, settings, store, a daily bonus wheel, a welcome reward and anniversary popups, currencies, experience levels, achievements and a save system.

| | |
|---|---|
| Unity | 6000.3.28f1 (the build agent's editor; upgrade the template when that changes) |
| Platforms | iOS 15+ and Android 7.1+ (API 25), landscape only; WebGL for browser previews |
| Scripting | IL2CPP, ARM64 (Android), .NET Standard 2.1 |
| Packages | Addressables, Remote Config (Unity Gaming Services), TextMesh Pro (uGUI), Test Framework, DOTween (in `Assets/Plugins`), Unity Pipeline (Editor tooling for the `unity` command-line tool; not in players) |

## Getting started

Install [Git LFS](https://git-lfs.com) and run `git lfs install` once before cloning: images, audio, video, models and fonts that a game adds are stored in LFS. The template's own binaries stay ordinary git files, so copies of the template (GitHub's **Use this template** doesn't copy LFS objects) work; `.gitattributes` lists them. Clone with `git clone --recurse-submodules`, or run `git submodule update --init` in an existing clone, to get the coding standards in `unity-coding-standards/`.

1. Open the project with Unity 6000.3.28f1.
2. Press **Play**. The game always starts from `Assets/Scenes/Initialisation.unity`, because that scene's `GameInitialiser` creates every service. You can switch this off under **Tools > Scenes > Always Play From Initialisation Scene**.
3. In the Editor, Addressables use the **Use Asset Database** play mode, so no content build is needed. To test real bundles, run **Window > Asset Management > Addressables > Groups > Build > New Build > Default Build Script**, then set **Play Mode Script** to **Use Existing Build**.

### Making a game from the template

Run **Tools > New Game Setup...**, type the game's name and press **Apply**. It does the following:

- sets the product name,
- sets the Android, iOS and Standalone identifiers to `com.LeonHiggin.<GameName>`,
- gives the game a new product GUID,
- optionally points the version text at the game's website.

Without the Editor window:

```bash
unity run . --no-tail -- -executeMethod UnityTemplate.EditorTools.NewGameSetup.ApplyFromCommandLine -gameName "My Game"
```

Add `-websiteUrl <url>` to set the link. To use Remote Config, also link the game to its own Unity Cloud project (see below).

### Device builds

- Player builds build the Addressables content first (**Build Addressables on Player Build** is on in the Addressables settings), so a plain build or `unity build` ships fresh bundles with no manual step.
- A **Development Build** turns on the cheat menu (bottom-right **CHEATS** button) and verbose `GameLog` messages. Release builds remove both.
- iOS: set your signing team in **Player Settings > iOS**. Android: set a keystore in **Player Settings > Android > Publishing Settings**.

### Browser preview (WebGL)

iOS and Android are the release targets; WebGL is for quick previews in a browser. Build with **File > Build Profiles > WebGL > Build**, or headlessly:

```bash
unity build . --profile WebGL -o Builds/WebGL
```

- The profile is `Assets/Settings/Build Profiles/WebGL.asset`: a release build of the boot scene.
- The build is uncompressed (**Compression Format: Disabled**), so any static web server can host it over plain HTTP without `Content-Encoding` headers.
- WebGL uses full exception support (**Enable Exceptions: Full Without Stacktrace**). With "explicitly thrown only", any null reference, even one that code catches, kills the page instead of being handled.
- Addressables load from the page's own server. Unity refuses plain-HTTP downloads unless **Allow downloads over HTTP** permits them (only `localhost` is exempt), so a preview on a LAN address would load without popups or stages. `WebGLHttpDownloads` (Editor) allows HTTP for the length of each WebGL build only; Player Settings keep *Not allowed*, so iOS and Android builds don't gain cleartext or arbitrary-load permissions.
- Remote Config and sign-in fail quietly in the browser when the project is unlinked or offline.

### Other desktop builds

Linux IL2CPP builds need `com.unity.toolchain.linux-x86_64-linux`, which the template leaves out: it adds about 300 MB to every clone's package cache. Add it in Package Manager before building for Linux.

### Unity Gaming Services (Remote Config)

- The template ships **unlinked** from Unity Cloud. Link each game to its own cloud project in **Edit > Project Settings > Services**; until then Remote Config logs a warning and the built-in defaults are used.
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
  Fonts/Inter/              Inter Regular and Semi Bold with their TMP font assets (SIL OFL licence included)
  Graphics/                 Sprites and textures; Shapes/ holds the white shapes the UI is built from (mobile import settings: Tools > Apply Mobile Import Settings)
  Prefabs/                  Pooled objects, menu items, menus and popups
    UserInterface/Kit/      Button.prefab and its PrimaryButton, SecondaryButton, DangerButton and DangerOutlineButton variants; Switch.prefab
    UserInterface/Menus/    Menu.prefab (the canvas set-up every menu shares) and the five menus, all variants of it
    UserInterface/PopUps/   PopUp.prefab (the frame every popup shares) and the confirmation popup, a variant of it
    UserInterface/ConditionalMenus/
                            First-open, daily and anniversary popups (PopUp variants, Addressable)
  Scenes/                   Initialisation.unity (boot scene) and Scenes/StageGroupOne/... (stages)
  Settings/                 StageCatalogue, AchievementCatalogue and Themes/ (light and dark palettes): designer-editable data
  Scripts/
    Runtime/                UnityTemplate assembly, one folder per feature (namespace UnityTemplate.<Folder>)
    Editor/                 UnityTemplate.Editor assembly: Tools menu, play-mode start scene, import settings
    Tests/EditMode/         Service, save and scene-wiring tests
    Tests/PlayMode/         Smoke tests that boot the game and walk the menu loop
  Plugins/DOTween/          Tweening library (vendor code, do not edit)
  AddressableAssetsData/    Addressables groups and settings
  Editor/RemoteConfig/      Remote Config editor data (this path is fixed by the package)
unity-coding-standards/     Git submodule: the coding standards every change follows (outside Assets, so Unity ignores it)
CLAUDE.md                   One-page map for coding agents (points into these docs)
.github/                    Workflows (Unity version tags; template origin tags in games), their scripts, and the Bug and Feature issue forms
```

How the code fits together (boot order, services, saves, popups) is described in [ARCHITECTURE.md](ARCHITECTURE.md).

## Common tasks

| Task | How |
|---|---|
| Add a stage | Create the scene under `Assets/Scenes/...`, mark it Addressable (Scenes group), then drag it into a slot of `Assets/Settings/StageCatalogue.asset`. Empty slots show as locked stages. Append new stages: ratings are stored by position. The stage loads on top of the boot scene, so give its camera no **AudioListener** (the boot scene's stays; a test checks this) and keep its own canvases' sort order below the BACK button's background canvas if BACK must stay tappable. |
| Name stages or unlock them all | In `StageCatalogue.asset`, fill a group's **Names** list (same order as its scenes; blank keeps the `group-stage` label), or tick **All Stages Unlocked** for a collection where nothing is rated. |
| Return from a stage | BACK (the on-screen button, which stays visible in stages, or Android back) calls `StageLoader.ReturnToMainMenu`: it fades out, unloads the stage and shows the main menu. Call it from your own stage UI, such as a pause or game-over screen, the same way. |
| Add a menu | Right-click `Prefabs/UserInterface/Menus/Menu.prefab` > **Create > Prefab Variant**, build the menu inside it and add a script that derives from `MenuBase` to its root. Put an instance under `UserInterface` in `Initialisation.unity`, then give `UserInterfaceManager` a field for it, add it to `allMenus` and initialise it in `Initialise`. |
| Add a popup | Right-click `Prefabs/UserInterface/PopUps/PopUp.prefab` > **Create > Prefab Variant**, move the variant to `Prefabs/UserInterface/ConditionalMenus` and build the popup inside `GRP_Content`. Add a script that derives from `PopUpWindow` to its root and set **Pop Up Menu** to `GRP_Content` and **Close Button** to `BTN_Close`. Mark it Addressable (ConditionalMenus group), give `UserInterfaceManager` an `AssetReferenceGameObject` field for it and open it with `OpenAddressablePopUpAsync`, as `ShowStartUpPopUpsAsync` does. |
| Add a button | Drag a variant from `Prefabs/UserInterface/Kit` into the menu or popup (`PrimaryButton` for the screen's main action, `SecondaryButton` for the rest, `DangerButton` or `DangerOutlineButton` for destructive actions), set its label and wire its `Button` in code. |
| Restyle or rebrand the UI | Colours, type and shapes follow one small design system described in [ARCHITECTURE.md](ARCHITECTURE.md#look). Change colours in `Assets/Settings/Themes/LightTheme.asset` and `DarkTheme.asset`; button and switch styles live in the `Kit` prefabs. |
| Change a menu or popup | Open its prefab from `Prefabs/UserInterface/Menus`, `PopUps` or `ConditionalMenus`. What every menu or popup shares (canvas scaling, the popup panel and backdrop) lives in `Menu.prefab` or `PopUp.prefab`. A variant stores only what it changes, shown in bold in the Inspector. |
| Add a service | Write a plain C# class (implement `ISaveable` if it saves), create it and register it in `GameInitialiser.Awake`, then resolve it from MonoBehaviours with `ServiceLocator.Get<T>()` in `OnEnable`. |
| Save a new value | Add a field to `SaveData`, read it in the owning service's `Load` and write it in `Save`. Never rename or retype an existing field; bump `SaveData.CurrentVersion` and migrate if a field's meaning changes. |
| Pool a new object | Add an entry to `PoolKey` (append only) and a pool on the `ObjectPooler` in `Initialisation.unity`, then use `ObjectPooler.Get`/`Release`. |
| Change achievements | Edit `Assets/Settings/AchievementCatalogue.asset`. Switch the feature on with **Achievements Enabled** on `GameInitialiser`. |
| Play a stage quickly | **Tools > Scenes > Play First Stage (Skip Start-Up Popups)**, or `StagePlayCheck.Play(group, stage)` from a script: boots the game, closes the start-up popups, loads the stage, and restores your save when you leave Play Mode. |
| Set the menu links | Fill in **Url** on the `OpenURLButton` of each link (the social and store buttons in `SettingsMenu.prefab`; the version text in `Initialisation.unity`, which New Game Setup can set). A link button with no URL hides itself, except the version text. In WebGL the link opens from the browser's own click, so pop-up blockers allow it, and a notice explains for 5 seconds that browsers may block the new tab (`UserInterfaceManager.ShowNotice` shows any short message the same way). |
| Change the welcome reward | Edit `WelcomeCredits` and `WelcomePremiumCredits` in `EngagementService`. The welcome popup shows on the first launch and again on later launches until CLAIM, X or Android back pays the reward once. |
| Tune features | Select `GlobalGameManagers` in `Initialisation.unity`: `GameInitialiser` exposes achievements, the daily bonus and the target frame rate. |
| Import new sprites or sounds | Put them in `Assets/Graphics` or `Assets/Audio` and run **Tools > Apply Mobile Import Settings**. |

## Coding conventions

The full standards are in [unity-coding-standards/README.md](unity-coding-standards/README.md); the points below are how this project applies them.

- Services are plain C# classes registered in the `ServiceLocator` by `GameInitialiser`; no `static` managers and no `Singleton<T>`.
- One central initialiser: everything that needs set-up is called from `GameInitialiser.Awake`, in order.
- Object pooling for anything created repeatedly; prefabs for anything reused. Menus are variants of `Menu.prefab` and popups of `PopUp.prefab`, so shared set-up is edited once.
- Cache references and precompute at start-up rather than at runtime.
- New text uses Inter (the TMP default) at a size from the type scale, new shapes use the tinted sprites in `Graphics/Shapes`, and new buttons and switches are instances of the `Kit` prefabs.
- Every UI colour comes from the theme: add a `ThemedGraphic` naming its `ThemeColour` (and set the graphic to that token's light colour), or, in a script that sets colours, use a `ThemeColour` field resolved through `ThemeService`. Never hard-code a colour.
- Self-documenting names, a one-line `/// <summary>` on every class and method, few other comments.
- Private by default; `[SerializeField]` only for values designers tweak.
- Namespaces `UnityTemplate.<Folder>`; never name a type after its own namespace.
- `StringBuilder` or TMP `SetText` for text that changes at runtime.
- Prefer `async`/`await` (Unity `Awaitable`) or DOTween to `Update` and coroutines. The one `Update` (in `UserInterfaceManager`: the Android back button and the menu render throttle) exists because the old Input Manager can only be polled.
- Every `async void` entry point catches and logs its exceptions; tweens use `SetLink(gameObject)`.
- Vocabulary: a **stage** is a playable scene; a **level** is an experience level.
- Keep **Reload Domain** on in Enter Play Mode Options: DOTween 1.2 and the Unity Services SDKs keep static state between play sessions.

## Tests

- **Window > General > Test Runner**: EditMode (services, saves, scene wiring, the template's set-up) and PlayMode (boots the game and walks first launch, the daily wheel, the RESET ALL confirmation, the dark mode switch and starting a stage). The PlayMode tests are in the `Smoke` category.
- Command line:
  - `unity test <project> --mode EditMode` takes seconds and loads no game; run it after every change.
  - `unity test <project> --mode PlayMode` boots the full game (about a minute, longer on software rendering); run it before a pull request. It backs up and restores your local save.

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

### Unity version tags

Every commit that changes the editor version in `ProjectSettings/ProjectVersion.txt` is tagged `unity/<version>` (for example `unity/6000.3.28f1`), so `git checkout unity/2022.3.16f1` returns to the project as it was on that editor.

- The **Unity version tags** GitHub Actions workflow (`.github/workflows/unity-version-tags.yml`) adds and pushes the tags whenever `develop` or `main` is pushed with an editor change; fetch them with `git fetch --tags`. The workflow asks for permission to push tags itself, so it needs no repository setting beyond GitHub Actions being enabled (the default).
- To tag by hand (no Actions, another clone, a rebase), run `.githooks/tag-unity-versions`; it scans every local branch and skips tags that already exist. Tags stay local until pushed with `git push --tags`.

No git hooks or `core.hooksPath` setting are needed: overriding `core.hooksPath` would switch off any hooks your machine already installs. If you set it for an earlier version of this template, undo it with `git config --unset core.hooksPath`.

### Template origin tags (games)

Every game made from this template (GitHub's **Use this template**, which `falcon-new-repo` also uses) gets a tag on its first commit naming the template commit it started from, for example `template/2026-11-21-d5efd2e`. Its annotation gives the full commit and how it was matched.

- The **Template origin tag** workflow (`.github/workflows/template-origin-tag.yml`, run by `.github/scripts/tag-template-origin`) adds it the first time a branch is pushed to the game. It finds the template commit with exactly the same files as the game's first commit, or, if that commit was changed, the template's last commit before the game was created. It does nothing in the template itself and nothing once the tag exists; it can also be started from the game's **Actions** tab.
- It needs the template to stay public (the game's workflow reads it without a token), and the game's organisation must let workflows push (`contents: write`).
- To see what changed in the template since a game started: `git log <the template commit>..main` in a clone of the template.
