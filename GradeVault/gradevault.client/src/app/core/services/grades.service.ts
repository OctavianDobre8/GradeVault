import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Grade } from '../../shared/models/grade.model';

@Injectable({
  providedIn: 'root',
})
export class GradesService {
  private apiUrl = '/api/grades';

  constructor(private http: HttpClient) {}

  public getMyGrades(): Observable<Grade[]> {
    return this.http.get<Grade[]>(`${this.apiUrl}/my-grades`);
  }
}
