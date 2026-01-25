import { Routes } from '@angular/router';
import { LoginPage } from './pages/login/login.page';
import { RegisterPage } from './pages/register/register.page';
import { CategoryPage } from './pages/categories/category.page';
import { MainLayoutComponent } from './shared/layout/main-layout.component';
export const routes: Routes = [
  // pagine fuori layout
  { path: 'login', loadComponent: () => LoginPage },
  { path: 'register', loadComponent: () => RegisterPage },

  // tutte le pagine dentro il layout
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: 'categories', component: CategoryPage },
      // altre pagine protette / principali
      // { path: 'dashboard', component: DashboardPage },
      { path: '', redirectTo: 'categories', pathMatch: 'full' }
    ]
  },

  // catch-all
  { path: '**', redirectTo: 'categories' },
];