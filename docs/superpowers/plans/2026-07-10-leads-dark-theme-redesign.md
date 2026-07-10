# Leads Dark-Theme Redesign Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Re-style the existing Angular leads skeleton (lead-list, lead-detail, lead-card, status-badge) with the dark glassmorphism CRM design system from CLAUDE.md, and add a sidebar app-shell that was missing from the grundgerüst.

**Architecture:** Global SCSS design tokens (CSS custom properties) feed all component styles. A new `core/shell/app-shell` standalone component wraps the router-outlet with a fixed sidebar. Existing components (`lead-list`, `lead-detail`, `lead-card`, `status-badge`) get their SCSS rewritten against the tokens; no TypeScript logic changes except a `kpi` computed signal on `LeadList` and a deterministic avatar-gradient helper for `LeadCard`.

**Tech Stack:** Angular 20 standalone components, SCSS, CSS custom properties, Angular signals (`computed`).

---

## Notes on testing for this plan

Per the approved spec (`docs/superpowers/specs/2026-07-10-leads-dark-theme-redesign-design.md`), this step is explicitly styling-only — no Jest/Playwright tests are introduced here (tests arrive in a later, dedicated step). "Verification" steps below mean: run `npx tsc --noEmit` for type safety, and visually confirm via the running dev server (`npm start`, already used in the previous step) — not automated tests. The one piece of actual logic introduced (the deterministic avatar-gradient index picker) is still verified with a quick manual check since it's pure and trivial; a dedicated unit test for it would be disproportionate to a one-step-only helper that will get proper test coverage in the future Jest step.

---

## Task 1: Design tokens

**Files:**
- Create: `frontend/src/styles/_tokens.scss`
- Modify: `frontend/src/styles.scss`

- [ ] **Step 1: Create the tokens partial**

Create `frontend/src/styles/_tokens.scss` with exactly these CSS custom properties (values copied 1:1 from CLAUDE.md's "Design-System" section / the CRM mockup — do not invent new values):

```scss
:root {
  // Background gradient
  --color-bg-gradient-start: #1a0d35;
  --color-bg-gradient-mid: #0f1117;
  --color-bg-gradient-end: #080a0f;

  // Glass surfaces
  --color-surface: rgba(255, 255, 255, 0.07);
  --color-surface-border: rgba(255, 255, 255, 0.11);
  --color-surface-hover: rgba(255, 255, 255, 0.13);

  // Brand purple
  --color-purple-start: #6d5cff;
  --color-purple-end: #b24cff;
  --color-purple-muted: #8e7cff;
  --color-purple-bg: rgba(141, 124, 255, 0.16);
  --color-purple-border: rgba(141, 124, 255, 0.35);

  // Neon accents
  --color-green: #8fffcb;
  --color-green-bg: rgba(80, 220, 140, 0.16);
  --color-green-border: rgba(80, 220, 140, 0.35);
  --color-amber: #ffc96e;
  --color-amber-bg: rgba(255, 201, 110, 0.16);
  --color-amber-border: rgba(255, 201, 110, 0.35);
  --color-red: #ff6b8a;
  --color-red-bg: rgba(255, 107, 138, 0.16);
  --color-red-border: rgba(255, 107, 138, 0.35);

  // Text
  --color-text-primary: #f5f7fb;
  --color-text-secondary: #aeb6c7;
  --color-text-tertiary: #cdd4e4;

  // Radii
  --radius-sm: 12px;
  --radius-md: 16px;
  --radius-lg: 20px;
  --radius-xl: 24px;
  --radius-pill: 999px;

  // Shadow
  --shadow-card: 0 24px 80px rgba(0, 0, 0, 0.28);

  // Spacing
  --space-lg: 24px;
  --space-xl: 32px;
}
```

- [ ] **Step 2: Import the tokens and set the page background**

Replace the full contents of `frontend/src/styles.scss` with:

```scss
@use './styles/tokens';

* {
  box-sizing: border-box;
}

html,
body {
  margin: 0;
  padding: 0;
  min-height: 100%;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Inter', 'Segoe UI', sans-serif;
  background: radial-gradient(
      circle at top left,
      var(--color-bg-gradient-start) 0%,
      var(--color-bg-gradient-mid) 38%,
      var(--color-bg-gradient-end) 100%
    )
    fixed;
  color: var(--color-text-primary);
}
```

Note: `_tokens.scss` (leading underscore) is a Sass partial — `@use './styles/tokens'` (no underscore, no extension) is the correct import path.

- [ ] **Step 3: Verify it compiles**

Run: `cd frontend && npx tsc --noEmit -p tsconfig.app.json`
Expected: no output (success). This only checks TypeScript, so also run the dev server to confirm SCSS compiles:

Run: `cd frontend && npm start` (background) then check the task output for `Application bundle generation complete` with no SCSS errors.

- [ ] **Step 4: Commit**

```bash
git add frontend/src/styles/_tokens.scss frontend/src/styles.scss
git commit -m "style(frontend): add CRM design tokens and dark page background"
```

---

## Task 2: `core/shell/app-shell` component

**Files:**
- Create: `frontend/src/app/core/shell/app-shell/app-shell.ts`
- Create: `frontend/src/app/core/shell/app-shell/app-shell.html`
- Create: `frontend/src/app/core/shell/app-shell/app-shell.scss`
- Modify: `frontend/src/app/app.ts`
- Modify: `frontend/src/app/app.html`

- [ ] **Step 1: Create the shell component class**

Create `frontend/src/app/core/shell/app-shell/app-shell.ts`:

```typescript
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShell {}
```

- [ ] **Step 2: Create the shell template**

Create `frontend/src/app/core/shell/app-shell/app-shell.html`:

```html
<div class="app-shell">
  <aside class="app-shell__sidebar">
    <div class="app-shell__logo">
      <span class="app-shell__logo-mark">LE</span>
      <span class="app-shell__logo-text">
        Lead Enrichment
        <span>Reference Demo</span>
      </span>
    </div>

    <nav class="app-shell__nav">
      <a
        class="app-shell__nav-link"
        routerLink="/leads"
        routerLinkActive="app-shell__nav-link--active"
      >
        Leads
      </a>
    </nav>
  </aside>

  <main class="app-shell__main">
    <router-outlet />
  </main>
</div>
```

- [ ] **Step 3: Create the shell styles**

Create `frontend/src/app/core/shell/app-shell/app-shell.scss`:

```scss
.app-shell {
  display: flex;
  min-height: 100vh;

  &__sidebar {
    width: 220px;
    flex-shrink: 0;
    background: rgba(8, 10, 15, 0.7);
    border-right: 1px solid var(--color-surface-border);
    backdrop-filter: blur(20px);
    padding: 24px 0;
    height: 100vh;
    position: sticky;
    top: 0;
  }

  &__logo {
    padding: 0 20px 28px;
    display: flex;
    align-items: center;
    gap: 10px;
  }

  &__logo-mark {
    width: 36px;
    height: 36px;
    border-radius: var(--radius-sm);
    background: linear-gradient(135deg, var(--color-purple-start), var(--color-purple-end));
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 12px;
    font-weight: 800;
    color: #fff;
    flex-shrink: 0;
  }

  &__logo-text {
    font-size: 13px;
    font-weight: 600;
    color: var(--color-text-secondary);
    line-height: 1.3;

    span {
      display: block;
      font-size: 11px;
      opacity: 0.6;
    }
  }

  &__nav {
    display: flex;
    flex-direction: column;
    gap: 2px;
    padding: 0 12px;
  }

  &__nav-link {
    display: block;
    padding: 10px 12px;
    border-radius: var(--radius-sm);
    font-size: 13px;
    font-weight: 500;
    color: var(--color-text-secondary);
    text-decoration: none;
    transition: background 0.15s, color 0.15s;

    &:hover {
      background: var(--color-surface-hover);
      color: var(--color-text-primary);
    }

    &--active {
      background: var(--color-purple-bg);
      color: #fff;
    }
  }

  &__main {
    flex: 1;
    min-width: 0;
    overflow-y: auto;
  }
}
```

- [ ] **Step 4: Wire the shell into the app root**

Replace the full contents of `frontend/src/app/app.ts`:

```typescript
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { AppShell } from './core/shell/app-shell/app-shell';

@Component({
  selector: 'app-root',
  imports: [AppShell],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {}
```

Replace the full contents of `frontend/src/app/app.html`:

```html
<app-shell />
```

- [ ] **Step 5: Verify**

Run: `cd frontend && npx tsc --noEmit -p tsconfig.app.json`
Expected: no output (success).

Run the dev server (`npm start`) and open `http://localhost:4200/leads` — confirm the dark sidebar with "Lead Enrichment" logo and a highlighted "Leads" nav link appears on the left, with the lead list rendering (still unstyled/light) in the main area.

- [ ] **Step 6: Commit**

```bash
git add frontend/src/app/core frontend/src/app/app.ts frontend/src/app/app.html
git commit -m "feat(frontend): add dark sidebar app-shell wrapping the router outlet"
```

---

## Task 3: Status-badge color tokens

**Files:**
- Modify: `frontend/src/app/features/leads/ui/status-badge/status-badge.scss`

- [ ] **Step 1: Replace the status-badge styles**

Replace the full contents of `frontend/src/app/features/leads/ui/status-badge/status-badge.scss`:

```scss
// Color-token reuse note: these are the same grau/lila/amber/grün -bg/-border
// token PAIRS used for lead *temperature* badges (hot/warm/cold/new) in the
// CRM mockup. There is no 1:1 status match for this project's pipeline
// states (new/enriched/contacted/qualified) — the mapping below is a fresh
// assignment of the existing color tokens, not a copied 1:1 status mapping.
// new       -> neutral/grey   (--color-surface / --color-text-secondary)
// enriched  -> purple         (--color-purple-bg / --color-purple-border / --color-purple-muted)
// contacted -> amber          (--color-amber-bg / --color-amber-border / --color-amber)
// qualified -> green          (--color-green-bg / --color-green-border / --color-green)
.status-badge {
  display: inline-block;
  padding: 3px 9px;
  border-radius: var(--radius-pill);
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  border: 1px solid transparent;

  &--new {
    background: var(--color-surface);
    border-color: var(--color-surface-border);
    color: var(--color-text-secondary);
  }

  &--enriched {
    background: var(--color-purple-bg);
    border-color: var(--color-purple-border);
    color: var(--color-purple-muted);
  }

  &--contacted {
    background: var(--color-amber-bg);
    border-color: var(--color-amber-border);
    color: var(--color-amber);
  }

  &--qualified {
    background: var(--color-green-bg);
    border-color: var(--color-green-border);
    color: var(--color-green);
  }
}
```

- [ ] **Step 2: Verify**

Run the dev server and check `/leads` — badges now render as small dark pills instead of the previous light-grey/blue/yellow/green pills. (Layout around them is still unstyled until Task 4/5.)

- [ ] **Step 3: Commit**

```bash
git add frontend/src/app/features/leads/ui/status-badge/status-badge.scss
git commit -m "style(leads): recolor status badges with CRM token pairs, documented as a fresh mapping"
```

---

## Task 4: Lead-card — avatar, glass card, gradient score

**Files:**
- Modify: `frontend/src/app/features/leads/ui/lead-card/lead-card.ts`
- Modify: `frontend/src/app/features/leads/ui/lead-card/lead-card.html`
- Modify: `frontend/src/app/features/leads/ui/lead-card/lead-card.scss`

- [ ] **Step 1: Add a deterministic avatar-gradient picker to the component**

Replace the full contents of `frontend/src/app/features/leads/ui/lead-card/lead-card.ts`:

```typescript
import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { Lead } from '../../data-access/lead.model';
import { StatusBadge } from '../status-badge/status-badge';

const AVATAR_GRADIENTS = [
  'linear-gradient(135deg, #5d6bff, #bb5cff)',
  'linear-gradient(135deg, #3b82f6, #8b5cf6)',
  'linear-gradient(135deg, #10b981, #059669)',
  'linear-gradient(135deg, #f59e0b, #d97706)',
  'linear-gradient(135deg, #ec4899, #8b5cf6)',
];

function hashString(value: string): number {
  let hash = 0;
  for (let i = 0; i < value.length; i++) {
    hash = (hash * 31 + value.charCodeAt(i)) >>> 0;
  }
  return hash;
}

@Component({
  selector: 'app-lead-card',
  imports: [StatusBadge],
  templateUrl: './lead-card.html',
  styleUrl: './lead-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadCard {
  readonly lead = input.required<Lead>();
  readonly selected = output<string>();

  readonly initials = computed(() => {
    const words = this.lead().companyName.split(' ').filter(Boolean);
    return words
      .slice(0, 2)
      .map((word) => word[0]?.toUpperCase() ?? '')
      .join('');
  });

  readonly avatarGradient = computed(() => {
    const index = hashString(this.lead().companyName) % AVATAR_GRADIENTS.length;
    return AVATAR_GRADIENTS[index];
  });

  onSelect(): void {
    this.selected.emit(this.lead().id);
  }
}
```

- [ ] **Step 2: Update the template**

Replace the full contents of `frontend/src/app/features/leads/ui/lead-card/lead-card.html`:

```html
<article class="lead-card" (click)="onSelect()">
  <div class="lead-card__avatar" [style.background]="avatarGradient()">{{ initials() }}</div>

  <div class="lead-card__info">
    <h3 class="lead-card__title">{{ lead().companyName }}</h3>
    <div class="lead-card__industry">{{ lead().industry }}</div>
    <app-status-badge [status]="lead().status" />
  </div>

  <div class="lead-card__score-col">
    <div class="lead-card__score">{{ lead().score }}</div>
    <div class="lead-card__score-label">Score</div>
  </div>
</article>
```

- [ ] **Step 3: Update the styles**

Replace the full contents of `frontend/src/app/features/leads/ui/lead-card/lead-card.scss`:

```scss
.lead-card {
  display: grid;
  grid-template-columns: 44px 1fr auto;
  gap: 14px;
  align-items: center;
  padding: 14px 16px;
  background: var(--color-surface);
  border: 1px solid var(--color-surface-border);
  border-radius: var(--radius-lg);
  backdrop-filter: blur(18px);
  cursor: pointer;
  transition: background 0.15s, border-color 0.15s;

  &:hover {
    background: var(--color-surface-hover);
    border-color: var(--color-purple-border);
  }

  &__avatar {
    width: 44px;
    height: 44px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 14px;
    font-weight: 800;
    color: #fff;
    flex-shrink: 0;
  }

  &__info {
    min-width: 0;
  }

  &__title {
    margin: 0 0 2px;
    font-size: 15px;
    font-weight: 700;
    color: var(--color-text-primary);
  }

  &__industry {
    font-size: 12px;
    color: var(--color-text-secondary);
    margin-bottom: 6px;
  }

  &__score-col {
    text-align: center;
    min-width: 48px;
  }

  &__score {
    font-size: 22px;
    font-weight: 800;
    background: linear-gradient(135deg, var(--color-purple-start), var(--color-purple-end));
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }

  &__score-label {
    font-size: 9px;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.06em;
    color: var(--color-text-secondary);
  }
}
```

- [ ] **Step 4: Verify**

Run: `cd frontend && npx tsc --noEmit -p tsconfig.app.json`
Expected: no output (success).

Reload `/leads` in the browser — each card now shows a circular gradient-avatar with initials on the left, a gradient-text score on the right, and the dark glass-card background. Refresh the page a few times and confirm each company's avatar color stays the same across reloads (deterministic, not random).

- [ ] **Step 5: Commit**

```bash
git add frontend/src/app/features/leads/ui/lead-card
git commit -m "style(leads): restyle lead-card as a glass card with deterministic avatar gradient"
```

---

## Task 5: Lead-list — KPI row and glass layout

**Files:**
- Modify: `frontend/src/app/features/leads/feature/lead-list/lead-list.ts`
- Modify: `frontend/src/app/features/leads/feature/lead-list/lead-list.html`
- Modify: `frontend/src/app/features/leads/feature/lead-list/lead-list.scss`

- [ ] **Step 1: Add KPI computed signals**

Replace the full contents of `frontend/src/app/features/leads/feature/lead-list/lead-list.ts`:

```typescript
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LeadsService } from '../../data-access/leads.service';
import { LeadCard } from '../../ui/lead-card/lead-card';

@Component({
  selector: 'app-lead-list',
  imports: [LeadCard],
  templateUrl: './lead-list.html',
  styleUrl: './lead-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadList {
  private readonly leadsService = inject(LeadsService);
  private readonly router = inject(Router);

  readonly leads = this.leadsService.allLeads;

  readonly averageScore = computed(() => {
    const leads = this.leads();
    if (leads.length === 0) {
      return 0;
    }
    const total = leads.reduce((sum, lead) => sum + lead.score, 0);
    return Math.round(total / leads.length);
  });

  readonly newLeadCount = computed(
    () => this.leads().filter((lead) => lead.status === 'new').length,
  );

  onLeadSelected(leadId: string): void {
    this.router.navigate(['/leads', leadId]);
  }
}
```

- [ ] **Step 2: Add the KPI row to the template**

Replace the full contents of `frontend/src/app/features/leads/feature/lead-list/lead-list.html`:

```html
<section class="lead-list">
  <header class="lead-list__header">
    <div class="lead-list__eyebrow">Lead Management</div>
    <h1>Leads</h1>
    <p class="lead-list__count">{{ leads().length }} Leads · Ø Score {{ averageScore() }}</p>
  </header>

  <div class="lead-list__kpi-row">
    <div class="lead-list__kpi">
      <div class="lead-list__kpi-value">{{ leads().length }}</div>
      <div class="lead-list__kpi-label">Leads gesamt</div>
    </div>
    <div class="lead-list__kpi">
      <div class="lead-list__kpi-value lead-list__kpi-value--green">{{ averageScore() }}</div>
      <div class="lead-list__kpi-label">Ø Score</div>
    </div>
    <div class="lead-list__kpi">
      <div class="lead-list__kpi-value lead-list__kpi-value--amber">{{ newLeadCount() }}</div>
      <div class="lead-list__kpi-label">Neu / unangereichert</div>
    </div>
  </div>

  @if (leads().length > 0) {
    <div class="lead-list__grid">
      @for (lead of leads(); track lead.id) {
        <app-lead-card [lead]="lead" (selected)="onLeadSelected($event)" />
      }
    </div>
  } @else {
    <p class="lead-list__empty">
      Aktuell sind keine Leads vorhanden. Neue Leads erscheinen hier, sobald sie
      über die Enrichment-Pipeline eingehen.
    </p>
  }
</section>
```

- [ ] **Step 3: Update the styles**

Replace the full contents of `frontend/src/app/features/leads/feature/lead-list/lead-list.scss`:

```scss
.lead-list {
  max-width: 960px;
  margin: 0 auto;
  padding: var(--space-xl);

  &__header {
    margin-bottom: 20px;
  }

  &__eyebrow {
    font-size: 11px;
    font-weight: 700;
    letter-spacing: 0.1em;
    text-transform: uppercase;
    color: var(--color-purple-muted);
    margin-bottom: 4px;
  }

  h1 {
    font-size: 26px;
    font-weight: 800;
    letter-spacing: -0.02em;
    margin: 0 0 4px;
    color: var(--color-text-primary);
  }

  &__count {
    color: var(--color-text-secondary);
    font-size: 13px;
    margin: 0;
  }

  &__kpi-row {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 12px;
    margin-bottom: var(--space-lg);
  }

  &__kpi {
    background: var(--color-surface);
    border: 1px solid var(--color-surface-border);
    border-radius: var(--radius-lg);
    backdrop-filter: blur(18px);
    padding: 16px 18px;
  }

  &__kpi-value {
    font-size: 24px;
    font-weight: 700;
    line-height: 1;
    margin-bottom: 4px;
    background: linear-gradient(135deg, var(--color-purple-start), var(--color-purple-end));
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;

    &--green {
      background: none;
      -webkit-text-fill-color: var(--color-green);
    }

    &--amber {
      background: none;
      -webkit-text-fill-color: var(--color-amber);
    }
  }

  &__kpi-label {
    font-size: 11px;
    color: var(--color-text-secondary);
  }

  &__grid {
    display: grid;
    gap: 10px;
  }

  &__empty {
    color: var(--color-text-secondary);
    padding: 2rem 0;
    text-align: center;
  }
}
```

- [ ] **Step 4: Verify**

Run: `cd frontend && npx tsc --noEmit -p tsconfig.app.json`
Expected: no output (success).

Reload `/leads` — confirm 3 KPI tiles appear above the card list showing: total lead count, average score (gradient-green number), and count of leads with status `new` (amber number). With the 5 mock leads from `LeadsService`, expect: "5" total, average score `(87+64+52+12+71)/5 = 57.2` rounded to `57`, and `1` new lead (Kupferbach Elektronik KG).

- [ ] **Step 5: Commit**

```bash
git add frontend/src/app/features/leads/feature/lead-list
git commit -m "feat(leads): add KPI row (total/avg score/new count) to lead-list"
```

---

## Task 6: Lead-detail — glass sections and large avatar header

**Files:**
- Modify: `frontend/src/app/features/leads/feature/lead-detail/lead-detail.ts`
- Modify: `frontend/src/app/features/leads/feature/lead-detail/lead-detail.html`
- Modify: `frontend/src/app/features/leads/feature/lead-detail/lead-detail.scss`

- [ ] **Step 1: Add the same initials/avatar-gradient computed signals**

Replace the full contents of `frontend/src/app/features/leads/feature/lead-detail/lead-detail.ts`:

```typescript
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { map } from 'rxjs';
import { LeadsService } from '../../data-access/leads.service';
import { StatusBadge } from '../../ui/status-badge/status-badge';

const AVATAR_GRADIENTS = [
  'linear-gradient(135deg, #5d6bff, #bb5cff)',
  'linear-gradient(135deg, #3b82f6, #8b5cf6)',
  'linear-gradient(135deg, #10b981, #059669)',
  'linear-gradient(135deg, #f59e0b, #d97706)',
  'linear-gradient(135deg, #ec4899, #8b5cf6)',
];

function hashString(value: string): number {
  let hash = 0;
  for (let i = 0; i < value.length; i++) {
    hash = (hash * 31 + value.charCodeAt(i)) >>> 0;
  }
  return hash;
}

@Component({
  selector: 'app-lead-detail',
  imports: [StatusBadge],
  templateUrl: './lead-detail.html',
  styleUrl: './lead-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadDetail {
  private readonly leadsService = inject(LeadsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private readonly leadId = toSignal(
    this.route.paramMap.pipe(map((params) => params.get('id'))),
    { initialValue: null },
  );

  readonly lead = computed(() => {
    const id = this.leadId();
    return id ? this.leadsService.getLeadById(id) : undefined;
  });

  readonly initials = computed(() => {
    const currentLead = this.lead();
    if (!currentLead) {
      return '';
    }
    const words = currentLead.companyName.split(' ').filter(Boolean);
    return words
      .slice(0, 2)
      .map((word) => word[0]?.toUpperCase() ?? '')
      .join('');
  });

  readonly avatarGradient = computed(() => {
    const currentLead = this.lead();
    if (!currentLead) {
      return AVATAR_GRADIENTS[0];
    }
    const index = hashString(currentLead.companyName) % AVATAR_GRADIENTS.length;
    return AVATAR_GRADIENTS[index];
  });

  goBack(): void {
    this.router.navigate(['/leads']);
  }
}
```

Note: the avatar-gradient logic is intentionally duplicated from `lead-card.ts` rather than extracted into a shared helper in this step — extracting it is a reasonable future cleanup once a third consumer appears, but two call sites don't yet justify a shared `core/` or `shared/` utility per YAGNI.

- [ ] **Step 2: Update the template**

Replace the full contents of `frontend/src/app/features/leads/feature/lead-detail/lead-detail.html`:

```html
@if (lead(); as currentLead) {
  <section class="lead-detail">
    <button class="lead-detail__back" type="button" (click)="goBack()">← Zurück zur Liste</button>

    <header class="lead-detail__header">
      <div class="lead-detail__avatar" [style.background]="avatarGradient()">{{ initials() }}</div>
      <div>
        <h1>{{ currentLead.companyName }}</h1>
        <app-status-badge [status]="currentLead.status" />
      </div>
    </header>

    <div class="lead-detail__facts glass-card">
      <div>
        <dt>Branche</dt>
        <dd>{{ currentLead.industry }}</dd>
      </div>
      <div>
        <dt>Score</dt>
        <dd>{{ currentLead.score }}</dd>
      </div>
      <div>
        <dt>Kontakt</dt>
        <dd>{{ currentLead.contactName }} · {{ currentLead.contactEmail }}</dd>
      </div>
      <div>
        <dt>Zuletzt angereichert</dt>
        <dd>{{ currentLead.enrichedAt ?? 'noch nicht angereichert' }}</dd>
      </div>
    </div>

    <section class="lead-detail__error-history glass-card">
      <h2>Fehlerhistorie</h2>
      @if (currentLead.errorHistory.length > 0) {
        <ul>
          @for (entry of currentLead.errorHistory; track entry.occurredAt) {
            <li>
              <span class="lead-detail__error-time">{{ entry.occurredAt }}</span>
              <span>{{ entry.message }}</span>
            </li>
          }
        </ul>
      } @else {
        <p class="lead-detail__error-empty">Keine Fehler für diesen Lead vermerkt.</p>
      }
    </section>
  </section>
} @else {
  <section class="lead-detail lead-detail--not-found">
    <p>Lead wurde nicht gefunden.</p>
    <button class="lead-detail__back" type="button" (click)="goBack()">← Zurück zur Liste</button>
  </section>
}
```

- [ ] **Step 3: Update the styles**

Replace the full contents of `frontend/src/app/features/leads/feature/lead-detail/lead-detail.scss`:

```scss
.lead-detail {
  max-width: 720px;
  margin: 0 auto;
  padding: var(--space-xl);

  &__back {
    background: none;
    border: none;
    color: var(--color-purple-muted);
    cursor: pointer;
    font-size: 13px;
    padding: 0;
    margin-bottom: 20px;
  }

  &__header {
    display: flex;
    align-items: center;
    gap: 16px;
    margin-bottom: 20px;

    h1 {
      font-size: 22px;
      font-weight: 800;
      margin: 0 0 6px;
      color: var(--color-text-primary);
    }
  }

  &__avatar {
    width: 56px;
    height: 56px;
    border-radius: var(--radius-md);
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 18px;
    font-weight: 800;
    color: #fff;
    flex-shrink: 0;
  }

  .glass-card {
    background: var(--color-surface);
    border: 1px solid var(--color-surface-border);
    border-radius: var(--radius-xl);
    backdrop-filter: blur(18px);
    box-shadow: var(--shadow-card);
    padding: 20px 22px;
    margin-bottom: 16px;
  }

  &__facts {
    display: grid;
    gap: 14px;
    grid-template-columns: repeat(2, minmax(0, 1fr));

    dt {
      font-size: 11px;
      text-transform: uppercase;
      color: var(--color-text-secondary);
      letter-spacing: 0.05em;
      margin: 0 0 2px;
    }

    dd {
      margin: 0;
      font-size: 14px;
      color: var(--color-text-primary);
    }
  }

  &__error-history {
    h2 {
      font-size: 15px;
      font-weight: 700;
      margin: 0 0 14px;
      color: var(--color-text-primary);
    }

    ul {
      list-style: none;
      margin: 0;
      padding: 0;
      display: grid;
      gap: 8px;
    }

    li {
      display: flex;
      gap: 12px;
      padding: 8px 12px;
      background: var(--color-red-bg);
      border: 1px solid var(--color-red-border);
      border-radius: var(--radius-md);
      font-size: 13px;
      color: var(--color-text-primary);
    }
  }

  &__error-time {
    color: var(--color-text-secondary);
    font-variant-numeric: tabular-nums;
  }

  &__error-empty {
    color: var(--color-text-secondary);
    margin: 0;
  }
}
```

- [ ] **Step 4: Verify**

Run: `cd frontend && npx tsc --noEmit -p tsconfig.app.json`
Expected: no output (success).

Navigate to `/leads`, click a lead card, and confirm the detail page shows a large square avatar with initials, the company name, status badge, two glass-card sections (facts grid, error history), and that "← Zurück zur Liste" navigates back to `/leads`.

- [ ] **Step 5: Commit**

```bash
git add frontend/src/app/features/leads/feature/lead-detail
git commit -m "style(leads): restyle lead-detail with glass sections and large avatar header"
```

---

## Task 7: Final full-app visual pass

**Files:** none (verification only)

- [ ] **Step 1: Full dev-server walkthrough**

Run: `cd frontend && npm start` (background, or reuse an already-running instance)

Open `http://localhost:4200/leads` and confirm:
- Dark radial-gradient background fills the whole viewport (no white gaps)
- Sidebar shows logo + "Leads" nav item highlighted in purple
- KPI row shows 3 tiles with correct numbers (5 total / 57 avg / 1 new)
- All 5 lead cards render as dark glass cards with colored avatar circles and pill status badges
- Clicking a card navigates to `/leads/:id` and shows the glass-card detail layout
- "← Zurück zur Liste" returns to the list

- [ ] **Step 2: Type-check the whole app one more time**

Run: `cd frontend && npx tsc --noEmit -p tsconfig.app.json`
Expected: no output (success).

- [ ] **Step 3: Note the open favicon item**

No action needed in this plan — per the spec's "Explizit offen" section, the favicon swap is intentionally deferred until an actual favicon asset is provided. Do not create or guess a favicon file in this task.
