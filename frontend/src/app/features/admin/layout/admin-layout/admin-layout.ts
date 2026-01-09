import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MenuItem } from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { DrawerModule } from 'primeng/drawer';
import { Auth } from '../../../../core/services/auth';

@Component({
  selector: 'app-admin-layout',
  imports: [CommonModule, RouterOutlet, MenuModule, AvatarModule, ButtonModule, DrawerModule],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
})
export class AdminLayout {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  readonly currentUser = this.authService.currentUser;
  readonly sidebarVisible = signal(true);

  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: ['/admin/dashboard'],
      command: () => { if (window.innerWidth < 1024) this.sidebarVisible.set(false); }
    },
    {
      separator: true
    },
    {
      label: 'Estudiantes',
      icon: 'pi pi-users',
      routerLink: ['/admin/students'],
      command: () => { if (window.innerWidth < 1024) this.sidebarVisible.set(false); }
    },
    {
      label: 'Profesores',
      icon: 'pi pi-id-card',
      routerLink: ['/admin/professors'],
      command: () => { if (window.innerWidth < 1024) this.sidebarVisible.set(false); }
    },
    {
      label: 'Materias',
      icon: 'pi pi-book',
      routerLink: ['/admin/subjects'],
      command: () => { if (window.innerWidth < 1024) this.sidebarVisible.set(false); }
    },
    {
      separator: true
    },
    {
      label: 'Configuración',
      icon: 'pi pi-cog',
      routerLink: ['/admin/system-config'],
      command: () => { if (window.innerWidth < 1024) this.sidebarVisible.set(false); }
    }
  ];

  toggleSidebar(): void {
    this.sidebarVisible.update(v => !v);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  getUserInitials(): string {
    const user = this.currentUser();
    if (!user?.email) return '?';
    return user.email.substring(0, 2).toUpperCase();
  }
}
