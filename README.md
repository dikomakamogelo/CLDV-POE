
EventEase – Venue Booking System
--- CLDV6211 Cloud Development A | Part 1 Submission

---

Project Overview
EventEase is an ASP.NET Core MVC web application for managing venue bookings. This is Part 1 of a 3-part project, covering local development with SQL LocalDB.

---

Technology Stack
- **Framework:** ASP.NET Core MVC (.NET 10)
- **Database:** SQL Server LocalDB (local development)
- **ORM:** Entity Framework Core (Code-First)
- **UI:** Bootstrap 5 + Bootstrap Icons

---

Getting Started

Prerequisites
- Visual Studio 2022
- .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio)

---

Entity-Relationship Diagram (ERD)

```
┌─────────────────────────┐         ┌──────────────────────────────┐
│         VENUES           │         │            EVENTS            │
│─────────────────────────│         │──────────────────────────────│
│ PK  VenueId   INT        │◄────────│ PK  EventId    INT           │
│     VenueName NVARCHAR   │  1   *  │     EventName  NVARCHAR      │
│     Location  NVARCHAR   │         │     Description NVARCHAR     │
│     Capacity  INT        │         │     StartDate  DATETIME2     │
│     ImageUrl  NVARCHAR   │         │     EndDate    DATETIME2     │
└─────────────────────────┘         │     ImageUrl   NVARCHAR      │
             ▲                      │ FK  VenueId    INT           │
             │                      └──────────────────────────────┘
             │ 1                                   ▲
             │                                     │ 1
             │                                     │
             │         ┌──────────────────────────────────────┐
             │         │              BOOKINGS                 │
             │         │──────────────────────────────────────│
             └─────────│ PK  BookingId        INT             │
                    *  │     BookingReference NVARCHAR (UNIQUE)│
                       │ FK  EventId          INT             │
                       │ FK  VenueId          INT             │
                       │     BookingDate      DATETIME2       │
                       │     CustomerName     NVARCHAR        │
                       │     CustomerEmail    NVARCHAR        │
                       └──────────────────────────────────────┘

Relationships:
- One Venue  → Many Events   (1:N)
- One Venue  → Many Bookings (1:N)
- One Event  → Many Bookings (1:N)

Constraints:
- FK_Events_Venues   : Events.VenueId   → Venues.VenueId   (NO ACTION on delete)
- FK_Bookings_Events : Bookings.EventId → Events.EventId   (NO ACTION on delete)
- FK_Bookings_Venues : Bookings.VenueId → Venues.VenueId   (NO ACTION on delete)
- UQ_BookingReference: Bookings.BookingReference is UNIQUE
- CHK_Venues_Capacity: Capacity >= 1
- CHK_Events_Dates   : EndDate > StartDate
```

---

Project Structure

```
EventEase/
├── Controllers/
│   ├── HomeController.cs
│   ├── VenuesController.cs
│   ├── EventsController.cs
│   └── BookingsController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Database/
│   └── EventEaseDb_CreateScript.sql
├── Docs/
│   └── CloudTheory_Part1.md
├── Models/
│   ├── Venue.cs
│   ├── Event.cs
│   ├── Booking.cs
│   └── ErrorViewModel.cs
├── Views/
│   ├── Home/Index.cshtml
│   ├── Venues/ (Index, Create, Edit, Details, Delete)
│   ├── Events/ (Index, Create, Edit, Details, Delete)
│   ├── Bookings/ (Index, Create, Edit, Details, Delete)
│   └── Shared/ (_Layout, Error, _ValidationScriptsPartial)
├── appsettings.json
└── Program.cs
```

---

Key Features (Part 1)
- Full CRUD for Venues, Events, and Bookings
- SQL LocalDB persistence via Entity Framework Core (Code-First)
- Connection string in `appsettings.json`
- Double-booking prevention logic
- Deletion restricted when active bookings/events exist
- Placeholder image URLs for venues and events
- Seed data on first run
- Bootstrap 5 responsive UI

---

GitHub
This repository follows the required version control workflow. Commits are made incrementally as features are developed.
