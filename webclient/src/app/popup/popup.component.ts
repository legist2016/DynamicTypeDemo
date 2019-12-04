import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.scss']
})
export class PopupComponent implements OnInit {
  @Input() closeRouterLink
  @Output() close = new EventEmitter()

  constructor() { }

  ngOnInit() {
  }

}
