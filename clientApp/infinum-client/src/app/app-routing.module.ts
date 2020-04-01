import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MainComponent } from './components/main/main.component';
import { DetailsComponent } from './components/details/details.component';
import { EditComponent } from './components/edit/edit.component';


const routes: Routes = [
  { path: 'contacts', component: MainComponent },
  { path: 'contacts/:id', component: DetailsComponent },
  { path: 'contacts/:id/edit',component: EditComponent },
  { path: '', redirectTo:'/contacts', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
