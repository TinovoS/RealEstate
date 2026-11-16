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

- `.github/workflows/build.yml`: za svaki pull request ka `develop` i posle spajanja pokreće `dotnet build` i `dotnet test` nad `RealEstate.slnx`. Provera `build` je obavezna u ruleset-u `develop`: pull request ne može da se spoji dok ne prođe.
- `.github/workflows/zakljucan-main.yml`: svaki pull request ka `main` dobija crvenu proveru, kao podsetnik da se cilja `develop`.
- Workflow ne može da zabrani push; to rade ruleset-i.

## Solution i zajednička podešavanja

- `RealEstate.slnx` nabraja sve .NET projekte. Novi projekat se dodaje sa `dotnet sln RealEstate.slnx add <putanja do .csproj>`, inače ga CI ne gradi.
- `Directory.Build.props` važi za sve projekte u repou: `net10.0`, `Nullable` i `ImplicitUsings`. Ta podešavanja se ne ponavljaju u pojedinačnim `.csproj` fajlovima.
- `.editorconfig` određuje formatiranje: UTF-8, LF, 2 razmaka, a 4 razmaka za C#.

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
