import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { PaginatedData } from '../models/paginated-data';
import { User } from '../models/user';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private paginatedUsers = new BehaviorSubject<PaginatedData<User>>(new PaginatedData());
  paginatedUsersObservable = this.paginatedUsers.asObservable();
  limit = 10;
  offset = 0;
  
  constructor(private http: HttpClient) { }

  loadUsers() {
    var urlParts = new Array();
    urlParts.push("limit=" + this.limit);
    urlParts.push("offset=" + this.offset);
    var url = environment.baseUrl + "/users?" + urlParts.join("&");
    this.loadUsersByUrl(url);
  }

  getAllUsers(): Observable<PaginatedData<User>> {
    return this.http.get<PaginatedData<User>>(environment.baseUrl + "/users");
  }

  loadPrevPage(url: string) {
    if (url != null) {
      this.loadUsersByUrl(url);
    }
  }

  loadNextPage(url: string) {
    if (url != null) {
      this.loadUsersByUrl(url);
    }
  }

  private loadUsersByUrl(url: string) {
    this.http.get<PaginatedData<User>>(url)
      .subscribe(data => this.paginatedUsers.next(data));
  }

  createNewUser(user: User): Observable<any> {
    return this.http.post(environment.baseUrl + "/users", user, environment.httpOptions)
      .pipe(map(() => this.loadUsers()));
  }

  updateUser(user: User, updateAll = true): Observable<any> {
    return this.http.put(environment.baseUrl + "/users/" + user.userId, user, environment.httpOptions)
      .pipe(map(() => { 
        if (updateAll) { 
          this.loadUsers() 
        }}));
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(environment.baseUrl + "/users/" + userId, environment.httpOptions)
      .pipe(map(() => this.loadUsers() ));
  }

  getUser(userId: string): Observable<User> {
    return this.http.get<User>(environment.baseUrl + "/users/" + userId, environment.httpOptions);
  }

  getUserProfileImage(userId: string): Observable<any> {
    return this.http.get(environment.baseUrl + "/users/" + userId + "/profileImage", environment.httpOptions);
  }

  uploadProfileImage(userId: string, file): Observable<any> {
    var formData = new FormData();
    formData.append('image', file, file.name);
    return this.http.post(environment.baseUrl + "/users/" + userId + "/profileImage", formData);
  }

  becomeRealtor(userId: string, action: Function) {
    this.getUser(userId).subscribe(user => {
      user.roles.push('Realtor');
      var clientRoleIndex = user.roles.indexOf('Client');
      if (clientRoleIndex >= 0) {
        user.roles.splice(clientRoleIndex, 1);
      }
      this.updateUser(user).subscribe(() => {
        action.call(this);
      });
    });
  }
}
