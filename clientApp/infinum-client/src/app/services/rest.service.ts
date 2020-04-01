import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { AppSettings } from '../settings/app-settings';

@Injectable({
  providedIn: 'root'
})
export class RestService {
  private baseUrl = "";

  constructor(private http: HttpClient) {
    this.baseUrl = AppSettings.serverUrl;
  }

  getRequest<T>(url) : Observable<T>{
    return this.http
    .get<T>(this.baseUrl + url)
    .pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  postRequest<T>(data, url) : Observable<T>{
    return this.http
    .post<T>(this.baseUrl + url, data )
    .pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  putRequest<T>(data, url) : Observable<T>{
    return this.http
    .put<T>(this.baseUrl + url, data )
    .pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  deleteRequest<T>(url) : Observable<T> {
    return this.http
    .delete<T>(this.baseUrl + url)
    .pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      return throwError(error.error.message);
    } else {
      return throwError(error.error);
    }
  }
}
