import { Component, OnInit } from '@angular/core';
//import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor(//private routerInfo: ActivatedRoute, private router:Router
    ) { }

  ngOnInit() {
    /*console.log(this.routerInfo.snapshot.queryParamMap.get('goto'));
    if(this.routerInfo.snapshot.queryParamMap.get('goto')){
      this.router.navigateByUrl(this.routerInfo.snapshot.queryParamMap.get('goto'))
    }*/
  }

}