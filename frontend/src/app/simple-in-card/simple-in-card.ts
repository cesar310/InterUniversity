import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-simple-in-card',
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    CheckboxModule,
    InputTextModule
  ],
  templateUrl: './simple-in-card.html',
  styleUrl: './simple-in-card.css',
})
export class SimpleInCard {

  checked1 = signal<boolean>(true);

}
