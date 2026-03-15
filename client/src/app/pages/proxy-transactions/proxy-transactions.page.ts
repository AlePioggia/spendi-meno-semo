import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-proxy-transactions',
  templateUrl: './proxy-transactions.page.html',
  styleUrl: './proxy-transactions.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],
})
export class ProxyTransactionsPage { }
