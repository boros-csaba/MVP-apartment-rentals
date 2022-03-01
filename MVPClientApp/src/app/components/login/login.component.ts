import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthenticationService } from '../../services/authentication.service';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService, GoogleLoginProvider, FacebookLoginProvider } from 'angularx-social-login';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  returnUrl: string;
  errors: string[];
  success: string;
  loading = false;
  submitted = false;
  emailConfirmed = false;
  userId: string;

  constructor(
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute,
    private socialAuthService: AuthService) { }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.route.queryParams.subscribe(params => {
      this.returnUrl = params['returnUrl'] || '/';
      if (params['registered']) this.success = 'Registration competed successfully, please check you inbox for the confirmation email!';
      else if (params['emailConfirmed']) this.success = 'Your email address was successfully verified, now you can log in!';
      else if (params['passwordChanged']) this.success = 'Your password was changed, now you can log in!';
      else this.success = null;
    });
  }

  onSubmit() {
    this.submitted = true;
    this.success = null;
    if (this.loginForm.invalid) {
      return;
    }
    this.loading = true;

    this.authenticationService
      .login(this.loginForm.controls.email.value, this.loginForm.controls.password.value)
      .pipe(first())
      .subscribe(
        () => {
          this.router.navigate([this.returnUrl]);
        },
        errorResponse => this.handleLoginErrors(errorResponse));
  }

  requestNewConfirmationEmail() {
    this.loading = true;
    this.authenticationService
      .requestNewConfirmationEmail(this.userId)
      .pipe(first())
      .subscribe(
        () => {
          this.success = "Confirmation email was sent again!";
          this.errors = null;
          this.loading = false;
        },
        errorResponse => {
          this.errors = errorResponse.error.errors;
          this.success = null;
          this.loading = false;
        })
  }

  loginWithGoogle() {
    let socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;
    this.socialAuthService.signIn(socialPlatformProvider)
      .then((userData) => {
        this.authenticationService.loginWithProvider("Google", userData.idToken, null)
          .pipe(first())
          .subscribe(
            response => this.handleExternalLoginResponse(response),
            errorResponse => this.handleLoginErrors(errorResponse));
      });
  }

  loginWithFacebook() {
    let socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;
    this.socialAuthService.signIn(socialPlatformProvider)
      .then((userData) => {
        this.authenticationService.loginWithProvider("Facebook", userData.authToken, userData.photoUrl)
          .pipe(first())
          .subscribe(
            response => { this.handleExternalLoginResponse(response) },
            errorResponse => this.handleLoginErrors(errorResponse));
      });
  }

  handleExternalLoginResponse(response) {
    this.authenticationService.setLoggedInUser(response);
    this.router.navigate([this.returnUrl]);
  }

  handleLoginErrors(errorResponse) {
    this.errors = errorResponse.error.errors;
    this.emailConfirmed = errorResponse.error.emailConfirmed;
    this.userId = errorResponse.error.userId;
    this.loading = false;
  }
}
