import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject} from 'rxjs';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';
import { User } from '../_Models/user';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();
  constructor(private http: HttpClient) { }

  ChangeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'auth/login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.userToReturn));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.userToReturn;
          this.ChangeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }
  register(user: User) {
    return this.http.post(this.baseUrl + 'auth/register', user);
  }

  loggedin() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}

