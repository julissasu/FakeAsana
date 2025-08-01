# Asana API

This is a RESTful Web API built with ASP.NET Core for managing projects and todos.

## Features

- **Projects Management**: Create, read, update, and delete projects
- **Todos Management**: Create, read, update, and delete todo items
- **Swagger Documentation**: Interactive API documentation at `/swagger`
- **CORS Support**: Configured to allow cross-origin requests

## API Endpoints

### Projects

- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get a specific project
- `POST /api/projects` - Create a new project
- `PUT /api/projects/{id}` - Update an existing project
- `DELETE /api/projects/{id}` - Delete a project

### Todos

- `GET /api/todos` - Get all todos
- `GET /api/todos/{id}` - Get a specific todo
- `GET /api/todos/project/{projectId}` - Get todos for a specific project
- `POST /api/todos` - Create a new todo
- `PUT /api/todos/{id}` - Update an existing todo
- `DELETE /api/todos/{id}` - Delete a todo
- `PATCH /api/todos/{id}/complete` - Toggle completion status

## Running the API

1. Navigate to the API project directory:

   ```bash
   cd Asana.API
   ```

2. Run the application:

   ```bash
   dotnet run
   ```

3. The API will be available at:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:7000
   - Swagger UI: https://localhost:7000/swagger

## Dependencies

- **Microsoft.AspNetCore.OpenApi**: For OpenAPI specification
- **Swashbuckle.AspNetCore**: For Swagger documentation
- **Asana.Library**: Shared models and services

## CORS Configuration

The API is configured with a permissive CORS policy (`AllowAll`) that allows:

- Any origin
- Any HTTP method
- Any headers

⚠️ **Note**: This is suitable for development but should be restricted for production use.
