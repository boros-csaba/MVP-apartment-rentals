import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, map, filter, take } from 'rxjs/operators';
import { AuthenticationService } from './services/authentication.service';

@Injectable()
export class AppInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authenticationService: AuthenticationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    let loginInformation = this.authenticationService.getLoginInformation();
    if (loginInformation && loginInformation.token) {
      request = this.addToken(request, loginInformation.token);
    }

    return next.handle(request).pipe(catchError(err => {
      if (err.status === 401) {
        this.handle401Error(request, next);
      }
      return throwError(err);
    }));
  }

  private addToken(request: HttpRequest<any>, token: string) {
    return request.clone({
      setHeaders: {
        'Authorization': `Bearer ${token}`
      }
    });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);
      return this.authenticationService.refreshToken().pipe(map(response => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(response.Token);
          return next.handle(this.addToken(request, response.token));
        })).subscribe();

    } else {
      this.isRefreshing = false;
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        map(response => {
          this.isRefreshing = false;
          return next.handle(this.addToken(request, response));
        })).subscribe();
    }
  }
}
