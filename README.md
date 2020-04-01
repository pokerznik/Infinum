# Infinum Assignment (by Zan Pokerznik)

Dear Infinums,

here are few words about my assignment. Implemented solution is web application which allow creating, updating and deleting contacts. Each contact is unique by their name and address. For easier set-up there is also exported PostgreSQL database with some test data.

## Technology stack
- backend: ASP .NET Core 3.0 Web API (with clean architecture and EF) and PostgreSQL database,
- frontend: Angular 8 with some libraries (SweetAlert, Bootstrap, SignalRClient, DataTable).

## Few notes
- Server url for signalR and api calls on client side can be set in `appsettings.ts` file (`src/app/settings/appsettings.ts`)
- If there is a contact is updated, there will be 
  - notification in contacts list view and freshing data by re-calling server api; 
  - notification in contact detail view and replacing old data with new one directly from signalR message.
