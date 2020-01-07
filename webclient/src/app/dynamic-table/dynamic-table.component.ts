import { Component, OnInit } from '@angular/core';
import { ManagerBase } from 'src/app/wizard';
import { DataService } from 'src/app/data.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-dynamic-table',
  templateUrl: './dynamic-table.component.html',
  styleUrls: ['./dynamic-table.component.scss']
})
export class DynamicTableComponent extends ManagerBase  implements OnInit{

  constructor(public ds:DataService,private route: ActivatedRoute) { 
    super()
  }

  typename = "Table"

  tableid

  ngOnInit() {
    if (this.typename) {
      
      this.route.paramMap.subscribe(params => {
        this.tableid = params.get('tableId');
      });      
      this.ds.data[this.typename + '_list'] = null
      this.ds.data[this.typename] = null
      this.ds.loadTableDef(this.typename,`${this.tableid}/define`,
      ()=>{
        let defs = []
        defs = this.ds.data[this.typename + '_define'].Fields 
        console.log(defs)
        this.columnDefs = defs.map((v)=>{
          return { headerName: v.Title, field: v.Name}
        });
        this.ds.loadData(this.typename,`${this.tableid}/entities`);
      });
    }
  }

}
