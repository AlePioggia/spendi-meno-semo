import { Component, EventEmitter, Input, Output, Signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-input-field',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './input-field.component.html',
  styleUrl: './input-field.component.css',
  standalone:true
})
export class InputFieldComponent {
  @Input() label = '';
  @Input() type: 'text' | 'password' | 'number' | 'date' = 'text';

  @Input() value!: Signal<string>;
  @Output() valueChange = new EventEmitter<string>();
}
