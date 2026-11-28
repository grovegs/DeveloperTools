---
name: grovegames-developertools
description: Rules for cheats and tuning options shown in the in-app developer tools with GroveGames.DeveloperTools. Use when writing or reviewing a class that implements IDeveloperOptions, a field or method marked with DeveloperOption, or a page added to the developer tools window.
---

# Using GroveGames.DeveloperTools

Rules for game code that uses this package. They hold in every game. The API is in the package's `README.md`.

- A feature's cheats and live tunables go in one `sealed XDeveloperOptions : IDeveloperOptions` class in the feature's folder. `Category` is the feature's name.
- Mark a private field or a private parameterless method with `[DeveloperOption]`. A read-only field, a method with parameters and a field of an unsupported type show up as a problem row instead of working.
- Set the header, tooltip and limits on the attribute itself: `[DeveloperOption(Header = "Jump", Tooltip = "...", Min = 0f, Max = 10f)]`. A header is set once, on the first member of its group. Actions are listed after all fields, so give the first action its own header when it belongs to a different group.
- Give an `int` or `float` both `Min` and `Max` to get a slider. Give it only `Min` or only `Max` to keep it on one side of a value. `Min` must not be greater than `Max`.
- Apply a changed value in a private `OnValidate()` method. It runs after every change from the window.
- An options class calls services. It holds no game state of its own and no logic that the game needs to run.
- Developer options never ship to players: register them only in development builds.

## Unity

- The option model is in the `GroveGames.DeveloperTools` namespace; the window, pages and settings are in `GroveGames.DeveloperTools.Unity`.
- `[Header]`, `[Tooltip]`, `[Range]` and `[Min]` from `UnityEngine` are not read. Use the properties of `[DeveloperOption]`.
- Wrap an options class and its registration in `#if GROVEGAMES_DEVELOPERTOOLS`, and add that symbol to Scripting Define Symbols of development builds only. Without it `AddDeveloperTools()` does not exist.
- Options survive code stripping on their own: the editor adds them, `OnValidate()` and parsable types' `TryParse` to a generated `link.xml` before every build. Do not add `[Preserve]` or a `link.xml` for them.
- Register with `builder.AddDeveloperTools()` once in the root installer, then `builder.AddDeveloperOptions<XDeveloperOptions>()` in the installer of the feature.
- A custom page implements `IDeveloperToolsPage<VisualElement>` with UI Toolkit and is added with `builder.AddDeveloperToolsPage<XPage>()`.
- An asmdef that overrides references lists `GroveGames.DeveloperTools.dll` in its precompiled references.
