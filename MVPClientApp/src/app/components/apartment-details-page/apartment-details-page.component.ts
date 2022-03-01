import { Component, OnInit } from '@angular/core';
import { Apartment } from 'src/app/models/apartment';
import { ApartmentService } from 'src/app/services/apartment.service';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-apartment-details-page',
  templateUrl: './apartment-details-page.component.html',
  styleUrls: ['./apartment-details-page.component.css']
})
export class ApartmentDetailsPageComponent implements OnInit {
  
  apartment: Apartment;
  realtor: User;
  realtorProfileImage: string;

  constructor(private route: ActivatedRoute, private apartmentService: ApartmentService, private userService: UserService) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      this.apartmentService.getApartmentById(params['id'])
        .subscribe(data => {
          this.apartment = data;
          this.userService.getUser(this.apartment.realtorUserId)
            .subscribe(result => this.realtor = result);
          this.userService.getUserProfileImage(this.apartment.realtorUserId)
            .subscribe(result => this.realtorProfileImage = result.base64Image);
        })
    );
  }

}
