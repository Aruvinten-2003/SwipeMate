# SwipeMate

SwipeMate is a modern dating web/PWA prototype built with:

- ASP.NET API for authentication, matching, messaging, safety, moderation, and profile endpoints.
- React + Vite frontend for the website and app shell.
- Domain/Application/Infrastructure project separation for clean architecture.

## Project layout

- `src/SwipeMate.Api` - ASP.NET HTTP API and SignalR hub.
- `src/SwipeMate.Application` - commands, queries, DTOs, validators, and interfaces.
- `src/SwipeMate.Domain` - entities, enums, events, and domain exceptions.
- `src/SwipeMate.Infrastructure` - persistence, identity, storage, caching, messaging, and time providers.
- `src/swipemate-web` - React frontend.
- `tests` - unit and integration tests.
- `docs` - architecture, API, and database notes.

## Start locally

From the project root:

```powershell
dotnet run --project .\src\SwipeMate.Api\SwipeMate.Api.csproj --launch-profile http
```

In another terminal:

```powershell
cd .\src\swipemate-web
npm run dev -- --host 127.0.0.1 --port 5173
```

Open `http://127.0.0.1:5173`.

## Useful checks

```powershell
dotnet build .\src\SwipeMate.Api\SwipeMate.Api.csproj --no-restore --verbosity minimal
dotnet test .\tests\SwipeMate.UnitTests\SwipeMate.UnitTests.csproj --no-restore --verbosity minimal
cd .\src\swipemate-web
npm test -- --run src/App.test.tsx src/features/auth/api/authApi.test.ts
npm run build
npm run lint
```

The current API includes demo handlers and demo authentication so the UI can be previewed before database-backed persistence is complete.
