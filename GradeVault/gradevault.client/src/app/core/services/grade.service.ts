import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GradeService {
  private apiUrl = 'api/grades'; // Using a relative URL for .NET Core integration

  constructor(private http: HttpClient) { }

  getGradesByAssignment(assignmentId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/assignment/${assignmentId}`);
  }

  getGradesByStudent(studentId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/student/${studentId}`);
  }

  getGradesByClass(classId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/class/${classId}`);
  }

  createGrade(gradeData: any): Observable<any> {
    // Convert string IDs to numbers
    const payload = {
      studentId: parseInt(gradeData.studentId),
      assignmentId: parseInt(gradeData.assignmentId),
      score: gradeData.score
    };
    return this.http.post<any>(this.apiUrl, payload);
  }

  updateGrade(id: string, gradeData: any): Observable<any> {
    // Convert string IDs to numbers
    const payload = {
      score: gradeData.score
    };
    return this.http.put<any>(`${this.apiUrl}/${id}`, payload);
  }

  deleteGrade(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}