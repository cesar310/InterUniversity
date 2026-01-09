import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfessorList } from './professor-list';

describe('ProfessorList', () => {
  let component: ProfessorList;
  let fixture: ComponentFixture<ProfessorList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfessorList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfessorList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
