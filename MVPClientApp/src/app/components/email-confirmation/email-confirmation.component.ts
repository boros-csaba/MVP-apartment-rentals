import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styleUrls: ['./email-confirmation.component.css']
})
export class EmailConfirmationComponent implements OnInit {

  token: string;
  userId: string;
  loading = false;
  errors: string[];

  constructor(private route: ActivatedRoute, private authenticationService: AuthenticationService, private router: Router) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      this.userId = params['userId'];
    });
  }

  confirmEmailAddress() {
    this.loading = true;
    this.authenticationService.confirmEmailAddress(this.userId, this.token)
      .pipe(first())
      .subscribe(() => {
        this.router.navigate(["/login"], { queryParams: { emailConfirmed: true } });
      },
      errorResponse => {
        this.errors = errorResponse.error.errors;
        this.loading = false;
      })
  }

}
