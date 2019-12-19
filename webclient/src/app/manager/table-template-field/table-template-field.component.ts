import { Component, EventEmitter, Output, Input } from '@angular/core';
import { ManagerBase } from 'src/app/wizard';

@Component({
  selector: 'app-table-template-field',
  templateUrl: './table-template-field.component.html',
  styleUrls: ['./table-template-field.component.scss']
})
export class TableTemplateFieldComponent extends ManagerBase {
  @Input() template

  constructor() {
    super()
    this.context['buttons'] = [{ key: 'delete', name: '删除' }];
  }
  @Output() save = new EventEmitter()
  @Output() close = new EventEmitter()

  typename = "TableTemplateField"

  ngOnInit() {
    //console.log(this.ds)
    if (this.typename) {
      this.ds.data[this.typename + '_list'] = null
      this.ds.data[this.typename] = null
      this.ds.loadData(this.typename,`${this.template.Id}/fields`);
    }
  }
  defaultColDef = {
    editable: true,
    resizable: true
  }
  columnDefs = [
    { headerName: "操作", field: '', cellRenderer: "childMessageRenderer", width: 60, editable: false },
    { headerName: '名称', field: 'Name', width: 120 },
    { headerName: '标题', field: 'Title', width: 120 },
    {
      headerName: '类型', field: 'Type', width: 120, cellEditor: 'agSelectCellEditor', cellEditorParams: { values: [1, 2] },
      valueFormatter: (params) => { return ['', '整型', '字符型'][params.value] }
    },
    {
      headerName: '主键', field: 'IsKey', width: 120, cellEditor: 'agSelectCellEditor', cellEditorParams: { values: [false, true] },
      valueFormatter: (params) => { return ['否', '是'][params.value] }
    },
    { headerName: '长度', field: 'Length', width: 120 },
  ]

  rowValueChanged(event) {
    //console.log('rowValueChanged', event)
    if (event.data._rowDataObjectStates == 'new') {
      event.data._rowDataObjectStates = 'add'
    } else if (event.data._rowDataObjectStates == 'add') {

    } else {
      event.data._rowDataObjectStates = 'modify'
    }
  }

  methodFromParent(key, node) {
    //console.log('methodFromParent', event, node)
    if (key == "delete") {
      this.gridApi.stopEditing();
      this.onDelete(node);
    }
  }

  rowEditingStopped(event) {
    //console.log('rowEditingStopped', event)
    if (event.data._rowDataObjectStates == 'new') {
      this.onDeleteAfter(event.data)
    }
  }
  rowDataChanged(event) {
    //console.log(event)
  }
  rowDataUpdated(event) {
    //console.log(event)
  }

  onNew() {
    //console.log(this.gridApi.getModel()	)
    let row = {
      Name: "FIELD_" + (this.gridApi.getDisplayedRowCount() + 1),
      Title: "字段 " + (this.gridApi.getDisplayedRowCount() + 1),
      Type: 2,
      Length: 20,
      _rowDataObjectStates: 'new'
    }
    this.saveNewData(row)
    this.gridApi.forEachNode((node, index) => {
      if (node.data == row) {
        this.gridApi.setFocusedCell(index, "Name")
        this.gridApi.startEditingCell({
          rowIndex: index,
          colKey: "Name"
        });
      }
    })
  }
  onSave() {
    let del = []
    let add = []
    let mod = []
    this.deleteList.forEach((data) => {
      if (data._rowDataObjectStates != 'add') {
        //console.log(data)
        del.push(data)
      }
    })
    this.gridApi.forEachNode((node, index) => {
      let data = node.data
      if (data._rowDataObjectStates == 'add') {
        add.push(data)
      } else if (data._rowDataObjectStates == 'modify') {
        mod.push(data)
      }
    })
    console.log([add, del, mod])
    this.save.emit([add, del, mod,()=>{
      this.deleteList=[]
      add.forEach(p=>{p._rowDataObjectStates=null})
      mod.forEach(p=>{p._rowDataObjectStates=null})
    }])
  }
}
