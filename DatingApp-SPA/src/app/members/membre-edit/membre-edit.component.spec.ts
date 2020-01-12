/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MembreEditComponent } from './membre-edit.component';

describe('MembreEditComponent', () => {
  let component: MembreEditComponent;
  let fixture: ComponentFixture<MembreEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MembreEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MembreEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
