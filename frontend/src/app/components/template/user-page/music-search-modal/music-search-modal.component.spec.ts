import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MusicSearchModalComponent } from './music-search-modal.component';

describe('MusicSearchModalComponent', () => {
  let component: MusicSearchModalComponent;
  let fixture: ComponentFixture<MusicSearchModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MusicSearchModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MusicSearchModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
