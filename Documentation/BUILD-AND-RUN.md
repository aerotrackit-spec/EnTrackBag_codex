# Build and Run

## API
From the solution directory:

```powershell
dotnet restore
dotnet build
```

Run Identity.Api and EnTrackBag.Api using their launch settings or Visual Studio profiles.

Expected development ports:
- Identity.Api: 5200
- EnTrackBag.Api: 5100

## UI

```powershell
cd UI
npm.cmd i
npm.cmd start
```

Expected UI port: 4200.

The browser should call Identity.Api directly:
`POST http://localhost:5200/api/auth/login`

No Angular proxy is used.

## CORS
Set this in both API appsettings files:

```json
"Frontend": {
  "Origin": "http://localhost:4200"
}
```

## Database
Run `Database/Install-EnTrackBag.sql` only to create EnTrackBag-owned tables. Do not recreate or migrate the existing BLTSMFT operational schema.
