import { Routes } from '@angular/router';
import { RaffleGridComponent } from './components/raffle-grid/raffle-grid.component';
import { AdminComponent } from './components/admin/admin.component';

export const routes: Routes = [
  { path: '', component: RaffleGridComponent },
  { path: 'admin', component: AdminComponent },
  { path: '**', redirectTo: '' } // Redirige a la página principal si la ruta no existe
];