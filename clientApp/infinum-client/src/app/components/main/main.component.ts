import { Component, OnInit, ViewChild } from '@angular/core';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import { ContactService } from 'src/app/services/contact.service';
import { Contact } from 'src/app/models/contact';
import { Subject } from 'rxjs';
import { TelephoneNumber } from 'src/app/models/telephone-number';
import { RestService } from 'src/app/services/rest.service';
import { NgbDateStruct, NgbCalendar} from '@ng-bootstrap/ng-bootstrap';
import { DialogService } from 'src/app/services/dialog.service';


@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {

  @ViewChild(DataTableDirective, {static: true}) dtElement: DataTableDirective;

  contacts: Contact[] = [];
  dtTrigger: Subject<boolean> = new Subject();
  isCollapsed = true;
  shouldResetForm = false;

  dtOptions: DataTables.Settings = {};

  private searchQuery = "";


  constructor(
    private contactService: ContactService, 
    private rest: RestService,
    private dialog: DialogService
    ) {

    // VRŽI VEN
    this.contactService.messageReceived.subscribe((msg: any) => {
      console.log("received!::");
      console.log(msg);
    });


    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 10,
      serverSide: true,
      processing: true, 
      ordering: false,
      ajax: (dataTablesParameters: any, callback) => {
        this.setSearchQuery(dataTablesParameters);
        this.rest.getRequest<any>("Contacts"+this.searchQuery).subscribe(resp => {
          this.contacts = resp.data;

          callback({
              recordsTotal: resp.totalNumber,
              recordsFiltered: resp.totalNumber,
              data: []
          });
        });
      },
    };

    
    this.getContacts();

  }


  ngOnInit() {

  }

  // VRŽI VEN
  send()
  {
    this.contactService.sendMessage("heheheh");

    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.destroy();
    });

    this.getContacts();
  }

  private getContacts()
  {
    /*this.contactService.connectionEstablished.subscribe((contacts: Contact[]) => {
      this.contacts = contacts;
      this.dtTrigger.next();
    });*/
    this.rest.getRequest<any>("Contacts").subscribe(resp => {
      this.contacts = resp.data;
      this.dtTrigger.next();
    });
  }



  private setSearchQuery(parameters: any)
  {
    this.searchQuery = "";

    let query = parameters.search.value;
    let pageNr = (parameters.start / parameters.length) + 1;
    let perPage = parameters.length;
 
    this.searchQuery += "?";
    
    if(query != "")
      this.searchQuery += "filter="+ query + "&";

    this.searchQuery += "pageNumber="+pageNr+"&perPage="+perPage;
  }

  createNewContact($event)
  {
    this.rest.postRequest<any>($event, "Contacts").subscribe(resp => {
      this.dialog.success("Good job!", "Contact has been saved successfully!");
      this.shouldResetForm = true;
      this.isCollapsed = true;
    }, error => {
      console.log(error);
      this.dialog.error("Error ...", error.Message);
    });
  }

}
