import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from '../../_Models/user';
import { ActivatedRoute } from '../../../../node_modules/@angular/router';
import { AlertifyService } from '../../_services/alertify.service';
import { NgForm } from '../../../../node_modules/@angular/forms';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/Auth.service';

@Component({
  selector: 'app-membre-edit',
  templateUrl: './membre-edit.component.html',
  styleUrls: ['./membre-edit.component.css']
})
export class MembreEditComponent implements OnInit {
  user: User;
  photoUrl: string;
  @ViewChild('editForm') editForm: NgForm;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }
  constructor(private route: ActivatedRoute, private alertify: AlertifyService, private userService: UserService,
    private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl );
  }

  updateUser() {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(
      next => {
        this.alertify.success('your information updated successfully ');
        this.editForm.reset(this.user);
    }, error => {
      this.alertify.error(error);
    });
  }
  updateMainPhoto(phototUrl) {
    this.user.photoUrl = phototUrl;
  }
}
