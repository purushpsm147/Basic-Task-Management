import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { JiraTaskStatus } from '../models/jira-task.model';

export interface FilterDialogData {
  searchTerm: string;
  statusFilter: JiraTaskStatus | null;
  showFavoritesOnly: boolean;
  statusOptions: { value: JiraTaskStatus | null; label: string }[];
}

@Component({
  selector: 'app-filter-dialog',
  templateUrl: './filter-dialog.component.html',
  styleUrls: ['./filter-dialog.component.scss'],
  host: {
    'class': 'filter-dialog-component'
  }
})
export class FilterDialogComponent {
  filterForm: FormGroup;
  statusOptions = this.data.statusOptions;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<FilterDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FilterDialogData
  ) {
    this.filterForm = this.fb.group({
      searchTerm: [data.searchTerm],
      statusFilter: [data.statusFilter],
      showFavoritesOnly: [data.showFavoritesOnly]
    });
  }

  apply(): void {
    this.dialogRef.close(this.filterForm.value);
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
  
  clearFilters(): void {
    this.filterForm.patchValue({
      searchTerm: '',
      statusFilter: null,
      showFavoritesOnly: false
    });
  }
  
  hasActiveFilters(): boolean {
    const formValue = this.filterForm.value;
    return formValue.searchTerm || 
           formValue.statusFilter !== null || 
           formValue.showFavoritesOnly;
  }
}
