# Room Booking Backend (MVP)

ASP.NET Core Web API for managing room availability and room bookings in a campus environment.

This backend provides RESTful APIs to manage rooms, handle booking schedules, enforce booking rules, and ensure data consistency using soft delete and validation mechanisms.

---

## Tech Stack
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI

---

## How to Run

### Prerequisites
- .NET SDK 8+
- SQL Server (Express / LocalDB)

### Steps
``bash
git clone https://github.com/rizalmaulanaairlangga/2026-CampusRoomRental-backend.git
cd 2026-CampusRoomRental-backend/Backend
dotnet run


## API Access

### Health Check
GET /health

### Swagger UI
http://localhost:{port}/swagger


## Available Endpoints

### Room
GET    /rooms  
POST   /rooms  
GET    /rooms/{id}  
PUT    /rooms/{id}  
DELETE /rooms/{id} (soft delete)

### Booking
GET    /bookings  
POST   /bookings  
GET    /bookings/{id}  
PUT    /bookings/{id}  
DELETE /bookings/{id} (soft delete)


## Business Rules

- Booking times must not overlap for the same room.
- All delete operations use soft delete.
- Validation errors return HTTP 400 responses.
- Booking conflicts return HTTP 409 responses.
- Internal errors are handled globally via middleware.


## Documentation

Additional documentation is available in the `docs/` folder:
- `database-schema.md` — Database table structure
- `erd.md` — Entity Relationship Diagram


## Notes

This project is designed as a backend-first MVP and can be integrated with a React web frontend and a Flutter mobile application.
