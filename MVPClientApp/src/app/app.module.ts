import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AgmCoreModule } from '@agm/core'

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { ApartmentListComponent } from './components/apartment-list/apartment-list.component';
import { ApartmentListPageComponent } from './components/apartment-list-page/apartment-list-page.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ApartmentDetailsPageComponent } from './components/apartment-details-page/apartment-details-page.component';
import { UserRegistrationPageComponent } from './components/user-registration-page/user-registration-page.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthenticationService } from './services/authentication.service';
import { HeaderComponent } from './components/header/header.component';
import { FiltersComponent } from './components/filters/filters.component';
import { LoginComponent } from './components/login/login.component';
import { EmailConfirmationComponent } from './components/email-confirmation/email-confirmation.component';
import { AuthenticationGuard } from './authentication.guard';
import { AppInterceptor } from './app.interceptor';
import { AuthServiceConfig, GoogleLoginProvider, FacebookLoginProvider, SocialLoginModule } from 'angularx-social-login';
import { RealtorDashboardPageComponent } from './components/realtor-dashboard-page/realtor-dashboard-page.component';
import { AdminDashboardPageComponent } from './components/admin-dashboard-page/admin-dashboard-page.component';
import { MatTableModule, MatDialog, MatDialogModule, MatSelectModule } from '@angular/material';
import { CdkTableModule } from '@angular/cdk/table';
import { UserService } from './services/user.service';
import { ApartmentEditorComponent } from './components/apartment-editor/apartment-editor.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MapViewPageComponent } from './components/map-view-page/map-view-page.component';
import { MapService } from './services/map.service';
import { AdminDashboardApartmentsPageComponent } from './components/admin-dashboard-apartments-page/admin-dashboard-apartments-page.component';
import { UserEditorComponent } from './components/user-editor/user-editor.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ProfileEditorComponent } from './components/profile-editor/profile-editor.component';
import { InviteUserComponent } from './components/invite-user/invite-user.component';

let config = new AuthServiceConfig([
  {
    id: GoogleLoginProvider.PROVIDER_ID,
    provider: new GoogleLoginProvider("484219155688-gui1o1cgsuob9gtnfdcou6bgnj3k4k08.apps.googleusercontent.com")
  },
  {
    id: FacebookLoginProvider.PROVIDER_ID,
    provider: new FacebookLoginProvider("2779230022109792")
  }
]);

export function provideConfig() {
  return config;
}

@NgModule({
  declarations: [
    AppComponent,
    ApartmentListComponent,
    ApartmentListPageComponent,
    ApartmentDetailsPageComponent,
    UserRegistrationPageComponent,
    HeaderComponent,
    FiltersComponent,
    LoginComponent,
    EmailConfirmationComponent,
    RealtorDashboardPageComponent,
    AdminDashboardPageComponent,
    ApartmentEditorComponent,
    MapViewPageComponent,
    AdminDashboardApartmentsPageComponent,
    UserEditorComponent,
    ResetPasswordComponent,
    ProfileEditorComponent,
    InviteUserComponent
  ],
  entryComponents: [
    ApartmentEditorComponent,
    UserEditorComponent,
    ProfileEditorComponent,
    InviteUserComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    HttpClientModule,
    SocialLoginModule,
    MatTableModule,
    CdkTableModule,
    MatDialogModule,
    MatSelectModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', component: ApartmentListPageComponent, canActivate: [AuthenticationGuard] },
      { path: 'apartments/:id', component: ApartmentDetailsPageComponent, canActivate: [AuthenticationGuard] },
      { path: 'realtorDashboard', component: RealtorDashboardPageComponent },
      { path: 'adminDashboard', component: AdminDashboardPageComponent, canActivate: [AuthenticationGuard] },
      { path: 'adminDashboardApartments', component: AdminDashboardApartmentsPageComponent, canActivate:  [AuthenticationGuard] },
      { path: 'mapView', component: MapViewPageComponent, canActivate: [AuthenticationGuard] },
      { path: 'register', component: UserRegistrationPageComponent },
      { path: 'login', component: LoginComponent },
      { path: 'emailConfirmation', component: EmailConfirmationComponent },
      { path: 'resetPassword', component: ResetPasswordComponent }
    ]),
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyCB8fJaPbKN3hJGrwjO2ptdu9-8ylrE0TQ'
    })
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AppInterceptor, multi: true },
    { provide: AuthServiceConfig, useFactory: provideConfig },
    UserService,
    AuthenticationService,
    MapService,
    AuthenticationGuard,
    MatDialog
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
