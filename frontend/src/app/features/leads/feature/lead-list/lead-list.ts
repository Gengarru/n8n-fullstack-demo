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
