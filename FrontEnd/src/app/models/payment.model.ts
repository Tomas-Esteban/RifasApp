export interface PaymentRequestDto {
  raffleId: string;
  amount: number;
  participantName: string;
  participantEmail: string;
  participantPhone: string;
  numbers: number[]; // Agregado para el backend
}

export interface PaymentResponseDto {
  isSuccess: boolean; // Agregado para el backend
  paymentId?: string;
  checkoutUrl?: string; // Agregado para el backend
  message?: string; // Agregado para el backend
}

export interface PaymentWebhookDto {
  paymentId: string; // Asumo que el backend lo envía como string, lo parsearemos si es necesario
  status: string;
}

export interface PaymentStatusDto {
  paymentId: string;
  status: string;
  amount: number;
  completedAt?: Date;
}

// Enum para el estado de pago, si lo usas en el frontend
export enum PaymentStatus {
  Pending = 'Pending',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}