import { Component, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule } from '@angular/forms';
import { InputFieldComponent } from '../../shared/input-field/input-field/input-field.component';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, ReactiveFormsModule, MatProgressSpinner, InputFieldComponent, RouterLink],
  templateUrl: './register.page.html',
  styleUrls: ['./register.page.css']
})
export class RegisterPage {
  username = signal('');
  email = signal('');
  password = signal('');
  confirmPassword = signal('');

  loading = signal(false);

  register() {
    if (!this.username() || !this.email() || !this.password() || !this.confirmPassword()) {
      alert('Compila tutti i campi!');
      return;
    }
    if (this.password() !== this.confirmPassword()) {
      alert('Le password non corrispondono!');
      return;
    }

    this.loading.set(true);
  }
}
