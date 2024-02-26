# RamSoft Task Management

This is a task management API for RamSoft. It provides endpoints to perform CRUD operations on tasks.

## Getting Started

To get started with the project, follow these steps:

1. Clone the repository: `git clone https://github.com/RamSoft/RamSoft-Task-Management.git`
2. Navigate to the project directory: `cd RamSoft-Task-Management`
3. Install the dependencies: `dotnet restore`
4. Build the project: `dotnet build`
5. Run the project: `dotnet run`

## API Endpoints

### Get all tasks

- **URL:** `/api/task`
- **Method:** GET
- **Response:** 200 OK
- **Response Body:** Array of JiraTask objects

### Sort tasks

- **URL:** `/api/task/sort`
- **Method:** GET
- **Response:** 200 OK
- **Response Body:** Array of sorted JiraTask objects

### Get task by ID

- **URL:** `/api/task/{id}`
- **Method:** GET
- **Response:** 200 OK
- **Response Body:** JiraTask object

### Get tasks by status

- **URL:** `/api/task/search?status={status}`
- **Method:** GET
- **Response:** 200 OK
- **Response Body:** Array of JiraTask objects

### Create task

- **URL:** `/api/task`
- **Method:** POST
- **Request Body:** JiraTask object
- **Response:** 201 Created
- **Response Body:** Created JiraTask object

### Update task

- **URL:** `/api/task/{id}`
- **Method:** PUT
- **Request Body:** JiraTask object
- **Response:** 200 OK

### Delete task

- **URL:** `/api/task/{id}`
- **Method:** DELETE
- **Response:** 200 OK

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- Microsoft SQL Server

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
