import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="footer">
      <span>DPWH HRIS &copy; 2026 — Department of Public Works and Highways. All rights reserved.</span>
    </footer>
  `,
  styles: [`
    .footer {
      background: white;
      border-top: 1px solid #e9ecef;
      padding: 0.75rem 1.5rem;
      text-align: center;
      font-size: 0.8rem;
      color: #888;
    }
  `]
})
export class FooterComponent {}
