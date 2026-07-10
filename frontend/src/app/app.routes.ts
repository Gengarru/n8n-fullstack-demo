import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'leads',
  },
  {
    path: 'leads',
    loadChildren: () => import('./features/leads/leads.routes').then((m) => m.LEADS_ROUTES),
  },
];
