import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { Registration } from 'src/app/models/registration';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { first } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-registration-page',
  templateUrl: './user-registration-page.component.html',
  styleUrls: ['./user-registration-page.component.css']
})
export class UserRegistrationPageComponent implements OnInit {

  registerForm: FormGroup;
  errors: string[];
  loading = false;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder, 
    private authenticationService: AuthenticationService, 
    private router: Router) { }

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      email: ['', [Validators.email, Validators.required]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmedPassword: ['', Validators.required]
    });
  }

  onSubmit() {
    this.submitted = true;
    if (this.registerForm.invalid) {
      return;
    }
    this.loading = true;

    var registration = new Registration();
    registration.email = this.registerForm.controls.email.value;
    registration.firstName = this.registerForm.controls.firstName.value;
    registration.lastName = this.registerForm.controls.lastName.value;
    registration.password = this.registerForm.controls.password.value;
    registration.confirmedPassword = this.registerForm.controls.confirmedPassword.value;

    this.authenticationService
      .register(registration)
      .pipe(first())
      .subscribe(
        () => {
          this.router.navigate(['/login'], { queryParams: { registered: true } });
        },
        errorResponse => {
          this.errors = errorResponse.error.errors;
          this.loading = false;
        });
  }

}
