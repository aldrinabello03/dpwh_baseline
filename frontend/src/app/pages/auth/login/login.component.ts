import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { MessageModule } from 'primeng/message';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonModule, InputTextModule, PasswordModule, MessageModule],
  template: `
    <div class="login-page">
      <div class="login-card">
        <!-- Logo -->
        <div class="login-card__logo">
          <img src="assets/images/dpwh-logo.svg" alt="DPWH Logo" class="login-card__logo-img"/>
        </div>

        <!-- Title -->
        <div class="login-card__title">
          <h1>DPWH HRIS</h1>
          <p>Human Resource Information System</p>
          <p class="login-card__subtitle">Department of Public Works and Highways</p>
        </div>

        <!-- Form -->
        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="login-card__form">
          <div class="form-group">
            <label for="username">Username or Email</label>
            <div class="p-input-icon-left">
              <i class="pi pi-user"></i>
              <input
                pInputText
                id="username"
                type="text"
                formControlName="username"
                placeholder="Enter your username or email"
                class="w-full"/>
            </div>
            <small class="error" *ngIf="loginForm.get('username')?.invalid && loginForm.get('username')?.touched">
              Username is required.
            </small>
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <p-password
              id="password"
              formControlName="password"
              placeholder="Enter your password"
              [feedback]="false"
              [toggleMask]="true"
              styleClass="w-full">
            </p-password>
            <small class="error" *ngIf="loginForm.get('password')?.invalid && loginForm.get('password')?.touched">
              Password is required.
            </small>
          </div>

          <!-- Error message -->
          <div class="login-error" *ngIf="errorMessage">
            <i class="pi pi-exclamation-triangle"></i>
            {{ errorMessage }}
          </div>

          <button
            pButton
            type="submit"
            label="Sign In"
            icon="pi pi-sign-in"
            class="login-btn"
            [loading]="loading"
            [disabled]="loginForm.invalid || loading">
          </button>
        </form>

        <div class="login-card__footer">
          <small>For account access, contact the DPWH-HRAS Office</small>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-page {
      min-height: 100vh;
      background: linear-gradient(135deg, #1B2A4A 0%, #1B3A6B 50%, #2C4A8A 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 2rem;
    }
    .login-card {
      background: white;
      border-radius: 16px;
      padding: 3rem 2.5rem;
      width: 100%;
      max-width: 440px;
      box-shadow: 0 20px 60px rgba(0,0,0,0.3);
    }
    .login-card__logo {
      text-align: center;
      margin-bottom: 1.5rem;
    }
    .login-card__logo-img {
      width: 100px;
      height: 100px;
    }
    .login-card__title {
      text-align: center;
      margin-bottom: 2rem;
    }
    .login-card__title h1 {
      font-size: 1.8rem;
      font-weight: 800;
      color: #1B2A4A;
      margin: 0 0 0.25rem;
    }
    .login-card__title p {
      color: #555;
      font-size: 0.9rem;
      margin: 0.25rem 0;
    }
    .login-card__subtitle { color: #888 !important; font-size: 0.8rem !important; }
    .login-card__form { display: flex; flex-direction: column; gap: 1.25rem; }
    .form-group { display: flex; flex-direction: column; gap: 0.4rem; }
    .form-group label { font-size: 0.875rem; font-weight: 600; color: #333; }
    .error { color: #dc3545; font-size: 0.75rem; }
    .login-error {
      background: #fff5f5;
      border: 1px solid #fecdd2;
      border-radius: 8px;
      padding: 0.75rem 1rem;
      color: #dc3545;
      font-size: 0.875rem;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    .login-btn {
      width: 100%;
      padding: 0.75rem !important;
      font-size: 1rem !important;
      font-weight: 600 !important;
      background: #E85D26 !important;
      border-color: #E85D26 !important;
    }
    .login-btn:hover { background: #c74d1a !important; border-color: #c74d1a !important; }
    .login-card__footer { text-align: center; margin-top: 1.5rem; color: #aaa; }
  `]
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  errorMessage = '';
  returnUrl = '/dashboard';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
    if (this.authService.isLoggedIn()) {
      this.router.navigate([this.returnUrl]);
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;
    this.loading = true;
    this.errorMessage = '';

    const { username, password } = this.loginForm.value;
    this.authService.login(username, password).subscribe({
      next: (result) => {
        this.loading = false;
        if (result.success) {
          this.router.navigate([this.returnUrl]);
        } else {
          this.errorMessage = result.message || 'Login failed. Please try again.';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error?.message || 'Login failed. Please check your credentials.';
      }
    });
  }
}
