# Concert Ticket Management System

A comprehensive .NET 9 Web API for managing concert tickets with Clean Architecture, featuring event management, ticket reservations, sales, and venue capacity management.

## 🎯 Features

### Core Functionality

- **Event Management**: Create, update, and manage concert events
- **Ticket Types**: Multiple pricing tiers per event
- **Reservation System**: 15-minute reservation window with automatic expiry
- **Purchase Flow**: Complete ticket purchasing workflow
- **Capacity Management**: Real-time availability tracking
- **Background Services**: Automatic cleanup of expired reservations

### Technical Features

- **Clean Architecture**: Proper separation of concerns
- **Repository Pattern**: With Unit of Work implementation
- **PostgreSQL Database**: With Entity Framework Core
- **Swagger Documentation**: Comprehensive API documentation
- **Data Seeding**: Pre-populated test data
- **Transaction Support**: Ensures data consistency
- **Comprehensive Logging**: Throughout the application

## 🏗️ Architecture

\`\`\`
src/
├── Domain/ # Core business entities and interfaces
│ ├── Entities/ # Event, TicketType, Ticket
│ ├── Enums/ # TicketStatus
│ └── Interfaces/ # Repository contracts
├── Application/ # Business logic and DTOs
│ ├── DTOs/ # Data transfer objects
│ └── Services/ # Business services
├── Infrastructure/ # Data access and external concerns
│ ├── Data/ # DbContext and seeding
│ ├── Repositories/ # Repository implementations
│ └── Mapping/ # AutoMapper profiles
└── API/ # Web API controllers and configuration
└── Controllers/ # REST API endpoints
\`\`\`

## 🚀 Quick Start Guide for macOS

### Prerequisites

1. **Install .NET 9 SDK**
   \`\`\`bash
   brew install dotnet
   \`\`\`

2. **Install PostgreSQL**
   \`\`\`bash
   brew install postgresql
   brew services start postgresql
   \`\`\`

3. **Install VS Code Extensions**
   - C# Dev Kit
   - C# Extensions
   - REST Client

### Setup Instructions

1. **Clone/Download the Project**
   \`\`\`bash
   mkdir ConcertTicketSystem
   cd ConcertTicketSystem
   \`\`\`

2. **Create the Project Structure**
   \`\`\`bash

   # Create all the directories as shown in the code files above

   mkdir -p src/Domain/Entities src/Domain/Enums src/Domain/Interfaces
   mkdir -p src/Application/DTOs src/Application/Services
   mkdir -p src/Infrastructure/Data src/Infrastructure/Repositories src/Infrastructure/Mapping src/Infrastructure/Extensions
   mkdir -p src/API/Controllers
   \`\`\`

3. **Copy All Code Files**

   - Copy each file from the CodeProject above into the corresponding directory
   - Ensure all file paths match exactly

4. **Set Up Database**
   \`\`\`bash

   # Create database

   createdb ConcertTicketSystem

   # Test connection

   psql -d ConcertTicketSystem -c "SELECT version();"
   \`\`\`

5. **Update Connection String**
   Edit `src/API/appsettings.Development.json`:
   \`\`\`json
   {
   "ConnectionStrings": {
   "DefaultConnection": "Host=localhost;Database=ConcertTicketSystem;Username=postgres;Password=YOUR_PASSWORD"
   }
   }
   \`\`\`

6. **Restore Packages and Run**
   \`\`\`bash
   cd src/API
   dotnet restore
   dotnet run
   \`\`\`

7. **Access the Application**
   - **Swagger UI**: https://localhost:5205
   - **API Base URL**: https://localhost:5205/api

## 📊 Pre-Seeded Test Data

The application automatically seeds the database with realistic test data:

### Events

- **Rock Legends Live** - Madison Square Garden (30 days from now)
- **Jazz Under the Stars** - Central Park (15 days from now)
- **Electronic Dance Festival** - Brooklyn Mirage (45 days from now)
- **Classical Symphony Night** - Lincoln Center (60 days from now)
- **Summer Music Festival 2023** - Past event (sold out)

### Sample Customers

- `john.doe@example.com` - Has purchased tickets
- `jane.smith@example.com` - Has purchased tickets
- `mike.johnson@example.com` - Has a reservation expiring soon
- `alice.johnson@example.com` - Has VIP tickets

### Ticket Types

Each event has multiple ticket types with different pricing:

- General Admission: $45 - $199
- Premium/Reserved Seating: $85 - $149
- VIP Experience: $125 - $449

## 🧪 Testing the API

### 1. View All Events

\`\`\`bash
curl -X GET "https://localhost:5001/api/events" -k
\`\`\`

### 2. Get Ticket Types for an Event

\`\`\`bash
curl -X GET "https://localhost:5001/api/tickettypes/event/{eventId}" -k
\`\`\`

### 3. Check Ticket Availability

\`\`\`bash
curl -X GET "https://localhost:5001/api/tickets/availability/{ticketTypeId}" -k
\`\`\`

### 4. Reserve a Ticket

\`\`\`bash
curl -X POST "https://localhost:5001/api/tickets/reserve" \
 -H "Content-Type: application/json" \
 -d '{
"ticketTypeId": "GUID_FROM_TICKET_TYPE",
"customerEmail": "test@example.com",
"customerName": "Test Customer"
}' -k
\`\`\`

### 5. Purchase Reserved Ticket

\`\`\`bash
curl -X POST "https://localhost:5001/api/tickets/purchase" \
 -H "Content-Type: application/json" \
 -d '{
"ticketId": "GUID_FROM_RESERVED_TICKET",
"paymentTransactionId": "test_payment_123"
}' -k
\`\`\`

### 6. Get Customer Tickets

\`\`\`bash
curl -X GET "https://localhost:5001/api/tickets/customer/john.doe@example.com" -k
\`\`\`

## 🔧 Development in VS Code

### 1. Open Project

\`\`\`bash
code .
\`\`\`

### 2. Configure Launch Settings

Create `.vscode/launch.json`:
\`\`\`json
{
"version": "0.2.0",
"configurations": [
{
"name": ".NET Core Launch (web)",
"type": "coreclr",
"request": "launch",
"preLaunchTask": "build",
"program": "${workspaceFolder}/src/API/bin/Debug/net8.0/ConcertTicketSystem.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/API",
"stopAtEntry": false,
"serverReadyAction": {
"action": "openExternally",
"pattern": "\\bNow listening on:\\s+(https?://\\S+)"
},
"env": {
"ASPNETCORE_ENVIRONMENT": "Development"
}
}
]
}
\`\`\`

### 3. Configure Build Tasks

Create `.vscode/tasks.json`:
\`\`\`json
{
"version": "2.0.0",
"tasks": [
{
"label": "build",
"command": "dotnet",
"type": "process",
"args": [
"build",
"${workspaceFolder}/src/API/ConcertTicketSystem.API.csproj"
],
"problemMatcher": "$msCompile"
}
]
}
\`\`\`

### 4. Debug the Application

- Press `F5` to start debugging
- Set breakpoints by clicking in the gutter
- Use Debug Console to inspect variables

## 🐛 Troubleshooting

### Database Connection Issues

\`\`\`bash

# Check PostgreSQL status

brew services list | grep postgresql

# Start PostgreSQL if not running

brew services start postgresql

# Test connection

psql -h localhost -U postgres -d ConcertTicketSystem
\`\`\`

### Port Already in Use

\`\`\`bash

# Find process using port

lsof -i :5001

# Kill the process

kill -9 <PID>

# Or use different port

dotnet run --urls="https://localhost:7001;http://localhost:5001"
\`\`\`

### Package Restore Issues

\`\`\`bash

# Clear NuGet cache

dotnet nuget locals all --clear

# Restore packages

dotnet restore
\`\`\`

### Reset Database and Re-seed

\`\`\`bash

# Stop the application

# Drop and recreate database

dropdb ConcertTicketSystem
createdb ConcertTicketSystem

# Restart application - data will be automatically seeded

dotnet run
\`\`\`

## 📈 Key Business Logic

### Reservation System

- **15-minute expiry**: Tickets are automatically released if not purchased
- **Real-time capacity**: Prevents overselling through transaction management
- **Background cleanup**: Expired reservations are cleaned up every minute

### Transaction Management

- **ACID compliance**: All operations are wrapped in transactions
- **Rollback support**: Failed operations don't leave partial data
- **Capacity restoration**: Cancelled/expired tickets restore availability

### Payment Integration Ready

- **Transaction ID tracking**: Ready for payment processor integration
- **Purchase confirmation**: Complete workflow from reservation to purchase

## 🚀 Production Considerations

### Environment Configuration

- Use environment variables for sensitive data
- Implement proper logging (Serilog, Application Insights)
- Add health checks and monitoring
- Configure CORS policies appropriately

### Security Enhancements

- Implement JWT authentication
- Add rate limiting
- Input validation and sanitization
- API versioning

### Performance Optimizations

- Database indexing (already implemented)
- Connection pooling (automatic with EF Core)
- Caching strategies (Redis)
- Background job processing

## 📝 API Documentation

The API includes comprehensive Swagger documentation available at the root URL when running in development mode. The documentation includes:

- Detailed endpoint descriptions
- Request/response schemas
- Example payloads
- Error response codes
- Interactive testing interface

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Maintain comprehensive error handling
3. Add appropriate logging
4. Update API documentation
5. Follow C# coding standards

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For questions or issues:

1. Check the troubleshooting section
2. Review the API documentation in Swagger
3. Check application logs for detailed error information

## 🎯 Future Enhancements

- [ ] JWT Authentication and Authorization
- [ ] Real payment gateway integration (Stripe, PayPal)
- [ ] Email notifications for reservations/purchases
- [ ] Event search and filtering capabilities
- [ ] Seat selection for venues with assigned seating
- [ ] Refund processing workflow
- [ ] Analytics and reporting dashboard
- [ ] Mobile app API support
- [ ] Real-time updates with SignalR
- [ ] Caching with Redis
- [ ] Rate limiting and API throttling
- [ ] Comprehensive unit and integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline setup
