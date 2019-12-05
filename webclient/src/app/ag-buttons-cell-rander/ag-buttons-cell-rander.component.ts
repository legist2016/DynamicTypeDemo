import { Component, OnInit } from '@angular/core';
import { ICellRendererAngularComp } from "@ag-grid-community/angular";


@Component({
  selector: 'app-ag-buttons-cell-rander',
  templateUrl: './ag-buttons-cell-rander.component.html',
  styleUrls: ['./ag-buttons-cell-rander.component.scss']
})
export class AgButtonsCellRanderComponent implements OnInit, ICellRendererAngularComp {
  public params: any;
  refresh(): boolean {
    return false;
  }
  agInit(params: any): void {
    //console.log(params)
    this.params = params
  }
  afterGuiAttached?(params?: any): void {

  }

  public invokeParentMethod(key) {
    //console.log(this.params)
    this.params.context.componentParent.methodFromParent(key, this.params.node)
  }

  constructor() { }

  ngOnInit() {
  }

}
