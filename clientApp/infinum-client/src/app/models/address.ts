import { Country } from './country';

export interface Address {
    id: number;
    street: string;
    houseNumber: string;
    zip: string;
    city: string;
    country: Country;
}
