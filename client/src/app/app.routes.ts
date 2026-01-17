import { Routes } from '@angular/router';
import { LoginPage } from './pages/login/login.page';
import { RegisterPage } from './pages/register/register.page';

export const routes: Routes = [
  { path: 'login', loadComponent: () => LoginPage},
  { path: 'register', loadComponent: () => RegisterPage},
  { path: '**', redirectTo: 'login' },
];
