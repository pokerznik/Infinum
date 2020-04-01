import { Injectable, EventEmitter } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { AppSettings } from '../settings/app-settings';
import { ContactDetails } from '../models/contact-details';

@Injectable({
  providedIn: 'root'
})

export class ContactService {

  private hubBase = ""

  constructor() {
    this.hubBase = AppSettings.serverUrl + "MainHub"

    this.createConnection();  
    this.registerOnServerEvents();  
    this.startConnection();  
   }

  contactUpdated = new EventEmitter<ContactDetails>();  
  contactDeleted = new EventEmitter<number>();
  connectionEstablished = new EventEmitter<Boolean>();  
  
  private connectionIsEstablished = false;  
  private hubConnection: HubConnection;  
    
    private createConnection() {  
      this.hubConnection = new HubConnectionBuilder()  
        .withUrl(this.hubBase)  
        .build();  
    }  
    
    private startConnection(): void {  
      this.hubConnection  
        .start()  
        .then(() => {  
          this.connectionIsEstablished = true;  
        })  
        .catch(err => {  
          console.log('Error while establishing connection, error:');  
          console.log(err);
        });  
    }  
    
    private registerOnServerEvents(): void {  
      this.hubConnection.on('ContactUpdated', (data: ContactDetails) => {
        this.contactUpdated.emit(data);
      });

      this.hubConnection.on('ContactDeleted', (id: any) => {
        console.log("contdel");
        console.log(id);
        this.contactDeleted.emit(id);
      });
      
      this.hubConnection.on('Connected', (data: any) => {
        this.connectionEstablished.emit(data);
      });
    }  

}
