import { Component } from "@angular/core";
import { MatToolbarModule } from "@angular/material/toolbar";
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { FooterComponent } from "./footer.component";
import { HeaderComponent } from "./header.component";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatListModule } from "@angular/material/list";

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    HeaderComponent,
    FooterComponent
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav mode="side" opened class="sidenav">
        <div class="sidenav-header">
          <h3>Menu</h3>
        </div>
        <mat-nav-list>
          <a mat-list-item routerLink="/transactions" routerLinkActive="active">Transazioni</a>
          <a mat-list-item routerLink="/recurring-transactions" routerLinkActive="active">Spese ricorrenti</a>
          <a mat-list-item routerLink="/categories" routerLinkActive="active">Categorie</a>
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
export class MainLayoutComponent {}
