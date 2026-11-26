# Identity Service

Registracija, prijava i izdavanje JWT tokena. Uloge: Seller, Broker, Buyer. Profili brokera.

Baza: `auth_db`

## Swagger UI

U Development okruženju Swagger UI je na `/swagger`. Token iz odgovora `POST /api/auth/login` (polje `accessToken`) unosi se kroz dugme **Authorize**, bez prefiksa `Bearer`. Šalje se samo uz operacije sa `[Authorize]` i ostaje zapamćen posle osvežavanja stranice, dok se ne klikne **Logout**.