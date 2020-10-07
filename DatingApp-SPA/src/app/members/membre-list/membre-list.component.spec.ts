/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MembreListComponent } from './membre-list.component';
import { single } from 'rxjs/operators';

describe('my first test', () => {
  let sut;

  beforeEach(()=>{
    sut={}
  })

  it('should be true if true',() => {
    sut.a=false;

    sut.a=true;

    expect(sut.a).toBe(true);
  })
});
