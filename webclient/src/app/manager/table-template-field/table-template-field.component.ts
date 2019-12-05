import { Component } from '@angular/core';
import { ManagerBase } from 'src/app/wizard';

@Component({
  selector: 'app-table-template-field',
  templateUrl: './table-template-field.component.html',
  styleUrls: ['./table-template-field.component.scss']
})
export class TableTemplateFieldComponent extends ManagerBase {

  constructor() { super() }

  typename = "TableTemplateField"

  ngOnInit() {
    console.log(this.ds)
    if (this.typename) {
      this.ds.data[this.typename + '_list'] = null
      this.ds.data[this.typename] = null
      this.ds.loadData(this.typename,"1");
    }
  }

  columnDefs = [
    { headerName: '名称', field: 'Name' },
    { headerName: '标题', field: 'Title' },
    { headerName: '类型', field: 'Type' },
    { headerName: '长度', field: 'Length' },
  ]
 

}
