import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CatalogService } from '../../Core/services/catalog.service';
import { Catalog } from '../../Core/models/catalog.model';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ToastrService, ToastrModule } from 'ngx-toastr';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ToastrModule
  ],
})
export class CatalogComponent implements OnInit {
  catalogForm!: FormGroup;
  catalogs: Catalog[] = [];
  selectedId: number | null = null;

  constructor(
    private catalogService: CatalogService,
    private fb: FormBuilder,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.catalogForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
    });

    this.loadCatalogs();
  }

  loadCatalogs(): void {
    this.catalogService.getAll().subscribe(data => {
      this.catalogs = data;
    });
  }

  submit(): void {
  const catalog: Catalog = {
    id: this.selectedId ?? 0,
    name: this.catalogForm.value.name,
    description: this.catalogForm.value.description,
  };

  if (this.selectedId) {
    this.catalogService.update(this.selectedId, catalog).subscribe(res => {
      const index = this.catalogs.findIndex(c => c.id === this.selectedId);
      if (index !== -1 && res.data) {
        this.catalogs[index] = res.data;
      }
      this.toastr.success(`✅ Catalog "${res.data?.name}" updated successfully`, 'Update Successful!');
      this.resetForm();
    });
  } else {
    this.catalogService.create(catalog).subscribe(res => {
      if (res.data) {
        this.catalogs.push(res.data);
      }
      this.toastr.success(`🎉 Catalog "${res.data?.name}" added successfully`, 'Created!');
      this.resetForm();
    });
  }
}

  edit(catalog: Catalog): void {
    this.selectedId = catalog.id;
    this.catalogForm.patchValue({
      name: catalog.name,
      description: catalog.description
    });

    const formEl = document.getElementById('catalog-form');
    formEl?.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

delete(id: number): void {
  const catalogName = this.catalogs.find(c => c.id === id)?.name;

  this.catalogService.delete(id).subscribe(res => {
    this.catalogs = this.catalogs.filter(c => c.id !== id);
    this.toastr.success(`🗑️ Catalog "${catalogName}" was deleted successfully`, 'Deleted!');
  });
}

  resetForm(): void {
    this.selectedId = null;
    this.catalogForm.reset();
  }
}
