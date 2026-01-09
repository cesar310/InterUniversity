import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-button-demo',
  imports: [ButtonModule],
  templateUrl: './button-demo.html',
  styleUrl: './button-demo.css',
})
export class ButtonDemo {

  toggleDarkMode() {
    const element: HTMLElement | null = document.querySelector('html');
    if (element) {
      element.classList.toggle('my-app-dark');
    }
  }

}
