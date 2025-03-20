import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-music-search-modal',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './music-search-modal.component.html',
  styleUrls: ['./music-search-modal.component.css']
})
export class MusicSearchModalComponent {
  songName: string = '';

  @Output() closeModal = new EventEmitter<void>();
  @Output() songSelected = new EventEmitter<string>();

  selectSong(songName: string) {
    this.songSelected.emit(songName);
  }

  searchSong() {
    if (this.songName.trim()) {
      this.songSelected.emit(this.songName);
      this.closeModal.emit();
    }
  }
}
