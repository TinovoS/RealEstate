# RealEstate

Seminarski rad iz predmeta Razvoj softvera 2: platforma za posredovanje u prodaji nekretnina. Četiri .NET mikroservisa (Identity, Property, Brokerage, Sales), YARP gateway, Angular frontend, PostgreSQL. Pregled strukture je u `README.md`, a predlog arhitekture u `docs/`.

## Jezik i stil

- Dokumentacija i commit poruke su na srpskom (latinica).
- Commit poruke prate oblik `tip: opis` malim slovima, npr. `build: docker okruzenje sa postgres bazom i pgadmin-om`.

## Grane i pull request-ovi

Podrazumevana grana je `develop`. Pravila ispod sprovodi GitHub kroz dva ruleset-a (Settings → Rules → Rulesets), `develop` i `main`, i važe i za admine.

- Nova grana se uvek pravi iz `develop`, a pull request se otvara ka `develop`.
- Nazivi grana prate oblik `tip/opis`, npr. `build/solution`.
- Direktan push na `develop` nije moguć; izmene ulaze samo kroz pull request. Odobrenje drugog člana nije obavezno.
- Grana `main` je za sada zaključana: ni push ni spajanje pull request-a.
- Na `develop` i `main` su zabranjeni force push i brisanje grane, pa se commit-i koji su jednom ušli ne mogu menjati ni brisati. Greška se ispravlja novim commit-om (`git revert`) kroz novi pull request.
- Na radnim granama nema ograničenja: dok pull request nije spojen, commit-i se mogu menjati i slati sa force push-om.
- Ne predlagati zaobilaženje pravila (gašenje ruleset-a, bypass lista). Ako pravilo smeta, to je odluka tima.

## CI

- `.github/workflows/build.yml` se pokreće za svaki pull request ka `develop` i posle spajanja. Ima dva posla nad `RealEstate.slnx`: `build` (`dotnet build`) i `test` (`dotnet test`), koji kreće tek kad `build` uspe.
- U ruleset-u `develop` obavezna je provera `build`: pull request ne može da se spoji dok ona ne prođe. Provera `test` se izvršava, ali još nije obavezna.
- `.github/workflows/zakljucan-main.yml`: svaki pull request ka `main` dobija crvenu proveru, kao podsetnik da se cilja `develop`.
- Workflow ne može da zabrani push; to rade ruleset-i.

## Testovi

- Test projekat stoji pored projekta koji testira i nosi njegovo ime sa nastavkom `.Tests`: `services/<Servis>/<Projekat>.Tests`, npr. `services/Identity/Identity.Api.Tests`. Za gateway je to `gateway/<Projekat>.Tests`.
- Testovi se pišu u xUnit-u. Novi test projekat se pravi sa `dotnet new xunit` i dodaje u `RealEstate.slnx`; CI pokreće samo testove iz solution-a.
- Lokalno se svi testovi pokreću sa `dotnet test RealEstate.slnx`.
- U CI-ju nema baze ni drugih servisa, pa test koji ih traži tamo pada.

## Solution i zajednička podešavanja

- `RealEstate.slnx` nabraja sve .NET projekte. Novi projekat se dodaje sa `dotnet sln RealEstate.slnx add <putanja do .csproj>`, inače ga CI ne gradi.
- `Directory.Build.props` važi za sve projekte u repou: `net10.0`, `Nullable` i `ImplicitUsings`. Ta podešavanja se ne ponavljaju u pojedinačnim `.csproj` fajlovima.
- `.editorconfig` određuje formatiranje: UTF-8, LF, 2 razmaka, a 4 razmaka za C#.
- Microsoft-ovi paketi (`Microsoft.*`) i alat `dotnet-ef` moraju biti iz serije 10.0.x. `dotnet add package` bez `--version` bira seriju 11, koja ne radi sa `net10.0` (greška `NU1202`).

## Baza i migracije

- Iz Dev Containera se Postgres vidi na `host.docker.internal:5432`. Na domaćinu mora biti pokrenut `docker compose up -d`.
- Alat `dotnet-ef` je upisan u `dotnet-tools.json`. Posle kloniranja ili rebuild-a kontejnera instalira se sa `dotnet tool restore`.
- Migracije servisa stoje u `Data/Migrations` unutar projekta i idu u Git. Komande se kucaju iz korena repoa:
  - nova migracija: `dotnet ef migrations add <Ime> --project services/Identity/Identity.Api --output-dir Data/Migrations`
  - izvršavanje nad bazom: `dotnet ef database update --project services/Identity/Identity.Api`
- Migracija koja je već spojena u `develop` se ne menja. Izmena tabela ide kroz novu migraciju.

## Razvojno okruženje

Razvija se u Dev Containeru (`.devcontainer/devcontainer.json`), ne na lokalno instaliranom SDK-u.

- **U kontejneru** (`/workspaces/RealEstate`): `dotnet` komande, build, testovi, debug. Tu je .NET 10 SDK.
- **Na računaru domaćinu**: `docker compose` iz foldera `docker/`. U kontejneru nema Docker CLI-ja.
- Node nije u kontejneru; dodaje se kad počne rad na Angular frontendu.

Ako `dotnet` nije na putanji, sesija radi na domaćinu, a ne u kontejneru. Tada ne predlagati lokalnu instalaciju SDK-a, nego otvaranje repoa u kontejneru.

## Pravila arhitekture

- Svaki servis čita i piše samo svoje tabele. Nema stranih ključeva ni JOIN-ova između servisa.
- Samo Identity pristupa bazi `auth_db`. Ostali servisi prepoznaju korisnika iz JWT-a.
- Klase događaja u `shared/Contracts` su dogovor tima i ne menjaju se bez dogovora.
