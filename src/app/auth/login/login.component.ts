import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../Core/services/auth.service';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ToastrModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  form!: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

 onSubmit(): void {
  if (this.form.invalid) return;

  this.loading = true;
  this.auth.login(this.form.value).subscribe({
    next: (res) => {
      this.auth.storeToken(res.token);
      const user = this.auth.getUserInfo();

      const userName = user?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || 'User';
      this.toastr.success(`Welcome ${userName}`);

      const role = user?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || user?.['role'];

      if (role === 'Admin') {
        this.router.navigate(['/dashboard']);
      } else if (role === 'Member') {
        this.router.navigate(['/home']);
      } else {
        this.router.navigate(['/unauthorized']);
      }
    },
    error: () => {
      this.toastr.error('Invalid email or password');
      this.loading = false;
    }
  });
}

}
