## Unity

- The option model is in the `GroveGames.DeveloperTools` namespace; the window, pages and settings are in `GroveGames.DeveloperTools.Unity`.
- `[Header]`, `[Tooltip]`, `[Range]` and `[Min]` from `UnityEngine` are not read. Use the properties of `[DeveloperOption]`.
- Wrap an options class and its registration in `#if GROVEGAMES_DEVELOPERTOOLS`, and add that symbol to Scripting Define Symbols of development builds only. Without it `AddDeveloperTools()` does not exist.
- With managed stripping above Minimal, mark `OnValidate()` with `[UnityEngine.Scripting.Preserve]`. A field whose type parses with a `TryParse` from the .NET class libraries, such as `Version`, needs that type kept in a `link.xml`, because those libraries are stripped at every level.
- Register with `builder.AddDeveloperTools()` once in the root installer, then `builder.AddDeveloperOptions<XDeveloperOptions>()` in the installer of the feature.
- A custom page implements `IDeveloperToolsPage<VisualElement>` with UI Toolkit and is added with `builder.AddDeveloperToolsPage<XPage>()`.
- An asmdef that overrides references lists `GroveGames.DeveloperTools.dll` in its precompiled references.
