import { Component, computed, inject, signal } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import {MatDialogModule, MatDialogRef} from "@angular/material/dialog";
import { InputFieldComponent } from "../../shared/input-field/input-field/input-field.component";

@Component({
  standalone: true,
  selector: 'app-category-create-dialog',
  template: `
    <h2 mat-dialog-title>Nuova categoria</h2>

    <mat-dialog-content>
      <app-input-field
        label="Nome"
        [value]="name"
        (valueChange)="name.set($event)"
      />

      <app-input-field
        label="Descrizione"
        [value]="description"
        (valueChange)="description.set($event)"
      />
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Annulla</button>

      <button
        mat-raised-button
        color="primary"
        [disabled]="!canCreate()"
        (click)="confirm()"
      >
        Crea
      </button>
    </mat-dialog-actions>
  `,
  imports: [
    MatDialogModule,
    MatButtonModule,
    InputFieldComponent
  ]
})
export class CategoryCreateDialog {

  name = signal('');
  description = signal('');

  canCreate = computed(() => this.name().trim().length > 0);

  private dialogRef = inject(MatDialogRef<CategoryCreateDialog>);

  confirm() {
    this.dialogRef.close({
      name: this.name(),
      description: this.description()
    });
  }
}
