import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'map',
  standalone: true // Make it standalone so it can be directly imported
})
export class MapPipe implements PipeTransform {
  transform<T>(array: T[] | null | undefined, key: keyof T): any[] {
    if (!array) {
      return [];
    }
    return array.map(item => item[key]);
  }
}