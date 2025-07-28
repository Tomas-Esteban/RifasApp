export interface RaffleDto {
  id: string;
  name: string;
  description: string;
  startDate: Date;
  endDate: Date;
  isActive: boolean;
  priceConfiguration?: PriceConfigurationDto; // Opcional si no siempre se carga
  raffleNumbers: RaffleNumberDto[];
}

export interface RaffleNumberDto {
  id: string;
  number: number;
  isAvailable: boolean;
  participantName?: string;
  participantEmail?: string;
  participantPhone?: string;
  purchasedAt?: Date;
  reservedAt?: Date;
  pricePaid?: number; // Agregado para el backend
}

export interface PriceConfigurationDto {
  id: string;
  priceFor1: number;
  priceFor2: number;
  priceFor3: number;
}

export interface CreateRaffleRequest {
  name: string;
  description: string;
  startDate: Date;
  endDate: Date;
  isActive: boolean;
}

export interface UpdatePricesRequest {
  priceFor1: number;
  priceFor2: number;
  priceFor3: number;
}

export interface ParticipantDto {
  name?: string;
  email?: string;
  phone?: string;
}

export interface RaffleStatsDto {
  totalNumbers: number;
  availableNumbers: number;
  soldNumbers: number;
  revenue: number; // Agregado para el backend
}