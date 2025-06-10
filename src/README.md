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
