import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigManager } from './config-manager';

describe('ConfigManager', () => {
  let component: ConfigManager;
  let fixture: ComponentFixture<ConfigManager>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfigManager]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfigManager);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
