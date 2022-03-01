import { Component, OnInit, Input } from '@angular/core';
import { ApartmentService } from 'src/app/services/apartment.service';
import { Apartment } from 'src/app/models/apartment';
import { PaginatedData } from 'src/app/models/paginated-data';
import { ApartmentFilter } from 'src/app/models/apartment-filter';

@Component({
  selector: 'app-apartment-list',
  templateUrl: './apartment-list.component.html',
  styleUrls: ['./apartment-list.component.css'],
  providers: [ApartmentService]
})
export class ApartmentListComponent implements OnInit {

  limit = 5;
  apartmentsData: PaginatedData<Apartment>;
  
  @Input() 
  set filter(filter: ApartmentFilter) {
    this.apartmentService.getAvailableApartments(this.limit, filter)
      .subscribe(data => this.apartmentsData = data);
  }

  constructor(private apartmentService: ApartmentService) { }

  ngOnInit() {
    this.apartmentService.getAvailableApartments(this.limit, this.filter ? this.filter : new ApartmentFilter())
      .subscribe(data => this.apartmentsData = data);
    this.apartmentService.updatedObservable
      .subscribe(() => {
        this.apartmentService.getAvailableApartments(this.limit, this.filter ? this.filter : new ApartmentFilter())
            .subscribe(data => this.apartmentsData = data);
      });
  }

  prevPage() {
    this.apartmentService.loadPage(this.apartmentsData.prev)
      .subscribe(data => this.apartmentsData = data);
  }

  nextPage() {
    this.apartmentService.loadPage(this.apartmentsData.next)
      .subscribe(data => this.apartmentsData = data);
  }

}
