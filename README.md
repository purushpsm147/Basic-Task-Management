# RamSoft Task Management System

A comprehensive full-stack task management application built with ASP.NET Core Web API backend and Angular frontend. This system provides a modern Kanban board interface for managing tasks with drag-and-drop functionality, advanced filtering, and real-time updates.

## 🚀 Features

- **Enhanced Kanban Board Interface**: Drag-and-drop task management with customizable columns and modern visual design
- **Advanced Task Management**: Create, read, update, and delete tasks with rich metadata and file attachments
- **Sophisticated Filtering**: Filter tasks by status, search terms, favorites with advanced dialog interface
- **Dynamic Column Management**: Create and manage custom workflow columns with color coding
- **Modern Glass Morphism UI**: Material Design components with enhanced visual appeal featuring glass morphism effects
- **Responsive Design**: Works seamlessly on desktop and mobile devices with dark mode support
- **Real-time Updates**: Instant UI updates for task and column changes
- **Enhanced Visual Effects**: Modern gradients, shadows, and animations throughout the interface
- **File Upload Support**: Drag-and-drop file upload functionality for task attachments
- **Deadline Management**: Visual deadline indicators with color-coded urgency levels

## 🏗️ Architecture

This is a full-stack application consisting of:

### Backend (ASP.NET Core Web API)
- **Framework**: .NET 8.0
- **Database**: Entity Framework Core with SQL Server
- **Architecture**: Clean Architecture with Repository Pattern
- **API Documentation**: RESTful API with comprehensive endpoints
- **Validation**: FluentValidation for robust input validation
- **Testing**: Comprehensive unit tests with xUnit

### Frontend (Angular)
- **Framework**: Angular 19.2.14 (Latest Stable)
- **UI Library**: Angular Material 18.2.14 + CDK
- **Styling**: SCSS with modern design patterns, glass morphism effects
- **State Management**: Service-based state management
- **Drag & Drop**: Angular CDK Drag and Drop
- **Build System**: Angular Application Builder (esbuild)
- **TypeScript**: 5.8.3 with ES2022 target

## 🎨 Recent Improvements & Updates

### Angular 19 Migration (May 2025)
- ✅ **Upgraded to Angular 19.2.14** - Latest stable version with improved performance
- ✅ **TypeScript 5.8.3** - Enhanced type checking and ES2022 features
- ✅ **New Build System** - Migrated to Angular Application Builder (esbuild) for faster builds
- ✅ **Standalone Components** - Updated component architecture for better tree-shaking
- ✅ **Breaking Changes Fixed** - Resolved all Angular Material migration issues (mat-chip-list → mat-chip-set)

### Enhanced Visual Design System
- ✅ **Glass Morphism Effects** - Modern backdrop blur and transparency effects
- ✅ **Purple Gradient Theme** - Consistent color scheme (#667eea to #764ba2) across all components
- ✅ **Advanced Shadows** - Multi-layered box shadows for depth and hierarchy
- ✅ **Smooth Animations** - 0.3s cubic-bezier transitions for all interactive elements
- ✅ **Mobile Responsive** - Enhanced mobile experience with touch-friendly interactions
- ✅ **Dark Mode Support** - Comprehensive dark theme implementation

### Component Enhancements
- 🎯 **Kanban Board**: Enhanced task cards with hover effects and improved deadline indicators
- 🎯 **Filter Dialog**: Glass morphism design with gradient headers and enhanced form fields
- 🎯 **Task Dialog**: Modern file upload with drag-and-drop zone and improved form layout
- 🎯 **Task List**: Advanced visual effects with loading states and empty state designs

### Performance & Build Optimizations
- ⚡ **Bundle Size Optimization** - Updated CSS budgets for enhanced styling (15kB limit)
- ⚡ **Build Performance** - Faster compilation with Angular 19 Application Builder
- ⚡ **Development Experience** - Improved hot reload and development server performance

## 📋 Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18 or higher - required for Angular 19)
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli@19`)
- [SQL Server](https://www.microsoft.com/sql-server) or SQL Server Express
- [Git](https://git-scm.com/)

## 🛠️ Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/RamSoft/RamSoft-Task-Management.git
cd RamSoft-Task-Management
```

### 2. Backend Setup (ASP.NET Core API)

#### Navigate to the API project
```bash
cd RamSoft-Task-Management
```

#### Install dependencies
```bash
dotnet restore
```

#### Update database connection string
Edit `appsettings.json` or `appsettings.Development.json` to configure your SQL Server connection:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RamSoftTaskManagement;Trusted_Connection=true;"
  }
}
```

#### Build and run the API
```bash
dotnet build
dotnet run
```

The API will be available at `https://localhost:7000` and `http://localhost:5000`

### 3. Frontend Setup (Angular)

#### Navigate to the UI project
```bash
cd ../task-management-ui
```

#### Install dependencies
```bash
npm install
```

#### Start the development server
```bash
ng serve
```

The Angular application will be available at `http://localhost:4200`

## 🧪 Running Tests

### Backend Tests
```bash
cd RamSoft-Task-Management
dotnet test
```

### Frontend Tests
```bash
cd task-management-ui
ng test
```

## 🐳 Docker Support

The application includes Docker support for easy deployment:

```bash
docker build -t ramsoft-task-management .
docker run -p 8080:80 ramsoft-task-management
```

## 📚 API Documentation

### Base URL
- Development: `https://localhost:7000/api`
- Production: `https://your-domain.com/api`

### Task Endpoints

#### Get All Tasks
- **URL:** `GET /api/task`
- **Description:** Retrieve all tasks
- **Response:** `200 OK`
- **Response Body:** Array of JiraTask objects

#### Get Task by ID
- **URL:** `GET /api/task/{id}`
- **Description:** Retrieve a specific task by ID
- **Parameters:** 
  - `id` (integer): Task ID
- **Response:** `200 OK` | `404 Not Found`
- **Response Body:** JiraTask object

#### Search Tasks by Status
- **URL:** `GET /api/task/search?status={status}`
- **Description:** Filter tasks by status
- **Parameters:**
  - `status` (string): Task status (ToDo, InProgress, Done, etc.)
- **Response:** `200 OK`
- **Response Body:** Array of filtered JiraTask objects

#### Sort Tasks
- **URL:** `GET /api/task/sort`
- **Description:** Get sorted tasks
- **Query Parameters:**
  - `sortBy` (string): Property to sort by (Name, Deadline, Status)
  - `direction` (string): Sort direction (Asc, Desc)
- **Response:** `200 OK`
- **Response Body:** Array of sorted JiraTask objects

#### Create Task
- **URL:** `POST /api/task`
- **Description:** Create a new task
- **Request Body:** JiraTask object
- **Response:** `201 Created`
- **Response Body:** Created JiraTask object

#### Update Task
- **URL:** `PUT /api/task/{id}`
- **Description:** Update an existing task
- **Parameters:**
  - `id` (integer): Task ID
- **Request Body:** JiraTask object
- **Response:** `200 OK` | `404 Not Found`

#### Delete Task
- **URL:** `DELETE /api/task/{id}`
- **Description:** Delete a task
- **Parameters:**
  - `id` (integer): Task ID
- **Response:** `200 OK` | `404 Not Found`

### Column Endpoints

#### Get All Columns
- **URL:** `GET /api/columns`
- **Description:** Retrieve all kanban columns
- **Response:** `200 OK`
- **Response Body:** Array of Column objects

#### Create Column
- **URL:** `POST /api/columns`
- **Description:** Create a new kanban column
- **Request Body:** Column object
- **Response:** `201 Created`

#### Update Column
- **URL:** `PUT /api/columns/{id}`
- **Description:** Update a kanban column
- **Response:** `200 OK` | `404 Not Found`

#### Delete Column
- **URL:** `DELETE /api/columns/{id}`
- **Description:** Delete a kanban column
- **Response:** `200 OK` | `404 Not Found`

### Data Models

#### JiraTask
```json
{
  "id": 1,
  "name": "Task Name",
  "description": "Task description",
  "status": "ToDo",
  "deadline": "2025-12-31T23:59:59",
  "columnId": 1,
  "isFavorite": false,
  "createdDate": "2025-05-28T10:00:00",
  "updatedDate": "2025-05-28T10:00:00"
}
```

#### Column
```json
{
  "id": 1,
  "name": "To Do",
  "color": "#e3f2fd",
  "order": 1,
  "isDefault": true
}
```

## 🛠️ Technologies Used

### Backend
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server** - Primary database
- **FluentValidation** - Input validation
- **xUnit** - Testing framework
- **FluentAssertions** - Assertion library for tests

### Frontend
- **Angular 19.2.14** - Latest frontend framework
- **Angular Material 18.2.14** - UI component library
- **Angular CDK 18.2.14** - Component dev kit for drag & drop
- **TypeScript 5.8.3** - Primary programming language with ES2022 target
- **SCSS** - Styling preprocessor with modern glass morphism effects
- **RxJS 7.8.0** - Reactive programming library
- **Zone.js 0.15.1** - Change detection

### Development Tools
- **Docker** - Containerization
- **Git** - Version control
- **Visual Studio/VS Code** - IDEs

## 📁 Project Structure

```
RamSoft-Task-Management/
├── RamSoft-Task-Management/          # ASP.NET Core Web API
│   ├── Controllers/                  # API Controllers
│   ├── Models/                       # Data models
│   ├── Services/                     # Business logic
│   ├── Infrastructure/               # Database context & migrations
│   ├── Validations/                  # Input validation rules
│   └── Program.cs                    # Application entry point
├── TamSoft-Task-Management-UnitTests/ # Backend unit tests
├── task-management-ui/               # Angular frontend
│   ├── src/app/
│   │   ├── kanban-board/            # Main kanban interface
│   │   ├── task-dialog/             # Task creation/editing
│   │   ├── filter-dialog/           # Advanced filtering
│   │   ├── column-dialog/           # Column management
│   │   ├── models/                  # TypeScript models
│   │   └── services/                # API services
│   └── src/assets/                  # Static assets
└── README.md                        # This file
```

## 🚀 Deployment

### Production Build

#### Backend
```bash
cd RamSoft-Task-Management
dotnet publish -c Release -o ./publish
```

#### Frontend
```bash
cd task-management-ui
ng build --prod
```

### Environment Configuration

Update production settings in:
- `appsettings.Production.json` for API configuration
- `environment.prod.ts` for Angular environment variables

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 Development Guidelines

### Code Standards
- Follow C# coding conventions for backend development
- Use Angular style guide for frontend development
- Implement TypeScript strict mode for better type safety
- Use meaningful commit messages following conventional commits

### Frontend Development
- **Components**: Use standalone: false for non-standalone components in Angular 19
- **Styling**: Follow the established glass morphism design system
- **Material Design**: Use Angular Material 18.2.14 components with proper imports
- **Performance**: Monitor bundle sizes and optimize when necessary
- **Responsive**: Ensure all components work on mobile and desktop

### Testing Requirements
- Write unit tests for new features using Jest/Jasmine
- Maintain minimum 80% code coverage
- Test all API endpoints with integration tests
- Verify mobile responsiveness for UI changes

### Build & Deployment
- Use Angular Application Builder for production builds
- Verify build success with `ng build` before committing
- Update documentation for API changes
- Test in both development and production configurations

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string in appsettings.json
   - Ensure database permissions are correct

2. **CORS Issues**
   - Verify CORS is properly configured in Program.cs
   - Check if frontend URL is in allowed origins

3. **Angular 19 Specific Issues**
   - **Node.js Version**: Ensure Node.js v18+ is installed (required for Angular 19)
   - **Package Installation**: Clear node_modules and reinstall: `rm -rf node_modules && npm install`
   - **Angular Cache**: Clear Angular cache: `ng cache clean`
   - **Build Errors**: If experiencing mat-chip issues, ensure migration to mat-chip-set is complete
   - **TypeScript Errors**: Verify TypeScript 5.8.3 compatibility with your IDE

4. **Build Performance Issues**
   - **Memory**: Increase Node.js memory limit: `node --max-old-space-size=8192`
   - **Bundle Size**: Review angular.json budget settings if CSS warnings appear
   - **Build Speed**: Use `ng build --configuration development` for faster development builds

5. **Material Design Issues**
   - Ensure Angular Material 18.2.14 is compatible with Angular 19
   - Check for deprecated Material components and update accordingly
   - Verify all Material modules are properly imported in app.module.ts

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Purushottam Gupta** - Initial work

## 🙏 Acknowledgments

- Angular team for the excellent framework and regular updates to Angular 19
- Angular Material team for the comprehensive UI component library
- Microsoft for the robust .NET ecosystem and ASP.NET Core
- TypeScript team for the enhanced language features in version 5.8
- Contributors to the open-source libraries used in this project
- Community feedback that helped shape the modern visual design improvements

---

*Last Updated: May 28, 2025 - Angular 19.2.14 Migration & Visual Enhancement Update*
