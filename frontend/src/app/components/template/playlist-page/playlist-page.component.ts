import { isPlatformBrowser, CommonModule } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-playlist-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './playlist-page.component.html',
  styleUrls: ['./playlist-page.component.css']
})
export class PlaylistPageComponent {
  playlistUrl: string | null = null;
  playlistImage: string | null = null;
  playlistName: string | null = null;

  constructor(@Inject(PLATFORM_ID) private platformId: object) {}

  async ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.playlistUrl = localStorage.getItem('playlistUrl');
      console.log("Playlist recuperada do localStorage:", this.playlistUrl);

      if (this.playlistUrl) {
        await this.fetchPlaylistDetails(this.playlistUrl);
      }
    }
  }

  async fetchPlaylistDetails(url: string) {
    try {
      const playlistId = url.match(/playlist\/([^?]+)/)?.[1];
      if (!playlistId) return;

      const response = await fetch(`http://localhost:5229/api/playlist/details?id=${playlistId}`);
      const data = await response.json();
      console.log("Dados da playlist:", data);

      this.playlistImage = data.imageUrl || 'default-image-url.jpg';
      this.playlistName = data.name || 'Nome da playlist não disponível';
    } catch (error) {
      console.error("Erro ao buscar detalhes da playlist:", error);
    }
  }


  async saveRecommendation() {
    const email = localStorage.getItem('userEmail');
    if (!email || !this.playlistUrl) {
      console.error("Erro: Email ou playlistUrl não disponíveis.");
      return;
    }

    try {
      const response = await fetch('http://localhost:5229/api/searchhistory/recommendation', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: email,
          playlistUrl: this.playlistUrl,
          searchDate: new Date().toISOString()
        }),
      });

      if (!response.ok) throw new Error("Erro ao salvar recomendação.");
      console.log("Recomendação salva com sucesso!");
    } catch (error) {
      console.error("Erro ao salvar recomendação:", error);
    }
  }
}
