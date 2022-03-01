import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApartmentDetailsPageComponent } from './apartment-details-page.component';

describe('ApartmentDetailsPageComponent', () => {
  let component: ApartmentDetailsPageComponent;
  let fixture: ComponentFixture<ApartmentDetailsPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApartmentDetailsPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApartmentDetailsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
