import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Column } from '../models/column.model';
import { JiraTask } from '../models/jira-task.model';
import { ColumnService } from '../services/column.service';
import { TaskService } from '../services/task.service';
import { TaskDialogComponent } from '../task-dialog/task-dialog.component';
import { ColumnDialogComponent } from '../column-dialog/column-dialog.component';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-kanban-board',
  templateUrl: './kanban-board.component.html',
  styleUrls: ['./kanban-board.component.scss']
})
export class KanbanBoardComponent implements OnInit {
  columns: Column[] = [];
  tasksMap: { [columnId: number]: JiraTask[] } = {};
  isLoading = true;
  
  // Properties for deadline checking
  now = new Date();
  soon = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000); // 7 days from now
  
  constructor(
    private columnService: ColumnService,
    private taskService: TaskService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadColumns();
  }

  loadColumns(): void {
    this.isLoading = true;
    this.columnService.getColumns().subscribe(
      columns => {
        this.columns = columns.sort((a, b) => a.order - b.order);
        this.loadTasksForColumns();
      },
      error => {
        console.error('Error loading columns:', error);
        this.isLoading = false;
        this.showErrorMessage('Failed to load columns. Please try again later.');
      }
    );
  }

  loadTasksForColumns(): void {
    if (this.columns.length === 0) {
      this.isLoading = false;
      return;
    }

    let loadedColumns = 0;
    this.columns.forEach(column => {
      this.taskService.getTasksByColumn(column.id).subscribe(
        tasks => {
          this.tasksMap[column.id] = tasks;
          loadedColumns++;
          if (loadedColumns === this.columns.length) {
            this.isLoading = false;
          }
        },
        error => {
          console.error(`Error loading tasks for column ${column.id}:`, error);
          loadedColumns++;
          if (loadedColumns === this.columns.length) {
            this.isLoading = false;
          }
          this.showErrorMessage(`Failed to load tasks for ${column.name}`);
        }
      );
    });
  }

  openAddColumnDialog(): void {
    const dialogRef = this.dialog.open(ColumnDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.createColumn(result);
      }
    });
  }

  createColumn(column: any): void {
    this.columnService.createColumn(column).subscribe(
      response => {
        this.showSuccessMessage('Column created successfully');
        this.loadColumns();
      },
      error => {
        console.error('Error creating column:', error);
        this.showErrorMessage('Failed to create column');
      }
    );
  }

  openEditColumnDialog(column: Column): void {
    const dialogRef = this.dialog.open(ColumnDialogComponent, {
      width: '400px',
      data: { column }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.updateColumn(column.id, result);
      }
    });
  }

  updateColumn(id: number, column: any): void {
    this.columnService.updateColumn(id, column).subscribe(
      response => {
        this.showSuccessMessage('Column updated successfully');
        this.loadColumns();
      },
      error => {
        console.error('Error updating column:', error);
        this.showErrorMessage('Failed to update column');
      }
    );
  }

  openDeleteColumnDialog(column: Column): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirm Delete',
        message: `Are you sure you want to delete the column "${column.name}"? All tasks will be moved to the default column.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteColumn(column.id);
      }
    });
  }

  deleteColumn(id: number): void {
    this.columnService.deleteColumn(id).subscribe(
      response => {
        this.showSuccessMessage('Column deleted successfully');
        this.loadColumns();
      },
      error => {
        console.error('Error deleting column:', error);
        this.showErrorMessage('Failed to delete column');
      }
    );
  }

  openAddTaskDialog(columnId: number): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '500px',
      data: { 
        columns: this.columns,
        selectedColumnId: columnId
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.createTask(result);
      }
    });
  }

  createTask(task: any): void {
    this.taskService.createTask(task).subscribe(
      response => {
        this.showSuccessMessage('Task created successfully');
        this.loadTasksForColumns();
      },
      error => {
        console.error('Error creating task:', error);
        this.showErrorMessage('Failed to create task');
      }
    );
  }

  openEditTaskDialog(task: JiraTask): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '500px',
      data: { 
        task,
        columns: this.columns
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.updateTask(task.id, result);
      }
    });
  }

  updateTask(id: number, task: any): void {
    this.taskService.updateTask(id, task).subscribe(
      response => {
        this.showSuccessMessage('Task updated successfully');
        this.loadTasksForColumns();
      },
      error => {
        console.error('Error updating task:', error);
        this.showErrorMessage('Failed to update task');
      }
    );
  }

  openDeleteTaskDialog(task: JiraTask): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirm Delete',
        message: `Are you sure you want to delete the task "${task.name}"?`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteTask(task.id);
      }
    });
  }

  deleteTask(id: number): void {
    this.taskService.deleteTask(id).subscribe(
      response => {
        this.showSuccessMessage('Task deleted successfully');
        this.loadTasksForColumns();
      },
      error => {
        console.error('Error deleting task:', error);
        this.showErrorMessage('Failed to delete task');
      }
    );
  }

  toggleFavorite(task: JiraTask): void {
    task.isFavorite = !task.isFavorite;
    this.taskService.updateTask(task.id, task).subscribe(
      response => {
        this.loadTasksForColumns();
      },
      error => {
        console.error('Error updating favorite status:', error);
        this.showErrorMessage('Failed to update task favorite status');
        // Revert the change locally
        task.isFavorite = !task.isFavorite;
      }
    );
  }

  // Handle task drops between or within columns
  onTaskDrop(event: CdkDragDrop<JiraTask[]>): void {
    if (event.previousContainer === event.container) {
      // Reordering within the same column
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      
      const columnId = parseInt(event.container.id);
      const taskIds = event.container.data.map(task => task.id);
      
      this.taskService.reorderTasksInColumn(columnId, taskIds).subscribe(
        () => {},
        error => {
          console.error('Error reordering tasks:', error);
          this.showErrorMessage('Failed to reorder tasks');
          this.loadTasksForColumns(); // Reload to revert changes
        }
      );
    } else {
      // Moving task to a different column
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      
      const task = event.container.data[event.currentIndex];
      const newColumnId = parseInt(event.container.id);
      
      this.taskService.moveTaskToColumn(task.id, newColumnId, event.currentIndex).subscribe(
        () => {},
        error => {
          console.error('Error moving task:', error);
          this.showErrorMessage('Failed to move task');
          this.loadTasksForColumns(); // Reload to revert changes
        }
      );
    }
  }

  // Handle column reordering
  onColumnDrop(event: CdkDragDrop<Column[]>): void {
    moveItemInArray(this.columns, event.previousIndex, event.currentIndex);
    
    // Update order of all columns
    this.columns.forEach((column, index) => {
      if (column.order !== index) {
        const updatedColumn = { ...column, order: index };
        this.columnService.updateColumn(column.id, updatedColumn).subscribe(
          () => {},
          error => {
            console.error('Error updating column order:', error);
            this.loadColumns(); // Reload to revert changes
          }
        );
      }
    });
  }

  getColumnTasks(columnId: number): JiraTask[] {
    return this.tasksMap[columnId] || [];
  }

  getColumnId(index: number): string {
    return this.columns[index].id.toString();
  }

  // Get connected drop list IDs for drag and drop
  getConnectedDropLists(): string[] {
    return this.columns.map(c => c.id.toString());
  }

  showSuccessMessage(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: 'success-snackbar',
      horizontalPosition: 'end'
    });
  }

  showErrorMessage(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: 'error-snackbar',
      horizontalPosition: 'end'
    });
  }
}
