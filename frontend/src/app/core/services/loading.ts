import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class Loading {
  private readonly loadingSignal = signal(false);
  readonly loading = this.loadingSignal.asReadonly();

  start(): void {
    this.loadingSignal.set(true);
  }

  stop(): void {
    this.loadingSignal.set(false);
  }
}
