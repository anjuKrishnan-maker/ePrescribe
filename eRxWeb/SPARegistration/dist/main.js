(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ "./$$_lazy_route_resource lazy recursive":
/*!******************************************************!*\
  !*** ./$$_lazy_route_resource lazy namespace object ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncaught exception popping up in devtools
	return Promise.resolve().then(function() {
		var e = new Error("Cannot find module '" + req + "'");
		e.code = 'MODULE_NOT_FOUND';
		throw e;
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "./$$_lazy_route_resource lazy recursive";

/***/ }),

/***/ "./src/app/Utils/String.Extension.ts":
/*!*******************************************!*\
  !*** ./src/app/Utils/String.Extension.ts ***!
  \*******************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
String.prototype.PasswordValidator = function () {
    var inputString = String(this);
    var options = 0;
    if (inputString) {
        if (/[a-z]/.test(inputString))
            options++;
        if (/[A-Z]/.test(inputString))
            options++;
        if (/[!@#\$%\^\&*\)\(+=._-]/.test(inputString))
            options++;
        if (/[0-9]/.test(inputString))
            options++;
        if (inputString.length < 8 || inputString.length > 25 || options < 3) {
            return false;
        }
        else {
            return true;
        }
    }
};


/***/ }),

/***/ "./src/app/app-routing.module.ts":
/*!***************************************!*\
  !*** ./src/app/app-routing.module.ts ***!
  \***************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.AppRoutingModule = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var AppRoutingModule = /** @class */ (function () {
    function AppRoutingModule() {
    }
    AppRoutingModule = tslib_1.__decorate([
        core_1.NgModule({
            imports: [router_1.RouterModule.forRoot([])],
            providers: [],
            exports: [router_1.RouterModule]
        })
    ], AppRoutingModule);
    return AppRoutingModule;
}());
exports.AppRoutingModule = AppRoutingModule;


/***/ }),

/***/ "./src/app/app.component.css":
/*!***********************************!*\
  !*** ./src/app/app.component.css ***!
  \***********************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".reg-header {\r\n    padding: 5px 0px;\r\n}\r\n\r\n.reg-footer {\r\n    clear: both;\r\n    min-height: 200px;\r\n    text-align: center;\r\n}\r\n\r\n.reg-content {\r\n    min-height: 100px;\r\n}\r\n\r\n.reg-divider {\r\n    height: 10px;\r\n    border: 0px;\r\n    margin-top: 0px;\r\n}\r\n\r\n.reg-container {\r\n    font-family: verdana, arial, sans-serif;\r\n    font-size: 13px;\r\n}\r\n\r\n.reg-contactus {\r\n    padding: 10px 0px 5px;\r\n    font-size: 13px;\r\n    font-weight: bold;\r\n    letter-spacing: 0.2px;\r\n}\r\n\r\n.reg-supportlink-anchor {\r\n    font-size: 11px;\r\n}\r\n\r\n.reg-supportlink {\r\n    color: #ffffff;\r\n}\r\n\r\n.reg-app-version {\r\n    display: none;\r\n}\r\n\r\na {\r\n    text-decoration: none;\r\n}\r\n\r\n.loader {\r\n    display: block;\r\n    z-index: 999999999;\r\n    background-color: #fefefe;\r\n    background-image: url('SPARegistration/dist/loading.gif');\r\n    -ms-opacity: 0.6;\r\n    opacity: 0.6;\r\n    -ms-background-repeat: no-repeat;\r\n    background-repeat: no-repeat;\r\n    -ms-background-position: center;\r\n    position: fixed;\r\n    background-position: center;\r\n    left: 0;\r\n    bottom: 0;\r\n    right: 0;\r\n    top: 0;\r\n}\r\n\r\n.reg-error-message {\r\n    color: #a94442;\r\n    background-color: #f2dede;\r\n    border-color: #ebccd1;\r\n    padding: .9em;\r\n    margin-bottom: 20px;\r\n    border: 1px solid transparent;\r\n    border-radius: 4px;\r\n    margin: 1em;\r\n}\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL2FwcC5jb21wb25lbnQuY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0lBQ0ksZ0JBQWdCO0FBQ3BCOztBQUVBO0lBQ0ksV0FBVztJQUNYLGlCQUFpQjtJQUNqQixrQkFBa0I7QUFDdEI7O0FBRUE7SUFDSSxpQkFBaUI7QUFDckI7O0FBRUE7SUFDSSxZQUFZO0lBQ1osV0FBVztJQUNYLGVBQWU7QUFDbkI7O0FBRUE7SUFDSSx1Q0FBdUM7SUFDdkMsZUFBZTtBQUNuQjs7QUFFQTtJQUNJLHFCQUFxQjtJQUNyQixlQUFlO0lBQ2YsaUJBQWlCO0lBQ2pCLHFCQUFxQjtBQUN6Qjs7QUFFQTtJQUNJLGVBQWU7QUFDbkI7O0FBRUE7SUFDSSxjQUFjO0FBQ2xCOztBQUVBO0lBQ0ksYUFBYTtBQUNqQjs7QUFFQTtJQUNJLHFCQUFxQjtBQUN6Qjs7QUFFQTtJQUNJLGNBQWM7SUFDZCxrQkFBa0I7SUFDbEIseUJBQXlCO0lBQ3pCLHlEQUFvRDtJQUNwRCxnQkFBZ0I7SUFDaEIsWUFBWTtJQUNaLGdDQUFnQztJQUNoQyw0QkFBNEI7SUFDNUIsK0JBQStCO0lBQy9CLGVBQWU7SUFDZiwyQkFBMkI7SUFDM0IsT0FBTztJQUNQLFNBQVM7SUFDVCxRQUFRO0lBQ1IsTUFBTTtBQUNWOztBQUVBO0lBQ0ksY0FBYztJQUNkLHlCQUF5QjtJQUN6QixxQkFBcUI7SUFDckIsYUFBYTtJQUNiLG1CQUFtQjtJQUNuQiw2QkFBNkI7SUFDN0Isa0JBQWtCO0lBQ2xCLFdBQVc7QUFDZiIsImZpbGUiOiJTUEFSZWdpc3RyYXRpb24vc3JjL2FwcC9hcHAuY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbIi5yZWctaGVhZGVyIHtcclxuICAgIHBhZGRpbmc6IDVweCAwcHg7XHJcbn1cclxuXHJcbi5yZWctZm9vdGVyIHtcclxuICAgIGNsZWFyOiBib3RoO1xyXG4gICAgbWluLWhlaWdodDogMjAwcHg7XHJcbiAgICB0ZXh0LWFsaWduOiBjZW50ZXI7XHJcbn1cclxuXHJcbi5yZWctY29udGVudCB7XHJcbiAgICBtaW4taGVpZ2h0OiAxMDBweDtcclxufVxyXG5cclxuLnJlZy1kaXZpZGVyIHtcclxuICAgIGhlaWdodDogMTBweDtcclxuICAgIGJvcmRlcjogMHB4O1xyXG4gICAgbWFyZ2luLXRvcDogMHB4O1xyXG59XHJcblxyXG4ucmVnLWNvbnRhaW5lciB7XHJcbiAgICBmb250LWZhbWlseTogdmVyZGFuYSwgYXJpYWwsIHNhbnMtc2VyaWY7XHJcbiAgICBmb250LXNpemU6IDEzcHg7XHJcbn1cclxuXHJcbi5yZWctY29udGFjdHVzIHtcclxuICAgIHBhZGRpbmc6IDEwcHggMHB4IDVweDtcclxuICAgIGZvbnQtc2l6ZTogMTNweDtcclxuICAgIGZvbnQtd2VpZ2h0OiBib2xkO1xyXG4gICAgbGV0dGVyLXNwYWNpbmc6IDAuMnB4O1xyXG59XHJcblxyXG4ucmVnLXN1cHBvcnRsaW5rLWFuY2hvciB7XHJcbiAgICBmb250LXNpemU6IDExcHg7XHJcbn1cclxuXHJcbi5yZWctc3VwcG9ydGxpbmsge1xyXG4gICAgY29sb3I6ICNmZmZmZmY7XHJcbn1cclxuXHJcbi5yZWctYXBwLXZlcnNpb24ge1xyXG4gICAgZGlzcGxheTogbm9uZTtcclxufVxyXG5cclxuYSB7XHJcbiAgICB0ZXh0LWRlY29yYXRpb246IG5vbmU7XHJcbn1cclxuXHJcbi5sb2FkZXIge1xyXG4gICAgZGlzcGxheTogYmxvY2s7XHJcbiAgICB6LWluZGV4OiA5OTk5OTk5OTk7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmVmZWZlO1xyXG4gICAgYmFja2dyb3VuZC1pbWFnZTogdXJsKCcuLi8uLi8uLi9pbWFnZXMvbG9hZGluZy5naWYnKTtcclxuICAgIC1tcy1vcGFjaXR5OiAwLjY7XHJcbiAgICBvcGFjaXR5OiAwLjY7XHJcbiAgICAtbXMtYmFja2dyb3VuZC1yZXBlYXQ6IG5vLXJlcGVhdDtcclxuICAgIGJhY2tncm91bmQtcmVwZWF0OiBuby1yZXBlYXQ7XHJcbiAgICAtbXMtYmFja2dyb3VuZC1wb3NpdGlvbjogY2VudGVyO1xyXG4gICAgcG9zaXRpb246IGZpeGVkO1xyXG4gICAgYmFja2dyb3VuZC1wb3NpdGlvbjogY2VudGVyO1xyXG4gICAgbGVmdDogMDtcclxuICAgIGJvdHRvbTogMDtcclxuICAgIHJpZ2h0OiAwO1xyXG4gICAgdG9wOiAwO1xyXG59XHJcblxyXG4ucmVnLWVycm9yLW1lc3NhZ2Uge1xyXG4gICAgY29sb3I6ICNhOTQ0NDI7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZjJkZWRlO1xyXG4gICAgYm9yZGVyLWNvbG9yOiAjZWJjY2QxO1xyXG4gICAgcGFkZGluZzogLjllbTtcclxuICAgIG1hcmdpbi1ib3R0b206IDIwcHg7XHJcbiAgICBib3JkZXI6IDFweCBzb2xpZCB0cmFuc3BhcmVudDtcclxuICAgIGJvcmRlci1yYWRpdXM6IDRweDtcclxuICAgIG1hcmdpbjogMWVtO1xyXG59XHJcbiJdfQ== */"

/***/ }),

/***/ "./src/app/app.component.html":
/*!************************************!*\
  !*** ./src/app/app.component.html ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"reg-container\">\r\n    <div class=\"reg-header\">\r\n        <div class=\"branded-logo\"></div>\r\n    </div>\r\n    <ng-container *ngIf=\"{ isPageLoadInProgress: this.loadingNavigation | async, isApiInProgress: this.loaderService.isLoading | async} as spinner;\">\r\n        <div id=\"divLoading\" *ngIf=\"spinner.isApiInProgress || spinner.isPageLoadInProgress\" class=\"loader\">\r\n        </div>\r\n    </ng-container>\r\n    <div class=\"reg-content\">\r\n        <router-outlet></router-outlet>\r\n    </div>\r\n    <div class=\"reg-footer branded-footer-color\">\r\n        <hr class=\"reg-divider branded-footer-stripe\" />\r\n        <div class=\"reg-contactus\">ePrescribe Customer Support</div>\r\n        <a class=\"reg-supportlink-anchor\" [href]=\"supportMailAddressHref\">\r\n            <span class=\"reg-supportlink\">{{supportMailAddress}}</span>\r\n        </a>\r\n        <span class=\"reg-app-version\">{{version}}</span>\r\n    </div>\r\n</div>\r\n\r\n\r\n\r\n"

/***/ }),

/***/ "./src/app/app.component.ts":
/*!**********************************!*\
  !*** ./src/app/app.component.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.AppComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var loader_service_1 = __webpack_require__(/*! ./service/loader.service */ "./src/app/service/loader.service.ts");
var rxjs_1 = __webpack_require__(/*! rxjs */ "../node_modules/rxjs/_esm5/index.js");
var message_service_1 = __webpack_require__(/*! ./service/message.service */ "./src/app/service/message.service.ts");
var AppComponent = /** @class */ (function () {
    function AppComponent(window, router, loaderService, messageService) {
        var _a, _b, _c, _d;
        this.window = window;
        this.router = router;
        this.loaderService = loaderService;
        this.messageService = messageService;
        this.loadingNavigation = new rxjs_1.Subject();
        this.version = (_b = (_a = this.window) === null || _a === void 0 ? void 0 : _a.appcontext) === null || _b === void 0 ? void 0 : _b.version;
        this.supportMailAddress = (_d = (_c = this.window) === null || _c === void 0 ? void 0 : _c.appcontext) === null || _d === void 0 ? void 0 : _d.supportMailAddress;
        this.supportMailAddressHref = "mailto:" + this.supportMailAddress + "?Subject=ePrescribe%20Registration";
    }
    AppComponent.prototype.ngAfterViewInit = function () {
        var _this = this;
        this.router.events
            .subscribe(function (event) {
            if (event instanceof router_1.NavigationStart) {
                _this.loadingNavigation.next(true);
            }
            else if (event instanceof router_1.NavigationEnd ||
                event instanceof router_1.NavigationCancel ||
                event instanceof router_1.NavigationError) {
                _this.loadingNavigation.next(false);
            }
        });
    };
    AppComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-root',
            template: __webpack_require__(/*! ./app.component.html */ "./src/app/app.component.html"),
            styles: [__webpack_require__(/*! ./app.component.css */ "./src/app/app.component.css")]
        }),
        tslib_1.__param(0, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [Object, router_1.Router,
            loader_service_1.LoaderService,
            message_service_1.MessageService])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;


/***/ }),

/***/ "./src/app/app.module.ts":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.AppModule = exports.getWindow = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var platform_browser_1 = __webpack_require__(/*! @angular/platform-browser */ "../node_modules/@angular/platform-browser/fesm5/platform-browser.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var app_routing_module_1 = __webpack_require__(/*! ./app-routing.module */ "./src/app/app-routing.module.ts");
var app_component_1 = __webpack_require__(/*! ./app.component */ "./src/app/app.component.ts");
var registration_module_1 = __webpack_require__(/*! ./registration/registration.module */ "./src/app/registration/registration.module.ts");
var data_service_1 = __webpack_require__(/*! ./service/data.service */ "./src/app/service/data.service.ts");
var loader_service_1 = __webpack_require__(/*! ./service/loader.service */ "./src/app/service/loader.service.ts");
var http_1 = __webpack_require__(/*! @angular/common/http */ "../node_modules/@angular/common/fesm5/http.js");
var loader_interceptor_1 = __webpack_require__(/*! ./loader.interceptor */ "./src/app/loader.interceptor.ts");
var global_errorhandler_1 = __webpack_require__(/*! ./global.errorhandler */ "./src/app/global.errorhandler.ts");
var message_service_1 = __webpack_require__(/*! ./service/message.service */ "./src/app/service/message.service.ts");
function getWindow() {
    return (typeof window !== "undefined") ? window : null;
}
exports.getWindow = getWindow;
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = tslib_1.__decorate([
        core_1.NgModule({
            declarations: [
                app_component_1.AppComponent
            ],
            imports: [
                platform_browser_1.BrowserModule,
                app_routing_module_1.AppRoutingModule,
                registration_module_1.RegistrationModule,
                http_1.HttpClientModule
            ],
            providers: [loader_service_1.LoaderService, data_service_1.DataService, message_service_1.MessageService,
                { provide: 'window', useFactory: getWindow },
                { provide: http_1.HTTP_INTERCEPTORS, useClass: loader_interceptor_1.LoaderInterceptor, multi: true },
                { provide: core_1.ErrorHandler, useClass: global_errorhandler_1.GlobalErrorHandler }],
            bootstrap: [app_component_1.AppComponent]
        })
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;


/***/ }),

/***/ "./src/app/global.errorhandler.ts":
/*!****************************************!*\
  !*** ./src/app/global.errorhandler.ts ***!
  \****************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.GlobalErrorHandler = void 0;
var GlobalErrorHandler = /** @class */ (function () {
    function GlobalErrorHandler() {
    }
    GlobalErrorHandler.prototype.handleError = function (error) {
        console.error(error);
        //TODO: Send to app-insight framework
    };
    return GlobalErrorHandler;
}());
exports.GlobalErrorHandler = GlobalErrorHandler;


/***/ }),

/***/ "./src/app/loader.interceptor.ts":
/*!***************************************!*\
  !*** ./src/app/loader.interceptor.ts ***!
  \***************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.LoaderInterceptor = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var operators_1 = __webpack_require__(/*! rxjs/operators */ "../node_modules/rxjs/_esm5/operators/index.js");
var loader_service_1 = __webpack_require__(/*! ./service/loader.service */ "./src/app/service/loader.service.ts");
var LoaderInterceptor = /** @class */ (function () {
    function LoaderInterceptor(loaderService) {
        this.loaderService = loaderService;
        this.apiCallsInProgress = [];
    }
    LoaderInterceptor.prototype.intercept = function (req, next) {
        var _this = this;
        if (req.url && req.url.length > 0)
            this.apiCallsInProgress.push(req.url.toLowerCase());
        if (this.apiCallsInProgress.length > 0)
            this.loaderService.show();
        return next.handle(req)
            .pipe(operators_1.finalize(function () {
            _this.apiCallsInProgress = _this.apiCallsInProgress.filter(function (api) { return api !== req.url.toLowerCase(); });
            if (_this.apiCallsInProgress.length <= 0 && !_this.loaderService.overrideLoading)
                _this.loaderService.hide();
        }));
    };
    LoaderInterceptor = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__metadata("design:paramtypes", [loader_service_1.LoaderService])
    ], LoaderInterceptor);
    return LoaderInterceptor;
}());
exports.LoaderInterceptor = LoaderInterceptor;


/***/ }),

/***/ "./src/app/registration/account-creation/account-creation.component.css":
/*!******************************************************************************!*\
  !*** ./src/app/registration/account-creation/account-creation.component.css ***!
  \******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".account-creation-form-label-spacing {\r\n    width: 155px;\r\n}\r\n\r\n.account-creation-user-name-control-spacing {\r\n    padding-left: 59px;\r\n}\r\n\r\n.account-creation-user-password-control-spacing {\r\n    padding-left: 0px;\r\n}\r\n\r\n.account-creation-security-control-spacing {\r\n    padding-left: 154px;\r\n}\r\n\r\n.account-creation-captcha-control-spacing {\r\n    padding-left: 85px;\r\n}\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY2NvdW50LWNyZWF0aW9uL2FjY291bnQtY3JlYXRpb24uY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLFlBQVk7QUFDaEI7O0FBRUE7SUFDSSxrQkFBa0I7QUFDdEI7O0FBRUE7SUFDSSxpQkFBaUI7QUFDckI7O0FBRUE7SUFDSSxtQkFBbUI7QUFDdkI7O0FBRUE7SUFDSSxrQkFBa0I7QUFDdEIiLCJmaWxlIjoiU1BBUmVnaXN0cmF0aW9uL3NyYy9hcHAvcmVnaXN0cmF0aW9uL2FjY291bnQtY3JlYXRpb24vYWNjb3VudC1jcmVhdGlvbi5jb21wb25lbnQuY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLmFjY291bnQtY3JlYXRpb24tZm9ybS1sYWJlbC1zcGFjaW5nIHtcclxuICAgIHdpZHRoOiAxNTVweDtcclxufVxyXG5cclxuLmFjY291bnQtY3JlYXRpb24tdXNlci1uYW1lLWNvbnRyb2wtc3BhY2luZyB7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDU5cHg7XHJcbn1cclxuXHJcbi5hY2NvdW50LWNyZWF0aW9uLXVzZXItcGFzc3dvcmQtY29udHJvbC1zcGFjaW5nIHtcclxuICAgIHBhZGRpbmctbGVmdDogMHB4O1xyXG59XHJcblxyXG4uYWNjb3VudC1jcmVhdGlvbi1zZWN1cml0eS1jb250cm9sLXNwYWNpbmcge1xyXG4gICAgcGFkZGluZy1sZWZ0OiAxNTRweDtcclxufVxyXG5cclxuLmFjY291bnQtY3JlYXRpb24tY2FwdGNoYS1jb250cm9sLXNwYWNpbmcge1xyXG4gICAgcGFkZGluZy1sZWZ0OiA4NXB4O1xyXG59Il19 */"

/***/ }),

/***/ "./src/app/registration/account-creation/account-creation.component.html":
/*!*******************************************************************************!*\
  !*** ./src/app/registration/account-creation/account-creation.component.html ***!
  \*******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #userAccountForm=\"ngForm\" (ngSubmit)=\"onSubmit(userAccountForm)\">\r\n    <div id=\"user-creation-title\" class=\"title-bar title-bar-heading branded-background-color branded-font-color\">\r\n        Create New Account\r\n    </div>\r\n    <div>\r\n        <span *ngIf=\"isValidationFailed\" class=\"validation-color\">\r\n            {{errorMessage}}\r\n        </span>\r\n        \r\n    </div>\r\n    <div class=\"erx-form-content-offset font-color-dimmed-black\">\r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            Personal Information\r\n        </div>\r\n        <div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label account-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>First Name</label>\r\n                    </span>\r\n\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"firstname\" name=\"firstname\" class=\"input-large\" type=\"text\"\r\n                               required maxlength=\"35\" [pattern]=\"namePattern\"\r\n                               [(ngModel)]=\"userAccount.firstname\" #firstname=\"ngModel\">\r\n                        <span *ngIf=\"firstname.invalid && (firstname.dirty || firstname.touched)\">\r\n                            <span id=\"firstNameRequiredError\" *ngIf=\"firstname.errors.required\" class=\"validation-color\">\r\n                                First Name is required.\r\n                            </span>\r\n                            <span id=\"firstnamePatternError\" *ngIf=\"firstname.errors.pattern\" class=\"validation-color\">\r\n                                Invalid first name. First name has to be characters, maximum of 35.\r\n                            </span>\r\n                        </span>\r\n\r\n                    </span>\r\n                </div>\r\n\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label account-creation-form-label-spacing\">\r\n                        <label>Middle Name</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"middlename\" name=\"middleName\" class=\"input-large\"\r\n                               maxlength=\"35\" [pattern]=\"namePattern\"\r\n                               [(ngModel)]=\"userAccount.middleName\" #middleName=\"ngModel\" />\r\n                    </span>\r\n                    <span *ngIf=\"middleName.invalid && (middleName.dirty || middleName.touched)\">\r\n                        <span id=\"middleNamePatternError\" *ngIf=\"middleName.errors.pattern\" class=\"validation-color\">\r\n                            Invalid middle name. Middle name has to be characters, maximum of 35.\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label account-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>Last Name</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input type=\"text\" id=\"lastName\" name=\"lastName\" class=\"input-large\"\r\n                               [pattern]=\"namePattern\"\r\n                               required maxlength=\"35\" [(ngModel)]=\"userAccount.lastName\" #lastName=\"ngModel\" />\r\n                        <span *ngIf=\"lastName.invalid && (lastName.dirty || lastName.touched)\">\r\n                            <span id=\"lastNameRequiredError\" *ngIf=\"lastName.errors.required\" class=\"validation-color\">\r\n                                Last Name is required.\r\n                            </span>\r\n                            <span id=\"firstnamePatternError\" *ngIf=\"lastName.errors.pattern\" class=\"validation-color\">\r\n                                Invalid first name. Last name has to be characters, maximum of 35.\r\n                            </span>\r\n                        </span>\r\n\r\n                    </span>\r\n                </div>\r\n\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label account-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>Personal Email</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input type=\"email\" id=\"personalEmail\" name=\"personalEmail\" class=\"input-large\"\r\n                               required maxlength=\"70\"\r\n                               [(ngModel)]=\"userAccount.personalEmail\" #personalEmail=\"ngModel\" email />\r\n                        <span *ngIf=\"personalEmail.invalid && (personalEmail.dirty || personalEmail.touched)\">\r\n                            <span id=\"emailRequiredError\" *ngIf=\"personalEmail.errors.required\" class=\"validation-color\">\r\n                                Personal Email is required.\r\n                            </span>\r\n                            <span id=\"emailPatternError\" *ngIf=\"personalEmail.errors.email && !personalEmail.errors.required \" class=\"validation-color\">\r\n                                Enter valid email address.\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n                <!--</div>-->\r\n            </div>\r\n\r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            USER CREDENTIALS & SECURITY\r\n        </div>\r\n        <div>\r\n            <div class=\"account-creation-user-name-control-spacing\">\r\n                <app-user-account #userNameChild [userExistsErrorMessage]=\"userExistsErrorMessage\" [userNameLabel]=\"userNameLabel\"></app-user-account>\r\n            </div>\r\n            <div class=\"account-creation-user-password-control-spacing\">\r\n                <app-user-password #pwdChild></app-user-password>\r\n            </div>\r\n            <div class=\"account-creation-security-control-spacing\">\r\n                <app-user-security-questions #securityQuestion></app-user-security-questions>\r\n            </div>\r\n        </div>\r\n     \r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            ACKNOWLEDGEMENT\r\n        </div>\r\n        <div>\r\n            <div class=\"erx-form-row account-creation-captcha-control-spacing\">\r\n                <show-captcha #captchaControl></show-captcha>\r\n            </div>\r\n\r\n            <div class=\"erx-form-row\">\r\n                <input type=\"checkbox\" [(ngModel)]=\"userAccount.isTermsAccepted\" id=\"chkAcceptedIDmeTerms\"\r\n                       name=\"isTermsAccepted\" #isTermsAccepted=\"ngModel\" required> I have reviewed my registration entries\r\n\r\n                <span *ngIf=\"isTermsAccepted.invalid && (isTermsAccepted.dirty || isTermsAccepted.touched)\" class=\"validation-color\">\r\n                    Kindly acknowledge the registration entries.\r\n                </span>\r\n            </div>\r\n        </div>\r\n        <div class=\"erx-form-row\">\r\n            <button id=\"btnSubmit\" type=\"submit\" class=\"button-style\">Submit</button>\r\n            <span *ngIf=\"isValidationFailed\" class=\"validation-color\">\r\n                Kindly review the form, there seems to be an incomplete entry.\r\n            </span>\r\n        </div>\r\n    </div>\r\n</form>\r\n\r\n\r\n"

/***/ }),

/***/ "./src/app/registration/account-creation/account-creation.component.ts":
/*!*****************************************************************************!*\
  !*** ./src/app/registration/account-creation/account-creation.component.ts ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.AccountCreationComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var account_creation_model_1 = __webpack_require__(/*! ../account-creation/account-creation.model */ "./src/app/registration/account-creation/account-creation.model.ts");
var user_create_service_1 = __webpack_require__(/*! ../../service/user-create.service */ "./src/app/service/user-create.service.ts");
__webpack_require__(/*! ../../Utils/String.Extension */ "./src/app/Utils/String.Extension.ts");
var show_captcha_component_1 = __webpack_require__(/*! ../../registration/show-captcha/show-captcha.component */ "./src/app/registration/show-captcha/show-captcha.component.ts");
var user_password_component_1 = __webpack_require__(/*! ../user-password/user-password.component */ "./src/app/registration/user-password/user-password.component.ts");
var user_account_name_component_1 = __webpack_require__(/*! ../user-account-name/user-account-name.component */ "./src/app/registration/user-account-name/user-account-name.component.ts");
var user_security_questions_component_1 = __webpack_require__(/*! ../user-security-questions/user-security-questions.component */ "./src/app/registration/user-security-questions/user-security-questions.component.ts");
var loader_service_1 = __webpack_require__(/*! ../../service/loader.service */ "./src/app/service/loader.service.ts");
var AccountCreationComponent = /** @class */ (function () {
    function AccountCreationComponent(userCreateService, loaderService, window) {
        this.userCreateService = userCreateService;
        this.loaderService = loaderService;
        this.window = window;
        this.isValidationFailed = false;
        this.userAccount = new account_creation_model_1.AccountDetails();
        this.isPasswordValid = false;
        this.isSecurityQuestionAnswerValid = false;
        this.isUserNameValid = true;
        this.isValidCaptcha = false;
        this.captcha = "";
        this.namePattern = "^([a-zA-Z]+[\\s-'.]{0,35})*";
    }
    AccountCreationComponent.prototype.ngOnInit = function () {
        this.userExistsErrorMessage = "Username is not available";
        this.userNameLabel = "Username";
    };
    AccountCreationComponent.prototype.validateShieldUserAccount = function () {
        this.getUserAccountNameDetails();
        this.getPasswordDetails();
        this.getSecurityQuestionAnswerDetails();
        return (this.isPasswordValid && this.isUserNameValid && this.isSecurityQuestionAnswerValid);
    };
    AccountCreationComponent.prototype.isUserAccountFormValid = function (data) {
        this.markFormControlsTouched(data);
        this.getCaptchaDetails();
        var isShieldUserAccountValid = this.validateShieldUserAccount();
        return data.valid
            && isShieldUserAccountValid
            && this.isValidCaptcha;
    };
    AccountCreationComponent.prototype.markFormControlsTouched = function (userAccountForm) {
        Object.keys(userAccountForm.controls).forEach(function (field) {
            var control = userAccountForm.controls[field];
            control.markAsTouched({ onlySelf: true });
        });
    };
    AccountCreationComponent.prototype.onSubmit = function (userAccountForm) {
        var _this = this;
        this.isValidationFailed = false;
        if (this.isUserAccountFormValid(userAccountForm)) {
            this.userAccount.secretQuestionsField = this.generateScretAnswer();
            this.userCreateService
                .saveUserData(this.userAccount).subscribe(function (validateCreateUser) {
                if (validateCreateUser.IsDataSaved) {
                    _this.loaderService.show(true);
                    _this.window.open(validateCreateUser.RedirectUrl, "_self");
                }
                else {
                    _this.isValidationFailed = true;
                    if (!validateCreateUser.IsValidCaptcha) {
                        _this.captchaControl.ShowInValidCaptchaError = true;
                    }
                    else {
                        _this.errorMessage = validateCreateUser.ErrorMessage;
                    }
                }
            });
        }
    };
    AccountCreationComponent.prototype.generateScretAnswer = function () {
        var secretAnsweList = [];
        var secretAnswer = new account_creation_model_1.SecretAnswers();
        var secretAnswer2 = new account_creation_model_1.SecretAnswers();
        var secretAnswer3 = new account_creation_model_1.SecretAnswers();
        secretAnswer.questionIDField = Number(this.userAccount.securityQuestion1);
        secretAnswer.answerField = this.userAccount.securityAnswer1;
        secretAnsweList.push(secretAnswer);
        secretAnswer2.questionIDField = Number(this.userAccount.securityQuestion2);
        secretAnswer2.answerField = this.userAccount.securityAnswer2;
        secretAnsweList.push(secretAnswer2);
        secretAnswer3.questionIDField = Number(this.userAccount.securityQuestion3);
        secretAnswer3.answerField = this.userAccount.securityAnswer3;
        secretAnsweList.push(secretAnswer3);
        return secretAnsweList;
    };
    AccountCreationComponent.prototype.getCaptchaDetails = function () {
        var captchaModel;
        captchaModel = this.captchaControl.GetCaptchaDetails();
        this.userAccount.CapchaText = captchaModel.captchaText;
        this.isValidCaptcha = captchaModel.isValid;
    };
    AccountCreationComponent.prototype.getUserAccountNameDetails = function () {
        var userAccountName;
        userAccountName = this.userNameChild.GetUserAccountNameDetails();
        this.userAccount.userName = userAccountName.shieldUserName;
        this.isUserNameValid = userAccountName.isValid;
    };
    AccountCreationComponent.prototype.getPasswordDetails = function () {
        var userPassword;
        userPassword = this.pwdChild.GetPasswordDetails();
        this.userAccount.password = userPassword.password;
        this.userAccount.confirmPassword = userPassword.confirmPassword;
        this.isPasswordValid = userPassword.isValid;
    };
    AccountCreationComponent.prototype.getSecurityQuestionAnswerDetails = function () {
        var securityQuestionAnswerModel;
        securityQuestionAnswerModel = this.securityQuestion.GetSecurityQuestionAnswerDetails();
        this.userAccount.securityQuestion1 = securityQuestionAnswerModel.selectedShieldQuestionOne;
        this.userAccount.securityQuestion2 = securityQuestionAnswerModel.selectedShieldQuestionTwo;
        this.userAccount.securityQuestion3 = securityQuestionAnswerModel.selectedShieldQuestionThree;
        this.userAccount.securityAnswer1 = securityQuestionAnswerModel.securityAnswerOne;
        this.userAccount.securityAnswer2 = securityQuestionAnswerModel.securityAnswerTwo;
        this.userAccount.securityAnswer3 = securityQuestionAnswerModel.securityAnswerThree;
        this.isSecurityQuestionAnswerValid = securityQuestionAnswerModel.isValid;
    };
    tslib_1.__decorate([
        core_1.ViewChild(show_captcha_component_1.ShowCaptchaComponent),
        tslib_1.__metadata("design:type", show_captcha_component_1.ShowCaptchaComponent)
    ], AccountCreationComponent.prototype, "captchaComponent", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('pwdChild'),
        tslib_1.__metadata("design:type", user_password_component_1.UserPasswordComponent)
    ], AccountCreationComponent.prototype, "pwdChild", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('userNameChild'),
        tslib_1.__metadata("design:type", user_account_name_component_1.UserAccountNameComponent)
    ], AccountCreationComponent.prototype, "userNameChild", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('securityQuestion'),
        tslib_1.__metadata("design:type", user_security_questions_component_1.UserSecurityQuestionsComponent)
    ], AccountCreationComponent.prototype, "securityQuestion", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('captchaControl'),
        tslib_1.__metadata("design:type", show_captcha_component_1.ShowCaptchaComponent)
    ], AccountCreationComponent.prototype, "captchaControl", void 0);
    AccountCreationComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-create-account',
            template: __webpack_require__(/*! ./account-creation.component.html */ "./src/app/registration/account-creation/account-creation.component.html"),
            styles: [__webpack_require__(/*! ./account-creation.component.css */ "./src/app/registration/account-creation/account-creation.component.css")]
        }),
        tslib_1.__param(2, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [user_create_service_1.UserCreateService,
            loader_service_1.LoaderService, Object])
    ], AccountCreationComponent);
    return AccountCreationComponent;
}());
exports.AccountCreationComponent = AccountCreationComponent;


/***/ }),

/***/ "./src/app/registration/account-creation/account-creation.model.ts":
/*!*************************************************************************!*\
  !*** ./src/app/registration/account-creation/account-creation.model.ts ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ValidateCreateUser = exports.SecretAnswers = exports.shieldSecretQuestion = exports.SecretQuestionsAns = exports.AccountDetails = void 0;
var AccountDetails = /** @class */ (function () {
    function AccountDetails() {
    }
    return AccountDetails;
}());
exports.AccountDetails = AccountDetails;
var SecretQuestionsAns = /** @class */ (function () {
    function SecretQuestionsAns() {
    }
    return SecretQuestionsAns;
}());
exports.SecretQuestionsAns = SecretQuestionsAns;
var shieldSecretQuestion = /** @class */ (function () {
    function shieldSecretQuestion() {
    }
    return shieldSecretQuestion;
}());
exports.shieldSecretQuestion = shieldSecretQuestion;
var SecretAnswers = /** @class */ (function () {
    function SecretAnswers() {
    }
    return SecretAnswers;
}());
exports.SecretAnswers = SecretAnswers;
var ValidateCreateUser = /** @class */ (function () {
    function ValidateCreateUser() {
    }
    return ValidateCreateUser;
}());
exports.ValidateCreateUser = ValidateCreateUser;


/***/ }),

/***/ "./src/app/registration/activate-user/activation-code.model.ts":
/*!*********************************************************************!*\
  !*** ./src/app/registration/activate-user/activation-code.model.ts ***!
  \*********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ValidateActivationCodeModel = exports.ActivationCodeModel = void 0;
var ActivationCodeModel = /** @class */ (function () {
    function ActivationCodeModel() {
    }
    return ActivationCodeModel;
}());
exports.ActivationCodeModel = ActivationCodeModel;
var ValidateActivationCodeModel = /** @class */ (function () {
    function ValidateActivationCodeModel() {
    }
    return ValidateActivationCodeModel;
}());
exports.ValidateActivationCodeModel = ValidateActivationCodeModel;


/***/ }),

/***/ "./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component.ts":
/*!*************************************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component.ts ***!
  \*************************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.LinkAccountConfirmationPopupComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var link_account_request_model_1 = __webpack_require__(/*! ../link-account-request-model */ "./src/app/registration/activate-user/link-account-request-model.ts");
var link_account_request_model_2 = __webpack_require__(/*! ../link-account-request-model */ "./src/app/registration/activate-user/link-account-request-model.ts");
var loader_service_1 = __webpack_require__(/*! ../../../service/loader.service */ "./src/app/service/loader.service.ts");
var LinkAccountConfirmationPopupComponent = /** @class */ (function () {
    function LinkAccountConfirmationPopupComponent(router, loaderService, window) {
        this.router = router;
        this.loaderService = loaderService;
        this.window = window;
        this.linkExistingUserResponse = new link_account_request_model_1.LinkExistingUserModel();
        this.showLinkedAccountConfPopup = false;
    }
    LinkAccountConfirmationPopupComponent.prototype.showLinkedAccountConfirmation = function () {
        if (this.linkExistingUserResponse.IsValid) {
            if (this.workFlowType == link_account_request_model_2.Workflow.Utilities) {
                this.showMessage = true;
                this.linkConfMessageText = "User successfully linked. You will now be directed to Activation home page.";
                this.setImageIcon("SUCCESS");
            }
            this.showLinkedAccountConfPopup = true;
            this.showLinkedAccountConfLayout = true;
        }
        else {
            this.displayErrorMessage(this.linkExistingUserResponse);
        }
    };
    LinkAccountConfirmationPopupComponent.prototype.goNext = function () {
        var _a, _b, _c, _d, _e, _f;
        this.loaderService.show(true);
        if (this.workFlowType == link_account_request_model_2.Workflow.Utilities) {
            this.window.open(((_b = (_a = this.window) === null || _a === void 0 ? void 0 : _a.appcontext) === null || _b === void 0 ? void 0 : _b.logout) + "?msg=Utlities user successfully activated", "_self");
        }
        else if (this.workFlowType == link_account_request_model_2.Workflow.Sso) {
            this.window.open(((_d = (_c = this.window) === null || _c === void 0 ? void 0 : _c.appcontext) === null || _d === void 0 ? void 0 : _d.logout) + "?SSOActivationSuccess=true", "_self");
        }
        else {
            this.window.open((_f = (_e = this.window) === null || _e === void 0 ? void 0 : _e.appcontext) === null || _f === void 0 ? void 0 : _f.twoNUserMediator, "_self");
        }
    };
    LinkAccountConfirmationPopupComponent.prototype.setImageIcon = function (value) {
        switch (value.toUpperCase()) {
            case "SUCCESS":
                this.linkConfImageUrl = "../images/info-global-16-x-16.png";
                this.imageIcon = "success-dialog";
                break;
            case "INFORMATION":
                this.linkConfImageUrl = "../images/warning-global-16-x-16.png";
                this.imageIcon = "information-dialog";
                break;
            default:
                this.linkConfImageUrl = "../images/info-global-16-x-16.png";
                this.imageIcon = "success-dialog";
                break;
        }
    };
    LinkAccountConfirmationPopupComponent.prototype.displayErrorMessage = function (linkResponse) {
        switch (linkResponse.ErrorReason) {
            case link_account_request_model_1.ErrorReason.ERX_UPDATE_FAILURE:
                {
                    this.showMessage = true;
                    this.linkConfMessageText = "Your existing Veradigm Security Account will successfully be linked to ePrescribe account upon login.";
                    this.setImageIcon("INFORMATION");
                    this.showLinkedAccountConfPopup = true;
                    this.showLinkedAccountConfLayout = true;
                    break;
                }
            default:
                {
                    this.showMessage = true;
                    this.showLinkedAccountConfPopup = false;
                    this.showLinkedAccountConfLayout = false;
                    break;
                }
        }
    };
    LinkAccountConfirmationPopupComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'link-account-confirmation-popup',
            template: __webpack_require__(/*! ./link-account-confirmation-popup-template.html */ "./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-template.html"),
            styles: [__webpack_require__(/*! ./link-account-confirmation-popup.style.css */ "./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup.style.css")]
        }),
        tslib_1.__param(2, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [router_1.Router,
            loader_service_1.LoaderService, Object])
    ], LinkAccountConfirmationPopupComponent);
    return LinkAccountConfirmationPopupComponent;
}());
exports.LinkAccountConfirmationPopupComponent = LinkAccountConfirmationPopupComponent;


/***/ }),

/***/ "./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-template.html":
/*!**************************************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-template.html ***!
  \**************************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"modal-backdrop fade in\" [style.display]=\"showLinkedAccountConfLayout ? 'block' : 'none'\"></div>\r\n<div *ngIf=\"showLinkedAccountConfPopup\">\r\n    <div class=\"modal\" id=\"mdlLinkedAccount\" [style.display]=\"'block'\" role=\"dialog\" [style.top]=\"'30%'\">\r\n        <div class=\"modal-dialog link-account-conf-dialog-size\">\r\n            <div class=\"modal-content\">\r\n                <div class=\"reg-overlayTitle\">\r\n                    Veradigm Security Account Confirmation\r\n                </div>\r\n                <div class=\"modal-body\">\r\n                    <div id=\"dvMessage\" [class]=\"imageIcon\" *ngIf=\"showMessage\">\r\n                        <span class=\"status-image-size\">\r\n                            <img id=\"imgStatus\" [src]=\"linkConfImageUrl\" />\r\n                        </span>\r\n                        <span>\r\n                            {{ linkConfMessageText }}\r\n                        </span>\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        Your existing Veradigm Security Account has been successfully linked to this eRX System.\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        <span class=\"erx-form-label\">\r\n                            <label>First Name:</label>\r\n                        </span>\r\n                        <span >\r\n                            <label id=\"lblFirstName\" class=\"link-account-conf-label-text\">{{ linkExistingUserResponse.FirstName}}</label>\r\n                        </span>\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        <span class=\"erx-form-label\">\r\n                            <label>Last Name:</label>\r\n                        </span>\r\n                        <span >\r\n                            <label id=\"lblLastName\" class=\"link-account-conf-label-text\">{{ linkExistingUserResponse.LastName}}</label>\r\n                        </span>\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        <span class=\"erx-form-label\">\r\n                            <label>Email:</label>\r\n                        </span>\r\n                        <span>\r\n                            <label id=\"lblEmail\" class=\"link-account-conf-label-text\">{{ linkExistingUserResponse.Email}}</label>\r\n                        </span>\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        <span class=\"erx-form-label\">\r\n                            <label>Security Account Name:</label>\r\n                        </span>\r\n                        <span>\r\n                            <label id=\"lblAccountName\"  class=\"link-account-conf-label-text\">{{ linkExistingUserResponse.AccountName}}</label>\r\n                        </span>\r\n                    </div>\r\n                </div>\r\n                <div class=\"reg-overlayFooter\">\r\n                        <button type=\"submit\" class=\"button-style\" ID=\"btnLinkNext\" (click)=\"goNext();\">Next</button>\r\n                    </div>\r\n                </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n"

/***/ }),

/***/ "./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup.style.css":
/*!**********************************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup.style.css ***!
  \**********************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".link-account-conf-dialog-size {\r\n    width: 600px;\r\n}\r\n\r\n.success-dialog {\r\n    margin-left: 10px;\r\n    background-color: #d6ecee;\r\n    padding-bottom: 5px;\r\n}\r\n\r\n.information-dialog {\r\n    margin-left: 10px;\r\n    background-color: #fff2cd;\r\n    padding-bottom: 5px;\r\n}\r\n\r\n.status-image-size {\r\n    width: 25px;\r\n}\r\n\r\n.link-account-conf-label-text {\r\n    font-weight: normal;\r\n    padding-top:1px;\r\n}\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY3RpdmF0ZS11c2VyL2xpbmstYWNjb3VudC1jb25maXJtYXRpb24tcG9wdXAvbGluay1hY2NvdW50LWNvbmZpcm1hdGlvbi1wb3B1cC5zdHlsZS5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxZQUFZO0FBQ2hCOztBQUVBO0lBQ0ksaUJBQWlCO0lBQ2pCLHlCQUF5QjtJQUN6QixtQkFBbUI7QUFDdkI7O0FBRUE7SUFDSSxpQkFBaUI7SUFDakIseUJBQXlCO0lBQ3pCLG1CQUFtQjtBQUN2Qjs7QUFDQTtJQUNJLFdBQVc7QUFDZjs7QUFDQTtJQUNJLG1CQUFtQjtJQUNuQixlQUFlO0FBQ25CIiwiZmlsZSI6IlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY3RpdmF0ZS11c2VyL2xpbmstYWNjb3VudC1jb25maXJtYXRpb24tcG9wdXAvbGluay1hY2NvdW50LWNvbmZpcm1hdGlvbi1wb3B1cC5zdHlsZS5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyIubGluay1hY2NvdW50LWNvbmYtZGlhbG9nLXNpemUge1xyXG4gICAgd2lkdGg6IDYwMHB4O1xyXG59XHJcblxyXG4uc3VjY2Vzcy1kaWFsb2cge1xyXG4gICAgbWFyZ2luLWxlZnQ6IDEwcHg7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZDZlY2VlO1xyXG4gICAgcGFkZGluZy1ib3R0b206IDVweDtcclxufVxyXG5cclxuLmluZm9ybWF0aW9uLWRpYWxvZyB7XHJcbiAgICBtYXJnaW4tbGVmdDogMTBweDtcclxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmYyY2Q7XHJcbiAgICBwYWRkaW5nLWJvdHRvbTogNXB4O1xyXG59XHJcbi5zdGF0dXMtaW1hZ2Utc2l6ZSB7XHJcbiAgICB3aWR0aDogMjVweDtcclxufVxyXG4ubGluay1hY2NvdW50LWNvbmYtbGFiZWwtdGV4dCB7XHJcbiAgICBmb250LXdlaWdodDogbm9ybWFsO1xyXG4gICAgcGFkZGluZy10b3A6MXB4O1xyXG59Il19 */"

/***/ }),

/***/ "./src/app/registration/activate-user/link-account-request-model.ts":
/*!**************************************************************************!*\
  !*** ./src/app/registration/activate-user/link-account-request-model.ts ***!
  \**************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.Workflow = exports.ErrorReason = exports.LinkExistingUserModel = exports.LinkAccountRequestModel = void 0;
var LinkAccountRequestModel = /** @class */ (function () {
    function LinkAccountRequestModel() {
    }
    return LinkAccountRequestModel;
}());
exports.LinkAccountRequestModel = LinkAccountRequestModel;
var LinkExistingUserModel = /** @class */ (function () {
    function LinkExistingUserModel() {
    }
    return LinkExistingUserModel;
}());
exports.LinkExistingUserModel = LinkExistingUserModel;
var ErrorReason;
(function (ErrorReason) {
    ErrorReason[ErrorReason["NOT_AUTHENTICATED"] = 0] = "NOT_AUTHENTICATED";
    ErrorReason[ErrorReason["FAILED_TO_LINK_USER"] = 1] = "FAILED_TO_LINK_USER";
    ErrorReason[ErrorReason["ERX_UPDATE_FAILURE"] = 2] = "ERX_UPDATE_FAILURE";
    ErrorReason[ErrorReason["FAILED_TO_RETRIEVE_USER_GUID"] = 3] = "FAILED_TO_RETRIEVE_USER_GUID";
    ErrorReason[ErrorReason["USER_PROFILE_ALREADY_LINKED_INSIDE_THIS_TENANT"] = 4] = "USER_PROFILE_ALREADY_LINKED_INSIDE_THIS_TENANT";
    ErrorReason[ErrorReason["EXTERNAL_ID_IS_NOT_USER_GUID"] = 5] = "EXTERNAL_ID_IS_NOT_USER_GUID";
    ErrorReason[ErrorReason["USERNAME_ALREADY_EXISTS_FOR_LICENSE"] = 6] = "USERNAME_ALREADY_EXISTS_FOR_LICENSE";
})(ErrorReason = exports.ErrorReason || (exports.ErrorReason = {}));
var Workflow;
(function (Workflow) {
    Workflow[Workflow["None"] = 0] = "None";
    Workflow[Workflow["Sso"] = 1] = "Sso";
    Workflow[Workflow["Utilities"] = 2] = "Utilities";
    Workflow[Workflow["Main"] = 3] = "Main";
})(Workflow = exports.Workflow || (exports.Workflow = {}));


/***/ }),

/***/ "./src/app/registration/activate-user/link-existing-account/link-existing-account-component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/link-existing-account/link-existing-account-component.ts ***!
  \*****************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.LinkExistingAccount = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var activation_code_service_1 = __webpack_require__(/*! ../../../service/activation-code.service */ "./src/app/service/activation-code.service.ts");
var link_account_request_model_1 = __webpack_require__(/*! ../link-account-request-model */ "./src/app/registration/activate-user/link-account-request-model.ts");
var link_account_confirmation_popup_component_1 = __webpack_require__(/*! ../../activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component */ "./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component.ts");
var user_existing_account_component_1 = __webpack_require__(/*! ../../user-existing-account/user-existing-account.component */ "./src/app/registration/user-existing-account/user-existing-account.component.ts");
var LinkExistingAccount = /** @class */ (function () {
    function LinkExistingAccount(router, activationService) {
        this.router = router;
        this.activationService = activationService;
        this.linkAccountModel = new link_account_request_model_1.LinkAccountRequestModel();
        this.isExistingAccountValid = false;
    }
    LinkExistingAccount.prototype.ngOnInit = function () {
    };
    LinkExistingAccount.prototype.createUser = function () {
        this.router.navigate(["register/createAccount"]);
    };
    LinkExistingAccount.prototype.linkUserToExistingAccount = function () {
        var _this = this;
        this.getExistingAccountDetails();
        if (this.isExistingAccountValid) {
            this.activationService.linkToExistingUser(this.linkAccountModel).subscribe(function (linkShieldResponse) {
                _this.linkedAccountConfirmPopup.linkExistingUserResponse = linkShieldResponse;
                if (_this.linkedAccountConfirmPopup.linkExistingUserResponse.IsValid) {
                    _this.IsLinkedUser = false;
                    _this.linkedAccountConfirmPopup.showLinkedAccountConfLayout = true;
                    _this.linkedAccountConfirmPopup.showLinkedAccountConfPopup = true;
                    _this.linkedAccountConfirmPopup.workFlowType = linkShieldResponse.WorkflowType;
                    _this.linkedAccountConfirmPopup.showLinkedAccountConfirmation();
                }
                else {
                    _this.DisplayErrorMessage(_this.linkedAccountConfirmPopup.linkExistingUserResponse);
                }
            });
        }
    };
    LinkExistingAccount.prototype.setMessageImageIcon = function (value) {
        switch (value.toUpperCase()) {
            case "SUCCESS":
                this.linkImageUrl = "../images/info-global-16-x-16.png";
                this.linkImageIcon = "success-dialog";
                break;
            case "INFORMATION":
                this.linkImageUrl = "../images/warning-global-16-x-16.png";
                this.linkImageIcon = "information-dialog";
                break;
            case "ERROR":
                this.linkImageUrl = "../images/alert-global-16-x-16.png";
                this.linkImageIcon = "error-dialog";
                break;
            default:
                this.linkImageUrl = "../images/info-global-16-x-16.png";
                this.linkImageIcon = "success-dialog";
                break;
        }
    };
    LinkExistingAccount.prototype.DisplayErrorMessage = function (linkResponse) {
        switch (linkResponse.ErrorReason) {
            case link_account_request_model_1.ErrorReason.NOT_AUTHENTICATED:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Incorrect Login ID or Password was specified.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case link_account_request_model_1.ErrorReason.FAILED_TO_LINK_USER:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Unable to link your Veradigm Security Account to your ePrescribe Account, Please try again.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case link_account_request_model_1.ErrorReason.ERX_UPDATE_FAILURE:
                {
                    this.IsLinkedUser = false;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfLayout = true;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfPopup = true;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfirmation();
                    return;
                }
            case link_account_request_model_1.ErrorReason.FAILED_TO_RETRIEVE_USER_GUID:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Failed to retrieve your eRx information from your Security Account.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case link_account_request_model_1.ErrorReason.USER_PROFILE_ALREADY_LINKED_INSIDE_THIS_TENANT:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Your Login ID is already linked to another profile in this license.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case link_account_request_model_1.ErrorReason.USERNAME_ALREADY_EXISTS_FOR_LICENSE:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Your username already exists within this license.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
        }
        this.linkedAccountConfirmPopup.showLinkedAccountConfLayout = false;
        this.linkedAccountConfirmPopup.showLinkedAccountConfPopup = false;
        return true;
    };
    LinkExistingAccount.prototype.getExistingAccountDetails = function () {
        var existingUserAccount;
        existingUserAccount = this.userExisting.GetExistingAccountDetails();
        this.linkAccountModel.shieldUserName = existingUserAccount.shieldUserName;
        this.linkAccountModel.shieldPassword = existingUserAccount.password;
        this.isExistingAccountValid = existingUserAccount.isValid;
    };
    tslib_1.__decorate([
        core_1.ViewChild('linkedAccountConfirmPopup'),
        tslib_1.__metadata("design:type", link_account_confirmation_popup_component_1.LinkAccountConfirmationPopupComponent)
    ], LinkExistingAccount.prototype, "linkedAccountConfirmPopup", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('userExisting'),
        tslib_1.__metadata("design:type", user_existing_account_component_1.UserExistingAccountComponent)
    ], LinkExistingAccount.prototype, "userExisting", void 0);
    LinkExistingAccount = tslib_1.__decorate([
        core_1.Component({
            selector: 'link-existing-account-template',
            template: __webpack_require__(/*! ./link-existing-account-template.html */ "./src/app/registration/activate-user/link-existing-account/link-existing-account-template.html"),
            styles: [__webpack_require__(/*! ./link-existing-account.style.css */ "./src/app/registration/activate-user/link-existing-account/link-existing-account.style.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [router_1.Router, activation_code_service_1.ActivationCodeService])
    ], LinkExistingAccount);
    return LinkExistingAccount;
}());
exports.LinkExistingAccount = LinkExistingAccount;


/***/ }),

/***/ "./src/app/registration/activate-user/link-existing-account/link-existing-account-template.html":
/*!******************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/link-existing-account/link-existing-account-template.html ***!
  \******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"modal-backdrop fade in\" [style.display]=\"IsLinkedUser ? 'block' : 'none'\"></div>\r\n<div *ngIf=\"IsLinkedUser\">\r\n    <div class=\"modal\" id=\"mdlLinkedAccount\" [style.display]=\"'block'\" role=\"dialog\" [style.top]=\"'20%'\">\r\n       <div class=\"modal-dialog  link-existing-account-dialog-size\">\r\n            <div class=\"modal-content\">\r\n                <div class=\"reg-overlayTitle\">\r\n                    Veradigm Security Account: Link to Existing Account\r\n                </div>\r\n                <div class=\"modal-body\">\r\n                    <div id=\"dvMessage\" [class]=\"linkImageIcon\" *ngIf=\"showLinkMessage\">\r\n                        <span  class=\"status-image-size\">\r\n                            <img id=\"imgStatus\" [src]=\"linkImageUrl\" />\r\n                        </span>\r\n                        <span>\r\n                            {{ linkMessageText }}\r\n                        </span>\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        Please enter your Veradigm Security Account credentials to link to your existing account.\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        <user-existing-account #userExisting></user-existing-account>\r\n                    </div>\r\n                    <div class=\"erx-form-row\">\r\n                        If you have forgotten your User Name or Password, please log into the system you originally used to configure your\r\n                        Veradigm Security Account to view user name and/or change password.\r\n                    </div>\r\n                </div>\r\n                <div class=\"reg-overlayFooter\">\r\n                        <span><button type=\"submit\" class=\"button-style\" (click)=\"linkUserToExistingAccount()\"> Link Account </button> </span>\r\n                        <span class=\"not-registered-style\">Not a registered user?</span>\r\n                        <span> <a routerLink=\"\" (click)=\"createUser()\"> Enroll now </a></span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<link-account-confirmation-popup #linkedAccountConfirmPopup></link-account-confirmation-popup>\r\n"

/***/ }),

/***/ "./src/app/registration/activate-user/link-existing-account/link-existing-account.style.css":
/*!**************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/link-existing-account/link-existing-account.style.css ***!
  \**************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".not-registered-style {\r\n    margin-top: 4px;\r\n    margin-left: 10px;\r\n    margin-right: 10px;\r\n}\r\n\r\n.success-dialog {\r\n    background-color: #d6ecee;\r\n    margin-left: 10px;\r\n    width:100%;\r\n}\r\n\r\n.error-dialog {\r\n    background-color: #ffe4e4;\r\n    margin-left: 10px;\r\n    width: 100%;\r\n}\r\n\r\n.information-dialog {\r\n    background-color: #fff2cd;\r\n    margin-left: 10px;\r\n    width: 100%;\r\n}\r\n\r\n.link-existing-account-dialog-size {\r\n    width: 600px;\r\n}\r\n\r\n.status-image-size {\r\n    width: 25px;\r\n}\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY3RpdmF0ZS11c2VyL2xpbmstZXhpc3RpbmctYWNjb3VudC9saW5rLWV4aXN0aW5nLWFjY291bnQuc3R5bGUuY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0lBQ0ksZUFBZTtJQUNmLGlCQUFpQjtJQUNqQixrQkFBa0I7QUFDdEI7O0FBRUE7SUFDSSx5QkFBeUI7SUFDekIsaUJBQWlCO0lBQ2pCLFVBQVU7QUFDZDs7QUFFQTtJQUNJLHlCQUF5QjtJQUN6QixpQkFBaUI7SUFDakIsV0FBVztBQUNmOztBQUVBO0lBQ0kseUJBQXlCO0lBQ3pCLGlCQUFpQjtJQUNqQixXQUFXO0FBQ2Y7O0FBRUE7SUFDSSxZQUFZO0FBQ2hCOztBQUVBO0lBQ0ksV0FBVztBQUNmIiwiZmlsZSI6IlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY3RpdmF0ZS11c2VyL2xpbmstZXhpc3RpbmctYWNjb3VudC9saW5rLWV4aXN0aW5nLWFjY291bnQuc3R5bGUuY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLm5vdC1yZWdpc3RlcmVkLXN0eWxlIHtcclxuICAgIG1hcmdpbi10b3A6IDRweDtcclxuICAgIG1hcmdpbi1sZWZ0OiAxMHB4O1xyXG4gICAgbWFyZ2luLXJpZ2h0OiAxMHB4O1xyXG59XHJcblxyXG4uc3VjY2Vzcy1kaWFsb2cge1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2Q2ZWNlZTtcclxuICAgIG1hcmdpbi1sZWZ0OiAxMHB4O1xyXG4gICAgd2lkdGg6MTAwJTtcclxufVxyXG5cclxuLmVycm9yLWRpYWxvZyB7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZlNGU0O1xyXG4gICAgbWFyZ2luLWxlZnQ6IDEwcHg7XHJcbiAgICB3aWR0aDogMTAwJTtcclxufVxyXG5cclxuLmluZm9ybWF0aW9uLWRpYWxvZyB7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZmMmNkO1xyXG4gICAgbWFyZ2luLWxlZnQ6IDEwcHg7XHJcbiAgICB3aWR0aDogMTAwJTtcclxufVxyXG5cclxuLmxpbmstZXhpc3RpbmctYWNjb3VudC1kaWFsb2ctc2l6ZSB7XHJcbiAgICB3aWR0aDogNjAwcHg7XHJcbn1cclxuXHJcbi5zdGF0dXMtaW1hZ2Utc2l6ZSB7XHJcbiAgICB3aWR0aDogMjVweDtcclxufSJdfQ== */"

/***/ }),

/***/ "./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.css":
/*!******************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.css ***!
  \******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".user-activation-dialog-size{\r\n    width:750px;\r\n}\r\n.erx-activation-label {\r\n    float: left;\r\n    padding-right: 10px;\r\n}\r\n.btn-spacing{\r\n    margin-left:20px;\r\n}\r\n.user-activation-captcha-control-spacing {\r\n    padding-left: 68px;\r\n}\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY3RpdmF0ZS11c2VyL3VzZXItYWN0aXZhdGlvbi1wb3B1cC91c2VyLWFjdGl2YXRpb24tcG9wdXAuY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLFdBQVc7QUFDZjtBQUNBO0lBQ0ksV0FBVztJQUNYLG1CQUFtQjtBQUN2QjtBQUVBO0lBQ0ksZ0JBQWdCO0FBQ3BCO0FBR0E7SUFDSSxrQkFBa0I7QUFDdEIiLCJmaWxlIjoiU1BBUmVnaXN0cmF0aW9uL3NyYy9hcHAvcmVnaXN0cmF0aW9uL2FjdGl2YXRlLXVzZXIvdXNlci1hY3RpdmF0aW9uLXBvcHVwL3VzZXItYWN0aXZhdGlvbi1wb3B1cC5jb21wb25lbnQuY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLnVzZXItYWN0aXZhdGlvbi1kaWFsb2ctc2l6ZXtcclxuICAgIHdpZHRoOjc1MHB4O1xyXG59XHJcbi5lcngtYWN0aXZhdGlvbi1sYWJlbCB7XHJcbiAgICBmbG9hdDogbGVmdDtcclxuICAgIHBhZGRpbmctcmlnaHQ6IDEwcHg7XHJcbn1cclxuXHJcbi5idG4tc3BhY2luZ3tcclxuICAgIG1hcmdpbi1sZWZ0OjIwcHg7XHJcbn1cclxuXHJcblxyXG4udXNlci1hY3RpdmF0aW9uLWNhcHRjaGEtY29udHJvbC1zcGFjaW5nIHtcclxuICAgIHBhZGRpbmctbGVmdDogNjhweDtcclxufSJdfQ== */"

/***/ }),

/***/ "./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.html":
/*!*******************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.html ***!
  \*******************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #userActivationForm=\"ngForm\" (ngSubmit)=\"activateUser(userActivationForm)\">\r\n    <div class=\"modal-backdrop fade in\" [style.display]=\"ShowModal ? 'block' : 'none'\"></div>\r\n    <div *ngIf=\"ShowModal\">\r\n        <div class=\"modal\" id=\"mdlUserActivation\" [style.display]=\"'block'\" role=\"dialog\" [style.top]=\"'30%'\">\r\n            <div class=\"modal-dialog user-activation-dialog-size\">\r\n                <div class=\"modal-content\">\r\n                    <div class=\"reg-overlayTitle\" id=\"divTitle\">\r\n                        Activate Profile\r\n                    </div>\r\n                    <div class=\"modal-body\">\r\n                        <div id=\"erx-form-row\">\r\n                            Please enter the activation code provided by an Administrator.\r\n                        </div>\r\n                        <div class=\"erx-form-row\">\r\n                            <span class=\"erx-form-label\">\r\n                                <label>Activation Code</label>\r\n                            </span>\r\n                            <span class=\"erx-form-input\">\r\n                                <span class=\"highlight-star erx-form-star-position\">*</span>\r\n                                <input id=\"txtActivationCode\" name=\"txtActivationCode\" #txtActivationCode=\"ngModel\" class=\"input-medium\" required [(ngModel)]=\"activationCodeModel.ActivationCode\" />\r\n                                <span *ngIf=\"txtActivationCode.invalid && (txtActivationCode.dirty || txtActivationCode.touched)\">\r\n                                    <span id=\"requiredActivationCodeError\" *ngIf=\"txtActivationCode.errors.required\" class=\"validation-color\">\r\n                                        Activation code is required.\r\n                                    </span>\r\n                                    <span id=\"alreadyUsedActivationCodeError\" *ngIf=\"!txtActivationCode.errors.required && txtActivationCode.errors.used\" class=\"validation-color\">\r\n                                        Activation code is not valid or has already been used.\r\n                                    </span>\r\n                                </span>\r\n                            </span>\r\n                        </div>\r\n                        <div class=\"erx-form-row user-activation-captcha-control-spacing\">\r\n                            <show-captcha #captchaControl></show-captcha>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"reg-overlayFooter\">\r\n                        <button class=\"button-style\"  type=\"submit\" name=\"btnActivate\" id=\"btnActivate\">Activate</button>\r\n                        <button type=\"button\" class=\"button-style btn-spacing\"  data-dismiss=\"modal\" (click)=\"hide()\">Cancel</button>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</form>\r\n<link-existing-account-template #linkExistingAccountControl></link-existing-account-template>"

/***/ }),

/***/ "./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.ts":
/*!*****************************************************************************************************!*\
  !*** ./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.ts ***!
  \*****************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserActivationPopup = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var show_captcha_component_1 = __webpack_require__(/*! ../../show-captcha/show-captcha.component */ "./src/app/registration/show-captcha/show-captcha.component.ts");
var link_existing_account_component_1 = __webpack_require__(/*! ../link-existing-account/link-existing-account-component */ "./src/app/registration/activate-user/link-existing-account/link-existing-account-component.ts");
var activation_code_service_1 = __webpack_require__(/*! ../../../service/activation-code.service */ "./src/app/service/activation-code.service.ts");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var activation_code_model_1 = __webpack_require__(/*! ../activation-code.model */ "./src/app/registration/activate-user/activation-code.model.ts");
var UserActivationPopup = /** @class */ (function () {
    function UserActivationPopup(activationService, router) {
        this.activationService = activationService;
        this.router = router;
        this.activationCodeModel = new activation_code_model_1.ActivationCodeModel();
        this.isValidCaptcha = false;
    }
    UserActivationPopup.prototype.isUserActivationFormValid = function (data) {
        this.markAllControlAsTouched(data);
        this.getCaptchaDetails();
        return data.valid
            && this.isValidCaptcha;
    };
    UserActivationPopup.prototype.markAllControlAsTouched = function (data) {
        Object.keys(data.controls).forEach(function (field) {
            var control = data.controls[field];
            control.markAsTouched({ onlySelf: true });
        });
    };
    UserActivationPopup.prototype.activateUser = function (activationForm) {
        var _this = this;
        if (this.isUserActivationFormValid(activationForm)) {
            this.activationService.validateActivationCode(this.activationCodeModel).subscribe(function (validateActivationCodeModel) {
                if (validateActivationCodeModel.IsValid) {
                    activationForm.controls["txtActivationCode"].setErrors(null);
                    _this.ShowModal = false;
                    if (_this.ShowLinkedAccount) {
                        _this.showLinkedAccountPopup();
                    }
                    else {
                        _this.router.navigate(["register/createAccount"]);
                    }
                }
                else {
                    if (!validateActivationCodeModel.IsValidCaptcha) {
                        _this.captchaControl.ShowInValidCaptchaError = true;
                    }
                    else {
                        if (activationForm.controls["txtActivationCode"].errors == null) {
                            activationForm.controls["txtActivationCode"].setErrors({
                                "used": "invalid"
                            });
                        }
                    }
                    _this.ShowModal = true;
                }
            });
        }
    };
    UserActivationPopup.prototype.hide = function () {
        this.activationCodeModel = new activation_code_model_1.ActivationCodeModel();
        this.ShowModal = false;
    };
    UserActivationPopup.prototype.showLinkedAccountPopup = function () {
        this.linkedAccount.IsLinkedUser = true;
    };
    UserActivationPopup.prototype.getCaptchaDetails = function () {
        var captchaModel;
        captchaModel = this.captchaControl.GetCaptchaDetails();
        this.activationCodeModel.CaptchaText = captchaModel.captchaText;
        this.isValidCaptcha = captchaModel.isValid;
    };
    tslib_1.__decorate([
        core_1.ViewChild('captchaControl'),
        tslib_1.__metadata("design:type", show_captcha_component_1.ShowCaptchaComponent)
    ], UserActivationPopup.prototype, "captchaControl", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('linkExistingAccountControl'),
        tslib_1.__metadata("design:type", link_existing_account_component_1.LinkExistingAccount)
    ], UserActivationPopup.prototype, "linkedAccount", void 0);
    UserActivationPopup = tslib_1.__decorate([
        core_1.Component({
            selector: 'user-activation-popup',
            template: __webpack_require__(/*! ./user-activation-popup.component.html */ "./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.html"),
            styles: [__webpack_require__(/*! ./user-activation-popup.component.css */ "./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [activation_code_service_1.ActivationCodeService, router_1.Router])
    ], UserActivationPopup);
    return UserActivationPopup;
}());
exports.UserActivationPopup = UserActivationPopup;


/***/ }),

/***/ "./src/app/registration/activate-user/user-activation/user-activation.component.css":
/*!******************************************************************************************!*\
  !*** ./src/app/registration/activate-user/user-activation/user-activation.component.css ***!
  \******************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".activation-container {\r\n    display: flex;\r\n    flex-flow: row wrap;\r\n    justify-content: center;\r\n    padding: 45px;\r\n}\r\n\r\n.activation-card {\r\n    float: left;\r\n    width: 45%;\r\n    min-width: 400px;\r\n    margin: 10px;\r\n    text-align: center;\r\n    padding: 15px 0px;\r\n    border-radius: 10px;\r\n    border: solid;\r\n    border-color: #5a5353;\r\n    border-width: thin;\r\n    background-color: #ffffff;\r\n}\r\n\r\n.activation-card-header {\r\n    margin: 0;\r\n    padding-bottom: 20px;\r\n}\r\n\r\n.activation-header-title {\r\n    font-size: 19px;\r\n    font-weight: bold;\r\n    color: #5a5353;\r\n    padding-top: 13px;\r\n    text-align: center;\r\n    height: 50px;\r\n}\r\n\r\n.activation-card-body {\r\n    font-weight: 400;\r\n    height: 100px;\r\n    font-size: 14px;\r\n    padding-left: 10px;\r\n    padding-right: 10px;\r\n}\r\n\r\n.activation-card-row {\r\n    height: 50px;\r\n}\r\n\r\n.activationText {\r\n    font-style: italic;\r\n}\r\n\r\n\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY3RpdmF0ZS11c2VyL3VzZXItYWN0aXZhdGlvbi91c2VyLWFjdGl2YXRpb24uY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLGFBQWE7SUFDYixtQkFBbUI7SUFDbkIsdUJBQXVCO0lBQ3ZCLGFBQWE7QUFDakI7O0FBRUE7SUFDSSxXQUFXO0lBQ1gsVUFBVTtJQUNWLGdCQUFnQjtJQUNoQixZQUFZO0lBQ1osa0JBQWtCO0lBQ2xCLGlCQUFpQjtJQUNqQixtQkFBbUI7SUFDbkIsYUFBYTtJQUNiLHFCQUFxQjtJQUNyQixrQkFBa0I7SUFDbEIseUJBQXlCO0FBQzdCOztBQUVBO0lBQ0ksU0FBUztJQUNULG9CQUFvQjtBQUN4Qjs7QUFFQTtJQUNJLGVBQWU7SUFDZixpQkFBaUI7SUFDakIsY0FBYztJQUNkLGlCQUFpQjtJQUNqQixrQkFBa0I7SUFDbEIsWUFBWTtBQUNoQjs7QUFFQTtJQUNJLGdCQUFnQjtJQUNoQixhQUFhO0lBQ2IsZUFBZTtJQUNmLGtCQUFrQjtJQUNsQixtQkFBbUI7QUFDdkI7O0FBRUE7SUFDSSxZQUFZO0FBQ2hCOztBQUVBO0lBQ0ksa0JBQWtCO0FBQ3RCIiwiZmlsZSI6IlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9hY3RpdmF0ZS11c2VyL3VzZXItYWN0aXZhdGlvbi91c2VyLWFjdGl2YXRpb24uY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbIi5hY3RpdmF0aW9uLWNvbnRhaW5lciB7XHJcbiAgICBkaXNwbGF5OiBmbGV4O1xyXG4gICAgZmxleC1mbG93OiByb3cgd3JhcDtcclxuICAgIGp1c3RpZnktY29udGVudDogY2VudGVyO1xyXG4gICAgcGFkZGluZzogNDVweDtcclxufVxyXG5cclxuLmFjdGl2YXRpb24tY2FyZCB7XHJcbiAgICBmbG9hdDogbGVmdDtcclxuICAgIHdpZHRoOiA0NSU7XHJcbiAgICBtaW4td2lkdGg6IDQwMHB4O1xyXG4gICAgbWFyZ2luOiAxMHB4O1xyXG4gICAgdGV4dC1hbGlnbjogY2VudGVyO1xyXG4gICAgcGFkZGluZzogMTVweCAwcHg7XHJcbiAgICBib3JkZXItcmFkaXVzOiAxMHB4O1xyXG4gICAgYm9yZGVyOiBzb2xpZDtcclxuICAgIGJvcmRlci1jb2xvcjogIzVhNTM1MztcclxuICAgIGJvcmRlci13aWR0aDogdGhpbjtcclxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmZmZmY7XHJcbn1cclxuXHJcbi5hY3RpdmF0aW9uLWNhcmQtaGVhZGVyIHtcclxuICAgIG1hcmdpbjogMDtcclxuICAgIHBhZGRpbmctYm90dG9tOiAyMHB4O1xyXG59XHJcblxyXG4uYWN0aXZhdGlvbi1oZWFkZXItdGl0bGUge1xyXG4gICAgZm9udC1zaXplOiAxOXB4O1xyXG4gICAgZm9udC13ZWlnaHQ6IGJvbGQ7XHJcbiAgICBjb2xvcjogIzVhNTM1MztcclxuICAgIHBhZGRpbmctdG9wOiAxM3B4O1xyXG4gICAgdGV4dC1hbGlnbjogY2VudGVyO1xyXG4gICAgaGVpZ2h0OiA1MHB4O1xyXG59XHJcblxyXG4uYWN0aXZhdGlvbi1jYXJkLWJvZHkge1xyXG4gICAgZm9udC13ZWlnaHQ6IDQwMDtcclxuICAgIGhlaWdodDogMTAwcHg7XHJcbiAgICBmb250LXNpemU6IDE0cHg7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDEwcHg7XHJcbiAgICBwYWRkaW5nLXJpZ2h0OiAxMHB4O1xyXG59XHJcblxyXG4uYWN0aXZhdGlvbi1jYXJkLXJvdyB7XHJcbiAgICBoZWlnaHQ6IDUwcHg7XHJcbn1cclxuXHJcbi5hY3RpdmF0aW9uVGV4dCB7XHJcbiAgICBmb250LXN0eWxlOiBpdGFsaWM7XHJcbn1cclxuXHJcblxyXG4iXX0= */"

/***/ }),

/***/ "./src/app/registration/activate-user/user-activation/user-activation.component.html":
/*!*******************************************************************************************!*\
  !*** ./src/app/registration/activate-user/user-activation/user-activation.component.html ***!
  \*******************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div id=\"activation-title\" class=\"title-bar title-bar-heading branded-background-color branded-font-color\">\r\n    Welcome to the Veradigm Security Account Activation Wizard!\r\n</div>\r\n<div *ngIf=\"workflow\" class=\"activation-container\">\r\n    <div id=\"divEnrollNow\" class=\"activation-card\">\r\n        <div class=\"activation-card-header\">\r\n            <span class=\"activation-header-title\">Enroll Now</span>\r\n        </div>\r\n        <div class=\"activation-card-body\">\r\n            <div class=\"activation-card-row\">\r\n                <span ID=\"lblEnrollDescription\">Please use this option to create a new Veradigm Security Account.</span>\r\n            </div>\r\n            <div class=\"activation-card-row\">\r\n                <span class=\"activationText\" id=\"lblEnrollActivation\" *ngIf=\"isActivationCodeVisible\">(Activation code is required).</span>\r\n            </div>\r\n        </div>\r\n        <div>\r\n            <button id=\"btnSignUp\" Text=\"Sign Up\" class=\"button-style\" (click)=\"OpenNewUserActivationPopup()\">Sign Up</button>\r\n        </div>\r\n    </div>\r\n\r\n    <div id=\"divLinkAccounts\" class=\"activation-card\">\r\n        <div class=\"activation-card-header\">\r\n            <span class=\"activation-header-title\">Link Accounts</span>\r\n        </div>\r\n        <div class=\"activation-card-body\">\r\n            <div class=\"activation-card-row\">\r\n                <span id=\"Label1\">Please use this option to use an existing Veradigm Security Account Login if you have one.</span>\r\n            </div>\r\n            <div class=\"activation-card-row\">\r\n                <span class=\"activationText\" id=\"lblLinkActivation\" *ngIf=\"isLinkActivationCodeVisible\">(Activation code is required).</span>\r\n            </div>\r\n        </div>\r\n        <div>\r\n            <button id=\"btnLinkAccounts\" class=\"button-style\" (click)=\"OpenLinkAccountActivationPopup()\">Link Accounts</button>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n<user-activation-popup #userActivationModalPopup></user-activation-popup>\r\n\r\n\r\n"

/***/ }),

/***/ "./src/app/registration/activate-user/user-activation/user-activation.component.ts":
/*!*****************************************************************************************!*\
  !*** ./src/app/registration/activate-user/user-activation/user-activation.component.ts ***!
  \*****************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserActivationComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var user_activation_popup_component_1 = __webpack_require__(/*! ../user-activation-popup/user-activation-popup.component */ "./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.ts");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var activation_code_service_1 = __webpack_require__(/*! ../../../service/activation-code.service */ "./src/app/service/activation-code.service.ts");
var link_account_request_model_1 = __webpack_require__(/*! ../link-account-request-model */ "./src/app/registration/activate-user/link-account-request-model.ts");
var UserActivationComponent = /** @class */ (function () {
    function UserActivationComponent(route, activationService, router) {
        this.route = route;
        this.activationService = activationService;
        this.router = router;
        this.isActivationCodeVisible = true;
        this.isLinkActivationCodeVisible = true;
    }
    UserActivationComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.appType = this.route.snapshot.queryParamMap.get('App');
        this.activationService.intializeWorkFlow({ AppType: this.appType }).subscribe(function (workflow) {
            _this.workflow = workflow;
            if (_this.workflow == link_account_request_model_1.Workflow.Sso) {
                _this.isActivationCodeVisible = false;
                _this.isLinkActivationCodeVisible = false;
            }
        });
    };
    UserActivationComponent.prototype.OpenNewUserActivationPopup = function () {
        if (this.workflow == link_account_request_model_1.Workflow.Sso) {
            this.userActivationModalPopup.ShowModal = false;
            this.userActivationModalPopup.WorkflowType = this.workflow;
            this.userActivationModalPopup.ShowLinkedAccount = false;
            this.router.navigate(["register/createAccount"]);
        }
        else {
            this.userActivationModalPopup.ShowModal = true;
            this.userActivationModalPopup.WorkflowType = this.workflow;
            this.userActivationModalPopup.ShowLinkedAccount = false;
        }
    };
    UserActivationComponent.prototype.OpenLinkAccountActivationPopup = function () {
        if (this.workflow == link_account_request_model_1.Workflow.Sso) {
            this.userActivationModalPopup.ShowLinkedAccount = true;
            this.userActivationModalPopup.ShowModal = false;
            this.userActivationModalPopup.WorkflowType = this.workflow;
            this.userActivationModalPopup.showLinkedAccountPopup();
        }
        else {
            this.userActivationModalPopup.ShowModal = true;
            this.userActivationModalPopup.ShowLinkedAccount = true;
            this.userActivationModalPopup.WorkflowType = this.workflow;
        }
    };
    tslib_1.__decorate([
        core_1.ViewChild('userActivationModalPopup'),
        tslib_1.__metadata("design:type", user_activation_popup_component_1.UserActivationPopup)
    ], UserActivationComponent.prototype, "userActivationModalPopup", void 0);
    UserActivationComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-user-activation',
            template: __webpack_require__(/*! ./user-activation.component.html */ "./src/app/registration/activate-user/user-activation/user-activation.component.html"),
            styles: [__webpack_require__(/*! ./user-activation.component.css */ "./src/app/registration/activate-user/user-activation/user-activation.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [router_1.ActivatedRoute,
            activation_code_service_1.ActivationCodeService,
            router_1.Router])
    ], UserActivationComponent);
    return UserActivationComponent;
}());
exports.UserActivationComponent = UserActivationComponent;


/***/ }),

/***/ "./src/app/registration/can-load-registration.guard.ts":
/*!*************************************************************!*\
  !*** ./src/app/registration/can-load-registration.guard.ts ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.CanActivateRegistrationStep = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var operators_1 = __webpack_require__(/*! rxjs/operators */ "../node_modules/rxjs/_esm5/operators/index.js");
var registration_service_1 = __webpack_require__(/*! ../service/registration.service */ "./src/app/service/registration.service.ts");
var registration_routing_module_1 = __webpack_require__(/*! ./registration-routing.module */ "./src/app/registration/registration-routing.module.ts");
var message_service_1 = __webpack_require__(/*! ../service/message.service */ "./src/app/service/message.service.ts");
var http_1 = __webpack_require__(/*! @angular/common/http */ "../node_modules/@angular/common/fesm5/http.js");
var CanActivateRegistrationStep = /** @class */ (function () {
    function CanActivateRegistrationStep(registrationService, router, window, messageService) {
        var _a;
        this.registrationService = registrationService;
        this.router = router;
        this.window = window;
        this.messageService = messageService;
        this.defaultRoute = "/" + registration_routing_module_1.RegistrationRoutes.default;
        this.registrantRouteDecoder = function (registrantContext, requestedRoute) {
            if (registrantContext.IsLOA3StatusConfirmed)
                return "/" + registration_routing_module_1.RegistrationRoutes.createlicense;
            if (registrantContext.IsUserCreated || registrantContext.IseRxUser)
                return "/" + registration_routing_module_1.RegistrationRoutes.welcome; //redirect to csp 
            if (!registrantContext.IsUserCreated)
                return "/" + registration_routing_module_1.RegistrationRoutes.createuser; //Redirect to create user page.
            return requestedRoute;
        };
        this.handlers = (_a = {},
            _a["/" + registration_routing_module_1.RegistrationRoutes.createuser] = this.registrantRouteDecoder,
            _a["/" + registration_routing_module_1.RegistrationRoutes.welcome] = this.registrantRouteDecoder,
            _a["/" + registration_routing_module_1.RegistrationRoutes.createlicense] = this.registrantRouteDecoder,
            _a);
    }
    CanActivateRegistrationStep.prototype.canActivate = function (route, routerStateSnapshot) {
        var _this = this;
        var currentState = this.router.getCurrentNavigation().extras.state;
        if ((currentState === null || currentState === void 0 ? void 0 : currentState.authentication) == false && routerStateSnapshot.url.indexOf("/" + registration_routing_module_1.RegistrationRoutes.createuser) > -1)
            return true;
        return this.registrationService
            .getRegistrantStatus(routerStateSnapshot.url == "/" + registration_routing_module_1.RegistrationRoutes.createlicense)
            .pipe(operators_1.map(function (registrantContext) {
            if (!registrantContext) {
                throw new http_1.HttpErrorResponse({ status: 401, statusText: "not authenticated" });
            }
            return _this.interpretRegistantStatus(routerStateSnapshot.url, registrantContext);
        }), operators_1.catchError(function (err, caught) {
            var url = '/register';
            var naviagtionExtras = { queryParamsHandling: "merge" };
            if ((err === null || err === void 0 ? void 0 : err.status) == 401 && routerStateSnapshot.url.indexOf("/" + registration_routing_module_1.RegistrationRoutes.createuser) > -1) {
                url = routerStateSnapshot.url;
                var tempState = {
                    state: tslib_1.__assign(tslib_1.__assign({}, currentState), { authentication: false })
                };
                naviagtionExtras = tslib_1.__assign(tslib_1.__assign({}, naviagtionExtras), tempState);
                return _this.router.navigateByUrl(url, naviagtionExtras);
            }
            if ((err === null || err === void 0 ? void 0 : err.status) == 401) {
                _this.messageService.notify("Not authorized");
            }
            return Promise.resolve(false);
        }));
    };
    CanActivateRegistrationStep.prototype.interpretRegistantStatus = function (command, regitratContext) {
        if (!this.containsHandler(command))
            return this.router.parseUrl(this.defaultRoute);
        var interpretedRoute = this.handlers[command](regitratContext, command);
        if (interpretedRoute === command)
            return true;
        if (!this.isKnownRoute(interpretedRoute)) {
            this.window.location.href = interpretedRoute;
            return false;
        }
        return this.router.parseUrl(interpretedRoute);
    };
    CanActivateRegistrationStep.prototype.isKnownRoute = function (url) {
        var routes = this.router.config;
        //Note:url contains starting /.. hence the substring
        return routes && routes.length > 0 && routes.find(function (x) { return x.path.indexOf(url.substring(1)) > -1; }) != undefined;
    };
    CanActivateRegistrationStep.prototype.containsHandler = function (key) {
        return this.handlers[key] != null;
    };
    CanActivateRegistrationStep = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__param(2, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [registration_service_1.RegistrationService,
            router_1.Router, Object, message_service_1.MessageService])
    ], CanActivateRegistrationStep);
    return CanActivateRegistrationStep;
}());
exports.CanActivateRegistrationStep = CanActivateRegistrationStep;


/***/ }),

/***/ "./src/app/registration/error-registration/error-registration.component.css":
/*!**********************************************************************************!*\
  !*** ./src/app/registration/error-registration/error-registration.component.css ***!
  \**********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".reg-error-message {\r\n    color: #a94442;\r\n    background-color: #f2dede;\r\n    border-color: #ebccd1;\r\n    padding: .9em;\r\n    margin-bottom: 20px;\r\n    border: 1px solid transparent;\r\n    border-radius: 4px;\r\n    margin: 1em;\r\n}\r\n\r\n    .reg-error-message p {\r\n        max-width: 60%;\r\n        line-height: 2em;\r\n    }\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9lcnJvci1yZWdpc3RyYXRpb24vZXJyb3ItcmVnaXN0cmF0aW9uLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxjQUFjO0lBQ2QseUJBQXlCO0lBQ3pCLHFCQUFxQjtJQUNyQixhQUFhO0lBQ2IsbUJBQW1CO0lBQ25CLDZCQUE2QjtJQUM3QixrQkFBa0I7SUFDbEIsV0FBVztBQUNmOztJQUVJO1FBQ0ksY0FBYztRQUNkLGdCQUFnQjtJQUNwQiIsImZpbGUiOiJTUEFSZWdpc3RyYXRpb24vc3JjL2FwcC9yZWdpc3RyYXRpb24vZXJyb3ItcmVnaXN0cmF0aW9uL2Vycm9yLXJlZ2lzdHJhdGlvbi5jb21wb25lbnQuY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLnJlZy1lcnJvci1tZXNzYWdlIHtcclxuICAgIGNvbG9yOiAjYTk0NDQyO1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2YyZGVkZTtcclxuICAgIGJvcmRlci1jb2xvcjogI2ViY2NkMTtcclxuICAgIHBhZGRpbmc6IC45ZW07XHJcbiAgICBtYXJnaW4tYm90dG9tOiAyMHB4O1xyXG4gICAgYm9yZGVyOiAxcHggc29saWQgdHJhbnNwYXJlbnQ7XHJcbiAgICBib3JkZXItcmFkaXVzOiA0cHg7XHJcbiAgICBtYXJnaW46IDFlbTtcclxufVxyXG5cclxuICAgIC5yZWctZXJyb3ItbWVzc2FnZSBwIHtcclxuICAgICAgICBtYXgtd2lkdGg6IDYwJTtcclxuICAgICAgICBsaW5lLWhlaWdodDogMmVtO1xyXG4gICAgfVxyXG4iXX0= */"

/***/ }),

/***/ "./src/app/registration/error-registration/error-registration.component.html":
/*!***********************************************************************************!*\
  !*** ./src/app/registration/error-registration/error-registration.component.html ***!
  \***********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"reg-error-message\" *ngIf=\"messageService.message && messageService.message.length>0\">\r\n    <div class=\"message\"><p>{{messageService.message}}</p></div>\r\n</div>\r\n<div class=\"reg-error-message\" *ngIf=\"!messageService.message || messageService!.message!.length<=0\">\r\n    <div class=\"message\"><p>{{defaultErrorMessage}}</p></div>\r\n</div>"

/***/ }),

/***/ "./src/app/registration/error-registration/error-registration.component.ts":
/*!*********************************************************************************!*\
  !*** ./src/app/registration/error-registration/error-registration.component.ts ***!
  \*********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ErrorRegistrationComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var message_service_1 = __webpack_require__(/*! ../../service/message.service */ "./src/app/service/message.service.ts");
var ErrorRegistrationComponent = /** @class */ (function () {
    function ErrorRegistrationComponent(messageService) {
        this.messageService = messageService;
        this.defaultErrorMessage = "Something went wrong while showing this page";
    }
    ErrorRegistrationComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'error-registration',
            template: __webpack_require__(/*! ./error-registration.component.html */ "./src/app/registration/error-registration/error-registration.component.html"),
            styles: [__webpack_require__(/*! ./error-registration.component.css */ "./src/app/registration/error-registration/error-registration.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [message_service_1.MessageService])
    ], ErrorRegistrationComponent);
    return ErrorRegistrationComponent;
}());
exports.ErrorRegistrationComponent = ErrorRegistrationComponent;


/***/ }),

/***/ "./src/app/registration/license-creation/license-creation.component.css":
/*!******************************************************************************!*\
  !*** ./src/app/registration/license-creation/license-creation.component.css ***!
  \******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".license-creation-form-label-spacing {\r\n    width: 150px;\r\n}\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9saWNlbnNlLWNyZWF0aW9uL2xpY2Vuc2UtY3JlYXRpb24uY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLFlBQVk7QUFDaEIiLCJmaWxlIjoiU1BBUmVnaXN0cmF0aW9uL3NyYy9hcHAvcmVnaXN0cmF0aW9uL2xpY2Vuc2UtY3JlYXRpb24vbGljZW5zZS1jcmVhdGlvbi5jb21wb25lbnQuY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLmxpY2Vuc2UtY3JlYXRpb24tZm9ybS1sYWJlbC1zcGFjaW5nIHtcclxuICAgIHdpZHRoOiAxNTBweDtcclxufVxyXG4iXX0= */"

/***/ }),

/***/ "./src/app/registration/license-creation/license-creation.component.html":
/*!*******************************************************************************!*\
  !*** ./src/app/registration/license-creation/license-creation.component.html ***!
  \*******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #licenseInfoForm=\"ngForm\" (ngSubmit)=\"onSubmit(licenseInfoForm)\">\r\n    <div id=\"license-creation-title\" class=\"title-bar title-bar-heading branded-background-color branded-font-color\">\r\n        Practice Primary Location Information\r\n    </div>\r\n        <div class=\"erx-form-content-offset font-color-dimmed-black\">\r\n            <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n                Practice Details\r\n            </div>\r\n            <div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>Practice Name</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"txtSiteName\"\r\n                               type=\"text\"\r\n                               name=\"SiteName\"\r\n                               maxlength=\"70\"\r\n                               class=\"input-large\"\r\n                               required\r\n                               [(ngModel)]=\"licenseInfo.PracticeName\"\r\n                               #siteName=\"ngModel\" />\r\n\r\n                        <span *ngIf=\"siteName.invalid && (siteName.dirty || siteName.touched)\">\r\n                            <span *ngIf=\"siteName.errors.required\"\r\n                                  id=\"siteNameMandatoryError\" class=\"validation-color\">\r\n                                Practice Name is required.\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>Address</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"txtAddress1\"\r\n                               type=\"text\"\r\n                               name=\"Address1\"\r\n                               maxlength=\"35\"\r\n                               class=\"input-large\"\r\n                               required\r\n                               [(ngModel)]=\"licenseInfo.Address\"\r\n                               #address1=\"ngModel\" />\r\n\r\n                        <span *ngIf=\"address1.invalid && (address1.dirty || address1.touched)\">\r\n                            <span *ngIf=\"address1.errors.required\"\r\n                                  id=\"address1MandatoryError\" class=\"validation-color\">\r\n                                Address is required.\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label>Address 2</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"txtAddress2\"\r\n                               type=\"text\"\r\n                               name=\"Address2\"\r\n                               maxlength=\"35\"\r\n                               class=\"input-large\"\r\n                               [(ngModel)]=\"licenseInfo.Address2\" />\r\n                    </span>\r\n                </div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>City</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"txtCity\"\r\n                               type=\"text\"\r\n                               name=\"City\"\r\n                               maxlength=\"20\"\r\n                               class=\"input-large\"\r\n                               required\r\n                               [pattern]=\"cityPattern\"\r\n                               [(ngModel)]=\"licenseInfo.City\"\r\n                               #city=\"ngModel\" />\r\n\r\n                        <span *ngIf=\"city.invalid && (city.dirty || city.touched)\">\r\n                            <span *ngIf=\"city.errors\"\r\n                                  id=\"cityMandatoryError\" class=\"validation-color\">\r\n                                Enter a valid city.\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>State</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <select id=\"ddlState\"\r\n                                type=\"text\"\r\n                                name=\"State\"\r\n                                class=\"input-medium\"\r\n                                required\r\n                                [(ngModel)]=\"licenseInfo.State\"\r\n                                #state=\"ngModel\">\r\n                            <option *ngFor=\"let item of stateList\" [value]=\"item.state\">\r\n                                {{item.description}}\r\n                            </option>\r\n                        </select>\r\n                        <span *ngIf=\"state.invalid && (state.dirty || state.touched)\">\r\n                            <span *ngIf=\"state.errors.required\"\r\n                                  id=\"cityMandatoryError\" class=\"validation-color\">\r\n                                Select a valid state.\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>ZIP Code</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"txtZip\"\r\n                               type=\"text\"\r\n                               name=\"Zip\"\r\n                               maxlength=\"9\"\r\n                               class=\"input-small\"\r\n                               required\r\n                               [pattern]=\"zipCodePattern\"\r\n                               [(ngModel)]=\"licenseInfo.Zipcode\"\r\n                               #zip=\"ngModel\" />\r\n\r\n                        <span *ngIf=\"zip.invalid && (city.dirty || zip.touched)\">\r\n                            <span *ngIf=\"zip.errors.required\"\r\n                                  id=\"zipMandatoryError\" class=\"validation-color\">\r\n                                Enter a valid ZIP Code.\r\n                            </span>\r\n                            <span id=\"zipCodePatternError\" *ngIf=\"zip.errors.pattern\" class=\"validation-color\">\r\n                                Enter a valid 5 or 9 digit ZIP code\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>Phone Number</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"txtPhone\"\r\n                               type=\"text\"\r\n                               name=\"Phone\"\r\n                               maxlength=\"14\"\r\n                               class=\"input-small\"\r\n                               required\r\n                               [pattern]=\"phoneFaxPattern\"\r\n                               [(ngModel)]=\"licenseInfo.PhoneNumber\"\r\n                               #phone=\"ngModel\" />\r\n\r\n                        <span *ngIf=\"phone.invalid && (phone.dirty || phone.touched)\">\r\n                            <span *ngIf=\"phone.errors\"\r\n                                  id=\"phoneMandatoryError\" class=\"validation-color\">\r\n                                Enter a valid phone number.\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n                <div class=\"erx-form-row\">\r\n                    <span class=\"erx-form-label license-creation-form-label-spacing\">\r\n                        <label><span class=\"highlight-star erx-form-star-position\">*</span>Fax Number</label>\r\n                    </span>\r\n                    <span class=\"erx-form-input\">\r\n                        <input id=\"txtFax\"\r\n                               type=\"text\"\r\n                               name=\"Fax\"\r\n                               maxlength=\"14\"\r\n                               class=\"input-small\"\r\n                               required\r\n                               [pattern]=\"phoneFaxPattern\"\r\n                               [(ngModel)]=\"licenseInfo.FaxNumber\"\r\n                               #fax=\"ngModel\" />\r\n\r\n                        <span *ngIf=\"fax.invalid && (fax.dirty || fax.touched)\">\r\n                            <span *ngIf=\"fax.errors\"\r\n                                  id=\"faxMandatoryError\" class=\"validation-color\">\r\n                                Enter a valid fax number\r\n                            </span>\r\n                        </span>\r\n                    </span>\r\n                </div>\r\n                <div class=\"erx-form-row\">\r\n                    <button type=\"submit\" id=\"btnSaveSite\" class=\"button-style\">\r\n                        Save Practice\r\n                    </button>\r\n                </div>\r\n            </div>\r\n        </div>\r\n</form>\r\n"

/***/ }),

/***/ "./src/app/registration/license-creation/license-creation.component.ts":
/*!*****************************************************************************!*\
  !*** ./src/app/registration/license-creation/license-creation.component.ts ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.LicenseCreationComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var license_creation_model_1 = __webpack_require__(/*! ../license-creation/license-creation.model */ "./src/app/registration/license-creation/license-creation.model.ts");
var license_create_service_1 = __webpack_require__(/*! ../../service/license-create.service */ "./src/app/service/license-create.service.ts");
var user_create_service_1 = __webpack_require__(/*! ../../service/user-create.service */ "./src/app/service/user-create.service.ts");
var operators_1 = __webpack_require__(/*! rxjs/operators */ "../node_modules/rxjs/_esm5/operators/index.js");
var loader_service_1 = __webpack_require__(/*! ../../service/loader.service */ "./src/app/service/loader.service.ts");
var LicenseCreationComponent = /** @class */ (function () {
    function LicenseCreationComponent(licenseCreateService, loaderService, userCreateService, window) {
        this.licenseCreateService = licenseCreateService;
        this.loaderService = loaderService;
        this.userCreateService = userCreateService;
        this.window = window;
        this.licenseInfo = new license_creation_model_1.LicenseInfo();
        this.phoneFaxPattern = "\\(?\\d{3}\\)?-? *\\d{3}-? *-?\\d{4}";
        this.cityPattern = "^([a-zA-Z]+[\\s-'.]{0,20})*";
        this.zipCodePattern = "^(\\d{5})(?:\\d{4})?$";
    }
    LicenseCreationComponent.prototype.getStateList = function () {
        var _this = this;
        this.userCreateService.getInitialPageData().subscribe(function (stateList) {
            if (stateList.States != null && stateList.States != undefined) {
                _this.stateList = stateList.States;
            }
        });
    };
    LicenseCreationComponent.prototype.ngOnInit = function () {
        this.getStateList();
    };
    LicenseCreationComponent.prototype.phoneFaxFormatting = function (phoneFaxNumber) {
        return phoneFaxNumber.replace(" ", "").replace("(", "").replace(")", "").replace("-", "");
    };
    LicenseCreationComponent.prototype.onSubmit = function (formData) {
        var _this = this;
        if (!formData.valid) {
            Object.keys(formData.controls).forEach(function (field) {
                var control = formData.controls[field];
                control.markAsTouched({ onlySelf: true });
            });
        }
        else {
            this.licenseInfo.PhoneNumber = this.phoneFaxFormatting(this.licenseInfo.PhoneNumber);
            this.licenseInfo.FaxNumber = this.phoneFaxFormatting(this.licenseInfo.FaxNumber);
            this.licenseCreateService
                .updateRegistrantPracticeDetails(this.licenseInfo)
                .pipe(operators_1.filter(function (result) { return !!result; }))
                .subscribe(function (response) {
                var _a, _b;
                _this.loaderService.show(true);
                _this.window.open((_b = (_a = _this.window) === null || _a === void 0 ? void 0 : _a.appcontext) === null || _b === void 0 ? void 0 : _b.mediator, "_self");
            });
        }
    };
    LicenseCreationComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-license-creation',
            template: __webpack_require__(/*! ./license-creation.component.html */ "./src/app/registration/license-creation/license-creation.component.html"),
            styles: [__webpack_require__(/*! ./license-creation.component.css */ "./src/app/registration/license-creation/license-creation.component.css")]
        }),
        tslib_1.__param(3, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [license_create_service_1.LicenseCreateService,
            loader_service_1.LoaderService,
            user_create_service_1.UserCreateService, Object])
    ], LicenseCreationComponent);
    return LicenseCreationComponent;
}());
exports.LicenseCreationComponent = LicenseCreationComponent;


/***/ }),

/***/ "./src/app/registration/license-creation/license-creation.model.ts":
/*!*************************************************************************!*\
  !*** ./src/app/registration/license-creation/license-creation.model.ts ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.StateInfo = exports.LicenseInfo = void 0;
var LicenseInfo = /** @class */ (function () {
    function LicenseInfo() {
    }
    return LicenseInfo;
}());
exports.LicenseInfo = LicenseInfo;
var StateInfo = /** @class */ (function () {
    function StateInfo() {
    }
    return StateInfo;
}());
exports.StateInfo = StateInfo;


/***/ }),

/***/ "./src/app/registration/max-try-failure-popup/max-try-failure.component.css":
/*!**********************************************************************************!*\
  !*** ./src/app/registration/max-try-failure-popup/max-try-failure.component.css ***!
  \**********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJTUEFSZWdpc3RyYXRpb24vc3JjL2FwcC9yZWdpc3RyYXRpb24vbWF4LXRyeS1mYWlsdXJlLXBvcHVwL21heC10cnktZmFpbHVyZS5jb21wb25lbnQuY3NzIn0= */"

/***/ }),

/***/ "./src/app/registration/max-try-failure-popup/max-try-failure.component.html":
/*!***********************************************************************************!*\
  !*** ./src/app/registration/max-try-failure-popup/max-try-failure.component.html ***!
  \***********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"modal\" id=\"myModal\" role=\"dialog\" [style.display]=\"'block'\" [style.top]=\"'25%'\">\r\n    <div class=\"modal-dialog\">\r\n        <div class=\"modal-content\">\r\n             <div class=\"reg-overlayTitle\">\r\n               User Creation Information\r\n            </div>\r\n            <div class=\"modal-body\">\r\n                User creation is taking longer than usual, please re-login with your created Username ({{shieldUserName}}) after 15 minutes\r\n                <br /><br />\r\n                You will be now redirected to Login page. Click continue.\r\n            </div>\r\n            <div class=\"reg-overlayFooter\">\r\n                <button type=\"button\" class=\"button-style\" data-dismiss=\"modal\" (click)=\"hide()\">Continue</button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n"

/***/ }),

/***/ "./src/app/registration/max-try-failure-popup/max-try-failure.component.ts":
/*!*********************************************************************************!*\
  !*** ./src/app/registration/max-try-failure-popup/max-try-failure.component.ts ***!
  \*********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.MaxTryFailureComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var MaxTryFailureComponent = /** @class */ (function () {
    function MaxTryFailureComponent(window) {
        this.window = window;
    }
    MaxTryFailureComponent.prototype.hide = function () {
        var _a, _b;
        this.window.open((_b = (_a = this.window) === null || _a === void 0 ? void 0 : _a.appcontext) === null || _b === void 0 ? void 0 : _b.mediator, "_self");
    };
    tslib_1.__decorate([
        core_1.Input(),
        tslib_1.__metadata("design:type", String)
    ], MaxTryFailureComponent.prototype, "shieldUserName", void 0);
    MaxTryFailureComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'max-retry-failure-popup',
            template: __webpack_require__(/*! ./max-try-failure.component.html */ "./src/app/registration/max-try-failure-popup/max-try-failure.component.html"),
            styles: [__webpack_require__(/*! ./max-try-failure.component.css */ "./src/app/registration/max-try-failure-popup/max-try-failure.component.css")]
        }),
        tslib_1.__param(0, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [Object])
    ], MaxTryFailureComponent);
    return MaxTryFailureComponent;
}());
exports.MaxTryFailureComponent = MaxTryFailureComponent;


/***/ }),

/***/ "./src/app/registration/registration-routing.module.ts":
/*!*************************************************************!*\
  !*** ./src/app/registration/registration-routing.module.ts ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.RegistrationRoutingModule = exports.registrationRoutes = exports.RegistrationRoutes = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var subscription_component_1 = __webpack_require__(/*! ./subscription/subscription.component */ "./src/app/registration/subscription/subscription.component.ts");
var user_creation_component_1 = __webpack_require__(/*! ./user-creation/user-creation.component */ "./src/app/registration/user-creation/user-creation.component.ts");
var welcome_page_component_1 = __webpack_require__(/*! ./welcome-page/welcome-page.component */ "./src/app/registration/welcome-page/welcome-page.component.ts");
var can_load_registration_guard_1 = __webpack_require__(/*! ./can-load-registration.guard */ "./src/app/registration/can-load-registration.guard.ts");
var license_creation_component_1 = __webpack_require__(/*! ./license-creation/license-creation.component */ "./src/app/registration/license-creation/license-creation.component.ts");
var error_registration_component_1 = __webpack_require__(/*! ./error-registration/error-registration.component */ "./src/app/registration/error-registration/error-registration.component.ts");
var user_activation_component_1 = __webpack_require__(/*! ./activate-user/user-activation/user-activation.component */ "./src/app/registration/activate-user/user-activation/user-activation.component.ts");
var account_creation_component_1 = __webpack_require__(/*! ./account-creation/account-creation.component */ "./src/app/registration/account-creation/account-creation.component.ts");
var user_csp_updater_component_1 = __webpack_require__(/*! ./user-csp-updater/user-csp-updater.component */ "./src/app/registration/user-csp-updater/user-csp-updater.component.ts");
//import { UserCspUpdaterComponent } from './user-csp-updater/user-csp-updater.component';
exports.RegistrationRoutes = {
    default: "register/",
    subscriptions: "register/subscriptions",
    createuser: "register/createuser",
    welcome: "register/welcome",
    createlicense: "register/createlicense",
    error: "register/error",
    activateuser: "register/activateuser",
    createAccount: "register/createAccount",
    usercspupdater: "register/usercspupdater"
};
exports.registrationRoutes = [
    { path: 'Register.aspx', redirectTo: exports.RegistrationRoutes.subscriptions },
    { path: 'register', redirectTo: exports.RegistrationRoutes.subscriptions },
    { path: exports.RegistrationRoutes.subscriptions, component: subscription_component_1.SubscriptionComponent },
    { path: exports.RegistrationRoutes.createuser, component: user_creation_component_1.UserCreationComponent, canActivate: [can_load_registration_guard_1.CanActivateRegistrationStep] },
    { path: exports.RegistrationRoutes.welcome, component: welcome_page_component_1.WelcomePageComponent, canActivate: [can_load_registration_guard_1.CanActivateRegistrationStep] },
    { path: exports.RegistrationRoutes.createlicense, component: license_creation_component_1.LicenseCreationComponent, canActivate: [can_load_registration_guard_1.CanActivateRegistrationStep] },
    { path: exports.RegistrationRoutes.error, component: error_registration_component_1.ErrorRegistrationComponent },
    { path: exports.RegistrationRoutes.activateuser, component: user_activation_component_1.UserActivationComponent },
    { path: exports.RegistrationRoutes.createAccount, component: account_creation_component_1.AccountCreationComponent },
    { path: exports.RegistrationRoutes.usercspupdater, component: user_csp_updater_component_1.UserCspUpdaterComponent }
];
var RegistrationRoutingModule = /** @class */ (function () {
    function RegistrationRoutingModule() {
    }
    RegistrationRoutingModule = tslib_1.__decorate([
        core_1.NgModule({
            imports: [router_1.RouterModule.forRoot(exports.registrationRoutes, { enableTracing: false })],
            providers: [can_load_registration_guard_1.CanActivateRegistrationStep],
            exports: [router_1.RouterModule]
        })
    ], RegistrationRoutingModule);
    return RegistrationRoutingModule;
}());
exports.RegistrationRoutingModule = RegistrationRoutingModule;


/***/ }),

/***/ "./src/app/registration/registration.module.ts":
/*!*****************************************************!*\
  !*** ./src/app/registration/registration.module.ts ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.RegistrationModule = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var common_1 = __webpack_require__(/*! @angular/common */ "../node_modules/@angular/common/fesm5/common.js");
var subscription_component_1 = __webpack_require__(/*! ./subscription/subscription.component */ "./src/app/registration/subscription/subscription.component.ts");
var user_creation_component_1 = __webpack_require__(/*! ./user-creation/user-creation.component */ "./src/app/registration/user-creation/user-creation.component.ts");
var license_creation_component_1 = __webpack_require__(/*! ./license-creation/license-creation.component */ "./src/app/registration/license-creation/license-creation.component.ts");
var forms_1 = __webpack_require__(/*! @angular/forms */ "../node_modules/@angular/forms/fesm5/forms.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var user_create_service_1 = __webpack_require__(/*! ../service/user-create.service */ "./src/app/service/user-create.service.ts");
var welcome_page_component_1 = __webpack_require__(/*! ../registration/welcome-page/welcome-page.component */ "./src/app/registration/welcome-page/welcome-page.component.ts");
var license_create_service_1 = __webpack_require__(/*! ../service/license-create.service */ "./src/app/service/license-create.service.ts");
var welcome_service_1 = __webpack_require__(/*! ../service/welcome-service */ "./src/app/service/welcome-service.ts");
var registration_routing_module_1 = __webpack_require__(/*! ./registration-routing.module */ "./src/app/registration/registration-routing.module.ts");
var error_registration_component_1 = __webpack_require__(/*! ./error-registration/error-registration.component */ "./src/app/registration/error-registration/error-registration.component.ts");
var max_try_failure_component_1 = __webpack_require__(/*! ./max-try-failure-popup/max-try-failure.component */ "./src/app/registration/max-try-failure-popup/max-try-failure.component.ts");
var user_activation_component_1 = __webpack_require__(/*! ../registration/activate-user/user-activation/user-activation.component */ "./src/app/registration/activate-user/user-activation/user-activation.component.ts");
var user_activation_popup_component_1 = __webpack_require__(/*! ../registration/activate-user/user-activation-popup/user-activation-popup.component */ "./src/app/registration/activate-user/user-activation-popup/user-activation-popup.component.ts");
var show_captcha_component_1 = __webpack_require__(/*! ../registration/show-captcha/show-captcha.component */ "./src/app/registration/show-captcha/show-captcha.component.ts");
var captcha_service_1 = __webpack_require__(/*! ../service/captcha.service */ "./src/app/service/captcha.service.ts");
var user_password_component_1 = __webpack_require__(/*! ../registration/user-password/user-password.component */ "./src/app/registration/user-password/user-password.component.ts");
var user_security_questions_component_1 = __webpack_require__(/*! ../registration/user-security-questions/user-security-questions.component */ "./src/app/registration/user-security-questions/user-security-questions.component.ts");
var user_account_name_component_1 = __webpack_require__(/*! ../registration/user-account-name/user-account-name.component */ "./src/app/registration/user-account-name/user-account-name.component.ts");
var user_existing_account_component_1 = __webpack_require__(/*! ../registration/user-existing-account/user-existing-account.component */ "./src/app/registration/user-existing-account/user-existing-account.component.ts");
var activation_code_service_1 = __webpack_require__(/*! ../service/activation-code.service */ "./src/app/service/activation-code.service.ts");
var link_existing_account_component_1 = __webpack_require__(/*! ../registration/activate-user/link-existing-account/link-existing-account-component */ "./src/app/registration/activate-user/link-existing-account/link-existing-account-component.ts");
var account_creation_component_1 = __webpack_require__(/*! ../registration/account-creation/account-creation.component */ "./src/app/registration/account-creation/account-creation.component.ts");
var user_csp_updater_component_1 = __webpack_require__(/*! ./user-csp-updater/user-csp-updater.component */ "./src/app/registration/user-csp-updater/user-csp-updater.component.ts");
var link_account_confirmation_popup_component_1 = __webpack_require__(/*! ../registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component */ "./src/app/registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component.ts");
var RegistrationModule = /** @class */ (function () {
    function RegistrationModule() {
    }
    RegistrationModule = tslib_1.__decorate([
        core_1.NgModule({
            declarations: [subscription_component_1.SubscriptionComponent,
                user_creation_component_1.UserCreationComponent,
                license_creation_component_1.LicenseCreationComponent,
                welcome_page_component_1.WelcomePageComponent,
                error_registration_component_1.ErrorRegistrationComponent,
                max_try_failure_component_1.MaxTryFailureComponent,
                user_activation_component_1.UserActivationComponent,
                user_activation_popup_component_1.UserActivationPopup,
                show_captcha_component_1.ShowCaptchaComponent,
                user_password_component_1.UserPasswordComponent,
                user_security_questions_component_1.UserSecurityQuestionsComponent,
                user_account_name_component_1.UserAccountNameComponent,
                user_existing_account_component_1.UserExistingAccountComponent,
                account_creation_component_1.AccountCreationComponent,
                user_csp_updater_component_1.UserCspUpdaterComponent,
                link_existing_account_component_1.LinkExistingAccount,
                link_account_confirmation_popup_component_1.LinkAccountConfirmationPopupComponent
            ],
            imports: [
                common_1.CommonModule,
                router_1.RouterModule,
                forms_1.FormsModule,
                registration_routing_module_1.RegistrationRoutingModule
            ],
            providers: [
                user_create_service_1.UserCreateService,
                license_create_service_1.LicenseCreateService,
                welcome_service_1.WelcomeService,
                captcha_service_1.CaptchaService,
                activation_code_service_1.ActivationCodeService
            ]
        })
    ], RegistrationModule);
    return RegistrationModule;
}());
exports.RegistrationModule = RegistrationModule;


/***/ }),

/***/ "./src/app/registration/show-captcha/show-captcha.component.css":
/*!**********************************************************************!*\
  !*** ./src/app/registration/show-captcha/show-captcha.component.css ***!
  \**********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".show-captcha-label-group {\r\n    float: left;\r\n    padding-right: 10px;\r\n}\r\n.show-captcha-label-item{\r\n    text-align:right;\r\n}\r\n.show-captcha-refresh-img {\r\n    cursor: pointer;\r\n}\r\n.show-captcha-input-group {\r\n    display: inline-block;\r\n    padding-left: 15px;\r\n    position: relative;\r\n}\r\n.show-captcha-input-item {\r\n    text-align: left;\r\n}\r\n.show-captcha-dimensions {\r\n    height: 50px;\r\n    width: 200px;\r\n}\r\n.show-captcha-input-block {\r\n    display: block;\r\n}\r\n.show-captcha-label-top {\r\n   padding-left: 10px;\r\n}\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9zaG93LWNhcHRjaGEvc2hvdy1jYXB0Y2hhLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxXQUFXO0lBQ1gsbUJBQW1CO0FBQ3ZCO0FBQ0E7SUFDSSxnQkFBZ0I7QUFDcEI7QUFDQTtJQUNJLGVBQWU7QUFDbkI7QUFFQTtJQUNJLHFCQUFxQjtJQUNyQixrQkFBa0I7SUFDbEIsa0JBQWtCO0FBQ3RCO0FBQ0E7SUFDSSxnQkFBZ0I7QUFDcEI7QUFDQTtJQUNJLFlBQVk7SUFDWixZQUFZO0FBQ2hCO0FBR0E7SUFDSSxjQUFjO0FBQ2xCO0FBRUE7R0FDRyxrQkFBa0I7QUFDckIiLCJmaWxlIjoiU1BBUmVnaXN0cmF0aW9uL3NyYy9hcHAvcmVnaXN0cmF0aW9uL3Nob3ctY2FwdGNoYS9zaG93LWNhcHRjaGEuY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbIi5zaG93LWNhcHRjaGEtbGFiZWwtZ3JvdXAge1xyXG4gICAgZmxvYXQ6IGxlZnQ7XHJcbiAgICBwYWRkaW5nLXJpZ2h0OiAxMHB4O1xyXG59XHJcbi5zaG93LWNhcHRjaGEtbGFiZWwtaXRlbXtcclxuICAgIHRleHQtYWxpZ246cmlnaHQ7XHJcbn1cclxuLnNob3ctY2FwdGNoYS1yZWZyZXNoLWltZyB7XHJcbiAgICBjdXJzb3I6IHBvaW50ZXI7XHJcbn1cclxuXHJcbi5zaG93LWNhcHRjaGEtaW5wdXQtZ3JvdXAge1xyXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xyXG4gICAgcGFkZGluZy1sZWZ0OiAxNXB4O1xyXG4gICAgcG9zaXRpb246IHJlbGF0aXZlO1xyXG59XHJcbi5zaG93LWNhcHRjaGEtaW5wdXQtaXRlbSB7XHJcbiAgICB0ZXh0LWFsaWduOiBsZWZ0O1xyXG59XHJcbi5zaG93LWNhcHRjaGEtZGltZW5zaW9ucyB7XHJcbiAgICBoZWlnaHQ6IDUwcHg7XHJcbiAgICB3aWR0aDogMjAwcHg7XHJcbn1cclxuXHJcblxyXG4uc2hvdy1jYXB0Y2hhLWlucHV0LWJsb2NrIHtcclxuICAgIGRpc3BsYXk6IGJsb2NrO1xyXG59XHJcblxyXG4uc2hvdy1jYXB0Y2hhLWxhYmVsLXRvcCB7XHJcbiAgIHBhZGRpbmctbGVmdDogMTBweDtcclxufVxyXG4iXX0= */"

/***/ }),

/***/ "./src/app/registration/show-captcha/show-captcha.component.html":
/*!***********************************************************************!*\
  !*** ./src/app/registration/show-captcha/show-captcha.component.html ***!
  \***********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #captchaForm=\"ngForm\">\r\n    <div class=\"show-captcha-label-group\">\r\n        <div class=\"show-captcha-label-item\">\r\n            <label>Captcha</label>\r\n        </div>\r\n        <div class=\"show-captcha-label-item\">\r\n            <img (click)=\"refreshCaptcha()\" title=\"Reload Captcha\" src=\"/images/registration/Refresh.gif\" class=\"show-captcha-refresh-img\" />\r\n        </div>\r\n    </div>\r\n    <div>\r\n        <img [src]=\"captchaImgSrc\" id=\"imgCaptcha\" class=\"show-captcha-dimensions\" />\r\n        <div class=\"show-captcha-input-group\">\r\n            <div class=\"show-captcha-input-item\">\r\n                <span class=\"show-captcha-label-top\">Type the Captcha code here</span>\r\n            </div>\r\n            <div class=\"show-captcha-input-item\">\r\n                <span class=\"erx-form-input\">\r\n                    <span class=\"highlight-star erx-form-star-position\">*</span>\r\n                    <input type=\"text\" id=\"txtCaptchaResponse\" name=\"txtCaptchaResponse\" class=\"input-small\"\r\n                           required [(ngModel)]=\"captchaModel.captchaText\" (change)=\"onCaptchaChanged()\" #txtCaptchaResponse=\"ngModel\" />\r\n                    <span *ngIf=\"txtCaptchaResponse.invalid && (txtCaptchaResponse.dirty || txtCaptchaResponse.touched)\">\r\n                        <span id=\"captchaRequiredError\" *ngIf=\"txtCaptchaResponse.errors.required\" class=\"validation-color\">\r\n                            Captcha is required.\r\n                        </span>\r\n                    </span>\r\n                    <span id=\"invalidCaptchaError\" *ngIf=\"ShowInValidCaptchaError\" class=\"validation-color\">\r\n                        Invalid Captcha.\r\n                    </span>\r\n                </span>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</form>\r\n\r\n"

/***/ }),

/***/ "./src/app/registration/show-captcha/show-captcha.component.ts":
/*!*********************************************************************!*\
  !*** ./src/app/registration/show-captcha/show-captcha.component.ts ***!
  \*********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ShowCaptchaComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var platform_browser_1 = __webpack_require__(/*! @angular/platform-browser */ "../node_modules/@angular/platform-browser/fesm5/platform-browser.js");
var captcha_service_1 = __webpack_require__(/*! ../../service/captcha.service */ "./src/app/service/captcha.service.ts");
var show_captcha_model_1 = __webpack_require__(/*! ../show-captcha/show-captcha.model */ "./src/app/registration/show-captcha/show-captcha.model.ts");
var forms_1 = __webpack_require__(/*! @angular/forms */ "../node_modules/@angular/forms/fesm5/forms.js");
var ShowCaptchaComponent = /** @class */ (function () {
    function ShowCaptchaComponent(captchaService, domSanitizer) {
        this.captchaService = captchaService;
        this.domSanitizer = domSanitizer;
        this.captchaModel = new show_captcha_model_1.CaptchaModel();
        this.ShowInValidCaptchaError = false;
    }
    ShowCaptchaComponent.prototype.ngOnInit = function () {
        this.getCaptcha();
    };
    ShowCaptchaComponent.prototype.refreshCaptcha = function () {
        this.getCaptcha();
    };
    ShowCaptchaComponent.prototype.getCaptcha = function () {
        var _this = this;
        this.captchaService.getCaptcha().subscribe(function (base64String) {
            _this.captchaImgSrc = _this.domSanitizer.bypassSecurityTrustUrl('data:image/png;base64,' + base64String);
        });
    };
    ShowCaptchaComponent.prototype.onCaptchaChanged = function () {
        this.ShowInValidCaptchaError = false;
    };
    ;
    ShowCaptchaComponent.prototype.GetCaptchaDetails = function () {
        this.captchaForm.controls.txtCaptchaResponse.markAsTouched({ onlySelf: true });
        this.captchaModel.isValid = this.captchaForm.valid;
        return this.captchaModel;
    };
    tslib_1.__decorate([
        core_1.Input(),
        tslib_1.__metadata("design:type", Boolean)
    ], ShowCaptchaComponent.prototype, "ShowInValidCaptchaError", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('captchaForm'),
        tslib_1.__metadata("design:type", forms_1.NgForm)
    ], ShowCaptchaComponent.prototype, "captchaForm", void 0);
    ShowCaptchaComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'show-captcha',
            template: __webpack_require__(/*! ./show-captcha.component.html */ "./src/app/registration/show-captcha/show-captcha.component.html"),
            styles: [__webpack_require__(/*! ./show-captcha.component.css */ "./src/app/registration/show-captcha/show-captcha.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [captcha_service_1.CaptchaService, platform_browser_1.DomSanitizer])
    ], ShowCaptchaComponent);
    return ShowCaptchaComponent;
}());
exports.ShowCaptchaComponent = ShowCaptchaComponent;


/***/ }),

/***/ "./src/app/registration/show-captcha/show-captcha.model.ts":
/*!*****************************************************************!*\
  !*** ./src/app/registration/show-captcha/show-captcha.model.ts ***!
  \*****************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.CaptchaModel = void 0;
var CaptchaModel = /** @class */ (function () {
    function CaptchaModel() {
    }
    return CaptchaModel;
}());
exports.CaptchaModel = CaptchaModel;


/***/ }),

/***/ "./src/app/registration/subscription/subscription.component.css":
/*!**********************************************************************!*\
  !*** ./src/app/registration/subscription/subscription.component.css ***!
  \**********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".subscription-card {\r\n    float: left;\r\n    width: 470px;\r\n    min-width: 470px;\r\n    margin: 10px;\r\n    text-align: center;\r\n    background-color: #ffffff;\r\n    padding: 15px 0px;\r\n    border-radius: 10px;\r\n    border: solid;\r\n    border-color: #5a5353;\r\n    border-width: thin;\r\n}\r\n\r\n.subscription-container {\r\n    background-color: #E3E3E3;\r\n    display: flex;\r\n    flex-flow: row wrap;\r\n    justify-content: center;\r\n    padding: 45px;\r\n}\r\n\r\n.subscription-card-header {\r\n    margin: 0;\r\n    padding-bottom: 20px;\r\n}\r\n\r\n.subscription-header-title {\r\n    font-size: 19px;\r\n    font-weight: bold;\r\n    color: #5a5353;\r\n    padding-top: 13px;\r\n    text-align: center;\r\n    height: 50px;\r\n}\r\n\r\n.subscription-card-body {\r\n    background-color: #ffffff;\r\n    font-weight: 400;\r\n    height: 300px;\r\n    background-color: #ffffff;\r\n    font-size: 16px;\r\n    font-weight: 400;\r\n    padding-left: 10px;\r\n    padding-left: 10px;\r\n    padding-right: 10px;\r\n}\r\n\r\n.subscription-price {\r\n    color: #5a5353;\r\n    font-weight: bold;\r\n    vertical-align: bottom;\r\n    font-size: 16px;\r\n}\r\n\r\n.subscription-link {\r\n    cursor: pointer;\r\n    color: #5e4da5;\r\n    font-size: 16px;\r\n    text-decoration:underline;\r\n}\r\n\r\n.subscription-card-row {\r\n    text-align: center;\r\n    font-size: 13px;\r\n    padding-top: 6px;\r\n}\r\n\r\n.subscription-card-group {\r\n    height: 160px;\r\n    margin-bottom: 10px;\r\n}\r\n\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9zdWJzY3JpcHRpb24vc3Vic2NyaXB0aW9uLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxXQUFXO0lBQ1gsWUFBWTtJQUNaLGdCQUFnQjtJQUNoQixZQUFZO0lBQ1osa0JBQWtCO0lBQ2xCLHlCQUF5QjtJQUN6QixpQkFBaUI7SUFDakIsbUJBQW1CO0lBQ25CLGFBQWE7SUFDYixxQkFBcUI7SUFDckIsa0JBQWtCO0FBQ3RCOztBQUVBO0lBQ0kseUJBQXlCO0lBQ3pCLGFBQWE7SUFDYixtQkFBbUI7SUFDbkIsdUJBQXVCO0lBQ3ZCLGFBQWE7QUFDakI7O0FBRUE7SUFDSSxTQUFTO0lBQ1Qsb0JBQW9CO0FBQ3hCOztBQUVBO0lBQ0ksZUFBZTtJQUNmLGlCQUFpQjtJQUNqQixjQUFjO0lBQ2QsaUJBQWlCO0lBQ2pCLGtCQUFrQjtJQUNsQixZQUFZO0FBQ2hCOztBQUVBO0lBQ0kseUJBQXlCO0lBQ3pCLGdCQUFnQjtJQUNoQixhQUFhO0lBQ2IseUJBQXlCO0lBQ3pCLGVBQWU7SUFDZixnQkFBZ0I7SUFDaEIsa0JBQWtCO0lBQ2xCLGtCQUFrQjtJQUNsQixtQkFBbUI7QUFDdkI7O0FBRUE7SUFDSSxjQUFjO0lBQ2QsaUJBQWlCO0lBQ2pCLHNCQUFzQjtJQUN0QixlQUFlO0FBQ25COztBQUdBO0lBQ0ksZUFBZTtJQUNmLGNBQWM7SUFDZCxlQUFlO0lBQ2YseUJBQXlCO0FBQzdCOztBQUVBO0lBQ0ksa0JBQWtCO0lBQ2xCLGVBQWU7SUFDZixnQkFBZ0I7QUFDcEI7O0FBRUE7SUFDSSxhQUFhO0lBQ2IsbUJBQW1CO0FBQ3ZCIiwiZmlsZSI6IlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi9zdWJzY3JpcHRpb24vc3Vic2NyaXB0aW9uLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuc3Vic2NyaXB0aW9uLWNhcmQge1xyXG4gICAgZmxvYXQ6IGxlZnQ7XHJcbiAgICB3aWR0aDogNDcwcHg7XHJcbiAgICBtaW4td2lkdGg6IDQ3MHB4O1xyXG4gICAgbWFyZ2luOiAxMHB4O1xyXG4gICAgdGV4dC1hbGlnbjogY2VudGVyO1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2ZmZmZmZjtcclxuICAgIHBhZGRpbmc6IDE1cHggMHB4O1xyXG4gICAgYm9yZGVyLXJhZGl1czogMTBweDtcclxuICAgIGJvcmRlcjogc29saWQ7XHJcbiAgICBib3JkZXItY29sb3I6ICM1YTUzNTM7XHJcbiAgICBib3JkZXItd2lkdGg6IHRoaW47XHJcbn1cclxuXHJcbi5zdWJzY3JpcHRpb24tY29udGFpbmVyIHtcclxuICAgIGJhY2tncm91bmQtY29sb3I6ICNFM0UzRTM7XHJcbiAgICBkaXNwbGF5OiBmbGV4O1xyXG4gICAgZmxleC1mbG93OiByb3cgd3JhcDtcclxuICAgIGp1c3RpZnktY29udGVudDogY2VudGVyO1xyXG4gICAgcGFkZGluZzogNDVweDtcclxufVxyXG5cclxuLnN1YnNjcmlwdGlvbi1jYXJkLWhlYWRlciB7XHJcbiAgICBtYXJnaW46IDA7XHJcbiAgICBwYWRkaW5nLWJvdHRvbTogMjBweDtcclxufVxyXG5cclxuLnN1YnNjcmlwdGlvbi1oZWFkZXItdGl0bGUge1xyXG4gICAgZm9udC1zaXplOiAxOXB4O1xyXG4gICAgZm9udC13ZWlnaHQ6IGJvbGQ7XHJcbiAgICBjb2xvcjogIzVhNTM1MztcclxuICAgIHBhZGRpbmctdG9wOiAxM3B4O1xyXG4gICAgdGV4dC1hbGlnbjogY2VudGVyO1xyXG4gICAgaGVpZ2h0OiA1MHB4O1xyXG59XHJcblxyXG4uc3Vic2NyaXB0aW9uLWNhcmQtYm9keSB7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZmZmZmO1xyXG4gICAgZm9udC13ZWlnaHQ6IDQwMDtcclxuICAgIGhlaWdodDogMzAwcHg7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZmZmZmO1xyXG4gICAgZm9udC1zaXplOiAxNnB4O1xyXG4gICAgZm9udC13ZWlnaHQ6IDQwMDtcclxuICAgIHBhZGRpbmctbGVmdDogMTBweDtcclxuICAgIHBhZGRpbmctbGVmdDogMTBweDtcclxuICAgIHBhZGRpbmctcmlnaHQ6IDEwcHg7XHJcbn1cclxuXHJcbi5zdWJzY3JpcHRpb24tcHJpY2Uge1xyXG4gICAgY29sb3I6ICM1YTUzNTM7XHJcbiAgICBmb250LXdlaWdodDogYm9sZDtcclxuICAgIHZlcnRpY2FsLWFsaWduOiBib3R0b207XHJcbiAgICBmb250LXNpemU6IDE2cHg7XHJcbn1cclxuXHJcblxyXG4uc3Vic2NyaXB0aW9uLWxpbmsge1xyXG4gICAgY3Vyc29yOiBwb2ludGVyO1xyXG4gICAgY29sb3I6ICM1ZTRkYTU7XHJcbiAgICBmb250LXNpemU6IDE2cHg7XHJcbiAgICB0ZXh0LWRlY29yYXRpb246dW5kZXJsaW5lO1xyXG59XHJcblxyXG4uc3Vic2NyaXB0aW9uLWNhcmQtcm93IHtcclxuICAgIHRleHQtYWxpZ246IGNlbnRlcjtcclxuICAgIGZvbnQtc2l6ZTogMTNweDtcclxuICAgIHBhZGRpbmctdG9wOiA2cHg7XHJcbn1cclxuXHJcbi5zdWJzY3JpcHRpb24tY2FyZC1ncm91cCB7XHJcbiAgICBoZWlnaHQ6IDE2MHB4O1xyXG4gICAgbWFyZ2luLWJvdHRvbTogMTBweDtcclxufVxyXG5cclxuIl19 */"

/***/ }),

/***/ "./src/app/registration/subscription/subscription.component.html":
/*!***********************************************************************!*\
  !*** ./src/app/registration/subscription/subscription.component.html ***!
  \***********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"subscription-container\">\r\n    <div *ngIf=\"!model.enterprisePricing\">\r\n\r\n        <div class=\"subscription-card\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Basic\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-basicPrice\" class=\"subscription-price\">Only ${{model.price.basic}} per month per prescriber</span>\r\n                    - 30 day free trial!\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n\r\n                    <span id=\"linkBasicRegister\" (click)=\"redirectToCreateUser('CompulsoryBasic')\" class=\"subscription-link\">Register Now</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkBasicLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span>\r\n                </div>\r\n            </div>\r\n        </div> <!--End of First Panel-->\r\n\r\n        <div class=\"subscription-card\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Deluxe\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Access Live Customer Support\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-deluxePrice\" class=\"subscription-price\">Only ${{model.price.deluxeLogRx}} per month per prescriber</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n\r\n                    <span id=\"linkDeluxeRegister\" (click)=\"redirectToCreateUser('DeluxeEpaLogRx')\" class=\"subscription-link\">Register Now</span> for ePrescribe Basic.\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Once completed and you're logged in the application, click on the Manage Account tab to upgrade to Deluxe.\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkBasicLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span> and click on the Manage Account\r\n                </div>\r\n            </div>\r\n        </div> <!--End of Second Panel-->\r\n\r\n        <div class=\"subscription-card\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Deluxe with EPCS\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescription Drug Monitoring Program (PDMP)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prescription Controlled Substances (EPCS)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Access Live Customer Support\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-epcsPrice\" class=\"subscription-price\">Only ${{model.price.deluxeEpcsLogRx}} per month per prescriber</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n\r\n                    <span id=\"linkEpcsRegister\" (click)=\"redirectToCreateUser('DeluxeEPCSEpaLogRx2017')\" class=\"subscription-link\">Register Now</span> for ePrescribe Basic.\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Once completed and you're logged in the application, click on the Get EPCS tab to upgrade to Deluxe with EPCS.\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkBasicLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span> and click on Get EPCS\r\n                </div>\r\n            </div>\r\n        </div> <!--End of Third Panel-->\r\n\r\n    </div>\r\n\r\n    <div *ngIf=\"model.enterprisePricing\">\r\n        <div class=\"subscription-card\" *ngIf=\"model.price.basic\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Basic\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-entBasicPrice\" class=\"subscription-price\">Only ${{model.price.basic}} per month per prescriber</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"linkEntBasicRegister\" (click)=\"redirectToCreateUser('CompulsoryBasic')\" class=\"subscription-link\">Register Now</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkBasicLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span>\r\n                </div>\r\n            </div>\r\n        </div> <!--End of enterprise Basic Panel-->\r\n        \r\n        <div class=\"subscription-card\" *ngIf=\"model.price.deluxeLogRx\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Deluxe with Sponsored Messages\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                      <div class=\"subscription-card-row\">\r\n                        Access Live Customer Support\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-entDeluxeLogRxPrice\" class=\"subscription-price\">Only ${{model.price.deluxeLogRx}} per month per prescriber</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"linkEntDeluxeLogRxRegister\" (click)=\"redirectToCreateUser('DeluxeEpaLogRx')\" class=\"subscription-link\">Register Now</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkEntDeluxeLogRxLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span>\r\n                </div>\r\n            </div>\r\n\r\n\r\n        </div> <!--End of enterprise deluxe with ads Panel-->\r\n       \r\n        <div class=\"subscription-card\" *ngIf=\"model.price.deluxe\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Deluxe\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        No Sponsored Messages\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Access Live Customer Support\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-entDeluxePrice\" class=\"subscription-price\">Only ${{model.price.deluxe}} per month per prescriber</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"linkEntDeluxeRegister\" (click)=\"redirectToCreateUser('DeluxeEpa')\" class=\"subscription-link\">Register Now</span>\r\n                </div>\r\n               <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkEntDeluxeLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span>\r\n                </div>\r\n            </div>\r\n        </div> <!--End of enterprise deluxe Panel-->\r\n\r\n        <div class=\"subscription-card\" *ngIf=\"model.price.deluxeEpcsLogRx\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Deluxe with EPCS with Sponsored Messages\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescription Drug Monitoring Program (PDMP)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prescription Controlled Substances (EPCS)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Access Live Customer Support\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-entDeluxeEpcsLogRxPrice\" class=\"subscription-price\">Only ${{model.price.deluxeEpcsLogRx}} per month per prescriber</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    NOTE: There is a ${{model.price.epcsSetup}} per prescriber one-time setup fee for EPCS\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"linkEntDeluxeEpcsLogRxRegister\" (click)=\"redirectToCreateUser('DeluxeEPCSEpaLogRx2017')\" class=\"subscription-link\">Register Now</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkEntDeluxeEpcsLogRxLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span>\r\n                </div>\r\n            </div>\r\n        </div> <!--End of enterprise deluxe with EPCS with ads Panel-->\r\n\r\n        <div class=\"subscription-card\" *ngIf=\"model.price.deluxeEpcs\">\r\n            <div class=\"subscription-card-header\">\r\n                <div>\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                    <img class=\"subscription-badge\" src=\"/images/registration/starPreRegdLogo.png\" />\r\n                </div>\r\n                <div class=\"subscription-header-title\">\r\n                    Veradigm ePrescribe&trade; Deluxe with EPCS\r\n                </div>\r\n            </div>\r\n            <div class=\"subscription-card-body\">\r\n                <div class=\"subscription-card-group\">\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescribe Medications Electronically\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prior Authorization (ePA)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Prescription Drug Monitoring Program (PDMP)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Electronic Prescription Controlled Substances (EPCS)\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        No Sponsored Messages\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Access Live Customer Support\r\n                    </div>\r\n                    <div class=\"subscription-card-row\">\r\n                        Easy to Use\r\n                    </div>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"subscription-entDeluxeEpcsPrice\" class=\"subscription-price\">Only ${{model.price.deluxeEpcs}} per month per prescriber</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    NOTE: There is a ${{model.price.epcsSetup}} per prescriber one-time setup fee for EPCS\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    <span id=\"linkEntDeluxeRegister\" (click)=\"redirectToCreateUser('DeluxeEPCSEpa2017')\" class=\"subscription-link\">Register Now</span>\r\n                </div>\r\n                <div class=\"subscription-card-row\">\r\n                    Already registered? <span id=\"linkEntDeluxeLogin\" (click)=\"redirectToLogin()\" class=\"subscription-link\">Sign in here</span>\r\n                </div>\r\n            </div>\r\n        </div> <!--End of enterprise deluxe with EPCS Panel-->\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/registration/subscription/subscription.component.ts":
/*!*********************************************************************!*\
  !*** ./src/app/registration/subscription/subscription.component.ts ***!
  \*********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.SubscriptionComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var subscription_model_1 = __webpack_require__(/*! ./subscription.model */ "./src/app/registration/subscription/subscription.model.ts");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var SubscriptionComponent = /** @class */ (function () {
    function SubscriptionComponent(window, router) {
        this.window = window;
        this.router = router;
        this.model = new subscription_model_1.subscription();
    }
    SubscriptionComponent.prototype.redirectToCreateUser = function (pricingStructure) {
        this.router.navigate(["register/createuser"], { state: { SelectedPricingStructure: pricingStructure } });
    };
    SubscriptionComponent.prototype.redirectToLogin = function () {
        var _a;
        window.location.href = (_a = this.appContext) === null || _a === void 0 ? void 0 : _a.login;
    };
    SubscriptionComponent.prototype.ngOnInit = function () {
        var _a, _b, _c, _d, _e, _f, _g, _h;
        this.appContext = (_a = this.window) === null || _a === void 0 ? void 0 : _a.appcontext;
        this.model.price = new subscription_model_1.PricingStructurePrice();
        this.model.price.basic = (_b = this.appContext) === null || _b === void 0 ? void 0 : _b.basicPrice;
        this.model.price.deluxe = (_c = this.appContext) === null || _c === void 0 ? void 0 : _c.deluxePrice;
        this.model.price.deluxeLogRx = (_d = this.appContext) === null || _d === void 0 ? void 0 : _d.deluxeLogRxPrice;
        this.model.price.deluxeEpcs = (_e = this.appContext) === null || _e === void 0 ? void 0 : _e.deluxeEpcsPrice;
        this.model.price.deluxeEpcsLogRx = (_f = this.appContext) === null || _f === void 0 ? void 0 : _f.deluxeEpcsLogRxPrice;
        this.model.price.epcsSetup = (_g = this.appContext) === null || _g === void 0 ? void 0 : _g.epcsSetupPrice;
        this.model.enterprisePricing = ((_h = this.appContext) === null || _h === void 0 ? void 0 : _h.enterprisePricing) && this.appContext.enterprisePricing.toLowerCase() == 'true';
    };
    SubscriptionComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-subscription',
            template: __webpack_require__(/*! ./subscription.component.html */ "./src/app/registration/subscription/subscription.component.html"),
            styles: [__webpack_require__(/*! ./subscription.component.css */ "./src/app/registration/subscription/subscription.component.css")]
        }),
        tslib_1.__param(0, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [Object, router_1.Router])
    ], SubscriptionComponent);
    return SubscriptionComponent;
}());
exports.SubscriptionComponent = SubscriptionComponent;


/***/ }),

/***/ "./src/app/registration/subscription/subscription.model.ts":
/*!*****************************************************************!*\
  !*** ./src/app/registration/subscription/subscription.model.ts ***!
  \*****************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.PricingStructureEnum = exports.PricingStructurePrice = exports.subscription = void 0;
var subscription = /** @class */ (function () {
    function subscription() {
    }
    return subscription;
}());
exports.subscription = subscription;
var PricingStructurePrice = /** @class */ (function () {
    function PricingStructurePrice() {
    }
    return PricingStructurePrice;
}());
exports.PricingStructurePrice = PricingStructurePrice;
var PricingStructureEnum;
(function (PricingStructureEnum) {
    PricingStructureEnum[PricingStructureEnum["CompulsoryBasic"] = 12] = "CompulsoryBasic";
    PricingStructureEnum[PricingStructureEnum["DeluxeEpa"] = 4] = "DeluxeEpa";
    PricingStructureEnum[PricingStructureEnum["DeluxeEpaLogRx"] = 11] = "DeluxeEpaLogRx";
    PricingStructureEnum[PricingStructureEnum["DeluxeEPCSEpa2017"] = 13] = "DeluxeEPCSEpa2017";
    PricingStructureEnum[PricingStructureEnum["DeluxeEPCSEpaLogRx2017"] = 14] = "DeluxeEPCSEpaLogRx2017";
})(PricingStructureEnum = exports.PricingStructureEnum || (exports.PricingStructureEnum = {}));


/***/ }),

/***/ "./src/app/registration/user-account-name/user-account-name.component.css":
/*!********************************************************************************!*\
  !*** ./src/app/registration/user-account-name/user-account-name.component.css ***!
  \********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJTUEFSZWdpc3RyYXRpb24vc3JjL2FwcC9yZWdpc3RyYXRpb24vdXNlci1hY2NvdW50LW5hbWUvdXNlci1hY2NvdW50LW5hbWUuY29tcG9uZW50LmNzcyJ9 */"

/***/ }),

/***/ "./src/app/registration/user-account-name/user-account-name.component.html":
/*!*********************************************************************************!*\
  !*** ./src/app/registration/user-account-name/user-account-name.component.html ***!
  \*********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #userAccForm=\"ngForm\" id=\"userAccForm\">\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label\">\r\n            <label><span class=\"highlight-star erx-form-star-position\">*</span>{{userNameLabel}}</label>\r\n        </span>\r\n\r\n        <span class=\"erx-form-input\">\r\n            <input type=\"text\" id=\"newShieldUserName\" name=\"newShieldUserName\" class=\"input-medium\" maxlength=\"30\" [pattern]=\"shieldUserNamePattern\"\r\n                   required [(ngModel)]=\"userAccountName.shieldUserName\" #newShieldUserName=\"ngModel\" (blur)=\"validateUserName();\" />\r\n            <span *ngIf=\"!isUserExists && newShieldUserName.valid\">\r\n                <img id=\"validUserImage\" src=\"/images/GreenCheck.png\" />\r\n            </span>\r\n\r\n            <span *ngIf=\"newShieldUserName.invalid && (newShieldUserName.dirty || newShieldUserName.touched)\">\r\n                <span id=\"userNameMandatoryError\" *ngIf=\"newShieldUserName.errors.required\" class=\"validation-color\">\r\n                    Username is required.\r\n                </span>\r\n                <span id=\"userNamePatternError\" *ngIf=\"newShieldUserName.errors.pattern\" class=\"validation-color\">\r\n                    Username can contain alphanumeric characters and the following symbols _ . , -\r\n                </span>\r\n                <span id=\"userNameinvalidError\" *ngIf=\"newShieldUserName.errors.uniqueShieldUserName && !newShieldUserName.errors.pattern\" class=\"validation-color\">\r\n                    {{userExistsErrorMessage}}\r\n                </span>\r\n            </span>\r\n        </span>\r\n    </div>\r\n</form>\r\n\r\n\r\n"

/***/ }),

/***/ "./src/app/registration/user-account-name/user-account-name.component.ts":
/*!*******************************************************************************!*\
  !*** ./src/app/registration/user-account-name/user-account-name.component.ts ***!
  \*******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserAccountNameComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var user_create_service_1 = __webpack_require__(/*! ../../service/user-create.service */ "./src/app/service/user-create.service.ts");
var user_account_name_model_1 = __webpack_require__(/*! ./user-account-name.model */ "./src/app/registration/user-account-name/user-account-name.model.ts");
var UserAccountNameComponent = /** @class */ (function () {
    function UserAccountNameComponent(userCreateService) {
        this.userCreateService = userCreateService;
        this.shieldUserNamePattern = "^([a-zA-Z0-9]+[-,_.]{0,30})*";
        this.isUserExists = true;
        this.userAccountName = new user_account_name_model_1.UserAccountName();
    }
    UserAccountNameComponent.prototype.ngOnInit = function () {
    };
    UserAccountNameComponent.prototype.validateUserName = function () {
        var _this = this;
        this.isUserExists = true;
        //Initialize variables
        var sldUserName = this.userAccountName.shieldUserName;
        if (sldUserName && !this.userAccForm.controls["newShieldUserName"].errors) {
            this.userCreateService.validateShieldUserName(sldUserName)
                .subscribe(function (userExists) {
                if (userExists) {
                    _this.isUserExists = true;
                    _this.userAccForm.controls["newShieldUserName"].setErrors({
                        "uniqueShieldUserName": "Invalid Username"
                    });
                }
                else {
                    _this.isUserExists = false;
                    _this.userAccForm.controls["newShieldUserName"].setErrors(null);
                }
            });
        }
    };
    UserAccountNameComponent.prototype.GetUserAccountNameDetails = function () {
        this.userAccForm.controls.newShieldUserName.markAsDirty();
        this.userAccountName.isValid = this.userAccForm.valid;
        return this.userAccountName;
    };
    tslib_1.__decorate([
        core_1.ViewChild('userAccForm'),
        tslib_1.__metadata("design:type", Object)
    ], UserAccountNameComponent.prototype, "userAccForm", void 0);
    tslib_1.__decorate([
        core_1.Input(),
        tslib_1.__metadata("design:type", String)
    ], UserAccountNameComponent.prototype, "userExistsErrorMessage", void 0);
    tslib_1.__decorate([
        core_1.Input(),
        tslib_1.__metadata("design:type", String)
    ], UserAccountNameComponent.prototype, "userNameLabel", void 0);
    UserAccountNameComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-user-account',
            template: __webpack_require__(/*! ./user-account-name.component.html */ "./src/app/registration/user-account-name/user-account-name.component.html"),
            styles: [__webpack_require__(/*! ./user-account-name.component.css */ "./src/app/registration/user-account-name/user-account-name.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [user_create_service_1.UserCreateService])
    ], UserAccountNameComponent);
    return UserAccountNameComponent;
}());
exports.UserAccountNameComponent = UserAccountNameComponent;


/***/ }),

/***/ "./src/app/registration/user-account-name/user-account-name.model.ts":
/*!***************************************************************************!*\
  !*** ./src/app/registration/user-account-name/user-account-name.model.ts ***!
  \***************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserAccountName = void 0;
var UserAccountName = /** @class */ (function () {
    function UserAccountName() {
    }
    return UserAccountName;
}());
exports.UserAccountName = UserAccountName;


/***/ }),

/***/ "./src/app/registration/user-creation/user-creation.component.css":
/*!************************************************************************!*\
  !*** ./src/app/registration/user-creation/user-creation.component.css ***!
  \************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".user-creation-password-rules {\r\n    height: 80px;\r\n}\r\n\r\n    .user-creation-password-rules > ul {\r\n        position: relative;\r\n        top: 3px;\r\n    }\r\n\r\n    .user-creation-password-rules > ul > li {\r\n            position: relative;\r\n            left: 2px;\r\n            list-style-position: inside;\r\n        }\r\n\r\n    .user-creation-dea-schedule {\r\n    margin-left: -2px;\r\n    margin-right: 8px;\r\n    padding-bottom: 11px;\r\n    margin-bottom: 7px;\r\n    position: relative;\r\n    top: -7px;\r\n}\r\n\r\n    .user-creation-captcha-input-group {\r\n    display: inline-block;\r\n    padding-left: 15px;\r\n    position: relative;\r\n    top: -9px;\r\n}\r\n\r\n    .user-creation-captcha-label {\r\n    display: inline-block;\r\n    padding-left: 25px;\r\n}\r\n\r\n    .user-creation-captcha-input {\r\n    display: block;\r\n    padding-top: 5px;\r\n    padding-left: 10px;\r\n}\r\n\r\n    .user-creation-security-questions {\r\n    padding-left: 1px;\r\n    padding-right: 365px;\r\n}\r\n\r\n    .user-creation-security-questions-first-row {\r\n    margin-top: 3px;\r\n}\r\n\r\n    .user-creation-already-completed {\r\n    margin: 10px 0px 25px 0px;\r\n}\r\n\r\n    .user-creation-already-completed > span {\r\n        font-weight: 900;\r\n        font-size: 14px;\r\n        padding-right: 10px;\r\n    }\r\n\r\n    .user-creation-already-completed > a {\r\n        color: #5e4da5;\r\n        text-decoration: none;\r\n    }\r\n\r\n    .user-creation-information {\r\n    margin: 20px 0px;\r\n    padding: 10px;\r\n    background-color: #D6ECEE;\r\n}\r\n\r\n    .user-creation-information > img {\r\n        position: relative;\r\n        top: 3px;\r\n    }\r\n\r\n    .user-creation-information > span {\r\n        font-weight: 900;\r\n    }\r\n\r\n    .user-creation-form-label-spacing {\r\n        width: 230px;\r\n    }\r\n\r\n    .user-creation-user-name-control-spacing {\r\n    padding-left: 133px;\r\n}\r\n\r\n    .user-creation-user-password-control-spacing {\r\n    padding-left: 74px;\r\n}\r\n\r\n    .user-creation-security-control-spacing {\r\n    padding-left: 220px;\r\n}\r\n\r\n    .user-creation-existing-user-control-spacing {\r\n    padding-left: 133px;\r\n}\r\n\r\n    .user-creation-captcha-control-spacing {\r\n    padding-left: 150px;\r\n}\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi91c2VyLWNyZWF0aW9uL3VzZXItY3JlYXRpb24uY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLFlBQVk7QUFDaEI7O0lBRUk7UUFDSSxrQkFBa0I7UUFDbEIsUUFBUTtJQUNaOztJQUVJO1lBQ0ksa0JBQWtCO1lBQ2xCLFNBQVM7WUFDVCwyQkFBMkI7UUFDL0I7O0lBRVI7SUFDSSxpQkFBaUI7SUFDakIsaUJBQWlCO0lBQ2pCLG9CQUFvQjtJQUNwQixrQkFBa0I7SUFDbEIsa0JBQWtCO0lBQ2xCLFNBQVM7QUFDYjs7SUFFQTtJQUNJLHFCQUFxQjtJQUNyQixrQkFBa0I7SUFDbEIsa0JBQWtCO0lBQ2xCLFNBQVM7QUFDYjs7SUFFQTtJQUNJLHFCQUFxQjtJQUNyQixrQkFBa0I7QUFDdEI7O0lBRUE7SUFDSSxjQUFjO0lBQ2QsZ0JBQWdCO0lBQ2hCLGtCQUFrQjtBQUN0Qjs7SUFFQTtJQUNJLGlCQUFpQjtJQUNqQixvQkFBb0I7QUFDeEI7O0lBRUE7SUFDSSxlQUFlO0FBQ25COztJQUVBO0lBQ0kseUJBQXlCO0FBQzdCOztJQUVJO1FBQ0ksZ0JBQWdCO1FBQ2hCLGVBQWU7UUFDZixtQkFBbUI7SUFDdkI7O0lBRUE7UUFDSSxjQUFjO1FBQ2QscUJBQXFCO0lBQ3pCOztJQUVKO0lBQ0ksZ0JBQWdCO0lBQ2hCLGFBQWE7SUFDYix5QkFBeUI7QUFDN0I7O0lBRUk7UUFDSSxrQkFBa0I7UUFDbEIsUUFBUTtJQUNaOztJQUVBO1FBQ0ksZ0JBQWdCO0lBQ3BCOztJQUdBO1FBQ0ksWUFBWTtJQUNoQjs7SUFFSjtJQUNJLG1CQUFtQjtBQUN2Qjs7SUFDQTtJQUNJLGtCQUFrQjtBQUN0Qjs7SUFDQTtJQUNJLG1CQUFtQjtBQUN2Qjs7SUFDQTtJQUNJLG1CQUFtQjtBQUN2Qjs7SUFFQTtJQUNJLG1CQUFtQjtBQUN2QiIsImZpbGUiOiJTUEFSZWdpc3RyYXRpb24vc3JjL2FwcC9yZWdpc3RyYXRpb24vdXNlci1jcmVhdGlvbi91c2VyLWNyZWF0aW9uLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyIudXNlci1jcmVhdGlvbi1wYXNzd29yZC1ydWxlcyB7XHJcbiAgICBoZWlnaHQ6IDgwcHg7XHJcbn1cclxuXHJcbiAgICAudXNlci1jcmVhdGlvbi1wYXNzd29yZC1ydWxlcyA+IHVsIHtcclxuICAgICAgICBwb3NpdGlvbjogcmVsYXRpdmU7XHJcbiAgICAgICAgdG9wOiAzcHg7XHJcbiAgICB9XHJcblxyXG4gICAgICAgIC51c2VyLWNyZWF0aW9uLXBhc3N3b3JkLXJ1bGVzID4gdWwgPiBsaSB7XHJcbiAgICAgICAgICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgICAgICAgICAgbGVmdDogMnB4O1xyXG4gICAgICAgICAgICBsaXN0LXN0eWxlLXBvc2l0aW9uOiBpbnNpZGU7XHJcbiAgICAgICAgfVxyXG5cclxuLnVzZXItY3JlYXRpb24tZGVhLXNjaGVkdWxlIHtcclxuICAgIG1hcmdpbi1sZWZ0OiAtMnB4O1xyXG4gICAgbWFyZ2luLXJpZ2h0OiA4cHg7XHJcbiAgICBwYWRkaW5nLWJvdHRvbTogMTFweDtcclxuICAgIG1hcmdpbi1ib3R0b206IDdweDtcclxuICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgIHRvcDogLTdweDtcclxufVxyXG5cclxuLnVzZXItY3JlYXRpb24tY2FwdGNoYS1pbnB1dC1ncm91cCB7XHJcbiAgICBkaXNwbGF5OiBpbmxpbmUtYmxvY2s7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDE1cHg7XHJcbiAgICBwb3NpdGlvbjogcmVsYXRpdmU7XHJcbiAgICB0b3A6IC05cHg7XHJcbn1cclxuXHJcbi51c2VyLWNyZWF0aW9uLWNhcHRjaGEtbGFiZWwge1xyXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xyXG4gICAgcGFkZGluZy1sZWZ0OiAyNXB4O1xyXG59XHJcblxyXG4udXNlci1jcmVhdGlvbi1jYXB0Y2hhLWlucHV0IHtcclxuICAgIGRpc3BsYXk6IGJsb2NrO1xyXG4gICAgcGFkZGluZy10b3A6IDVweDtcclxuICAgIHBhZGRpbmctbGVmdDogMTBweDtcclxufVxyXG5cclxuLnVzZXItY3JlYXRpb24tc2VjdXJpdHktcXVlc3Rpb25zIHtcclxuICAgIHBhZGRpbmctbGVmdDogMXB4O1xyXG4gICAgcGFkZGluZy1yaWdodDogMzY1cHg7XHJcbn1cclxuXHJcbi51c2VyLWNyZWF0aW9uLXNlY3VyaXR5LXF1ZXN0aW9ucy1maXJzdC1yb3cge1xyXG4gICAgbWFyZ2luLXRvcDogM3B4O1xyXG59XHJcblxyXG4udXNlci1jcmVhdGlvbi1hbHJlYWR5LWNvbXBsZXRlZCB7XHJcbiAgICBtYXJnaW46IDEwcHggMHB4IDI1cHggMHB4O1xyXG59XHJcblxyXG4gICAgLnVzZXItY3JlYXRpb24tYWxyZWFkeS1jb21wbGV0ZWQgPiBzcGFuIHtcclxuICAgICAgICBmb250LXdlaWdodDogOTAwO1xyXG4gICAgICAgIGZvbnQtc2l6ZTogMTRweDtcclxuICAgICAgICBwYWRkaW5nLXJpZ2h0OiAxMHB4O1xyXG4gICAgfVxyXG5cclxuICAgIC51c2VyLWNyZWF0aW9uLWFscmVhZHktY29tcGxldGVkID4gYSB7XHJcbiAgICAgICAgY29sb3I6ICM1ZTRkYTU7XHJcbiAgICAgICAgdGV4dC1kZWNvcmF0aW9uOiBub25lO1xyXG4gICAgfVxyXG5cclxuLnVzZXItY3JlYXRpb24taW5mb3JtYXRpb24ge1xyXG4gICAgbWFyZ2luOiAyMHB4IDBweDtcclxuICAgIHBhZGRpbmc6IDEwcHg7XHJcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjRDZFQ0VFO1xyXG59XHJcblxyXG4gICAgLnVzZXItY3JlYXRpb24taW5mb3JtYXRpb24gPiBpbWcge1xyXG4gICAgICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgICAgICB0b3A6IDNweDtcclxuICAgIH1cclxuXHJcbiAgICAudXNlci1jcmVhdGlvbi1pbmZvcm1hdGlvbiA+IHNwYW4ge1xyXG4gICAgICAgIGZvbnQtd2VpZ2h0OiA5MDA7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIC51c2VyLWNyZWF0aW9uLWZvcm0tbGFiZWwtc3BhY2luZyB7XHJcbiAgICAgICAgd2lkdGg6IDIzMHB4O1xyXG4gICAgfVxyXG5cclxuLnVzZXItY3JlYXRpb24tdXNlci1uYW1lLWNvbnRyb2wtc3BhY2luZyB7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDEzM3B4O1xyXG59XHJcbi51c2VyLWNyZWF0aW9uLXVzZXItcGFzc3dvcmQtY29udHJvbC1zcGFjaW5nIHtcclxuICAgIHBhZGRpbmctbGVmdDogNzRweDtcclxufVxyXG4udXNlci1jcmVhdGlvbi1zZWN1cml0eS1jb250cm9sLXNwYWNpbmcge1xyXG4gICAgcGFkZGluZy1sZWZ0OiAyMjBweDtcclxufVxyXG4udXNlci1jcmVhdGlvbi1leGlzdGluZy11c2VyLWNvbnRyb2wtc3BhY2luZyB7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDEzM3B4O1xyXG59XHJcblxyXG4udXNlci1jcmVhdGlvbi1jYXB0Y2hhLWNvbnRyb2wtc3BhY2luZyB7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDE1MHB4O1xyXG59Il19 */"

/***/ }),

/***/ "./src/app/registration/user-creation/user-creation.component.html":
/*!*************************************************************************!*\
  !*** ./src/app/registration/user-creation/user-creation.component.html ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #userDetailForm=\"ngForm\" (ngSubmit)=\"onSubmit(userDetailForm)\">\r\n    <div id=\"user-creation-title\" class=\"title-bar title-bar-heading branded-background-color branded-font-color\">\r\n        Create New Account\r\n    </div>\r\n    <div class=\"erx-form-content-offset font-color-dimmed-black\">\r\n        <div class=\"user-creation-already-completed\">\r\n            <span>Already completed this step?</span> <a [href]=\"mainAppLink\">Click here</a> and then login to continue registration.\r\n        </div>\r\n        <div>\r\n            Welcome to ePrescribe. The first step to prescribing is to create a user account and go through our identity proofing process. Please enter your information below.\r\n        </div>\r\n        <div class=\"user-creation-information\">\r\n            <img src=\"/images/info-global-16-x-16.png\" />\r\n            Please have your <span>Driver's License/State Issued ID/Passport and Social Security Number</span> available to complete the registration credentialing process.\r\n        </div>\r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            Personal Information\r\n        </div>\r\n        <div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>Title</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"title\" name=\"title\" class=\"input-x-small\" autofocus\r\n                           maxlength=\"10\" #title=\"ngModel\"\r\n                           [(ngModel)]=\"userDetail.title\">\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>First Name</label>\r\n                </span>\r\n\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"firstname\" name=\"firstname\" class=\"input-large\" type=\"text\"\r\n                           required maxlength=\"35\" [pattern]=\"namePattern\"\r\n                           [(ngModel)]=\"userDetail.firstname\" #firstname=\"ngModel\">\r\n                    <span *ngIf=\"firstname.invalid && (firstname.dirty || firstname.touched)\">\r\n                        <span id=\"firstNameRequiredError\" *ngIf=\"firstname.errors.required\" class=\"validation-color\">\r\n                            First Name is required.\r\n                        </span>\r\n                        <span id=\"firstnamePatternError\" *ngIf=\"firstname.errors.pattern\" class=\"validation-color\">\r\n                            Invalid first name. First name has to be characters, maximum of 35.\r\n                        </span>\r\n                    </span>\r\n\r\n                </span>\r\n            </div>\r\n\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>Middle Name</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"middlename\" name=\"middleName\" class=\"input-large\"\r\n                           maxlength=\"35\" [pattern]=\"namePattern\"\r\n                           [(ngModel)]=\"userDetail.middleName\" #middleName=\"ngModel\" />\r\n                </span>\r\n                <span *ngIf=\"middleName.invalid && (middleName.dirty || middleName.touched)\">\r\n                    <span id=\"middleNamePatternError\" *ngIf=\"middleName.errors.pattern\" class=\"validation-color\">\r\n                        Invalid middle name. Middle name has to be characters, maximum of 35.\r\n                    </span>\r\n                </span>\r\n            </div>\r\n\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>Last Name</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input type=\"text\" id=\"lastName\" name=\"lastName\" class=\"input-large\"\r\n                           [pattern]=\"namePattern\"\r\n                           required maxlength=\"35\" [(ngModel)]=\"userDetail.lastName\" #lastName=\"ngModel\" />\r\n                    <span *ngIf=\"lastName.invalid && (lastName.dirty || lastName.touched)\">\r\n                        <span id=\"lastNameRequiredError\" *ngIf=\"lastName.errors.required\" class=\"validation-color\">\r\n                            Last Name is required.\r\n                        </span>\r\n                        <span id=\"firstnamePatternError\" *ngIf=\"lastName.errors.pattern\" class=\"validation-color\">\r\n                            Invalid first name. Last name has to be characters, maximum of 35.\r\n                        </span>\r\n                    </span>\r\n\r\n                </span>\r\n            </div>\r\n\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>Suffix</label>\r\n                </span>\r\n\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"suffix\" name=\"suffix\" class=\"input-x-small\"\r\n                           maxlength=\"6\" #suffix=\"ngModel\" [pattern]=\"suffixPattern\"\r\n                           [(ngModel)]=\"userDetail.suffix\">\r\n                    <span *ngIf=\"suffix.invalid && (suffix.dirty || suffix.touched)\">\r\n                        <span id=\"suffixPatternError\" *ngIf=\"suffix.errors.pattern\" class=\"validation-color\">\r\n                            Invalid suffix. Kindly enter a valid suffix (letters only).\r\n                        </span>\r\n                    </span>\r\n\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>Personal Email</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input type=\"email\" id=\"personalEmail\" name=\"personalEmail\" class=\"input-large\"\r\n                           required maxlength=\"70\"\r\n                           [(ngModel)]=\"userDetail.personalEmail\" #personalEmail=\"ngModel\" email />\r\n                    <span *ngIf=\"personalEmail.invalid && (personalEmail.dirty || personalEmail.touched)\">\r\n                        <span id=\"emailRequiredError\" *ngIf=\"personalEmail.errors.required\" class=\"validation-color\">\r\n                            Personal Email is required.\r\n                        </span>\r\n                        <span id=\"emailPatternError\" *ngIf=\"personalEmail.errors.email && !personalEmail.errors.required \" class=\"validation-color\">\r\n                            Enter valid email address.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>Contact Phone Number</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input type=\"text\"\r\n                           id=\"txtContactPhoneNumber\"\r\n                           name=\"contactPhoneNumber\"\r\n                           class=\"input-large\"\r\n                           required\r\n                           [pattern]=\"phoneFaxPattern\"\r\n                           maxlength=\"14\"\r\n                           [(ngModel)]=\"userDetail.contactPhoneNumber\"\r\n                           #contactPhoneNumber=\"ngModel\" />\r\n\r\n                    <span *ngIf=\"contactPhoneNumber.touched || (contactPhoneNumber.invalid && contactPhoneNumber.dirty)\">\r\n                        <span id=\"contactPhoneNumberError\" *ngIf=\"contactPhoneNumber.errors\" class=\"validation-color\">\r\n                            Enter a valid contact phone number.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>Home Address</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"txtAddress1\" class=\"input-large\"\r\n                           type=\"text\"\r\n                           name=\"HomeAddress\"\r\n                           maxlength=\"35\"\r\n                           required\r\n                           [(ngModel)]=\"userDetail.HomeAddress\"\r\n                           #HomeAddress=\"ngModel\" />\r\n\r\n                    <span *ngIf=\"HomeAddress.invalid && (HomeAddress.dirty || HomeAddress.touched)\">\r\n                        <span id=\"addressRequiredError\" *ngIf=\"HomeAddress.errors.required\"\r\n                              class=\"validation-color\">\r\n                            Home Address is required.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>Home Address 2</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"txtAddress2\" class=\"input-large\"\r\n                           type=\"text\"\r\n                           name=\"HomeAddress2\"\r\n                           maxlength=\"35\"\r\n                           [(ngModel)]=\"userDetail.HomeAddress2\" />\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>City</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"txtCity\" class=\"input-large\"\r\n                           type=\"text\"\r\n                           name=\"city\"\r\n                           maxlength=\"20\"\r\n                           required\r\n                           [pattern]=\"cityPattern\"\r\n                           [(ngModel)]=\"userDetail.city\"\r\n                           #city=\"ngModel\" />\r\n\r\n                    <span *ngIf=\"city.invalid && (city.dirty || city.touched)\">\r\n                        <span *ngIf=\"city.errors.required\"\r\n                              id=\"cityMandatoryError\"\r\n                              class=\"validation-color\">\r\n                            City is required.\r\n                        </span>\r\n                        <span id=\"CityPatternError\" *ngIf=\"city.errors.pattern\" class=\"validation-color\">\r\n                            Enter a valid city.\r\n                        </span>\r\n                    </span>\r\n\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>State</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <select id=\"ddlState\" class=\"input-medium\"\r\n                            type=\"text\"\r\n                            name=\"State\"\r\n                            required\r\n                            [(ngModel)]=\"userDetail.state\"\r\n                            #state=\"ngModel\">\r\n                        <option [value]=\"undefined\" selected disabled>-- Select a State --</option>\"\r\n                        <option *ngFor=\"let item of states\" [value]=\"item.state\">\r\n                            {{item.description}}\r\n                        </option>\r\n                    </select>\r\n                    <span *ngIf=\"state.invalid && (state.dirty || state.touched)\">\r\n                        <span *ngIf=\"state.errors.required\"\r\n                              id=\"stateMandatoryError\"\r\n                              class=\"validation-color\">\r\n                            State is required.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>ZIP Code</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"txtZip\" class=\"input-medium\"\r\n                           type=\"text\"\r\n                           name=\"zipCode\"\r\n                           maxlength=\"9\"\r\n                           required\r\n                           [pattern]=\"zipCodePattern\"\r\n                           [(ngModel)]=\"userDetail.zipCode\"\r\n                           #zipCode=\"ngModel\" />\r\n\r\n                    <span *ngIf=\"zipCode.invalid && (zipCode.dirty || zipCode.touched)\">\r\n                        <span *ngIf=\"zipCode.errors.required\"\r\n                              id=\"zipMandatoryError\"\r\n                              class=\"validation-color\">\r\n                            ZIP Code is required.\r\n                        </span>\r\n                        <span id=\"zipCodePatternError\" *ngIf=\"zipCode.errors.pattern\" class=\"validation-color\">\r\n                            Enter a valid 5 or 9 digit ZIP code\r\n                        </span>\r\n\r\n                    </span>\r\n                </span>\r\n            </div>\r\n        </div>\r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            Provider Information\r\n        </div>\r\n        <div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>NPI</label>\r\n                </span>\r\n\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"npi\" name=\"npi\" class=\"input-medium\"\r\n                           required maxlength=\"10\"\r\n                           [(ngModel)]=\"userDetail.npi\" #npi=\"ngModel\" (change)=\"onNpiChanged()\">\r\n\r\n                    <span *ngIf=\"npi.invalid && (npi.dirty || npi.touched)\">\r\n                        <span id=\"NPIMandatoryError\" *ngIf=\"npi.errors.required\" class=\"validation-color\">\r\n                            NPI is required.\r\n                        </span>\r\n                    </span>\r\n                    <span id=\"NPIinvalidError\" *ngIf=\"!isValidNPI\" class=\"validation-color\">\r\n                        Provide valid NPI number.\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>Specialty 1</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n\r\n                    <select id=\"ddlSpecialty1\"\r\n                            type=\"text\"\r\n                            name=\"speciality1\"\r\n                            class=\"input-large\"\r\n                            required\r\n                            [(ngModel)]=\"userDetail.speciality1\"\r\n                            #speciality1=\"ngModel\">\r\n                        <option [value]=\"undefined\" selected disabled>-- Select a Specialty --</option>\r\n                        <option *ngFor=\"let item of SpecialityList\" [value]=\"item.Specialty_CD\">\r\n                            {{item.Specialty}}\r\n                        </option>\r\n                    </select>\r\n                    <span *ngIf=\"speciality1.invalid && (speciality1.dirty || speciality1.touched)\">\r\n                        <span *ngIf=\"speciality1.errors.required\"\r\n                              id=\"speciality1MandatoryError\"\r\n                              class=\"validation-color\">\r\n                            Select a valid Specialty 1.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>Specialty 2</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <select id=\"ddlSpecialty2\"\r\n                            type=\"text\"\r\n                            name=\"speciality2\"\r\n                            class=\"input-large\"\r\n                            [(ngModel)]=\"userDetail.speciality2\"\r\n                            #speciality2=\"ngModel\">\r\n                        <option [value]=\"undefined\" selected>-- Select a Specialty --</option>\r\n                        <option *ngFor=\"let item of SpecialityList\" [value]=\"item.Specialty_CD\">\r\n                            {{item.Specialty}}\r\n                        </option>\r\n                    </select>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>DEA Number</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input type=\"text\" id=\"deaNumber\" name=\"deaNumber\" class=\"input-medium\"\r\n                           maxlength=\"35\" [(ngModel)]=\"userDetail.deaNumber\" #deaNumber=\"ngModel\"\r\n                           (change)=\"setDeaValidation()\" [pattern]=\"noLeadingSpacePattern\" />\r\n                    <span id=\"DEAMandatoryError\" \r\n                          *ngIf=\"isDEARequired && (deaExpirationDate.dirty || deaExpirationDate.touched)\" \r\n                          class=\"validation-color\">\r\n                        DEA Number is required.\r\n                    </span>\r\n                    <span *ngIf=\"(deaNumber.invalid && (deaNumber.dirty || deaNumber.touched)) \r\n                          || showInvalidDeaError\">\r\n                        <span id=\"DEAinvalidError\" \r\n                              *ngIf=\"showInvalidDeaError \r\n                              || (!isDEARequired && deaNumber.errors.pattern)\" class=\"validation-color\">\r\n                            Provide valid DEA Number.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>DEA Number Exp. Date</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input type=\"text\" id=\"deaExpirationDate\" name=\"deaExpirationDate\" class=\"input-medium\" [pattern]=\"datePattern\"\r\n                           placeholder=\"MM-DD-YYYY\" [(ngModel)]=\"userDetail.deaExpirationDate\" #deaExpirationDate=\"ngModel\" (change)=\"setDeaValidation()\">\r\n                    <span *ngIf=\"deaExpirationDate.invalid && (deaExpirationDate.dirty || deaExpirationDate.touched)\">\r\n                        <span id=\"DEAExpiryDatePatternError\" \r\n                              *ngIf=\"deaExpirationDate.errors.pattern && !isDEAExpDateRequired\" class=\"validation-color\">\r\n                            Enter a valid DEA Number Exp. Date(MM-DD-YYYY).\r\n                        </span>\r\n                    </span>\r\n                    <span id=\"DEAExpiryDateRequiredError\" \r\n                          *ngIf=\"isDEAExpDateRequired && (deaExpirationDate.dirty || deaExpirationDate.touched)\" class=\"validation-color\">\r\n                        DEA Number Exp. Date is required.\r\n                    </span>\r\n\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label>DEA Schedule</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n\r\n                    <input type=\"checkbox\" [(ngModel)]=\"userDetail.deaScheduleII\" id=\"deaSchedule2\"\r\n                           name=\"deaSchedule2\" #deaScheduleII=\"ngModel\" (change)=\"setDeaValidation()\">\r\n                    <span class=\"user-creation-dea-schedule\">\r\n                        II\r\n                    </span>\r\n                    <input type=\"checkbox\" [(ngModel)]=\"userDetail.deaScheduleIII\" id=\"deaSchedule3\"\r\n                           name=\"deaSchedule3\" #deaScheduleIII=\"ngModel\" (change)=\"setDeaValidation()\">\r\n                    <span class=\"user-creation-dea-schedule\">\r\n                        III\r\n                    </span>\r\n                    <input type=\"checkbox\" [(ngModel)]=\"userDetail.deaScheduleIV\" id=\"deaSchedule4\"\r\n                           name=\"deaSchedule4\" #deaScheduleIV=\"ngModel\" (change)=\"setDeaValidation()\">\r\n                    <span class=\"user-creation-dea-schedule\">\r\n                        IV\r\n                    </span>\r\n                    <input type=\"checkbox\" [(ngModel)]=\"userDetail.deaScheduleV\" id=\"deaSchedule5\"\r\n                           name=\"deaSchedule5\" #deaScheduleV=\"ngModel\" (change)=\"setDeaValidation()\">\r\n                    <span class=\"user-creation-dea-schedule\">\r\n                        V\r\n                    </span>\r\n\r\n                    <span id=\"ControlSubstanceSelectionError\" \r\n                          *ngIf=\"isDEAScheduleRequired &&  \r\n                           (deaScheduleII.dirty || deaScheduleII.touched\r\n                           || deaScheduleIII.dirty || deaScheduleIII.touched\r\n                           || deaScheduleIV.dirty || deaScheduleIV.touched\r\n                          || deaScheduleV.dirty || deaScheduleV.touched)\" class=\"validation-color\">\r\n                        Select atleast one DEA Schedule.\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>State License Number</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <input id=\"stateLicenseNumber\" name=\"stateLicenseNumber\" class=\"input-medium\"\r\n                           required maxlength=\"35\" [pattern]=\"noLeadingSpacePattern\"\r\n                           [(ngModel)]=\"userDetail.stateLicenseNumber\" #stateLicenseNumber=\"ngModel\">\r\n\r\n                    <span *ngIf=\"stateLicenseNumber.invalid && (stateLicenseNumber.dirty || stateLicenseNumber.touched)\">\r\n                        <span id=\"StateLicenseMandatoryError\" *ngIf=\"stateLicenseNumber.errors.required\" class=\"validation-color\">\r\n                            State License Number is required.\r\n                        </span>\r\n                        <span id=\"StateLicensePatternError\" *ngIf=\"stateLicenseNumber.errors.pattern\" class=\"validation-color\">\r\n                            Enter a valid State License Number.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>State License Issuing State</label>\r\n                </span>\r\n                <span class=\"erx-form-input\">\r\n                    <select id=\"ddlStateLicenseIssueState\"\r\n                            type=\"text\"\r\n                            name=\"stateLicenseIssueState\"\r\n                            class=\"input-medium\"\r\n                            required\r\n                            [(ngModel)]=\"userDetail.stateLicenseIssueState\"\r\n                            #stateLicenseIssueState=\"ngModel\">\r\n                        <option [value]=\"undefined\" selected disabled>-- Select a State --</option>\r\n                        <option *ngFor=\"let item of states\" [value]=\"item.state\">\r\n                            {{item.description}}\r\n                        </option>\r\n                    </select>\r\n                    <span *ngIf=\"stateLicenseIssueState.invalid && (stateLicenseIssueState.dirty || stateLicenseIssueState.touched)\">\r\n                        <span *ngIf=\"stateLicenseIssueState.errors.required\"\r\n                              id=\"stateLicenseIssuingStateMandatoryError\"\r\n                              class=\"validation-color\">\r\n                            State License Issuing State is required.\r\n                        </span>\r\n                    </span>\r\n                </span>\r\n            </div>\r\n            <div class=\"erx-form-row\">\r\n                <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n                    <label><span class=\"highlight-star erx-form-star-position\">*</span>State License Exp. Date</label>\r\n                </span>\r\n\r\n                <span class=\"erx-form-input\">\r\n                    <input type=\"text\" id=\"stateLicenseExpirationDate\" name=\"stateLicenseExpirationDate\" class=\"input-medium\"\r\n                           required [pattern]=\"datePattern\" placeholder=\"MM-DD-YYYY\"\r\n                           [(ngModel)]=\"userDetail.stateLicenseExpirationDate\" #stateLicenseExpirationDate=\"ngModel\">\r\n\r\n                    <span *ngIf=\"stateLicenseExpirationDate.invalid && (stateLicenseExpirationDate.dirty || stateLicenseExpirationDate.touched)\">\r\n                        <span *ngIf=\"stateLicenseExpirationDate.errors.required\" id=\"stateLicenseExpDateMandatoryError\" class=\"validation-color\">\r\n                            State License Exp. Date is required.\r\n                        </span>\r\n                        <span id=\"stateLicenseExpirationDatePatternError\" *ngIf=\"stateLicenseExpirationDate.errors.pattern\" class=\"validation-color\">\r\n                            Enter a valid State License Exp. Date(MM-DD-YYYY).\r\n                        </span>\r\n                    </span>\r\n\r\n                </span>\r\n            </div>\r\n        </div>\r\n        <!--Cut started here-->\r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            User Credentials & Security\r\n        </div>\r\n        <div>\r\n            <input type=\"radio\" [value]=\"false\" name=\"isLinkExistingShieldUser\" checked [(ngModel)]=\"isLinkExistingShieldUser\"> Create a New ePrescribe Account\r\n            <input type=\"radio\" [value]=\"true\" name=\"isLinkExistingShieldUser\" [(ngModel)]=\"isLinkExistingShieldUser\"> Link to an existing Account\r\n        </div>\r\n        <div [style.display]=\"!isLinkExistingShieldUser ? 'block' : 'none'\">\r\n            <div class=\"user-creation-user-name-control-spacing\">\r\n                <app-user-account #userNameChild [userExistsErrorMessage]=\"userExistsErrorMessage\" [userNameLabel]=\"userNameLabel\"></app-user-account>\r\n            </div>\r\n            <div class=\"user-creation-user-password-control-spacing\">\r\n                <app-user-password #pwdChild></app-user-password>\r\n            </div>\r\n            <div class=\"user-creation-security-control-spacing\">\r\n                <app-user-security-questions #securityQuestion></app-user-security-questions>\r\n            </div>\r\n        </div>\r\n        <div [style.display]=\"isLinkExistingShieldUser ? 'block' : 'none'\">\r\n            <div class=\"user-creation-existing-user-control-spacing\">\r\n                <user-existing-account #userExisting></user-existing-account>\r\n            </div> \r\n        </div>\r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            Acknowledgement\r\n        </div>\r\n        <div class=\"erx-form-row user-creation-captcha-control-spacing\">\r\n            <show-captcha #captchaControl ></show-captcha>         \r\n        </div>\r\n        <div class=\"erx-form-row\">\r\n            The next step is to verify you are who you say you are through an online identity management process provided by ID.me. ID.me will ask you to provide information about yourself to verify your identity including  driver's license or passport information or state issued ID and social security number. Please review the ID.me Terms of Service listed below and then click on Submit.\r\n        </div>\r\n        <div class=\"erx-form-row\">\r\n            <input type=\"checkbox\" [(ngModel)]=\"userDetail.idMeTermsAccepted\" id=\"chkAcceptedIDmeTerms\"\r\n                   name=\"idMeTermsAccepted\" #idMeTermsAccepted=\"ngModel\" required> I have reviewed my registration entries and\r\n            <a [href]=\"idmeTerms\" target=\"_blank\" id=\"lnkIDmeTerms\">ID.me Terms of Service</a>.\r\n            <span *ngIf=\"idMeTermsAccepted.invalid && (idMeTermsAccepted.dirty || idMeTermsAccepted.touched)\" class=\"validation-color\">\r\n                Kindly review the ID.me Terms of Service and accept it.\r\n            </span>\r\n        </div>\r\n        <div class=\"erx-form-row\">\r\n            <button id=\"btnSubmit\" type=\"submit\" class=\"button-style\">Submit</button>\r\n            <span *ngIf=\"isValidationFailed\" class=\"validation-color\">\r\n                Kindly review the form, there seems to be an incomplete entry.\r\n            </span>\r\n        </div>\r\n    </div>\r\n</form>\r\n<div class=\"modal-backdrop fade in\" [style.display]=\"showModal ? 'block' : 'none'\"></div>\r\n<div *ngIf=\"showModal\">\r\n    <max-retry-failure-popup [shieldUserName]=\"userDetail.shieldUserName\"></max-retry-failure-popup>\r\n</div>\r\n\r\n"

/***/ }),

/***/ "./src/app/registration/user-creation/user-creation.component.ts":
/*!***********************************************************************!*\
  !*** ./src/app/registration/user-creation/user-creation.component.ts ***!
  \***********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserCreationComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var user_creation_model_1 = __webpack_require__(/*! ../user-creation/user-creation.model */ "./src/app/registration/user-creation/user-creation.model.ts");
var platform_browser_1 = __webpack_require__(/*! @angular/platform-browser */ "../node_modules/@angular/platform-browser/fesm5/platform-browser.js");
var user_create_service_1 = __webpack_require__(/*! ../../service/user-create.service */ "./src/app/service/user-create.service.ts");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
__webpack_require__(/*! ../../Utils/String.Extension */ "./src/app/Utils/String.Extension.ts");
var show_captcha_component_1 = __webpack_require__(/*! ../../registration/show-captcha/show-captcha.component */ "./src/app/registration/show-captcha/show-captcha.component.ts");
var user_password_component_1 = __webpack_require__(/*! ../user-password/user-password.component */ "./src/app/registration/user-password/user-password.component.ts");
var user_account_name_component_1 = __webpack_require__(/*! ../user-account-name/user-account-name.component */ "./src/app/registration/user-account-name/user-account-name.component.ts");
var user_security_questions_component_1 = __webpack_require__(/*! ../user-security-questions/user-security-questions.component */ "./src/app/registration/user-security-questions/user-security-questions.component.ts");
var user_existing_account_component_1 = __webpack_require__(/*! ../user-existing-account/user-existing-account.component */ "./src/app/registration/user-existing-account/user-existing-account.component.ts");
var string_validator_1 = __webpack_require__(/*! ../../utils/string-validator */ "./src/app/utils/string-validator.ts");
var UserCreationComponent = /** @class */ (function () {
    function UserCreationComponent(userCreateService, domSanitizer, router, window) {
        var _a, _b;
        this.userCreateService = userCreateService;
        this.domSanitizer = domSanitizer;
        this.router = router;
        this.window = window;
        this.registrantuserdata = new user_creation_model_1.CreateRegistrantUserRequest();
        this.isValidUser = false;
        this.isValidNPI = true;
        this.showInvalidDeaError = false;
        this.isValidCaptcha = false;
        this.isDEARequired = false;
        this.isDEAExpDateRequired = false;
        this.isDEAScheduleRequired = false;
        this.isValidationFailed = false;
        this.isLinkExistingShieldUser = false;
        this.namePattern = "^([a-zA-Z]+[\\s-'.]{0,35})*";
        this.suffixPattern = "^([a-zA-Z]+[\\s-'.]{0,6})*";
        this.datePattern = "^(0?[1-9]|1[0-2])\\-(0?[1-9]|1\\d|2\\d|3[01])-\\d{4}$";
        this.zipCodePattern = "^(\\d{5})(?:\\d{4})?$";
        this.cityPattern = "^([a-zA-Z]+[\\s-'.]{0,20})*";
        this.shieldUserNamePattern = "^([a-zA-Z0-9]+[-,_.]{0,30})*";
        this.phoneFaxPattern = "\\(?\\d{3}\\)?-? *\\d{3}-? *-?\\d{4}";
        this.noLeadingSpacePattern = "^[^ ]([\\s\\S])*";
        this.showModal = false;
        this.isPasswordValid = false;
        this.isSecurityQuestionAnswerValid = false;
        this.isUserNameValid = false;
        this.isExistingAccountValid = false;
        this.componentState = (_b = (_a = this.router.getCurrentNavigation()) === null || _a === void 0 ? void 0 : _a.extras) === null || _b === void 0 ? void 0 : _b.state;
    }
    UserCreationComponent.prototype.ngOnInit = function () {
        var _a, _b;
        this.userDetail = new user_creation_model_1.UserDetails();
        this.userDetail.deaScheduleII = false;
        this.userDetail.deaScheduleIII = false;
        this.userDetail.deaScheduleIV = false;
        this.userDetail.deaScheduleV = false;
        this.getInitialPageData();
        this.idmeTerms = "https://www.id.me/terms"; //Fetch this from server call - db config
        if (this.componentState !== undefined && this.componentState !== null) {
            this.userDetail.pricingStructure = this.componentState.SelectedPricingStructure;
        }
        this.mainAppLink = (_b = (_a = this.window) === null || _a === void 0 ? void 0 : _a.appcontext) === null || _b === void 0 ? void 0 : _b.login;
        this.userExistsErrorMessage = "Username already exists. Kindly provide another username.";
        this.userNameLabel = "Username";
    };
    UserCreationComponent.prototype.getInitialPageData = function () {
        var _this = this;
        this.userCreateService.getInitialPageData().subscribe(function (result) {
            _this.states = result.States;
            _this.SpecialityList = result.Speciality;
        });
    };
    UserCreationComponent.prototype.onNpiChanged = function () {
        this.isValidNPI = true;
    };
    UserCreationComponent.prototype.setDeaValidation = function () {
        this.showInvalidDeaError = false;
        var isDEARequired = false;
        var isDEAExpDateRequired = false;
        var isDEAScheduleRequired = false;
        if (!string_validator_1.IsStringNullUndefinedEmpty(this.userDetail.deaNumber)
            || !string_validator_1.IsStringNullUndefinedEmpty(this.userDetail.deaExpirationDate)
            || this.userDetail.deaScheduleII || this.userDetail.deaScheduleIII || this.userDetail.deaScheduleIV || this.userDetail.deaScheduleV) {
            if (string_validator_1.IsStringNullUndefinedEmpty(this.userDetail.deaExpirationDate)) {
                isDEAExpDateRequired = true;
            }
            if (!(this.userDetail.deaScheduleII || this.userDetail.deaScheduleIII || this.userDetail.deaScheduleIV || this.userDetail.deaScheduleV)) {
                isDEAScheduleRequired = true;
            }
            if (string_validator_1.IsStringNullUndefinedEmpty(this.userDetail.deaNumber)) {
                isDEARequired = true;
            }
        }
        this.isDEARequired = isDEARequired;
        this.isDEAExpDateRequired = isDEAExpDateRequired;
        this.isDEAScheduleRequired = isDEAScheduleRequired;
    };
    UserCreationComponent.prototype.validateDEADetails = function () {
        this.setDeaValidation();
        return !(this.isDEARequired && this.isDEAExpDateRequired && this.isDEAScheduleRequired);
    };
    UserCreationComponent.prototype.generateScretAnswer = function () {
        var secretAnsweList = [];
        if (!this.isLinkExistingShieldUser) {
            var secretAnswer = new user_creation_model_1.SecretAnswers();
            var secretAnswer2 = new user_creation_model_1.SecretAnswers();
            var secretAnswer3 = new user_creation_model_1.SecretAnswers();
            secretAnswer.questionId = Number(this.userDetail.securityQuestion1);
            secretAnswer.answer = this.userDetail.securityAnswer1;
            secretAnsweList.push(secretAnswer);
            secretAnswer2.questionId = Number(this.userDetail.securityQuestion2);
            secretAnswer2.answer = this.userDetail.securityAnswer2;
            secretAnsweList.push(secretAnswer2);
            secretAnswer3.questionId = Number(this.userDetail.securityQuestion3);
            secretAnswer3.answer = this.userDetail.securityAnswer3;
            secretAnsweList.push(secretAnswer3);
        }
        return secretAnsweList;
    };
    UserCreationComponent.prototype.setApiError = function (validateRegistrant) {
        this.isValidationFailed = true;
        if (!validateRegistrant.IsValidCaptcha) {
            this.captchaControl.ShowInValidCaptchaError = true;
            this.isValidNPI = true;
            this.showInvalidDeaError = false;
        }
        else if (!validateRegistrant.IsValid && validateRegistrant.IsMaxRetryFinished) {
            this.showModal = true;
        }
        else {
            this.captchaControl.ShowInValidCaptchaError = false;
            this.isValidNPI = validateRegistrant.IsValidNpi;
            this.userExisting.ShowInValidExistingShieldAccountError = !validateRegistrant.IsValidExistingShieldUser;
            this.showInvalidDeaError = false;
            if (validateRegistrant.IsDeaCheck && !validateRegistrant.IsValidDEA) {
                this.showInvalidDeaError = true;
            }
        }
    };
    UserCreationComponent.prototype.validateShieldUserAccount = function () {
        if (this.isLinkExistingShieldUser) {
            this.getExistingAccountDetails();
            return this.isExistingAccountValid;
        }
        else {
            this.getUserAccountNameDetails();
            this.getPasswordDetails();
            this.getSecurityQuestionAnswerDetails();
            return (this.isPasswordValid && this.isUserNameValid && this.isSecurityQuestionAnswerValid);
        }
    };
    UserCreationComponent.prototype.isUserCreationFormValid = function (data) {
        this.markAllControlAsTouched(data);
        this.getCaptchaDetails();
        var isDeaValid = this.validateDEADetails();
        var isShieldUserAccountValid = this.validateShieldUserAccount();
        var isFrmValid = data.valid
            && isDeaValid
            && isShieldUserAccountValid
            && this.isValidCaptcha;
        this.isValidationFailed = !isFrmValid;
        return isFrmValid;
    };
    UserCreationComponent.prototype.markAllControlAsTouched = function (data) {
        Object.keys(data.controls).forEach(function (field) {
            var control = data.controls[field];
            control.markAsTouched({ onlySelf: true });
        });
    };
    UserCreationComponent.prototype.onSubmit = function (data) {
        var _this = this;
        if (this.isUserCreationFormValid(data)) {
            this.registrantuserdata.RegistrantUser = this.userDetail;
            this.registrantuserdata.SecretAnswers = this.generateScretAnswer();
            this.registrantuserdata.Captcha = this.userDetail.txtCapchaResponse;
            this.registrantuserdata.IsLinkExistingShieldUser = this.isLinkExistingShieldUser;
            this.userCreateService
                .saveRegistrationData(this.registrantuserdata)
                .subscribe(function (validateRegistrant) {
                if (validateRegistrant.IsValid) {
                    if (_this.isLinkExistingShieldUser) {
                        _this.router.navigate(["register/createlicense"]);
                    }
                    else {
                        _this.router.navigate(["register/welcome"]);
                    }
                }
                else {
                    _this.setApiError(validateRegistrant);
                }
            });
        }
    };
    UserCreationComponent.prototype.getCaptchaDetails = function () {
        var captchaModel;
        captchaModel = this.captchaControl.GetCaptchaDetails();
        this.userDetail.txtCapchaResponse = captchaModel.captchaText;
        this.isValidCaptcha = captchaModel.isValid;
    };
    UserCreationComponent.prototype.getUserAccountNameDetails = function () {
        var userAccountName;
        userAccountName = this.userNameChild.GetUserAccountNameDetails();
        this.userDetail.shieldUserName = userAccountName.shieldUserName;
        this.isUserNameValid = userAccountName.isValid;
    };
    UserCreationComponent.prototype.getPasswordDetails = function () {
        var userPassword;
        userPassword = this.pwdChild.GetPasswordDetails();
        this.userDetail.password = userPassword.password;
        this.userDetail.confirmPassword = userPassword.confirmPassword;
        this.isPasswordValid = userPassword.isValid;
    };
    UserCreationComponent.prototype.getSecurityQuestionAnswerDetails = function () {
        var securityQuestionAnswerModel;
        securityQuestionAnswerModel = this.securityQuestion.GetSecurityQuestionAnswerDetails();
        this.userDetail.securityQuestion1 = securityQuestionAnswerModel.selectedShieldQuestionOne;
        this.userDetail.securityQuestion2 = securityQuestionAnswerModel.selectedShieldQuestionTwo;
        this.userDetail.securityQuestion3 = securityQuestionAnswerModel.selectedShieldQuestionThree;
        this.userDetail.securityAnswer1 = securityQuestionAnswerModel.securityAnswerOne;
        this.userDetail.securityAnswer2 = securityQuestionAnswerModel.securityAnswerTwo;
        this.userDetail.securityAnswer3 = securityQuestionAnswerModel.securityAnswerThree;
        this.isSecurityQuestionAnswerValid = securityQuestionAnswerModel.isValid;
    };
    UserCreationComponent.prototype.getExistingAccountDetails = function () {
        var existingUserAccount;
        existingUserAccount = this.userExisting.GetExistingAccountDetails();
        this.userDetail.shieldUserName = existingUserAccount.shieldUserName;
        this.userDetail.password = existingUserAccount.password;
        this.isExistingAccountValid = existingUserAccount.isValid;
    };
    tslib_1.__decorate([
        core_1.ViewChild('pwdChild'),
        tslib_1.__metadata("design:type", user_password_component_1.UserPasswordComponent)
    ], UserCreationComponent.prototype, "pwdChild", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('userNameChild'),
        tslib_1.__metadata("design:type", user_account_name_component_1.UserAccountNameComponent)
    ], UserCreationComponent.prototype, "userNameChild", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('securityQuestion'),
        tslib_1.__metadata("design:type", user_security_questions_component_1.UserSecurityQuestionsComponent)
    ], UserCreationComponent.prototype, "securityQuestion", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('userExisting'),
        tslib_1.__metadata("design:type", user_existing_account_component_1.UserExistingAccountComponent)
    ], UserCreationComponent.prototype, "userExisting", void 0);
    tslib_1.__decorate([
        core_1.ViewChild('captchaControl'),
        tslib_1.__metadata("design:type", show_captcha_component_1.ShowCaptchaComponent)
    ], UserCreationComponent.prototype, "captchaControl", void 0);
    UserCreationComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-user-creation',
            template: __webpack_require__(/*! ./user-creation.component.html */ "./src/app/registration/user-creation/user-creation.component.html"),
            styles: [__webpack_require__(/*! ./user-creation.component.css */ "./src/app/registration/user-creation/user-creation.component.css")]
        }),
        tslib_1.__param(3, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [user_create_service_1.UserCreateService, platform_browser_1.DomSanitizer, router_1.Router, Object])
    ], UserCreationComponent);
    return UserCreationComponent;
}());
exports.UserCreationComponent = UserCreationComponent;


/***/ }),

/***/ "./src/app/registration/user-creation/user-creation.model.ts":
/*!*******************************************************************!*\
  !*** ./src/app/registration/user-creation/user-creation.model.ts ***!
  \*******************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ValidateRegistrantModel = exports.CreateRegistrantUserRequest = exports.Speciality = exports.State = exports.SecretAnswers = exports.shieldSecretQuestion = exports.UserDetails = void 0;
var UserDetails = /** @class */ (function () {
    function UserDetails() {
    }
    return UserDetails;
}());
exports.UserDetails = UserDetails;
var shieldSecretQuestion = /** @class */ (function () {
    function shieldSecretQuestion() {
    }
    return shieldSecretQuestion;
}());
exports.shieldSecretQuestion = shieldSecretQuestion;
var SecretAnswers = /** @class */ (function () {
    function SecretAnswers() {
    }
    return SecretAnswers;
}());
exports.SecretAnswers = SecretAnswers;
var State = /** @class */ (function () {
    function State() {
    }
    return State;
}());
exports.State = State;
var Speciality = /** @class */ (function () {
    function Speciality() {
    }
    return Speciality;
}());
exports.Speciality = Speciality;
var CreateRegistrantUserRequest = /** @class */ (function () {
    function CreateRegistrantUserRequest() {
    }
    return CreateRegistrantUserRequest;
}());
exports.CreateRegistrantUserRequest = CreateRegistrantUserRequest;
var ValidateRegistrantModel = /** @class */ (function () {
    function ValidateRegistrantModel() {
    }
    return ValidateRegistrantModel;
}());
exports.ValidateRegistrantModel = ValidateRegistrantModel;


/***/ }),

/***/ "./src/app/registration/user-csp-updater/user-csp-updater.component.ts":
/*!*****************************************************************************!*\
  !*** ./src/app/registration/user-csp-updater/user-csp-updater.component.ts ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserCspUpdaterComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var registration_service_1 = __webpack_require__(/*! ../../service/registration.service */ "./src/app/service/registration.service.ts");
var UserCspUpdaterComponent = /** @class */ (function () {
    function UserCspUpdaterComponent(registrationService) {
        this.registrationService = registrationService;
    }
    UserCspUpdaterComponent.prototype.ngOnInit = function () {
        this.registrationService.updateUserCsp().subscribe(function (registrantContext) {
            window.location.href = registrantContext.RedirectUrl;
        });
    };
    UserCspUpdaterComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'user-csp-updater',
            template: __webpack_require__(/*! ./user-csp-updater.template.html */ "./src/app/registration/user-csp-updater/user-csp-updater.template.html"),
        }),
        tslib_1.__metadata("design:paramtypes", [registration_service_1.RegistrationService])
    ], UserCspUpdaterComponent);
    return UserCspUpdaterComponent;
}());
exports.UserCspUpdaterComponent = UserCspUpdaterComponent;


/***/ }),

/***/ "./src/app/registration/user-csp-updater/user-csp-updater.template.html":
/*!******************************************************************************!*\
  !*** ./src/app/registration/user-csp-updater/user-csp-updater.template.html ***!
  \******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div>\r\n    Processing.....\r\n</div>"

/***/ }),

/***/ "./src/app/registration/user-existing-account/user-existing-account.component.css":
/*!****************************************************************************************!*\
  !*** ./src/app/registration/user-existing-account/user-existing-account.component.css ***!
  \****************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".user-existing-account-form-label-spacing {\r\n    width: 96px;\r\n}\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi91c2VyLWV4aXN0aW5nLWFjY291bnQvdXNlci1leGlzdGluZy1hY2NvdW50LmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSxXQUFXO0FBQ2YiLCJmaWxlIjoiU1BBUmVnaXN0cmF0aW9uL3NyYy9hcHAvcmVnaXN0cmF0aW9uL3VzZXItZXhpc3RpbmctYWNjb3VudC91c2VyLWV4aXN0aW5nLWFjY291bnQuY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbIi51c2VyLWV4aXN0aW5nLWFjY291bnQtZm9ybS1sYWJlbC1zcGFjaW5nIHtcclxuICAgIHdpZHRoOiA5NnB4O1xyXG59XHJcbiJdfQ== */"

/***/ }),

/***/ "./src/app/registration/user-existing-account/user-existing-account.component.html":
/*!*****************************************************************************************!*\
  !*** ./src/app/registration/user-existing-account/user-existing-account.component.html ***!
  \*****************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #userExisting=\"ngForm\" id=\"userExisting\">\r\n    <div>\r\n        <div class=\"erx-form-row\">\r\n            <span class=\"erx-form-label user-existing-account-form-label-spacing\">\r\n                <label><span class=\"highlight-star erx-form-star-position\">*</span>Username</label>\r\n            </span>\r\n            <span class=\"erx-form-input\">\r\n                <input type=\"text\" id=\"existingShieldUserName\" name=\"existingShieldUserName\" class=\"input-medium\" maxlength=\"30\"\r\n                       required [(ngModel)]=\"userExistingAccount.shieldUserName\" #existingShieldUserName=\"ngModel\" autocomplete=\"none\" (change)=\"onCredentialChanged()\"/>\r\n                <span *ngIf=\"existingShieldUserName.invalid && (existingShieldUserName.dirty || existingShieldUserName.touched)\">\r\n                    <span id=\"userNameMandatoryError\" *ngIf=\"existingShieldUserName.errors.required\" class=\"validation-color\">\r\n                        Username is required.\r\n                    </span>\r\n                </span>\r\n                <span *ngIf=\"ShowInValidExistingShieldAccountError\" class=\"validation-color\">\r\n                    Invalid user credentials\r\n                </span>\r\n            </span>\r\n        </div>\r\n    </div>\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label user-existing-account-form-label-spacing\">\r\n            <label><span class=\"highlight-star erx-form-star-position\">*</span>Password</label>\r\n        </span>\r\n        <span class=\"erx-form-input\">\r\n            <input type=\"password\" id=\"existingShieldUserPassword\" name=\"existingShieldUserPassword\" class=\"input-medium\"\r\n                   required [(ngModel)]=\"userExistingAccount.password\" #existingShieldUserPassword=\"ngModel\" maxlength=\"25\" (change)=\"onCredentialChanged()\"/>\r\n            <span *ngIf=\"existingShieldUserPassword.invalid && (existingShieldUserPassword.dirty || existingShieldUserPassword.touched)\">\r\n                <span id=\"passwordRequiredError\" *ngIf=\"existingShieldUserPassword.errors.required\" class=\"validation-color\">\r\n                    Password is required.\r\n                </span>\r\n            </span>\r\n        </span>\r\n    </div>\r\n</form>"

/***/ }),

/***/ "./src/app/registration/user-existing-account/user-existing-account.component.ts":
/*!***************************************************************************************!*\
  !*** ./src/app/registration/user-existing-account/user-existing-account.component.ts ***!
  \***************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserExistingAccountComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var user_existing_account_model_1 = __webpack_require__(/*! ./user-existing-account.model */ "./src/app/registration/user-existing-account/user-existing-account.model.ts");
var UserExistingAccountComponent = /** @class */ (function () {
    function UserExistingAccountComponent() {
        this.userExistingAccount = new user_existing_account_model_1.ExistingUserAccount();
    }
    UserExistingAccountComponent.prototype.ngOnInit = function () {
    };
    UserExistingAccountComponent.prototype.makeDirty = function () {
        this.userExisting.controls.existingShieldUserName.markAsDirty();
        this.userExisting.controls.existingShieldUserPassword.markAsDirty();
    };
    UserExistingAccountComponent.prototype.GetExistingAccountDetails = function () {
        this.makeDirty();
        this.userExistingAccount.isValid = this.userExisting.valid;
        return this.userExistingAccount;
    };
    UserExistingAccountComponent.prototype.onCredentialChanged = function () {
        this.ShowInValidExistingShieldAccountError = false;
    };
    tslib_1.__decorate([
        core_1.ViewChild('userExisting'),
        tslib_1.__metadata("design:type", Object)
    ], UserExistingAccountComponent.prototype, "userExisting", void 0);
    UserExistingAccountComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'user-existing-account',
            template: __webpack_require__(/*! ./user-existing-account.component.html */ "./src/app/registration/user-existing-account/user-existing-account.component.html"),
            styles: [__webpack_require__(/*! ./user-existing-account.component.css */ "./src/app/registration/user-existing-account/user-existing-account.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [])
    ], UserExistingAccountComponent);
    return UserExistingAccountComponent;
}());
exports.UserExistingAccountComponent = UserExistingAccountComponent;


/***/ }),

/***/ "./src/app/registration/user-existing-account/user-existing-account.model.ts":
/*!***********************************************************************************!*\
  !*** ./src/app/registration/user-existing-account/user-existing-account.model.ts ***!
  \***********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ExistingUserAccount = void 0;
var ExistingUserAccount = /** @class */ (function () {
    function ExistingUserAccount() {
    }
    return ExistingUserAccount;
}());
exports.ExistingUserAccount = ExistingUserAccount;


/***/ }),

/***/ "./src/app/registration/user-password/user-password.component.css":
/*!************************************************************************!*\
  !*** ./src/app/registration/user-password/user-password.component.css ***!
  \************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".user-creation-form-label-spacing {\r\n    width: 155px;\r\n}\r\n\r\n.user-creation-form-rules {\r\n    margin-left: 131px;\r\n}\r\n\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi91c2VyLXBhc3N3b3JkL3VzZXItcGFzc3dvcmQuY29tcG9uZW50LmNzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtJQUNJLFlBQVk7QUFDaEI7O0FBRUE7SUFDSSxrQkFBa0I7QUFDdEIiLCJmaWxlIjoiU1BBUmVnaXN0cmF0aW9uL3NyYy9hcHAvcmVnaXN0cmF0aW9uL3VzZXItcGFzc3dvcmQvdXNlci1wYXNzd29yZC5jb21wb25lbnQuY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLnVzZXItY3JlYXRpb24tZm9ybS1sYWJlbC1zcGFjaW5nIHtcclxuICAgIHdpZHRoOiAxNTVweDtcclxufVxyXG5cclxuLnVzZXItY3JlYXRpb24tZm9ybS1ydWxlcyB7XHJcbiAgICBtYXJnaW4tbGVmdDogMTMxcHg7XHJcbn1cclxuXHJcbiJdfQ== */"

/***/ }),

/***/ "./src/app/registration/user-password/user-password.component.html":
/*!*************************************************************************!*\
  !*** ./src/app/registration/user-password/user-password.component.html ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #pwdForm=\"ngForm\" id=\"pwdForm\">\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n            <label><span class=\"highlight-star erx-form-star-position\">*</span>Password</label>\r\n        </span>\r\n        <span class=\"erx-form-input\">\r\n            <input type=\"password\" id=\"password1\" name=\"password1\" class=\"input-medium\"\r\n                   required [(ngModel)]=\"userPassWord.password\" #password1=\"ngModel\" maxlength=\"25\" minlength=\"8\" (blur)=\"validatePassword()\"/>\r\n            <span *ngIf=\"password1.invalid && (password1.dirty || password1.touched)\">\r\n                <span id=\"passwordRequiredError1\" *ngIf=\"password1.errors.required\" class=\"validation-color\">\r\n                    Password is required.\r\n                </span>\r\n\r\n                <span id=\"passwordPatternError\" *ngIf=\"password1.errors.invalid\" class=\"validation-color\">\r\n                    Password does not meet criteria mentioned below.\r\n                </span>\r\n            </span>\r\n        </span>\r\n    </div>\r\n\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n            <label><span class=\"highlight-star erx-form-star-position\">*</span>Confirm Password</label>\r\n        </span>\r\n        <span class=\"erx-form-input\">\r\n            <input type=\"password\" id=\"confirmPassword1\" name=\"confirmPassword1\" class=\"input-medium\"\r\n                   required [(ngModel)]=\"userPassWord.confirmPassword\" #confirmPassword1=\"ngModel\" maxlength=\"25\" minlength=\"8\" />\r\n            \r\n            <span *ngIf=\"confirmPassword1.invalid && (confirmPassword1.dirty || confirmPassword1.touched)\">\r\n                <span id=\"confirmPasswordMandatoryError\" *ngIf=\"confirmPassword1.errors.required\" class=\"validation-color\">\r\n                    Confirm Password is required.\r\n                </span>\r\n                <span id=\"confirmPasswordPatternError\" *ngIf=\"confirmPassword1.errors.pattern\" class=\"validation-color\">\r\n                    Passwords do not match.\r\n                </span>\r\n            </span>\r\n       \r\n        </span>\r\n    </div>\r\n\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label user-creation-form-label-spacing\">\r\n            <label>Rules</label>\r\n        </span>\r\n        <span class=\"erx-form-input\">\r\n            Must have between 8 (min) and 25 (max) characters and any three of the following:\r\n        </span>\r\n        <div class=\"user-creation-form-rules\">\r\n            <ul>\r\n                <li>One (1) upper case character</li>\r\n                <li>One (1) lower case character</li>\r\n                <li>One (1) special character</li>\r\n                <li>One (1) number</li>\r\n            </ul>\r\n        </div>\r\n    </div>\r\n</form>"

/***/ }),

/***/ "./src/app/registration/user-password/user-password.component.ts":
/*!***********************************************************************!*\
  !*** ./src/app/registration/user-password/user-password.component.ts ***!
  \***********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserPasswordComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var user_password_model_1 = __webpack_require__(/*! ../user-password/user-password.model */ "./src/app/registration/user-password/user-password.model.ts");
var UserPasswordComponent = /** @class */ (function () {
    function UserPasswordComponent() {
        this.userPassWord = new user_password_model_1.UserPassword();
    }
    UserPasswordComponent.prototype.makeDirty = function () {
        this.pwdForm.controls.password1.markAsDirty();
        this.pwdForm.controls.confirmPassword1.markAsDirty();
    };
    UserPasswordComponent.prototype.GetPasswordDetails = function () {
        this.makeDirty();
        this.userPassWord.isValid = this.pwdForm.valid;
        return this.userPassWord;
    };
    UserPasswordComponent.prototype.validatePassword = function () {
        var txtPasswordValue = this.userPassWord.password;
        if (!txtPasswordValue.PasswordValidator()) {
            this.pwdForm.controls["password1"].setErrors({
                "invalid": "Invalid Password"
            });
        }
        else {
            this.pwdForm.controls["password1"].setErrors(null);
        }
    };
    UserPasswordComponent.prototype.validateConfirmPassword = function () {
        var txtPasswordValue = this.userPassWord.password;
        var txtConfirmPasswordValue = this.userPassWord.confirmPassword;
        if (txtConfirmPasswordValue != txtPasswordValue) {
            this.pwdForm.controls["confirmPassword1"].setErrors({
                "pattern": "Passwords do not match"
            });
            return;
        }
        else {
            this.pwdForm.controls["confirmPassword1"].setErrors(null);
            return;
        }
    };
    tslib_1.__decorate([
        core_1.ViewChild('pwdForm'),
        tslib_1.__metadata("design:type", Object)
    ], UserPasswordComponent.prototype, "pwdForm", void 0);
    UserPasswordComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-user-password',
            template: __webpack_require__(/*! ./user-password.component.html */ "./src/app/registration/user-password/user-password.component.html"),
            styles: [__webpack_require__(/*! ./user-password.component.css */ "./src/app/registration/user-password/user-password.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [])
    ], UserPasswordComponent);
    return UserPasswordComponent;
}());
exports.UserPasswordComponent = UserPasswordComponent;


/***/ }),

/***/ "./src/app/registration/user-password/user-password.model.ts":
/*!*******************************************************************!*\
  !*** ./src/app/registration/user-password/user-password.model.ts ***!
  \*******************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserPassword = void 0;
var UserPassword = /** @class */ (function () {
    function UserPassword() {
    }
    return UserPassword;
}());
exports.UserPassword = UserPassword;


/***/ }),

/***/ "./src/app/registration/user-security-questions/user-security-questions.component.css":
/*!********************************************************************************************!*\
  !*** ./src/app/registration/user-security-questions/user-security-questions.component.css ***!
  \********************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "\r\n.user-creation-password-rules {\r\n    height: 80px;\r\n}\r\n\r\n    .user-creation-password-rules > ul {\r\n        position: relative;\r\n        top: 3px;\r\n    }\r\n\r\n    .user-creation-password-rules > ul > li {\r\n            position: relative;\r\n            left: 2px;\r\n            list-style-position: inside;\r\n        }\r\n\r\n    .user-creation-dea-schedule {\r\n    margin-left: -7px;\r\n    margin-right: 8px;\r\n    padding-bottom: 11px;\r\n    margin-bottom: 7px;\r\n    position: relative;\r\n    top: -7px;\r\n}\r\n\r\n    .captcha-dimensions {\r\n    height: 50px;\r\n    width: 200px;\r\n}\r\n\r\n    .user-creation-captcha-input-group {\r\n    display: inline-block;\r\n    padding-left: 15px;\r\n    position: relative;\r\n    top: -9px;\r\n}\r\n\r\n    .user-creation-captcha-label {\r\n    display: inline-block;\r\n    padding-left: 25px;\r\n}\r\n\r\n    .user-creation-captcha-input {\r\n    display: block;\r\n    padding-top: 5px;\r\n    padding-left: 10px;\r\n}\r\n\r\n    .user-creation-security-questions {\r\n    padding-left: 1px;\r\n    padding-right: 365px;\r\n}\r\n\r\n    .user-creation-security-questions-first-row {\r\n    margin-top: 3px;\r\n}\r\n\r\n    .user-creation-captcha-refresh {\r\n    clear: both;\r\n    cursor: pointer;\r\n    padding-top: 5px;\r\n}\r\n\r\n    .user-creation-already-completed {\r\n    margin: 10px 0px 25px 0px;\r\n}\r\n\r\n    .user-creation-already-completed > span {\r\n        font-weight: 900;\r\n        font-size: 14px;\r\n        padding-right: 10px;\r\n    }\r\n\r\n    .user-creation-already-completed > a {\r\n        color: #5e4da5;\r\n        text-decoration: none;\r\n    }\r\n\r\n    .user-creation-information {\r\n    margin: 20px 0px;\r\n    padding: 10px;\r\n    background-color: #D6ECEE;\r\n}\r\n\r\n    .user-creation-information > img {\r\n        position: relative;\r\n        top: 3px;\r\n    }\r\n\r\n    .user-creation-information > span {\r\n        font-weight: 900;\r\n    }\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi91c2VyLXNlY3VyaXR5LXF1ZXN0aW9ucy91c2VyLXNlY3VyaXR5LXF1ZXN0aW9ucy5jb21wb25lbnQuY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7QUFDQTtJQUNJLFlBQVk7QUFDaEI7O0lBRUk7UUFDSSxrQkFBa0I7UUFDbEIsUUFBUTtJQUNaOztJQUVJO1lBQ0ksa0JBQWtCO1lBQ2xCLFNBQVM7WUFDVCwyQkFBMkI7UUFDL0I7O0lBRVI7SUFDSSxpQkFBaUI7SUFDakIsaUJBQWlCO0lBQ2pCLG9CQUFvQjtJQUNwQixrQkFBa0I7SUFDbEIsa0JBQWtCO0lBQ2xCLFNBQVM7QUFDYjs7SUFFQTtJQUNJLFlBQVk7SUFDWixZQUFZO0FBQ2hCOztJQUVBO0lBQ0kscUJBQXFCO0lBQ3JCLGtCQUFrQjtJQUNsQixrQkFBa0I7SUFDbEIsU0FBUztBQUNiOztJQUVBO0lBQ0kscUJBQXFCO0lBQ3JCLGtCQUFrQjtBQUN0Qjs7SUFFQTtJQUNJLGNBQWM7SUFDZCxnQkFBZ0I7SUFDaEIsa0JBQWtCO0FBQ3RCOztJQUVBO0lBQ0ksaUJBQWlCO0lBQ2pCLG9CQUFvQjtBQUN4Qjs7SUFFQTtJQUNJLGVBQWU7QUFDbkI7O0lBRUE7SUFDSSxXQUFXO0lBQ1gsZUFBZTtJQUNmLGdCQUFnQjtBQUNwQjs7SUFFQTtJQUNJLHlCQUF5QjtBQUM3Qjs7SUFFSTtRQUNJLGdCQUFnQjtRQUNoQixlQUFlO1FBQ2YsbUJBQW1CO0lBQ3ZCOztJQUVBO1FBQ0ksY0FBYztRQUNkLHFCQUFxQjtJQUN6Qjs7SUFFSjtJQUNJLGdCQUFnQjtJQUNoQixhQUFhO0lBQ2IseUJBQXlCO0FBQzdCOztJQUVJO1FBQ0ksa0JBQWtCO1FBQ2xCLFFBQVE7SUFDWjs7SUFFQTtRQUNJLGdCQUFnQjtJQUNwQiIsImZpbGUiOiJTUEFSZWdpc3RyYXRpb24vc3JjL2FwcC9yZWdpc3RyYXRpb24vdXNlci1zZWN1cml0eS1xdWVzdGlvbnMvdXNlci1zZWN1cml0eS1xdWVzdGlvbnMuY29tcG9uZW50LmNzcyIsInNvdXJjZXNDb250ZW50IjpbIlxyXG4udXNlci1jcmVhdGlvbi1wYXNzd29yZC1ydWxlcyB7XHJcbiAgICBoZWlnaHQ6IDgwcHg7XHJcbn1cclxuXHJcbiAgICAudXNlci1jcmVhdGlvbi1wYXNzd29yZC1ydWxlcyA+IHVsIHtcclxuICAgICAgICBwb3NpdGlvbjogcmVsYXRpdmU7XHJcbiAgICAgICAgdG9wOiAzcHg7XHJcbiAgICB9XHJcblxyXG4gICAgICAgIC51c2VyLWNyZWF0aW9uLXBhc3N3b3JkLXJ1bGVzID4gdWwgPiBsaSB7XHJcbiAgICAgICAgICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgICAgICAgICAgbGVmdDogMnB4O1xyXG4gICAgICAgICAgICBsaXN0LXN0eWxlLXBvc2l0aW9uOiBpbnNpZGU7XHJcbiAgICAgICAgfVxyXG5cclxuLnVzZXItY3JlYXRpb24tZGVhLXNjaGVkdWxlIHtcclxuICAgIG1hcmdpbi1sZWZ0OiAtN3B4O1xyXG4gICAgbWFyZ2luLXJpZ2h0OiA4cHg7XHJcbiAgICBwYWRkaW5nLWJvdHRvbTogMTFweDtcclxuICAgIG1hcmdpbi1ib3R0b206IDdweDtcclxuICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcclxuICAgIHRvcDogLTdweDtcclxufVxyXG5cclxuLmNhcHRjaGEtZGltZW5zaW9ucyB7XHJcbiAgICBoZWlnaHQ6IDUwcHg7XHJcbiAgICB3aWR0aDogMjAwcHg7XHJcbn1cclxuXHJcbi51c2VyLWNyZWF0aW9uLWNhcHRjaGEtaW5wdXQtZ3JvdXAge1xyXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xyXG4gICAgcGFkZGluZy1sZWZ0OiAxNXB4O1xyXG4gICAgcG9zaXRpb246IHJlbGF0aXZlO1xyXG4gICAgdG9wOiAtOXB4O1xyXG59XHJcblxyXG4udXNlci1jcmVhdGlvbi1jYXB0Y2hhLWxhYmVsIHtcclxuICAgIGRpc3BsYXk6IGlubGluZS1ibG9jaztcclxuICAgIHBhZGRpbmctbGVmdDogMjVweDtcclxufVxyXG5cclxuLnVzZXItY3JlYXRpb24tY2FwdGNoYS1pbnB1dCB7XHJcbiAgICBkaXNwbGF5OiBibG9jaztcclxuICAgIHBhZGRpbmctdG9wOiA1cHg7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDEwcHg7XHJcbn1cclxuXHJcbi51c2VyLWNyZWF0aW9uLXNlY3VyaXR5LXF1ZXN0aW9ucyB7XHJcbiAgICBwYWRkaW5nLWxlZnQ6IDFweDtcclxuICAgIHBhZGRpbmctcmlnaHQ6IDM2NXB4O1xyXG59XHJcblxyXG4udXNlci1jcmVhdGlvbi1zZWN1cml0eS1xdWVzdGlvbnMtZmlyc3Qtcm93IHtcclxuICAgIG1hcmdpbi10b3A6IDNweDtcclxufVxyXG5cclxuLnVzZXItY3JlYXRpb24tY2FwdGNoYS1yZWZyZXNoIHtcclxuICAgIGNsZWFyOiBib3RoO1xyXG4gICAgY3Vyc29yOiBwb2ludGVyO1xyXG4gICAgcGFkZGluZy10b3A6IDVweDtcclxufVxyXG5cclxuLnVzZXItY3JlYXRpb24tYWxyZWFkeS1jb21wbGV0ZWQge1xyXG4gICAgbWFyZ2luOiAxMHB4IDBweCAyNXB4IDBweDtcclxufVxyXG5cclxuICAgIC51c2VyLWNyZWF0aW9uLWFscmVhZHktY29tcGxldGVkID4gc3BhbiB7XHJcbiAgICAgICAgZm9udC13ZWlnaHQ6IDkwMDtcclxuICAgICAgICBmb250LXNpemU6IDE0cHg7XHJcbiAgICAgICAgcGFkZGluZy1yaWdodDogMTBweDtcclxuICAgIH1cclxuXHJcbiAgICAudXNlci1jcmVhdGlvbi1hbHJlYWR5LWNvbXBsZXRlZCA+IGEge1xyXG4gICAgICAgIGNvbG9yOiAjNWU0ZGE1O1xyXG4gICAgICAgIHRleHQtZGVjb3JhdGlvbjogbm9uZTtcclxuICAgIH1cclxuXHJcbi51c2VyLWNyZWF0aW9uLWluZm9ybWF0aW9uIHtcclxuICAgIG1hcmdpbjogMjBweCAwcHg7XHJcbiAgICBwYWRkaW5nOiAxMHB4O1xyXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI0Q2RUNFRTtcclxufVxyXG5cclxuICAgIC51c2VyLWNyZWF0aW9uLWluZm9ybWF0aW9uID4gaW1nIHtcclxuICAgICAgICBwb3NpdGlvbjogcmVsYXRpdmU7XHJcbiAgICAgICAgdG9wOiAzcHg7XHJcbiAgICB9XHJcblxyXG4gICAgLnVzZXItY3JlYXRpb24taW5mb3JtYXRpb24gPiBzcGFuIHtcclxuICAgICAgICBmb250LXdlaWdodDogOTAwO1xyXG4gICAgfVxyXG4iXX0= */"

/***/ }),

/***/ "./src/app/registration/user-security-questions/user-security-questions.component.html":
/*!*********************************************************************************************!*\
  !*** ./src/app/registration/user-security-questions/user-security-questions.component.html ***!
  \*********************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<form #securityQuestionForm=\"ngForm\" id=\"securityQuestionForm\">\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label\"></span>\r\n        <span class=\"user-creation-security-questions\">\r\n            Select Security Questions\r\n        </span>\r\n        <span>\r\n            Provide Answers\r\n        </span>\r\n    </div>\r\n    <div class=\"erx-form-row user-creation-security-questions-first-row\">\r\n        <span class=\"erx-form-label\"></span>\r\n        <span class=\"erx-form-input\">\r\n            <select ID=\"questionOne\" name=\"questionOne\" Width=\"335px\" [(ngModel)]=\"securityQuestionAnswerModel.selectedShieldQuestionOne\" class=\"input-x-large\">\r\n                <option *ngFor=\"let item of ShieldQuestions\" [value]=\"item.questionIDField\">\r\n                    {{item.questionField}}\r\n                </option>\r\n            </select>\r\n        </span>\r\n        <span class=\"erx-form-input\">\r\n            <span class=\"highlight-star erx-form-star-position\">*</span>\r\n            <input type=\"text\" id=\"ansOne\" name=\"ansOne\" Width=\"335\" class=\"input-medium\"\r\n                   required [(ngModel)]=\"securityQuestionAnswerModel.securityAnswerOne\" #ansOne=\"ngModel\" />\r\n\r\n\r\n            <span *ngIf=\"ansOne.invalid && (ansOne.dirty || ansOne.touched)\">\r\n                <span *ngIf=\"ansOne.errors.required\" class=\"validation-color\">\r\n                    Select security questions and provide answers.\r\n                </span>\r\n            </span>\r\n        </span>\r\n    </div>\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label\"></span>\r\n        <span class=\"erx-form-input\">\r\n            <select ID=\"questionTwo\" name=\"questionTwo\" Width=\"335px\" [(ngModel)]=\"securityQuestionAnswerModel.selectedShieldQuestionTwo\" class=\"input-x-large\">\r\n                <option *ngFor=\"let item of ShieldQuestions\" [value]=\"item.questionIDField\">\r\n                    {{item.questionField}}\r\n                </option>\r\n            </select>\r\n        </span>\r\n        <span class=\"erx-form-input\">\r\n            <span class=\"highlight-star erx-form-star-position\">*</span>\r\n            <input id=\"ansTwo\" name=\"ansTwo\" Width=\"335\" class=\"input-medium\"\r\n                   required [(ngModel)]=\"securityQuestionAnswerModel.securityAnswerTwo\" #ansTwo=\"ngModel\" />\r\n            <span *ngIf=\"ansTwo.invalid && (ansTwo.dirty || ansTwo.touched)\">\r\n                <span *ngIf=\"ansTwo.errors.required\" class=\"validation-color\">\r\n                    Select security questions and provide answers.\r\n                </span>\r\n            </span>\r\n            <span *ngIf=\"(securityQuestionAnswerModel.selectedShieldQuestionTwo == securityQuestionAnswerModel.selectedShieldQuestionThree \r\n                  || securityQuestionAnswerModel.selectedShieldQuestionTwo==securityQuestionAnswerModel.selectedShieldQuestionOne) \r\n                  && securityQuestionAnswerModel.selectedShieldQuestionTwo\" class=\"validation-color\">\r\n                Select different question.\r\n            </span>\r\n            <span *ngIf=\"(securityQuestionAnswerModel.securityAnswerTwo == securityQuestionAnswerModel.securityAnswerThree \r\n                  || securityQuestionAnswerModel.securityAnswerTwo==securityQuestionAnswerModel.securityAnswerOne) \r\n                  && securityQuestionAnswerModel.securityAnswerTwo\" class=\"validation-color\">\r\n                Select different answer.\r\n            </span>\r\n        </span>\r\n    </div>\r\n    <div class=\"erx-form-row\">\r\n        <span class=\"erx-form-label\"></span>\r\n        <span class=\"erx-form-input\">\r\n            <select ID=\"questionThree\" name=\"questionThree\" Width=\"335px\" [(ngModel)]=\"securityQuestionAnswerModel.selectedShieldQuestionThree\" class=\"input-x-large\">\r\n                <option *ngFor=\"let item of ShieldQuestions\" [value]=\"item.questionIDField\">\r\n                    {{item.questionField}}\r\n                </option>\r\n            </select>\r\n        </span>\r\n        <span class=\"erx-form-input\">\r\n            <span class=\"highlight-star erx-form-star-position\">*</span>\r\n            <input id=\"ansThree\" name=\"ansThree\" Width=\"335\" class=\"input-medium\"\r\n                   required [(ngModel)]=\"securityQuestionAnswerModel.securityAnswerThree\" #ansThree=\"ngModel\" />\r\n            <span *ngIf=\"ansThree.invalid && (ansThree.dirty || ansThree.touched)\">\r\n                <span *ngIf=\"ansThree.errors.required\" class=\"validation-color\">\r\n                    Select security questions and provide answers.\r\n                </span>\r\n            </span>\r\n            <span *ngIf=\"(securityQuestionAnswerModel.selectedShieldQuestionThree == securityQuestionAnswerModel.selectedShieldQuestionTwo \r\n                  || securityQuestionAnswerModel.selectedShieldQuestionThree==securityQuestionAnswerModel.selectedShieldQuestionOne) \r\n                  && securityQuestionAnswerModel.selectedShieldQuestionThree\" class=\"validation-color\">\r\n                Select different question.\r\n            </span>\r\n            <span *ngIf=\"(securityQuestionAnswerModel.securityAnswerThree == securityQuestionAnswerModel.securityAnswerTwo \r\n                  || securityQuestionAnswerModel.securityAnswerThree==securityQuestionAnswerModel.securityAnswerOne) \r\n                  && securityQuestionAnswerModel.securityAnswerThree\" class=\"validation-color\">\r\n                Select different answer.\r\n            </span>\r\n        </span>\r\n    </div>\r\n    </form>\r\n\r\n"

/***/ }),

/***/ "./src/app/registration/user-security-questions/user-security-questions.component.ts":
/*!*******************************************************************************************!*\
  !*** ./src/app/registration/user-security-questions/user-security-questions.component.ts ***!
  \*******************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserSecurityQuestionsComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
/// <reference path="user-security-questions.model.ts" />
/// <reference path="user-security-questions.model.ts" />
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var user_create_service_1 = __webpack_require__(/*! ../../service/user-create.service */ "./src/app/service/user-create.service.ts");
var user_security_questions_model_1 = __webpack_require__(/*! ./user-security-questions.model */ "./src/app/registration/user-security-questions/user-security-questions.model.ts");
var UserSecurityQuestionsComponent = /** @class */ (function () {
    function UserSecurityQuestionsComponent(userCreateService) {
        this.userCreateService = userCreateService;
        this.securityQuestionAnswerModel = new user_security_questions_model_1.SecurityQuestionAnswerModel();
    }
    UserSecurityQuestionsComponent.prototype.makeDirty = function () {
        this.securityQuestionForm.controls.ansOne.markAsDirty();
        this.securityQuestionForm.controls.ansTwo.markAsDirty();
        this.securityQuestionForm.controls.ansThree.markAsDirty();
    };
    UserSecurityQuestionsComponent.prototype.validateSecurityQuestions = function () {
        if (this.securityQuestionAnswerModel.selectedShieldQuestionOne == this.securityQuestionAnswerModel.selectedShieldQuestionTwo
            || this.securityQuestionAnswerModel.selectedShieldQuestionOne == this.securityQuestionAnswerModel.selectedShieldQuestionThree
            || this.securityQuestionAnswerModel.selectedShieldQuestionTwo == this.securityQuestionAnswerModel.selectedShieldQuestionThree) {
            return false;
        }
        else {
            return true;
        }
    };
    UserSecurityQuestionsComponent.prototype.validateSecurityAnswers = function () {
        if (this.securityQuestionAnswerModel.securityAnswerOne == this.securityQuestionAnswerModel.securityAnswerTwo
            || this.securityQuestionAnswerModel.securityAnswerOne == this.securityQuestionAnswerModel.securityAnswerThree
            || this.securityQuestionAnswerModel.securityAnswerTwo == this.securityQuestionAnswerModel.securityAnswerThree) {
            return false;
        }
        else {
            return true;
        }
    };
    UserSecurityQuestionsComponent.prototype.GetSecurityQuestionAnswerDetails = function () {
        this.makeDirty();
        this.securityQuestionAnswerModel.isValid = this.securityQuestionForm.valid
            && this.validateSecurityQuestions()
            && this.validateSecurityAnswers();
        return this.securityQuestionAnswerModel;
    };
    UserSecurityQuestionsComponent.prototype.ngOnInit = function () {
        this.getInitialPageData();
    };
    UserSecurityQuestionsComponent.prototype.getInitialPageData = function () {
        var _this = this;
        this.userCreateService.getSecurityQuestions().subscribe(function (result) {
            _this.setShieldQuestionsDefaultValues(result.SecurityQuestions);
        });
    };
    UserSecurityQuestionsComponent.prototype.setShieldQuestionsDefaultValues = function (securityquestions) {
        if (securityquestions) {
            this.ShieldQuestions = securityquestions;
            this.securityQuestionAnswerModel.selectedShieldQuestionOne = this.ShieldQuestions.find(function (x) { return x.questionIDField == 1; }).questionIDField;
            this.securityQuestionAnswerModel.selectedShieldQuestionTwo = this.ShieldQuestions.find(function (x) { return x.questionIDField == 2; }).questionIDField;
            this.securityQuestionAnswerModel.selectedShieldQuestionThree = this.ShieldQuestions.find(function (x) { return x.questionIDField == 3; }).questionIDField;
        }
    };
    tslib_1.__decorate([
        core_1.ViewChild('securityQuestionForm'),
        tslib_1.__metadata("design:type", Object)
    ], UserSecurityQuestionsComponent.prototype, "securityQuestionForm", void 0);
    UserSecurityQuestionsComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'app-user-security-questions',
            template: __webpack_require__(/*! ./user-security-questions.component.html */ "./src/app/registration/user-security-questions/user-security-questions.component.html"),
            styles: [__webpack_require__(/*! ./user-security-questions.component.css */ "./src/app/registration/user-security-questions/user-security-questions.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [user_create_service_1.UserCreateService])
    ], UserSecurityQuestionsComponent);
    return UserSecurityQuestionsComponent;
}());
exports.UserSecurityQuestionsComponent = UserSecurityQuestionsComponent;


/***/ }),

/***/ "./src/app/registration/user-security-questions/user-security-questions.model.ts":
/*!***************************************************************************************!*\
  !*** ./src/app/registration/user-security-questions/user-security-questions.model.ts ***!
  \***************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ShieldSecretQuestion = exports.SecurityQuestionAnswerModel = void 0;
var SecurityQuestionAnswerModel = /** @class */ (function () {
    function SecurityQuestionAnswerModel() {
    }
    return SecurityQuestionAnswerModel;
}());
exports.SecurityQuestionAnswerModel = SecurityQuestionAnswerModel;
var ShieldSecretQuestion = /** @class */ (function () {
    function ShieldSecretQuestion() {
    }
    return ShieldSecretQuestion;
}());
exports.ShieldSecretQuestion = ShieldSecretQuestion;


/***/ }),

/***/ "./src/app/registration/welcome-page/welcome-page.component.css":
/*!**********************************************************************!*\
  !*** ./src/app/registration/welcome-page/welcome-page.component.css ***!
  \**********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".welcome-left-panel {\r\n    padding: 15px 10px 10px 10px;\r\n    margin: 10px;\r\n    float: left;\r\n}\r\n\r\n.welcome-right-panel {\r\n    padding: 15px 10px 10px 10px;\r\n    margin: 10px;\r\n    padding-right: 0px;\r\n    margin-right: 0px;\r\n}\r\n\r\n.welcome-right-panel > img {\r\n        width: 360px;\r\n    }\r\n\r\n.welcome-group {\r\n    margin-bottom: 2px;\r\n}\r\n\r\n.welcome-buttons {\r\n    padding: 10px;\r\n    margin: 10px;\r\n    clear: both;\r\n}\r\n\r\nul {\r\n    padding-left: 18px;\r\n}\r\n\r\nli {\r\n    padding-bottom: 3px;\r\n}\r\n\r\n.welcome-nested-list-style {\r\n    padding-left: 10px;\r\n    margin-top: -10px;\r\n}\r\n\r\n.welcome-congratulations {\r\n    margin-bottom: 15px;\r\n    font-weight: 900;\r\n}\r\n\r\n.welcome-download-process-document {\r\n    margin-top: 12px;\r\n}\r\n\r\n.welcome-download-process-document > a {\r\n        font-weight: 900;\r\n    }\r\n\r\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi93ZWxjb21lLXBhZ2Uvd2VsY29tZS1wYWdlLmNvbXBvbmVudC5jc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7SUFDSSw0QkFBNEI7SUFDNUIsWUFBWTtJQUNaLFdBQVc7QUFDZjs7QUFFQTtJQUNJLDRCQUE0QjtJQUM1QixZQUFZO0lBQ1osa0JBQWtCO0lBQ2xCLGlCQUFpQjtBQUNyQjs7QUFFSTtRQUNJLFlBQVk7SUFDaEI7O0FBRUo7SUFDSSxrQkFBa0I7QUFDdEI7O0FBRUE7SUFDSSxhQUFhO0lBQ2IsWUFBWTtJQUNaLFdBQVc7QUFDZjs7QUFFQTtJQUNJLGtCQUFrQjtBQUN0Qjs7QUFFQTtJQUNJLG1CQUFtQjtBQUN2Qjs7QUFFQTtJQUNJLGtCQUFrQjtJQUNsQixpQkFBaUI7QUFDckI7O0FBRUE7SUFDSSxtQkFBbUI7SUFDbkIsZ0JBQWdCO0FBQ3BCOztBQUVBO0lBQ0ksZ0JBQWdCO0FBQ3BCOztBQUVJO1FBQ0ksZ0JBQWdCO0lBQ3BCIiwiZmlsZSI6IlNQQVJlZ2lzdHJhdGlvbi9zcmMvYXBwL3JlZ2lzdHJhdGlvbi93ZWxjb21lLXBhZ2Uvd2VsY29tZS1wYWdlLmNvbXBvbmVudC5jc3MiLCJzb3VyY2VzQ29udGVudCI6WyIud2VsY29tZS1sZWZ0LXBhbmVsIHtcclxuICAgIHBhZGRpbmc6IDE1cHggMTBweCAxMHB4IDEwcHg7XHJcbiAgICBtYXJnaW46IDEwcHg7XHJcbiAgICBmbG9hdDogbGVmdDtcclxufVxyXG5cclxuLndlbGNvbWUtcmlnaHQtcGFuZWwge1xyXG4gICAgcGFkZGluZzogMTVweCAxMHB4IDEwcHggMTBweDtcclxuICAgIG1hcmdpbjogMTBweDtcclxuICAgIHBhZGRpbmctcmlnaHQ6IDBweDtcclxuICAgIG1hcmdpbi1yaWdodDogMHB4O1xyXG59XHJcblxyXG4gICAgLndlbGNvbWUtcmlnaHQtcGFuZWwgPiBpbWcge1xyXG4gICAgICAgIHdpZHRoOiAzNjBweDtcclxuICAgIH1cclxuXHJcbi53ZWxjb21lLWdyb3VwIHtcclxuICAgIG1hcmdpbi1ib3R0b206IDJweDtcclxufVxyXG5cclxuLndlbGNvbWUtYnV0dG9ucyB7XHJcbiAgICBwYWRkaW5nOiAxMHB4O1xyXG4gICAgbWFyZ2luOiAxMHB4O1xyXG4gICAgY2xlYXI6IGJvdGg7XHJcbn1cclxuXHJcbnVsIHtcclxuICAgIHBhZGRpbmctbGVmdDogMThweDtcclxufVxyXG5cclxubGkge1xyXG4gICAgcGFkZGluZy1ib3R0b206IDNweDtcclxufVxyXG5cclxuLndlbGNvbWUtbmVzdGVkLWxpc3Qtc3R5bGUge1xyXG4gICAgcGFkZGluZy1sZWZ0OiAxMHB4O1xyXG4gICAgbWFyZ2luLXRvcDogLTEwcHg7XHJcbn1cclxuXHJcbi53ZWxjb21lLWNvbmdyYXR1bGF0aW9ucyB7XHJcbiAgICBtYXJnaW4tYm90dG9tOiAxNXB4O1xyXG4gICAgZm9udC13ZWlnaHQ6IDkwMDtcclxufVxyXG5cclxuLndlbGNvbWUtZG93bmxvYWQtcHJvY2Vzcy1kb2N1bWVudCB7XHJcbiAgICBtYXJnaW4tdG9wOiAxMnB4O1xyXG59XHJcblxyXG4gICAgLndlbGNvbWUtZG93bmxvYWQtcHJvY2Vzcy1kb2N1bWVudCA+IGEge1xyXG4gICAgICAgIGZvbnQtd2VpZ2h0OiA5MDA7XHJcbiAgICB9XHJcbiJdfQ== */"

/***/ }),

/***/ "./src/app/registration/welcome-page/welcome-page.component.html":
/*!***********************************************************************!*\
  !*** ./src/app/registration/welcome-page/welcome-page.component.html ***!
  \***********************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div id=\"welcome-title\" class=\"title-bar title-bar-heading branded-background-color branded-font-color\">\r\n    Welcome to {{title}}!\r\n</div>\r\n\r\n<div class=\"welcome-left-panel\">\r\n    <div class=\"welcome-group welcome-congratulations\">\r\n        Congratulations on completing Step 1 of the {{title}} Registration!\r\n    </div>\r\n    <div class=\"welcome-group\">\r\n        In order to send prescriptions electronically you will need to complete our identity proofing process through our trusted vendor ID.me.\r\n    </div>\r\n\r\n    <div class=\"welcome-group\">\r\n        <div class=\"erx-form-sub-heading font-setting-bold font-uppercase\">\r\n            Before You Proceed\r\n        </div>\r\n        <ul>\r\n            <li>Download and install the ID.me Authenticator app from the application store associated with your smart phone <span>(Apple Store or Google Play).</span></li>\r\n            <li>Be sure that your smart phone <span class=\"highlight-star\">*</span> (or tablet) has a functioning camera and browser that can receive text messages</li>\r\n            <li>Once you begin the ID.me identity proofing registration, you will need the following information</li>\r\n        </ul>\r\n        <div class=\"welcome-nested-list-style\">\r\n            <ol>\r\n                <li>Your ePrescribe User ID and Password that you just created</li>\r\n                <li>Your individual email address (group/shared emails are NOT allowed)</li>\r\n                <li>Either your Drivers License or Passport or State Issued ID</li>\r\n                <li>Your Social Security Number</li>\r\n            </ol>\r\n        </div>\r\n    </div>\r\n   \r\n    <div class=\"welcome-group\">\r\n        <span class=\"highlight-star\">*</span> This must be the same device you will always use to generate the passcodes when prescribing controlled substances.\r\n    </div>\r\n    <div class=\"welcome-group welcome-download-process-document\">\r\n        <a target=\"_blank\" href=\"/Help/Documents/IDmeRegistrationProcess.pdf\"> Click here to review the process</a>.\r\n    </div>\r\n</div>\r\n<div class=\"welcome-right-panel\">\r\n    <img src=\"/images/registration/idmePlayStoreImage.png\" />\r\n</div>\r\n\r\n<div class=\"welcome-buttons\">\r\n    <input type=\"button\" (click)=\"navigateToCSP()\" class=\"button-style\" value=\"Continue\">\r\n</div>\r\n\r\n"

/***/ }),

/***/ "./src/app/registration/welcome-page/welcome-page.component.ts":
/*!*********************************************************************!*\
  !*** ./src/app/registration/welcome-page/welcome-page.component.ts ***!
  \*********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.WelcomePageComponent = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var welcome_service_1 = __webpack_require__(/*! ../../service/welcome-service */ "./src/app/service/welcome-service.ts");
var WelcomePageComponent = /** @class */ (function () {
    function WelcomePageComponent(welcomeService) {
        this.welcomeService = welcomeService;
        this.title = "ePrescribe";
        var appContext = window.appcontext;
        this.title = appContext.appName;
    }
    WelcomePageComponent.prototype.navigateToCSP = function () {
        this.welcomeService
            .logRegistrantUserNavigationToCSP()
            .subscribe(function (registrantContext) {
            window.location.href = registrantContext.CspUrl;
        });
    };
    WelcomePageComponent = tslib_1.__decorate([
        core_1.Component({
            selector: 'welcome-page',
            template: __webpack_require__(/*! ./welcome-page.component.html */ "./src/app/registration/welcome-page/welcome-page.component.html"),
            styles: [__webpack_require__(/*! ./welcome-page.component.css */ "./src/app/registration/welcome-page/welcome-page.component.css")]
        }),
        tslib_1.__metadata("design:paramtypes", [welcome_service_1.WelcomeService])
    ], WelcomePageComponent);
    return WelcomePageComponent;
}());
exports.WelcomePageComponent = WelcomePageComponent;


/***/ }),

/***/ "./src/app/service/activation-code.service.ts":
/*!****************************************************!*\
  !*** ./src/app/service/activation-code.service.ts ***!
  \****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.ActivationCodeService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var data_service_1 = __webpack_require__(/*! ./data.service */ "./src/app/service/data.service.ts");
var ActivationCodeService = /** @class */ (function () {
    function ActivationCodeService(dataService) {
        this.dataService = dataService;
    }
    ActivationCodeService.prototype.validateActivationCode = function (activationCodeModel) {
        return this.dataService.post("api/anonymous/user-activation/validate-activation-code/", activationCodeModel);
    };
    ActivationCodeService.prototype.intializeWorkFlow = function (data) {
        return this.dataService.post("api/anonymous/user-activation/intialize-workflow/", data);
    };
    ActivationCodeService.prototype.linkToExistingUser = function (linkAccountModel) {
        return this.dataService.post("api/anonymous/user-activation/link-existing-user/", linkAccountModel);
    };
    ActivationCodeService = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__metadata("design:paramtypes", [data_service_1.DataService])
    ], ActivationCodeService);
    return ActivationCodeService;
}());
exports.ActivationCodeService = ActivationCodeService;


/***/ }),

/***/ "./src/app/service/captcha.service.ts":
/*!********************************************!*\
  !*** ./src/app/service/captcha.service.ts ***!
  \********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.CaptchaService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var data_service_1 = __webpack_require__(/*! ./data.service */ "./src/app/service/data.service.ts");
var CaptchaService = /** @class */ (function () {
    function CaptchaService(dataService) {
        this.dataService = dataService;
    }
    CaptchaService.prototype.getCaptcha = function () {
        return this.dataService.get("api/anonymous/user-registration/getcaptcha");
    };
    CaptchaService = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__metadata("design:paramtypes", [data_service_1.DataService])
    ], CaptchaService);
    return CaptchaService;
}());
exports.CaptchaService = CaptchaService;


/***/ }),

/***/ "./src/app/service/data.service.ts":
/*!*****************************************!*\
  !*** ./src/app/service/data.service.ts ***!
  \*****************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.DataService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var operators_1 = __webpack_require__(/*! rxjs/operators */ "../node_modules/rxjs/_esm5/operators/index.js");
var http_1 = __webpack_require__(/*! @angular/common/http */ "../node_modules/@angular/common/fesm5/http.js");
var rxjs_1 = __webpack_require__(/*! rxjs */ "../node_modules/rxjs/_esm5/index.js");
var message_service_1 = __webpack_require__(/*! ./message.service */ "./src/app/service/message.service.ts");
var DataService = /** @class */ (function () {
    function DataService(http, messageService) {
        this.http = http;
        this.messageService = messageService;
    }
    DataService.prototype.get = function (url, skipErrorPage) {
        var _this = this;
        if (skipErrorPage === void 0) { skipErrorPage = null; }
        var headers = new http_1.HttpHeaders({ 'Content-Type': 'application/json' });
        return this.http.get(url, { headers: headers, observe: "response" })
            .pipe(operators_1.map(function (resp) {
            return _this.extractApiResponse(resp);
        }), operators_1.catchError(this.handleError(url, null, false, skipErrorPage)));
    };
    DataService.prototype.post = function (url, data, customErrorMessage) {
        var _this = this;
        if (customErrorMessage === void 0) { customErrorMessage = ""; }
        var headers = new http_1.HttpHeaders({ 'Content-Type': 'application/json' });
        return this.http.post(url, data, { headers: headers, observe: "response" })
            .pipe(operators_1.map(function (resp) {
            return _this.extractApiResponse(resp, customErrorMessage);
        }), operators_1.catchError(this.handleError(url, null, customErrorMessage.length > 0)));
    };
    DataService.prototype.extractApiResponse = function (apiResponse, customErrorMessage) {
        if (customErrorMessage === void 0) { customErrorMessage = ""; }
        if (apiResponse === null || apiResponse === void 0 ? void 0 : apiResponse.body.ErrorContext) {
            throw new http_1.HttpErrorResponse({ status: 500, statusText: customErrorMessage + " " + (apiResponse === null || apiResponse === void 0 ? void 0 : apiResponse.body.ErrorContext.Message) });
        }
        //If anything unable to interpret from server redirect to the exception page
        return apiResponse.body.Payload != null ? apiResponse.body.Payload : {};
    };
    /**
    * Handle Http operation that failed.
    * Let the app continue.
    * @param operation - name of the operation that failed
    * @param result - optional value to return as the observable result
    */
    DataService.prototype.handleError = function (operation, result, avoidCustomText, skipErrorPage) {
        var _this = this;
        if (operation === void 0) { operation = 'operation'; }
        if (avoidCustomText === void 0) { avoidCustomText = false; }
        if (skipErrorPage === void 0) { skipErrorPage = null; }
        return function (error) {
            if (skipErrorPage && skipErrorPage(error))
                return rxjs_1.of(result);
            // TODO: send the error to app insight infrastructure
            var errorAddition = avoidCustomText ? "" : "Please contact customer support!";
            //Display to customer
            _this.messageService.notify(error.statusText + ". " + errorAddition);
            // Let the app keep running by returning an empty result.
            return rxjs_1.of(result);
        };
    };
    DataService = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__metadata("design:paramtypes", [http_1.HttpClient, message_service_1.MessageService])
    ], DataService);
    return DataService;
}());
exports.DataService = DataService;


/***/ }),

/***/ "./src/app/service/license-create.service.ts":
/*!***************************************************!*\
  !*** ./src/app/service/license-create.service.ts ***!
  \***************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.LicenseCreateService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var data_service_1 = __webpack_require__(/*! ./data.service */ "./src/app/service/data.service.ts");
var LicenseCreateService = /** @class */ (function () {
    function LicenseCreateService(dataService) {
        this.dataService = dataService;
    }
    LicenseCreateService.prototype.updateRegistrantPracticeDetails = function (licenseData) {
        return this.dataService.post("api/license-creation/update-practice-details", licenseData);
    };
    LicenseCreateService = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__metadata("design:paramtypes", [data_service_1.DataService])
    ], LicenseCreateService);
    return LicenseCreateService;
}());
exports.LicenseCreateService = LicenseCreateService;


/***/ }),

/***/ "./src/app/service/loader.service.ts":
/*!*******************************************!*\
  !*** ./src/app/service/loader.service.ts ***!
  \*******************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.LoaderService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var rxjs_1 = __webpack_require__(/*! rxjs */ "../node_modules/rxjs/_esm5/index.js");
var LoaderService = /** @class */ (function () {
    function LoaderService() {
        this.isLoading = new rxjs_1.Subject();
    }
    Object.defineProperty(LoaderService.prototype, "overrideLoading", {
        get: function () {
            return this._overrideLoading;
        },
        enumerable: false,
        configurable: true
    });
    LoaderService.prototype.show = function (forceOverrideLoader) {
        if (forceOverrideLoader === void 0) { forceOverrideLoader = false; }
        this._overrideLoading = forceOverrideLoader;
        this.isLoading.next(true);
    };
    LoaderService.prototype.hide = function () {
        this.isLoading.next(false);
    };
    LoaderService = tslib_1.__decorate([
        core_1.Injectable()
    ], LoaderService);
    return LoaderService;
}());
exports.LoaderService = LoaderService;


/***/ }),

/***/ "./src/app/service/message.service.ts":
/*!********************************************!*\
  !*** ./src/app/service/message.service.ts ***!
  \********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.MessageService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var router_1 = __webpack_require__(/*! @angular/router */ "../node_modules/@angular/router/fesm5/router.js");
var MessageService = /** @class */ (function () {
    function MessageService(router) {
        this.router = router;
        this.message = "";
    }
    MessageService.prototype.notify = function (message) {
        this.message = message;
        this.router.navigate(["register/error"]);
    };
    MessageService = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__metadata("design:paramtypes", [router_1.Router])
    ], MessageService);
    return MessageService;
}());
exports.MessageService = MessageService;


/***/ }),

/***/ "./src/app/service/registration.service.ts":
/*!*************************************************!*\
  !*** ./src/app/service/registration.service.ts ***!
  \*************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.RegistrationService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var data_service_1 = __webpack_require__(/*! ./data.service */ "./src/app/service/data.service.ts");
var RegistrationService = /** @class */ (function () {
    function RegistrationService(dataService) {
        this.dataService = dataService;
    }
    RegistrationService.prototype.getRegistrantStatus = function (isLicencePage) {
        var showErrorMessage = function (error) {
            //When response URL is login.aspx,means status check was not successful
            //? In https://github.com/angular/angular/blob/7.2.x/packages/common/http/src/xhr.ts
            // IE11 for XHR request dont values responseURL, hence in this f/w as fallback they keep request url
            if (error.url && (error.url.toLowerCase().indexOf("login.aspx") > -1 ||
                error.url.toLowerCase().indexOf("api/registrantstatus") > -1)) {
                return true;
            }
            return false;
        };
        return this.dataService.get("api/registrantstatus/" + isLicencePage, showErrorMessage);
    };
    RegistrationService.prototype.updateUserCsp = function () {
        return this.dataService.post("api/update-user-csp", null);
    };
    RegistrationService = tslib_1.__decorate([
        core_1.Injectable({ providedIn: 'root' }),
        tslib_1.__metadata("design:paramtypes", [data_service_1.DataService])
    ], RegistrationService);
    return RegistrationService;
}());
exports.RegistrationService = RegistrationService;


/***/ }),

/***/ "./src/app/service/user-create.service.ts":
/*!************************************************!*\
  !*** ./src/app/service/user-create.service.ts ***!
  \************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.UserCreateService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var data_service_1 = __webpack_require__(/*! ./data.service */ "./src/app/service/data.service.ts");
var UserCreateService = /** @class */ (function () {
    function UserCreateService(dataService, window) {
        var _a, _b;
        this.dataService = dataService;
        this.window = window;
        this.supportMailAddress = (_b = (_a = this.window) === null || _a === void 0 ? void 0 : _a.appcontext) === null || _b === void 0 ? void 0 : _b.supportMailAddress;
    }
    UserCreateService.prototype.getInitialPageData = function () {
        return this.dataService.get("api/anonymous/user-registration/setup-information");
    };
    UserCreateService.prototype.getSecurityQuestions = function () {
        return this.dataService.get("api/anonymous/user-registration/setup-security-questions");
    };
    UserCreateService.prototype.validateShieldUserName = function (userName) {
        return this.dataService.get("api/anonymous/user-registration/checkuser/" + userName);
    };
    UserCreateService.prototype.getCaptcha = function () {
        return this.dataService.get("api/anonymous/user-registration/getcaptcha");
    };
    UserCreateService.prototype.saveRegistrationData = function (userData) {
        var customErrorMessage = "We encountered an error while trying to register. Kindly contact the ePrescribe support at (" + this.supportMailAddress + "), include your email address (" + userData.RegistrantUser.personalEmail + ") for assistance.";
        return this.dataService.post("api/anonymous/user-registration/create-registrant", userData, customErrorMessage);
    };
    UserCreateService.prototype.saveUserData = function (userData) {
        var customErrorMessage = "We encountered an error while trying to register. Kindly contact the ePrescribe support at (" + this.supportMailAddress + "), include your email address (" + userData.personalEmail + ") for assistance.";
        return this.dataService.post("api/anonymous/user-registration/create-user", userData, customErrorMessage);
    };
    UserCreateService = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__param(1, core_1.Inject('window')),
        tslib_1.__metadata("design:paramtypes", [data_service_1.DataService, Object])
    ], UserCreateService);
    return UserCreateService;
}());
exports.UserCreateService = UserCreateService;


/***/ }),

/***/ "./src/app/service/welcome-service.ts":
/*!********************************************!*\
  !*** ./src/app/service/welcome-service.ts ***!
  \********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.WelcomeService = void 0;
var tslib_1 = __webpack_require__(/*! tslib */ "../node_modules/tslib/tslib.es6.js");
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var data_service_1 = __webpack_require__(/*! ./data.service */ "./src/app/service/data.service.ts");
var WelcomeService = /** @class */ (function () {
    function WelcomeService(dataService) {
        this.dataService = dataService;
    }
    WelcomeService.prototype.logRegistrantUserNavigationToCSP = function () {
        return this.dataService.post("api/welcome/geturl-for-navigation-to-csp", null);
    };
    WelcomeService = tslib_1.__decorate([
        core_1.Injectable(),
        tslib_1.__metadata("design:paramtypes", [data_service_1.DataService])
    ], WelcomeService);
    return WelcomeService;
}());
exports.WelcomeService = WelcomeService;


/***/ }),

/***/ "./src/app/utils/string-validator.ts":
/*!*******************************************!*\
  !*** ./src/app/utils/string-validator.ts ***!
  \*******************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
exports.IsStringNullUndefinedEmpty = void 0;
function IsStringNullUndefinedEmpty(input) {
    if (input == undefined
        || input == null) {
        return true;
    }
    var inputString = String(input);
    if (inputString == '') {
        return true;
    }
    return false;
}
exports.IsStringNullUndefinedEmpty = IsStringNullUndefinedEmpty;


/***/ }),

/***/ "./src/main.ts":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__(/*! @angular/core */ "../node_modules/@angular/core/fesm5/core.js");
var platform_browser_dynamic_1 = __webpack_require__(/*! @angular/platform-browser-dynamic */ "../node_modules/@angular/platform-browser-dynamic/fesm5/platform-browser-dynamic.js");
var app_module_1 = __webpack_require__(/*! ./app/app.module */ "./src/app/app.module.ts");
//if (environment.production) {//Until we figure out the file replacement in webpack config.
core_1.enableProdMode();
platform_browser_dynamic_1.platformBrowserDynamic().bootstrapModule(app_module_1.AppModule)
    .catch(function (err) { return console.error(err); });


/***/ }),

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! C:\Anju\eprescribe\ePrescribe\eRxWeb\SPARegistration\src\main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map