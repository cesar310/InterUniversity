import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimpleInCard } from './simple-in-card';

describe('SimpleInCard', () => {
  let component: SimpleInCard;
  let fixture: ComponentFixture<SimpleInCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SimpleInCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SimpleInCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
