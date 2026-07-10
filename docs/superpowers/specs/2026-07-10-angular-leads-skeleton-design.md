# Design: Angular-Grundgerüst — Feature „leads" mit Mock-Daten

Datum: 2026-07-10
Status: Genehmigt

## Kontext

Erster Umsetzungsschritt der in `CLAUDE.md` festgelegten Referenzarchitektur
(„Lead-Enrichment-Pipeline"). Laut CLAUDE.md-Abschnitt „Nächste Schritte"
beginnt die Umsetzung mit dem Angular-Frontend-Grundgerüst und Mock-Daten,
bevor Backend und n8n-Workflows entstehen.

## Repo-Setup

- Neues, eigenständiges Git-Repo: `n8n-fullstack-demo`
  (`c:\Users\v.sahin\Documents\Programmieren\Eigene Projekte\n8n-fullstack-demo`)
- `CLAUDE.md` wurde aus dem `CV-Demo`-Repo (der statischen Landingpage) hierher
  verschoben, da sie die Spezifikation dieses Projekts ist, nicht der
  Landingpage. `CV-Demo` bleibt reine Demo-Hub-Landingpage ohne
  Projektspezifikation.

## Ziel dieses Schritts

Lauffähiger Angular-Dev-Server mit einer Lead-Liste (Mock-Daten), die per
Klick zur Detailansicht eines Leads navigiert. Sichtbarer, demofähiger
Fortschritt, auf dem Backend-Anbindung und Tests in Folgeschritten aufsetzen.

## Angular-Workspace & Struktur

- Neuer Angular-Workspace unter `frontend/`
- Standalone Components von Anfang an, kein NgModule-Ballast
- Ordnerstruktur exakt wie in CLAUDE.md vorgegeben:
  ```
  frontend/src/app/
  ├── core/
  ├── shared/
  └── features/
      └── leads/
          ├── data-access/
          ├── feature/
          └── ui/
  ```
- Routing mit Lazy Loading für das `leads`-Feature, auch wenn es aktuell die
  einzige Route ist — das Pattern wird von Anfang an korrekt etabliert

## Feature „leads" (Mock-Daten, kein Backend)

- **data-access/**: `LeadsService`, Signals-basiert (kein RxJS-Subject) mit
  fest verdrahteten Mock-Leads (Fake-Firmennamen, Status, Scoring-Faktoren —
  passend zum Lead-Enrichment-Szenario aus der CLAUDE.md)
- **feature/**: `LeadListComponent` (smart, kennt den Service) — Liste aller
  Leads mit Status und Score
- **feature/**: `LeadDetailComponent` (smart) — Detailansicht eines Leads
  inkl. Platzhalter für Fehlerhistorie (Mock)
- **ui/**: reine Presentational-Komponenten (z. B. `LeadCardComponent`,
  `StatusBadgeComponent`) — ausschließlich Input/Output, kein
  Service-Zugriff
- `OnPush` Change Detection konsequent auf allen Komponenten

## Explizit nicht Teil dieses Schritts

- Kein echter HTTP-Call, kein HTTP-Interceptor
- Keine Typed Reactive Forms (kommen mit dem „Status ändern"-Flow in einem
  späteren Schritt)
- Keine Tests (Jest/Playwright folgen als eigener Schritt, sobald es echtes
  Verhalten zu testen gibt und nicht nur Mock-Rendering)

## Nicht-Ziele / Abgrenzung

Backend (.NET), n8n-Workflows, Datenbankanbindung und Deployment sind
ausdrücklich nicht Teil dieses Schritts — sie folgen gemäß der Reihenfolge in
der CLAUDE.md („Nächste Schritte") als eigene Design-/Plan-Zyklen.
