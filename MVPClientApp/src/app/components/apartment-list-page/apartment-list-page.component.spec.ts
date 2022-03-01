import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApartmentListPageComponent } from './apartment-list-page.component';

describe('ApartmentListPageComponent', () => {
  let component: ApartmentListPageComponent;
  let fixture: ComponentFixture<ApartmentListPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApartmentListPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApartmentListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
