# BarberBooking

BarberBooking is a full-stack appointment booking system organized as a single repository.

## Stack

- Backend: .NET 8, Clean Architecture, DDD, EF Core, MySQL, Kafka
- Web: React, Vite, Tailwind, React Query
- Mobile: Flutter

## Repository Layout

- `API/` - ASP.NET Core API
- `Application/` - application layer
- `Domain/` - domain model
- `Infrastructure/` - persistence, messaging, AI integrations
- `BarberBooking.Contracts/` - shared contracts
- `NotificationService/` - background notification service
- `apps/web/` - React frontend
- `apps/mobile/` - Flutter mobile app

## Local Setup

### Backend

1. Create your own local `appsettings.Development.json` files for `API/` and `NotificationService/`.
2. Add your local MySQL connection strings and any AI provider keys there.
3. Run the API and notification service from the solution:

```powershell
dotnet restore .\BarberBooking.sln
dotnet build .\BarberBooking.sln
```

### Web

```powershell
npm --prefix .\apps\web install
npm --prefix .\apps\web run dev
```

### Mobile

```powershell
flutter pub get --directory .\apps\mobile
flutter analyze .\apps\mobile
```

## Notes

- Secret-bearing local config files are intentionally ignored.
- The committed `appsettings.json` files contain safe placeholder values only.
- Remote legacy history was merged and preserved, while the active project layout remains at the repository root.
