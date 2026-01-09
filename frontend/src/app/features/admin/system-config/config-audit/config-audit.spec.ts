import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigAudit } from './config-audit';

describe('ConfigAudit', () => {
  let component: ConfigAudit;
  let fixture: ComponentFixture<ConfigAudit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfigAudit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfigAudit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
