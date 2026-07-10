# Design: Dark-Glassmorphism-Redesign für die Lead-Liste/Detailansicht

Datum: 2026-07-10
Status: Genehmigt

## Kontext

Der erste Umsetzungsschritt (`2026-07-10-angular-leads-skeleton-design.md`) hat
das Angular-Grundgerüst mit einem generischen, hellen Bootstrap-Styling
gebaut. Für das öffentliche Referenzprojekt soll das Frontend stattdessen das
Design-System aus dem echten CRM-Produkt des Nutzers (`codeitsolutions-crm`)
übernehmen, damit die Demo optisch klar als Ableger desselben
Anbieters/Produkts erkennbar ist — siehe CLAUDE.md-Abschnitt
„Design-System (1:1 aus dem CRM übernommen)".

Der Nutzer hat dafür ein vollständiges HTML/CSS-Mockup des CRM
(`crm-mockup.html`) bereitgestellt: dunkles Glassmorphism-Theme mit
Radial-Gradient-Hintergrund, Glass-Cards, Lila/Pink-Markenfarbe,
Avatar-Kreisen mit Gradient, Pill-Badges und Gradient-Text für
KPI-/Score-Werte.

## Ziel dieses Schritts

Die bestehenden Komponenten aus dem Grundgerüst (Lead-Liste, Lead-Detail,
Lead-Card, Status-Badge) werden rein visuell auf das CRM-Theme umgestellt.
Keine neue Funktionalität, keine Änderung an Datenmodell oder Routing —
reines Re-Styling plus eine neue Sidebar-Shell, die im Grundgerüst noch
fehlte.

## Scope

### Design-Tokens
- Neue `frontend/src/styles/_tokens.scss` mit den Variablen aus dem
  CLAUDE.md-Abschnitt „Design-System" (Hintergrund-Gradient,
  Glass-Oberflächen, Purple-Gradient, Statusfarben, Typografie, Radii,
  Spacing, Shadow) — als CSS Custom Properties, 1:1 aus dem CRM-Mockup
  übernommen, keine Neuerfindung von Werten.
- Einbindung global über `styles.scss`, damit kein Hex-Wert hartcodiert in
  einzelnen Komponenten verstreut wird.

### Neue Shell-Komponente (`core/`)
- `core/shell/app-shell`: Sidebar (Logo/Produktname + Nav-Eintrag „Leads")
  plus Hauptbereich mit `<router-outlet>`. Ersetzt den aktuellen reinen
  `<router-outlet>` in `app.html`.
- Passt in die von CLAUDE.md vorgegebene `core/`-Ordnerstruktur (App-weite
  Infrastruktur, nicht feature-spezifisch).
- Referenz: Sidebar-Pattern aus dem CRM-Mockup (feste linke Sidebar, Glass-
  Hintergrund mit Blur, aktiver Nav-Link mit `--color-purple-bg`).

### Lead-Liste (`feature/lead-list`)
- KPI-Zeile oberhalb der Karten: „Leads gesamt", „Ø Score",
  „Neu / unangereichert" — als `computed()`-Signals aus den bestehenden
  Leads des `LeadsService` abgeleitet (keine neuen Datenfelder).
- Lead-Card-Liste bleibt strukturell (Grid), erhält aber das Glass-Card-
  Styling.

### Lead-Card (`ui/lead-card`)
- Kreis-Avatar mit Firmen-Initialen; Gradient-Hintergrund deterministisch
  aus dem Firmennamen abgeleitet (z. B. Hash über Zeichencodes → Index in
  eine feste Palette von 4–5 Gradient-Paaren aus dem Mockup), damit die
  Farbe bei jedem Rendern gleich bleibt und nicht zufällig wechselt.
- Score weiterhin sichtbar, als Gradient-Text analog zu den KPI-Werten im
  Mockup.
- Status als Pill-Badge (siehe eigener Abschnitt unten).

### Status-Badge (`ui/status-badge`)
**Wichtiger Klarstellungspunkt (kein Kopierfehler):** Das CRM-Mockup nutzt
Lead-*Temperatur*-Status (`hot` / `warm` / `cold` / `new`), das
Lead-Enrichment-Szenario dieses Projekts hat aber eigene Pipeline-Status
(`new` / `enriched` / `contacted` / `qualified`) — es gibt kein 1:1-Pendant.
Übernommen werden ausschließlich die **Farb-Tokens** (grau/lila/amber/grün
als `-bg`/`-border`-Paare aus dem Mockup), nicht die Status-Namen oder deren
fachliche Bedeutung. Die Zuordnung ist bewusst neu getroffen:

| Status        | Farb-Token (aus CRM-Mockup übernommen)                  |
|----------------|----------------------------------------------------------|
| `new`          | grau/neutral (`--color-surface` / `--color-text-secondary`) |
| `enriched`     | lila (`--color-purple-bg` / `--color-purple-border` / `--color-purple-muted`) |
| `contacted`    | amber (`--color-amber-bg` / `--color-amber-border` / `--color-amber`) |
| `qualified`    | grün (`--color-green-bg` / `--color-green-border` / `--color-green`) |

Diese Tabelle wird als Kommentar im SCSS der Komponente hinterlegt, damit in
einigen Monaten klar bleibt, dass die Zuordnung bewusst neu definiert wurde
und keine fehlerhafte 1:1-Übernahme der `hot/warm/cold/new`-Logik ist.

### Lead-Detail (`feature/lead-detail`)
- Gleiche Glass-Card-Optik für Fakten-Bereich und Fehlerhistorie-Platzhalter.
- Header mit großem Avatar (analog zum Lead-Profil-Header im Mockup) statt
  reinem Text-Titel.
- Fehlerhistorie-Darstellung bleibt funktional unverändert (weiterhin leeres
  Array als Mock-Zustand zulässig).

## Explizit offen (kein Blocker für diesen Schritt)

**Favicon:** Laut CLAUDE.md-Abschnitt „Favicon" noch ungeklärt — im
übergebenen CRM-Mockup ist kein Favicon-Link referenziert, und in diesem
Schritt wurde keine Favicon-Datei bereitgestellt. Das aktuelle
Angular-Default-Favicon (`frontend/public/favicon.ico`) bleibt vorerst
bestehen. Sobald die tatsächliche Favicon-Datei vorliegt, wird sie unter
`frontend/src/favicon.ico` eingebunden und in `index.html` verlinkt — dieser
Schritt bleibt bewusst als offener Punkt für ein späteres Increment stehen
und wird hier nicht mehr adressiert.

## Nicht Teil dieses Schritts

- Keine neuen Features, keine Änderung an `Lead`-Datenmodell oder
  `LeadsService`-Signatur
- Kein HTTP, keine Reactive Forms, keine Tests (weiterhin gemäß
  Grundgerüst-Schritt aufgeschoben)
- Keine weiteren Sidebar-Nav-Einträge über „Leads" hinaus (Backend/n8n-
  Bereiche existieren noch nicht)
