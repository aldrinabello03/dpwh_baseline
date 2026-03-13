import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, DropdownModule],
  template: `
    <div class="data-table-wrapper">
      <div class="data-table__toolbar">
        <input pInputText type="text" (input)="onSearch($event)" placeholder="Search..." class="search-input" *ngIf="showSearch"/>
        <div class="toolbar-actions">
          <button pButton icon="pi pi-file-pdf" class="p-button-sm p-button-outlined" (click)="exportPdf.emit()" title="Export PDF" *ngIf="showExport"></button>
          <button pButton icon="pi pi-file-excel" class="p-button-sm p-button-outlined p-button-success" (click)="exportExcel.emit()" title="Export Excel" *ngIf="showExport"></button>
          <button pButton icon="pi pi-refresh" class="p-button-sm p-button-outlined" (click)="refresh.emit()" title="Refresh"></button>
        </div>
      </div>
      <p-table
        [value]="data"
        [columns]="columns"
        [paginator]="paginator"
        [rows]="rows"
        [totalRecords]="totalRecords"
        [lazy]="lazy"
        (onLazyLoad)="lazyLoad.emit($event)"
        [loading]="loading"
        [globalFilterFields]="filterFields"
        [responsiveLayout]="'stack'"
        styleClass="p-datatable-sm p-datatable-striped p-datatable-gridlines"
        #dt>
        <ng-template pTemplate="header">
          <tr>
            <th *ngFor="let col of columns" [pSortableColumn]="col.field">
              {{ col.header }}
              <p-sortIcon [field]="col.field" *ngIf="col.sortable !== false"></p-sortIcon>
            </th>
            <th *ngIf="showActions">Actions</th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-row>
          <tr>
            <td *ngFor="let col of columns">
              <span class="p-column-title">{{ col.header }}</span>
              {{ row[col.field] }}
            </td>
            <td *ngIf="showActions">
              <button pButton icon="pi pi-eye" class="p-button-sm p-button-text" (click)="view.emit(row)" title="View"></button>
              <button pButton icon="pi pi-pencil" class="p-button-sm p-button-text p-button-warning" (click)="edit.emit(row)" title="Edit" *ngIf="showEdit"></button>
              <button pButton icon="pi pi-trash" class="p-button-sm p-button-text p-button-danger" (click)="delete.emit(row)" title="Delete" *ngIf="showDelete"></button>
            </td>
          </tr>
        </ng-template>
        <ng-template pTemplate="emptymessage">
          <tr><td [attr.colspan]="columns.length + (showActions ? 1 : 0)" class="empty-message">No records found.</td></tr>
        </ng-template>
      </p-table>
    </div>
  `,
  styles: [`
    .data-table-wrapper { background: white; border-radius: 8px; overflow: hidden; }
    .data-table__toolbar { display: flex; justify-content: space-between; align-items: center; padding: 0.75rem 1rem; border-bottom: 1px solid #e9ecef; }
    .search-input { width: 280px; }
    .toolbar-actions { display: flex; gap: 0.5rem; }
    .empty-message { text-align: center; color: #999; padding: 2rem !important; }
  `]
})
export class DataTableComponent {
  @Input() data: any[] = [];
  @Input() columns: { field: string; header: string; sortable?: boolean }[] = [];
  @Input() paginator = true;
  @Input() rows = 20;
  @Input() totalRecords = 0;
  @Input() lazy = false;
  @Input() loading = false;
  @Input() showSearch = true;
  @Input() showExport = true;
  @Input() showActions = true;
  @Input() showEdit = true;
  @Input() showDelete = false;
  @Input() filterFields: string[] = [];

  @Output() view = new EventEmitter<any>();
  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();
  @Output() exportPdf = new EventEmitter<void>();
  @Output() exportExcel = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();
  @Output() lazyLoad = new EventEmitter<any>();
  @Output() search = new EventEmitter<string>();

  onSearch(event: Event): void {
    this.search.emit((event.target as HTMLInputElement).value);
  }
}
