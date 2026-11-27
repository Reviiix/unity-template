# Agent guide

A one-page map for coding agents. It doesn't repeat the docs; read them for detail.

- [README.md](README.md): set-up, project layout, common tasks, tests, troubleshooting, git workflow.
- [ARCHITECTURE.md](ARCHITECTURE.md): boot order, services, saves, popups, the look (theme colours, type scale, kit prefabs).
- [unity-coding-standards/](unity-coding-standards/README.md) (git submodule): the standards every change follows. Run `git submodule update --init` if it's empty.

## Rules that are easy to miss

- **Vocabulary:** a *stage* is a playable scene; a *level* is an experience level.
- **Boot:** the game only runs from `Assets/Scenes/Initialisation.unity`. Its `GameInitialiser` creates and registers every service, so any other scene played alone fails with `No X is registered`.
- **Colours** come from the theme: a `ThemedGraphic` naming a `ThemeColour`, or a `ThemeColour` field resolved through `ThemeService`. Never hard-code a colour.
- **Accessibility:** every UI text carries `AccessibleText` (`SceneWiringTests` checks), and animations take their length from `AccessibilityService.MotionSeconds` so REDUCE MOTION skips them. Every control needs a spoken label for VoiceOver and TalkBack: its own text, the text just before it in its row, or a `ScreenReaderLabel` (`SceneWiringTests.EveryControlHasASpokenLabel` checks).
- **UI** is built from the `Prefabs/UserInterface/Kit` prefabs, and menus and popups are variants of `Menu.prefab` and `PopUp.prefab`. Never re-parent or rebuild `PopUp.prefab` itself: its variants' overrides point at its objects.
- **Saves:** add `SaveData` fields; never rename or retype one.
- Keep **Reload Domain** on, and don't add static state that only a domain reload clears.
- Don't set `core.hooksPath`. Unity version tags come from a GitHub Actions workflow, or by hand with `.githooks/tag-unity-versions`. A game's `template/<date>-<sha>` tag (added by a workflow on its first push) names the template commit it started from.

## New game from the template

Run New Game Setup once on the `feature/setup` branch. It sets the product name and the Android, iOS and Standalone identifiers (`com.LeonHiggin.<GameName>`), gives the game a new product GUID and optionally sets the version-text link.

```bash
unity run . --no-tail -- -executeMethod UnityTemplate.EditorTools.NewGameSetup.ApplyFromCommandLine -gameName "My Game"
```

Pass `-websiteUrl <url>` to point the version text at a page. With a live Editor, `NewGameSetup.Apply("My Game")` does the same. Humans use **Tools > New Game Setup...**. Linking the game to its own Unity Cloud project (for Remote Config) is a separate, manual step.

## Editing assets headlessly

Edit scenes, prefabs and ScriptableObjects through Editor scripts, never by hand-editing YAML references.

- `EditorSceneManager.NewScene` or `OpenScene` in Single mode unloads unused assets. Anything loaded before the call (a catalogue, a palette) becomes a destroyed object, and `SerializedObject` writes to it silently do nothing. Load assets after creating or opening the scene, then check the saved `.unity` file holds the reference (`{fileID: 11400000, guid: ...}`).

- Prefabs: open them with `PrefabUtility.LoadPrefabContents`, change fields through `SerializedObject`, then `SaveAsPrefabAsset` and `UnloadPrefabContents`. Create a variant by instantiating the base with `PrefabUtility.InstantiatePrefab` and saving it with `SaveAsPrefabAsset`.
- Adding a stage:
  1. Create the scene under `Assets/Scenes/`.
  2. Add it to the Addressables `Scenes` group with `AddressableAssetSettings.CreateOrMoveEntry`.
  3. Append it to `Assets/Settings/StageCatalogue.asset` through `SerializedObject` (optionally with a name in the group's `names`).
  4. Give its camera no `AudioListener`: stages load on top of the boot scene, whose listener stays.
- Adding a menu or popup: follow the README's Common tasks table.
- Saving a scene or prefab drops prefab overrides on RectTransform properties a component drives (a Slider's fill and handle, layout groups, root canvases); `DrivenPropertyOverrideCleaner` does this so an unchanged save leaves the file unchanged. Don't hand-add such overrides back.
- After an edit, run the EditMode tests. `SceneWiringTests` catches unassigned references and wrong prefab parents.

## Checking a stage in Play Mode

On a first boot the welcome popup (on later boots, the daily or anniversary popup) covers the menu, and START waits until it closes. `StagePlayCheck` does the steps in one call, also in a batch-mode Editor:

1. With an Editor open on the project, run `UnityTemplate.EditorTools.StagePlayCheck.Play(group, stage)`, e.g. `Play(0, 0)` for the first stage. Humans use **Tools > Scenes > Play First Stage (Skip Start-Up Popups)**.
2. It enters Play Mode from `Initialisation.unity`, closes the start-up popups and calls `StageLoader.LoadStage`. When the stage shows, it logs `StagePlayCheck: stage loaded: <group>-<stage>`. A missing scene or a 60-second timeout logs an error instead.
3. Capture the screen or check the scene.
4. Exit Play Mode. The local `save.json` is put back as it was before step 1, or deleted if there was none.

To do the steps by hand instead: close each open `PopUpWindow` with `PressBack()` until `UserInterfaceManager.IsShowingStartUpPopUps` is false, then call `ServiceLocator.Get<StageLoader>().LoadStage(group, stage)`. Any Play Mode session writes `save.json` (in `Application.persistentDataPath`), so delete it afterwards if there was none before.

## Test and build

| What | Command | When |
|---|---|---|
| EditMode tests | `unity test . --mode EditMode` | After every change. Takes seconds and loads no game. |
| PlayMode smoke tests | `unity test . --mode PlayMode` | Before a pull request. Boots the real game; slow on software rendering. All are in the `Smoke` category. |
| Browser preview | `unity build . --profile WebGL -o Builds/WebGL` | Uses `Assets/Settings/Build Profiles/WebGL.asset`: release build, uncompressed, so any static HTTP server can host it. |

iOS and Android are the release targets; WebGL is only the preview. `Builds/` is git-ignored.
