import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Order, Student, Product, OrderItem } from "./order";
import { environment } from 'src/environments/environment';



/**装饰器-捕获Promise对象异常*/
export function CatchErr(msg?) {
  return function (target, key, desc) {
    var oMethod = desc.value;
    desc.value = function (...args) {
      let r = oMethod.apply(this, args)
      let after = args.slice(-1)[0]
      //console.log(after)
      if (after && after.constructor === Function) {
        r = r.then(after)
      }
      return r.catch((err) => {
        //console.log(window.alert)
        if (msg && err && err.status && msg[err.status]) {

          window.alert(msg[err.status])
          //msg[err.status]
        } else {
          window.alert(err.error && err.error.Message || err.message || '发生错误')
        }
      })
    }

  }
}

/**装饰器-更新查询状态*/
export function Querying() {
  return function (target, key, desc) {
    var oMethod = desc.value;
    desc.value = function (...args) {
      this.querying = true
      return oMethod.apply(this, args)
        .finally(() => {
          this.querying = false
        })
    }
  }
}

/*export function catcherr(err) {
  //console.log('err:', err)
  window.alert(err.error && err.error.Message || err.message || '发生错误')
}*/



/**数据服务*/
export class DataService {
  constructor(public http: HttpClient) {
    /*console.log(environment)
    if (window.alert !== this.alert) {
      //console.log('hack window alert')
      window['_alert'] = window.alert
      window.alert['message'] = "ddddddddddddddd"
      window['ds'] = this
      //alert = this.alert
      window.alert = this.alert
    }*/
  }

  init() {
    this.querying = false;
    //this.message = null;
    this.order = null;
    this.items = null;
    this.flows = null
  }

  /*alert = function (msg) {
    let ds = window['ds']
    //console.log(ds)
    ds.message = (ds.message && `${ds.message}\n${msg}`) || msg
  }*/

  //message
  /**产品数组*/
  products: Array<Product> = null;

  /**查询状态*/
  querying: boolean = false;

  /**当前订单选取项目数组*/
  items: Array<OrderItem> = null;

  /**订单流程信息 */
  flows

  /**当前订单*/
  order: Order = null;

  newOrder(student?) {
    this.order = Object.assign(new Order(), student)
    this.items = this.initItems(new Array<OrderItem>())
    this.flows = null;
  }

  setOrder(order, items?, flows?) {
    this.order = Object.assign(new Order(), order)
    if (items) {
      this.items = this.initItems(items)
      this.flows = flows
      praseArray(OrderItem, items)
    }
  }



  @Querying() @CatchErr()
  findOrder(id, key, after?) {
    //this.http.get("assets/order.json").toPromise<any>()
    return this.http.get(`${environment.config.apiOrderUrl}/key/${key}/${id}`).toPromise<any>()
      .then(
        (data) => {
          //throw ""
          this.setOrder(data.order, data.items, data.flows)
          //console.log(data)
        });

  }


  initItems(items) {
    items.api = {
      add: function (product) {
        let item = this.find(item => item.productId == product.id)
        if (item) {
          item.count++;
          item.name = product.name
          item.price = product.price
        }
        else {
          this.push(new OrderItem(product.name, product.id, 1, product.price))
        }
      }.bind(items),
      delete: function (item) {
        //if (window.confirm(`是否删除项目：${item.name}（${item.count}份）？`)) 
        window.alert(
          {
            msg: `是否删除项目：${item.name}（${item.count}份）？`,
            buttons: [{
              text: "是",
              action: () => {
                let index = this.findIndex(i => i == item)
                //console.log(item, index)
                if (index >= 0) {
                  this.splice(index, 1)
                  if (item.id != 0) {
                    this.api.deleted.push(item)
                    //console.log(this.api.deleted)
                  }
                }
              }
            },
            { text: "否" }
            ]
          }
        )

      }.bind(items),
      deleted: []
    }
    return items
  }


  /**载入产品列表
   * @param state 产品状态筛选
   */
  @Querying() @CatchErr({'404':'载入数据时发生错误！'})
  LoadProductList(state?: number, after?) {
    //if (!this.products) {    
    let url = environment.config.apiProductUrl
    if (state != undefined) {
      url = url + "/state/" + state
    }
    //this.querying = true
    return this.http
      .get(url)
      .toPromise<any>()
      .then(data => {
        this.products = new Array<Product>()
        for (let item of data) {
          this.products.push(item)
        }
      })
    /*} else {
      return new Promise<any>((resolve, reject) => {
        resolve(this.products)
      })
    }*/
  }
  /**保存新产品项目 */
  @Querying() @CatchErr()
  postProduct(product, after?) {
    return this.http.post(environment.config.apiProductUrl, product).toPromise()
  }
  /**
   * 修改产品项目
   * @param product 
   */
  @Querying() @CatchErr()
  putProduct(product, after?) {
    return this.http.put(environment.config.apiProductUrl + '/' + product.id, product).toPromise()
  }
  /**
   * 删除一组产品
   * @param ids 
   */
  @Querying() @CatchErr()
  deleteProduct(ids, after?) {
    return this.http.delete(environment.config.apiProductUrl + "/delete/" + ids.join(',')).toPromise()
  }

  /**
   * 当前订单列表
   */
  orders: Array<Order> = null
  /**
   * 载入订单列表
   * @param state 
   */
  @Querying() @CatchErr({'404':'载入数据时发生错误！'})
  loadOrderList(states?: string, after?) {
    //if (this.orders == null) {
    let url = environment.config.apiOrderUrl
    if (states) {
      url += `/state/${states}`
    }
    return this.http.get(url).toPromise<any>()
      .then(data => {
        this.orders = this.initItems(new Array<Order>())
        praseArray(Order, data)
        for (let item of data) {
          this.orders.push(item)
        }
      })//.catch(this.catch)
    //.finally(this.finally)
    //}
    /*return new Promise((resolve, reject) => {
      resolve(this.orders)
    })*/
  }
  /**保存新订单 */
  @Querying() @CatchErr()
  postOrder(order, items, after?) {
    return this.http.post(environment.config.apiOrderUrl, { order: order, items: items }).toPromise()
  }
  /**修改订单 */
  @Querying() @CatchErr()
  putOrder(order, items, after?) {
    //console.log(items)
    return this.http.put(`${environment.config.apiOrderUrl}/${order.id}`, { order: order, items: items, deleted: items.api.deleted })
      .toPromise()//.then(after)
  }

  @Querying() @CatchErr()
  putOrderState(order, newState, after?) {
    return this.http.put(`${environment.config.apiOrderUrl}/state/${order.id}/${newState}`, null).toPromise()
  }

  /**删除一组订单 */
  /*@Querying() @CatchErr()
  deleteOrder(ids) {
    return this.http.delete(environment.config.apiOrderUrl + '/delete/' + ids.join(',')).toPromise()
  }*/


  getStudentInfo(xh, xm) {
    return this.http.get(`${environment.config.apiStudentUrl}/${xh}`).toPromise<any>();
  }

  test() {
    this.http.get('').toPromise()

  }
}



@Injectable({
  providedIn: "root"
})
export class ApplyDataService extends DataService {
  model: {
    student: Order;
    query: Student;
    step: number
  };


  constructor(public http: HttpClient) {
    super(http)

  }

  init() {
    super.init();
    this.model = {
      student: null,
      query: new Student(),
      step: 1

    };
  }

  @Querying()
  @CatchErr({ "404": "没有找到可用的学生信息，请直接填写信息", "123": "学号姓名不匹配，请填入正确的姓名，或直接填写信息。" })
  queryStudent(after?) {

    var xh = this.model.query.xh;
    var xm = this.model.query.xm;
    this.model.student = null;
    return this.getStudentInfo(xh, xm)
      .then(data => {
        data = {
          xh: data.XH,
          xm: data.XM,
          xb: data.XB,
          csrq: data.CSRQ,
          yx: data.YX, zy: data.ZY, rxsj: data.RXRQ, bysj: data.BYRQ
        };
        if (xm && xm != data.xm) {
          throw { status: 123 };
        }
        this.model.student = data
        return;
      })
  }

}

function praseArray<T>(type: (new (...args) => T), array, callback?: any) {
  for (let index in array) {
    let item = array[index]
    let newItem = new type()
    newItem = Object.assign(newItem, item)
    array[index] = newItem
    callback && callback(newItem)
  }
}



