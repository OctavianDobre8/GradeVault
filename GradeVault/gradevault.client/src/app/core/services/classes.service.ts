import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Class } from '../../shared/models/class.model';

@Injectable({
  providedIn: 'root',
})
export class ClassesService {
  private apiUrl = '/api/grades';

  constructor(private http: HttpClient) {}

  getMyClasses(): Observable<Class[]> {
        console.log(
          'Attempting to fetch classes from: ' + `${this.apiUrl}/my-classes`
        );
    return this.http.get<Class[]>(`${this.apiUrl}/my-classes`);
  }
}
