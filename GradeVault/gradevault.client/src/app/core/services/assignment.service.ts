import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})

export class AssignmentService {
  private apiUrl = `api/assignments`;

  constructor(private http: HttpClient) { }

  getAssignmentsByClass(classId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/class/${classId}`);
  }

  getAssignment(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createAssignment(assignmentData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, assignmentData);
  }

  updateAssignment(id: string, assignmentData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, assignmentData);
  }

  deleteAssignment(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}