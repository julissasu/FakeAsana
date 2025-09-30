A task management application built with .NET that demonstrates full-stack development across multiple platforms (CLI, Desktop/Mobile GUI, and REST API).
Features

Create, edit, and delete tasks with priorities and due dates
Organize tasks into projects
Track completion status and project progress
Search and filter tasks
Multiple interfaces: Command-line, Desktop/Mobile app, and REST API

Tech Stack

Backend: C#, .NET 9.0, ASP.NET Core
Frontend: .NET MAUI (cross-platform)
Architecture: MVVM pattern, RESTful API
Storage: JSON file-based persistence

Project Structure
Asana/
├── Asana.Library/    # Core business logic and models
├── Asana.CLI/        # Command-line interface
├── Asana.Maui/       # Cross-platform GUI app
└── Asana.API/        # REST API with Swagger docs
Quick Start
Build the solution:
bashdotnet build Asana.sln
Run CLI:
bashcd Asana.CLI
dotnet run
Run API:
bashcd Asana.API
dotnet run
Access Swagger UI at: http://localhost:5000/swagger
Run MAUI App:
Open solution in Visual Studio 2022, set Asana.Maui as startup project, and run.
API Endpoints

GET/POST/PUT/DELETE /api/todos - Task operations
GET/POST/PUT/DELETE /api/projects - Project operations
GET /api/todos/project/{id} - Get tasks by project

Screenshots
(Add screenshots of your CLI, MAUI app, and Swagger interface here)
