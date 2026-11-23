# GroveGames.DeveloperTools

[![Build Status](https://github.com/grovegs/DeveloperTools/actions/workflows/release.yml/badge.svg)](https://github.com/grovegs/DeveloperTools/actions/workflows/release.yml)
[![Tests](https://github.com/grovegs/DeveloperTools/actions/workflows/tests.yml/badge.svg)](https://github.com/grovegs/DeveloperTools/actions/workflows/tests.yml)
[![NuGet](https://img.shields.io/nuget/v/GroveGames.DeveloperTools)](https://www.nuget.org/packages/GroveGames.DeveloperTools)
[![Latest Release](https://img.shields.io/github/v/release/grovegs/DeveloperTools)](https://github.com/grovegs/DeveloperTools/releases/latest)

In-app developer tools: an options model read from annotated fields and methods, with a UI Toolkit window for Unity

## Features

- `[DeveloperOption]` turns a field into a toggle, slider, number, text, enum or parsable input, and a method into a button
- `DeveloperOptionsReader` reads a class once and returns its options with label, header, tooltip and limits
- `DeveloperOptionsRegistry` holds the option objects that are currently alive
- `IDeveloperToolsPage<TRoot>` and `DeveloperToolsPageRegistry<TRoot>` hold the pages a tools window shows, in order; the engine picks the root type
- `CornerTapGesture` detects quick taps in the top left corner of the screen
- Unity: a window built with UI Toolkit, an options page, pages that other packages add, and a settings asset

## .NET

### Installation

```bash
dotnet add package GroveGames.DeveloperTools
```

### Usage

```csharp
using GroveGames.DeveloperTools;

public sealed class GameplayDeveloperOptions : IDeveloperOptions
{
    [DeveloperOption(Header = "Cheats", Tooltip = "Ignores all damage.")] private bool _godMode;
    [DeveloperOption(Min = 0f, Max = 10f)] private float _timeScale = 1f;
    [DeveloperOption("Coins", Min = 1f)] private int _amount = 100;

    public string Category => "Gameplay";

    [DeveloperOption]
    private void AddCoins()
    {
    }

    private void OnValidate()
    {
    }
}

var options = DeveloperOptionsReader.Read(typeof(GameplayDeveloperOptions));
```

- A field with both `Min` and `Max` is a slider. `Header` applies to the field it is set on and to every field after it.
- A field of a type with a static `TryParse(string, out T)` is a text input that parses to that type.
- A private `OnValidate()` method runs after every value change.
- Options are read by reflection. In a trimmed or AOT build, keep the marked members.

## Unity

### Installation

1. Install the core package via [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity):
   - Open Window > NuGet > Manage NuGet Packages
   - Search for `GroveGames.DeveloperTools` and install

2. Add the Unity package via Git URL:
   - Open Package Manager (Window > Package Manager)
   - Click "+" > "Add package from git URL"
   - Enter: `https://github.com/grovegs/DeveloperTools.git?path=src/GroveGames.DeveloperTools.Unity/Packages/com.grovegames.developertools`

3. Create a `csc.rsp` file in your `Assets/` directory with the following content to enable C# 10 features:

   ```text
   -langversion:10
   -nullable:enable
   ```

### Usage

With [GroveGames.DependencyInjection](https://github.com/grovegs/DependencyInjection) installed:

```csharp
using GroveGames.DeveloperTools;
using GroveGames.DeveloperTools.Unity;

builder.AddDeveloperTools();
builder.AddDeveloperOptions<GameplayDeveloperOptions>();
builder.AddDeveloperToolsPage<LogsPage>();
```

The window opens with a triple tap in the top left corner or with the toggle key. Both are set in Project Settings > GroveGames > Developer Tools.

## Development

### Project Structure

- `src/GroveGames.DeveloperTools/` - Core .NET library (`net10.0`, `netstandard2.1`)
- `src/GroveGames.DeveloperTools.Unity/` - Unity package
- `tests/` - xUnit tests
- `sandbox/` - Sample applications for testing

### Commands

```bash
dotnet build                        # Build all projects
dotnet format                       # Format code
dotnet test                         # Run tests
dotnet pack -c Release              # Create NuGet packages
```

### Requirements

- .NET 10.0 SDK

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
