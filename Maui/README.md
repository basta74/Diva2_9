# Diva2Maui

Mobilní klient rezervačního systému Diva2.

## Tok aplikace

1. načtení provozovatelů z `GET https://api.diva2.cz/api/v1/tenants`,
2. výběr tenant domény,
3. přihlášení přes `POST {tenant}/api/v1/auth/login`,
4. seznam poboček z `GET {tenant}/api/v1/branches`,
5. seznam lekcí z `GET {tenant}/api/v1/branches/{id}/lessons`.

Přístupový token je uložený v MAUI `SecureStorage` odděleně pro každý tenant.

## Vývoj

Otevřete `Diva2Maui.csproj` ve Visual Studiu 2026 a jako cíl vyberte Windows Machine nebo Android Emulator.

Debug sestavení pro Windows používá `https://localhost:7123/`. Release sestavení používá `https://api.diva2.cz/`. Pokud je katalog na localhostu, klient automaticky použije lokální server také pro přihlášení, pobočky a lekce.
