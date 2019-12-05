import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";


import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ReactiveFormsModule } from "@angular/forms";
import { APP_BASE_HREF } from '@angular/common';

import { AppComponent } from "./app.component";

import { DataService } from './data.service';

import { HttpClientModule } from "@angular/common/http";
import { HomeComponent } from './home/home.component';


import { ConverComponent } from './conver/conver.component';

import { ManagerComponent } from './manager/manager.component';


import { AgGridModule } from '@ag-grid-community/angular';
import { ProductsComponent } from './manager/products/products.component';
import { ProductEditComponent } from './manager/products/product-edit/product-edit.component';
import { ManagerMenuComponent } from './manager/manager-menu/manager-menu.component';
import { PopupComponent } from './popup/popup.component';


import { environment } from '../environments/environment';
import {PathLocationStrategy, LocationStrategy,HashLocationStrategy } from '@angular/common';
import { WindowConfirmComponent } from './window-confirm/window-confirm.component';
import { TableTemplateComponent } from './manager/table-template/table-template.component';
import { TableTemplateEditComponent } from './manager/table-template/table-template-edit/table-template-edit.component';
import { AgButtonsCellRanderComponent } from './ag-buttons-cell-rander/ag-buttons-cell-rander.component';
import { TableTemplateFieldComponent } from './manager/table-template-field/table-template-field.component';

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
    AgGridModule.withComponents([AgButtonsCellRanderComponent])
  ],
  declarations: [
    AppComponent,
    HomeComponent,
    ConverComponent,
    ManagerComponent,
    ProductsComponent,
    ProductEditComponent,
    ManagerMenuComponent,
    PopupComponent, WindowConfirmComponent, TableTemplateComponent, TableTemplateEditComponent, AgButtonsCellRanderComponent, TableTemplateFieldComponent, 
  ],
  bootstrap: [AppComponent],
  providers: [
    DataService,
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
