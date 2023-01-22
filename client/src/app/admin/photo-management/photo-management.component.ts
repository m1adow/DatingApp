import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { PhotoApprove } from 'src/app/_models/approvePhoto';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photoUsers?: PhotoApprove[];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.adminService.getPhotosToModerate().pipe(take(1)).subscribe({
      next: _ => this.photoUsers = _ 
    });
  }

  openApprovePhotosModal(photos: Photo[]) {

  }
}
