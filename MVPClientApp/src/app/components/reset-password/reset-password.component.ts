import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { PasswordReset } from 'src/app/models/password-reset';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {

  resetPasswordForm: FormGroup;
  errors: string[];
  loading = false;
  submitted = false;
  token: string;
  userId: string;

  constructor(
    private formBuilder: FormBuilder, 
    private authenticationService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.resetPasswordForm = this.formBuilder.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmedPassword: ['', Validators.required]
    });
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      this.userId = params['userId'];
    });
  }

  onSubmit() {
    this.submitted = true;
    if (this.resetPasswordForm.invalid) {
      return;
    }
    this.loading = true;

    var passwordReset = new PasswordReset();
    passwordReset.userId = this.userId;
    passwordReset.token = this.token;
    passwordReset.password = this.resetPasswordForm.controls.password.value;
    passwordReset.confirmedPassword = this.resetPasswordForm.controls.confirmedPassword.value;

    this.authenticationService
      .resetPassword(passwordReset)
      .pipe(first())
      .subscribe(
        () => {
          this.router.navigate(['/login'], { queryParams: { passwordChanged: true } });
        },
        errorResponse => {
          this.errors = errorResponse.error.errors;
          this.loading = false;
        });
  }

}
