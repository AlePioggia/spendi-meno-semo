import { Routes } from '@angular/router';
import { CategoryPage } from './pages/categories/category.page';
import { RecurringTransactionsPage } from './pages/recurring-transactions/recurring-transactions.page';
import { TransactionsPage } from './pages/transactions/transactions.page';
import { MainLayoutComponent } from './shared/layout/main-layout.component';
export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: 'transactions', component: TransactionsPage },
      { path: 'recurring-transactions', component: RecurringTransactionsPage },
      { path: 'categories', component: CategoryPage },
      { path: '', redirectTo: 'transactions', pathMatch: 'full' }
    ]
  },

  { path: '**', redirectTo: 'transactions' },
];