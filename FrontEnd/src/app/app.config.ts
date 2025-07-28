import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http'; // Importa esto

import { routes } from './app.routes'; // Asume que tienes un app.routes.ts
import { RaffleService } from './services/raffle.service';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async'; // Importa tu servicio

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(), // Habilita HttpClient
    RaffleService, provideAnimationsAsync() // Provee tu servicio
  ]
};