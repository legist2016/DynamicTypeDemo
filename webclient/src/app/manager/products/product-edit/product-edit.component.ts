import { Component, OnInit, Output,EventEmitter,Input } from '@angular/core';
import { Product } from 'src/app/order';

@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss']
})
export class ProductEditComponent implements OnInit {
  @Output() save = new EventEmitter()
  @Input() product:Product
  constructor() { }

  ngOnInit() {
  }

  do(arg){
    this.save.emit(arg)
  }

}
