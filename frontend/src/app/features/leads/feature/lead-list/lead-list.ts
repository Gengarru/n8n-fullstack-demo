import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
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

  onLeadSelected(leadId: string): void {
    this.router.navigate(['/leads', leadId]);
  }
}
