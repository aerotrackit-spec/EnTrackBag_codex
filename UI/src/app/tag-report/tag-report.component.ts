import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnDestroy, ViewChild, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ApiService } from '../core/api.service';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { environment } from '../../environments/environment';

export interface TagHistoryEntry {
  time: string | null;
  alarmType: string | null;
  description: string | null;
  location: string | null;
  smis: boolean | null;
}

export interface TagReportRow {
  tagId: string;
  threat: string | null;
  alarmType: string | null;
  location: string | null;
  lastSeen: string | null;
  count: number;
  // Undefined means history has not been loaded, not that no events exist.
  history?: TagHistoryEntry[];
}
@Component({
  selector: "app-tag-report",
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tag-report.component.html',
  styleUrl: './tag-report.component.scss',
})
export class TagReportComponent implements OnDestroy {
  private readonly api = inject(ApiService);
  private readonly http = inject(HttpClient);
  readonly auth = inject(AuthService);
  private exportRequest?: Subscription;
  exporting = false;
  exportError = '';
  private searchRequest?: Subscription;
  private historyRequest?: Subscription;
  private searchParameters: URLSearchParams | null = null;
  loading = false;
  searched = false;
  historyLoading = false;
  historyError = '';
  historyHeader: { globalId: string | null; iataCode: string | null; lastStage: number | null; lastSeenLocation: string | null } | null = null;
  total = 0;
  page = 1;
  readonly pageSize = 50;
  readonly today = new Date();
  tagId = '';
  prohibited = false;
  normal = true;
  from = this.localDate(new Date(new Date().setHours(0, 0, 0, 0)));
  to = this.localDate(new Date());
  @ViewChild('historyDialog') private historyDialog!: ElementRef<HTMLDialogElement>;
  results: TagReportRow[] = [];
  selectedTag: TagReportRow | null = null;
  submitted = false;
  message = '';
  readonly alarms = [
    { name: 'Delay Proper Path', selected: true },
    { name: 'Delay Improper Path', selected: true },
    { name: 'Missed Luggage', selected: true }
  ];

  get selectedCount(): number { return this.alarms.filter(alarm => alarm.selected).length; }
  get invalidPeriod(): boolean { return !!this.from && !!this.to && this.from > this.to; }
  get hasBagType(): boolean { return this.normal || this.prohibited || this.selectedCount > 0; }

  selectAll(selected: boolean): void { this.alarms.forEach(alarm => alarm.selected = selected); }

  openHistory(row: TagReportRow): void {
    if (!this.results.includes(row)) return;
    this.historyRequest?.unsubscribe();
    this.selectedTag = row;
    row.history = undefined;
    this.historyHeader = null;
    this.historyError = '';
    this.historyLoading = true;
    this.historyDialog.nativeElement.showModal();
    this.historyRequest = this.api.get<{ entries: TagHistoryEntry[]; globalId: string | null; iataCode: string | null; lastStage: number | null; lastSeenLocation: string | null }>(
      `tag-report/history?tagId=${encodeURIComponent(row.tagId)}`).subscribe({
      // API follows the legacy newest-first table; show the journey left-to-right, oldest first.
      next: history => { row.history = [...history.entries].reverse(); this.historyHeader = history; this.historyLoading = false; },
      error: error => { this.historyLoading = false; this.historyError = error?.error?.message ?? 'Tag history could not be loaded. Please try again.'; }
    });
  }

  closeHistory(): void { this.historyDialog.nativeElement.close(); }
  onHistoryClosed(): void { this.historyRequest?.unsubscribe(); this.selectedTag = null; this.historyLoading = false; this.historyHeader = null; }

  ngOnDestroy(): void { this.searchRequest?.unsubscribe(); this.historyRequest?.unsubscribe(); this.exportRequest?.unsubscribe(); }

  exportCsv(): void {
    if (!this.searchParameters || this.loading || this.exporting || !this.total || !this.auth.hasAccess('TagReport', 'EXPORT')) return;
    this.exporting = true;
    this.exportError = '';
    // Use the submitted search snapshot, not unsaved filter edits or the current page.
    const parameters = new URLSearchParams(this.searchParameters);
    parameters.delete('page');
    parameters.delete('pageSize');
    this.exportRequest = this.http.get(`${environment.apiUrl.replace(/\/$/, '')}/tag-report/export?${parameters}`, { responseType: 'blob' }).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `TagReport-${new Date().toISOString().slice(0, 10)}.csv`;
        document.body.appendChild(link);
        link.click();
        link.remove();
        setTimeout(() => URL.revokeObjectURL(url), 1000);
        this.exporting = false;
      },
      error: error => {
        this.exporting = false;
        this.exportError = error.status === 403 ? 'You do not have permission to export this report.'
          : error.status === 400 ? 'Export could not be completed. Narrow the date range or enter an exact Tag ID and search again.'
          : 'Export failed. Please try again.';
      }
    });
  }

  search(form: NgForm): void {
    this.submitted = true;
    if (this.loading || form.invalid || this.invalidPeriod || !this.hasBagType) return;
    this.exportRequest?.unsubscribe();
    this.exporting = false;
    this.exportError = '';
    this.searchParameters = new URLSearchParams({ tagId: this.tagId.trim(), from: this.from, to: this.to,
      prohibited: String(this.prohibited), normal: String(this.normal),
      delayProperPath: String(this.alarms[0].selected), delayImproperPath: String(this.alarms[1].selected),
      missedLuggage: String(this.alarms[2].selected), pageSize: String(this.pageSize) });
    this.loadPage(1);
  }

  loadPage(page: number): void {
    if (!this.searchParameters || this.loading || page < 1) return;
    this.searchRequest?.unsubscribe();
    this.loading = true;
    this.searched = true;
    this.message = '';
    this.results = [];
    this.total = 0;
    this.page = page;
    const parameters = new URLSearchParams(this.searchParameters);
    parameters.set('page', String(page));
    this.searchRequest = this.api.get<{ items: TagReportRow[]; total: number; page: number }>(`tag-report?${parameters}`).subscribe({
      next: result => { this.results = result.items; this.total = result.total; this.page = result.page; this.loading = false; },
      error: error => { this.loading = false; this.message = error?.error?.message ?? 'Report search failed. Please check the filters and try again.'; }
    });
  }

  clear(form: NgForm): void {
    this.exportRequest?.unsubscribe();
    this.exporting = false;
    this.exportError = '';
    this.searchRequest?.unsubscribe();
    this.loading = false;
    this.searched = false;
    this.results = [];
    this.total = 0;
    this.page = 1;
    this.searchParameters = null;
    this.tagId = '';
    this.prohibited = false;
    this.normal = true;
    this.selectAll(true);
    this.from = this.localDate(new Date(new Date().setHours(0, 0, 0, 0)));
    this.to = this.localDate(new Date());
    this.submitted = false;
    this.message = '';
    form.resetForm({ tagId: this.tagId, prohibited: this.prohibited, normal: this.normal, from: this.from, to: this.to });
  }

  private localDate(date: Date): string {
    return new Date(date.getTime() - date.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
  }
}
