import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-history-page',
  standalone: true,
  imports: [
    RouterModule,
    CommonModule
  ],
  templateUrl: './history-page.component.html',
  styleUrl: './history-page.component.css'
})
export class HistoryPageComponent implements OnInit {
  userEmail: string | null = null;
  playlists: any[] = [];

  async ngOnInit() {
    this.userEmail = localStorage.getItem('userEmail');
    if (!this.userEmail) {
      console.warn('Usuário não autenticado!');
      window.location.href = '/login-page';
      return;
    }

    await this.fetchHistory();
  }

  async fetchHistory() {
    try {
      const response = await fetch(`http://localhost:5229/api/searchhistory/history/${this.userEmail}`);
      if (!response.ok) throw new Error('Erro ao buscar histórico.');

      this.playlists = await response.json();
      console.log('Histórico recebido:', this.playlists);
    } catch (error) {
      console.error('Erro ao carregar histórico:', error);
    }
  }

  openPlaylist(url: string) {
    window.open(url, "_blank");
  }
}
