import { Component, OnInit } from '@angular/core';
import { BaseEdit } from 'src/app/wizard'

@Component({ 
  selector: 'app-table-template-edit',
  templateUrl: './table-template-edit.component.html',
  styleUrls: ['./table-template-edit.component.scss']
})
export class TableTemplateEditComponent extends BaseEdit implements OnInit {

  constructor() { super() }

  ngOnInit() {
  }

}
