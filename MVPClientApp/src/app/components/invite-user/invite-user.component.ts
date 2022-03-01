import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-invite-user',
  templateUrl: './invite-user.component.html',
  styleUrls: ['./invite-user.component.css']
})
export class InviteUserComponent implements OnInit {

  form: FormGroup;
  errors: string[];
  success = null;
  loading = false;
  submitted = false;

  constructor(private dialogRef: MatDialogRef<InviteUserComponent>,
              private formBuilder: FormBuilder,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      email: ["", [Validators.email, Validators.required]]
    });
  }

  save() {
    this.submitted = true;
    this.errors = null;
    if (this.form.invalid) {
      return;
    }
    this.loading = true;

    this.authenticationService.sendInvitation(this.form.value.email)
      .subscribe(() => {
          this.errors = null;
          this.loading = false;
          this.success = "Invitation was successfully sent!";
        },
        errors => {
          this.success = null;
          this.errors = errors.error.errors;
          this.loading = false;
        });
  }

  close() {
    this.dialogRef.close();
  }

}
