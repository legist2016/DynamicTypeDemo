import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-window-confirm',
  templateUrl: './window-confirm.component.html',
  styleUrls: ['./window-confirm.component.scss']
})
export class WindowConfirmComponent implements OnInit, OnDestroy {
  _alert
  _confirm
  ngOnDestroy(): void {
    window.alert = this._alert
    window.confirm = this._confirm
  }
  messages = []
  buttons
  action

  constructor() {
    this._alert = window.alert
    this._confirm = window.confirm
    window.alert = (msg) => this.alert(msg)
    window.confirm = (msg) => this.alert(msg)    
  }

  alert(msg) {
    //this.message = this.message && this.message!=msg && this.message+'\n'+msg || msg
    //console.log(msg)
    if (!msg) return false
    if (msg.constructor === String) {
      this.buttons = ["确定"]
    } else {
      if(msg.buttons.constructor === String){
        this.buttons = msg.buttons.split(',')
      }
      else {
        this.buttons = msg.buttons
      }
      this.action = msg.action
      msg = msg.msg
    }
    if (!this.messages.includes(msg))
      this.messages.push(msg)
    return false;
  }

  ngOnInit() {
  }

  close() {
    this.messages = []
  }
  message() {
    return this.messages.join('\n')
  }

  onClick(button){
    this.action && this.action(button)
    this.close()
  }

}
