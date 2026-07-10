import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Lead } from '../../data-access/lead.model';
import { StatusBadge } from '../status-badge/status-badge';

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

  onSelect(): void {
    this.selected.emit(this.lead().id);
  }
}
