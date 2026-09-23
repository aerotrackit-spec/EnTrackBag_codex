import { bootstrapApplication } from "@angular/platform-browser";
import { provideRouter } from "@angular/router";
import {
  provideHttpClient,
  withInterceptors,
} from "@angular/common/http";
import { AppComponent } from "./app/app.component";
import { routes } from "./app/app.routes";
import { authInterceptor } from "./app/core/auth.interceptor";
import { correlationInterceptor } from "./app/core/correlation.interceptor";
import { retryInterceptor } from "./app/core/retry.interceptor";

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([
        correlationInterceptor,
        retryInterceptor,
        authInterceptor,
      ]),
    ),
  ],
}).catch(console.error);
