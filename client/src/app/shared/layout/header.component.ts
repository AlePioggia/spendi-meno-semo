import { Component } from "@angular/core";
import { MatToolbarModule } from "@angular/material/toolbar";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [MatToolbarModule],
  template: `
    <mat-toolbar class="app-toolbar">
      <div class="toolbar-inner">
        <span class="brand">spendi-meno-semo</span>
      </div>
    </mat-toolbar>
  `,
  styles: [`
    .app-toolbar {
      position: sticky;
      top: 0;
      z-index: 1000;
      background: #ffffff;
      color: #111827;
      border-bottom: 1px solid #e5e7eb;
      box-shadow: 0 2px 10px rgba(17, 24, 39, 0.06);
    }

    .toolbar-inner {
      width: 100%;
      max-width: 1100px;
      margin: 0 auto;
      padding: 0 16px;
      display: flex;
      align-items: center;
      gap: 12px;
      box-sizing: border-box;
    }

    .brand {
      font-weight: 700;
      letter-spacing: 0.2px;
    }
  `]
})
export class HeaderComponent {}
