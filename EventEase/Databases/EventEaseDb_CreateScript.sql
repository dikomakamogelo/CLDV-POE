-- ============================================================
-- EventEase Venue Booking System
-- Database Creation Script
-- Module: CLDV6211 - Cloud Development A
-- ============================================================

USE master;
GO

-- Create the database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'EventEaseDb')
BEGIN
    CREATE DATABASE EventEaseDb;
END
GO

USE EventEaseDb;
GO

-- ============================================================
-- DROP TABLES (if re-running script)
-- ============================================================
IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID('dbo.Events',   'U') IS NOT NULL DROP TABLE dbo.Events;
IF OBJECT_ID('dbo.Venues',   'U') IS NOT NULL DROP TABLE dbo.Venues;
GO

-- ============================================================
-- TABLE: Venues
-- ============================================================
CREATE TABLE dbo.Venues (
    VenueId    INT IDENTITY(1,1) NOT NULL,
    VenueName  NVARCHAR(100)     NOT NULL,
    Location   NVARCHAR(200)     NOT NULL,
    Capacity   INT               NOT NULL,
    ImageUrl   NVARCHAR(500)     NULL,

    CONSTRAINT PK_Venues        PRIMARY KEY (VenueId),
    CONSTRAINT CHK_Venues_Capacity CHECK (Capacity >= 1)
);
GO

-- ============================================================
-- TABLE: Events
-- ============================================================
CREATE TABLE dbo.Events (
    EventId     INT IDENTITY(1,1) NOT NULL,
    EventName   NVARCHAR(100)     NOT NULL,
    Description NVARCHAR(500)     NOT NULL,
    StartDate   DATETIME2         NOT NULL,
    EndDate     DATETIME2         NOT NULL,
    ImageUrl    NVARCHAR(500)     NULL,
    VenueId     INT               NOT NULL,

    CONSTRAINT PK_Events        PRIMARY KEY (EventId),
    CONSTRAINT FK_Events_Venues FOREIGN KEY (VenueId)
        REFERENCES dbo.Venues(VenueId)
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT CHK_Events_Dates CHECK (EndDate > StartDate)
);
GO

-- ============================================================
-- TABLE: Bookings
-- ============================================================
CREATE TABLE dbo.Bookings (
    BookingId        INT IDENTITY(1,1) NOT NULL,
    BookingReference NVARCHAR(20)      NOT NULL,
    EventId          INT               NOT NULL,
    VenueId          INT               NOT NULL,
    BookingDate      DATETIME2         NOT NULL,
    CustomerName     NVARCHAR(100)     NOT NULL,
    CustomerEmail    NVARCHAR(100)     NOT NULL,

    CONSTRAINT PK_Bookings            PRIMARY KEY (BookingId),
    CONSTRAINT UQ_BookingReference    UNIQUE (BookingReference),
    CONSTRAINT FK_Bookings_Events     FOREIGN KEY (EventId)
        REFERENCES dbo.Events(EventId)
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Bookings_Venues     FOREIGN KEY (VenueId)
        REFERENCES dbo.Venues(VenueId)
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

-- ============================================================
-- INDEXES for performance
-- ============================================================
CREATE INDEX IX_Events_VenueId   ON dbo.Events(VenueId);
CREATE INDEX IX_Bookings_EventId ON dbo.Bookings(EventId);
CREATE INDEX IX_Bookings_VenueId ON dbo.Bookings(VenueId);
GO

-- ============================================================
-- SEED DATA
-- ============================================================
INSERT INTO dbo.Venues (VenueName, Location, Capacity, ImageUrl) VALUES
('The Grand Hall',              '123 Main Street, Johannesburg', 500, 'https://images.unsplash.com/photo-1519167758481-83f550bb49b3?w=800'),
('Riverside Conference Centre', '45 River Road, Cape Town',      200, 'https://images.unsplash.com/photo-1505373877841-8d25f7d46678?w=800'),
('Skyline Rooftop Venue',       '78 High Street, Durban',        150, 'https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?w=800');
GO

INSERT INTO dbo.Events (EventName, Description, StartDate, EndDate, VenueId, ImageUrl) VALUES
('Annual Tech Summit 2026',     'A premier technology conference bringing together industry leaders.', '2026-07-15 09:00', '2026-07-15 17:00', 1, 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800'),
('Corporate Strategy Workshop', 'Full-day workshop for senior management teams.',                     '2026-08-20 08:30', '2026-08-20 16:00', 2, 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=800');
GO

INSERT INTO dbo.Bookings (BookingReference, EventId, VenueId, BookingDate, CustomerName, CustomerEmail) VALUES
('BK-2026-0001', 1, 1, '2026-05-01 10:00', 'Sarah Johnson', 'sarah.johnson@example.com');
GO

PRINT 'EventEaseDb created and seeded successfully.';
GO