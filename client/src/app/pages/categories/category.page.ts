import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal
} from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { CategoryService } from '../../services/category.service';
import {
  CategoryRequestDto,
  CategoryResponseDto
} from '../../interfaces/category.interface';

import { InputFieldComponent } from '../../shared/input-field/input-field/input-field.component';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CategoryCreateDialog } from './category-create.dialog';

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
    InputFieldComponent,
    RouterLink
  ]
})
export class CategoryPage {

  private readonly categoryService = inject(CategoryService);
  private readonly dialog = inject(MatDialog);

  categories = signal<CategoryResponseDto[]>([]);
  loading = signal(false);

  displayedColumns = ['name', 'description', 'createdAt', 'actions'];

  constructor() {
    this.load();
  }

  load() {
    this.categoryService.getCategories().subscribe(this.categories.set);
  }

  deleteCategory(id: number) {
    this.categoryService.deleteCategory(id).subscribe(() => this.load());
  }

  /** --- NUOVO: apre il dialog per creare una categoria --- */
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
}
