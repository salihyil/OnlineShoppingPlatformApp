# Online Shopping Platform API

A robust e-commerce platform built with .NET Core 9.0, implementing clean architecture and modern development practices.

## 🚀 Features

### User Management

- User registration and authentication
- JWT-based authorization
- Role-based access control (Admin/User)
- Profile management
- Password change functionality

### Product Management

- CRUD operations for products
- Stock tracking
- Product listing and filtering
- Price management

### Order Management

- Shopping cart functionality
- Order creation and processing
- Order status tracking
- Order history
- Real-time stock validation

## 🏗️ Architecture

The project follows N-Tier Architecture with these main layers:

### WebApi Layer

- REST API endpoints
- Request/Response models
- Authentication/Authorization
- Scalar API documentation
- Exception handling middleware

### Business Layer

- Business logic
- Service implementations
- DTOs (Data Transfer Objects)
- Validation rules
- Business rules

### Data Layer

- Entity Framework Core
- Database operations
- Repository Pattern
- Unit of Work Pattern
- Entity models

## 🛠️ Technologies

- **.NET Core 9.0**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **Scalar/Open API**
- **AutoMapper**

## 🔒 Security Features

- JWT token authentication
- Password hashing
- Role-based authorization
- Input validation
- Exception handling
- XSS protection

## 🏃‍♂️ Getting Started

### Prerequisites

- .NET Core 9.0 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository

```bash
git clone https://github.com/salihyil/OnlineShoppingPlatformApp.git
```

2. Navigate to the project directory

```bash
cd OnlineShoppingPlatformApp
```

3. Update database connection string in `appsettings.json`

4. Run migrations

```bash
dotnet ef database update
```

5. Run the application

```bash
cd OnlineShoppingPlatformApp.WebApi
```

```bash
dotnet run
```

The API will be available at `http://localhost:5267`

## 📝 API Documentation

After running the application, visit `http://localhost:5267/scalar/v1` for detailed API documentation.

### Main Endpoints

#### Authentication

- POST /api/Auth/register
- POST /api/Auth/login

#### Products

- GET /api/Products
- GET /api/Products/{id}
- POST /api/Products
- PUT /api/Products/{id}
- DELETE /api/Products/{id}

#### Orders

- GET /api/Orders/my-orders
- GET /api/Orders/{id}
- POST /api/Orders/create
- PUT /api/Orders/{id}
- PATCH /api/Orders/{id}/status

## 🔍 Design Patterns Used

- Repository Pattern
- Unit of Work Pattern
- Factory Pattern
- Dependency Injection

## ⚡ Performance Considerations

- Implemented N+1 query prevention
- Optimized database queries
- Proper transaction management
- Efficient error handling

## 🤝 Contributing

1. Fork the Project
2. Create your Feature Branch
3. Commit your Changes
4. Push to the Branch
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details
