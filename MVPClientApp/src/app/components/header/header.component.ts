import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { LoginInformation } from 'src/app/models/login-information';
import { Router } from '@angular/router';
import { MatDialogRef, MatDialog } from '@angular/material';
import { ProfileEditorComponent } from '../profile-editor/profile-editor.component';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  loginInformation: LoginInformation;
  profileEditorDialog: MatDialogRef<ProfileEditorComponent>;

  constructor(private authetnticationService: AuthenticationService, 
              private router: Router, 
              private dialog: MatDialog,
              private userService: UserService) {
    authetnticationService.loginInformationSubject
      .subscribe(value => this.loginInformation = value);
  }

  ngOnInit() {
  }

  goHome() {
    this.router.navigate(["/"]);
  }

  goToRealtorDashboard() {
    this.router.navigate(["/realtorDashboard"]);
  }

  goToAdminDashboard() {
    this.router.navigate(["/adminDashboard"]);
  }

  goToMapView() {
    this.router.navigate(["/mapView"]);
  }

  logout() {
    this.authetnticationService.logout();
    this.router.navigate(["/login"]);
  }

  becomeRealtor() {
    if (confirm("Are you sure you want to become a realtor?")) {
      this.userService.becomeRealtor(this.loginInformation.userId, () => {
        alert("You need to log in again for the changes to apply!");
        this.authetnticationService.logout();
      });
    }
  }

  openProfileEditorDialog() {
    this.profileEditorDialog = this.dialog.open(ProfileEditorComponent, {
      hasBackdrop: true,
      disableClose: true,
      data: {},
      autoFocus: true,
      width: "600px"
    });
  }
}
