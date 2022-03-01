import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RealtorDashboardPageComponent } from './realtor-dashboard-page.component';

describe('RealtorDashboardPageComponent', () => {
  let component: RealtorDashboardPageComponent;
  let fixture: ComponentFixture<RealtorDashboardPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RealtorDashboardPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RealtorDashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
