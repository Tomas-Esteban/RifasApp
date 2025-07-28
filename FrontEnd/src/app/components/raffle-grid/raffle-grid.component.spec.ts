import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RaffleGridComponent } from './raffle-grid.component';

describe('RaffleGridComponent', () => {
  let component: RaffleGridComponent;
  let fixture: ComponentFixture<RaffleGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RaffleGridComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RaffleGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
