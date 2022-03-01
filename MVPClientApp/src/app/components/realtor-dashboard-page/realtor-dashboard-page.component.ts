import { Component, OnInit } from '@angular/core';
import { MatTableDataSource, MatDialogRef, MatDialog } from '@angular/material';
import { Apartment } from 'src/app/models/apartment';
import { ApartmentEditorComponent } from '../apartment-editor/apartment-editor.component';
import { ApartmentService } from 'src/app/services/apartment.service';

@Component({
  selector: 'app-realtor-dashboard-page',
  templateUrl: './realtor-dashboard-page.component.html',
  styleUrls: ['./realtor-dashboard-page.component.css']
})
export class RealtorDashboardPageComponent implements OnInit {

  dataSource = new MatTableDataSource();
  prev: string;
  next: string;
  apartmentEditorDialog: MatDialogRef<ApartmentEditorComponent>;
  
  constructor(private dialog: MatDialog, private apartmentService: ApartmentService) { }

  ngOnInit() {
    this.apartmentService.getOwnApartments(15).subscribe(data => {
        this.dataSource = new MatTableDataSource(data.data);
        this.prev = data.prev;
        this.next = data.next;
      });
    this.apartmentService.updatedObservable.subscribe(() => {
      this.apartmentService.getOwnApartments(15).subscribe(data => {
        this.dataSource = new MatTableDataSource(data.data);
        this.prev = data.prev;
        this.next = data.next;
      });
    });
  }

  prevPage() {
    this.apartmentService.loadPage(this.prev);
  }

  nextPage() {
    this.apartmentService.loadPage(this.next);
  }

  openDialog(apartment: Apartment) {
    this.apartmentEditorDialog = this.dialog.open(ApartmentEditorComponent, {
      hasBackdrop: true,
      disableClose: true,
      data: { apartment: apartment, asAdmin: false },
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
