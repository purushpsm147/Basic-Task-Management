import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TaskService } from '../services/task.service';
import { JiraTask, JiraTaskStatus, CreateTaskRequest, UpdateTaskRequest } from '../models/jira-task.model';

export interface TaskDialogData {
  mode: 'create' | 'edit';
  task?: JiraTask;
}

@Component({
  selector: 'app-task-dialog',
  templateUrl: './task-dialog.component.html',
  styleUrls: ['./task-dialog.component.scss'],
  host: {
    'class': 'task-dialog-component'
  }
})
export class TaskDialogComponent implements OnInit {
  taskForm: FormGroup;
  statusOptions = this.taskService.getTaskStatusOptions();
  selectedFile: File | null = null;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<TaskDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TaskDialogData
  ) {
    this.taskForm = this.createForm();
  }

  ngOnInit(): void {
    if (this.data.mode === 'edit' && this.data.task) {
      this.populateForm(this.data.task);
    }
  }

  createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      deadline: ['', Validators.required],
      status: [JiraTaskStatus.Unassigned, Validators.required],
      isFavorite: [false]
    });
  }

  populateForm(task: JiraTask): void {
    this.taskForm.patchValue({
      name: task.name,
      description: task.description,
      deadline: new Date(task.deadline),
      status: task.status,
      isFavorite: task.isFavorite
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file type and size
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
      const maxSize = 5 * 1024 * 1024; // 5MB

      if (!allowedTypes.includes(file.type)) {
        this.snackBar.open('Please select a valid image file (JPEG, PNG, GIF)', 'Close', { duration: 3000 });
        return;
      }

      if (file.size > maxSize) {
        this.snackBar.open('File size must be less than 5MB', 'Close', { duration: 3000 });
        return;
      }

      this.selectedFile = file;
    }
  }

  onSubmit(): void {
    if (this.taskForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      const formValue = this.taskForm.value;

      if (this.data.mode === 'create') {
        this.createTask(formValue);
      } else {
        this.updateTask(formValue);
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  createTask(formValue: any): void {
    const createRequest: CreateTaskRequest = {
      name: formValue.name,
      description: formValue.description,
      deadline: formValue.deadline,
      status: formValue.status,
      isFavorite: formValue.isFavorite
    };

    this.taskService.createTask(createRequest).subscribe({
      next: (task) => {
        this.snackBar.open('Task created successfully', 'Close', { duration: 2000 });
        
        // Upload image if selected
        if (this.selectedFile) {
          this.uploadImage(task.id);
        } else {
          this.closeDialog(true);
        }
      },
      error: (error) => {
        console.error('Error creating task:', error);
        this.snackBar.open('Error creating task', 'Close', { duration: 3000 });
        this.isSubmitting = false;
      }
    });
  }

  updateTask(formValue: any): void {
    if (!this.data.task) return;

    const updateRequest: UpdateTaskRequest = {
      id: this.data.task.id,
      name: formValue.name,
      description: formValue.description,
      deadline: formValue.deadline,
      status: formValue.status,
      isFavorite: formValue.isFavorite,
      imageURL: this.data.task.imageURL
    };

    this.taskService.updateTask(this.data.task.id, updateRequest).subscribe({
      next: () => {
        this.snackBar.open('Task updated successfully', 'Close', { duration: 2000 });
        
        // Upload image if selected
        if (this.selectedFile) {
          this.uploadImage(this.data.task!.id);
        } else {
          this.closeDialog(true);
        }
      },
      error: (error) => {
        console.error('Error updating task:', error);
        this.snackBar.open('Error updating task', 'Close', { duration: 3000 });
        this.isSubmitting = false;
      }
    });
  }

  uploadImage(taskId: number): void {
    if (!this.selectedFile) {
      this.closeDialog(true);
      return;
    }

    this.taskService.uploadTaskImage(taskId, this.selectedFile).subscribe({
      next: () => {
        this.snackBar.open('Image uploaded successfully', 'Close', { duration: 2000 });
        this.closeDialog(true);
      },
      error: (error) => {
        console.error('Error uploading image:', error);
        this.snackBar.open('Task saved but image upload failed', 'Close', { duration: 3000 });
        this.closeDialog(true);
      }
    });
  }

  closeDialog(result: boolean = false): void {
    this.isSubmitting = false;
    this.dialogRef.close(result);
  }

  markFormGroupTouched(): void {
    Object.keys(this.taskForm.controls).forEach(key => {
      const control = this.taskForm.get(key);
      control?.markAsTouched();
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.taskForm.get(controlName);
    if (control?.hasError('required')) {
      return `${controlName.charAt(0).toUpperCase() + controlName.slice(1)} is required`;
    }
    if (control?.hasError('maxlength')) {
      const maxLength = control.errors?.['maxlength'].requiredLength;
      return `${controlName.charAt(0).toUpperCase() + controlName.slice(1)} cannot exceed ${maxLength} characters`;
    }
    return '';
  }

  get dialogTitle(): string {
    return this.data.mode === 'create' ? 'Create New Task' : 'Edit Task';
  }

  get submitButtonText(): string {
    if (this.isSubmitting) {
      return this.data.mode === 'create' ? 'Creating...' : 'Updating...';
    }
    return this.data.mode === 'create' ? 'Create Task' : 'Update Task';
  }
}
