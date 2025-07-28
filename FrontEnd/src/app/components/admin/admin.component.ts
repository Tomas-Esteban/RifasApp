import { Component, OnInit } from '@angular/core';
import { RaffleService } from '../../services/raffle.service';
import { RaffleDto, UpdatePricesRequest, PriceConfigurationDto } from '../../models/raffle.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin',
  standalone: true, // Si usas standalone
  imports: [CommonModule, FormsModule], // Importa aquí si es standalone
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'] // Puedes usar .scss si configuraste así
})
export class AdminComponent implements OnInit {
  raffles: RaffleDto[] = [];
  selectedRaffleId: string = '';
  currentPrices: PriceConfigurationDto | null = null;
  priceFor1: number = 0;
  priceFor2: number = 0;
  priceFor3: number = 0;
  message: string = '';

  constructor(private raffleService: RaffleService) { }

  ngOnInit(): void {
    this.loadRaffles();
  }

  loadRaffles(): void {
    this.raffleService.getAllRaffles().subscribe({
      next: (data) => {
        this.raffles = data;
        if (this.raffles.length > 0) {
          // Selecciona la primera rifa por defecto o la que sea relevante
          this.selectedRaffleId = this.raffles[0].id;
          this.loadPriceConfiguration();
        } else {
          this.message = 'No hay rifas disponibles. Crea una primero desde el backend si es necesario.';
        }
      },
      error: (err) => {
        console.error('Error al cargar rifas:', err);
        this.message = 'Error al cargar las rifas.';
      }
    });
  }

  onRaffleSelected(): void {
    this.loadPriceConfiguration();
  }

  loadPriceConfiguration(): void {
    if (!this.selectedRaffleId) {
      this.currentPrices = null;
      this.priceFor1 = 0;
      this.priceFor2 = 0;
      this.priceFor3 = 0;
      return;
    }

    this.raffleService.getRaffleDetails(this.selectedRaffleId).subscribe({
      next: (raffle) => {
        this.currentPrices = raffle.priceConfiguration || null;
        if (this.currentPrices) {
          this.priceFor1 = this.currentPrices.priceFor1;
          this.priceFor2 = this.currentPrices.priceFor2;
          this.priceFor3 = this.currentPrices.priceFor3;
        } else {
          this.message = 'No se encontró configuración de precios para esta rifa. Establece los precios.';
          this.priceFor1 = 0;
          this.priceFor2 = 0;
          this.priceFor3 = 0;
        }
      },
      error: (err) => {
        console.error('Error al cargar la configuración de precios:', err);
        this.message = 'Error al cargar la configuración de precios.';
        this.currentPrices = null;
      }
    });
  }

  savePrices(): void {
    if (!this.selectedRaffleId) {
      this.message = 'Por favor, selecciona una rifa.';
      return;
    }

    const request: UpdatePricesRequest = {
      priceFor1: this.priceFor1,
      priceFor2: this.priceFor2,
      priceFor3: this.priceFor3
    };

    this.raffleService.updatePriceConfiguration(this.selectedRaffleId, request).subscribe({
      next: (success) => {
        if (success) {
          this.message = '¡Precios actualizados exitosamente!';
          this.loadPriceConfiguration(); // Recargar para confirmar
        } else {
          this.message = 'Error al actualizar los precios.';
        }
      },
      error: (err) => {
        console.error('Error al guardar precios:', err);
        this.message = 'Error al guardar los precios. Asegúrate de que el backend esté funcionando.';
      }
    });
  }
}