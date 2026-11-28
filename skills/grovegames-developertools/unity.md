## Unity

- The option model is in the `GroveGames.DeveloperTools` namespace; the window, pages and settings are in `GroveGames.DeveloperTools.Unity`.
- `[Header]`, `[Tooltip]`, `[Range]` and `[Min]` from `UnityEngine` are not read. Use the properties of `[DeveloperOption]`.
- Wrap an options class and its registration in `#if GROVEGAMES_DEVELOPERTOOLS`, and add that symbol to Scripting Define Symbols of development builds only. Without it `AddDeveloperTools()` does not exist.
- Options survive code stripping on their own: the editor adds them, `OnValidate()` and parsable types' `TryParse` to a generated `link.xml` before every build. Do not add `[Preserve]` or a `link.xml` for them.
- Register with `builder.AddDeveloperTools()` once in the root installer, then `builder.AddDeveloperOptions<XDeveloperOptions>()` in the installer of the feature.
- A custom page implements `IDeveloperToolsPage<VisualElement>` with UI Toolkit and is added with `builder.AddDeveloperToolsPage<XPage>()`.
- An asmdef that overrides references lists `GroveGames.DeveloperTools.dll` in its precompiled references.
