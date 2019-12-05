import { EventEmitter, Output, Input, OnInit } from "@angular/core"
import { DataService } from "src/app/data.service";
import { localeText } from 'src/app/aggrid.localtext';
import { AllCommunityModules } from '@ag-grid-community/all-modules';
import { AgButtonsCellRanderComponent } from './ag-buttons-cell-rander/ag-buttons-cell-rander.component';

export class Wizard {
    @Output() do: EventEmitter<any> = new EventEmitter()
    @Input() step: number
    @Input() ds: DataService
    constructor() { }
    Do(event) {
        this.do.emit(event)
    }

    get doemit() {
        return this.Do.bind(this)
    }
}

export class BaseEdit {
    @Output() do = new EventEmitter()
    @Input() data
    ngOnInit() {
    }
}


export class ManagerBase implements OnInit {
    //数据服务对象
    @Input() ds: DataService

    //通用数据处理属性
    newData
    editData
    addList = []
    editList = []
    deleteList = []
    resolve
    typename
    constructor() { }

    ngOnInit() {
    }

    //for ag-grid  start
    gridApi
    modules = AllCommunityModules;
    context = { componentParent: this }
    localeText = localeText
    columnDefs:any = [
        { headerName: 'ID', field: 'Id', checkboxSelection: true, width: 80 },
        { headerName: "操作", field: '', cellRenderer: "childMessageRenderer" },
        { headerName: '名称', field: 'Name' },
    ]
    frameworkComponents = {
        childMessageRenderer: AgButtonsCellRanderComponent
    };

    gridReady(param) {
        this.gridApi = param.api
    }
    onRowDbclick(event) {
        this.onEdit(event)
    }
    //for ag-grid  end


    //一般数据处理方法
    //Add 相关
    onNew(data) {
        this.newData = Object.assign({}, data);
    }

    saveNewData(data, submit?) {
        if (data) {
            if (submit) {
                submit((data) => {
                    this.saveNewDataAfter(data)
                })
            } else {
                this.saveNewDataAfter(data)
            }
        }
        else {
            this.newData = null;
        }
    }


    saveNewDataAfter(data) {
        this.newData = null;
        this.addList.push(data)
        this.gridApi.updateRowData({ add: [data] })
        console.log('数据本地保存。')
    }


    //Edit 相关
    onEdit(node) {
        console.log(node)
        this.editData = Object.assign({}, node.data)
        this.resolve = data => {
            if (data && this.saveEditData) {
                this.saveEditData(data, node)
            } else {
                this.editData = null
            }
        }
    }
    onEditSave(data) {
        this.resolve && this.resolve(data)
    }
    /**
     * 保存数据
     * @param data 对象数据
     * @param node grid行节点
     * @param submit 回调子类数据远程异步提交 submit(after), after ： 子类执行异步提交后回调 after() 处理本地状态
     * 
     */
    saveEditData(data, node, submit?) {
        console.log('super saveEditData')

        if (submit) {
            console.log('super saveEditData submit')
            submit(() => {
                this.saveEditDataAfter(data, node)
            })
        } else {
            this.saveEditDataAfter(data, node)
        }
    }
    saveEditDataAfter(data, node) {
        console.log('super saveEditData setData')
        if (!this.editList.includes(data)) {
            this.editList.push(data)
        }
        node.setData(data)
        this.editData = null
        this.resolve = null
        console.log('local data saved')

    }

    //Delete 相关
    onDelete(node?, submit?) {
        let data = null;
        if (node) {
            data = node.data
        } else {
            let rows = this.gridApi.getSelectedRows()
            if (rows.length == 0) return;
            data = rows[0]
        }
        window.alert({
            msg: "是否删除选中的项目？",
            buttons: "是,否",
            action: (button) => {
                if (button == "是") {
                    if (submit) {
                        submit(data, () => {
                            this.onDeleteAfter(data)
                        })
                    } else {
                        this.onDeleteAfter(data)
                    }
                }
            }
        });
    }
    onDeleteAfter(data) {
        this.deleteList.push(data)
        this.gridApi.updateRowData({ remove: [data] })
    }

}
