import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MonitoringService } from '../services/monitoring.service';
import { ApiService } from '../core/api.service';
import { finalize, forkJoin } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-device-status',
  imports: [CommonModule],
  templateUrl: './device-status.component.html',
  styleUrl: './device-status.component.scss'
})
export class DeviceStatusComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly monitoring = inject(MonitoringService);
  summary: any;
  details: any[] = [];
  selectedCategory = '';
  private allDetails: any[] = [];
  loading = false;
  error = '';
  lastRefreshed?: Date;
  readonly today = new Date();
  readonly antennaPorts = [1, 2, 3, 4, 5, 6, 7, 8];
  deviceTypes = [
    'Tagging Station', 'Tagging Read Point', 'Dog House Airside', 'Dog House Landside',
    'Exit Gate', 'Inside Lounge', 'Exit Lounge', 'BHS Return Feed', 'Recheck Station'
  ].map(category => ({ category, total: 0, online: 0, offline: 0 }));
  chartMaximum = 4;
  chartTicks = [4, 3, 2, 1, 0];
  systemOverview: { total: number; online: number; offline: number; onlinePercent: number; offlinePercent: number; background: string } | null = null;
  readonly categories = [
    { key: 'taggingStations', category: 'Tagging Station', title: 'Total Tagging Stations', icon: '▣', color: 'blue' },
    { key: 'readers', category: 'Reader', title: 'Total Readers', icon: '◉', color: 'green' },
    { key: 'antennas', category: 'Antenna', title: 'Total Antennas', icon: '⋔', color: 'purple' },
    { key: 'controllers', category: 'Controller', title: 'Total Controllers', icon: '▦', color: 'orange' }
  ];

  ngOnInit(): void {
    this.monitoring.devices.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(value => this.update(value.summary, value.details ?? []));
    this.load();
  }

  selectCategory(category = ''): void {
    this.selectedCategory = category;
    this.details = this.allDetails.filter(item => category ? item.category === category : item.deviceType != null);
  }

  load(): void {
    if (this.loading) return;
    this.loading = true;
    this.error = '';
    const started = Date.now();
    forkJoin({ summary: this.api.get<any>('device-status/summary'), details: this.api.get<any[]>('device-status/details') })
      .pipe(takeUntilDestroyed(this.destroyRef), finalize(() => this.loading = false)).subscribe({
        next: value => {
          if (!this.lastRefreshed || this.lastRefreshed.getTime() <= started)
            this.update(value.summary, value.details ?? []);
        },
        error: () => this.error = 'Unable to refresh device status. Please try again.'
      });
  }

  private update(summary: any, details: any[]): void {
    this.summary = summary;
    const counts = this.categories.map(category => summary?.[category.key]);
    // Use only the four KPI populations, never logical-device rows that duplicate readers.
    if (counts.every(count => count && Number.isInteger(count.total) && Number.isInteger(count.online)
      && count.total >= 0 && count.online >= 0 && count.online <= count.total)) {
      const total = counts.reduce((sum, count) => sum + count.total, 0);
      const online = counts.reduce((sum, count) => sum + count.online, 0);
      const offline = total - online;
      const onlinePercent = total ? online / total * 100 : 0;
      const offlinePercent = total ? offline / total * 100 : 0;
      this.systemOverview = { total, online, offline, onlinePercent, offlinePercent,
        background: total ? `conic-gradient(#00a77b 0% ${onlinePercent}%, #df1025 ${onlinePercent}% 100%)` : 'transparent' };
    } else {
      this.systemOverview = null;
    }
    this.allDetails = details;
    this.deviceTypes = this.deviceTypes.map(row => {
      const devices = details.filter(device => device.deviceType != null && device.category === row.category);
      const online = devices.filter(device => device.status === 'Online').length;
      const offline = devices.filter(device => device.status === 'Offline').length;
      return { ...row, total: devices.length, online, offline };
    });
    const maximum = Math.max(1, ...this.deviceTypes.flatMap(row => [row.online, row.offline]));
    const step = Math.ceil(maximum / 4);
    this.chartMaximum = step * 4;
    this.chartTicks = [4, 3, 2, 1, 0].map(value => value * step);
    this.selectCategory(this.selectedCategory);
    this.lastRefreshed = new Date();
    this.error = '';
  }
}
