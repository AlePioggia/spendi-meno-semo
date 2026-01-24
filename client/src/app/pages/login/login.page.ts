import { Component, signal } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { InputFieldComponent } from '../../shared/input-field/input-field/input-field.component';
import { inject } from '@angular/core';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { RouterLink, RouterOutlet } from '@angular/router';
import { keycloak } from '../../services/keycloak.service';
import { CategoryService } from '../../services/category.service';
import { CategoryResponseDto } from '../../interfaces/category.interface';

@Component({
  selector: 'app-login',
  imports: [
    MatCardModule,
    MatButtonModule,
    ReactiveFormsModule,
    InputFieldComponent,
    MatProgressSpinner,
    RouterLink
  ],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
  standalone: true
})
export class LoginPage {
  username = signal('');
  password = signal('');

  loading = signal(false);

  private categoryService = inject(CategoryService);

  login() {
    if (!this.username() || !this.password()) {
      alert('Inserisci username e password!');

      const fakeCategory = {
        name: 'Categoria di test',
        description: 'Descrizione di test'
      }

      this.categoryService.createCategory(fakeCategory).subscribe({
        next: (fakeCategory: CategoryResponseDto) => {
          this.loading.set(false);
        },
        error: (err) => {
          this.loading.set(false);
        }
      });

      return;
    }

    this.loading.set(true);
  }
}
