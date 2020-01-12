import { Component, OnInit } from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './Home.component.html',
  styleUrls: ['./Home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  values: any;
  constructor(private http: HttpClient) { }

  ngOnInit() {
  }
  registreToggle() {
    this.registerMode = !this.registerMode;
  }
  cancelregisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
