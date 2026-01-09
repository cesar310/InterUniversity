import { Component, input } from '@angular/core';
import { ButtonModule } from 'primeng/button';

export interface HeaderButton {
  label: string;
  icon?: string;
  severity?: 'success' | 'info' | 'danger' | 'secondary';
  action: () => void;
}

@Component({
  selector: 'app-page-header',
  imports: [ButtonModule],
  templateUrl: './page-header.html',
  styleUrl: './page-header.css',
})
export class PageHeader {
  readonly title = input.required<string>();
  readonly buttons = input<HeaderButton[]>([]);
}
