import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../Core/services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ToastrModule
  ],
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  form!: FormGroup;
  message: string | null = null;
  isSuccess: boolean = false;
  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}
  ngOnInit(): void {
    this.form = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      role: ['', Validators.required] 
    });
  }
  

  onSubmit() {
    if (this.form.invalid) return;
  
    const { fullName, email, password, role } = this.form.value;
  
    this.auth.register({ fullName, email, password, role }).subscribe({
      next: (res) => {
        this.message = 'Registration successful! Check your email.';
        this.isSuccess = true;
        // Optional: also use Toastr
        this.toastr.success(this.message);
        // Optional delay before redirect:
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        console.error('Registration error:', err.error);
        this.message = err.error || 'Registration failed. Please try again.';
        this.isSuccess = false;
        if (this.message) {
          this.toastr.error(this.message);
        } else {
          this.toastr.error('An unexpected error occurred.');
        }
      }
    });
  }
  
  

}