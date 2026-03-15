import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CategoryService } from '../../services/category.service';
import {
  CategoryResponseDto
} from '../../interfaces/category.interface';

import { InputFieldComponent } from '../../shared/input-field/input-field/input-field.component';
import { CommonModule } from '@angular/common';
import { CategoryCreateDialog } from './category-create.dialog';
import { TableViewConfig } from '../../shared/table-view/table-column.interface';
import { TableViewComponent } from '../../shared/table-view/table-view.component';

@Component({
  standalone: true,
  selector: 'app-category-page',
  templateUrl: './category.page.html',
  styleUrl: './category.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatTooltipModule,
    InputFieldComponent,
    TableViewComponent
  ]
})
export class CategoryPage {

  private readonly categoryService = inject(CategoryService);
  private readonly dialog = inject(MatDialog);

  categories = signal<CategoryResponseDto[]>([]);
  loading = signal(false);

  tableConfig: TableViewConfig<CategoryResponseDto> = {
    columns: [
      {
        key: 'name',
        label: 'Nome',
        type: 'string',
      },
      {
        key: 'description',
        label: 'Descrizione',
        type: 'string'
      },
      {
        key: 'createdAt',
        label: 'Creata il',
        type: 'date',
        formatter: (value) => {
          const date = new Date(value);
          return new Intl.DateTimeFormat('it-IT', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
          }).format(date);
        }
      },
    ],
    sortable: true,
    filterable: true,
    showActions: true,
  }

  constructor() {
    this.load();
  }

  deleteCategory(id: number) {
    this.categoryService.deleteCategory(id).subscribe(() => this.load());
  }

  openCreateDialog() {
    const dialogRef = this.dialog.open(CategoryCreateDialog, {
      width: '420px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      this.loading.set(true);
      this.categoryService.createCategory(result).subscribe({
        next: () => {
          this.load();
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    });
  }

  private load() {
    this.loading.set(true);

    this.categoryService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onTableRowAction(event: { action: string; row: CategoryResponseDto }) {
      if (event.action === 'delete') {
        this.deleteCategory(event.row.id);
      }
  }
}
