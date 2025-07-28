import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  RaffleDto,
  RaffleNumberDto,
  PriceConfigurationDto,
  UpdatePricesRequest,
  RaffleStatsDto,
  ParticipantDto,
  CreateRaffleRequest
} from '../models/raffle.model';
import { PaymentRequestDto, PaymentResponseDto, PaymentStatusDto, PaymentWebhookDto } from '../models/payment.model'; // <-- ENSURE THESE ARE IMPORTED FROM payment.model

@Injectable({
  providedIn: 'root'
})
export class RaffleService {
  private apiUrl = 'http://localhost:5100/api'; // Asegúrate de que esta URL coincida con tu backend

  constructor(private http: HttpClient) { }

  // Métodos para el lado del usuario (RaffleGridComponent)
  getRaffleDetails(raffleId: string): Observable<RaffleDto> {
    return this.http.get<RaffleDto>(`${this.apiUrl}/raffle/${raffleId}`);
  }

  getAvailableNumbers(raffleId: string): Observable<RaffleNumberDto[]> {
    return this.http.get<RaffleNumberDto[]>(`${this.apiUrl}/raffle/${raffleId}/available-numbers`);
  }

  getAllNumbers(raffleId: string): Observable<RaffleNumberDto[]> {
    return this.http.get<RaffleNumberDto[]>(`${this.apiUrl}/raffle/${raffleId}/all-numbers`);
  }

  calculatePrice(raffleId: string, quantity: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/raffle/${raffleId}/calculate-price?quantity=${quantity}`);
  }

  reserveNumbers(raffleId: string, numbers: number[], participantEmail: string): Observable<boolean> {
    const body = { raffleId, numbers, participantEmail };
    return this.http.post<boolean>(`${this.apiUrl}/raffle/reserve-numbers`, body);
  }

  createPayment(request: PaymentRequestDto): Observable<PaymentResponseDto> {
    return this.http.post<PaymentResponseDto>(`${this.apiUrl}/payment/create`, request);
  }

  confirmPurchase(paymentId: string): Observable<boolean> {
    // Este método podría ser llamado por un webhook o una redirección del gateway de pago
    // Por ahora, simulamos una confirmación directa.
    return this.http.post<boolean>(`${this.apiUrl}/payment/confirm/${paymentId}`, {});
  }

  getPaymentStatus(paymentId: string): Observable<PaymentStatusDto> {
    return this.http.get<PaymentStatusDto>(`${this.apiUrl}/payment/status/${paymentId}`);
  }

  // Métodos para el lado del administrador (AdminComponent)
  getAllRaffles(): Observable<RaffleDto[]> {
    return this.http.get<RaffleDto[]>(`${this.apiUrl}/admin/raffles`);
  }

  createRaffle(request: CreateRaffleRequest): Observable<RaffleDto> {
    return this.http.post<RaffleDto>(`${this.apiUrl}/admin/raffles`, request);
  }

  updatePriceConfiguration(raffleId: string, prices: UpdatePricesRequest): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/admin/raffles/${raffleId}/price-configuration`, prices);
  }

  getParticipants(raffleId: string): Observable<ParticipantDto[]> {
    return this.http.get<ParticipantDto[]>(`${this.apiUrl}/admin/raffles/${raffleId}/participants`);
  }

  getRaffleStats(raffleId: string): Observable<RaffleStatsDto> {
    return this.http.get<RaffleStatsDto>(`${this.apiUrl}/admin/raffles/${raffleId}/stats`);
  }

  updateRaffleStatus(raffleId: string, isActive: boolean): Observable<boolean> {
    const body = { isActive };
    return this.http.put<boolean>(`${this.apiUrl}/admin/raffles/${raffleId}/status`, body);
  }
}