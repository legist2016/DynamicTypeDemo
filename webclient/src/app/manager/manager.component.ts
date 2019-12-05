import { Component, OnInit } from '@angular/core';
import { localeText } from "../aggrid.localtext"
import { DataService } from 'src/app/data.service';

@Component({
  selector: 'app-manager',
  templateUrl: './manager.component.html',
  styleUrls: ['./manager.component.scss']
})
export class ManagerComponent implements OnInit {

  //content="products"
  content="products"

  localeText = localeText

  constructor(public ds:DataService) { }

  ngOnInit() {
  }

}
