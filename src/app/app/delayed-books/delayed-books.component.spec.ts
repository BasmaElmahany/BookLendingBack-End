import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DelayedBooksComponent } from './delayed-books.component';

describe('DelayedBooksComponent', () => {
  let component: DelayedBooksComponent;
  let fixture: ComponentFixture<DelayedBooksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DelayedBooksComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DelayedBooksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
