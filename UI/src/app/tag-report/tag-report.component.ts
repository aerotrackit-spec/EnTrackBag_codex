import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
@Component({
  selector: "app-tag-report",
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tag-report.component.html',
  styleUrl: './tag-report.component.scss',
})
export class TagReportComponent {
  readonly today = new Date();
  tagId = '';
  prohibited = false;
  normal = true;
  from = this.localDate(new Date(new Date().setHours(0, 0, 0, 0)));
  to = this.localDate(new Date());
  historyOpen = true;
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

  search(form: NgForm): void {
    this.submitted = true;
    if (form.invalid || this.invalidPeriod || !this.hasBagType) return;
    this.message = 'Report search is not connected yet. No query has been sent and no results have been loaded.';
  }

  clear(form: NgForm): void {
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
