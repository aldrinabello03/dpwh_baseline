import { HttpInterceptorFn, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { StorageService } from '../services/storage.service';

export const jwtInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn) => {
  const storage = inject(StorageService);
  const token = storage.getToken();
  if (token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
  return next(req);
};

export const errorInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn) => {
  const router = inject(Router);
  const storage = inject(StorageService);
  return next(req).pipe(
    catchError(error => {
      if (error.status === 401) {
        storage.clear();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
