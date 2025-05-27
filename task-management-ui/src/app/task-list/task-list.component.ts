import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TaskService } from '../services/task.service';
import { JiraTask, JiraTaskStatus, SortDirection } from '../models/jira-task.model';
import { TaskDialogComponent } from '../task-dialog/task-dialog.component';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { FilterDialogComponent, FilterDialogData } from '../filter-dialog/filter-dialog.component';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss'],
  animations: []
})
export class TaskListComponent implements OnInit {
  tasks: JiraTask[] = [];
  filteredTasks: JiraTask[] = [];
  loading = false;
  
  // UI state
  showFilters = false; // Keep this for potential inline filters in future
  
  // Filter and sort options
  statusFilter: JiraTaskStatus | null = null;
  sortProperty = 'Name';
  sortDirection = SortDirection.Asc;
  showFavoritesOnly = false;
  searchTerm = '';
  
  // Options for dropdowns
  statusOptions = this.taskService.getTaskStatusOptions();
  sortProperties = this.taskService.getSortableProperties();
  sortDirections = this.taskService.getSortDirectionOptions();
  
  displayedColumns: string[] = ['favorite', 'name', 'description', 'deadline', 'status', 'actions'];

  constructor(
    private taskService: TaskService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.loading = true;
    this.taskService.getTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.applyFilters();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading tasks:', error);
        this.snackBar.open('Error loading tasks', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  applySort(): void {
    this.loading = true;
    this.taskService.getSortedTasks(this.sortDirection, this.sortProperty).subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.applyFilters();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error sorting tasks:', error);
        this.snackBar.open('Error sorting tasks', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.tasks];

    // Filter by status
    if (this.statusFilter !== null) {
      filtered = filtered.filter(task => task.status === this.statusFilter);
    }

    // Filter by favorites
    if (this.showFavoritesOnly) {
      filtered = filtered.filter(task => task.isFavorite);
    }

    // Filter by search term
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(task => 
        task.name.toLowerCase().includes(term) || 
        task.description.toLowerCase().includes(term)
      );
    }

    this.filteredTasks = filtered;
  }

  onStatusFilterChange(): void {
    if (this.statusFilter !== null) {
      this.loading = true;
      this.taskService.getTasksByStatus(this.statusFilter).subscribe({
        next: (tasks) => {
          this.tasks = tasks;
          this.applyFilters();
          this.loading = false;
        },
        error: (error) => {
          console.error('Error filtering tasks:', error);
          this.snackBar.open('Error filtering tasks', 'Close', { duration: 3000 });
          this.loading = false;
        }
      });
    } else {
      this.loadTasks();
    }
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onFavoritesToggle(): void {
    this.applyFilters();
  }

  openCreateDialog(): void {
    // Prevent opening multiple create dialogs
    if (this.dialog.openDialogs.some(d => d.componentInstance instanceof TaskDialogComponent)) {
      return;
    }
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '600px',
      data: { mode: 'create' },
      panelClass: 'custom-dialog-container',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTasks();
      }
    });
  }

  openEditDialog(task: JiraTask): void {
    // Prevent opening multiple edit dialogs
    if (this.dialog.openDialogs.some(d => d.componentInstance instanceof TaskDialogComponent)) {
      return;
    }
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '600px',
      data: { mode: 'edit', task: { ...task } },
      panelClass: 'custom-dialog-container',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTasks();
      }
    });
  }

  toggleFavorite(task: JiraTask): void {
    const updatedTask = { ...task, isFavorite: !task.isFavorite };
    this.taskService.updateTask(task.id, updatedTask).subscribe({
      next: () => {
        this.snackBar.open('Task updated successfully', 'Close', { duration: 2000 });
        this.loadTasks();
      },
      error: (error) => {
        console.error('Error updating task:', error);
        this.snackBar.open('Error updating task', 'Close', { duration: 3000 });
      }
    });
  }

  deleteTask(task: JiraTask): void {
    // Prevent opening multiple dialogs
    if (this.dialog.openDialogs.some(d => 
        d.componentInstance instanceof ConfirmDialogComponent ||
        d.componentInstance instanceof TaskDialogComponent ||
        d.componentInstance instanceof FilterDialogComponent)) {
      return;
    }
    
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Task',
        message: `Are you sure you want to delete "${task.name}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      },
      panelClass: 'custom-dialog-container'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.taskService.deleteTask(task.id).subscribe({
          next: () => {
            this.snackBar.open('Task deleted successfully', 'Close', { duration: 2000 });
            this.loadTasks();
          },
          error: (error) => {
            console.error('Error deleting task:', error);
            this.snackBar.open('Error deleting task', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  openFilterDialog(): void {
    // Prevent opening multiple filter dialogs
    if (this.dialog.openDialogs.some(d => d.componentInstance instanceof FilterDialogComponent)) {
      return;
    }
    
    const data: FilterDialogData = {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      showFavoritesOnly: this.showFavoritesOnly,
      statusOptions: this.statusOptions
    };
    
    const dialogRef = this.dialog.open(FilterDialogComponent, {
      width: '400px',
      data,
      panelClass: 'custom-dialog-container',
      position: { top: '80px' },
      maxWidth: '95vw'
    });
    
    dialogRef.afterClosed().subscribe((result: FilterDialogData | null) => {
      if (result) {
        this.searchTerm = result.searchTerm;
        this.statusFilter = result.statusFilter;
        this.showFavoritesOnly = result.showFavoritesOnly;
        if (this.statusFilter !== null) {
          this.onStatusFilterChange();
        } else {
          this.applyFilters();
        }
      }
    });
  }

  // Get empty state message based on current filters
  getEmptyStateMessage(): string {
    if (this.searchTerm) {
      return `No tasks found matching "${this.searchTerm}"`;
    }
    if (this.statusFilter !== null) {
      return `No tasks found with ${this.getStatusLabel(this.statusFilter)} status`;
    }
    if (this.showFavoritesOnly) {
      return 'No favorite tasks found';
    }
    return 'Get started by creating your first task';
  }

  // Get status label for display
  getStatusLabel(status: JiraTaskStatus): string {
    const option = this.statusOptions.find(opt => opt.value === status);
    return option ? option.label : 'Unknown';
  }

  clearFilters(): void {
    this.statusFilter = null;
    this.showFavoritesOnly = false;
    this.searchTerm = '';
    this.sortProperty = 'Name';
    this.sortDirection = SortDirection.Asc;
    this.loadTasks();
  }

  isOverdue(deadline: Date): boolean {
    return new Date(deadline) < new Date();
  }

  onImageError(event: Event, task: JiraTask) {
    // Optionally, set a placeholder image or hide the element
    // For now, we just log it. Hiding is done via *ngIf in template.
    console.warn(`Image failed to load for task ${task.id}:`, (event.target as HTMLImageElement).src);
    // To prevent broken image icon, you could set a flag on the task object
    // task.imageLoadError = true; 
    // And then use *ngIf="!task.imageLoadError && task.imageURL" in the template
  }
}
