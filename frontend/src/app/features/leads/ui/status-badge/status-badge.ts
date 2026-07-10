import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { LeadStatus } from '../../data-access/lead.model';

@Component({
  selector: 'app-status-badge',
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatusBadge {
  readonly status = input.required<LeadStatus>();
}
