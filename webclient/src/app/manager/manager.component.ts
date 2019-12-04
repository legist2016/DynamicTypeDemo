import { Component, OnInit } from '@angular/core';
import { localeText } from "../aggrid.localtext"

@Component({
  selector: 'app-manager',
  templateUrl: './manager.component.html',
  styleUrls: ['./manager.component.scss']
})
export class ManagerComponent implements OnInit {

  //content="products"
  content="pending orders"

  localeText = localeText

  constructor() { }

  ngOnInit() {
  }

}
