import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="stat-card" [style.background]="bgColor">
      <div class="stat-card__icon">
        <i [class]="icon" [style.color]="iconColor"></i>
      </div>
      <div class="stat-card__info">
        <div class="stat-card__count">{{ count | number }}</div>
        <div class="stat-card__label">{{ label }}</div>
      </div>
    </div>
  `,
  styles: [`
    .stat-card {
      display: flex;
      align-items: center;
      padding: 1.5rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      gap: 1rem;
    }
    .stat-card__icon i { font-size: 2.5rem; }
    .stat-card__count { font-size: 2rem; font-weight: 700; color: #1B2A4A; }
    .stat-card__label { font-size: 0.875rem; color: #666; font-weight: 500; margin-top: 0.25rem; }
  `]
})
export class StatCardComponent {
  @Input() label = '';
  @Input() count = 0;
  @Input() icon = 'pi pi-users';
  @Input() bgColor = '#FFF9E6';
  @Input() iconColor = '#E85D26';
}
