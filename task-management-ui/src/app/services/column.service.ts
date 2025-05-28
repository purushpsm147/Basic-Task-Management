import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Column, CreateColumnRequest, UpdateColumnRequest } from '../models/column.model';

@Injectable({
  providedIn: 'root'
})
export class ColumnService {
  private readonly baseUrl = 'https://localhost:7094/api/columns';
  private columnsSubject = new BehaviorSubject<Column[]>([]);
  public columns$ = this.columnsSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Get all columns
  getColumns(): Observable<Column[]> {
    return this.http.get<Column[]>(this.baseUrl).pipe(
      tap(columns => this.columnsSubject.next(columns))
    );
  }

  // Get column by ID
  getColumnById(id: number): Observable<Column> {
    return this.http.get<Column>(`${this.baseUrl}/${id}`);
  }

  // Create column
  createColumn(column: CreateColumnRequest): Observable<Column> {
    return this.http.post<Column>(this.baseUrl, column).pipe(
      tap(() => this.refreshColumns())
    );
  }

  // Update column
  updateColumn(id: number, column: UpdateColumnRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, column).pipe(
      tap(() => this.refreshColumns())
    );
  }

  // Delete column
  deleteColumn(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`).pipe(
      tap(() => this.refreshColumns())
    );
  }

  // Helper method to refresh columns
  private refreshColumns(): void {
    this.getColumns().subscribe();
  }

  // Generate a random color for new columns
  generateRandomColor(): string {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }
}
