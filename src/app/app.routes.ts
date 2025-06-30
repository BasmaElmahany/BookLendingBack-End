import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { HomeComponent } from './landing/home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { CatalogComponent } from './app/catalog/catalog.component';
import { DashboardComponent } from './app/dashboard/dashboard/dashboard.component';
import { BookComponent } from './app/book/book.component';
import { BookDetailsComponent } from './app/book-details/book-details.component';
import { DelayedBooksComponent } from './app/delayed-books/delayed-books.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'home', component: HomeComponent },
     { path: 'catalog', component: CatalogComponent },
      { path: 'book', component: BookComponent },
     { path: 'dashboard', component: DashboardComponent },
      { path: 'delayedbooks', component: DelayedBooksComponent },
     { path: 'catalog/:id', component: HomeComponent },
      { path: 'book/:id', component: BookDetailsComponent },
    { path: '', redirectTo: 'login', pathMatch: 'full' }
];
