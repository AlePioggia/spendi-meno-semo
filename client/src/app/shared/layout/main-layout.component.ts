import { Component, ViewChild, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { MatSidenav, MatSidenavModule } from "@angular/material/sidenav";
import { FooterComponent } from "./footer.component";
import { HeaderComponent } from "./header.component";
import { MatListModule } from "@angular/material/list";

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    HeaderComponent,
    FooterComponent
  ],
  template: `
    <button *ngIf="!sidenavOpened()" mat-icon-button (click)="sidenav.open()" class="toggle-btn-floating" aria-label="Apri menu">
      <mat-icon>menu</mat-icon>
    </button>

    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav #sidenav mode="side" opened (openedChange)="sidenavOpened.set($event)" class="sidenav">
        <div class="sidenav-header">
          <button mat-icon-button (click)="sidenav.toggle()" class="toggle-btn" aria-label="Chiudi menu">
            <mat-icon>menu</mat-icon>
          </button>
          <h3>Menu</h3>
        </div>
        <mat-nav-list>
          <a mat-list-item routerLink="/transactions" routerLinkActive="active">Transazioni</a>
          <a mat-list-item routerLink="/recurring-transactions" routerLinkActive="active">Spese ricorrenti</a>
          <a mat-list-item routerLink="/categories" routerLinkActive="active">Categorie</a>
          <a mat-list-item routerLink="/proxy-transactions" routerLinkActive="active">Spese previste</a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content class="main-content">
        <app-header></app-header>
        <main class="page">
          <div class="page-inner">
            <router-outlet></router-outlet>
          </div>
        </main>
        <app-footer></app-footer>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .sidenav-container {
      height: 100vh;
    }

    .sidenav {
      width: 190px;
      background-color: #1f2937;
      color: #f9fafb;
    }

    .sidenav-header {
      padding: 14px 14px;
      font-size: 1rem;
      font-weight: 600;
      color: #ffffff;
      border-bottom: 1px solid rgba(255, 255, 255, 0.1);
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .sidenav-header h3 {
      margin: 0;
      flex: 1;
    }

    .toggle-btn {
      --mdc-icon-button-state-layer-size: 32px;
      --mdc-icon-button-icon-size: 20px;
      color: #e5e7eb;
    }

    .toggle-btn:hover {
      color: #ffffff;
    }

    .toggle-btn-floating {
      position: fixed;
      top: 16px;
      left: 16px;
      z-index: 1001;
      --mdc-icon-button-state-layer-size: 40px;
      --mdc-icon-button-icon-size: 24px;
      background: #1f2937;
      color: #f9fafb;
      animation: slideInMenu 0.3s ease-out;
      transition: opacity 0.3s ease-out;
    }

    .toggle-btn-floating:hover {
      background: #374151;
    }

    @keyframes slideInMenu {
      from {
        opacity: 0;
        transform: translateX(-40px);
      }
      to {
        opacity: 1;
        transform: translateX(0);
      }
    }

    :host ::ng-deep .sidenav .mat-mdc-list-item {
      --mdc-list-list-item-one-line-container-height: 40px;
    }

    :host ::ng-deep .sidenav a.mat-mdc-list-item .mdc-list-item__primary-text {
    color: #e5e7eb !important;
    }

    :host ::ng-deep .sidenav a.mat-mdc-list-item:hover {
    background-color: rgba(59, 130, 246, 0.15) !important;
    }
    :host ::ng-deep .sidenav a.mat-mdc-list-item:hover .mdc-list-item__primary-text {
    color: #ffffff !important;
    }

    :host ::ng-deep .sidenav a.mat-mdc-list-item.active {
    background-color: #3b82f6 !important;
    }
    :host ::ng-deep .sidenav a.mat-mdc-list-item.active .mdc-list-item__primary-text {
    color: #ffffff !important;
    }

    .main-content {
      padding: 16px 12px;
      display: flex;
      flex-direction: column;
      height: 100%;
      background: #f8fafc;
    }

    .page {
      flex: 1;
      display: flex;
      justify-content: center;
      width: 100%;
      padding: 16px 0;
      box-sizing: border-box;
    }

    .page-inner {
      width: 100%;
      max-width: 1100px;
    }

    @media (max-width: 768px) {
      .page {
        padding: 12px 0;
      }
    }
  `]
})
export class MainLayoutComponent {
  @ViewChild('sidenav') sidenav!: MatSidenav;
  sidenavOpened = signal(true);
}
