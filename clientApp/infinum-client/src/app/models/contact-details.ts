import { Address } from './address';
import { TelephoneNumber } from './telephone-number';

export interface ContactDetails {
    id: number;
    name: string;
    dateOfBirth: string;
    address: Address;
    telephoneNumbers: TelephoneNumber[];
}
