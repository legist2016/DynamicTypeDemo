import { EventEmitter, Output, Input } from "@angular/core"
import { ApplyDataService } from "src/app/data.service";

export class Wizard {
    @Output() do: EventEmitter<any> = new EventEmitter()
    @Input() step: number
    @Input() ds: ApplyDataService
    constructor() { }
    get diagnostic() {
        return JSON.stringify(this.ds.model);
    }
    Do(event) {
        this.do.emit(event)
        //console.log(event)
    }

    get doemit(){
        return this.Do.bind(this)
    }
}