# Projekt für Microservices und Frontendanwendung

## Vorraussetzung

- NET 9.0 SDK für den Build
- Für das Frontend Blazor Workload für Visual Studio (Code)/ Rider
- Keine Ausführungsrichtlinie gegen Powershell-Skripts
- Alle Services sollten eine funktionierende Default-appsettings besitzen
    - Falls die Ports 65532 - 65535 lokal bereits belegt sein sollten, dann im Knoten `urls` jeweils einen anderen freien Port angeben

## Starten der Services
- Im Root des Repos das `start.ps1` ausführen
- Nicht verwundern, das Starten aller Dienste dauert rund 20s
- Sobald alles gestartet ist, sollte nun verfügbar sein:
    - Datenbankservice
    - ReadOnlyDataService
    - MutableDataService
    - Blazor-Anwendung

## Anwendung aufrufen
- In den Default-Einstellungen ist die Blazor-Anwendung unter `https://localhost:65532` erreichbar
