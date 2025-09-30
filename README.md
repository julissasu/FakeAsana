# Asana Task Manager

A task management application built with .NET that demonstrates full-stack development across multiple platforms (CLI, Desktop/Mobile GUI, and REST API).

## Features

- Create, edit, and delete tasks with priorities and due dates
- Organize tasks into projects
- Track completion status and project progress
- Search and filter tasks
- Multiple interfaces: Command-line, Desktop/Mobile app, and REST API

## Tech Stack

- **Backend**: C#, .NET 9.0, ASP.NET Core
- **Frontend**: .NET MAUI (cross-platform)
- **Architecture**: MVVM pattern, RESTful API
- **Storage**: JSON file-based persistence

## Project Structure
```
Asana/
├── Asana.Library/    # Core business logic and models
├── Asana.CLI/        # Command-line interface
├── Asana.Maui/       # Cross-platform GUI app
└── Asana.API/        # REST API with Swagger docs
```

## Quick Start

**Build the solution:**
```bash
dotnet build Asana.sln
```

**Run CLI:**
```bash
cd Asana.CLI
dotnet run
```

**Run API:**
```bash
cd Asana.API
dotnet run
```

Access Swagger UI at: ```http://localhost:5000/swagger```

Run MAUI App:
Open solution in Visual Studio 2022, set ```Asana.Maui``` as startup project, and run.

## API Endpoints

**Tasks:**
- `GET /api/todos` - Get all tasks
- `POST /api/todos` - Create new task
- `PUT /api/todos/{id}` - Update task
- `DELETE /api/todos/{id}` - Delete task

**Projects:**
- `GET /api/projects` - Get all projects
- `POST /api/projects` - Create new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project
