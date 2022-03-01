import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from './services/authentication.service';

@Injectable()
export class AuthenticationGuard implements CanActivate {

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService) { }

  canActivate(_next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    const loginInformation = this.authenticationService.getLoginInformation();
    if (loginInformation) {
      return true;
    }
    this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
    return false;
  }
}
