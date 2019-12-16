import { Component } from '@angular/core';
import { ManagerBase } from 'src/app/wizard';

@Component({
  selector: 'app-table-template',
  templateUrl: './table-template.component.html',
  styleUrls: ['./table-template.component.scss']
})
export class TableTemplateComponent extends ManagerBase {

  designData = null
  constructor() {
    super()
    this.context['buttons'] = [{ key: 'design', name: '设计' }, { key: 'edit', name: '编辑' }, { key: 'delete', name: '删除' }];
  }

  ngOnInit() {
    if (this.typename) {
      this.ds.data[this.typename + '_list'] = null
      this.ds.data[this.typename] = null
      this.ds.loadData(this.typename);
    }

  }

  typename = 'TableTemplate'

  methodFromParent(key, node) {
    if (key == "edit") this.onEdit(node);
    if (key == "delete") this.onDelete(node);
    if (key == "design") this.onDesign(node);
  }

  columnDefs = [
    { headerName: 'ID', field: 'Id', checkboxSelection: true, width: 80 },
    { headerName: "操作", field: '', cellRenderer: "childMessageRenderer" },
    { headerName: '名称', field: 'Name' },
  ]

  saveNewData(data) {
    super.saveNewData(data, (after) => {
      this.ds.postData("TableTemplate", data, (data) => {
        after(data)
      })

    })
  }


  saveEditData(data, node) {
    super.saveEditData(data, node, (after) => {
      this.ds.putData("TableTemplate", data.Id, data, () => {
        after && after()
      });
    });
  }
  onDelete(node?) {
    super.onDelete(node, (data, after) => {
      this.ds.deleteData("TableTemplate", data.Id, () => {
        after()
      })
    })
  }

  onDesign(node){
    this.designData = node.data 
    console.log(this.designData)
  }

  saveDesign(data){
    if(data){
      this.ds.putData("TableTemplateField", `${this.designData.Id}/fields`,data)
    } else{
      this.designData = null
    }    
  }
}
