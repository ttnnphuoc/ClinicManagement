# Clinic Management System

A full-stack clinic management application built with .NET Core 8 and React TypeScript.

## Tech Stack

### Backend
- .NET Core 8 Web API
- PostgreSQL Database
- Entity Framework Core
- Clean Architecture (Core, Infrastructure, API)

### Frontend
- React 18 + TypeScript
- Vite
- Ant Design (UI Components)
- Tailwind CSS (Styling)
- React Router (Navigation)
- Axios (HTTP Client)

## Project Structure

```
ClinicManagement/
├── src/
│   ├── ClinicManagement.API/          # Web API Layer
│   │   ├── Controllers/               # API Controllers
│   │   └── Program.cs                 # App configuration
│   │
│   ├── ClinicManagement.Core/         # Domain Layer
│   │   ├── Entities/                  # Domain Models
│   │   └── Interfaces/                # Repository Interfaces
│   │
│   └── ClinicManagement.Infrastructure/ # Infrastructure Layer
│       ├── Data/                      # DbContext
│       ├── Repositories/              # Repository Implementations
│       └── Services/                  # Business Services
│
└── client/                            # React Frontend
    ├── src/
    │   ├── components/                # Reusable components
    │   ├── pages/                     # Page components
    │   ├── layouts/                   # Layout components
    │   ├── services/                  # API services
    │   └── types/                     # TypeScript types
    └── package.json

```

## Features

Based on the MVP requirements:

1. **Login/Logout** - Authentication for clinic owners and staff
2. **Patient Management** - Add, edit, delete, search patients and view treatment history
3. **Appointment Management** - Create, edit, delete appointments with calendar views
4. **Service/Treatment Management** - Manage clinic services and treatments
5. **Revenue/Expense Management** - Track financial transactions and generate reports
6. **Inventory Management** - Basic inventory tracking for supplies and medicine
7. **Staff Management** - Manage staff and access control
8. **Dashboard Reports** - Overview of revenue, customers, services, and inventory

## Getting Started

### Prerequisites
- .NET Core 8 SDK
- Node.js 18+
- PostgreSQL 14+

### Backend Setup

1. Update the connection string in `src/ClinicManagement.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=ClinicManagementDB;Username=your_username;Password=your_password"
}
```

2. Create database migration:
```bash
cd src/ClinicManagement.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../ClinicManagement.API
```

3. Update database:
```bash
dotnet ef database update --startup-project ../ClinicManagement.API
```

4. Run the API:
```bash
cd src/ClinicManagement.API
dotnet run
```

The API will be available at `http://localhost:5000`

### Frontend Setup

1. Install dependencies:
```bash
cd client
npm install
```

2. Run the development server:
```bash
npm run dev
```

The app will be available at `http://localhost:5173`

## API Endpoints

The API documentation is available via Swagger at `http://localhost:5000/swagger` when running in development mode.

## Responsive Design

The frontend is built with mobile-first approach using:
- Ant Design's responsive Grid system
- Tailwind CSS responsive utilities
- Collapsible sidebar for mobile devices
- Responsive tables and forms

## License

MIT