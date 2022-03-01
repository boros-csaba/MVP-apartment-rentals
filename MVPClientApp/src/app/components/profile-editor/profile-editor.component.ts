import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { LoginInformation } from 'src/app/models/login-information';
import { UserService } from 'src/app/services/user.service';
import { MatDialogRef } from '@angular/material';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-profile-editor',
  templateUrl: './profile-editor.component.html',
  styleUrls: ['./profile-editor.component.css']
})
export class ProfileEditorComponent implements OnInit {

  form: FormGroup;
  user: LoginInformation;
  profileImage: string;
  uplaodedFile: any = null;
  submitted = false;
  errors: string[];
  loading = false;

  constructor(private dialogRef: MatDialogRef<ProfileEditorComponent>,
    private authenticationService: AuthenticationService,
    private formBuilder: FormBuilder,
    private userService: UserService) { }

  ngOnInit() {
    this.user = this.authenticationService.getLoginInformation();
    this.form = this.formBuilder.group({
      firstName: [this.user.firstName, Validators.required],
      lastName: [this.user.lastName, Validators.required]
    });
    this.userService.getUserProfileImage(this.user.userId)
      .subscribe(response => this.profileImage = response.base64Image);
  }

  close() {
    this.dialogRef.close();
  }

  onFileSelected(event) {
    this.uplaodedFile = event.target.files[0];
    if (this.uplaodedFile) {
      this.userService.uploadProfileImage(this.user.userId, this.uplaodedFile)
        .subscribe(response => {
          this.profileImage = response.base64Image;
        },
          error => {
            this.errors = error.error.errors;
            this.loading = false;
          })
    }
  }

  save() {
    this.submitted = true;
    this.errors = null;
    if (this.form.invalid) {
      return;
    }
    this.loading = true;

    var updatedUser = new User();
    updatedUser.userId = this.user.userId;
    updatedUser.firstName = this.form.value.firstName;
    updatedUser.lastName = this.form.value.lastName;

    this.userService.updateUser(updatedUser, false)
      .subscribe(() => {
        this.user.firstName = updatedUser.firstName;
        this.user.lastName = updatedUser.lastName;
        this.authenticationService.setLoggedInUser(this.user);
        this.close();
      },
      error => {
        this.errors = error.error.errors;
        this.loading = false;
      });
  }

}
