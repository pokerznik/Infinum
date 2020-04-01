import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RestService } from 'src/app/services/rest.service';
import { DialogService } from 'src/app/services/dialog.service';
import { ContactDetails } from 'src/app/models/contact-details';
import { FormControl, FormGroup, Validators } from '@angular/forms'

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit {

  private detailId : number;

  contact : ContactDetails = {
    name: "",
    address: {
      id : 0,
      city : "",
      country: {
        id: 0,
        name : ""
      },
      houseNumber : "",
      street: "",
      zip: ""
    },
    dateOfBirth: "",
    id: 0,
    telephoneNumbers: []
  };

  constructor(
    private actRoute: ActivatedRoute,
    private rest: RestService,
    private dialog: DialogService
    ) 
  { 
    this.detailId = this.actRoute.snapshot.params.id;
    
    this.getContactDetails();
  }

  private getContactDetails()
  {
    this.rest.getRequest<ContactDetails>("Contacts/"+this.detailId).subscribe(resp => {
      this.contact = resp;
    }, error => {
      this.dialog.error("Error ...", error.Message);
    });
  }

  ngOnInit() {
  }

  updateContact($event)
  {
    $event.id = this.contact.id;
    this.rest.putRequest<any>($event, "Contacts").subscribe(resp => {
      this.dialog.success("Great!", "Contact has been updated successfully!");
    }, error => {
      this.dialog.error("Oh no!", error.Message);
    });
  }

}
