import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { map, take } from 'rxjs';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-approve-photo-modal',
  templateUrl: './approve-photo-modal.component.html',
  styleUrls: ['./approve-photo-modal.component.css']
})
export class ApprovePhotoModalComponent {
  photo?: Photo;
  username?: string;
  result = false;

  constructor(private modalRef: BsModalRef, private adminService: AdminService) { }

  approvePhoto(isApproved: boolean) {
    this.adminService.approvePhoto(isApproved, this.username!, this.photo!.id).pipe(take(1)).subscribe({
      next: _ => {
        this.result = true;
        this.modalRef.hide();
      }
    });
  }

  close() {
    this.result = false;
    this.modalRef.hide();
  }
}
