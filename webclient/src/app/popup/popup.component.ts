import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.scss']
})
export class PopupComponent implements OnInit {
  @Input() closeRouterLink
  @Input() Title
  @Output() close = new EventEmitter()

  constructor() { }

  ngOnInit() {
  }

}
