import { Component, OnInit, ViewChild } from '@angular/core';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import { ContactService } from 'src/app/services/contact.service';
import { Contact } from 'src/app/models/contact';
import { Subject } from 'rxjs';
import { TelephoneNumber } from 'src/app/models/telephone-number';
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { RestService } from 'src/app/services/rest.service';
import { Country } from 'src/app/models/country';
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
  countries: Country[] = [];

  dtOptions: DataTables.Settings = {};

  private searchQuery = "";


  newContactForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    dateOfBirth: new FormControl('', [Validators.required]),
    street: new FormControl('', [Validators.required]),
    houseNumber: new FormControl('', [Validators.required]),
    city: new FormControl('', [Validators.required]),
    ZIP: new FormControl('', [Validators.required]),
    country: new FormControl('', [Validators.required]),
    countryCode: new FormControl(''),
    areaCode: new FormControl(''),
    phoneNumber: new FormControl('')
  });

  phoneNumbersControlNames: string[] = [];

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

    this.getCountries();
    this.getContacts();

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

  private getCountries()
  {
    this.rest.getRequest<Country[]>("countries").subscribe(resp => {
      this.countries = resp;
    });
  }

  ngOnInit() {

  }

  contactDetails(id)
  {
    alert(id);
  }

  addPhoneLine()
  {
    let randomName = Math.floor(Math.random() * Math.floor(99999)).toString();

    if(!this.phoneNumbersControlNames.includes(randomName))
    {
      this.newContactForm.addControl(randomName+"_countryCode", new FormControl(''));
      this.newContactForm.addControl(randomName+"_areaCode", new FormControl(''));
      this.newContactForm.addControl(randomName+"_phoneNumber", new FormControl(''));

      this.phoneNumbersControlNames.push(randomName);
    }
    else 
    {
      this.addPhoneLine();
    }
  }

  removeNumber(controlName)
  {
    this.newContactForm.removeControl(controlName+"_countryCode");
    this.newContactForm.removeControl(controlName+"_areaCode");
    this.newContactForm.removeControl(controlName+"_phoneNumber");

    let index = this.phoneNumbersControlNames.findIndex(x => controlName);
    this.phoneNumbersControlNames.splice(index, 1);
  }

  private constructTelephoneNumbers() : any[] 
  {
    let numbers = [];
    
    if((this.newContactForm.value.countryCode != "") && (this.newContactForm.value.phoneNumber != ""))
    {
      let number = {
        countryCode: this.newContactForm.value.countryCode,
        areaCode: this.newContactForm.value.areaCode,
        phoneNumber: this.newContactForm.value.phoneNumber
      };

      numbers.push(number);
    }

    this.phoneNumbersControlNames.forEach(nr => {
      let countryCodeKey = nr+"_countryCode";
      let areaCodeKey = nr+"_areaCode";
      let phoneNumberKey = nr+"_phoneNumber";

      let number = {
        countryCode: this.newContactForm.value[countryCodeKey],
        areaCode: this.newContactForm.value[areaCodeKey],
        phoneNumber: this.newContactForm.value[phoneNumberKey]
      };

      numbers.push(number);
    });

    return numbers;

  }

  private constructSendingObject()
  {
    let dateOfBirth = new Date(this.newContactForm.value.dateOfBirth.year, this.newContactForm.value.dateOfBirth.month-1, this.newContactForm.value.dateOfBirth.day+1);
    return {
      name: this.newContactForm.value.name,
      dateOfBirth: dateOfBirth.toISOString(),
      address: {
        street: this.newContactForm.value.street,
        houseNumber: this.newContactForm.value.houseNumber,
        ZIP: this.newContactForm.value.ZIP,
        city: this.newContactForm.value.city,
        country: {
          id: parseInt(this.newContactForm.value.country)
        }
      },
      telephoneNumbers: this.constructTelephoneNumbers()
    };
  }

  public contactFormSubmit()
  {
    if(this.newContactForm.valid)
    {
      let toSend = this.constructSendingObject();
      this.rest.postRequest<any>(toSend, "Contacts").subscribe(resp => {
        this.dialog.success("Good job!", "Contact has been saved successfully!");
        this.newContactForm.reset();
        this.isCollapsed = true;
      }, error => {
        console.log(error);
        this.dialog.error("Error ...", error.Message);
      });
    }
    else 
    {
      this.dialog.error("Error ...", "All fields except phone number(s) details are mandatory!");
    }
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

}
