import { Injectable } from '@angular/core';
import { Registration } from '../models/registration';
import { Observable, BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { LoginInformation } from '../models/login-information';
import { Router } from '@angular/router';
import { PasswordReset } from '../models/password-reset';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  loginInformationSubject: BehaviorSubject<LoginInformation>;

  constructor(private http: HttpClient, private router: Router) {
    this.loginInformationSubject = new BehaviorSubject<LoginInformation>(JSON.parse(localStorage.getItem('LoginInformation')));
  }

  getLoginInformation(): LoginInformation {
    return this.loginInformationSubject.value;
  }

  register(registration: Registration): Observable<any> {
    return this.http.post(environment.baseUrl + "/authentication/registration", registration, environment.httpOptions);
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post(environment.baseUrl + "/authentication/login", { Email: email, Password: password }, environment.httpOptions)
      .pipe(map(response => {
        var loginResponse = response as LoginInformation;
        this.setLoggedInUser(loginResponse);
      }));
  }

  refreshToken(): Observable<any> {
    var loginInformation = this.getLoginInformation();
    return this.http.post<LoginInformation>(environment.baseUrl + "/authentication/refreshToken", {
      token: loginInformation.token,
      refreshToken: loginInformation.refreshToken
    }).pipe(map((response) => {
      var loginResponse = response as LoginInformation;
      this.setLoggedInUser(loginResponse);
    }));
  }

  setLoggedInUser(loginInformation: LoginInformation) {
    localStorage.setItem('LoginInformation', JSON.stringify(loginInformation));
    this.loginInformationSubject.next(loginInformation);
  }

  logout() {
    localStorage.removeItem('User');
    this.loginInformationSubject.next(null);
    this.router.navigate(["/login"]);
  }

  confirmEmailAddress(userId: string, token: string): Observable<any> {
    return this.http.post(environment.baseUrl + "/authentication/emailConfirmation", { UserId: userId, Token: token }, environment.httpOptions);
  }

  requestNewConfirmationEmail(userId: string): Observable<any> {
    return this.http.post(environment.baseUrl + "/authentication/confirmationEmailRequest", { UserId: userId }, environment.httpOptions);
  }

  loginWithProvider(provider: string, token: string, photoUrl: string): Observable<any> {
    return this.http.post(environment.baseUrl + "/authentication/externalLogin", { Provider: provider, Token: token, PhotoUrl: photoUrl }, environment.httpOptions);
  }

  resetPassword(passwordReset: PasswordReset): Observable<any> {
    return this.http.post(environment.baseUrl + "/authentication/passwordReset", passwordReset, environment.httpOptions)
  }

  unblockUser(userId: string): Observable<any> {
    return this.http.delete(environment.baseUrl + "/authentication/" + userId + "/block", environment.httpOptions);
  }

  sendInvitation(email: string): Observable<any> {
    return this.http.post(environment.baseUrl + "/authentication/invitations", { email: email }, environment.httpOptions);
  }
}
