import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { map } from 'rxjs';
import { LeadsService } from '../../data-access/leads.service';
import { StatusBadge } from '../../ui/status-badge/status-badge';

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

  goBack(): void {
    this.router.navigate(['/leads']);
  }
}
