# RealEstate

Seminarski rad iz predmeta Razvoj softvera 2: platforma za posredovanje u prodaji nekretnina. Četiri .NET mikroservisa (Identity, Property, Brokerage, Sales), YARP gateway, Angular frontend, PostgreSQL. Pregled strukture je u `README.md`, a predlog arhitekture u `docs/`.

## Jezik i stil

- Dokumentacija i commit poruke su na srpskom (latinica).
- Commit poruke prate oblik `tip: opis` malim slovima, npr. `build: docker okruzenje sa postgres bazom i pgadmin-om`.

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
