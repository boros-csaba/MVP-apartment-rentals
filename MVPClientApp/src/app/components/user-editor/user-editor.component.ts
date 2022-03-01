import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef} from "@angular/material";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';
@Component({
  selector: 'app-user-editor',
  templateUrl: './user-editor.component.html',
  styleUrls: ['./user-editor.component.css']
})
export class UserEditorComponent implements OnInit {

  form: FormGroup;
  roles = ["Admin", "Realtor", "Client"];
  errors: string[];
  loading = false;
  submitted = false;

  constructor(
    private dialogRef: MatDialogRef<UserEditorComponent>,
    private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public user: User,
    private userService: UserService) {
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      email: [this.user.email, [Validators.email, Validators.required]],
      firstName: [this.user.firstName, Validators.required],
      lastName: [this.user.lastName, Validators.required],
      roles: [this.user.roles, Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      passwordConfirmation: ['', Validators.required],
    });
  }

  save() {
    this.submitted = true;
    this.errors = null;
    if (this.user.userId != null) {{
        this.form.controls.password.setErrors(null);
        this.form.controls.passwordConfirmation.setErrors(null);
        this.form.controls.email.setErrors(null);
    }}
    if (this.form.invalid) {
      return;
    }
    this.loading = true;

    var user = this.user;
    user.email = this.form.value.email;
    user.firstName = this.form.value.firstName;
    user.lastName = this.form.value.lastName;
    user.roles = this.form.value.roles;
    user.password = this.form.value.password;
    user.confirmedPassword = this.form.value.passwordConfirmation;
    if (user.userId == null) {
      this.userService.createNewUser(user)
        .subscribe(() => {
          this.userService.loadUsers();
          this.close();
        },
        errors => {
          console.log(errors);
          this.errors = errors.error.errors;
          this.loading = false;
        });
    }
    else {
      this.userService.updateUser(user)
        .subscribe(() => {
          this.userService.loadUsers();
          this.close();
        },
        errors => {
          console.log(errors);
          this.errors = errors.error.errors;
          this.loading = false;
        });
    }
  }

  close() {
    this.dialogRef.close();
  }
}
