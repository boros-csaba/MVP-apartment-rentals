import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDashboardApartmentsPageComponent } from './admin-dashboard-apartments-page.component';

describe('AdminDashboardApartmentsPageComponent', () => {
  let component: AdminDashboardApartmentsPageComponent;
  let fixture: ComponentFixture<AdminDashboardApartmentsPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdminDashboardApartmentsPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminDashboardApartmentsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
