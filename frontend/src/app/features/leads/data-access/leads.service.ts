import { Injectable, computed, signal } from '@angular/core';
import { Lead } from './lead.model';

const MOCK_LEADS: Lead[] = [
  {
    id: 'ld-1001',
    companyName: 'Nordwind Logistik GmbH',
    status: 'qualified',
    score: 87,
    contactName: 'Petra Lindgren',
    contactEmail: 'p.lindgren@nordwind-logistik.example',
    industry: 'Logistik',
    enrichedAt: '2026-06-30T09:12:00Z',
    errorHistory: [],
  },
  {
    id: 'ld-1002',
    companyName: 'Blaufeld Systemtechnik AG',
    status: 'enriched',
    score: 64,
    contactName: 'Markus Vogt',
    contactEmail: 'm.vogt@blaufeld-systemtechnik.example',
    industry: 'Maschinenbau',
    enrichedAt: '2026-07-02T14:45:00Z',
    errorHistory: [],
  },
  {
    id: 'ld-1003',
    companyName: 'Rheinallee Consulting Partners',
    status: 'contacted',
    score: 52,
    contactName: 'Sabine Hoff',
    contactEmail: 's.hoff@rheinallee-consulting.example',
    industry: 'Beratung',
    enrichedAt: '2026-06-28T11:30:00Z',
    errorHistory: [],
  },
  {
    id: 'ld-1004',
    companyName: 'Kupferbach Elektronik KG',
    status: 'new',
    score: 12,
    contactName: 'Jonas Wetter',
    contactEmail: 'j.wetter@kupferbach-elektronik.example',
    industry: 'Elektronik',
    enrichedAt: null,
    errorHistory: [],
  },
  {
    id: 'ld-1005',
    companyName: 'Silberpfad Media Solutions',
    status: 'enriched',
    score: 71,
    contactName: 'Anke Brummer',
    contactEmail: 'a.brummer@silberpfad-media.example',
    industry: 'Medien',
    enrichedAt: '2026-07-05T08:05:00Z',
    errorHistory: [],
  },
];

@Injectable({ providedIn: 'root' })
export class LeadsService {
  private readonly leads = signal<Lead[]>(MOCK_LEADS);

  readonly allLeads = this.leads.asReadonly();
  readonly leadCount = computed(() => this.leads().length);

  getLeadById(id: string): Lead | undefined {
    return this.leads().find((lead) => lead.id === id);
  }
}
