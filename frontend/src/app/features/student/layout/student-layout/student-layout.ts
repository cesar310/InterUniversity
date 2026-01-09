import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MenuItem } from 'primeng/api';
import { MenubarModule } from 'primeng/menubar';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { DrawerModule } from 'primeng/drawer';
import { MenuModule } from 'primeng/menu';
import { Auth } from '../../../../core/services/auth';

@Component({
  selector: 'app-student-layout',
  imports: [CommonModule, RouterOutlet, MenubarModule, AvatarModule, ButtonModule, DrawerModule, MenuModule],
  templateUrl: './student-layout.html',
  styleUrl: './student-layout.css',
})
export class StudentLayout {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  readonly currentUser = this.authService.currentUser;
  readonly mobileMenuVisible = signal(false);

  menuItems: MenuItem[] = [
    {
      label: 'Mis Inscripciones',
      icon: 'pi pi-list',
      routerLink: ['/student/enrollments'],
      command: () => this.mobileMenuVisible.set(false)
    },
    {
      label: 'Materias Disponibles',
      icon: 'pi pi-book',
      routerLink: ['/student/subjects'],
      command: () => this.mobileMenuVisible.set(false)
    }
  ];

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  getUserInitials(): string {
    const user = this.currentUser();
    if (!user?.email) return '?';
    return user.email.substring(0, 2).toUpperCase();
  }

  toggleMobileMenu(): void {
    this.mobileMenuVisible.update(v => !v);
  }
}
