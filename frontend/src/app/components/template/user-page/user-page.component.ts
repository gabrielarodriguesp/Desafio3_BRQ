import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MusicSearchModalComponent } from './music-search-modal/music-search-modal.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-page',
  standalone: true,
  imports: [
    RouterModule,
    MusicSearchModalComponent,
    CommonModule
  ],
  templateUrl: './user-page.component.html',
  styleUrl: './user-page.component.css'
})
export class UserPageComponent {
  selectedSongs: { name?: string; artist?: string; albumCover?: string }[] = [{}, {}, {}, {}, {}];
  showModal = false;
  selectedIndex: number | null = null;
  isLoading = false;

  openModal(index: number) {
    this.selectedIndex = index;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  async getSongInfo(songName: string) {
    if (this.selectedIndex === null) return;

    try {
      const songAlreadyExists = this.selectedSongs.some(
        (song) => song.name?.toLowerCase() === songName.toLowerCase()
      );

      if (songAlreadyExists) {
        alert('Essa música já foi adicionada!');
        return;
      }

      const response = await fetch(`http://localhost:5229/api/music/search?name=${encodeURIComponent(songName)}`);
      const data = await response.json();
      console.log(data);

      this.selectedSongs[this.selectedIndex] = {
        name: data.name,
        artist: data.artist,
        albumCover: data.albumCover
      };
    } catch (error) {
      console.error('Erro ao buscar música:', error);
    }

    this.closeModal();
  }

  removeSong(index: number, event: Event) {
    event.stopPropagation();
    this.selectedSongs[index] = {};
  }

  async searchPlaylist() {
    const email = localStorage.getItem('userEmail');
    if (!email) {
      console.warn('Usuário não autenticado! Redirecionando para login...');
      window.location.href = '/login-page';
    }

    const filteredSongs = this.selectedSongs.filter(song => song.name);
    if (filteredSongs.length === 0) {
      alert('Nenhuma música foi selecionada!');
      return;
    }

    const searchData = {
      email: email,
      songs: filteredSongs.map(song => song.name)
    };

    this.isLoading = true;

    try {
      const searchResponse = await fetch('http://localhost:5229/api/searchhistory/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(searchData),
      });

      const searchDataResult = await searchResponse.json();
      console.log('Pesquisa registrada:', searchDataResult);

      // Recomenda uma playlist
      const playlistResponse = await fetch('http://localhost:5229/api/playlist/recommend', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(filteredSongs.map(song => song.name))
      });

      if (!playlistResponse.ok) {
        const errorText = await playlistResponse.text();

        if (playlistResponse.status === 400) {
          alert("Não foi possível encontrar um gênero comum.");
        } else if (playlistResponse.status === 500) {
          alert("Não conseguimos encontrar playlist :(");
        }

        throw new Error(`Erro do servidor: ${errorText}`);
      }

      const playlistData = await playlistResponse.json();
      console.log("Resposta do backend:", playlistData);

      if (playlistData.playlistUrl) {
        localStorage.setItem('playlistUrl', playlistData.playlistUrl);

        await fetch('http://localhost:5229/api/searchhistory/recommendation', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            email: email,
            playlistUrl: playlistData.playlistUrl
          }),
        });

        console.log('Recomendação registrada com sucesso!');
        window.location.href = '/playlist-page';
      } else {
        console.error("Nenhuma playlist foi retornada.");
      }
    } catch (error) {
      console.error('Erro ao buscar playlist:', error);
    } finally {
      this.isLoading = false;
    }
  }
}
