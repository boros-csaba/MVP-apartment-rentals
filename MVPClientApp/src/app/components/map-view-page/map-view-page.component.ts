import { Component, OnInit } from '@angular/core';
import { ApartmentService } from 'src/app/services/apartment.service';
import { ApartmentFilter } from 'src/app/models/apartment-filter';
import { PaginatedData } from 'src/app/models/paginated-data';
import { Apartment } from 'src/app/models/apartment';

@Component({
  selector: 'app-map-view-page',
  templateUrl: './map-view-page.component.html',
  styleUrls: ['./map-view-page.component.css'],
  providers: [ApartmentService]
})
export class MapViewPageComponent implements OnInit {

  apartmentsData: PaginatedData<Apartment>;
  filter = new ApartmentFilter();

  constructor(private apartmentService: ApartmentService) { }

  ngOnInit() {
    this.apartmentService.getAvailableApartments(0, this.filter)
      .subscribe(data => this.apartmentsData = data);
      this.apartmentService.updatedObservable
      .subscribe(() => {
        this.apartmentService.getAvailableApartments(0, this.filter)
            .subscribe(data => this.apartmentsData = data);
      });
  }

  updateFilters(filter: ApartmentFilter) {
    this.filter = filter;
    this.apartmentService.getAvailableApartments(0, this.filter)
      .subscribe(data => this.apartmentsData = data);
  }

}
