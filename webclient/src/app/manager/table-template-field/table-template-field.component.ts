import { Component } from '@angular/core';
import { ManagerBase } from 'src/app/wizard';

@Component({
  selector: 'app-table-template-field',
  templateUrl: './table-template-field.component.html',
  styleUrls: ['./table-template-field.component.scss']
})
export class TableTemplateFieldComponent extends ManagerBase {

  constructor() { super() 
    this.context['buttons'] = [{ key: 'delete', name: '删除' }];
  }

  typename = "TableTemplateField"

  ngOnInit() {
    console.log(this.ds)
    if (this.typename) {
      this.ds.data[this.typename + '_list'] = [{},{}]
      this.ds.data[this.typename] = null
      //this.ds.loadData(this.typename,"1");
    }
  }
  defaultColDef = {
    editable: true,
    resizable: true
  }
  columnDefs = [
    { headerName: "操作", field: '', cellRenderer: "childMessageRenderer",width:60 ,editable:false},
    { headerName: '名称', field: 'Name' ,width:120 },
    { headerName: '标题', field: 'Title' ,width:120 },
    { headerName: '类型', field: 'Type' ,width:120 , cellEditor:'agSelectCellEditor', cellEditorParams:{values:['1','2']} , 
    valueFormatter:(params)=>{return ['','整型','字符型'][params.value]}},
    { headerName: '长度', field: 'Length' ,width:120 },
  ]
 
  rowValueChanged(event){
    console.log('rowValueChanged', event)
  }

  methodFromParent(event,node){
    console.log('methodFromParent', event,node)
  }

  rowEditingStopped(event){
    console.log('rowEditingStopped', event)
  }
}
