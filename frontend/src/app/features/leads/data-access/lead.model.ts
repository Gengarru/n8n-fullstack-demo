export type LeadStatus = 'new' | 'enriched' | 'contacted' | 'qualified';

export interface LeadErrorHistoryEntry {
  occurredAt: string;
  message: string;
}

export interface Lead {
  id: string;
  companyName: string;
  status: LeadStatus;
  score: number;
  contactName: string;
  contactEmail: string;
  industry: string;
  enrichedAt: string | null;
  errorHistory: LeadErrorHistoryEntry[];
}
