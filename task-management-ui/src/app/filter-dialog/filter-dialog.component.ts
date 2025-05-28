import { Component, Inject } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { JiraTaskStatus, SortDirection } from '../models/jira-task.model';

export interface FilterDialogData {
  searchTerm: string;
  statusFilter: JiraTaskStatus | null;
  showFavoritesOnly: boolean;
  statusOptions: { value: JiraTaskStatus | null; label: string }[];
  sortProperty?: string;
  sortDirection?: SortDirection;
  sortProperties?: { value: string; label: string }[];
  sortDirections?: { value: SortDirection; label: string }[];
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
  filterForm: UntypedFormGroup;
  statusOptions: any[];
  sortProperties: any[];
  sortDirections: any[];

  constructor(
    private fb: UntypedFormBuilder,
    public dialogRef: MatDialogRef<FilterDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FilterDialogData
  ) {
    this.statusOptions = this.data.statusOptions;
    this.sortProperties = this.data.sortProperties || [];
    this.sortDirections = this.data.sortDirections || [];
    
    this.filterForm = this.fb.group({
      searchTerm: [data.searchTerm],
      statusFilter: [data.statusFilter],
      showFavoritesOnly: [data.showFavoritesOnly],
      sortProperty: [data.sortProperty || 'Name'],
      sortDirection: [data.sortDirection || SortDirection.Asc]
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
      showFavoritesOnly: false,
      sortProperty: 'Name',
      sortDirection: SortDirection.Asc
    });
  }
    hasActiveFilters(): boolean {
    const formValue = this.filterForm.value;
    return formValue.searchTerm || 
           formValue.statusFilter !== null || 
           formValue.showFavoritesOnly ||
           (formValue.sortProperty !== 'Name' || formValue.sortDirection !== SortDirection.Asc);
  }

  // Get the icon to display based on the sort direction
  getSortDirectionIcon(): string {
    const direction = this.filterForm.get('sortDirection')?.value;
    return direction === SortDirection.Asc ? 'arrow_upward' : 'arrow_downward';
  }

  // Get the label for the selected sort property
  getSortPropertyLabel(): string {
    const property = this.filterForm.get('sortProperty')?.value;
    const option = this.sortProperties.find(opt => opt.value === property);
    return option ? option.label : '';
  }

  // Get the label for the selected sort direction
  getSortDirectionLabel(): string {
    const direction = this.filterForm.get('sortDirection')?.value;
    const option = this.sortDirections.find(opt => opt.value === direction);
    return option ? option.label : '';
  }
}
