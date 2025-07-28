import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface NumberStatus {
  number: number;
  status: 'libre' | 'reservado' | 'ocupado';
  selected: boolean;
}

@Component({
  selector: 'app-raffle-grid',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './raffle-grid.component.html',
  styleUrls: ['./raffle-grid.component.css']
})
export class RaffleGridComponent implements OnInit {
  numbers: NumberStatus[] = [];
  selectedNumbers: number[] = [];
  pricePerNumber: number = 1000; // Precio por número en pesos
  totalPrice: number = 0;

  ngOnInit(): void {
    this.initializeNumbers();
  }

  initializeNumbers(): void {
    // Crear números del 1 al 100
    for (let i = 1; i <= 100; i++) {
      // Simular algunos números ocupados y reservados para ejemplo
      let status: 'libre' | 'reservado' | 'ocupado' = 'libre';
      
      // Algunos números ocupados de ejemplo
      if ([5, 13, 27, 34, 45, 67, 89].includes(i)) {
        status = 'ocupado';
      }
      // Algunos números reservados de ejemplo
      else if ([12, 23, 56, 78, 91].includes(i)) {
        status = 'reservado';
      }

      this.numbers.push({
        number: i,
        status: status,
        selected: false
      });
    }
  }

  toggleNumberSelection(numberObj: NumberStatus): void {
    // Solo se pueden seleccionar números libres
    if (numberObj.status !== 'libre') {
      return;
    }

    const index = this.selectedNumbers.indexOf(numberObj.number);
    
    if (index > -1) {
      // Deseleccionar
      this.selectedNumbers.splice(index, 1);
      numberObj.selected = false;
    } else {
      // Seleccionar
      this.selectedNumbers.push(numberObj.number);
      numberObj.selected = true;
    }

    this.selectedNumbers.sort((a, b) => a - b); // Mantener ordenados
    this.calculateTotalPrice();
  }

  calculateTotalPrice(): void {
    this.totalPrice = this.selectedNumbers.length * this.pricePerNumber;
  }

  getNumberClass(numberObj: NumberStatus): string {
    if (numberObj.selected) {
      return 'number-item number-item-seleccionado';
    }
    
    return `number-item number-item-${numberObj.status}`;
  }

  proceedToPurchase(): void {
    if (this.selectedNumbers.length === 0) {
      alert('Por favor selecciona al menos un número');
      return;
    }

    const confirmation = confirm(
      `¿Confirmas la compra de ${this.selectedNumbers.length} número(s): ${this.selectedNumbers.join(', ')}?\n` +
      `Total: $${this.totalPrice.toLocaleString()}`
    );

    if (confirmation) {
      // Aquí iría la lógica de compra
      alert('¡Compra realizada con éxito!');
      
      // Marcar los números como ocupados
      this.selectedNumbers.forEach(num => {
        const numberObj = this.numbers.find(n => n.number === num);
        if (numberObj) {
          numberObj.status = 'ocupado';
          numberObj.selected = false;
        }
      });

      // Limpiar selección
      this.selectedNumbers = [];
      this.totalPrice = 0;
    }
  }
}