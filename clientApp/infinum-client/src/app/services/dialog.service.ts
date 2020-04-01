import { Injectable } from '@angular/core';
import swal, { SweetAlertResult } from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  constructor() { }

  info(title: string, message: string){
    swal.fire(title, message, 'info');
  }

  success(title: string, message: string){
    swal.fire(title, message, 'success');
  }

  error(title: string, message: string){
    swal.fire(title, message, 'error');
  }
}
