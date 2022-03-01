import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MatTableDataSource } from '@angular/material';
import { UserService } from 'src/app/services/user.service';
import { UserEditorComponent } from '../user-editor/user-editor.component';
import { User } from 'src/app/models/user';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { InviteUserComponent } from '../invite-user/invite-user.component';

@Component({
  selector: 'app-admin-dashboard-page',
  templateUrl: './admin-dashboard-page.component.html',
  styleUrls: ['./admin-dashboard-page.component.css']
})
export class AdminDashboardPageComponent implements OnInit {

  dataSource = new MatTableDataSource();
  prev: string;
  next: string;
  userEditorDialog: MatDialogRef<UserEditorComponent>;
  inviteUserDialog: MatDialogRef<InviteUserComponent>;

  constructor(private userService: UserService, private dialog: MatDialog, private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.userService.paginatedUsersObservable.subscribe(data => {
        this.dataSource = new MatTableDataSource(data.data);
        this.prev = data.prev;
        this.next = data.next;
      });
    this.userService.loadUsers();
  }

  prevPage() {
    this.userService.loadPrevPage(this.prev);
  }

  nextPage() {
    this.userService.loadNextPage(this.next);
  }

  openDialog(user: User) {
    this.userEditorDialog = this.dialog.open(UserEditorComponent, {
      hasBackdrop: true,
      disableClose: true,
      data: user,
      autoFocus: true,
      width: "600px"
    });
  }

  openAddUserPopup() {
    this.openDialog(new User());
  }

  deleteUser(user: User) {
    if (confirm("Are you sure you want to delete this user and all of it`s appartments ?")) {
      this.userService.deleteUser(user.userId)
        .subscribe(() => { },
        errors => {
          alert(errors.error.errors);
        });
    }
  }

  unblockUser(user: User) {
    if (confirm("Are you sure you want to unblock this user ?")) {
      this.authenticationService.unblockUser(user.userId)
        .subscribe(() => this.userService.loadUsers(),
        errors => {
          alert(errors.error.errors);
        });
    }
  }

  openInviteUserDialog() {
    this.inviteUserDialog = this.dialog.open(InviteUserComponent, {
      hasBackdrop: true,
      disableClose: true,
      data: {},
      autoFocus: true,
      width: "600px"
    });
  }

}
