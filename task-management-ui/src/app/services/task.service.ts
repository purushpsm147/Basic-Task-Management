import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JiraTask, JiraTaskStatus, SortDirection, CreateTaskRequest, UpdateTaskRequest } from '../models/jira-task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly baseUrl = 'https://localhost:7094/api/task'; // Update this to match your backend URL
  private tasksSubject = new BehaviorSubject<JiraTask[]>([]);
  public tasks$ = this.tasksSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Get all tasks
  getTasks(): Observable<JiraTask[]> {
    return this.http.get<JiraTask[]>(this.baseUrl).pipe(
      tap(tasks => this.tasksSubject.next(tasks))
    );
  }

  // Get task by ID
  getTaskById(id: number): Observable<JiraTask> {
    return this.http.get<JiraTask>(`${this.baseUrl}/${id}`);
  }

  // Get tasks by status
  getTasksByStatus(status: JiraTaskStatus): Observable<JiraTask[]> {
    const params = new HttpParams().set('status', status.toString());
    return this.http.get<JiraTask[]>(`${this.baseUrl}/search`, { params });
  }

  // Sort tasks
  getSortedTasks(direction: SortDirection = SortDirection.Asc, prop: string = 'Name'): Observable<JiraTask[]> {
    const params = new HttpParams()
      .set('direction', direction.toString())
      .set('prop', prop);
    return this.http.get<JiraTask[]>(`${this.baseUrl}/sort`, { params }).pipe(
      tap(tasks => this.tasksSubject.next(tasks))
    );
  }

  // Create task
  createTask(task: CreateTaskRequest): Observable<JiraTask> {
    return this.http.post<JiraTask>(this.baseUrl, task).pipe(
      tap(() => this.refreshTasks())
    );
  }

  // Update task
  updateTask(id: number, task: UpdateTaskRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, task).pipe(
      tap(() => this.refreshTasks())
    );
  }

  // Delete task
  deleteTask(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`).pipe(
      tap(() => this.refreshTasks())
    );
  }

  // Upload image for task
  uploadTaskImage(id: number, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.baseUrl}/${id}/image`, formData).pipe(
      tap(() => this.refreshTasks())
    );
  }

  // Helper method to refresh tasks
  private refreshTasks(): void {
    this.getTasks().subscribe();
  }

  // Get task status options
  getTaskStatusOptions(): Array<{value: JiraTaskStatus, label: string}> {
    return [
      { value: JiraTaskStatus.Unassigned, label: 'Unassigned' },
      { value: JiraTaskStatus.Approved, label: 'Approved' },
      { value: JiraTaskStatus.InProgress, label: 'In Progress' },
      { value: JiraTaskStatus.Done, label: 'Done' }
    ];
  }

  // Get sort direction options
  getSortDirectionOptions(): Array<{value: SortDirection, label: string}> {
    return [
      { value: SortDirection.Asc, label: 'Ascending' },
      { value: SortDirection.Desc, label: 'Descending' }
    ];
  }

  // Get sortable properties
  getSortableProperties(): Array<{value: string, label: string}> {
    return [
      { value: 'Name', label: 'Name' },
      { value: 'Description', label: 'Description' },
      { value: 'Deadline', label: 'Deadline' },
      { value: 'Status', label: 'Status' },
      { value: 'IsFavorite', label: 'Favorite' }
    ];
  }
}
