import { Routes } from '@angular/router';

export const LEADS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./feature/lead-list/lead-list').then((m) => m.LeadList),
  },
  {
    path: ':id',
    loadComponent: () => import('./feature/lead-detail/lead-detail').then((m) => m.LeadDetail),
  },
];
