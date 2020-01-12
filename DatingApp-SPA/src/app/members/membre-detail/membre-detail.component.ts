import { Component, OnInit } from '@angular/core';
import { User } from '../../_Models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from '../../../../node_modules/ngx-gallery';

@Component({
  selector: 'app-membre-detail',
  templateUrl: './membre-detail.component.html',
  styleUrls: ['./membre-detail.component.css']
})
export class MembreDetailComponent implements OnInit {
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private userService: UserService, private alertifyService: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
      this.galleryOptions = [
        {
          width: '500px',
          height: '500px',
          imagePercent: 100,
          thumbnailsColumns: 4,
          imageAnimation: NgxGalleryAnimation.Slide,
          preview: false
      }];

      this.galleryImages = this.getImages();
    }
      getImages() {
        const imagesUrl = [];
        for (let i = 0; i < this.user.photos.length; i++) {
          imagesUrl.push({
            small: this.user.photos[i].url,
            medium: this.user.photos[i].url,
            big: this.user.photos[i].url,
            description: this.user.photos[i].description
          });
        }
        return imagesUrl;
      }
}
