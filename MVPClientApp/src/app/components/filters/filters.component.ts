import { Component, OnInit, Output } from '@angular/core';
import { ApartmentFilter } from 'src/app/models/apartment-filter';
import { EventEmitter } from '@angular/core';

@Component({
  selector: 'app-filters',
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.css']
})
export class FiltersComponent implements OnInit {

  @Output()
  filterUpdated = new EventEmitter<ApartmentFilter>();
  filter = new ApartmentFilter();

  constructor() { }

  ngOnInit() {}

  onSubmit() {
    var newFilter = new ApartmentFilter();
    newFilter.minArea = this.filter.minArea;
    newFilter.maxArea = this.filter.maxArea;
    newFilter.minRooms = this.filter.minRooms;
    newFilter.maxRooms = this.filter.maxRooms;
    newFilter.minPrice = this.filter.minPrice;
    newFilter.maxPrice = this.filter.maxPrice;
    this.filterUpdated.next(newFilter);
  }

}
