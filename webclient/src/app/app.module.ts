import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";


import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ReactiveFormsModule } from "@angular/forms";
import { APP_BASE_HREF } from '@angular/common';

import { AppComponent } from "./app.component";
import { HelloComponent } from "./hello.component";

import { ApplyDataService } from './data.service';

import { HttpClientModule } from "@angular/common/http";
import { HomeComponent } from './home/home.component';


import { ConverComponent } from './conver/conver.component';

import { ManagerComponent } from './manager/manager.component';


import { AgGridModule } from 'ag-grid-angular';
import { ProductsComponent } from './manager/products/products.component';
import { ProductEditComponent } from './manager/products/product-edit/product-edit.component';
import { ManagerMenuComponent } from './manager/manager-menu/manager-menu.component';
import { PopupComponent } from './popup/popup.component';


import { environment } from '../environments/environment';
import {PathLocationStrategy, LocationStrategy,HashLocationStrategy } from '@angular/common';
import { WindowConfirmComponent } from './window-confirm/window-confirm.component';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule.forRoot([
      { path: "", component: HomeComponent },
      { path: "manager", component: ManagerComponent },

    ]),
    AgGridModule.withComponents([])
  ],
  declarations: [
    AppComponent,
    HelloComponent,
    HomeComponent,
    ConverComponent,
    ManagerComponent,
    ProductsComponent,
    ProductEditComponent,
    ManagerMenuComponent,
    PopupComponent, WindowConfirmComponent
  ],
  bootstrap: [AppComponent],
  providers: [
    ApplyDataService,
    {
      provide: APP_BASE_HREF,
      //useValue: '/'
      useValue: environment.publicBase //'/ap'       
      //useFactory: () => getBaseUrl()
    },
    {provide: LocationStrategy, useClass: HashLocationStrategy}
  ]
})
export class AppModule { }
