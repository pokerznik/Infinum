import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { Country } from 'src/app/models/country';
import { RestService } from 'src/app/services/rest.service';
import { DialogService } from 'src/app/services/dialog.service';
import { ContactDetails } from 'src/app/models/contact-details';
import { Contact } from 'src/app/models/contact';

@Component({
  selector: 'app-contact-input-template',
  templateUrl: './contact-input-template.component.html',
  styleUrls: ['./contact-input-template.component.css']
})
export class ContactInputTemplateComponent implements OnInit {

  @Output() inputConfirmed = new EventEmitter<any>();

  @Input() 
  set formReset(reset: boolean)
  {
    if(reset)
    {
      this.newContactForm.reset();
    }
  }

  @Input()
  set contact(existing: ContactDetails)
  {
    if(existing != null)
    {
      let birthdayDate = new Date(existing.dateOfBirth);
      console.log(birthdayDate);
      let birthdayObj = {
        day: birthdayDate.getDate(),
        month: birthdayDate.getMonth()+1,
        year: birthdayDate.getFullYear()
      };

      this.newContactForm.patchValue({'name': existing.name});
      this.newContactForm.patchValue({'dateOfBirth': birthdayObj});
      this.newContactForm.patchValue({'street': existing.address.street});
      this.newContactForm.patchValue({'houseNumber': existing.address.houseNumber});
      this.newContactForm.patchValue({'city': existing.address.city});
      this.newContactForm.patchValue({'ZIP': existing.address.zip});
      this.newContactForm.patchValue({'country': existing.address.country.id});

      if(existing.telephoneNumbers.length > 0)
      {
        let firstNumber = existing.telephoneNumbers[0];
        existing.telephoneNumbers.splice(0, 1);

        this.newContactForm.patchValue({'countryCode': firstNumber.countryCode});
        this.newContactForm.patchValue({'areaCode': firstNumber.areaCode});
        this.newContactForm.patchValue({'phoneNumber': firstNumber.phoneNumber});
        
        existing.telephoneNumbers.forEach(x => {
          this.addPhoneLine();
          let countryCodeKey = this.lastRandomName+"_countryCode";
          let areaCodeKey = this.lastRandomName+"_areaCode";
          let phoneNumberKey = this.lastRandomName+"_phoneNumber";

          this.newContactForm.patchValue({[countryCodeKey]: x.countryCode});
          this.newContactForm.patchValue({[areaCodeKey]: x.areaCode});
          this.newContactForm.patchValue({[phoneNumberKey]: x.phoneNumber});
        });
      }
    }
  }

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

  countries: Country[] = [];
  phoneNumbersControlNames: string[] = [];

  private lastRandomName = "";
  
  constructor(
    private rest: RestService,
    private dialog: DialogService
    ) 
    {
      this.getCountries();
    }

  ngOnInit() {
  }

  private getCountries()
  {
    this.rest.getRequest<Country[]>("countries").subscribe(resp => {
      this.countries = resp;
    });
  }

  addPhoneLine()
  {
    let randomName = Math.floor(Math.random() * Math.floor(99999)).toString();

    if(!this.phoneNumbersControlNames.includes(randomName))
    {
      this.lastRandomName = randomName;

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

  public submit()
  {
    if(this.newContactForm.valid)
    {
      let data = this.constructSendingObject();
      this.inputConfirmed.emit(data);
    }
    else
    {
      this.dialog.error("Error ...", "All fields except phone number(s) details are mandatory!");
    }
  }

}
