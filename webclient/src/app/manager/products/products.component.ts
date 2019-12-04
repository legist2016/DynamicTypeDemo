import { Component, OnInit } from '@angular/core';
import { ApplyDataService } from 'src/app/data.service';
import { Product } from 'src/app/order';
import { localeText } from 'src/app/aggrid.localtext';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  constructor(public ds: ApplyDataService) { }

  columnDefs = [
    { headerName: 'ID', field: 'id', checkboxSelection: true, width: 80 },
    { headerName: '状态', field: 'state', valueFormatter: (params) => { return ['删除', '停用', '启用'][params.value] }, width: 60 },
    { headerName: '名称', field: 'name' },
    { headerName: '价格', field: 'price', width: 60 },
    { headerName: '描述', field: 'description', width: 400, resizable: true },
  ]

  newProduct: Product = null
  editProduct: Product = null
  gridApi
  resolve
  reject
  localeText = localeText
  ngOnInit() {
    //this.ds.products = null
    this.ds.init()
    this.ds.LoadProductList()
  }

  gridReady(param) {
    this.gridApi = param.api
  }

  new() {
    this.newProduct = new Product()
  }

  onNewProductSave(product) {
    //console.log(product)
    if (product) {
      this.ds.postProduct(product, data => {
        this.newProduct = null
        this.gridApi.updateRowData({ add: [data] })
      })
    } else {
      this.newProduct = null
    }

  }

  onRowDbclick(event) {

    this.editProduct = Object.assign({}, event.data)
    this.resolve = product => {
      if (product) {
        this.ds.putProduct(product, data => {
          event.node.setData(product)
          this.editProduct = null
          this.resolve = null
          //console.log(this.gridApi.getModel())
        })
      } else {
        this.editProduct = null
      }
    }

  }
  onEditProductSave(product) {
    this.resolve && this.resolve(product)
  }

  del() {
    //if (window.confirm("是否删除选中的项目？")) 
    window.alert({
      msg: "是否删除选中的项目？",
      buttons: "是,否",
      action: (button) => {
        if (button == "是") {
          let rows = this.gridApi.getSelectedRows()
          let ids = []
          for (let row of rows) {
            ids.push(row.id)
          }
          this.ds.deleteProduct(ids, data => {
            //window.alert(data)
            this.gridApi.updateRowData({ remove: rows })
          })//.then()
        }
      }
    })
  }
}
