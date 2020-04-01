import { Component, OnInit, ViewChild } from '@angular/core';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import { ContactService } from 'src/app/services/contact.service';
import { Contact } from 'src/app/models/contact';
import { Subject } from 'rxjs';
import { TelephoneNumber } from 'src/app/models/telephone-number';
import { RestService } from 'src/app/services/rest.service';
import { NgbDateStruct, NgbCalendar} from '@ng-bootstrap/ng-bootstrap';
import { DialogService } from 'src/app/services/dialog.service';
import { ContactDetails } from 'src/app/models/contact-details';


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

    this.contactService.contactUpdated.subscribe((contact: ContactDetails) => {
      this.refreshData();
      this.dialog.info("Just a sec!", "Some contacts have changed. We have retrieved the new data with.");
    });

    this.contactService.contactDeleted.subscribe((id: number) => {
      this.refreshData();
      this.dialog.info("Just a sec!", "Some contacts have changed. We have retrieved the new data.");
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


  private refreshData()
  {
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.destroy();
    });

    this.getContacts();
  }

  private getContacts()
  {
    let query = "";
    if(this.searchQuery != "")
      query = this.searchQuery;

    this.rest.getRequest<any>("Contacts"+query).subscribe(resp => {
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
