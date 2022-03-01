import { Component, OnInit } from '@angular/core';
import { ApartmentFilter } from 'src/app/models/apartment-filter';

@Component({
  selector: 'app-apartment-list-page',
  templateUrl: './apartment-list-page.component.html',
  styleUrls: ['./apartment-list-page.component.css']
})
export class ApartmentListPageComponent implements OnInit {

  filter = new ApartmentFilter();

  constructor() { }

  ngOnInit() {
  }

  updateFilters(filter: ApartmentFilter) {
    this.filter = filter;
  }

}
