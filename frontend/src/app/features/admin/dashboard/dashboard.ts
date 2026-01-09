import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { SkeletonModule } from 'primeng/skeleton';
import { PrimeIcons } from 'primeng/api';
import { Dashboard as DashboardService } from '../services/dashboard';
import { SystemConfigService } from '../../../core/services/system-config';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, CardModule, SkeletonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  readonly configService = inject(SystemConfigService);

  readonly stats = this.dashboardService.stats;
  readonly loading = this.dashboardService.loading;

  // Iconos usando constantes de PrimeIcons
  readonly icons = {
    users: PrimeIcons.USERS,
    idCard: PrimeIcons.ID_CARD,
    book: PrimeIcons.BOOK,
    checkCircle: PrimeIcons.CHECK_CIRCLE,
    chartLine: PrimeIcons.CHART_LINE,
    star: PrimeIcons.STAR,
    cog: PrimeIcons.COG
  };

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe();
  }
}
