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
