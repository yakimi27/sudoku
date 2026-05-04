# Sudoku Game - WinUI 3 MVVM Application

A comprehensive Sudoku game implementation using modern C# and WinUI 3, following MVVM architecture with clean separation of concerns.

## Architecture

- **Core Layer**: Pure business logic, no UI dependencies
- **Infrastructure Layer**: Data access and external services
- **Presentation Layer**: WinUI 3 ViewModels and UI controls

## Project Structure

```
src/
├── Sudoku/
│   ├── Core/
│   │   ├── Enums/
│   │   ├── Models/
│   │   ├── Engine/
│   │   ├── Services/
│   │   ├── GameModes/
│   │   └── Commands/
│   ├── Infrastructure/
│   │   ├── Storage/
│   │   └── Services/
│   ├── Presentation/
│   │   ├── ViewModels/
│   │   ├── Views/
│   │   └── Converters/
│   ├── Resources/
│   └── DI/
└── Sudoku.Tests/
```

## Building

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

## Design Principles

- **SOLID**: All five principles strictly followed
- **DRY**: No code duplication
- **KISS**: Simple, maintainable solutions
- **MVVM**: Clean separation of UI and logic
- **Dependency Injection**: Services registered in DI container

## Services

- **IStorageService**: Persistent game storage
- **IStatisticsService**: Player statistics tracking
- **IProfileService**: Player profile management (Singleton)
- **ITimerService**: Game timing functionality (Singleton)
- **IHintService**: Hint generation and management
