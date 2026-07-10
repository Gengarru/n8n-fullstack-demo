import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { Lead } from '../../data-access/lead.model';
import { StatusBadge } from '../status-badge/status-badge';

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
  selector: 'app-lead-card',
  imports: [StatusBadge],
  templateUrl: './lead-card.html',
  styleUrl: './lead-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadCard {
  readonly lead = input.required<Lead>();
  readonly selected = output<string>();

  readonly initials = computed(() => {
    const words = this.lead().companyName.split(' ').filter(Boolean);
    return words
      .slice(0, 2)
      .map((word) => word[0]?.toUpperCase() ?? '')
      .join('');
  });

  readonly avatarGradient = computed(() => {
    const index = hashString(this.lead().companyName) % AVATAR_GRADIENTS.length;
    return AVATAR_GRADIENTS[index];
  });

  onSelect(): void {
    this.selected.emit(this.lead().id);
  }
}
