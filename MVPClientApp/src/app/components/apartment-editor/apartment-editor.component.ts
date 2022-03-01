import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { HttpClient } from '@angular/common/http';
import { first } from 'rxjs/operators';
import { Apartment } from 'src/app/models/apartment';
import { ApartmentService } from 'src/app/services/apartment.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-apartment-editor',
  templateUrl: './apartment-editor.component.html',
  styleUrls: ['./apartment-editor.component.css']
})
export class ApartmentEditorComponent implements OnInit {

  form: FormGroup;
  address: string = "";
  addressError: string;
  loading = false;
  submitted = false;
  errors: string[];
  users: User[];
  
  constructor(private dialogRef: MatDialogRef<ApartmentEditorComponent>,
              private formBuilder: FormBuilder, 
              @Inject(MAT_DIALOG_DATA) private data, 
              private http: HttpClient, 
              private apartmentService: ApartmentService, 
              private authenticationService: AuthenticationService,
              private userService: UserService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      name: [this.data.apartment.name, Validators.required],
      floorAreaSize: [this.data.apartment.floorAreaSize, [Validators.min(1), Validators.required]],
      pricePerMonth: [this.data.apartment.pricePerMonth, Validators.required],
      numberOfRooms: [this.data.apartment.numberOfRooms, [Validators.min(1), Validators.required]],
      isAvailable: [this.data.apartment.isAvailable],
      longitude: [this.data.apartment.longitude, [Validators.min(-180), Validators.max(180), Validators.required]],
      latitude: [this.data.apartment.latitude, [Validators.min(-90), Validators.max(90), Validators.required]],
      description: [this.data.apartment.description],
      userId: this.data.apartment.realtorUserId
    });
    if (this.data.asAdmin) {
      this.userService.getAllUsers()
        .subscribe(data => { 
          this.users = data.data;
          var user = this.users.filter(u => u.userId == this.data.apartment.realtorUserId)[0];
          this.form.controls['userId'].setValue(user);
        });
    }
  }

  searchAddress() {
    this.http.get("https://cors-anywhere.herokuapp.com/https://maps.googleapis.com/maps/api/geocode/json?key=***" + this.address)
      .pipe(first())
      .subscribe(response => {
        var location = response as any;
        if (location != null && location.results != null) {
          location = location.results[0];
          if (location != null && location.geometry != null) {
            var geometry = location.geometry;
            this.form.controls['latitude'].setValue(geometry.location.lat);
            this.form.controls['longitude'].setValue(geometry.location.lng);
            this.addressError = null;
            return;
          }
        }
        this.addressError = "Invalid address";
      },
      error => {
        this.addressError = error.toString();
      });
  }

  save() {
    this.submitted = true;
    this.errors = null;
    if (this.form.invalid) {
      return;
    }
    this.loading = true;

    var apartment = new Apartment();
    apartment.id = this.data.apartment.id;
    apartment.name = this.form.value.name;
    apartment.floorAreaSize = this.form.value.floorAreaSize;
    apartment.pricePerMonth = this.form.value.pricePerMonth;
    apartment.numberOfRooms = this.form.value.numberOfRooms;
    apartment.isAvailable = this.form.value.isAvailable;
    apartment.longitude = this.form.value.longitude;
    apartment.latitude = this.form.value.latitude;
    apartment.description = this.form.value.description;
    if (this.form.value.userId && this.form.value.userId.userId) {
      apartment.realtorUserId = this.form.value.userId.userId;
    }
    else {
      apartment.realtorUserId = this.authenticationService.getLoginInformation().userId;
    }

    if (apartment.id == null) {
      this.apartmentService.createNewApartment(apartment)
        .subscribe(() => {
          this.close();
        },
        errors => {
          console.log(errors);
          this.errors = errors.error.errors;
          this.loading = false;
        });
    }
    else {
      this.apartmentService.updateApartment(apartment)
        .subscribe(() => {
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
