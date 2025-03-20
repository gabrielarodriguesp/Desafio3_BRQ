import { ApplicationConfig } from '@angular/core';
import { provideRouter, Routes } from '@angular/router';
import { importProvidersFrom } from '@angular/core';

const routes: Routes = [
  { path: '', redirectTo: 'first-page', pathMatch: 'full' },
  { path: 'first-page', loadComponent: () => import('./components/template/first-page/first-page.component').then(m => m.FirstPageComponent) },
  { path: 'second-page', loadComponent: () => import('./components/template/second-page/second-page.component').then(m => m.SecondPageComponent) },
  { path: 'login-page', loadComponent: () => import('./components/template/login-page/login-page.component').then(m => m.LoginPageComponent) },
  { path: 'user-page', loadComponent: () => import('./components/template/user-page/user-page.component').then(m => m.UserPageComponent) },
  { path: 'history-page', loadComponent: () => import('./components/template/history-page/history-page.component').then(m => m.HistoryPageComponent) },
  { path: 'playlist-page', loadComponent: () => import('./components/template/playlist-page/playlist-page.component').then(m => m.PlaylistPageComponent) }


];

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes)]
};
