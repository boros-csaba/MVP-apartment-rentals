import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApartmentEditorComponent } from './apartment-editor.component';

describe('ApartmentEditorComponent', () => {
  let component: ApartmentEditorComponent;
  let fixture: ComponentFixture<ApartmentEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApartmentEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApartmentEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
