# RealEstate

Seminarski rad (Razvoj softvera 2): platforma za posredovanje u prodaji nekretnina. Četiri .NET 10 mikroservisa, YARP gateway, Angular frontend, PostgreSQL. Ljudski pregled je u `README.md`.

## Način rada (obavezno za svaku sesiju)

- Prvo pročitaj ovaj fajl; ne pretražuj ceo repo da bi otkrio strukturu.
- Pre čitanja izvornog koda pokreni `git status` i `git diff` i gledaj samo skorašnje izmene.
- Ceo fajl čitaj samo ako ga menjaš ili ako ovde nema potpisa ili logike koja ti treba.
- Kad dodaš, premestiš ili promeniš odgovornost neke celine, ažuriraj ovaj fajl u istom koraku.

## Mapa repoa

| Putanja | Celina |
|---|---|
| `services/Identity/Identity.Api/` | Identity servis: registracija, prijava, izdavanje JWT-a, profili; jedini pristupa `auth_db`. |
| `services/Property/` | Property servis (još nema koda): nekretnine, slike, cene, statusi `Draft → OpenForBrokers → Represented → Listed → Sold`; `main_db`, tabele `property_*`. |
| `services/Brokerage/` | Brokerage servis (još nema koda): ponude brokera, izbor brokera, ugovor o zastupanju; `main_db`, tabele `brokerage_*`. |
| `services/Sales/` | Sales servis (još nema koda): ponude kupaca, kontraponude, prodaje; `main_db`, tabele `sales_*`. |
| `gateway/` | YARP API Gateway (još nema koda): jedina ulazna tačka za frontend, prosleđuje po prefiksu rute, CORS. |
| `shared/Contracts/` | Zajedničke klase događaja između servisa (još prazno; menja se samo uz dogovor tima). |
| `frontend/` | Angular SPA (još nema koda). |
| `docker/` | `docker-compose.yaml` (Postgres 16 `re-postgres`, pgAdmin `re-pgadmin`), `postgres/init.sql` pravi `auth_db` i `main_db`, `.env.example`. |
| `docs/` | Predlog arhitekture i dogovori tima. |
| `.github/workflows/` | CI (`build.yml`) i podsetnik da je `main` zaključan (`zakljucan-main.yml`). |
| `.devcontainer/` | Dev Container sa .NET 10 SDK-om. |
| `RealEstate.slnx`, `Directory.Build.props`, `.editorconfig`, `dotnet-tools.json` | Solution, zajednička MSBuild podešavanja, formatiranje, lokalni `dotnet-ef`. |

## Identity servis (`services/Identity/Identity.Api/`, port 5001)

- `Program.cs`: ulazna tačka. Redom: `AuthDb` connection string → `AppDbContext` (Npgsql) → `AddIdentityCore<AppUser>` (jedinstven mejl; lozinka ≥ 8, veliko i malo slovo, cifra, bez specijalnog znaka) → `JwtOptions` sa `ValidateOnStart` → `TokenService` (singleton) → `AddJwtBearer` (`MapInboundClaims = false`, `ClockSkew` 30 s, name = `name`, role = `role`) → kontroleri → `AddOpenApi` sa dva transformera iz `Swagger/`. Pri pokretanju upisuje uloge iz `Roles.All` koje fale (zato servis bez baze pada na startu). U Development: `/openapi/v1.json` i Swagger UI na `/swagger` (dugme Authorize, token se pamti posle osvežavanja); u Production oba vraćaju 404.
- `Controllers/AuthController.cs` (`api/auth`): `POST register` → 201 `RegisterResponse` (korisnik i uloga u jednoj transakciji), 400, 409; `POST login` → 200 `LoginResponse`, 401 isti za pogrešan mejl i lozinku.
- `Controllers/UsersController.cs` (`api/users`, `[Authorize]` na klasi): `GET me` → `UserProfileResponse`; `PUT me` (`UpdateProfileRequest`, samo ime i prezime); `GET {id}` → `PublicProfileResponse` (bez mejla). Id prijavljenog korisnika uvek iz claim-a `sub`.
- `Controllers/HealthController.cs` (`api/health`): 200 ili 503 po `Database.CanConnectAsync()`.
- `Auth/JwtOptions.cs`: sekcija `Jwt` (`Issuer`, `Audience`, `Key` ≥ 32 znaka, `ExpiresInMinutes` 1–1440).
- `Auth/TokenService.cs`: `CreateToken(AppUser, roles)` → `LoginResponse`; claims `sub`, `email`, `name`, `role` (`TokenService.RoleClaimType`), HMAC-SHA256.
- `Swagger/BearerSecuritySchemeTransformer.cs`: dodaje šemu `Bearer` (http, JWT) u OpenAPI opis. `Swagger/BearerSecurityRequirementTransformer.cs`: označava katancem operacije sa `[Authorize]` bez `[AllowAnonymous]`.
- `Models/AppUser.cs`: `IdentityUser` + `FirstName`, `LastName`, `CreatedAt`. `Models/Roles.cs`: `Seller`, `Broker`, `Buyer`, `All` (jedino mesto za nazive uloga).
- `Data/AppDbContext.cs`: `IdentityDbContext<AppUser>`. `Data/Migrations/`: `InitialIdentity` (tabele `AspNet*`).
- `Dtos/`: `Register*`, `Login*`, `UserProfileResponse`, `PublicProfileResponse`, `UpdateProfileRequest`; validacija atributima, `[ApiController]` sam vraća 400.
- Greške: `Problem(...)` (`ProblemDetails`) i `ValidationProblem(...)`; svaka akcija ima `[ProducesResponseType]`.
- Konfiguracija: `appsettings.json` (Jwt bez ključa), `appsettings.Development.json` (lokalna baza `app`/`app` i lažni Jwt ključ). Pravi ključ: env `Jwt__Key`. Primeri zahteva: `Identity.Api.http`.

## Tok podataka

- Prijava: `POST /api/auth/login` → `UserManager.CheckPasswordAsync` → `TokenService` → JWT (issuer `identity-service`, audience `realestate`, 60 min).
- Zaštićeni zahtev: `Authorization: Bearer <token>` → `UseAuthentication` proveri potpis, izdavača, publiku i rok → `[Authorize]` → kontroler čita `sub`.
- Ostali servisi ne pitaju Identity: proveravaju isti JWT istim ključem (`Jwt__Key`) i čitaju `sub` i `role`.
- Gateway (plan, port 5000): `/api/auth`, `/api/users`, `/api/brokers` → Identity 5001; `/api/properties` → Property 5002; `/api/broker-offers`, `/api/contracts`, `/api/brokerage` → Brokerage 5003; `/api/bids`, `/api/transactions` → Sales 5004. Gateway samo prosleđuje; ne proverava token i nema poslovnu logiku.

## Jezik i stil

- Dokumentacija i commit poruke su na srpskom (latinica).
- Commit poruke: `tip: opis` malim slovima, npr. `build: docker okruzenje sa postgres bazom i pgadmin-om`.

## Grane i pull request-ovi

Podrazumevana grana je `develop`. Pravila sprovode GitHub ruleset-i `develop` i `main` i važe i za admine.

- Nova grana uvek iz `develop`, pull request ka `develop`. Naziv `tip/opis`, npr. `build/solution`.
- Direktan push na `develop` nije moguć; samo pull request. Odobrenje drugog člana nije obavezno.
- `main` je zaključan: ni push ni spajanje pull request-a.
- Na `develop` i `main` zabranjeni su force push i brisanje; greška se ispravlja `git revert`-om kroz novi pull request.
- Na radnim granama nema ograničenja dok pull request nije spojen.
- Ne predlagati zaobilaženje pravila (gašenje ruleset-a, bypass lista); to je odluka tima.

## CI

- `.github/workflows/build.yml`: za pull request ka `develop` i posle spajanja; posao `build` (`dotnet build RealEstate.slnx`), pa `test` (`dotnet test RealEstate.slnx`) tek kad `build` uspe.
- Ruleset `develop` traži prolaz provere `build`; `test` se izvršava, ali još nije obavezan.
- `zakljucan-main.yml`: svaki pull request ka `main` dobija crvenu proveru.
- Workflow ne može da zabrani push; to rade ruleset-i.

## Testovi

- Test projekat stoji pored testiranog i nosi ime sa `.Tests`: `services/<Servis>/<Projekat>.Tests`, za gateway `gateway/<Projekat>.Tests`.
- xUnit; novi projekat sa `dotnet new xunit`, pa u `RealEstate.slnx` (CI pokreće samo testove iz solution-a).
- Lokalno: `dotnet test RealEstate.slnx`.
- U CI-ju nema baze ni drugih servisa, pa test koji ih traži tamo pada. Identity trenutno nema testova.

## Solution i zajednička podešavanja

- Novi projekat: `dotnet sln RealEstate.slnx add <putanja do .csproj>`, inače ga CI ne gradi.
- `Directory.Build.props` (`net10.0`, `Nullable`, `ImplicitUsings`) važi za sve projekte; ne ponavljati u `.csproj`.
- `.editorconfig`: UTF-8, LF, 2 razmaka, za C# 4.
- `Microsoft.*` paketi i `dotnet-ef` moraju biti 10.0.x. `dotnet add package` bez `--version` bira seriju 11 (greška `NU1202`).

## Baza i migracije

- Iz Dev Containera Postgres je na `host.docker.internal:5432` (`app`/`app`); na domaćinu mora raditi `docker compose up -d` iz `docker/`.
- `dotnet-ef` iz `dotnet-tools.json`; posle kloniranja ili rebuild-a: `dotnet tool restore`.
- Migracije su u `Data/Migrations` projekta i idu u Git. Iz korena repoa:
  - nova: `dotnet ef migrations add <Ime> --project services/Identity/Identity.Api --output-dir Data/Migrations`
  - izvršavanje: `dotnet ef database update --project services/Identity/Identity.Api`
- Migracija spojena u `develop` se ne menja; izmena tabela ide kroz novu migraciju.

## Razvojno okruženje

- Razvija se u Dev Containeru (`.devcontainer/devcontainer.json`), ne na lokalnom SDK-u.
- U kontejneru (`/workspaces/RealEstate`): `dotnet` komande, build, testovi, debug.
- Na računaru domaćinu: `docker compose` iz `docker/`. U kontejneru nema Docker CLI-ja ni Node-a (Node se dodaje kad počne Angular).
- Servis se pokreće iz svog foldera (`dotnet run` ili `dotnet bin/Debug/net10.0/<Projekat>.dll`); iz korena repoa ne nalazi `appsettings.Development.json`.
- Ako `dotnet` nije na putanji, sesija radi na domaćinu: ne predlagati instalaciju SDK-a, nego otvaranje repoa u kontejneru.

## Pravila arhitekture

- Svaki servis čita i piše samo svoje tabele. Nema stranih ključeva ni JOIN-ova između servisa.
- Samo Identity pristupa bazi `auth_db`. Ostali servisi prepoznaju korisnika iz JWT-a.
- Klase događaja u `shared/Contracts` su dogovor tima i ne menjaju se bez dogovora.
