import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ApprovedPhoto, PhotoApprove } from '../_models/approvePhoto';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {});
  }

  getPhotosToModerate() {
    return this.http.get<PhotoApprove[]>(this.baseUrl + 'admin/photos-to-moderate');
  }

  approvePhoto(isApproved: boolean, username: string, photoId: number) {
    return this.http.put<ApprovedPhoto>(this.baseUrl + 'admin/approve-photo?username=' + username + '&photoId=' + photoId + '&isApproved=' + isApproved, {});
  }
}
