import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Column } from '../models/column.model';
import { ColumnService } from '../services/column.service';

@Component({
  selector: 'app-column-dialog',
  template: `
    <h2 mat-dialog-title>{{ isEditMode ? 'Edit Column' : 'Create New Column' }}</h2>
    <form [formGroup]="columnForm" (ngSubmit)="onSubmit()">
      <div mat-dialog-content>
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" placeholder="Column name" required>
          <mat-error *ngIf="columnForm.controls['name'].hasError('required')">
            Name is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Description</mat-label>
          <textarea matInput formControlName="description" placeholder="Column description"></textarea>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Color</mat-label>
          <input matInput formControlName="color" placeholder="Color (hex)" 
                 [style.background]="columnForm.controls['color'].value"
                 [style.color]="getContrastColor(columnForm.controls['color'].value)">
        </mat-form-field>

        <div class="color-picker">
          <button type="button" mat-mini-fab *ngFor="let color of predefinedColors" 
                  [style.background-color]="color"
                  (click)="selectColor(color)"></button>
        </div>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Order</mat-label>
          <input matInput type="number" formControlName="order" placeholder="Column order">
        </mat-form-field>

        <div class="is-default-toggle">
          <mat-slide-toggle formControlName="isDefault">Set as Default Column</mat-slide-toggle>
        </div>
      </div>
      <div mat-dialog-actions align="end">
        <button type="button" mat-button (click)="onCancel()">Cancel</button>
        <button type="submit" mat-raised-button color="primary" [disabled]="columnForm.invalid">
          {{ isEditMode ? 'Update' : 'Create' }}
        </button>
      </div>
    </form>
  `,
  styles: [`
    .full-width {
      width: 100%;
      margin-bottom: 15px;
    }
    .color-picker {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-bottom: 15px;
    }
    .is-default-toggle {
      margin-bottom: 15px;
    }
    .mat-mini-fab {
      width: 30px;
      height: 30px;
      min-height: 30px;
      line-height: 30px;
    }
  `]
})
export class ColumnDialogComponent implements OnInit {
  columnForm: UntypedFormGroup;
  isEditMode = false;
  
  predefinedColors = [
    '#f44336', '#e91e63', '#9c27b0', '#673ab7', 
    '#3f51b5', '#2196f3', '#03a9f4', '#00bcd4', 
    '#009688', '#4caf50', '#8bc34a', '#cddc39', 
    '#ffeb3b', '#ffc107', '#ff9800', '#ff5722'
  ];

  constructor(
    private fb: UntypedFormBuilder,
    private columnService: ColumnService,
    private dialogRef: MatDialogRef<ColumnDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { column?: Column }
  ) {
    this.columnForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      color: [this.columnService.generateRandomColor()],
      order: [0],
      isDefault: [false]
    });

    if (data && data.column) {
      this.isEditMode = true;
      this.columnForm.patchValue({
        name: data.column.name,
        description: data.column.description || '',
        color: data.column.color || this.columnService.generateRandomColor(),
        order: data.column.order,
        isDefault: data.column.isDefault
      });
    }
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.columnForm.valid) {
      this.dialogRef.close(this.columnForm.value);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  selectColor(color: string): void {
    this.columnForm.patchValue({ color });
  }

  getContrastColor(hexColor: string): string {
    // Function to determine whether to use white or black text based on background color
    if (!hexColor) return 'black';
    
    // Remove the #
    hexColor = hexColor.replace('#', '');
    
    // Convert to RGB
    const r = parseInt(hexColor.substr(0, 2), 16);
    const g = parseInt(hexColor.substr(2, 2), 16);
    const b = parseInt(hexColor.substr(4, 2), 16);
    
    // Calculate luminance
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
    
    return luminance > 0.5 ? 'black' : 'white';
  }
}
