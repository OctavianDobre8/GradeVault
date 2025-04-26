import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private apiUrl = `api/classes`;

  constructor(private http: HttpClient) { }

  getTeacherClasses(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/teacher`);
  }

  getClass(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createClass(classData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, classData);
  }

  updateClass(id: string, classData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, classData);
  }

  deleteClass(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}