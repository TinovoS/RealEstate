# RealEstate

Platforma za posredovanje u prodaji nekretnina: prodavac nudi nekretninu, brokeri se nadmeću da je zastupaju, a izabrani broker je prodaje kupcu.

Seminarski rad iz predmeta Razvoj softvera 2.

## Struktura

| Folder | Sadržaj |
|---|---|
| `services/` | 4 mikroservisa (.NET) |
| `gateway/` | API Gateway |
| `shared/` | zajednički ugovori između servisa |
| `frontend/` | Angular SPA |
| `docker/` | docker-compose i konfiguracija okruženja |
| `docs/` | dokumentacija i dijagrami |

## Razvojno okruženje (Dev Container)

Kod se piše i kompajlira u kontejneru, pa niko ne instalira .NET SDK na svoj računar i svi imaju istu verziju.

**Šta zamenjuje:** lokalnu instalaciju .NET 10 SDK-a i C# ekstenzije za VS Code. Oboje se nalazi u kontejneru.

**Šta ne zamenjuje:** Docker Desktop i VS Code, koji moraju biti instalirani na računaru.

Pokretanje:

1. Upaliti Docker Desktop.
2. U VS Code-u instalirati ekstenziju **Dev Containers** (`ms-vscode-remote.remote-containers`).
3. Otvoriti folder repozitorijuma, pritisnuti `F1` i izabrati **Dev Containers: Reopen in Container**.
4. Sačekati da se kontejner napravi. Prvi put se skida image od oko 3 GB.
5. U terminalu VS Code-a proveriti: `dotnet --version` treba da ispiše `10.0.x`.

Povratak na običan prozor: `F1`, pa **Dev Containers: Reopen Folder Locally**. Posle izmene fajla `.devcontainer/devcontainer.json` treba izabrati **Dev Containers: Rebuild Container**.

| Gde | Šta se tu pokreće |
|---|---|
| Terminal u VS Code-u (kontejner) | `dotnet` komande: build, pokretanje servisa, testovi, migracije |
| Terminal na računaru (PowerShell) | `docker compose` komande iz foldera `docker/` |

Folder repozitorijuma je isti na oba mesta, pa se izmene fajlova vide odmah i u kontejneru i na računaru.

Konfiguracija instalira i ekstenziju Claude Code i čuva njena podešavanja u Docker volume-u `realestate-claude-config`. Ko je ne koristi može da je ignoriše.

## Pokretanje okruženja

Ove komande se kucaju na računaru, ne u Dev Containeru:

    cd docker
    copy .env.example .env
    docker compose up -d

| Servis | Adresa |
|---|---|
| PostgreSQL | localhost:5432 (app / app) |
| pgAdmin | http://localhost:5050 (admin@admin.com / admin) |

## Status

Dogovor oko arhitekture je u toku. Predlog je u `docs/`.