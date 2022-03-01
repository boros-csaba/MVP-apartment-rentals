import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Apartment } from '../models/apartment';
import { environment } from 'src/environments/environment';
import { PaginatedData } from '../models/paginated-data';
import { ApartmentFilter } from '../models/apartment-filter';
import { map } from 'rxjs/operators';
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class ApartmentService {

  private updated = new BehaviorSubject<number>(0);
  updatedObservable = this.updated.asObservable();

  constructor(private http: HttpClient, private authenticationService: AuthenticationService) { }

  getAvailableApartments(limit: number, filter: ApartmentFilter): Observable<PaginatedData<Apartment>> {
    var urlParts = new Array();
    if (limit > 0) {
      urlParts.push("limit=" + limit);
    }
    urlParts.push("onlyAvailable=true");
    if (filter.minArea) urlParts.push("minArea=" + filter.minArea);
    if (filter.maxArea) urlParts.push("maxArea=" + filter.maxArea);
    if (filter.minRooms) urlParts.push("minRooms=" + filter.minRooms);
    if (filter.maxRooms) urlParts.push("maxRooms=" + filter.maxRooms);
    if (filter.minPrice) urlParts.push("minPrice=" + filter.minPrice);
    if (filter.maxPrice) urlParts.push("maxPrice=" + filter.maxPrice);
    var url = environment.baseUrl + "/apartments?" + urlParts.join("&");
    return this.http.get<PaginatedData<Apartment>>(url);
  }

  getOwnApartments(limit: number): Observable<PaginatedData<Apartment>>  {
    var urlParts = new Array();
    if (limit > 0) {
      urlParts.push("limit=" + limit);
    }
    var user = this.authenticationService.getLoginInformation();
    var url = environment.baseUrl + "/users/" + user.userId + "/apartments?" + urlParts.join("&");
    return this.http.get<PaginatedData<Apartment>>(url);
  }

  getAllApartments(limit: number): Observable<PaginatedData<Apartment>>  {
    var urlParts = new Array();
    if (limit > 0) {
      urlParts.push("limit=" + limit);
    }
    var url = environment.baseUrl + "/apartments?" + urlParts.join("&");
    return this.http.get<PaginatedData<Apartment>>(url);
  }

  loadPage(url: string): Observable<PaginatedData<Apartment>> {
    if (url != null) {
      return this.http.get<PaginatedData<Apartment>>(url);
    }
  }

  getApartmentById(apartmentId: number): Observable<Apartment> {
      return this.http.get<Apartment>(environment.baseUrl + "/apartments/" + apartmentId);
  }

  createNewApartment(apartment: Apartment): Observable<any> {
    return this.http.post(environment.baseUrl + "/apartments", apartment, environment.httpOptions)
      .pipe(map(() => this.updated.next(this.updated.value + 1)));
  }

  updateApartment(apartment: Apartment): Observable<any> {
    return this.http.put(environment.baseUrl + "/apartments/" + apartment.id, apartment, environment.httpOptions)
      .pipe(map(() => this.updated.next(this.updated.value + 1)));
  }

  deleteApartment(apartmentId: string): Observable<any> {
    return this.http.delete(environment.baseUrl + "/apartments/" + apartmentId, environment.httpOptions)
      .pipe(map(() => this.updated.next(this.updated.value + 1)));
  }
}
