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

## Pokretanje okruženja

    cd docker
    copy .env.example .env
    docker compose up -d

| Servis | Adresa |
|---|---|
| PostgreSQL | localhost:5432 (app / app) |
| pgAdmin | http://localhost:5050 (admin@admin.com / admin) |

## Status

Dogovor oko arhitekture je u toku. Predlog je u `docs/`.