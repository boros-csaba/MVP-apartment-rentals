import { Component, OnInit } from '@angular/core';
import { Apartment } from 'src/app/models/apartment';
import { MatTableDataSource, MatDialogRef, MatDialog } from '@angular/material';
import { ApartmentEditorComponent } from '../apartment-editor/apartment-editor.component';
import { ApartmentService } from 'src/app/services/apartment.service';

@Component({
  selector: 'app-admin-dashboard-apartments-page',
  templateUrl: './admin-dashboard-apartments-page.component.html',
  styleUrls: ['./admin-dashboard-apartments-page.component.css']
})
export class AdminDashboardApartmentsPageComponent implements OnInit {

  dataSource = new MatTableDataSource();
  prev: string;
  next: string;
  apartmentEditorDialog: MatDialogRef<ApartmentEditorComponent>;
  
  constructor(private apartmentService: ApartmentService, private dialog: MatDialog) { }

  ngOnInit() {
    this.apartmentService.getAllApartments(15).subscribe(data => {
        this.dataSource = new MatTableDataSource(data.data);
        this.prev = data.prev;
        this.next = data.next;
      });
    this.apartmentService.updatedObservable.subscribe(() => {
      this.apartmentService.getAllApartments(15).subscribe(data => {
        this.dataSource = new MatTableDataSource(data.data);
        this.prev = data.prev;
        this.next = data.next;
      });
    });
  }

  prevPage() {
    this.apartmentService.loadPage(this.prev)
      .subscribe(data => {
        this.dataSource = new MatTableDataSource(data.data);
        this.prev = data.prev;
        this.next = data.next;
      });
  }

  nextPage() {
    this.apartmentService.loadPage(this.next)
    .subscribe(data => {
      this.dataSource = new MatTableDataSource(data.data);
      this.prev = data.prev;
      this.next = data.next;
    });
  }

  openDialog(apartment: Apartment) {
    this.apartmentEditorDialog = this.dialog.open(ApartmentEditorComponent, {
      hasBackdrop: true,
      disableClose: true,
      data: { apartment: apartment, asAdmin: true },
      autoFocus: true,
      width: "1000px"
    });
  }

  openAddApartmentPopup() {
    this.openDialog(new Apartment());
  }

  deleteApartment(apartment: Apartment) {
    if (confirm("Are you sure you want to delete this apartment ?")) {
      this.apartmentService.deleteApartment(apartment.id)
        .subscribe(() => { },
        errors => {
          alert(errors.error.errors);
        });
    }
  }

}
