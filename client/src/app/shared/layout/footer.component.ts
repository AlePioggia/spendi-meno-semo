import { Component } from "@angular/core";

@Component({
  selector: 'app-footer',
  standalone: true,
  template: `
    <footer class="footer">
      <div class="footer-inner">
        <span>© 2026 · Spendi meno semo</span>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      padding: 16px;
      border-top: 1px solid #e5e7eb;
      background: rgba(255, 255, 255, 0.8);
      backdrop-filter: blur(8px);
    }

    .footer-inner {
      max-width: 1100px;
      margin: 0 auto;
      text-align: center;
      font-size: 12px;
      color: #6b7280;
      letter-spacing: 0.2px;
    }
  `]
})
export class FooterComponent {}
