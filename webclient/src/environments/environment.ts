// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
    publicBase:"/",
    config: {
      apiUrl:{
        Table:"http://localhost:4062/api/table",
        TableTemplate:"http://localhost:4062/api/template",
        TableTemplateField:"http://localhost:4062/api/template",
      },
      apiProductUrl: "http://192.168.202.213/cm/api/product",
      apiOrderUrl: "http://192.168.202.213/cm/api/order",
      apiStudentUrl: "http://192.168.202.213/cm/api/student",
    }
    };
  
  /*
   * For easier debugging in development mode, you can import the following file
   * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
   *
   * This import should be commented out in production mode because it will have a negative impact
   * on performance if an error is thrown.
   */
  // import 'zone.js/dist/zone-error';  // Included with Angular CLI.
  
  
  /*
  Copyright Google LLC. All Rights Reserved.
  Use of this source code is governed by an MIT-style license that
  can be found in the LICENSE file at http://angular.io/license
  */

