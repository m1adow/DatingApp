import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { map, take } from 'rxjs';
import { ApprovePhotoModalComponent } from 'src/app/modals/approve-photo-modal/approve-photo-modal.component';
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
  bsModalRef: BsModalRef<ApprovePhotoModalComponent> = new BsModalRef<ApprovePhotoModalComponent>();

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.adminService.getPhotosToModerate().pipe(take(1)).subscribe({
      next: _ => this.photoUsers = _
    });
  }

  openApprovePhotosModal(username: string, photo: Photo) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: username,
        photo: photo
      }
    };
    this.bsModalRef = this.modalService.show(ApprovePhotoModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const user = this.photoUsers?.find(u => u.username === username);
        if (user && this.bsModalRef.content!.result) {
          user.photos = user.photos.filter(p => p.id !== photo.id);
          this.photoUsers = this.photoUsers?.filter(u => u.username !== username);
          if (user.photos && user.photos.length > 0) {
            this.photoUsers = [...this.photoUsers!, user];
          }
        }
      }
    });
  }
}
