import { Component, OnInit, EventEmitter,Output,Input } from '@angular/core';

@Component({
  selector: 'app-manager-menu',
  templateUrl: './manager-menu.component.html',
  styleUrls: ['./manager-menu.component.scss']
})
export class ManagerMenuComponent implements OnInit {
  @Output() menu = new EventEmitter();
  constructor() { }

  @Input() actived
  ngOnInit() {
  }

  do(something){
    //this.actived = something
    this.menu.emit(something)
  }

}
