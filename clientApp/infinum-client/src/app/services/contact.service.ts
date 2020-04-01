import { Injectable, EventEmitter } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { AppSettings } from '../settings/app-settings';

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

  messageReceived = new EventEmitter<any>();  
  connectionEstablished = new EventEmitter<Boolean>();  
  
  private connectionIsEstablished = false;  
  private _hubConnection: HubConnection;  

    // VRŽI VEN
    sendMessage(message: any) {  
      this._hubConnection.invoke('SendMessage', message);  
    }  
    
    private createConnection() {  
      this._hubConnection = new HubConnectionBuilder()  
        .withUrl(this.hubBase)  
        .build();  
    }  
    
    private startConnection(): void {  
      this._hubConnection  
        .start()  
        .then(() => {  
          this.connectionIsEstablished = true;  
          //this.connectionEstablished.emit(true);  
        })  
        .catch(err => {  
          console.log('Error while establishing connection, error:');  
          console.log(err);
        });  
    }  
    
    private registerOnServerEvents(): void {  
      this._hubConnection.on('MessageReceived', (data: any) => {  
        this.messageReceived.emit(data);  
      }); 
      
      this._hubConnection.on('Connected', (data: any) => {
        this.connectionEstablished.emit(data);
      });
    }  

}
