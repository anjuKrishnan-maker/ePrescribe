if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.DomEventMixin)=="undefined"||typeof (window.RadControlsNamespace.DomEventMixin.Version)==null||window.RadControlsNamespace.DomEventMixin.Version<3){
RadControlsNamespace.DomEventMixin={Version:3,Initialize:function(_1){
_1.CreateEventHandler=this.CreateEventHandler;
_1.AttachDomEvent=this.AttachDomEvent;
_1.DetachDomEvent=this.DetachDomEvent;
_1.DisposeDomEventHandlers=this.DisposeDomEventHandlers;
_1._domEventHandlingEnabled=true;
_1.EnableDomEventHandling=this.EnableDomEventHandling;
_1.DisableDomEventHandling=this.DisableDomEventHandling;
_1.RemoveHandlerRegister=this.RemoveHandlerRegister;
_1.GetHandlerRegister=this.GetHandlerRegister;
_1.AddHandlerRegister=this.AddHandlerRegister;
_1.handlerRegisters=[];
},EnableDomEventHandling:function(){
this._domEventHandlingEnabled=true;
},DisableDomEventHandling:function(){
this._domEventHandlingEnabled=false;
},CreateEventHandler:function(_2,_3){
var _4=this;
return function(e){
if(!_4._domEventHandlingEnabled&&!_3){
return;
}
return _4[_2](e||window.event);
};
},AttachDomEvent:function(_6,_7,_8,_9){
var _a=this.CreateEventHandler(_8,_9);
var _b=this.GetHandlerRegister(_6,_7,_8);
if(_b!=null){
this.DetachDomEvent(_b.Element,_b.EventName,_8);
}
var _c={"Element":_6,"EventName":_7,"HandlerName":_8,"Handler":_a};
this.AddHandlerRegister(_c);
if(_6.addEventListener){
_6.addEventListener(_7,_a,false);
}else{
if(_6.attachEvent){
_6.attachEvent("on"+_7,_a);
}
}
},DetachDomEvent:function(_d,_e,_f){
var _10=null;
var _11="";
if(typeof _f=="string"){
_11=_f;
_10=this.GetHandlerRegister(_d,_e,_11);
if(_10==null){
return;
}
_f=_10.Handler;
}
if(!_d){
return;
}
if(_d.removeEventListener){
_d.removeEventListener(_e,_f,false);
}else{
if(_d.detachEvent){
_d.detachEvent("on"+_e,_f);
}
}
if(_10!=null&&_11!=""){
this.RemoveHandlerRegister(_10);
_10=null;
}
},DisposeDomEventHandlers:function(){
for(var i=0;i<this.handlerRegisters.length;i++){
var _13=this.handlerRegisters[i];
if(_13!=null){
this.DetachDomEvent(_13.Element,_13.EventName,_13.Handler);
}
}
this.handlerRegisters=[];
},RemoveHandlerRegister:function(_14){
try{
var _15=_14.index;
for(var i in _14){
_14[i]=null;
}
this.handlerRegisters[_15]=null;
}
catch(e){
}
},GetHandlerRegister:function(_17,_18,_19){
for(var i=0;i<this.handlerRegisters.length;i++){
var _1b=this.handlerRegisters[i];
if(_1b!=null&&_1b.Element==_17&&_1b.EventName==_18&&_1b.HandlerName==_19){
return this.handlerRegisters[i];
}
}
return null;
},AddHandlerRegister:function(_1c){
_1c.index=this.handlerRegisters.length;
this.handlerRegisters[this.handlerRegisters.length]=_1c;
}};
RadControlsNamespace.DomEvent={};
RadControlsNamespace.DomEvent.PreventDefault=function(e){
if(!e){
return true;
}
if(e.preventDefault){
e.preventDefault();
}
e.returnValue=false;
return false;
};
RadControlsNamespace.DomEvent.StopPropagation=function(e){
if(!e){
return;
}
if(e.stopPropagation){
e.stopPropagation();
}else{
e.cancelBubble=true;
}
};
RadControlsNamespace.DomEvent.GetTarget=function(e){
if(!e){
return null;
}
return e.target||e.srcElement;
};
RadControlsNamespace.DomEvent.GetRelatedTarget=function(e){
if(!e){
return null;
}
return e.relatedTarget||(e.type=="mouseout"?e.toElement:e.fromElement);
};
RadControlsNamespace.DomEvent.GetKeyCode=function(e){
if(!e){
return 0;
}
return e.which||e.keyCode;
};
}
var RadGridNamespace={};
RadGridNamespace.Prefix="grid_";
RadGridNamespace.InitializeClient=function(_22){
var _23=document.getElementById(_22+"AtlasCreation");
if(!_23){
return;
}
var _24=document.createElement("script");
if(navigator.userAgent.indexOf("Safari")!=-1){
_24.innerHTML=_23.innerHTML;
}else{
_24.text=_23.innerHTML;
}
if(!window.netscape){
document.body.appendChild(_24);
document.body.removeChild(_24);
}else{
document.body.insertBefore(_24,document.body.firstChild);
_24.parentNode.removeChild(_24);
}
_23.parentNode.removeChild(_23);
};
RadGridNamespace.AsyncRequest=function(_25,_26,_27,e){
var _29=window[_27];
if(_29!=null&&typeof (_29.AsyncRequest)=="function"){
_29.AsyncRequest(_25,_26,e);
}
};
RadGridNamespace.AsyncRequestWithOptions=function(_2a,_2b,e){
var _2d=window[_2b];
if(_2d!=null&&typeof (_2d.AsyncRequestWithOptions)=="function"){
_2d.AsyncRequestWithOptions(_2a,e);
}
};
RadGridNamespace.GetVisibleCols=function(_2e){
var _2f=0;
for(var i=0,l=_2e.length;i<l;i++){
if(_2e[i].style.display=="none"){
continue;
}
_2f++;
}
return _2f;
};
RadGridNamespace.HideShowCells=function(_32,_33,_34,_35){
var _36=RadGridNamespace.GetVisibleCols(_35);
for(var i=0,l=_32.rows.length;i<l;i++){
if(_32.rows[i].cells.length!=_36){
if(_32.rows[i].cells.length==1){
_32.rows[i].cells[0].colSpan=_36;
}else{
for(var j=0;j<_32.rows[i].cells.length;j++){
if(_32.rows[i].cells[j].colSpan>1&&j>=_33){
if(!_34){
_32.rows[i].cells[j].colSpan=_32.rows[i].cells[j].colSpan-1;
}else{
_32.rows[i].cells[j].colSpan=_32.rows[i].cells[j].colSpan+1;
}
break;
}
}
}
}
var _3a=_32.rows[i].cells[_33];
var _3b=(navigator.userAgent.toLowerCase().indexOf("safari")!=-1&&navigator.userAgent.indexOf("Mac")!=-1)?0:1;
if(!_34){
if(_3a!=null&&_3a.colSpan==_3b&&_3a.style.display!="none"){
_3a.style.display="none";
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
RadGridNamespace.HideShowSelect(_3a,_34);
}
}
}else{
if(_3a!=null&&_3a.colSpan==_3b&&_3a.style.display=="none"){
_3a.style.display=(window.netscape)?"table-cell":"";
}
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
RadGridNamespace.HideShowSelect(_3a,_34);
}
}
}
};
RadGridNamespace.HideShowSelect=function(_3c,_3d){
if(!_3c){
return;
}
var _3e=_3c.getElementsByTagName("select");
for(var i=0;i<_3e.length;i++){
_3e[i].style.display=(_3d)?"":"none";
}
};
RadGridNamespace.GetWidth=function(_40){
var _41;
if(window.getComputedStyle){
_41=window.getComputedStyle(_40,"").getPropertyValue("width");
}else{
if(_40.currentStyle){
_41=_40.currentStyle.width;
}else{
_41=_40.offsetWidth;
}
}
if(_41.toString().indexOf("%")!=-1){
_41=_40.offsetWidth;
}
if(_41.toString().indexOf("px")!=-1){
_41=parseInt(_41);
}
return _41;
};
RadGridNamespace.GetScrollBarWidth=function(){
try{
if(typeof (RadGridNamespace.scrollbarWidth)=="undefined"){
var _42,_43=0;
var _44=document.createElement("div");
_44.style.position="absolute";
_44.style.top="-1000px";
_44.style.left="-1000px";
_44.style.width="100px";
_44.style.overflow="auto";
var _45=document.createElement("div");
_45.style.width="1000px";
_44.appendChild(_45);
document.body.appendChild(_44);
_42=_44.offsetWidth;
_43=_44.clientWidth;
document.body.removeChild(document.body.lastChild);
RadGridNamespace.scrollbarWidth=_42-_43;
if(RadGridNamespace.scrollbarWidth<=0||_43==0){
RadGridNamespace.scrollbarWidth=16;
}
}
return RadGridNamespace.scrollbarWidth;
}
catch(error){
return false;
}
};
RadGridNamespace.GetScrollBarHeight=function(){
try{
if(typeof (RadGridNamespace.scrollbarHeight)=="undefined"){
var _46,_47=0;
var _48=document.createElement("div");
_48.style.position="absolute";
_48.style.top="-1000px";
_48.style.left="-1000px";
_48.style.width="100px";
_48.style.height="100px";
_48.style.overflow="auto";
var _49=document.createElement("div");
_49.style.width="1000px";
_49.style.height="1000px";
_48.appendChild(_49);
document.body.appendChild(_48);
_46=_48.offsetHeight;
_47=_48.clientHeight;
document.body.removeChild(document.body.lastChild);
RadGridNamespace.scrollbarHeight=_46-_47;
if(RadGridNamespace.scrollbarHeight<=0||_47==0){
RadGridNamespace.scrollbarHeight=16;
}
}
return RadGridNamespace.scrollbarHeight;
}
catch(error){
return false;
}
};
RadGridNamespace.GetTableColGroup=function(_4a){
try{
return _4a.getElementsByTagName("colgroup")[0];
}
catch(error){
return false;
}
};
RadGridNamespace.GetTableColGroupCols=function(_4b){
try{
var _4c=new Array();
var _4d=_4b.childNodes[0];
for(var i=0;i<_4b.childNodes.length;i++){
if((_4b.childNodes[i].tagName)&&(_4b.childNodes[i].tagName.toLowerCase()=="col")){
_4c[_4c.length]=_4b.childNodes[i];
}
}
return _4c;
}
catch(error){
return false;
}
};
RadGridNamespace.Confirm=function(_4f,e){
if(!confirm(_4f)){
e.cancelBubble=true;
e.returnValue=false;
return false;
}
};
RadGridNamespace.SynchronizeWithWindow=function(){
};
RadGridNamespace.IsRightToLeft=function(_51){
try{
while(_51){
if(_51.currentStyle&&_51.currentStyle.direction.toLowerCase()=="rtl"){
return true;
}else{
if(getComputedStyle&&getComputedStyle(_51,"").getPropertyValue("direction").toLowerCase()=="rtl"){
return true;
}else{
if(_51.dir.toLowerCase()=="rtl"){
return true;
}
}
}
_51=_51.parentNode;
}
return false;
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError,this.OnError);
}
};
RadGridNamespace.FireEvent=function(_52,_53,_54){
try{
var _55=true;
if(typeof (_52[_53])=="string"){
eval(_52[_53]);
}else{
if(typeof (_52[_53])=="function"){
if(_54){
switch(_54.length){
case 1:
_55=_52[_53](_54[0]);
break;
case 2:
_55=_52[_53](_54[0],_54[1]);
break;
}
}else{
_55=_52[_53]();
}
}
}
if(typeof (_55)!="boolean"){
return true;
}else{
return _55;
}
}
catch(error){
throw error;
}
};
RadGridNamespace.CheckParentNodesFor=function(_56,_57){
while(_56){
if(_56==_57){
return true;
}
_56=_56.parentNode;
}
return false;
};
RadGridNamespace.GetCurrentElement=function(e){
if(!e){
var e=window.event;
}
var _59;
if(e.srcElement){
_59=e.srcElement;
}else{
_59=e.target;
}
return _59;
};
RadGridNamespace.GetEventPosX=function(e){
var x=e.clientX;
var _5c=RadGridNamespace.GetCurrentElement(e);
while(_5c.parentNode){
if(typeof (_5c.parentNode.scrollLeft)=="number"){
x+=_5c.parentNode.scrollLeft;
}
_5c=_5c.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)-parseInt(document.body.currentStyle.marginLeft);
}
if(RadGridNamespace.IsRightToLeft(document.body)){
x=x-RadGridNamespace.GetScrollBarWidth();
}
return x;
};
RadGridNamespace.GetEventPosY=function(e){
var y=e.clientY;
var _5f=RadGridNamespace.GetCurrentElement(e);
while(_5f.parentNode){
if(typeof (_5f.parentNode.scrollTop)=="number"){
y+=_5f.parentNode.scrollTop;
}
_5f=_5f.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)-parseInt(document.body.currentStyle.marginTop);
}
return y;
};
RadGridNamespace.IsChildOf=function(_60,_61){
while(_60.parentNode){
if(_60.parentNode==_61){
return true;
}
_60=_60.parentNode;
}
return false;
};
RadGridNamespace.GetFirstParentByTagName=function(_62,_63){
while(_62.parentNode){
if(_62.tagName.toLowerCase()==_63.toLowerCase()){
return _62;
}
_62=_62.parentNode;
}
return null;
};
RadGridNamespace.FindScrollPosX=function(_64){
var x=0;
while(_64.parentNode){
if(typeof (_64.parentNode.scrollLeft)=="number"){
x+=_64.parentNode.scrollLeft;
}
_64=_64.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)-parseInt(document.body.currentStyle.marginLeft);
}
return x;
};
RadGridNamespace.FindScrollPosY=function(_66){
var y=0;
while(_66.parentNode){
if(typeof (_66.parentNode.scrollTop)=="number"){
y+=_66.parentNode.scrollTop;
}
_66=_66.parentNode;
}
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)-parseInt(document.body.currentStyle.marginTop);
}
return y;
};
RadGridNamespace.FindPosX=function(_68){
try{
var x=0;
var _6a=0;
if(_68.offsetParent){
while(_68.offsetParent){
x+=_68.offsetLeft;
if(_68.currentStyle&&_68.currentStyle.borderLeftWidth&&_68.currentStyle.borderLeftWidth.indexOf("px")!=-1&&!window.opera){
_6a+=parseInt(_68.currentStyle.borderLeftWidth);
}
_68=_68.offsetParent;
}
}else{
if(_68.x){
x+=_68.x;
}
}
if(document.compatMode=="BackCompat"||navigator.userAgent.indexOf("Safari")!=-1){
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)-parseInt(document.body.currentStyle.marginLeft);
}
if(document.defaultView&&document.defaultView.getComputedStyle&&document.defaultView.getComputedStyle(document.body,"").marginLeft.indexOf("px")!=-1&&!window.opera){
x=parseInt(x)+parseInt(document.defaultView.getComputedStyle(document.body,"").marginLeft);
}
}
return x+_6a;
}
catch(error){
return x;
}
};
RadGridNamespace.FindPosY=function(_6b){
var y=0;
var _6d=0;
if(_6b.offsetParent){
while(_6b.offsetParent){
y+=_6b.offsetTop;
if(_6b.currentStyle&&_6b.currentStyle.borderTopWidth&&_6b.currentStyle.borderTopWidth.indexOf("px")!=-1&&!window.opera){
_6d+=parseInt(_6b.currentStyle.borderTopWidth);
}
_6b=_6b.offsetParent;
}
}else{
if(_6b.y){
y+=_6b.y;
}
}
if(document.compatMode=="BackCompat"||navigator.userAgent.indexOf("Safari")!=-1){
if(document.body.currentStyle&&document.body.currentStyle&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)-parseInt(document.body.currentStyle.marginTop);
}
if(document.defaultView&&document.defaultView.getComputedStyle&&document.defaultView.getComputedStyle(document.body,"").marginTop.indexOf("px")!=-1&&!window.opera){
y=parseInt(y)+parseInt(document.defaultView.getComputedStyle(document.body,"").marginTop);
}
}
return y+_6d;
};
RadGridNamespace.GetNodeNextSiblingByTagName=function(_6e,_6f){
while((_6e!=null)&&(_6e.tagName!=_6f)){
_6e=_6e.nextSibling;
}
return _6e;
};
RadGridNamespace.GetNodeNextSibling=function(_70){
while(_70!=null){
if(_70.nextSibling){
_70=_70.nextSibling;
}else{
_70=null;
}
if(_70){
if(_70.nodeType==1){
break;
}
}
}
return _70;
};
RadGridNamespace.DeleteSubString=function(_71,_72,_73){
return _71=_71.substring(0,_72)+_71.substring(_73+1,_71.length);
};
RadGridNamespace.ClearDocumentEvents=function(){
if(document.onmousedown!=this.mouseDownHandler){
this.documentOnMouseDown=document.onmousedown;
}
if(document.onselectstart!=this.selectStartHandler){
this.documentOnSelectStart=document.onselectstart;
}
if(document.ondragstart!=this.dragStartHandler){
this.documentOnDragStart=document.ondragstart;
}
this.mouseDownHandler=function(e){
return false;
};
this.selectStartHandler=function(){
return false;
};
this.dragStartHandler=function(){
return false;
};
document.onmousedown=this.mouseDownHandler;
document.onselectstart=this.selectStartHandler;
document.ondragstart=this.dragStartHandler;
};
RadGridNamespace.RestoreDocumentEvents=function(){
if((typeof (this.documentOnMouseDown)=="function")&&(document.onmousedown!=this.mouseDownHandler)){
document.onmousedown=this.documentOnMouseDown;
}else{
document.onmousedown="";
}
if((typeof (this.documentOnSelectStart)=="function")&&(document.onselectstart!=this.selectStartHandler)){
document.onselectstart=this.documentOnSelectStart;
}else{
document.onselectstart="";
}
if((typeof (this.documentOnDragStart)=="function")&&(document.ondragstart!=this.dragStartHandler)){
document.ondragstart=this.documentOnDragStart;
}else{
document.ondragstart="";
}
};
RadGridNamespace.AddStyleSheet=function(_75){
if(RadGridNamespace.StyleSheets==null){
RadGridNamespace.StyleSheets={};
}
var _76=RadGridNamespace.StyleSheets[_75];
if(_76!=null){
return null;
}
var css=null;
var _78=null;
var _79=document.getElementsByTagName("head")[0];
if(window.netscape||navigator.userAgent.indexOf("Safari")!=-1){
css=document.createElement("style");
css.media="all";
css.type="text/css";
_79.appendChild(css);
}else{
try{
css=document.createStyleSheet();
}
catch(e){
return false;
}
}
var _7a=document.styleSheets[document.styleSheets.length-1];
RadGridNamespace.StyleSheets[_75]=_7a;
return _7a;
};
RadGridNamespace.AddRule=function(ss,_7c,_7d){
try{
if(!ss){
return false;
}
if(ss.insertRule&&navigator.userAgent.indexOf("Safari")==-1){
var _7e=ss.insertRule(_7c+" {"+_7d+"}",ss.cssRules.length);
return ss.cssRules[ss.cssRules.length-1];
}
if(navigator.userAgent.indexOf("Safari")!=-1){
ss.addRule(_7c,_7d);
return ss.cssRules[ss.cssRules.length-1];
}
if(ss.addRule){
ss.addRule(_7c,_7d);
return true;
}
return false;
}
catch(e){
return false;
}
};
RadGridNamespace.addClassName=function(_7f,_80){
var s=_7f.className;
var p=s.split(" ");
if(p.length==1&&p[0]==""){
p=[];
}
var l=p.length;
for(var i=0;i<l;i++){
if(p[i]==_80){
return;
}
}
p[p.length]=_80;
_7f.className=p.join(" ");
};
RadGridNamespace.removeClassName=function(_85,_86){
if(_85.className.replace(/^\s*|\s*$/g,"")==_86){
_85.className="";
return;
}
var _87=_85.className.split(" ");
var _88=[];
for(var i=0,l=_87.length;i<l;i++){
if(_87[i]==""){
continue;
}
if(_86.indexOf(_87[i])==-1){
_88[_88.length]=_87[i];
}
}
_85.className=_88.join(" ");
return;
_85.className=(_85.className.toString()==_86)?"":_85.className.replace(_86,"").replace(/\s*$/g,"");
return;
var p=s.split(" ");
var np=[];
var l=p.length;
var j=0;
for(var i=0;i<l;i++){
if(p[i]!=_86){
np[j++]=p[i];
}
}
_85.className=np.join(" ");
};
RadGridNamespace.CheckIsParentDisplay=function(_8e){
try{
while(_8e){
if(_8e.style){
if(_8e.currentStyle){
if(_8e.currentStyle.display=="none"){
return false;
}
}else{
if(_8e.style.display=="none"){
return false;
}
}
}
_8e=_8e.parentNode;
}
if(window.top){
if(window.top.location!=window.location){
return false;
}
}
return true;
}
catch(e){
return false;
}
};
RadGridNamespace.EncodeURI=function(_8f){
if(encodeURI){
return encodeURI(_8f);
}else{
return escape(_8f);
}
};
if(typeof (window.RadControlsNamespace)=="undefined"){
window.RadControlsNamespace=new Object();
}
RadControlsNamespace.AppendStyleSheet=function(_90,_91,_92){
if(!_92){
return;
}
if(!_90){
document.write("<"+"link"+" rel='stylesheet' type='text/css' href='"+_92+"' />");
}else{
var _93=document.createElement("link");
_93.rel="stylesheet";
_93.type="text/css";
_93.href=_92;
var _94=document.getElementById(_91+"StyleSheetHolder");
if(_94!=null){
document.getElementById(_91+"StyleSheetHolder").appendChild(_93);
}
}
};
RadGridNamespace.RadGrid=function(_95){
var _96=window[_95.ClientID];
if(_96!=null&&typeof (_96.Dispose)=="function"){
window.setTimeout(function(){
_96.Dispose();
},100);
}
RadControlsNamespace.DomEventMixin.Initialize(this);
this.AttachDomEvent(window,"unload","OnWindowUnload");
window[_95.ClientID]=this;
window["grid_"+_95.ClientID]=this;
if(RadGridNamespace.DocumentCanBeModified()){
this._constructor(_95);
}else{
this.objectData=_95;
this.AttachDomEvent(window,"load","OnWindowLoad");
}
};
RadGridNamespace.DocumentCanBeModified=function(){
return (RadGridNamespace.DocumentHasBeenFullyLoaded==true)||(document.readyState=="complete")||window.opera||window.netscape;
};
RadGridNamespace.RadGrid.prototype.OnWindowUnload=function(e){
this.Dispose();
};
RadGridNamespace.RadGrid.prototype.OnWindowLoad=function(e){
RadGridNamespace.DocumentHasBeenFullyLoaded=true;
this._constructor(this.objectData);
this.objectData=null;
};
RadGridNamespace.RadGrid.prototype._constructor=function(_99){
this.Type="RadGrid";
if(_99.ClientSettings){
this.InitializeEvents(_99.ClientSettings.ClientEvents);
}
RadGridNamespace.FireEvent(this,"OnGridCreating");
for(var _9a in _99){
this[_9a]=_99[_9a];
}
this.Initialize();
RadGridNamespace.FireEvent(this,"OnMasterTableViewCreating");
this.GridStyleSheet=RadGridNamespace.AddStyleSheet(this.ClientID);
if(this.ClientSettings&&this.ClientSettings.Scrolling.AllowScroll&&this.ClientSettings.Scrolling.UseStaticHeaders){
var ID=_99.MasterTableView.ClientID;
_99.MasterTableView.ClientID=ID+"_Header";
this.MasterTableViewHeader=new RadGridNamespace.RadGridTable(_99.MasterTableView);
this.MasterTableViewHeader._constructor(this);
if(document.getElementById(ID+"_Footer")){
_99.MasterTableView.ClientID=ID+"_Footer";
this.MasterTableViewFooter=new RadGridNamespace.RadGridTable(_99.MasterTableView);
this.MasterTableViewFooter._constructor(this);
}
_99.MasterTableView.ClientID=ID;
}
this.MasterTableView._constructor(this);
RadGridNamespace.FireEvent(this,"OnMasterTableViewCreated");
this.DetailTablesCollection=new Array();
this.LoadDetailTablesCollection(this.MasterTableView,1);
this.AttachDomEvents();
RadGridNamespace.FireEvent(this,"OnGridCreated");
this.InitializeFeatures(_99);
if(typeof (window.event)=="undefined"){
window.event=null;
}
};
RadGridNamespace.RadGrid.prototype.Dispose=function(){
if(this.Disposed){
return;
}
this.Disposed=true;
try{
RadGridNamespace.FireEvent(this,"OnGridDestroying");
this.DisposeDomEventHandlers();
this.DisposeEvents();
this.GridStyleSheet=null;
this.DisposeFeatures();
this.DisposeDetailTablesCollection(this.MasterTableView,1);
if(this.MasterTableViewHeader!=null){
this.MasterTableViewHeader.Dispose();
}
if(this.MasterTableViewFooter!=null){
this.MasterTableViewFooter.Dispose();
}
if(this.MasterTableView!=null){
this.MasterTableView.Dispose();
}
this.DisposeProperties();
}
catch(error){
}
};
RadGridNamespace.RadGrid.ClientEventNames={OnGridCreating:true,OnGridCreated:true,OnGridDestroying:true,OnMasterTableViewCreating:true,OnMasterTableViewCreated:true,OnTableCreating:true,OnTableCreated:true,OnTableDestroying:true,OnScroll:true,OnKeyPress:true,OnRequestStart:true,OnRequestEnd:true,OnRequestError:true,OnError:true,OnRowDeleting:true,OnRowDeleted:true};
RadGridNamespace.RadGrid.prototype.IsClientEventName=function(_9c){
return RadGridNamespace.RadGrid.ClientEventNames[_9c]==true;
};
RadGridNamespace.RadGrid.prototype.InitializeEvents=function(_9d){
for(var _9e in _9d){
if(typeof (_9d[_9e])!="string"){
continue;
}
if(this.IsClientEventName(_9e)){
if(_9d[_9e]!=""){
var _9f=_9d[_9e];
if(_9f.indexOf("(")!=-1){
this[_9e]=_9f;
}else{
this[_9e]=eval(_9f);
}
}else{
this[_9e]=null;
}
}
}
};
RadGridNamespace.RadGrid.prototype.DisposeEvents=function(){
for(var _a0 in RadGridNamespace.RadGrid.ClientEventNames){
this[_a0]=null;
}
};
RadGridNamespace.RadGrid.prototype.GetDetailTable=function(_a1,_a2){
if(_a1.HierarchyIndex==_a2){
return _a1;
}
if(_a1.DetailTables){
for(var i=0;i<_a1.DetailTables.length;i++){
var res=this.GetDetailTable(_a1.DetailTables[i],_a2);
if(res){
return res;
}
}
}
};
RadGridNamespace.RadGrid.prototype.LoadDetailTablesCollection=function(_a5,_a6){
try{
if(_a5.Controls[0]!=null&&_a5.Controls[0].Rows!=null){
for(var i=0;i<_a5.Controls[0].Rows.length;i++){
var _a8=_a5.Controls[0].Rows[i].ItemType;
if(_a8=="NestedView"){
var _a9=_a5.Controls[0].Rows[i].NestedTableViews;
for(var j=0;j<_a9.length;j++){
var _ab=_a9[j];
if(_ab.Visible){
var _ac=this.GetDetailTable(this.MasterTableView,_ab.HierarchyIndex);
RadGridNamespace.FireEvent(this,"OnTableCreating",[_ac]);
_ab._constructor(this);
this.DetailTablesCollection[this.DetailTablesCollection.length]=_ab;
if(_ab.AllowFilteringByColumn){
this.InitializeFilterMenu(_ab);
}
RadGridNamespace.FireEvent(this,"OnTableCreated",[_ab]);
}
this.LoadDetailTablesCollection(_ab,_a6+1);
}
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DisposeDetailTablesCollection=function(_ad,_ae){
if(_ad.Controls[0]!=null&&_ad.Controls[0].Rows!=null){
for(var i=0;i<_ad.Controls[0].Rows.length;i++){
var _b0=_ad.Controls[0].Rows[i].ItemType;
if(_b0=="NestedView"){
var _b1=_ad.Controls[0].Rows[i].NestedTableViews;
for(var j=0;j<_b1.length;j++){
var _b3=_b1[j];
_b3.Dispose();
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.AddRtlClass=function(){
if(RadGridNamespace.IsRightToLeft(this.Control)){
RadGridNamespace.addClassName(this.Control,"RadGridRTL_"+this.Skin);
}
};
RadGridNamespace.RadGrid.prototype.Initialize=function(){
this.Control=document.getElementById(this.ClientID);
if(this.Control==null){
return;
}
this.Control.tabIndex=0;
this.AddRtlClass();
this.GridDataDiv=document.getElementById(this.ClientID+"_GridData");
if(this.GroupPanel){
this.GroupPanelControl=document.getElementById(this.GroupPanel.ClientID+"_GroupPanel");
}
this.GridHeaderDiv=document.getElementById(this.ClientID+"_GridHeader");
this.GridFooterDiv=document.getElementById(this.ClientID+"_GridFooter");
this.PostDataValue=document.getElementById(this.ClientID+"PostDataValue");
this.LoadingTemplate=document.getElementById(this.ClientID+"_LoadingTemplate");
this.PagerControl=document.getElementById(this.MasterTableView.ClientID+"_Pager");
this.TopPagerControl=document.getElementById(this.MasterTableView.ClientID+"_TopPager");
if(this.LoadingTemplate){
this.LoadingTemplate.style.display="none";
if(this.GridDataDiv){
this.GridDataDiv.appendChild(this.LoadingTemplate);
}
}
};
RadGridNamespace.RadGrid.prototype.DisposeProperties=function(){
this.Control=null;
this.GridDataDiv=null;
this.GroupPanelControl=null;
this.GridHeaderDiv=null;
this.GridFooterDiv=null;
this.PostDataValue=null;
this.LoadingTemplate=null;
this.PagerControl=null;
};
RadGridNamespace.RadGrid.prototype.InitializeFeatures=function(_b4){
if(!this.MasterTableView.Control){
return;
}
if(this.GroupPanelControl!=null){
this.GroupPanelObject=new RadGridNamespace.RadGridGroupPanel(this.GroupPanelControl,this);
}
if(this.ClientSettings&&this.ClientSettings.Scrolling.AllowScroll){
this.InitializeDimensions();
this.InitializeScroll();
}
if(this.AllowFilteringByColumn||this.MasterTableView.AllowFilteringByColumn){
var _b5=(this.MasterTableViewHeader)?this.MasterTableViewHeader:this.MasterTableView;
this.InitializeFilterMenu(_b5);
}
if(this.ClientSettings&&this.ClientSettings.AllowKeyboardNavigation&&this.MasterTableView.Rows){
if(!this.MasterTableView.RenderActiveItemStyleClass||this.MasterTableView.RenderActiveItemStyleClass==""){
if(this.MasterTableView.RenderActiveItemStyle&&this.MasterTableView.RenderActiveItemStyle!=""){
RadGridNamespace.AddRule(this.GridStyleSheet,".ActiveItemStyle"+this.MasterTableView.ClientID+"1 td",this.MasterTableView.RenderActiveItemStyle);
}else{
RadGridNamespace.AddRule(this.GridStyleSheet,".ActiveItemStyle"+this.MasterTableView.ClientID+"2 td","background-color:#FFA07A;");
}
}
if(this.ActiveRow==null){
this.ActiveRow=this.MasterTableView.Rows[0];
}
this.SetActiveRow(this.ActiveRow);
}
if(this.ClientSettings&&this.ClientSettings.Slider!=null&&this.ClientSettings.Slider!=""){
eval(this.ClientSettings.Slider);
}
if(window[this.ClientID+"_Slider"]){
this.Slider=new RadGridNamespace.Slider(window[this.ClientID+"_Slider"]);
}
};
RadGridNamespace.RadGrid.prototype.DisposeFeatures=function(){
if(this.Slider!=null){
this.Slider.Dispose();
this.Slider=null;
}
if(this.GroupPanelControl!=null){
this.GroupPanelObject.Dispose();
this.GroupPanelControl=null;
}
if(this.AllowFilteringByColumn||this.MasterTableView.AllowFilteringByColumn){
var _b6=(this.MasterTableViewHeader)?this.MasterTableViewHeader:this.MasterTableView;
this.DisposeFilterMenu(_b6);
}
this.Control=null;
};
RadGridNamespace.RadGrid.prototype.AsyncRequest=function(_b7,_b8,e){
var _ba;
if(this.StatusBarSettings!=null&&this.StatusBarSettings.StatusLabelID!=null&&this.StatusBarSettings.StatusLabelID!=""){
var _bb=document.getElementById(this.StatusBarSettings.StatusLabelID);
if(_bb!=null){
_ba=_bb.innerHTML;
_bb.innerHTML=this.StatusBarSettings.LoadingText;
}
}
var _bc=this.ClientID;
this.OnRequestEndInternal=function(){
RadGridNamespace.FireEvent(window[_bc],"OnRequestEnd");
if(_bb){
_bb.innerHTML=_ba;
}
};
RadAjaxNamespace.AsyncRequest(_b7,_b8,_bc,e);
};
RadGridNamespace.RadGrid.prototype.AjaxRequest=function(_bd,_be){
this.AsyncRequest(_bd,_be);
};
RadGridNamespace.RadGrid.prototype.ClearSelectedRows=function(){
for(var i=0;i<this.DetailTablesCollection.length;i++){
var _c0=this.DetailTablesCollection[i];
_c0.ClearSelectedRows();
}
this.MasterTableView.ClearSelectedRows();
};
RadGridNamespace.RadGrid.prototype.AsyncRequestWithOptions=function(_c1,e){
RadAjaxNamespace.AsyncRequestWithOptions(_c1,this.ClientID,e);
};
RadGridNamespace.RadGrid.prototype.DeleteRow=function(_c3,_c4,e){
var _c6=(e.srcElement)?e.srcElement:e.target;
if(!_c6){
return;
}
var row=_c6.parentNode.parentNode;
var _c8=row.parentNode.parentNode;
var _c9=row.rowIndex;
var _ca=row.cells.length;
var _cb=this.GetTableObjectByID(_c3);
var _cc=this.GetRowObjectByRealRow(_cb,row);
var _cd={Row:_cc};
if(!RadGridNamespace.FireEvent(this,"OnRowDeleting",[_cb,_cd])){
return;
}
_c8.deleteRow(row.rowIndex);
for(var i=_c9;i<_c8.rows.length;i++){
if(_c8.rows[i].cells.length!=_ca&&_c8.rows[i].style.display!="none"){
_c8.deleteRow(i);
i--;
}else{
break;
}
}
if(_c8.tBodies[0].rows.length==1&&_c8.tBodies[0].rows[0].style.display=="none"){
_c8.tBodies[0].rows[0].style.display="";
}
this.PostDataValue.value+="DeletedRows,"+_c3+","+_c4+";";
RadGridNamespace.FireEvent(this,"OnRowDeleted",[_cb,_cd]);
};
RadGridNamespace.RadGrid.prototype.SelectRow=function(_cf,_d0,e){
var _d2=(e.srcElement)?e.srcElement:e.target;
if(!_d2){
return;
}
var row=RadGridNamespace.GetFirstParentByTagName(_d2,"tr");
var _d4=RadGridNamespace.GetFirstParentByTagName(row,"table");
var _d5=row.rowIndex;
var _d6;
if(_cf==this.MasterTableView.UID){
_d6=this.MasterTableView;
}else{
for(var i=0;i<this.DetailTablesCollection.length;i++){
if(this.DetailTablesCollection[i].ClientID==_d4.id){
_d6=this.DetailTablesCollection[i];
break;
}
}
}
if(_d6!=null){
if(this.AllowMultiRowSelection){
_d6.SelectRow(row,false);
}else{
_d6.SelectRow(row,true);
}
}
};
RadGridNamespace.RadGrid.prototype.SelectAllRows=function(_d8,_d9,e){
var _db=(e.srcElement)?e.srcElement:e.target;
if(!_db){
return;
}
var row=_db.parentNode.parentNode;
var _dd=row.parentNode.parentNode;
var _de=row.rowIndex;
var _df;
if(_d8==this.MasterTableView.UID){
_df=this.MasterTableView;
}else{
for(var i=0;i<this.DetailTablesCollection.length;i++){
if(this.DetailTablesCollection[i].UID==_d8){
_df=this.DetailTablesCollection[i];
break;
}
}
}
if(_df!=null){
if(this.AllowMultiRowSelection){
if(_df==this.MasterTableViewHeader){
_df=this.MasterTableView;
}
_df.ClearSelectedRows();
if(_db.checked){
for(var i=0;i<_df.Control.tBodies[0].rows.length;i++){
var row=_df.Control.tBodies[0].rows[i];
_df.SelectRow(row,false);
}
}else{
for(var i=0;i<_df.Control.tBodies[0].rows.length;i++){
var row=_df.Control.tBodies[0].rows[i];
_df.DeselectRow(row);
}
this.UpdateClientRowSelection();
}
}
}
};
RadGridNamespace.RadGrid.prototype.UpdateClientRowSelection=function(){
var _e1=this.MasterTableView.GetSelectedRowsIndexes();
this.SavePostData("SelectedRows",this.MasterTableView.ClientID,_e1);
for(var i=0;i<this.DetailTablesCollection.length;i++){
_e1=this.DetailTablesCollection[i].GetSelectedRowsIndexes();
this.SavePostData("SelectedRows",this.DetailTablesCollection[i].ClientID,_e1);
}
};
RadGridNamespace.RadGrid.prototype.HandleActiveRow=function(e){
if((this.AllowRowResize)||(this.AllowRowSelect)){
var _e4=this.GetCellFromPoint(e);
if((_e4!=null)&&(_e4.parentNode.id!="")&&(_e4.parentNode.id!=-1)&&(_e4.cellIndex==0)){
var _e5=_e4.parentNode.parentNode.parentNode;
this.SetActiveRow(_e5,_e4.parentNode.rowIndex);
}
}
};
RadGridNamespace.RadGrid.prototype.SetActiveRow=function(_e6){
if(_e6==null){
return;
}
if(_e6.Owner.RenderActiveItemStyle){
RadGridNamespace.removeClassName(this.ActiveRow.Control,"ActiveItemStyle"+_e6.Owner.ClientID+"1");
}else{
RadGridNamespace.removeClassName(this.ActiveRow.Control,"ActiveItemStyle"+_e6.Owner.ClientID+"2");
}
RadGridNamespace.removeClassName(this.ActiveRow.Control,_e6.Owner.RenderActiveItemStyleClass);
if(this.ActiveRow.Control.style.cssText==_e6.Owner.RenderActiveItemStyle){
this.ActiveRow.Control.style.cssText="";
}
this.ActiveRow=_e6;
if(!this.ActiveRow.Owner.RenderActiveItemStyleClass||this.ActiveRow.Owner.RenderActiveItemStyleClass==""){
if(this.ActiveRow.Owner.RenderActiveItemStyle&&this.ActiveRow.Owner.RenderActiveItemStyle!=""){
RadGridNamespace.addClassName(this.ActiveRow.Control,"ActiveItemStyle"+this.ActiveRow.Owner.ClientID+"1");
}else{
RadGridNamespace.addClassName(this.ActiveRow.Control,"ActiveItemStyle"+this.ActiveRow.Owner.ClientID+"2");
}
}else{
RadGridNamespace.addClassName(this.ActiveRow.Control,this.ActiveRow.Owner.RenderActiveItemStyleClass);
}
this.SavePostData("ActiveRow",this.ActiveRow.Owner.ClientID,this.ActiveRow.RealIndex);
};
RadGridNamespace.RadGrid.prototype.GetNextRow=function(_e7,_e8){
if(_e7!=null){
if(_e7.tBodies[0].rows[_e8]!=null){
while(_e7.tBodies[0].rows[_e8]!=null){
_e8++;
if(_e8<=(_e7.tBodies[0].rows.length-1)){
return _e7.tBodies[0].rows[_e8];
}else{
return null;
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.GetPreviousRow=function(_e9,_ea){
if(_e9!=null){
if(_e9.tBodies[0].rows[_ea]!=null){
while(_e9.tBodies[0].rows[_ea]!=null){
_ea--;
if(_ea>=0){
return _e9.tBodies[0].rows[_ea];
}else{
return null;
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.GetNextHierarchicalRow=function(_eb,_ec){
if(_eb!=null){
if(_eb.tBodies[0].rows[_ec]!=null){
_ec++;
var row=_eb.tBodies[0].rows[_ec];
if(_eb.tBodies[0].rows[_ec]!=null){
if((row.cells[1]!=null)&&(row.cells[2]!=null)){
if((row.cells[1].getElementsByTagName("table").length>0)||(row.cells[2].getElementsByTagName("table").length>0)){
var _ee=this.GetNextRow(row.cells[2].firstChild,0);
return _ee;
}else{
return null;
}
}
}
}
}
};
RadGridNamespace.RadGrid.prototype.GetPreviousHierarchicalRow=function(_ef,_f0){
if(_ef!=null){
if(_ef.parentNode!=null){
if(_ef.parentNode.tagName.toLowerCase()=="td"){
var _f1=_ef.parentNode.parentNode.parentNode.parentNode;
var _f2=_ef.parentNode.parentNode.rowIndex;
return this.GetPreviousRow(_f1,_f2);
}else{
return null;
}
}else{
return this.GetPreviousRow(_ef,_f0);
}
}
};
RadGridNamespace.RadGrid.prototype.HandleCellEdit=function(e){
var _f4=RadGridNamespace.GetCurrentElement(e);
var _f5=RadGridNamespace.GetFirstParentByTagName(_f4,"td");
if(_f5!=null){
_f4=_f5;
var _f6=_f4.parentNode.parentNode.parentNode;
var _f7=this.GetTableObjectByID(_f6.id);
if((_f7!=null)&&(_f7.Columns.length>0)&&(_f7.Columns[_f4.cellIndex]!=null)){
if(_f7.Columns[_f4.cellIndex].ColumnType!="GridBoundColumn"){
return;
}
this.EditedCell=_f7.Control.rows[_f4.parentNode.rowIndex].cells[_f4.cellIndex];
this.CellEditor=new RadGridNamespace.RadGridCellEditor(this.EditedCell,_f7.Columns[_f4.cellIndex],this);
}
}
};
RadGridNamespace.RadGridCellEditor=function(_f8,_f9,_fa){
if(_fa.CellEditor){
return;
}
this.Control=document.createElement("input");
this.Control.style.border="1px groove";
this.Control.style.width="100%";
this.Control.value=_f8.innerHTML;
this.OldValue=this.Control.value;
_f8.innerHTML="";
var _fb=this;
this.Control.onblur=function(e){
if(!e){
var e=window.event;
}
_f8.removeChild(this);
_f8.innerHTML=this.value;
if(this.value!=_fb.OldValue){
alert(1);
}
_fa.CellEditor=null;
};
_f8.appendChild(this.Control);
if(this.Control.focus){
this.Control.focus();
}
};
if(!("console" in window)||!("firebug" in console)){
var names=["log","debug","info","warn","error","assert","dir","dirxml","group","groupEnd","time","timeEnd","count","trace","profile","profileEnd"];
window.console={};
for(var i=0;i<names.length;++i){
window.console[names[i]]=function(){
};
}
}
RadGridNamespace.Error=function(_fd,_fe,_ff){
if((!_fd)||(!_fe)||(!_ff)){
return false;
}
this.Message=_fd.message;
if(_ff!=null){
if("string"==typeof (_ff)){
try{
eval(_ff);
}
catch(e){
var _100="";
_100="";
_100+="Telerik RadGrid Error:\r\n";
_100+="-----------------\r\n";
_100+="Message: \""+e.message+"\"\r\n";
_100+="Raised by: "+_fe.Type+"\r\n";
alert(_100);
}
}else{
if("function"==typeof (_ff)){
try{
_ff(this);
}
catch(e){
var _100="";
_100="";
_100+="Telerik RadGrid Error:\r\n";
_100+="-----------------\r\n";
_100+="Message: \""+e.message+"\"\r\n";
_100+="Raised by: "+_fe.Type+"\r\n";
alert(_100);
}
}
}
}else{
this.Owner=_fe;
for(var _101 in _fd){
this[_101]=_fd[_101];
}
this.Message="";
this.Message+="Telerik RadGrid Error:\r\n";
this.Message+="-----------------\r\n";
this.Message+="Message: \""+_fd.message+"\"\r\n";
this.Message+="Raised by: "+_fe.Type+"\r\n";
alert(this.Message);
}
};
RadGridNamespace.RadGrid.prototype.GetTableObjectByID=function(id){
if(this.MasterTableView.ClientID==id||this.MasterTableView.UID==id){
return this.MasterTableView;
}else{
for(var i=0;i<this.DetailTablesCollection.length;i++){
if(this.DetailTablesCollection[i].ClientID==id||this.DetailTablesCollection[i].UID==id){
return this.DetailTablesCollection[i];
}
}
}
if(this.MasterTableViewHeader!=null){
if(this.MasterTableViewHeader.ClientID==id||this.MasterTableViewHeader.UID==id){
return table=this.MasterTableViewHeader;
}
}
};
RadGridNamespace.RadGrid.prototype.GetRowObjectByRealRow=function(_104,row){
if(_104.Rows!=null){
for(var i=0;i<_104.Rows.length;i++){
if(_104.Rows[i].Control==row){
return _104.Rows[i];
}
}
}
};
RadGridNamespace.RadGrid.prototype.SavePostData=function(){
try{
var _107=new String();
for(var i=0;i<arguments.length;i++){
_107+=arguments[i]+",";
}
_107=_107.substring(0,_107.length-1);
if(this.PostDataValue!=null){
switch(arguments[0]){
case "ReorderedColumns":
this.PostDataValue.value+=_107+";";
break;
case "HidedColumns":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="ShowedColumns"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "ShowedColumns":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="HidedColumns"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "HidedRows":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="ShowedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "ShowedRows":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="HidedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "ResizedColumns":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "ResizedRows":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "ResizedControl":
var _109=arguments[0]+","+arguments[1];
this.UpdatePostData(_107,_109);
break;
case "ClientCreated":
var _109=arguments[0]+","+arguments[1];
this.UpdatePostData(_107,_109);
break;
case "ScrolledControl":
var _109=arguments[0]+","+arguments[1];
this.UpdatePostData(_107,_109);
break;
case "AJAXScrolledControl":
var _109=arguments[0]+","+arguments[1];
this.UpdatePostData(_107,_109);
break;
case "SelectedRows":
var _109=arguments[0]+","+arguments[1]+",";
this.UpdatePostData(_107,_109);
break;
case "EditRow":
var _109=arguments[0]+","+arguments[1];
this.UpdatePostData(_107,_109);
break;
case "ActiveRow":
var _109=arguments[0]+","+arguments[1];
this.UpdatePostData(_107,_109);
break;
case "CollapsedRows":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="ExpandedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "ExpandedRows":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="CollapsedRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "CollapsedGroupRows":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="ExpandedGroupRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
case "ExpandedGroupRows":
var _109=arguments[0]+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
_109="CollapsedGroupRows"+","+arguments[1]+","+arguments[2];
this.UpdatePostData(_107,_109);
break;
default:
this.UpdatePostData(_107,_107);
break;
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.UpdatePostData=function(_10a,_10b){
var _10c,_10d=new Array();
_10c=this.PostDataValue.value.split(";");
for(var i=0;i<_10c.length;i++){
if(_10c[i].indexOf(_10b)==-1){
_10d[_10d.length]=_10c[i];
}
}
this.PostDataValue.value=_10d.join(";");
this.PostDataValue.value+=_10a+";";
};
RadGridNamespace.RadGrid.prototype.DeletePostData=function(_10f,_110){
var _111,_112=new Array();
_111=this.PostDataValue.value.split(";");
for(var i=0;i<_111.length;i++){
if(_111[i].indexOf(_110)==-1){
_112[_112.length]=_111[i];
}
}
this.PostDataValue.value=_112.join(";");
};
RadGridNamespace.RadGrid.prototype.HandleDragAndDrop=function(e,_115){
try{
var _116=this;
if(_117!=null&&_117.Columns.length>0&&_117.Columns[_118]!=null&&!_117.Columns[_118].Reorderable){
this.Control.style.cursor="no-drop";
this.DisableDrop();
}else{
this.Control.style.cursor="";
}
if(this.MoveHeaderDiv!=null&&_115!=null&&_115.tagName.toLowerCase()!="th"&&!RadGridNamespace.IsChildOf(_115,this.MoveHeaderDivRefCell.parentNode)&&!(this.GroupPanelControl!=null&&RadGridNamespace.IsChildOf(_115,this.GroupPanelControl))){
this.Control.style.cursor="no-drop";
this.DisableDrop();
}else{
this.Control.style.cursor="";
}
if((_115!=null)&&(_115.tagName.toLowerCase()=="th")){
var _119=_115.parentNode.parentNode.parentNode;
var _117=this.GetTableObjectByID(_119.id);
var _118=RadGridNamespace.GetRealCellIndex(_117,_115);
if((_117!=null)&&(_117.Columns.length>0)&&(_117.Columns[_118]!=null)&&((_117.Columns[_118].Reorderable)||(_117.Owner.ClientSettings.AllowDragToGroup&&_117.Columns[_118].Groupable))){
var _11a=RadGridNamespace.GetEventPosX(e);
var _11b=RadGridNamespace.FindPosX(_115);
var endX=_11b+_115.offsetWidth;
this.ResizeTolerance=5;
var _11d=_115.title;
var _11e=_115.style.cursor;
if(!((_11a>=endX-this.ResizeTolerance)&&(_11a<=endX+this.ResizeTolerance))){
if(this.MoveHeaderDiv){
if(this.MoveHeaderDiv.innerHTML!=_115.innerHTML){
_115.title=this.ClientSettings.ClientMessages.DropHereToReorder;
_115.style.cursor="default";
if(_115.parentNode.parentNode.parentNode==this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode){
this.MoveReorderIndicators(e,_115);
}else{
this.DisableDrop();
}
}
}else{
_115.title=this.ClientSettings.ClientMessages.DragToGroupOrReorder;
_115.style.cursor="move";
}
this.AttachDomEvent(_115,"mousedown","OnDragDropMouseDown");
}else{
_115.style.cursor=_11e;
_115.title="";
}
}
}
if(_117!=null&&_117.Columns.length>0&&_117.Columns[_118]!=null&&!_117.Columns[_118].Reorderable){
this.Control.style.cursor="no-drop";
this.DisableDrop();
}
if(this.MoveHeaderDiv!=null){
this.MoveHeaderDiv.style.visibility="";
this.MoveHeaderDiv.style.display="";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DisableDrop=function(e){
if(this.ReorderIndicator1!=null){
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
}
if(this.ReorderIndicator2!=null){
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
}
};
RadGridNamespace.RadGrid.PositionDragElement=function(_120,_121){
_120.style.top=_121.clientY+document.documentElement.scrollTop+document.body.scrollTop+1+"px";
_120.style.left=_121.clientX+document.documentElement.scrollLeft+document.body.scrollLeft+1+"px";
};
RadGridNamespace.RadGrid.prototype.OnDragDropMouseDown=function(e){
var _123=RadGridNamespace.GetCurrentElement(e);
var _124=false;
var form=document.getElementById(this.FormID);
if(form!=null&&form["__EVENTTARGET"]!=null&&form["__EVENTTARGET"].value!=""){
_124=true;
}
if((_123.tagName.toLowerCase()=="input"&&_123.type.toLowerCase()=="text")||(_123.tagName.toLowerCase()=="textarea")){
return;
}
_123=RadGridNamespace.GetFirstParentByTagName(_123,"th");
if(_123.tagName.toLowerCase()=="th"&&!this.IsResize){
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1)&&(e.button==0))||(e.button==1)){
this.CreateDragAndDrop(e,_123);
}
RadGridNamespace.ClearDocumentEvents();
this.DetachDomEvent(_123,"mousedown","OnDragDropMouseDown");
this.AttachDomEvent(document,"mouseup","OnDragDropMouseUp");
if(this.GroupPanelControl!=null){
this.AttachDomEvent(this.GroupPanelControl,"mouseup","OnDragDropMouseUp");
}
}
};
RadGridNamespace.RadGrid.prototype.OnDragDropMouseUp=function(e){
this.DetachDomEvent(document,"mouseup","OnDragDropMouseUp");
if(this.GroupPanelControl!=null){
this.DetachDomEvent(this.GroupPanelControl,"mouseup","OnDragDropMouseUp");
}
this.FireDropAction(e);
this.DestroyDragAndDrop(e);
RadGridNamespace.RestoreDocumentEvents();
};
RadGridNamespace.CopyAttributes=function(_127,_128){
for(var i=0;i<_128.attributes.length;i++){
try{
if(_128.attributes[i].name.toLowerCase()=="id"){
continue;
}
if(_128.attributes[i].value!=null&&_128.attributes[i].value!="null"&&_128.attributes[i].value!=""){
_127.setAttribute(_128.attributes[i].name,_128.attributes[i].value);
}
}
catch(e){
continue;
}
}
};
RadGridNamespace.RadGrid.prototype.CreateDragAndDrop=function(e,_12b){
this.MoveHeaderDivRefCell=_12b;
this.MoveHeaderDiv=document.createElement("div");
var _12c=document.createElement("table");
if(this.MoveHeaderDiv.mergeAttributes){
this.MoveHeaderDiv.mergeAttributes(this.Control);
}else{
RadGridNamespace.CopyAttributes(this.MoveHeaderDiv,this.Control);
}
if(_12c.mergeAttributes){
_12c.mergeAttributes(this.MasterTableView.Control);
}else{
RadGridNamespace.CopyAttributes(_12c,this.MasterTableView.Control);
}
_12c.style.margin="0px";
_12c.style.height=_12b.offsetHeight+"px";
_12c.style.width=_12b.offsetWidth+"px";
var _12d=document.createElement("thead");
var tr=document.createElement("tr");
_12c.appendChild(_12d);
_12d.appendChild(tr);
tr.appendChild(_12b.cloneNode(true));
this.MoveHeaderDiv.appendChild(_12c);
document.body.appendChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.style.height=_12b.offsetHeight+"px";
this.MoveHeaderDiv.style.width=_12b.offsetWidth+"px";
this.MoveHeaderDiv.style.position="absolute";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
if(window.netscape){
this.MoveHeaderDiv.style.MozOpacity=3/4;
}else{
this.MoveHeaderDiv.style.filter="alpha(opacity=75);";
}
this.MoveHeaderDiv.style.cursor="move";
this.MoveHeaderDiv.style.visibility="hidden";
this.MoveHeaderDiv.style.display="none";
this.MoveHeaderDiv.style.fontWeight="bold";
this.MoveHeaderDiv.onmousedown=null;
RadGridNamespace.ClearDocumentEvents();
if(this.ClientSettings.AllowColumnsReorder){
this.CreateReorderIndicators(_12b);
}
};
RadGridNamespace.RadGrid.prototype.DestroyDragAndDrop=function(){
if(this.MoveHeaderDiv!=null){
var _12f=this.MoveHeaderDiv.parentNode;
_12f.removeChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.onmouseup=null;
this.MoveHeaderDiv.onmousemove=null;
this.MoveHeaderDiv=null;
this.MoveHeaderDivRefCell=null;
this.DragCellIndex=null;
RadGridNamespace.RestoreDocumentEvents();
this.DestroyReorderIndicators();
}
};
RadGridNamespace.RadGrid.prototype.FireDropAction=function(e){
if((this.MoveHeaderDiv!=null)&&(this.MoveHeaderDiv.style.display!="none")){
var _131=RadGridNamespace.GetCurrentElement(e);
if((_131!=null)&&(this.MoveHeaderDiv!=null)){
if(_131!=this.MoveHeaderDivRefCell){
var _132=this.GetTableObjectByID(this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode.id);
var _133=_132.HeaderRow;
if(RadGridNamespace.IsChildOf(_131,_133)){
if(_131.tagName.toLowerCase()!="th"){
_131=RadGridNamespace.GetFirstParentByTagName(_131,"th");
}
var _134=_131.parentNode.parentNode.parentNode;
var _135=this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode;
if(_134.id==_135.id){
var _136=this.GetTableObjectByID(_134.id);
var _137=_131.cellIndex;
if(navigator.userAgent.indexOf("Safari")!=-1){
_137=RadGridNamespace.GetRealCellIndex(_136,_131);
}
var _138=this.MoveHeaderDivRefCell.cellIndex;
if(navigator.userAgent.indexOf("Safari")!=-1){
_138=RadGridNamespace.GetRealCellIndex(_136,this.MoveHeaderDivRefCell);
}
if(!_136||!_136.Columns[_137]){
return;
}
if(!_136.Columns[_137].Reorderable){
return;
}
_136.SwapColumns(_137,_138,(this.ClientSettings.ColumnsReorderMethod!="Reorder"));
if(this.ClientSettings.ColumnsReorderMethod=="Reorder"){
if((!this.ClientSettings.ReorderColumnsOnClient)&&(this.ClientSettings.PostBackReferences.PostBackColumnsReorder!="")){
eval(this.ClientSettings.PostBackReferences.PostBackColumnsReorder);
}
}
}
}else{
if(RadGridNamespace.CheckParentNodesFor(_131,this.GroupPanelControl)){
if((this.ClientSettings.PostBackReferences.PostBackGroupByColumn!="")&&(this.ClientSettings.AllowDragToGroup)){
var _136=this.GetTableObjectByID(this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode.id);
var _139=this.MoveHeaderDivRefCell.cellIndex;
_139=RadGridNamespace.GetRealCellIndex(_136,this.MoveHeaderDivRefCell);
var _13a=_136.Columns[_139].RealIndex;
if(_136.Columns[_139].Groupable){
if(_136==this.MasterTableViewHeader){
this.SavePostData("GroupByColumn",this.MasterTableView.ClientID,_13a);
}else{
this.SavePostData("GroupByColumn",_136.ClientID,_13a);
}
eval(this.ClientSettings.PostBackReferences.PostBackGroupByColumn);
}
}
}
}
}
}
}
};
RadGridNamespace.GetRealCellIndex=function(_13b,cell){
for(var i=0;i<_13b.Columns.length;i++){
if(_13b.Columns[i].Control==cell){
return i;
}
}
};
RadGridNamespace.RadGrid.prototype.CreateReorderIndicators=function(_13e){
if((this.ReorderIndicator1==null)&&(this.ReorderIndicator2==null)){
var _13f=this.MoveHeaderDivRefCell.parentNode.parentNode.parentNode;
var _140=this.GetTableObjectByID(_13f.id);
var _141=_140.HeaderRow;
if(!RadGridNamespace.IsChildOf(_13e,_141)){
return;
}
this.ReorderIndicator1=document.createElement("span");
this.ReorderIndicator2=document.createElement("span");
if(this.Skin==""||this.Skin=="None"){
this.ReorderIndicator1.innerHTML="&darr;";
this.ReorderIndicator2.innerHTML="&uarr;";
}else{
this.ReorderIndicator1.className="TopReorderIndicator_"+this.Skin;
this.ReorderIndicator2.className="BottomReorderIndicator_"+this.Skin;
this.ReorderIndicator1.style.width=this.ReorderIndicator1.style.height=this.ReorderIndicator2.style.width=this.ReorderIndicator2.style.height="10px";
}
this.ReorderIndicator1.style.backgroundColor="transparent";
this.ReorderIndicator1.style.color="darkblue";
this.ReorderIndicator1.style.font="bold 18px Arial";
this.ReorderIndicator2.style.backgroundColor=this.ReorderIndicator1.style.backgroundColor;
this.ReorderIndicator2.style.color=this.ReorderIndicator1.style.color;
this.ReorderIndicator2.style.font=this.ReorderIndicator1.style.font;
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_13e)-this.ReorderIndicator1.offsetHeight+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_13e)+"px";
this.ReorderIndicator2.style.top=RadGridNamespace.FindPosY(_13e)+_13e.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
document.body.appendChild(this.ReorderIndicator1);
document.body.appendChild(this.ReorderIndicator2);
}
};
RadGridNamespace.RadGrid.prototype.DestroyReorderIndicators=function(){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
document.body.removeChild(this.ReorderIndicator1);
document.body.removeChild(this.ReorderIndicator2);
this.ReorderIndicator1=null;
this.ReorderIndicator2=null;
}
};
RadGridNamespace.RadGrid.prototype.MoveReorderIndicators=function(e,_143){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
this.ReorderIndicator1.style.visibility="visible";
this.ReorderIndicator1.style.display="";
this.ReorderIndicator2.style.visibility="visible";
this.ReorderIndicator2.style.display="";
var _144=0;
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1){
_144=parseInt(document.body.currentStyle.marginTop);
}
_144-=(this.Skin==""||this.Skin=="None"&&navigator.userAgent.indexOf("Safari")==-1)?10:5;
if(navigator.userAgent.indexOf("Safari")!=-1){
_144-=10;
}
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_143)-this.ReorderIndicator1.offsetHeight/2+_144+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_143)-RadGridNamespace.FindScrollPosX(_143)+document.documentElement.scrollLeft+document.body.scrollLeft+"px";
if(parseInt(this.ReorderIndicator1.style.left)<RadGridNamespace.FindPosX(this.Control)){
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(this.Control)+5;
}
this.ReorderIndicator2.style.top=parseInt(this.ReorderIndicator1.style.top)+_143.offsetHeight+this.ReorderIndicator2.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
this.ReorderIndicator2.style.zIndex=this.ReorderIndicator1.style.zIndex=99999;
}
};
RadGridNamespace.RadGrid.prototype.AttachDomEvents=function(){
try{
this.AttachDomEvent(this.Control,"mousemove","OnMouseMove");
this.AttachDomEvent(this.Control,"keydown","OnKeyDown");
this.AttachDomEvent(this.Control,"keyup","OnKeyUp");
this.AttachDomEvent(this.Control,"click","OnClick");
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.OnMouseMove=function(e){
var _146=RadGridNamespace.GetCurrentElement(e);
if(this.ClientSettings&&this.ClientSettings.Resizing.AllowRowResize){
this.DetectResizeCursorsOnRows(e,_146);
this.MoveRowResizer(e);
}
if(this.ClientSettings&&(this.ClientSettings.AllowDragToGroup||this.ClientSettings.AllowColumnsReorder)){
this.HandleDragAndDrop(e,_146);
}
};
RadGridNamespace.RadGrid.prototype.OnKeyDown=function(e){
var _148={KeyCode:e.keyCode,IsShiftPressed:e.shiftKey,IsCtrlPressed:e.ctrlKey,IsAltPressed:e.altKey,Event:e};
if(!RadGridNamespace.FireEvent(this,"OnKeyPress",[_148])){
return;
}
if(e.keyCode==16){
this.IsShiftPressed=true;
}
if(e.keyCode==17){
this.IsCtrlPressed=true;
}
if(this.ClientSettings&&this.ClientSettings.AllowKeyboardNavigation){
this.ActiveRow.HandleActiveRow(e);
}
if(e.keyCode==27&&this.MoveHeaderDiv){
this.DestroyDragAndDrop();
}
};
RadGridNamespace.RadGrid.prototype.OnClick=function(e){
};
RadGridNamespace.RadGrid.prototype.OnKeyUp=function(e){
if(e.keyCode==16){
this.IsShiftPressed=false;
}
if(e.keyCode==17){
this.IsCtrlPressed=false;
}
};
RadGridNamespace.RadGrid.prototype.DetectResizeCursorsOnRows=function(e,_14c){
try{
var _14d=this;
if((_14c!=null)&&(_14c.tagName.toLowerCase()=="td")&&!this.MoveHeaderDiv){
var _14e=_14c.parentNode.parentNode.parentNode;
var _14f=this.GetTableObjectByID(_14e.id);
if(_14f!=null){
if(_14f.Columns!=null){
if(_14f.Columns[_14c.cellIndex].ColumnType!="GridRowIndicatorColumn"){
return;
}
}
if(!_14f.Control.tBodies[0]){
return;
}
var _150=this.GetRowObjectByRealRow(_14f,_14c.parentNode);
if(_150!=null){
var _151=RadGridNamespace.GetEventPosY(e);
var _152=RadGridNamespace.FindPosY(_14c);
var endY=_152+_14c.offsetHeight;
this.ResizeTolerance=5;
var _154=_14c.title;
if((_151>endY-this.ResizeTolerance)&&(_151<endY+this.ResizeTolerance)){
_14c.style.cursor="n-resize";
_14c.title=this.ClientSettings.ClientMessages.DragToResize;
this.AttachDomEvent(_14c,"mousedown","OnResizeMouseDown");
}else{
_14c.style.cursor="default";
_14c.title="";
this.DetachDomEvent(_14c,"mousedown","OnResizeMouseDown");
}
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.OnResizeMouseDown=function(e){
this.CreateRowResizer(e);
RadGridNamespace.ClearDocumentEvents();
this.AttachDomEvent(document,"mouseup","OnResizeMouseUp");
};
RadGridNamespace.RadGrid.prototype.OnResizeMouseUp=function(e){
this.DetachDomEvent(document,"mouseup","OnResizeMouseUp");
this.DestroyRowResizerAndResizeRow(e,true);
RadGridNamespace.RestoreDocumentEvents();
};
RadGridNamespace.RadGrid.prototype.CreateRowResizer=function(e){
try{
this.DestroyRowResizer();
var _158=RadGridNamespace.GetCurrentElement(e);
if((_158!=null)&&(_158.tagName.toLowerCase()=="td")){
if(_158.cellIndex>0){
var _159=_158.parentNode.rowIndex;
_158=_158.parentNode.parentNode.parentNode.rows[_159].cells[0];
}
this.RowResizer=null;
this.CellToResize=_158;
var _15a=_158.parentNode.parentNode.parentNode;
var _15b=this.GetTableObjectByID(_15a.id);
this.RowResizer=document.createElement("div");
this.RowResizer.style.backgroundColor="navy";
this.RowResizer.style.height="1px";
this.RowResizer.style.fontSize="1";
this.RowResizer.style.position="absolute";
this.RowResizer.style.cursor="n-resize";
if(_15b!=null){
this.RowResizerRefTable=_15b;
if(this.GridDataDiv){
this.RowResizer.style.left=RadGridNamespace.FindPosX(this.GridDataDiv)+"px";
var _15c=(RadGridNamespace.FindPosX(this.GridDataDiv)+this.GridDataDiv.offsetWidth)-parseInt(this.RowResizer.style.left);
if(_15c>_15b.Control.offsetWidth){
this.RowResizer.style.width=_15b.Control.offsetWidth+"px";
}else{
this.RowResizer.style.width=_15c+"px";
}
if(parseInt(this.RowResizer.style.width)>this.GridDataDiv.offsetWidth){
this.RowResizer.style.width=this.GridDataDiv.offsetWidth+"px";
}
}else{
this.RowResizer.style.width=_15b.Control.offsetWidth+"px";
this.RowResizer.style.left=RadGridNamespace.FindPosX(_158)+"px";
}
}
this.RowResizer.style.top=RadGridNamespace.GetEventPosY(e)-(RadGridNamespace.GetEventPosY(e)-e.clientY)+document.body.scrollTop+document.documentElement.scrollTop+"px";
var _15d=document.body;
_15d.appendChild(this.RowResizer);
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DestroyRowResizerAndResizeRow=function(e,_15f){
try{
if((this.CellToResize!="undefined")&&(this.CellToResize!=null)&&(this.CellToResize.tagName.toLowerCase()=="td")&&(this.RowResizer!="undefined")&&(this.RowResizer!=null)){
var _160;
if(this.GridDataDiv){
_160=parseInt(this.RowResizer.style.top)+this.GridDataDiv.scrollTop-(RadGridNamespace.FindPosY(this.CellToResize));
}else{
_160=parseInt(this.RowResizer.style.top)-(RadGridNamespace.FindPosY(this.CellToResize));
}
if(_160>0){
var _161=this.CellToResize.parentNode.parentNode.parentNode;
var _162=this.GetTableObjectByID(_161.id);
if(_162!=null){
_162.ResizeRow(this.CellToResize.parentNode.rowIndex,_160);
}
}
}
if(_15f){
this.DestroyRowResizer();
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.DestroyRowResizer=function(){
try{
if((this.RowResizer!="undefined")&&(this.RowResizer!=null)&&(this.RowResizer.parentNode!=null)){
var _163=this.RowResizer.parentNode;
_163.removeChild(this.RowResizer);
this.RowResizer=null;
this.RowResizerRefTable=null;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.MoveRowResizer=function(e){
try{
if((this.RowResizer!="undefined")&&(this.RowResizer!=null)&&(this.RowResizer.parentNode!=null)){
this.RowResizer.style.top=RadGridNamespace.GetEventPosY(e)-(RadGridNamespace.GetEventPosY(e)-e.clientY)+document.body.scrollTop+document.documentElement.scrollTop+"px";
if(this.ClientSettings.Resizing.EnableRealTimeResize){
this.DestroyRowResizerAndResizeRow(e,false);
this.UpdateRowResizerWidth(e);
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.UpdateRowResizerWidth=function(e){
var _166=RadGridNamespace.GetCurrentElement(e);
if((_166!=null)&&(_166.tagName.toLowerCase()=="td")){
var _167=this.RowResizerRefTable;
if(_167!=null){
if(this.GridDataDiv){
var _168=(RadGridNamespace.FindPosX(this.GridDataDiv)+this.GridDataDiv.offsetWidth)-parseInt(this.RowResizer.style.left);
if(_168>_167.Control.offsetWidth){
this.RowResizer.style.width=_167.Control.offsetWidth+"px";
}else{
this.RowResizer.style.width=_168+"px";
}
if(parseInt(this.RowResizer.style.width)>this.GridDataDiv.offsetWidth){
this.RowResizer.style.width=this.GridDataDiv.offsetWidth+"px";
}
}else{
this.RowResizer.style.width=_167.Control.offsetWidth+"px";
}
}
}
};
RadGridNamespace.RadGrid.prototype.SetHeaderAndFooterDivsWidth=function(){
if((document.compatMode=="BackCompat"&&navigator.userAgent.toLowerCase().indexOf("msie")!=-1)||(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1)){
if(this.ClientSettings.Scrolling.UseStaticHeaders){
if(this.GridHeaderDiv!=null&&this.GridDataDiv!=null&&this.GridHeaderDiv!=null){
this.GridHeaderDiv.style.width="100%";
if(this.GridHeaderDiv&&this.GridDataDiv){
if(this.GridDataDiv.offsetWidth>0){
this.GridHeaderDiv.style.width=this.GridDataDiv.offsetWidth-RadGridNamespace.GetScrollBarHeight()+"px";
}
}
if(this.GridHeaderDiv&&this.GridFooterDiv){
this.GridFooterDiv.style.width=this.GridHeaderDiv.style.width;
}
}
}
}
if(this.ClientSettings.Scrolling.AllowScroll&&this.ClientSettings.Scrolling.UseStaticHeaders){
var _169=RadGridNamespace.IsRightToLeft(this.GridHeaderDiv);
if((!_169&&this.GridHeaderDiv&&parseInt(this.GridHeaderDiv.style.marginRight)!=RadGridNamespace.GetScrollBarHeight())||(_169&&this.GridHeaderDiv&&parseInt(this.GridHeaderDiv.style.marginLeft)!=RadGridNamespace.GetScrollBarHeight())){
if(!_169){
this.GridHeaderDiv.style.marginRight=RadGridNamespace.GetScrollBarHeight()+"px";
this.GridHeaderDiv.style.marginLeft="";
}else{
this.GridHeaderDiv.style.marginLeft=RadGridNamespace.GetScrollBarHeight()+"px";
this.GridHeaderDiv.style.marginRight="";
}
}
if(this.GridHeaderDiv&&this.GridDataDiv){
if((this.GridDataDiv.clientWidth==this.GridDataDiv.offsetWidth)){
this.GridHeaderDiv.style.width="100%";
if(!_169){
this.GridHeaderDiv.style.marginRight="";
}else{
this.GridHeaderDiv.style.marginLeft="";
}
}
}
if(this.GroupPanelObject&&this.GroupPanelObject.Items.length>0&&navigator.userAgent.toLowerCase().indexOf("msie")!=-1){
if(this.MasterTableView&&this.MasterTableViewHeader){
this.MasterTableView.Control.style.width=this.MasterTableViewHeader.Control.offsetWidth+"px";
}
}
if(this.GridFooterDiv){
this.GridFooterDiv.style.marginRight=this.GridHeaderDiv.style.marginRight;
this.GridFooterDiv.style.marginLeft=this.GridHeaderDiv.style.marginLeft;
this.GridFooterDiv.style.width=this.GridHeaderDiv.style.width;
}
}
};
RadGridNamespace.RadGrid.prototype.SetDataDivHeight=function(){
if(this.GridDataDiv&&this.Control.style.height!=""){
this.GridDataDiv.style.height="10px";
var _16a=0;
if(this.GroupPanelObject){
_16a+=this.GroupPanelObject.Control.offsetHeight;
}
if(this.GridHeaderDiv){
_16a+=this.GridHeaderDiv.offsetHeight;
}
if(this.GridFooterDiv){
_16a+=this.GridFooterDiv.offsetHeight;
}
if(this.PagerControl){
_16a+=this.PagerControl.offsetHeight;
}
if(this.TopPagerControl){
_16a+=this.TopPagerControl.offsetHeight;
}
if(this.ClientSettings.Scrolling.FrozenColumnsCount>0){
_16a+=RadGridNamespace.GetScrollBarHeight();
}
var _16b=this.Control.clientHeight-_16a;
if(_16b>0){
var _16c=this.Control.style.position;
if(window.netscape){
this.Control.style.position="absolute";
}
this.GridDataDiv.style.height=_16b+"px";
if(window.netscape){
this.Control.style.position=_16c;
}
}
}
};
RadGridNamespace.RadGrid.prototype.InitializeDimensions=function(){
try{
var _16d=this;
this.InitializeAutoLayout();
this.ApplyFrozenScroll();
if(!this.EnableAJAX){
this.OnWindowResize();
}else{
var _16e=function(){
_16d.OnWindowResize();
};
if(window.netscape&&!window.opera){
_16e();
}else{
setTimeout(_16e,0);
}
}
this.Control.RadResize=function(){
_16d.OnWindowResize();
};
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1){
setTimeout(function(){
_16d.AttachDomEvent(window,"resize","OnWindowResize");
},0);
}else{
this.AttachDomEvent(window,"resize","OnWindowResize");
}
this.Control.RadShow=function(){
_16d.OnWindowResize();
};
this.ClientSettings.Scrolling.FrozenColumnsCount+=this.MasterTableViewHeader.ExpandCollapseColumns.length+this.MasterTableViewHeader.GroupSplitterColumns.length;
}
catch(error){
new RadGridNamespace.Error(error,this,this.OnError);
}
};
RadGridNamespace.RadGrid.prototype.OnWindowResize=function(e){
this.InitializeAutoLayout();
this.SetHeaderAndFooterDivsWidth();
this.SetDataDivHeight();
};
RadGridNamespace.RadGrid.prototype.InitializeAutoLayout=function(){
if(this.ClientSettings.Scrolling.AllowScroll&&this.ClientSettings.Scrolling.UseStaticHeaders){
if(this.MasterTableView&&this.MasterTableViewHeader){
if(this.MasterTableView.TableLayout!="Auto"||window.netscape||window.opera){
return;
}
this.MasterTableView.Control.style.tableLayout=this.MasterTableViewHeader.Control.style.tableLayout="";
if(this.MasterTableView.Control.tBodies.length>0){
var _170=this.MasterTableView.Control.tBodies[0].rows[this.ClientSettings.FirstDataRowClientRowIndex];
var _171=this.MasterTableViewHeader.HeaderRow;
var _172=0;
for(var i=0;i<_170.cells.length;i++){
if(parseInt(_170.cells[i].colSpan)>1){
_172+=parseInt(_170.cells[i].colSpan);
}
}
var _174=Math.min(_171.cells.length,(_170.cells.length+(_172-1)));
var _175=0;
var _176=0;
for(var i=0;i<_174;i++){
var col=this.MasterTableViewHeader.ColGroup.Cols[i];
if(!col){
continue;
}
if(col.width!=""){
continue;
}
var _178=_171.cells[i].offsetWidth;
var _179=0;
if(_176>0){
_179=_178;
_176--;
}else{
_179=_170.cells[i].offsetWidth;
if(_170.cells[i].colSpan>1){
_176=_170.cells[i].colSpan;
_179=_171.cells[i].offsetWidth;
_176--;
}
}
var _17a=(_178>_179)?_178:_179;
if(this.MasterTableViewFooter&&this.MasterTableViewFooter.Control){
if(this.MasterTableViewFooter.Control.tBodies[0].rows[0]&&this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i]){
if(this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i].offsetWidth>_17a){
_17a=this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i].offsetWidth;
}
}
}
if(_17a<=0){
continue;
}
_171.cells[i].style.width=_17a;
if(_176==0){
_170.cells[i].style.width=_17a;
this.MasterTableView.ColGroup.Cols[i].width=col.width=_17a;
}
if(this.MasterTableViewFooter&&this.MasterTableViewFooter.Control){
if(this.MasterTableViewFooter.Control.tBodies[0].rows[0]&&this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i]){
this.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[i].style.width=_17a;
}
}
}
}
this.MasterTableView.Control.style.tableLayout=this.MasterTableViewHeader.Control.style.tableLayout="fixed";
if(this.MasterTableViewFooter&&this.MasterTableViewFooter.Control){
this.MasterTableViewFooter.Control.style.tableLayout="fixed";
}
if(window.netscape){
this.OnWindowResize();
}
}
}
};
RadGridNamespace.RadGrid.prototype.InitializeSaveScrollPosition=function(){
if(!this.ClientSettings.Scrolling.SaveScrollPosition){
return;
}
if(this.ClientSettings.Scrolling.ScrollTop!=""&&!this.ClientSettings.Scrolling.EnableAJAXScrollPaging){
this.GridDataDiv.scrollTop=this.ClientSettings.Scrolling.ScrollTop;
}
if(this.ClientSettings.Scrolling.ScrollLeft!=""){
var _17b=document.getElementById(this.ClientID+"_Frozen");
if(this.GridHeaderDiv&&!_17b){
this.GridHeaderDiv.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
if(this.GridFooterDiv&&!_17b){
this.GridFooterDiv.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
if(_17b){
_17b.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}else{
this.GridDataDiv.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
}
};
RadGridNamespace.RadGrid.prototype.InitializeAjaxScrollPaging=function(){
if(!this.ClientSettings.Scrolling.EnableAJAXScrollPaging){
return;
}
this.ScrollCounter=0;
this.CurrentAJAXScrollTop=0;
if(this.ClientSettings.Scrolling.AJAXScrollTop!=""){
this.CurrentAJAXScrollTop=this.ClientSettings.Scrolling.AJAXScrollTop;
}
var _17c=this.CurrentPageIndex*this.MasterTableView.PageSize*20;
var _17d=this.MasterTableView.PageCount*this.MasterTableView.PageSize*20;
var _17e=this.MasterTableView.Control;
var _17f=_17e.offsetHeight;
if(!window.opera){
_17e.style.marginTop=_17c+"px";
_17e.style.marginBottom=_17d-_17c-_17f+"px";
}else{
_17e.style.position="relative";
_17e.style.top=_17c+"px";
_17e.style.marginBottom=_17d-_17f+"px";
}
this.CurrentAJAXScrollTop=_17c;
this.GridDataDiv.scrollTop=_17c;
this.CreateScrollerToolTip();
this.AttachDomEvent(this.GridDataDiv,"scroll","OnAJAXScroll");
};
RadGridNamespace.RadGrid.prototype.CreateScrollerToolTip=function(){
var _180=document.getElementById(this.ClientID+"ScrollerToolTip");
if(!_180){
this.ScrollerToolTip=document.createElement("span");
this.ScrollerToolTip.id=this.ClientID+"ScrollerToolTip";
this.ScrollerToolTip.style.backgroundColor="#F5F5DC";
this.ScrollerToolTip.style.border="1px solid";
this.ScrollerToolTip.style.position="absolute";
this.ScrollerToolTip.style.display="none";
this.ScrollerToolTip.style.font="icon";
this.ScrollerToolTip.style.padding="2";
document.body.appendChild(this.ScrollerToolTip);
}
};
RadGridNamespace.RadGrid.prototype.HideScrollerToolTip=function(){
var _181=this;
setTimeout(function(){
var _182=document.getElementById(_181.ClientID+"ScrollerToolTip");
if(_182&&_182.parentNode){
_182.style.display="none";
}
},200);
};
RadGridNamespace.RadGrid.prototype.ShowScrollerTooltip=function(_183,_184){
var _185=document.getElementById(this.ClientID+"ScrollerToolTip");
if(_185){
_185.style.display="";
_185.style.top=parseInt(RadGridNamespace.FindPosY(this.GridDataDiv))+Math.round(this.GridDataDiv.offsetHeight*_183)+"px";
_185.style.left=parseInt(RadGridNamespace.FindPosX(this.GridDataDiv))+this.GridDataDiv.offsetWidth-(this.GridDataDiv.offsetWidth-this.GridDataDiv.clientWidth)-_185.offsetWidth+"px";
this.ApplyPagerTooltipText(_185,_184,this.MasterTableView.PageCount);
}
};
RadGridNamespace.RadGrid.prototype.ApplyPagerTooltipText=function(_186,_187,_188){
var _189=this.ClientSettings.ClientMessages.PagerTooltipFormatString;
var _18a=/\{0[^\}]*\}/g;
var _18b=/\{1[^\}]*\}/g;
var _18c=((_187==0)?1:_187+1);
var _18d=_188;
_189=_189.replace(_18a,_18c).replace(_18b,_18d);
_186.innerHTML=_189;
};
RadGridNamespace.RadGrid.prototype.InitializeScroll=function(){
var _18e=this;
var grid=this;
var _190=function(){
grid.InitializeSaveScrollPosition();
};
if((window.netscape&&!window.opera)||this.EnableAJAX){
window.setTimeout(_190,0);
}else{
_190();
}
this.InitializeAjaxScrollPaging();
this.AttachDomEvent(this.GridDataDiv,"scroll","OnGridScroll");
if(this.GridHeaderDiv){
this.AttachDomEvent(this.GridHeaderDiv,"scroll","OnGridScroll");
}
};
RadGridNamespace.RadGrid.prototype.ApplyFrozenScroll=function(){
this.isFrozenScroll=false;
if(this.MasterTableView.FrozenColumnsCount==0){
return;
}
var _191=document.getElementById(this.ClientID+"_Frozen");
var _192=RadGridNamespace.GetScrollBarHeight();
if(_191){
var _193=document.getElementById(this.ClientID+"_FrozenScroll");
this.AttachDomEvent(_191,"scroll","OnGridFrozenScroll");
if(this.MasterTableView.Control.offsetWidth>this.GridDataDiv.clientWidth){
_191.style.height=_192+"px";
_193.style.width=this.GridDataDiv.scrollWidth+"px";
_193.style.height=_192+"px";
if(this.ClientSettings.Scrolling.SaveScrollPosition&&this.ClientSettings.Scrolling.ScrollLeft!=""){
_191.scrollLeft=this.ClientSettings.Scrolling.ScrollLeft;
}
if(this.GridDataDiv.style.overflowX!=null){
this.GridDataDiv.style.overflowX="hidden";
}else{
_191.style.marginTop="-16px";
_191.style.zIndex=99999;
_191.style.position="relative";
}
if(window.netscape&&!window.opera){
_191.style.width=this.GridDataDiv.offsetWidth-_192+"px";
}
if(this.GridHeaderDiv&&this.GridDataDiv){
if((this.GridDataDiv.clientWidth==this.GridDataDiv.offsetWidth)){
if(typeof (_191.style.overflowX)!="undefined"&&typeof (_191.style.overflowY)!="undefined"){
_191.style.overflowX="auto";
_191.style.overflowY="hidden";
if(window.netscape){
_191.style.width=parseInt(_191.style.width)+_192+"px";
}
}
}
}
this.isFrozenScroll=true;
}else{
_191.style.height="";
_193.style.width="";
this.GridDataDiv.style.overflow="auto";
this.isFrozenScroll=false;
}
}
};
RadGridNamespace.RadGrid.prototype.OnGridFrozenScroll=function(e){
if(!this.FrozenScrollCounter){
this.FrozenScrollCounter=0;
}
this.FrozenScrollCounter++;
var _195=this;
_195.currentElement=(e.srcElement)?e.srcElement:e.target;
RadGridNamespace.FrozenScrollHanlder=function(_196){
if(_195.FrozenScrollCounter!=_196){
return;
}
if(!_195.LastScrollIndex){
_195.LastScrollIndex=0;
}
var _197=_195.currentElement;
if(_195.ClientSettings.Scrolling.FrozenColumnsCount>_195.MasterTableViewHeader.Columns.length){
_195.isFrozenScroll=false;
}
if(_195.isFrozenScroll){
var _198=_195.MasterTableView.Columns[_195.ClientSettings.Scrolling.FrozenColumnsCount-1].Control;
var _199=RadGridNamespace.FindPosX(_198)-RadGridNamespace.FindScrollPosX(_198)+document.documentElement.scrollLeft+document.body.scrollLeft+_198.offsetWidth;
var _19a=_197.scrollWidth-_199;
_195.notFrozenColumns=[];
var _19b=_195.MasterTableView.Control.tBodies[0].rows[_195.ClientSettings.FirstDataRowClientRowIndex];
for(var i=_195.ClientSettings.Scrolling.FrozenColumnsCount;i<_195.MasterTableView.Columns.length;i++){
var _19d=_195.MasterTableView.Columns[i];
var _19e=false;
if(window.netscape&&_19d.Control.style.display=="none"){
_19d.Control.style.display="table-cell";
_19e=true;
}
var _19f=(_19d.Control.offsetWidth>0)?_19d.Control.offsetWidth:_19b.cells[i].offsetWidth;
_195.notFrozenColumns[_195.notFrozenColumns.length]={Index:i,Width:_19f};
if(window.netscape&&_19e){
_19d.Control.style.display="none";
_19e=false;
}
}
var _1a0=RadGridNamespace.GetScrollBarHeight();
if(window.netscape&&!window.opera){
_1a0=0;
}
var _1a1=Math.floor(_197.scrollLeft/(_197.scrollWidth-(1.5*_198.offsetWidth))*100);
var _1a2=0;
var i=0;
while(i<_195.notFrozenColumns.length-1){
var _19d=_195.notFrozenColumns[i];
var _1a3=Math.floor(_19d.Width/_19a*100);
if(_1a3+_1a2<_1a1){
if(typeof (_195.MasterTableView.Columns[_19d.Index].FrozenDisplay)=="boolean"&&!_195.MasterTableView.Columns[_19d.Index].FrozenDisplay){
i++;
continue;
}
_195.MasterTableViewHeader.HideNotFrozenColumn(_19d.Index);
_1a2+=_1a3;
}else{
if(typeof (_195.MasterTableView.Columns[_19d.Index].FrozenDisplay)=="boolean"&&_195.MasterTableView.Columns[_19d.Index].FrozenDisplay){
i++;
continue;
}
_195.MasterTableViewHeader.ShowNotFrozenColumn(_19d.Index);
}
i++;
}
_195.MasterTableView.Control.style.width=_195.MasterTableViewHeader.Control.offsetWidth+"px";
if(_195.MasterTableViewFooter){
_195.MasterTableViewFooter.Control.style.width=_195.MasterTableViewHeader.Control.offsetWidth+"px";
}
_195.SavePostData("ScrolledControl",_195.ClientID,_195.GridDataDiv.scrollTop,_197.scrollLeft);
}else{
_195.GridDataDiv.scrollLeft=_197.scrollLeft;
}
_195.FrozenScrollCounter=0;
};
setTimeout("RadGridNamespace.FrozenScrollHanlder("+this.FrozenScrollCounter+")",100);
};
RadGridNamespace.RadGrid.prototype.OnGridScroll=function(e){
var _1a5=(e.srcElement)?e.srcElement:e.target;
if(window.opera&&this.isFrozenScroll){
this.GridDataDiv.scrollLeft=this.GridHeaderDiv.scrollLeft=0;
return;
}
if(this.ClientSettings.Scrolling.UseStaticHeaders){
if(!this.isFrozenScroll){
if(this.GridHeaderDiv){
if(_1a5==this.GridHeaderDiv){
this.GridDataDiv.scrollLeft=this.GridHeaderDiv.scrollLeft;
}
if(_1a5==this.GridDataDiv){
this.GridHeaderDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
}
if(this.GridFooterDiv){
this.GridFooterDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
}else{
if(this.GridHeaderDiv){
this.GridHeaderDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
if(this.GridFooterDiv){
this.GridFooterDiv.scrollLeft=this.GridDataDiv.scrollLeft;
}
}
}
this.SavePostData("ScrolledControl",this.ClientID,this.GridDataDiv.scrollTop,this.GridDataDiv.scrollLeft);
var evt={};
evt.ScrollTop=this.GridDataDiv.scrollTop;
evt.ScrollLeft=this.GridDataDiv.scrollLeft;
evt.ScrollControl=this.GridDataDiv;
evt.IsOnTop=(this.GridDataDiv.scrollTop==0)?true:false;
evt.IsOnBottom=((this.GridDataDiv.scrollHeight-this.GridDataDiv.offsetHeight+16)==this.GridDataDiv.scrollTop)?true:false;
RadGridNamespace.FireEvent(this,"OnScroll",[evt]);
};
RadGridNamespace.RadGrid.prototype.OnAJAXScroll=function(e){
if(this.GridDataDiv){
this.CurrentScrollTop=this.GridDataDiv.scrollTop;
}
this.ScrollCounter++;
var _1a8=this;
RadGridNamespace.AJAXScrollHanlder=function(_1a9){
if(_1a8.ScrollCounter!=_1a9){
return;
}
if(_1a8.CurrentAJAXScrollTop!=_1a8.GridDataDiv.scrollTop){
if(_1a8.CurrentPageIndex==_1aa){
return;
}
var _1ab=_1a8.ClientID;
var _1ac=_1a8.MasterTableView.ClientID;
_1a8.SavePostData("AJAXScrolledControl",_1a8.GridDataDiv.scrollLeft,_1a8.LastScrollTop,_1a8.GridDataDiv.scrollTop,_1aa);
var _1ad=_1a8.ClientSettings.PostBackFunction;
_1ad=_1ad.replace("{0}",_1a8.UniqueID);
eval(_1ad);
}
_1a8.ScrollCounter=0;
_1a8.HideScrollerToolTip();
};
var evt={};
evt.ScrollTop=this.GridDataDiv.scrollTop;
evt.ScrollLeft=this.GridDataDiv.scrollLeft;
evt.ScrollControl=this.GridDataDiv;
evt.IsOnTop=(this.GridDataDiv.scrollTop==0)?true:false;
evt.IsOnBottom=((this.GridDataDiv.scrollHeight-this.GridDataDiv.offsetHeight+16)==this.GridDataDiv.scrollTop)?true:false;
RadGridNamespace.FireEvent(this,"OnScroll",[evt]);
var _1af=this.GridDataDiv.scrollTop/(this.GridDataDiv.scrollHeight-this.GridDataDiv.offsetHeight+16);
var _1aa=Math.round((this.MasterTableView.PageCount-1)*_1af);
setTimeout("RadGridNamespace.AJAXScrollHanlder("+this.ScrollCounter+")",500);
this.ShowScrollerTooltip(_1af,_1aa);
};
RadGridNamespace.RadGridTable=function(_1b0){
if((!_1b0)||typeof (_1b0)!="object"){
return;
}
for(var _1b1 in _1b0){
this[_1b1]=_1b0[_1b1];
}
this.Type="RadGridTable";
this.ServerID=this.ID;
this.SelectedRows=new Array();
this.SelectedCells=new Array();
this.SelectedColumns=new Array();
this.ExpandCollapseColumns=new Array();
this.GroupSplitterColumns=new Array();
this.HeaderRow=null;
};
RadGridNamespace.RadGridTable.prototype._constructor=function(_1b2){
if((!_1b2)||typeof (_1b2)!="object"){
return;
}
this.Control=document.getElementById(this.ClientID);
if(!this.Control){
return;
}
this.ColGroup=RadGridNamespace.GetTableColGroup(this.Control);
if(!this.ColGroup){
return;
}
this.ColGroup.Cols=RadGridNamespace.GetTableColGroupCols(this.ColGroup);
this.Owner=_1b2;
if(this.Owner.ClientSettings){
this.InitializeEvents(this.Owner.ClientSettings.ClientEvents);
this.Control.style.overflow=((this.Owner.ClientSettings.Resizing.ClipCellContentOnResize&&((this.Owner.ClientSettings.Resizing.AllowColumnResize)||(this.Owner.ClientSettings.Resizing.AllowRowResize)))||(this.Owner.ClientSettings.Scrolling.AllowScroll&&this.Owner.ClientSettings.Scrolling.UseStaticHeaders))?"hidden":"";
}
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&this.Control.style.tableLayout=="fixed"&&this.Control.style.width.indexOf("%")!=-1){
this.Control.style.width="";
}
this.CreateStyles();
if(this.Owner.ClientSettings&&this.Owner.ClientSettings.Scrolling.AllowScroll&&this.Owner.ClientSettings.Scrolling.UseStaticHeaders){
if(this.ClientID.indexOf("_Header")!=-1||this.ClientID.indexOf("_Detail")!=-1){
this.Columns=this.GetTableColumns(this.Control,this.RenderColumns);
}else{
this.Columns=this.Owner.MasterTableViewHeader.Columns;
this.ExpandCollapseColumns=this.Owner.MasterTableViewHeader.ExpandCollapseColumns;
this.GroupSplitterColumns=this.Owner.MasterTableViewHeader.GroupSplitterColumns;
}
}else{
this.Columns=this.GetTableColumns(this.Control,this.RenderColumns);
}
if(this.Owner.ClientSettings&&this.Owner.ClientSettings.ShouldCreateRows){
this.InitializeRows(this.Controls[0].Rows);
}
};
RadGridNamespace.RadGridTable.prototype.Dispose=function(){
if(this.ColGroup&&this.ColGroup.Cols){
this.ColGroup.Cols=null;
this.ColGroup=null;
}
this.Owner=null;
this.DisposeEvents();
this.ExpandCollapseColumns=null;
this.GroupSplitterColumns=null;
this.DisposeRows();
this.DisposeColumns();
this.RenderColumns=null;
this.SelectedRows=null;
this.ExpandCollapseColumns=null;
this.DetailTables=null;
this.DetailTablesCollection=null;
this.Control=null;
this.HeaderRow=null;
};
RadGridNamespace.RadGridTable.prototype.CreateStyles=function(){
if(!this.SelectedItemStyleClass||this.SelectedItemStyleClass==""){
if(this.SelectedItemStyle&&this.SelectedItemStyle!=""){
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".SelectedItemStyle"+this.ClientID+"1 td",this.SelectedItemStyle);
}else{
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".SelectedItemStyle"+this.ClientID+"2 td","background-color:Navy;color:White;");
}
}
RadGridNamespace.addClassName(this.Control,"grid"+this.ClientID);
if(window.netscape&&!window.opera){
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" td","overflow: hidden;-moz-user-select:-moz-none;");
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" th","overflow: hidden;-moz-user-select:-moz-none;");
}else{
if(window.opera||navigator.userAgent.indexOf("Safari")!=-1){
var _1b3=this;
setTimeout(function(){
RadGridNamespace.AddRule(_1b3.Owner.GridStyleSheet,".grid"+_1b3.ClientID+" td","overflow: hidden;");
RadGridNamespace.AddRule(_1b3.Owner.GridStyleSheet,".grid"+_1b3.ClientID+" th","overflow: hidden;");
},100);
}else{
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" td","overflow: hidden; text-overflow: ellipsis;");
RadGridNamespace.AddRule(this.Owner.GridStyleSheet,".grid"+this.ClientID+" th","overflow: hidden; text-overflow: ellipsis;");
}
}
};
RadGridNamespace.RadGridTable.prototype.InitializeEvents=function(_1b4){
for(clientEvent in _1b4){
if(typeof (_1b4[clientEvent])!="string"){
continue;
}
if(!this.Owner.IsClientEventName(clientEvent)){
if(_1b4[clientEvent]!=""){
var _1b5=_1b4[clientEvent];
if(_1b5.indexOf("(")!=-1){
this[clientEvent]=_1b5;
}else{
this[clientEvent]=eval(_1b5);
}
}else{
this[clientEvent]=null;
}
}
}
};
RadGridNamespace.RadGridTable.prototype.DisposeEvents=function(){
for(var _1b6 in RadGridNamespace.RadGridTable.ClientEventNames){
this[_1b6]=null;
}
};
RadGridNamespace.RadGridTable.prototype.InitializeRows=function(rows){
if(this.ClientID.indexOf("_Header")!=-1||this.ClientID.indexOf("_Footer")!=-1){
return;
}
try{
var _1b8=[];
for(var i=0;i<rows.length;i++){
if(!rows[i].Visible||rows[i].ClientRowIndex<0){
continue;
}
if(rows[i].ItemType=="THead"||rows[i].ItemType=="TFoot"||rows[i].ItemType=="NoRecordsItem"){
continue;
}
RadGridNamespace.FireEvent(this,"OnRowCreating");
rows[i]._constructor(this);
_1b8[_1b8.length]=rows[i];
RadGridNamespace.FireEvent(this,"OnRowCreated",[rows[i]]);
}
this.Rows=_1b8;
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.DisposeRows=function(){
if(this.Rows!=null){
for(var i=0;i<this.Rows.length;i++){
var row=this.Rows[i];
row.Dispose();
}
this.Rows=null;
}
};
RadGridNamespace.RadGridTable.prototype.DisposeColumns=function(){
if(this.Columns!=null){
for(var i=0;i<this.Columns.length;i++){
var _1bd=this.Columns[i];
_1bd.Dispose();
}
this.Columns=null;
}
};
RadGridNamespace.RadGridTable.prototype.GetTableRows=function(_1be,_1bf){
if(this.ClientID.indexOf("_Header")!=-1||this.ClientID.indexOf("_Footer")!=-1){
return;
}
try{
var _1c0=this;
var _1c1=new Array();
var j=0;
for(var i=0;i<_1bf.length;i++){
if((_1bf[i].ItemType=="THead")||(_1bf[i].ItemType=="TFoot")){
continue;
}
if((_1bf[i])&&(_1bf[i].Visible)){
RadGridNamespace.FireEvent(this,"OnRowCreating");
setTimeout(function(){
_1c1[_1c1.length]=_1bf[i]._constructor(_1c0);
},0);
RadGridNamespace.FireEvent(this,"OnRowCreated",[_1c1[j]]);
j++;
}
}
return _1c1;
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.GetTableHeaderRow=function(){
try{
if(this.Control.tHead){
for(var i=0;i<this.Control.tHead.rows.length;i++){
if(this.Control.tHead.rows[i]!=null){
if(this.Control.tHead.rows[i].cells[0]!=null){
if(this.Control.tHead.rows[i].cells[0].tagName!=null){
if(this.Control.tHead.rows[i].cells[0].tagName.toLowerCase()=="th"){
this.HeaderRow=this.Control.tHead.rows[i];
break;
}
}
}
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.GetTableColumns=function(_1c5,_1c6){
try{
this.GetTableHeaderRow();
var _1c7=new Array();
if(!this.HeaderRow){
return;
}
if(!this.HeaderRow.cells[0]){
return;
}
var j=0;
for(var i=0;i<_1c6.length;i++){
if(_1c6[i].Visible){
RadGridNamespace.FireEvent(this,"OnColumnCreating");
_1c7[_1c7.length]=new RadGridNamespace.RadGridTableColumn(_1c6[i]);
if(this.HeaderRow.cells[j]!=null){
_1c7[j]._constructor(this.HeaderRow.cells[j],this);
_1c7[j].RealIndex=i;
if(_1c6[i].ColumnType=="GridExpandColumn"){
this.ExpandCollapseColumns[this.ExpandCollapseColumns.length]=_1c7[j];
}
if(_1c6[i].ColumnType=="GridGroupSplitterColumn"){
this.GroupSplitterColumns[this.GroupSplitterColumns.length]=_1c7[j];
}
if(_1c6[i].ColumnType=="GridRowIndicatorColumn"){
if(this.ClientID.indexOf("_Header")!=-1){
this.Owner.ClientSettings.Scrolling.FrozenColumnsCount++;
}
}
RadGridNamespace.FireEvent(this,"OnColumnCreated",[_1c7[j]]);
}
j++;
}
}
return _1c7;
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.RemoveTableLayOut=function(){
this.masterTableLayOut=this.Owner.MasterTableView.Control.style.tableLayout;
this.detailTablesTableLayOut=new Array();
for(var i=0;i<this.Owner.DetailTablesCollection.length;i++){
this.detailTablesTableLayOut[this.detailTablesTableLayOut.length]=this.Owner.DetailTablesCollection[i].Control.style.tableLayout;
this.Owner.DetailTablesCollection[i].Control.style.tableLayout="";
}
};
RadGridNamespace.RadGridTable.prototype.RestoreTableLayOut=function(){
this.Owner.MasterTableView.Control.style.tableLayout=this.masterTableLayOut;
for(var i=0;i<this.Owner.DetailTablesCollection.length;i++){
this.Owner.DetailTablesCollection[i].Control.style.tableLayout=this.detailTablesTableLayOut[i];
}
};
RadGridNamespace.RadGridTable.prototype.SelectRow=function(row,_1cd){
try{
if(!this.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
var _1ce=this.Owner.GetRowObjectByRealRow(this,row);
if(_1ce!=null){
if(_1ce.ItemType=="Item"||_1ce.ItemType=="AlternatingItem"){
_1ce.SetSelected(_1cd);
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.DeselectRow=function(row){
try{
if(!this.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
var _1d0=this.Owner.GetRowObjectByRealRow(this,row);
if(_1d0!=null){
if(_1d0.ItemType=="Item"||_1d0.ItemType=="AlternatingItem"){
this.RemoveFromSelectedRows(_1d0);
_1d0.RemoveSelectedRowStyle();
_1d0.Selected=false;
_1d0.CheckClientSelectColumns();
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ResizeRow=function(_1d1,_1d2,_1d3){
try{
if(!this.Owner.ClientSettings.Resizing.AllowRowResize){
return;
}
if(!RadGridNamespace.FireEvent(this,"OnRowResizing",[_1d1,_1d2])){
return;
}
this.RemoveTableLayOut();
var _1d4=this.Control.style.tableLayout;
this.Control.style.tableLayout="";
var _1d5=this.Control.parentNode.parentNode.parentNode.parentNode;
var _1d6=this.Owner.GetTableObjectByID(_1d5.id);
var _1d7;
if(_1d6!=null){
_1d7=_1d6.Control.style.tableLayout;
_1d6.Control.style.tableLayout="";
}
if(!_1d3){
if(this.Control){
if(this.Control.rows[_1d1]){
if(this.Control.rows[_1d1].cells[0]){
this.Control.rows[_1d1].cells[0].style.height=_1d2+"px";
this.Control.rows[_1d1].style.height=_1d2+"px";
}
}
}
}else{
if(this.Control){
if(this.Control.tBodies[0]){
if(this.Control.tBodies[0].rows[_1d1]){
if(this.Control.tBodies[0].rows[_1d1].cells[0]){
this.Control.tBodies[0].rows[_1d1].cells[0].style.height=_1d2+"px";
this.Control.tBodies[0].rows[_1d1].style.height=_1d2+"px";
}
}
}
}
}
this.Control.style.tableLayout=_1d4;
if(_1d6!=null){
_1d6.Control.style.tableLayout=_1d7;
}
this.RestoreTableLayOut();
var _1d8=this.Owner.GetRowObjectByRealRow(this,this.Control.rows[_1d1]);
this.Owner.SavePostData("ResizedRows",this.Control.id,_1d8.RealIndex,_1d2+"px");
RadGridNamespace.FireEvent(this,"OnRowResized",[_1d1,_1d2]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ResizeColumn=function(_1d9,_1da){
if(isNaN(parseInt(_1d9))){
var _1db="Column index must be of type \"Number\"!";
alert(_1db);
return;
}
if(isNaN(parseInt(_1da))){
var _1db="Column width must be of type \"Number\"!";
alert(_1db);
return;
}
if(_1d9<0){
var _1db="Column index must be non-negative!";
alert(_1db);
return;
}
if(_1da<0){
var _1db="Column width must be non-negative!";
alert(_1db);
return;
}
if(_1d9>(this.Columns.length-1)){
var _1db="Column index must be less than columns count!";
alert(_1db);
return;
}
if(!this.Owner.ClientSettings.Resizing.AllowColumnResize){
return;
}
if(!this.Columns){
return;
}
if(!this.Columns[_1d9].Resizable){
return;
}
if(!RadGridNamespace.FireEvent(this,"OnColumnResizing",[_1d9,_1da])){
return;
}
try{
if(this==this.Owner.MasterTableView&&this.Owner.MasterTableViewHeader){
this.Owner.MasterTableViewHeader.ResizeColumn(_1d9,_1da);
}
var _1dc=this.Control.clientWidth;
var _1dd=this.Owner.Control.clientWidth;
if(this.HeaderRow){
var _1de=this.HeaderRow.cells[_1d9].scrollWidth-_1da;
}
if(window.netscape||window.opera){
if(this.HeaderRow){
if(this.HeaderRow.cells[_1d9]){
this.HeaderRow.cells[_1d9].style.width=_1da+"px";
}
}
if(this==this.Owner.MasterTableViewHeader){
var _1df=this.Owner.MasterTableView.Control.tBodies[0].rows[this.Owner.ClientSettings.FirstDataRowClientRowIndex];
if(_1df){
if(_1df.cells[_1d9]){
_1df.cells[_1d9].style.width=_1da+"px";
}
}
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.Control){
if(this.Owner.MasterTableViewFooter.Control.tBodies[0].rows[0]&&this.Owner.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[_1d9]){
if(_1da>0){
this.Owner.MasterTableViewFooter.Control.tBodies[0].rows[0].cells[_1d9].style.width=_1da+"px";
}
}
}
}
}
if(this.ColGroup){
if(this.ColGroup.Cols[_1d9]){
if(_1da>0){
this.ColGroup.Cols[_1d9].width=_1da+"px";
}
}
}
if(this==this.Owner.MasterTableViewHeader){
if(this.Owner.MasterTableView.ColGroup){
if(this.Owner.MasterTableView.ColGroup.Cols[_1d9]){
if(_1da>0){
this.Owner.MasterTableView.ColGroup.Cols[_1d9].width=_1da+"px";
}
}
}
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.ColGroup){
if(this.Owner.MasterTableViewFooter.ColGroup.Cols[_1d9]){
if(_1da>0){
this.Owner.MasterTableViewFooter.ColGroup.Cols[_1d9].width=_1da+"px";
}
}
}
}
if(_1da.toString().indexOf("px")!=-1){
_1da=_1da.replace("px","");
}
if(this==this.Owner.MasterTableView||this==this.Owner.MasterTableViewHeader){
if(_1da.toString().indexOf("%")!=-1){
this.Owner.SavePostData("ResizedColumns",this.Owner.MasterTableView.ClientID,this.Columns[_1d9].RealIndex,_1da);
}else{
this.Owner.SavePostData("ResizedColumns",this.Owner.MasterTableView.ClientID,this.Columns[_1d9].RealIndex,_1da+"px");
}
}else{
if(_1da.toString().indexOf("%")!=-1){
this.Owner.SavePostData("ResizedColumns",this.ClientID,this.Columns[_1d9].RealIndex,_1da);
}else{
this.Owner.SavePostData("ResizedColumns",this.ClientID,this.Columns[_1d9].RealIndex,_1da+"px");
}
}
if(this.Owner.MasterTableViewHeader){
this.Owner.ClientSettings.Resizing.ResizeGridOnColumnResize=true;
}
if(this.Owner.ClientSettings.Resizing.ResizeGridOnColumnResize){
if(this==this.Owner.MasterTableViewHeader){
for(var i=0;i<this.ColGroup.Cols.length;i++){
if(i!=_1d9&&this.ColGroup.Cols[i].width==""){
this.ColGroup.Cols[i].width=this.HeaderRow.cells[i].scrollWidth+"px";
this.Owner.MasterTableView.ColGroup.Cols[i].width=this.ColGroup.Cols[i].width;
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.ColGroup){
this.Owner.MasterTableViewFooter.ColGroup.Cols[i].width=this.ColGroup.Cols[i].width;
}
}
}
this.Control.style.width=(this.Control.offsetWidth-_1de)+"px";
this.Owner.MasterTableView.Control.style.width=this.Control.style.width;
if(this.Owner.MasterTableViewFooter&&this.Owner.MasterTableViewFooter.Control){
this.Owner.MasterTableViewFooter.Control.style.width=this.Control.style.width;
}
var _1e1=(this.Control.scrollWidth>this.Control.offsetWidth)?this.Control.scrollWidth:this.Control.offsetWidth;
var _1e2=this.Owner.GridDataDiv.offsetWidth;
this.Owner.SavePostData("ResizedControl",this.ClientID,_1e1+"px",_1e2+"px",this.Owner.Control.offsetHeight+"px");
}else{
if(window.netscape||window.opera){
this.Control.style.width=(this.Control.offsetWidth-_1de)+"px";
this.Owner.Control.style.width=this.Control.style.width;
}
var _1e1=(this.Control.scrollWidth>this.Control.offsetWidth)?this.Control.scrollWidth:this.Control.offsetWidth;
this.Owner.SavePostData("ResizedControl",this.ClientID,_1e1+"px",this.Owner.Control.offsetWidth+"px",this.Owner.Control.offsetHeight+"px");
}
}else{
var _1e3=(this.Control.offsetWidth-_1dd)/this.ColGroup.Cols.length;
var _1e4="";
for(var i=_1d9+1;i<this.ColGroup.Cols.length;i++){
var _1e5=0;
if(this.ColGroup.Cols[i].width!=""){
_1e5=parseInt(this.ColGroup.Cols[i].width)-_1e3;
}
if(this.HeaderRow){
_1e5=this.HeaderRow.cells[i].scrollWidth-_1e3;
}
this.ColGroup.Cols[i].width="";
if(this==this.Owner.MasterTableViewHeader){
this.Owner.MasterTableView.ColGroup.Cols[i].width="";
}
if(this.Owner.MasterTableViewFooter){
this.Owner.MasterTableViewFooter.ColGroup.Cols[i].width="";
}
}
if(_1dd>0){
this.Owner.Control.style.width=_1dd+"px";
}
this.Control.style.width=_1dc+"px";
if(this==this.Owner.MasterTableViewHeader){
this.Owner.MasterTableView.Control.style.width=this.Control.style.width;
}
if(this.Owner.MasterTableViewFooter){
this.Owner.MasterTableViewFooter.Control.style.width=this.Control.style.width;
}
}
if(this.Owner.GroupPanelObject&&this.Owner.GroupPanelObject.Items.length>0&&navigator.userAgent.toLowerCase().indexOf("msie")!=-1){
if(this.Owner.MasterTableView&&this.Owner.MasterTableViewHeader){
this.Owner.MasterTableView.Control.style.width=this.Owner.MasterTableViewHeader.Control.offsetWidth+"px";
}
}
RadGridNamespace.FireEvent(this,"OnColumnResized",[_1d9,_1da]);
if(window.netscape){
this.Control.style.cssText=this.Control.style.cssText;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ReorderColumns=function(_1e6,_1e7){
if(isNaN(parseInt(_1e6))){
var _1e8="First column index must be of type \"Number\"!";
alert(_1e8);
return;
}
if(isNaN(parseInt(_1e7))){
var _1e8="Second column index must be of type \"Number\"!";
alert(_1e8);
return;
}
if(_1e6<0){
var _1e8="First column index must be non-negative!";
alert(_1e8);
return;
}
if(_1e7<0){
var _1e8="Second column index must be non-negative!";
alert(_1e8);
return;
}
if(_1e6>(this.Columns.length-1)){
var _1e8="First column index must be less than columns count!";
alert(_1e8);
return;
}
if(_1e7>(this.Columns.length-1)){
var _1e8="Second column index must be less than columns count!";
alert(_1e8);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
if(!this.Columns){
return;
}
if(!this.Columns[_1e6].Reorderable){
return;
}
if(!this.Columns[_1e7].Reorderable){
return;
}
this.SwapColumns(_1e6,_1e7);
if((!this.Owner.ClientSettings.ReorderColumnsOnClient)&&(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder!="")){
if(this==this.Owner.MasterTableView){
eval(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder);
}
}
};
RadGridNamespace.RadGridTable.prototype.SwapColumns=function(_1e9,_1ea,_1eb){
if(isNaN(parseInt(_1e9))){
var _1ec="First column index must be of type \"Number\"!";
alert(_1ec);
return;
}
if(isNaN(parseInt(_1ea))){
var _1ec="Second column index must be of type \"Number\"!";
alert(_1ec);
return;
}
if(_1e9<0){
var _1ec="First column index must be non-negative!";
alert(_1ec);
return;
}
if(_1ea<0){
var _1ec="Second column index must be non-negative!";
alert(_1ec);
return;
}
if(_1e9>(this.Columns.length-1)){
var _1ec="First column index must be less than columns count!";
alert(_1ec);
return;
}
if(_1ea>(this.Columns.length-1)){
var _1ec="Second column index must be less than columns count!";
alert(_1ec);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
if(!this.Columns){
return;
}
if(!this.Columns[_1e9].Reorderable){
return;
}
if(!this.Columns[_1ea].Reorderable){
return;
}
try{
if(this==this.Owner.MasterTableView&&this.Owner.MasterTableViewHeader){
this.Owner.MasterTableViewHeader.SwapColumns(_1e9,_1ea,!this.Owner.ClientSettings.ReorderColumnsOnClient);
return;
}
if(typeof (_1eb)=="undefined"){
_1eb=true;
}
if(this.Owner.ClientSettings.ColumnsReorderMethod=="Reorder"){
if(_1ea>_1e9){
while(_1e9+1<_1ea){
this.SwapColumns(_1ea-1,_1ea,false);
_1ea--;
}
}else{
while(_1ea<_1e9-1){
this.SwapColumns(_1ea+1,_1ea,false);
_1ea++;
}
}
}
if(!RadGridNamespace.FireEvent(this,"OnColumnSwapping",[_1e9,_1ea])){
return;
}
var _1ed=this.Control;
var _1ee=this.Columns[_1e9];
var _1ef=this.Columns[_1ea];
this.Columns[_1e9]=_1ef;
this.Columns[_1ea]=_1ee;
var _1f0=this.ColGroup.Cols[_1e9].width;
if(_1f0==""&&this.HeaderRow){
_1f0=this.HeaderRow.cells[_1e9].offsetWidth;
}
var _1f1=this.ColGroup.Cols[_1ea].width;
if(_1f1==""&&this.HeaderRow){
_1f1=this.HeaderRow.cells[_1ea].offsetWidth;
}
var _1f2=this.Owner.ClientSettings.Resizing.AllowColumnResize;
var _1f3=(typeof (this.Columns[_1e9].Resizable)=="boolean")?this.Columns[_1e9].Resizable:false;
var _1f4=(typeof (this.Columns[_1ea].Resizable)=="boolean")?this.Columns[_1ea].Resizable:false;
this.Owner.ClientSettings.Resizing.AllowColumnResize=true;
this.Columns[_1e9].Resizable=true;
this.Columns[_1ea].Resizable=true;
this.ResizeColumn(_1e9,_1f1);
this.ResizeColumn(_1ea,_1f0);
this.Owner.ClientSettings.Resizing.AllowColumnResize=_1f2;
this.Columns[_1e9].Resizable=_1f3;
this.Columns[_1ea].Resizable=_1f4;
if(navigator.userAgent.indexOf("Safari")!=-1){
var _1f5=this.Columns[_1e9].Control;
var _1f6=this.Columns[_1ea].Control;
this.Columns[_1e9].Control=_1f6;
this.Columns[_1ea].Control=_1f5;
}
var _1f7=(this==this.Owner.MasterTableViewHeader)?this.Owner.MasterTableView.ClientID:this.ClientID;
this.Owner.SavePostData("ReorderedColumns",_1f7,this.Columns[_1e9].UniqueName,this.Columns[_1ea].UniqueName);
for(var i=0;i<_1ed.rows.length;i++){
if(_1ed.rows[i]!=null){
if((_1ed.rows[i].cells[_1e9]!=null)&&(_1ed.rows[i].cells[_1ea]!=null)){
if(_1ed.rows[i].cells[_1e9].innerHTML!=null){
var _1f9=_1ed.rows[i].cells[_1e9].innerHTML;
var _1fa=_1ed.rows[i].cells[_1ea].innerHTML;
_1ed.rows[i].cells[_1e9].innerHTML=_1fa;
_1ed.rows[i].cells[_1ea].innerHTML=_1f9;
}
}
}
}
if(this.Owner.MasterTableViewHeader==this){
var _1ed=this.Owner.MasterTableView.Control;
for(var i=0;i<_1ed.rows.length;i++){
if(_1ed.rows[i]!=null){
if((_1ed.rows[i].cells[_1e9]!=null)&&(_1ed.rows[i].cells[_1ea]!=null)){
if(_1ed.rows[i].cells[_1e9].innerHTML!=null){
var _1f9=_1ed.rows[i].cells[_1e9].innerHTML;
var _1fa=_1ed.rows[i].cells[_1ea].innerHTML;
_1ed.rows[i].cells[_1e9].innerHTML=_1fa;
_1ed.rows[i].cells[_1ea].innerHTML=_1f9;
}
}
}
}
}
if(_1eb&&(!this.Owner.ClientSettings.ReorderColumnsOnClient)&&(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder!="")){
eval(this.Owner.ClientSettings.PostBackReferences.PostBackColumnsReorder);
}
RadGridNamespace.FireEvent(this,"OnColumnSwapped",[_1e9,_1ea]);
this.Owner.InitializeFilterMenu(this);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.MoveColumnToLeft=function(_1fb){
if(isNaN(parseInt(_1fb))){
var _1fc="Column index must be of type \"Number\"!";
alert(_1fc);
return;
}
if(_1fb<0){
var _1fc="Column index must be non-negative!";
alert(_1fc);
return;
}
if(_1fb>(this.Columns.length-1)){
var _1fc="Column index must be less than columns count!";
alert(_1fc);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnMovingToLeft",[_1fb])){
return;
}
var _1fd=_1fb--;
this.SwapColumns(_1fb,_1fd);
RadGridNamespace.FireEvent(this,"OnColumnMovedToLeft",[_1fb]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.MoveColumnToRight=function(_1fe){
if(isNaN(parseInt(_1fe))){
var _1ff="Column index must be of type \"Number\"!";
alert(_1ff);
return;
}
if(_1fe<0){
var _1ff="Column index must be non-negative!";
alert(_1ff);
return;
}
if(_1fe>(this.Columns.length-1)){
var _1ff="Column index must be less than columns count!";
alert(_1ff);
return;
}
if(!this.Owner.ClientSettings.AllowColumnsReorder){
return;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnMovingToRight",[_1fe])){
return;
}
var _200=_1fe++;
this.SwapColumns(_1fe,_200);
RadGridNamespace.FireEvent(this,"OnColumnMovedToRight",[_1fe]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.CanShowHideColumn=function(_201){
if(!this.Owner.ClientSettings.AllowColumnHide){
return false;
}
if(isNaN(parseInt(_201))){
var _202="Column index must be of type \"Number\"!";
alert(_202);
return false;
}
if(_201<0){
var _202="Column index must be non-negative!";
alert(_202);
return false;
}
if(_201>(this.Columns.length-1)){
var _202="Column index must be less than columns count!";
alert(_202);
return false;
}
return true;
};
RadGridNamespace.RadGridTable.prototype.HideNotFrozenColumn=function(_203){
this.HideShowNotFrozenColumn(_203,false);
};
RadGridNamespace.RadGridTable.prototype.ShowNotFrozenColumn=function(_204){
this.HideShowNotFrozenColumn(_204,true);
};
RadGridNamespace.RadGridTable.prototype.HideShowNotFrozenColumn=function(_205,_206){
if(this.Owner.MasterTableViewHeader){
this.Owner.MasterTableViewHeader.Columns[_205].FrozenDisplay=_206;
if(!window.netscape&&navigator.userAgent.toLowerCase().indexOf("safari")==-1){
this.HideShowCol(this.Owner.MasterTableViewHeader,_205,_206);
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
var _207=this.Owner.MasterTableViewHeader.Control.getElementsByTagName("select");
if(_207.length>0){
var _208=this;
setTimeout(function(){
RadGridNamespace.HideShowCells(_208.Owner.MasterTableViewHeader.Control,_205,_206,_208.Owner.MasterTableViewHeader.ColGroup.Cols);
},0);
}
}
}else{
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewHeader.Control,_205,_206,this.Owner.MasterTableViewHeader.ColGroup.Cols);
}
}
if(this.Owner.MasterTableView){
this.Owner.MasterTableView.Columns[_205].FrozenDisplay=_206;
if(!window.netscape&&navigator.userAgent.toLowerCase().indexOf("safari")==-1){
this.HideShowCol(this.Owner.MasterTableView,_205,_206);
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
var _207=this.Owner.MasterTableView.Control.getElementsByTagName("select");
if(_207.length>0){
var _208=this;
setTimeout(function(){
RadGridNamespace.HideShowCells(_208.Owner.MasterTableView.Control,_205,_206,_208.Owner.MasterTableView.ColGroup.Cols);
},0);
}
}
}else{
RadGridNamespace.HideShowCells(this.Owner.MasterTableView.Control,_205,_206,this.Owner.MasterTableView.ColGroup.Cols);
}
}
if(this.Owner.MasterTableViewFooter){
this.Owner.MasterTableViewFooter.Columns[_205].FrozenDisplay=_206;
if(!window.netscape&&navigator.userAgent.toLowerCase().indexOf("safari")==-1){
this.HideShowCol(this.Owner.MasterTableViewFooter,_205,_206);
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
var _207=this.Owner.MasterTableViewFooter.Control.getElementsByTagName("select");
if(_207.length>0){
var _208=this;
setTimeout(function(){
RadGridNamespace.HideShowCells(_208.Owner.MasterTableViewFooter.Control,_205,_206,_208.Owner.MasterTableViewFooter.ColGroup.Cols);
},0);
}
}
}else{
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewFooter.Control,_205,_206,this.Owner.MasterTableViewFooter.ColGroup.Cols);
}
}
};
RadGridNamespace.RadGridTable.prototype.HideColumn=function(_209){
if(!this.CanShowHideColumn(_209)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnHiding",[_209])){
return;
}
this.HideShowColumn(_209,false);
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("HidedColumns",this.ClientID,this.Columns[_209].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnColumnHidden",[_209]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ShowColumn=function(_20a){
if(!this.CanShowHideColumn(_20a)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnColumnShowing",[_20a])){
return;
}
this.HideShowColumn(_20a,true);
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("ShowedColumns",this.ClientID,this.Columns[_20a].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnColumnShowed",[_20a]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.HideShowCol=function(_20b,_20c,_20d){
if(_20b&&_20b.ColGroup&&_20b.ColGroup.Cols&&_20b.ColGroup.Cols[_20c]){
var _20e=(_20b.ColGroup.Cols[_20c].style.display=="")?true:false;
if(_20e!=_20d){
_20b.ColGroup.Cols[_20c].style.display=(_20d)?"":"none";
}
}
};
RadGridNamespace.RadGridTable.prototype.HideShowColumn=function(_20f,_210){
var _210=this.Columns[_20f].Display=_210;
if(this!=this.Owner.MasterTableViewHeader&&this!=this.Owner.MasterTableView&&this!=this.Owner.MasterTableViewFooter){
if(window.netscape||this.Owner.GridHeaderDiv){
this.HideShowCol(this,_20f,_210);
}
RadGridNamespace.HideShowCells(this.Control,_20f,_210,this.ColGroup.Cols);
return;
}
if(this.Owner.MasterTableViewHeader){
if(window.netscape||this.Owner.GridHeaderDiv){
this.HideShowCol(this.Owner.MasterTableViewHeader,_20f,_210);
}
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewHeader.Control,_20f,_210,this.Owner.MasterTableViewHeader.ColGroup.Cols);
}
if(this.Owner.MasterTableView){
if(window.netscape||this.Owner.GridHeaderDiv){
this.HideShowCol(this.Owner.MasterTableView,_20f,_210);
}
RadGridNamespace.HideShowCells(this.Owner.MasterTableView.Control,_20f,_210,this.Owner.MasterTableView.ColGroup.Cols);
}
if(this.Owner.MasterTableViewFooter){
if(window.netscape||this.Owner.GridHeaderDiv){
this.HideShowCol(this.Owner.MasterTableViewFooter,_20f,_210);
}
RadGridNamespace.HideShowCells(this.Owner.MasterTableViewFooter.Control,_20f,_210,this.Owner.MasterTableViewFooter.ColGroup.Cols);
}
};
RadGridNamespace.RadGridTable.prototype.CanShowHideRow=function(_211){
if(!this.Owner.ClientSettings.AllowRowHide){
return false;
}
if(isNaN(parseInt(_211))){
var _212="Row index must be of type \"Number\"!";
alert(_212);
return false;
}
if(_211<0){
var _212="Row index must be non-negative!";
alert(_212);
return false;
}
if(_211>(this.Rows.length-1)){
var _212="Row index must be less than rows count!";
alert(_212);
return false;
}
return true;
};
RadGridNamespace.RadGridTable.prototype.HideRow=function(_213){
if(!this.CanShowHideRow(_213)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnRowHiding",[_213])){
return;
}
if(this.Rows){
if(this.Rows[_213]){
if(this.Rows[_213].Control){
this.Rows[_213].Control.style.display="none";
this.Rows[_213].Display=false;
}
}
}
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("HidedRows",this.ClientID,this.Rows[_213].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnRowHidden",[_213]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ShowRow=function(_214){
if(!this.CanShowHideRow(_214)){
return false;
}
try{
if(!RadGridNamespace.FireEvent(this,"OnRowShowing",[_214])){
return;
}
if(this.Rows){
if(this.Rows[_214]){
if(this.Rows[_214].Control){
if(this.Rows[_214].ItemType!="NestedView"){
if(window.netscape){
this.Rows[_214].Control.style.display="table-row";
}else{
this.Rows[_214].Control.style.display="";
}
this.Rows[_214].Display=true;
}
}
}
}
if(this!=this.Owner.MasterTableViewHeader){
this.Owner.SavePostData("ShowedRows",this.ClientID,this.Rows[_214].RealIndex);
}
RadGridNamespace.FireEvent(this,"OnRowShowed",[_214]);
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.OnError);
}
};
RadGridNamespace.RadGridTable.prototype.ExportToExcel=function(_215){
try{
this.Owner.SavePostData("ExportToExcel",this.ClientID,_215);
__doPostBack(this.Owner.UniqueID,"ExportToExcel");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ExportToWord=function(_216){
try{
this.Owner.SavePostData("ExportToWord",this.ClientID,_216);
__doPostBack(this.Owner.UniqueID,"ExportToWord");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ExportToCSV=function(_217){
try{
this.Owner.SavePostData("ExportToCSV",this.ClientID,_217);
__doPostBack(this.Owner.UniqueID,"ExportToCSV");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ExportToPdf=function(_218){
try{
this.Owner.SavePostData("ExportToPdf",this.ClientID,_218);
__doPostBack(this.Owner.UniqueID,"ExportToPdf");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.AddToSelectedRows=function(_219){
try{
this.SelectedRows[this.SelectedRows.length]=_219;
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.IsInSelectedRows=function(_21a){
try{
for(var i=0;i<this.SelectedRows.length;i++){
if(this.SelectedRows[i]!=_21a){
return true;
}
}
return false;
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.ClearSelectedRows=function(){
var _21c=this.SelectedRows;
for(var i=0;i<this.SelectedRows.length;i++){
if(!RadGridNamespace.FireEvent(this,"OnRowDeselecting",[this.SelectedRows[i]])){
continue;
}
this.SelectedRows[i].Selected=false;
this.SelectedRows[i].CheckClientSelectColumns();
this.SelectedRows[i].RemoveSelectedRowStyle();
var last=this.SelectedRows[i];
try{
this.SelectedRows.splice(i,1);
i--;
}
catch(ex){
}
RadGridNamespace.FireEvent(this,"OnRowDeselected",[last]);
}
this.SelectedRows=new Array();
};
RadGridNamespace.RadGridTable.prototype.RemoveFromSelectedRows=function(_21f){
try{
var _220=new Array();
for(var i=0;i<this.SelectedRows.length;i++){
var last=this.SelectedRows[i];
if(this.SelectedRows[i]!=_21f){
_220[_220.length]=this.SelectedRows[i];
}else{
if(!this.Owner.AllowMultiRowSelection){
if(!RadGridNamespace.FireEvent(this,"OnRowDeselecting",[this.SelectedRows[i]])){
continue;
}
}
try{
this.SelectedRows.splice(i,1);
i--;
}
catch(ex){
}
_21f.CheckClientSelectColumns();
setTimeout(function(){
RadGridNamespace.FireEvent(_21f.Owner,"OnRowDeselected",[_21f]);
},100);
}
}
this.SelectedRows=_220;
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.GetSelectedRowsIndexes=function(){
try{
var _223=new Array();
for(var i=0;i<this.SelectedRows.length;i++){
_223[_223.length]=this.SelectedRows[i].RealIndex;
}
return _223.join(",");
}
catch(e){
throw e;
}
};
RadGridNamespace.RadGridTable.prototype.GetCellByColumnUniqueName=function(_225,_226){
if(this.ClientID.indexOf("_Header")!=-1){
return;
}
if((!_225)||(!_226)){
return;
}
if(!this.Columns){
return;
}
for(var i=0;i<this.Columns.length;i++){
if(this.Columns[i].UniqueName.toUpperCase()==_226.toUpperCase()){
return _225.Control.cells[i];
}
}
return null;
};
RadGridNamespace.RadGridTableColumn=function(_228){
if((!_228)||typeof (_228)!="object"){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
for(var _229 in _228){
this[_229]=_228[_229];
}
this.Type="RadGridTableColumn";
this.ResizeTolerance=5;
this.CanResize=false;
};
RadGridNamespace.RadGridTableColumn.prototype._constructor=function(_22a,_22b){
this.Control=_22a;
this.Owner=_22b;
this.Index=_22a.cellIndex;
if((window.opera&&typeof (_22a.cellIndex)=="undefined")||(navigator.userAgent.indexOf("Safari")!=-1)){
var _22c=this;
setTimeout(function(){
_22c.Index=RadGridNamespace.GetRealCellIndex(_22c.Owner,_22c.Control);
},200);
}
this.AttachDomEvent(this.Control,"click","OnClick");
this.AttachDomEvent(this.Control,"dblclick","OnDblClick");
this.AttachDomEvent(this.Control,"mousemove","OnMouseMove");
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
this.AttachDomEvent(this.Control,"mouseup","OnMouseUp");
this.AttachDomEvent(this.Control,"mouseover","OnMouseOver");
this.AttachDomEvent(this.Control,"mouseout","OnMouseOut");
this.AttachDomEvent(this.Control,"contextmenu","OnContextMenu");
};
RadGridNamespace.RadGridTableColumn.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
if(this.ColumnResizer){
this.ColumnResizer.Dispose();
}
this.Control=null;
this.Owner=null;
this.Index=null;
};
RadGridNamespace.RadGridTableColumn.prototype.OnContextMenu=function(e){
try{
this.Index=RadGridNamespace.GetRealCellIndex(this.Owner,this.Control);
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnContextMenu",[this.Index,e])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnClick=function(e){
try{
this.Index=RadGridNamespace.GetRealCellIndex(this.Owner,this.Control);
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnClick",[this.Index])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnDblClick=function(e){
try{
this.Index=RadGridNamespace.GetRealCellIndex(this.Owner,this.Control);
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnDblClick",[this.Index])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseMove=function(e){
if(this.Owner.Owner.ClientSettings&&this.Owner.Owner.ClientSettings.Resizing.AllowColumnResize&&this.Resizable&&this.Control.tagName.toLowerCase()=="th"){
var _231=RadGridNamespace.GetEventPosX(e);
var _232=RadGridNamespace.FindPosX(this.Control);
var endX=_232+this.Control.offsetWidth;
var _234=RadGridNamespace.GetCurrentElement(e);
if(!_234.initalCursorStyle){
_234.initalCursorStyle=_234.style.cursor;
}
if(this.Owner.Owner.GridDataDiv&&!this.Owner.Owner.GridHeaderDiv&&!window.netscape){
var _235=0;
if(document.body.currentStyle&&document.body.currentStyle.margin&&document.body.currentStyle.margin.indexOf("px")!=-1&&!window.opera){
_235=parseInt(document.body.currentStyle.marginLeft);
}
this.ResizeTolerance=10;
}
if((_231>=endX-this.ResizeTolerance)&&(_231<=endX+this.ResizeTolerance)&&!this.Owner.Owner.MoveHeaderDiv){
this.Control.style.cursor="e-resize";
this.Control.title=this.Owner.Owner.ClientSettings.ClientMessages.DragToResize;
this.CanResize=true;
_234.style.cursor="e-resize";
this.Owner.Owner.IsResize=true;
}else{
this.Control.style.cursor="";
this.Control.title="";
this.CanResize=false;
_234.style.cursor=_234.initalCursorStyle;
this.Owner.Owner.IsResize=false;
}
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseDown=function(e){
this.AttachDomEvent(document,"mouseup","OnMouseUp");
if(this.CanResize){
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1)&&(e.button==0))||(e.button==1)){
var _237=RadGridNamespace.GetEventPosX(e);
var _238=RadGridNamespace.FindPosX(this.Control);
var endX=_238+this.Control.offsetWidth;
if((_237>=endX-this.ResizeTolerance)&&(_237<=endX+this.ResizeTolerance)){
this.ColumnResizer=new RadGridNamespace.RadGridColumnResizer(this,this.Owner.Owner.ClientSettings.Resizing.EnableRealTimeResize);
this.ColumnResizer.Position(e);
}
}
RadGridNamespace.ClearDocumentEvents();
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseUp=function(e){
this.DetachDomEvent(document,"mouseup","OnMouseUp");
RadGridNamespace.RestoreDocumentEvents();
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseOver=function(e){
this.Index=RadGridNamespace.GetRealCellIndex(this.Owner,this.Control);
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnMouseOver",[this.Index])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.addClassName(this.Control,"GridHeaderOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridTableColumn.prototype.OnMouseOut=function(e){
this.Index=RadGridNamespace.GetRealCellIndex(this.Owner,this.Control);
if(!RadGridNamespace.FireEvent(this.Owner,"OnColumnMouseOut",[this.Index])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.removeClassName(this.Control,"GridHeaderOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridColumnResizer=function(_23d,_23e){
if(!_23d){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
this.Column=_23d;
this.IsRealTimeResize=_23e;
this.CurrentWidth=null;
this.LeftResizer=document.createElement("span");
this.LeftResizer.style.backgroundColor="navy";
this.LeftResizer.style.width="1"+"px";
this.LeftResizer.style.position="absolute";
this.LeftResizer.style.cursor="e-resize";
this.RightResizer=document.createElement("span");
this.RightResizer.style.backgroundColor="navy";
this.RightResizer.style.width="1"+"px";
this.RightResizer.style.position="absolute";
this.RightResizer.style.cursor="e-resize";
this.ResizerToolTip=document.createElement("span");
this.ResizerToolTip.style.backgroundColor="#F5F5DC";
this.ResizerToolTip.style.border="1px solid";
this.ResizerToolTip.style.position="absolute";
this.ResizerToolTip.style.font="icon";
this.ResizerToolTip.style.padding="2";
this.ResizerToolTip.innerHTML="Width: <b>"+this.Column.Control.offsetWidth+"</b> <em>pixels</em>";
document.body.appendChild(this.LeftResizer);
document.body.appendChild(this.RightResizer);
document.body.appendChild(this.ResizerToolTip);
this.CanDestroy=true;
this.AttachDomEvent(document,"mouseup","OnMouseUp");
this.AttachDomEvent(this.Column.Owner.Owner.Control,"mousemove","OnMouseMove");
};
RadGridNamespace.RadGridColumnResizer.prototype.OnMouseUp=function(e){
this.Destroy(e);
};
RadGridNamespace.RadGridColumnResizer.prototype.OnMouseMove=function(e){
this.Move(e);
};
RadGridNamespace.RadGridColumnResizer.prototype.Position=function(e){
this.LeftResizer.style.top=RadGridNamespace.FindPosY(this.Column.Control)-RadGridNamespace.FindScrollPosY(this.Column.Control)+document.documentElement.scrollTop+document.body.scrollTop+"px";
this.LeftResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)-RadGridNamespace.FindScrollPosX(this.Column.Control)+document.documentElement.scrollLeft+document.body.scrollLeft+"px";
this.RightResizer.style.top=this.LeftResizer.style.top;
this.RightResizer.style.left=parseInt(this.LeftResizer.style.left)+this.Column.Control.offsetWidth+"px";
this.ResizerToolTip.style.top=parseInt(this.RightResizer.style.top)-20+"px";
this.ResizerToolTip.style.left=parseInt(this.RightResizer.style.left)-5+"px";
if(parseInt(this.LeftResizer.style.left)<RadGridNamespace.FindPosX(this.Column.Owner.Control)){
this.LeftResizer.style.display="none";
}
if(!this.Column.Owner.Owner.ClientSettings.Scrolling.AllowScroll){
this.LeftResizer.style.height=this.Column.Owner.Control.tBodies[0].offsetHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
if(this.Column.Owner.Owner.ClientSettings.Scrolling.UseStaticHeaders){
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+"px";
}
}
this.RightResizer.style.height=this.LeftResizer.style.height;
};
RadGridNamespace.RadGridColumnResizer.prototype.Destroy=function(e){
if(this.CanDestroy){
this.DetachDomEvent(document,"mouseup","OnMouseUp");
this.DetachDomEvent(this.Column.Owner.Owner.Control,"mousemove","OnMouseMove");
if(this.CurrentWidth!=null){
if(this.CurrentWidth>0){
this.Column.Owner.ResizeColumn(this.Column.Control.cellIndex,this.CurrentWidth);
this.CurrentWidth=null;
}
}
document.body.removeChild(this.LeftResizer);
document.body.removeChild(this.RightResizer);
document.body.removeChild(this.ResizerToolTip);
this.CanDestroy=false;
}
};
RadGridNamespace.RadGridColumnResizer.prototype.Dispose=function(){
try{
this.Destroy();
}
catch(error){
}
this.DisposeDomEventHandlers();
this.MouseUpHandler=null;
this.MouseMoveHandler=null;
this.LeftResizer=null;
this.RightResizer=null;
this.ResizerToolTip=null;
};
RadGridNamespace.RadGridColumnResizer.prototype.Move=function(e){
this.LeftResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)-RadGridNamespace.FindScrollPosX(this.Column.Control)+document.documentElement.scrollLeft+document.body.scrollLeft+"px";
this.RightResizer.style.left=parseInt(this.LeftResizer.style.left)+(RadGridNamespace.GetEventPosX(e)-RadGridNamespace.FindPosX(this.Column.Control))+"px";
this.ResizerToolTip.style.left=parseInt(this.RightResizer.style.left)-5+"px";
var _244=parseInt(this.RightResizer.style.left)-parseInt(this.LeftResizer.style.left);
var _245=this.Column.Control.scrollWidth-_244;
this.ResizerToolTip.innerHTML="Width: <b>"+_244+"</b> <em>pixels</em>";
if(!RadGridNamespace.FireEvent(this.Column.Owner,"OnColumnResizing",[this.Column.Index,_244])){
return;
}
if(_244<=0){
this.RightResizer.style.left=this.RightResizer.style.left;
this.Destroy(e);
return;
}
this.CurrentWidth=_244;
if(this.IsRealTimeResize){
var _246=(navigator.userAgent.indexOf("Safari")!=-1)?RadGridNamespace.GetRealCellIndex(this.Column.Owner,this.Column.Control):this.Column.Control.cellIndex;
this.Column.Owner.ResizeColumn(_246,_244);
}else{
this.CurrentWidth=_244;
return;
}
if(RadGridNamespace.FindPosX(this.LeftResizer)!=RadGridNamespace.FindPosX(this.Column.Control)){
this.LeftResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)+"px";
}
if(RadGridNamespace.FindPosX(this.RightResizer)!=(RadGridNamespace.FindPosX(this.Column.Control)+this.Column.Control.offsetWidth)){
this.RightResizer.style.left=RadGridNamespace.FindPosX(this.Column.Control)+this.Column.Control.offsetWidth+"px";
}
if(RadGridNamespace.FindPosY(this.LeftResizer)!=RadGridNamespace.FindPosY(this.Column.Control)){
this.LeftResizer.style.top=RadGridNamespace.FindPosY(this.Column.Control)+"px";
this.RightResizer.style.top=RadGridNamespace.FindPosY(this.Column.Control)+"px";
}
if(this.Column.Owner.Owner.GridDataDiv){
this.LeftResizer.style.left=parseInt(this.LeftResizer.style.left.replace("px",""))-this.Column.Owner.Owner.GridDataDiv.scrollLeft+"px";
this.RightResizer.style.left=parseInt(this.LeftResizer.style.left.replace("px",""))+this.Column.Control.offsetWidth+"px";
this.ResizerToolTip.style.left=parseInt(this.RightResizer.style.left)-5+"px";
}
if(!this.Column.Owner.Owner.ClientSettings.Scrolling.AllowScroll){
this.LeftResizer.style.height=this.Column.Owner.Control.tBodies[0].offsetHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
if(this.Column.Owner.Owner.ClientSettings.Scrolling.UseStaticHeaders){
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+this.Column.Owner.Control.tHead.offsetHeight+"px";
}else{
this.LeftResizer.style.height=this.Column.Owner.Owner.GridDataDiv.clientHeight+"px";
}
}
this.RightResizer.style.height=this.LeftResizer.style.height;
};
RadGridNamespace.RadGridTableRow=function(_247){
if((!_247)||typeof (_247)!="object"){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
for(var _248 in _247){
this[_248]=_247[_248];
}
this.Type="RadGridTableRow";
var _249=document.getElementById(this.OwnerID);
this.Control=_249.tBodies[0].rows[this.ClientRowIndex];
if(!this.Control){
return;
}
this.Index=this.Control.sectionRowIndex;
this.RealIndex=this.RowIndex;
};
RadGridNamespace.RadGridTableRow.prototype._constructor=function(_24a){
this.Owner=_24a;
this.CreateStyles();
if(this.Selected){
this.LoadSelected();
}
this.CheckClientSelectColumns();
if(this.Owner.HierarchyLoadMode=="Client"){
if(this.Owner.Owner.ClientSettings.AllowExpandCollapse){
for(var i=0;i<this.Owner.ExpandCollapseColumns.length;i++){
var _24c=this.Owner.ExpandCollapseColumns[i].Control.cellIndex;
var _24d=this.Control.cells[_24c];
var html=this.Control.innerHTML;
if(!_24d){
continue;
}
var _24f;
for(var j=0;j<_24d.childNodes.length;j++){
if(!_24d.childNodes[j].tagName){
continue;
}
var _251;
if(this.Owner.ExpandCollapseColumns[i].ButtonType=="ImageButton"){
_251="img";
}else{
if(this.Owner.ExpandCollapseColumns[i].ButtonType=="LinkButton"){
_251="a";
}else{
if(this.Owner.ExpandCollapseColumns[i].ButtonType=="PushButton"){
_251="button";
}
}
}
if(_24d.childNodes[j].tagName.toLowerCase()==_251){
_24f=_24d.childNodes[j];
break;
}
}
if(_24f){
var _252=this;
var _253=function(){
_252.OnHierarchyExpandButtonClick(this);
};
_24f.onclick=_253;
_24f.ondblclick=null;
_253=null;
}
_24f=null;
}
}
}
if(this.Owner.GroupLoadMode=="Client"){
if(this.Owner.Owner.ClientSettings.AllowGroupExpandCollapse){
for(var i=0;i<this.Owner.GroupSplitterColumns.length;i++){
var _24c=this.Owner.GroupSplitterColumns[i].Control.cellIndex;
var html=this.Control.innerHTML;
var _24d=this.Control.cells[_24c];
if(!_24d){
continue;
}
var _24f;
for(var j=0;j<_24d.childNodes.length;j++){
if(!_24d.childNodes[j].tagName){
continue;
}
if(_24d.childNodes[j].tagName.toLowerCase()=="img"){
_24f=_24d.childNodes[j];
break;
}
}
if(_24f){
var _252=this;
var _253=function(){
_252.OnGroupExpandButtonClick(this);
};
_24f.onclick=_253;
_24f.ondblclick=null;
_253=null;
}
_24f=null;
}
}
}
this.AttachDomEvent(this.Control,"click","OnClick");
this.AttachDomEvent(this.Control,"dblclick","OnDblClick");
this.AttachDomEvent(document,"mousedown","OnMouseDown");
this.AttachDomEvent(document,"mouseup","OnMouseUp");
this.AttachDomEvent(document,"mousemove","OnMouseMove");
this.AttachDomEvent(this.Control,"mouseover","OnMouseOver");
this.AttachDomEvent(this.Control,"mouseout","OnMouseOut");
this.AttachDomEvent(this.Control,"contextmenu","OnContextMenu");
if(this.Owner.Owner.ClientSettings.ActiveRowData&&this.Owner.Owner.ClientSettings.ActiveRowData!=""){
var data=this.Owner.Owner.ClientSettings.ActiveRowData.split(";")[0].split(",");
if(data[0]==this.Owner.ClientID&&data[1]==this.RealIndex){
this.Owner.Owner.ActiveRow=this;
}
}
};
RadGridNamespace.GroupRowExpander=function(_255){
this.startRow=_255;
};
RadGridNamespace.GroupRowExpander.prototype.NotFinished=function(_256){
var _257=(this.currentGridRow!=null);
if(!_257){
return false;
}
var _258=(this.currentGridRow.GroupIndex=="");
var _259=(this.currentGridRow.GroupIndex==_256.GroupIndex);
var _25a=(this.currentGridRow.GroupIndex.indexOf(_256.GroupIndex+"_")==0);
return (_258||_259||_25a);
};
RadGridNamespace.GroupRowExpander.prototype.ToggleExpandCollapse=function(_25b){
var _25c=this.startRow;
var _25d=_25c.Owner;
var _25e=_25b.parentNode.parentNode.sectionRowIndex;
var _25f=_25d.Rows[_25e];
if(_25f.Expanded){
if(!RadGridNamespace.FireEvent(_25f.Owner,"OnGroupCollapsing",[_25f])){
return;
}
}else{
if(!RadGridNamespace.FireEvent(_25f.Owner,"OnGroupExpanding",[_25f])){
return;
}
}
var _260=_25d.Control.rows[_25e+1];
if(!_260){
return;
}
this.currentRowIndex=_260.rowIndex;
this.lastGroupIndex=null;
while(true){
this.currentGridRow=_25d.Rows[this.currentRowIndex];
var _261=this.NotFinished(_25f);
if(!_261){
break;
}
var _262=(this.lastGroupIndex!=null)&&(this.currentGridRow.GroupIndex.indexOf(this.lastGroupIndex)!=-1);
var _263=(this.currentGridRow.ItemType!="GroupHeader")&&(!this.currentGridRow.IsVisible());
var _264=_262&&_263;
if(this.currentGridRow.ItemType=="GroupHeader"&&!this.currentGridRow.Expanded){
if(this.currentGridRow.IsVisible()){
this.currentGridRow.Hide();
_25b.src=_25d.GroupSplitterColumns[0].ExpandImageUrl;
_25b.title=_25d.Owner.GroupingSettings.ExpandTooltip;
if(_25d.Rows[this.currentRowIndex+1]==null||_25d.Rows[this.currentRowIndex+1].ItemType=="GroupHeader"){
this.currentGridRow.Expanded=false;
}
}else{
_25b.src=_25d.GroupSplitterColumns[0].CollapseImageUrl;
_25b.title=_25d.Owner.GroupingSettings.CollapseTooltip;
this.currentGridRow.Show();
if(_25d.Rows[this.currentRowIndex+1]==null||_25d.Rows[this.currentRowIndex+1].ItemType=="GroupHeader"){
this.currentGridRow.Expanded=true;
}
}
this.lastGroupIndex=this.currentGridRow.GroupIndex;
}else{
if(!_264){
if(this.currentGridRow.ItemType=="NestedView"){
if(this.currentGridRow.Expanded){
if(this.currentGridRow.IsVisible()){
this.currentGridRow.Hide();
}else{
this.currentGridRow.Show();
}
}
}else{
if(this.currentGridRow.IsVisible()){
this.currentGridRow.Hide();
_25b.src=_25d.GroupSplitterColumns[0].ExpandImageUrl;
_25b.title=_25d.Owner.GroupingSettings.ExpandTooltip;
_25f.Expanded=false;
}else{
_25b.src=_25d.GroupSplitterColumns[0].CollapseImageUrl;
_25b.title=_25d.Owner.GroupingSettings.CollapseTooltip;
this.currentGridRow.Show();
_25f.Expanded=true;
}
}
}
}
this.currentRowIndex++;
}
if(_25f.Expanded!=null){
if(_25f.Expanded){
_25d.Owner.SavePostData("ExpandedGroupRows",_25d.ClientID,_25f.RealIndex);
_25c.title=_25d.Owner.GroupingSettings.CollapseTooltip;
}else{
_25d.Owner.SavePostData("CollapsedGroupRows",_25d.ClientID,_25f.RealIndex);
_25c.title=_25d.Owner.GroupingSettings.ExpandTooltip;
}
}
if(_25f.Expanded){
if(!RadGridNamespace.FireEvent(_25f.Owner,"OnGroupExpanded",[_25f])){
return;
}
}else{
if(!RadGridNamespace.FireEvent(_25f.Owner,"OnGroupCollapsed",[_25f])){
return;
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnGroupExpandButtonClick=function(_265){
var _266=new RadGridNamespace.GroupRowExpander(this);
_266.ToggleExpandCollapse(_265);
};
RadGridNamespace.RadGridTableRow.prototype.OnHierarchyExpandButtonClick=function(_267){
var _268=this.Owner.Control.rows[_267.parentNode.parentNode.rowIndex+1];
var _269=this.Owner.Rows[_267.parentNode.parentNode.sectionRowIndex];
if(!_268){
return;
}
if(this.TableRowIsVisible(_268)){
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyCollapsing",[this])){
return;
}
this.HideTableRow(_268);
_269.Expanded=false;
if(this.Owner.ExpandCollapseColumns[0].ButtonType=="ImageButton"){
_267.src=this.Owner.ExpandCollapseColumns[0].ExpandImageUrl;
}else{
_267.innerHTML="+";
}
_267.title=this.Owner.Owner.HierarchySettings.ExpandTooltip;
this.Owner.Owner.SavePostData("CollapsedRows",this.Owner.ClientID,this.RealIndex);
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyCollapsed",[this])){
return;
}
}else{
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyExpanding",[this])){
return;
}
if(this.Owner.ExpandCollapseColumns[0].ButtonType=="ImageButton"){
_267.src=this.Owner.ExpandCollapseColumns[0].CollapseImageUrl;
}else{
_267.innerHTML="-";
}
_267.title=this.Owner.Owner.HierarchySettings.CollapseTooltip;
this.ShowTableRow(_268);
_269.Expanded=true;
this.Owner.Owner.SavePostData("ExpandedRows",this.Owner.ClientID,this.RealIndex);
if(!RadGridNamespace.FireEvent(this.Owner,"OnHierarchyExpanded",[this])){
return;
}
}
};
RadGridNamespace.RadGridTableRow.prototype.TableRowIsVisible=function(_26a){
return _26a.style.display!="none";
};
RadGridNamespace.RadGridTableRow.prototype.IsVisible=function(){
return this.TableRowIsVisible(this.Control);
};
RadGridNamespace.RadGridTableRow.prototype.HideTableRow=function(_26b){
if(this.TableRowIsVisible(_26b)){
_26b.style.display="none";
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
var _26c=_26b.getElementsByTagName("select");
for(var i=0;i<_26c.length;i++){
_26c[i].style.display="none";
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.Hide=function(){
this.HideTableRow(this.Control);
};
RadGridNamespace.RadGridTableRow.prototype.ShowTableRow=function(_26e){
if(window.netscape||window.opera){
_26e.style.display="table-row";
}else{
_26e.style.display="block";
if(navigator.userAgent.toLowerCase().indexOf("msie")!=-1&&navigator.userAgent.toLowerCase().indexOf("6.0")!=-1){
var _26f=_26e.getElementsByTagName("select");
for(var i=0;i<_26f.length;i++){
_26f[i].style.display="";
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.Show=function(){
this.ShowTableRow(this.Control);
};
RadGridNamespace.RadGridTableRow.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
this.Control=null;
this.Owner=null;
};
RadGridNamespace.RadGridTableRow.prototype.CreateStyles=function(){
if(!this.Owner.Owner.ClientSettings.ApplyStylesOnClient){
return;
}
switch(this.ItemType){
case "GroupHeader":
break;
case "EditFormItem":
this.Control.className+=" "+this.Owner.RenderEditItemStyleClass;
this.Control.style.cssText+=" "+this.Owner.RenderEditItemStyle;
break;
default:
var _271=eval("this.Owner.Render"+this.ItemType+"StyleClass");
if(typeof (_271)!="undefined"){
this.Control.className+=" "+_271;
}
var _272=eval("this.Owner.Render"+this.ItemType+"Style");
if(typeof (_272)!="undefined"){
this.Control.style.cssText+=" "+_272;
}
break;
}
if(!this.Display){
if(this.Control.style.cssText!=""){
if(this.Control.style.cssText.lastIndexOf(";")==this.Control.style.cssText.length-1){
this.Control.style.cssText+="display:none;";
}else{
this.Control.style.cssText+=";display:none;";
}
}else{
this.Control.style.cssText+="display:none;";
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnContextMenu=function(e){
try{
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowContextMenu",[this.Index,e])){
return;
}
if(this.Owner.Owner.ClientSettings.ClientEvents.OnRowContextMenu!=""){
if(e.preventDefault){
e.preventDefault();
}else{
e.returnValue=false;
return false;
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableRow.prototype.OnClick=function(e){
try{
if(this.Owner.Owner.RowResizer){
return;
}
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowClick",[this.Control.sectionRowIndex,e])){
return;
}
if(this.ItemType=="EditFormItem"){
return;
}
if(e.shiftKey&&this.Owner.SelectedRows[0]){
if(this.Owner.SelectedRows[0].Control.rowIndex>this.Control.rowIndex){
for(var i=this.Control.rowIndex;i<this.Owner.SelectedRows[0].Control.rowIndex+1;i++){
var _276=this.Owner.Owner.GetRowObjectByRealRow(this.Owner,this.Owner.Control.rows[i]);
if(_276){
if(!_276.Selected){
this.Owner.SelectRow(this.Owner.Control.rows[i],false);
}
}
}
}
if(this.Owner.SelectedRows[0].Control.rowIndex<this.Control.rowIndex){
for(var i=this.Owner.SelectedRows[0].Control.rowIndex;i<this.Control.rowIndex+1;i++){
var _276=this.Owner.Owner.GetRowObjectByRealRow(this.Owner,this.Owner.Control.rows[i]);
if(_276){
if(!_276.Selected){
this.Owner.SelectRow(this.Owner.Control.rows[i],false);
}
}
}
}
}
if(!e.shiftKey){
this.HandleRowSelection(e);
}
var _277=RadGridNamespace.GetCurrentElement(e);
if(!_277){
return;
}
if(!_277.tagName){
return;
}
if(_277.tagName.toLowerCase()=="input"||_277.tagName.toLowerCase()=="select"||_277.tagName.toLowerCase()=="option"||_277.tagName.toLowerCase()=="button"||_277.tagName.toLowerCase()=="a"||_277.tagName.toLowerCase()=="textarea"){
return;
}
if(this.ItemType=="Item"||this.ItemType=="AlternatingItem"){
if(this.Owner.Owner.ClientSettings.EnablePostBackOnRowClick){
var _278=this.Owner.Owner.ClientSettings.PostBackFunction;
_278=_278.replace("{0}",this.Owner.Owner.UniqueID).replace("{1}","RowClick;"+this.ItemIndexHierarchical);
var form=document.getElementById(this.Owner.Owner.FormID);
if(form!=null&&form["__EVENTTARGET"]!=null&&form["__EVENTTARGET"].value==this.Owner.Owner.UniqueID){
form["__EVENTTARGET"].value="";
}
if(form!=null&&form["__EVENTTARGET"]!=null&&form["__EVENTTARGET"].value==""){
eval(_278);
}
}
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableRow.prototype.HandleActiveRow=function(e){
var _27b=RadGridNamespace.GetCurrentElement(e);
if(_27b!=null&&_27b.tagName&&(_27b.tagName.toLowerCase()=="input"||_27b.tagName.toLowerCase()=="textarea")){
return;
}
var _27c={13:"",40:"",39:"",38:"",37:"",32:""};
if(this.Owner.Owner.ActiveRow!=null){
if(!RadGridNamespace.FireEvent(this.Owner,"OnActiveRowChanging",[this.Owner.Owner.ActiveRow])){
return;
}
if(e.keyCode==13){
this.Owner.Owner.SavePostData("EditRow",this.Owner.ClientID,this.Owner.Owner.ActiveRow.RealIndex);
eval(this.Owner.Owner.ClientSettings.PostBackReferences.PostBackEditRow);
}
if(e.keyCode==40){
var _27d=this.Owner.Rows[this.Owner.Owner.ActiveRow.Control.sectionRowIndex+1];
if(_27d!=null){
this.Owner.Owner.SetActiveRow(_27d);
this.ScrollIntoView(_27d);
}
}
if(e.keyCode==39){
return;
var _27d=this.Owner.Owner.GetNextHierarchicalRow(_27e,this.Owner.Owner.ActiveRow.Control.sectionRowIndex);
if(_27d!=null){
_27e=_27d.parentNode.parentNode;
this.Owner.Owner.SetActiveRow(_27e,_27d.sectionRowIndex);
this.ScrollIntoView(_27d);
}
}
if(e.keyCode==38){
var _27f=this.Owner.Rows[this.Owner.Owner.ActiveRow.Control.sectionRowIndex-1];
if(_27f!=null){
this.Owner.Owner.SetActiveRow(_27f);
this.ScrollIntoView(_27f);
}
}
if(e.keyCode==37){
return;
var _27f=this.Owner.Owner.GetPreviousHierarchicalRow(_27e,this.Owner.Owner.ActiveRow.Control.sectionRowIndex);
if(_27f!=null){
var _27e=_27f.parentNode.parentNode;
this.Owner.Owner.SetActiveRow(_27e,_27f.sectionRowIndex);
this.ScrollIntoView(_27f);
}
}
if(e.keyCode==32){
if(this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect){
this.Owner.Owner.ActiveRow.Owner.SelectRow(this.Owner.Owner.ActiveRow.Control,!this.Owner.Owner.AllowMultiRowSelection);
}
}
}
RadGridNamespace.FireEvent(this.Owner,"OnActiveRowChanged",[this.Owner.Owner.ActiveRow]);
if(_27c[e.keyCode]!=null){
if(window.netscape){
e.preventDefault();
return false;
}else{
e.returnValue=false;
}
}
};
RadGridNamespace.RadGridTableRow.prototype.ScrollIntoView=function(row){
if(row.Control&&row.Control.focus){
row.Control.scrollIntoView(false);
try{
row.Control.focus();
}
catch(e){
}
}
};
RadGridNamespace.RadGridTableRow.prototype.HandleExpandCollapse=function(){
};
RadGridNamespace.RadGridTableRow.prototype.HandleGroupExpandCollapse=function(){
};
RadGridNamespace.RadGridTableRow.prototype.HandleRowSelection=function(e){
var _282=RadGridNamespace.GetCurrentElement(e);
if(_282.onclick){
return;
}
if(_282.tagName.toLowerCase()=="input"||_282.tagName.toLowerCase()=="select"||_282.tagName.toLowerCase()=="option"||_282.tagName.toLowerCase()=="button"||_282.tagName.toLowerCase()=="a"||_282.tagName.toLowerCase()=="textarea"||_282.tagName.toLowerCase()=="img"){
return;
}
this.SetSelected(!e.ctrlKey,e);
};
RadGridNamespace.RadGridTableRow.prototype.CheckClientSelectColumns=function(){
if(!this.Owner.Columns){
return;
}
for(var i=0;i<this.Owner.Columns.length;i++){
if(this.Owner.Columns[i].ColumnType=="GridClientSelectColumn"){
var cell=this.Owner.GetCellByColumnUniqueName(this,this.Owner.Columns[i].UniqueName);
if(cell!=null){
var _285=cell.getElementsByTagName("input")[0];
if(_285!=null){
_285.checked=this.Selected;
}
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.SetSelected=function(_286,e){
if(!this.Selected){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowSelecting",[this,e])){
return;
}
}
if((this.ItemType=="Item")||(this.ItemType=="AlternatingItem")){
if(_286){
this.SingleSelect();
}else{
this.MultiSelect();
}
}
this.CheckClientSelectColumns();
if(this.Selected){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowSelected",[this,e])){
return;
}
}
};
RadGridNamespace.RadGridTableRow.prototype.SingleSelect=function(){
if(!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
this.Owner.ClearSelectedRows();
this.Owner.Owner.ClearSelectedRows();
this.Selected=true;
this.ApplySelectedRowStyle();
this.Owner.AddToSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
};
RadGridNamespace.RadGridTableRow.prototype.SingleDeselect=function(){
if(!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect){
return;
}
this.Owner.ClearSelectedRows();
this.Owner.Owner.ClearSelectedRows();
this.Selected=false;
this.RemoveSelectedRowStyle();
this.Owner.RemoveFromSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
};
RadGridNamespace.RadGridTableRow.prototype.MultiSelect=function(){
if((!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect)||(!this.Owner.Owner.AllowMultiRowSelection)){
return;
}
if(this.Selected){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowDeselecting",[this])){
return;
}
this.Selected=false;
this.RemoveSelectedRowStyle();
this.Owner.RemoveFromSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
}else{
this.Selected=true;
this.ApplySelectedRowStyle();
this.Owner.AddToSelectedRows(this);
this.Owner.Owner.UpdateClientRowSelection();
}
};
RadGridNamespace.RadGridTableRow.prototype.LoadSelected=function(){
this.ApplySelectedRowStyle();
this.Owner.AddToSelectedRows(this);
};
RadGridNamespace.RadGridTableRow.prototype.ApplySelectedRowStyle=function(){
if(!this.Owner.SelectedItemStyleClass||this.Owner.SelectedItemStyleClass==""){
if(this.Owner.SelectedItemStyle&&this.Owner.SelectedItemStyle!=""){
RadGridNamespace.addClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"1");
}else{
RadGridNamespace.addClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"2");
}
}else{
RadGridNamespace.addClassName(this.Control,this.Owner.SelectedItemStyleClass);
}
};
RadGridNamespace.RadGridTableRow.prototype.RemoveSelectedRowStyle=function(){
if(this.Owner.SelectedItemStyle){
RadGridNamespace.removeClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"1");
}else{
RadGridNamespace.removeClassName(this.Control,"SelectedItemStyle"+this.Owner.ClientID+"2");
}
RadGridNamespace.removeClassName(this.Control,this.Owner.SelectedItemStyleClass);
if(this.Control.style.cssText==this.Owner.SelectedItemStyle){
this.Control.style.cssText="";
}
};
RadGridNamespace.RadGridTableRow.prototype.OnDblClick=function(e){
try{
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowDblClick",[this.Control.sectionRowIndex,e])){
return;
}
}
catch(error){
new RadGridNamespace.Error(error,this,this.Owner.Owner.OnError);
}
};
RadGridNamespace.RadGridTableRow.prototype.CreateRowSelectorArea=function(e){
if((this.Owner.Owner.RowResizer)||(e.ctrlKey)){
return;
}
var _28a=null;
if(e.srcElement){
_28a=e.srcElement;
}else{
if(e.target){
_28a=e.target;
}
}
if(!_28a.tagName){
return;
}
if(_28a.tagName.toLowerCase()=="input"||_28a.tagName.toLowerCase()=="textarea"){
return;
}
if((!this.Owner.Owner.ClientSettings.Selecting.AllowRowSelect)||(!this.Owner.Owner.AllowMultiRowSelection)){
return;
}
var _28b=RadGridNamespace.GetCurrentElement(e);
if((!_28b)||(!RadGridNamespace.IsChildOf(_28b,this.Control))){
return;
}
if(!this.RowSelectorArea){
this.RowSelectorArea=document.createElement("span");
this.RowSelectorArea.style.backgroundColor="navy";
this.RowSelectorArea.style.border="indigo 1px solid";
this.RowSelectorArea.style.position="absolute";
this.RowSelectorArea.style.font="icon";
if(window.netscape&&!window.opera){
this.RowSelectorArea.style.MozOpacity=1/10;
}else{
if(window.opera||navigator.userAgent.indexOf("Safari")>-1){
this.RowSelectorArea.style.opacity=0.1;
}else{
this.RowSelectorArea.style.filter="alpha(opacity=10);";
}
}
if(this.Owner.Owner.GridDataDiv){
this.RowSelectorArea.style.top=RadGridNamespace.FindPosY(this.Control)-this.Owner.Owner.GridDataDiv.scrollTop+"px";
this.RowSelectorArea.style.left=RadGridNamespace.FindPosX(this.Control)-this.Owner.Owner.GridDataDiv.scrollLeft+"px";
if(parseInt(this.RowSelectorArea.style.left)<RadGridNamespace.FindPosX(this.Owner.Owner.Control)){
this.RowSelectorArea.style.left=RadGridNamespace.FindPosX(this.Owner.Owner.Control)+"px";
}
}else{
this.RowSelectorArea.style.top=RadGridNamespace.FindPosY(this.Control)+"px";
this.RowSelectorArea.style.left=RadGridNamespace.FindPosX(this.Control)+"px";
}
document.body.appendChild(this.RowSelectorArea);
this.FirstRow=this.Control;
RadGridNamespace.ClearDocumentEvents();
}
};
RadGridNamespace.RadGridTableRow.prototype.DestroyRowSelectorArea=function(e){
if(this.RowSelectorArea){
var _28d=this.RowSelectorArea.style.height;
document.body.removeChild(this.RowSelectorArea);
this.RowSelectorArea=null;
RadGridNamespace.RestoreDocumentEvents();
var _28e=RadGridNamespace.GetCurrentElement(e);
var _28f;
if((!_28e)||(!RadGridNamespace.IsChildOf(_28e,this.Owner.Control))){
return;
}
var _290=RadGridNamespace.GetFirstParentByTagName(_28e,"td");
if((_28e.tagName.toLowerCase()=="td")||(_28e.tagName.toLowerCase()=="tr")||_290.tagName.toLowerCase()=="td"){
if(_28e.tagName.toLowerCase()=="td"){
_28f=_28e.parentNode;
}else{
if(_290.tagName.toLowerCase()=="td"){
_28f=_290.parentNode;
}else{
if(_28e.tagName.toLowerCase()=="tr"){
_28f=_28e;
}
}
}
for(var i=this.FirstRow.rowIndex;i<_28f.rowIndex+1;i++){
var _292=this.Owner.Owner.GetRowObjectByRealRow(this.Owner,this.Owner.Control.rows[i]);
if(_292){
if(_28d!=""){
if(!_292.Selected){
this.Owner.SelectRow(this.Owner.Control.rows[i],false);
}
}
}
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.ResizeRowSelectorArea=function(e){
if((this.RowSelectorArea)&&(this.RowSelectorArea.parentNode)){
var _294=RadGridNamespace.GetCurrentElement(e);
if((!_294)||(!RadGridNamespace.IsChildOf(_294,this.Owner.Control))){
return;
}
var _295=parseInt(this.RowSelectorArea.style.left);
if(this.Owner.Owner.GridDataDiv){
var _296=RadGridNamespace.GetEventPosX(e)-this.Owner.Owner.GridDataDiv.scrollLeft;
}else{
var _296=RadGridNamespace.GetEventPosX(e);
}
var _297=parseInt(this.RowSelectorArea.style.top);
if(this.Owner.Owner.GridDataDiv){
var _298=RadGridNamespace.GetEventPosY(e)-this.Owner.Owner.GridDataDiv.scrollTop;
}else{
var _298=RadGridNamespace.GetEventPosY(e);
}
if((_296-_295-5)>0){
this.RowSelectorArea.style.width=_296-_295-5+"px";
}
if((_298-_297-5)>0){
this.RowSelectorArea.style.height=_298-_297-5+"px";
}
if(this.RowSelectorArea.offsetWidth>this.Owner.Control.offsetWidth){
this.RowSelectorArea.style.width=this.Owner.Control.offsetWidth+"px";
}
var _299=(RadGridNamespace.FindPosX(this.Owner.Control)+this.Owner.Control.offsetHeight)-parseInt(this.RowSelectorArea.style.top);
if(this.RowSelectorArea.offsetHeight>_299){
if(_299>0){
this.RowSelectorArea.style.height=_299+"px";
}
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseDown=function(e){
if(this.Owner.Owner.ClientSettings.Selecting.EnableDragToSelectRows&&this.Owner.Owner.AllowMultiRowSelection){
if(!this.Owner.Owner.RowResizer){
this.CreateRowSelectorArea(e);
}
}
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseUp=function(e){
this.DestroyRowSelectorArea(e);
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseMove=function(e){
this.ResizeRowSelectorArea(e);
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseOver=function(e){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowMouseOver",[this.Control.sectionRowIndex,e])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.addClassName(this.Control,"GridRowOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridTableRow.prototype.OnMouseOut=function(e){
if(!RadGridNamespace.FireEvent(this.Owner,"OnRowMouseOut",[this.Control.sectionRowIndex,e])){
return;
}
if(this.Owner.Owner.Skin!=""&&this.Owner.Owner.Skin!="None"){
RadGridNamespace.removeClassName(this.Control,"GridRowOver_"+this.Owner.Owner.Skin);
}
};
RadGridNamespace.RadGridGroupPanel=function(_29f,_2a0){
this.Control=_29f;
this.Owner=_2a0;
this.Items=new Array();
this.groupPanelItemCounter=0;
this.getGroupPanelItems(this.Control,0);
var _2a1=this;
};
RadGridNamespace.RadGridGroupPanel.prototype.Dispose=function(){
this.UnLoadHandler=null;
this.Control=null;
this.Owner=null;
this.DisposeItems();
for(var _2a2 in this){
this[_2a2]=null;
}
};
RadGridNamespace.RadGridGroupPanel.prototype.DisposeItems=function(){
if(this.Items!=null){
for(var i=0;i<this.Items.length;i++){
var item=this.Items[i];
item.Dispose();
}
}
};
RadGridNamespace.RadGridGroupPanel.prototype.groupPanelItemCounter=0;
RadGridNamespace.RadGridGroupPanel.prototype.getGroupPanelItems=function(_2a5){
for(var i=0;i<_2a5.rows.length;i++){
var _2a7=false;
var row=_2a5.rows[i];
for(var j=0;j<row.cells.length;j++){
var cell=row.cells[j];
if(cell.tagName.toLowerCase()=="th"){
var _2ab;
if(this.Owner.GroupPanel.GroupPanelItems[this.groupPanelItemCounter]){
_2ab=this.Owner.GroupPanel.GroupPanelItems[this.groupPanelItemCounter].HierarchicalIndex;
}
if(_2ab){
this.Items[this.Items.length]=new RadGridNamespace.RadGridGroupPanelItem(cell,this,_2ab);
_2a7=true;
this.groupPanelItemCounter++;
}
}
if((cell.firstChild)&&(cell.firstChild.tagName)){
if(cell.firstChild.tagName.toLowerCase()=="table"){
this.getGroupPanelItems(cell.firstChild);
}
}
}
}
};
RadGridNamespace.RadGridGroupPanel.prototype.IsItem=function(_2ac){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Control==_2ac){
return this.Items[i];
}
}
return null;
};
RadGridNamespace.RadGridGroupPanelItem=function(_2ae,_2af,_2b0){
RadControlsNamespace.DomEventMixin.Initialize(this);
this.Control=_2ae;
this.Owner=_2af;
this.HierarchicalIndex=_2b0;
this.Control.style.cursor="move";
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
};
RadGridNamespace.RadGridGroupPanelItem.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
for(var _2b1 in this){
this[_2b1]=null;
}
this.Control=null;
this.Owner=null;
};
RadGridNamespace.RadGridGroupPanelItem.prototype.OnMouseDown=function(e){
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1)&&(e.button==0))||(e.button==1)){
this.CreateDragDrop(e);
this.CreateReorderIndicators(this.Control);
this.AttachDomEvent(document,"mouseup","OnMouseUp");
this.AttachDomEvent(document,"mousemove","OnMouseMove");
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.OnMouseUp=function(e){
this.FireDropAction(e);
this.DestroyDragDrop(e);
this.DestroyReorderIndicators();
this.DetachDomEvent(document,"mouseup","OnMouseUp");
this.DetachDomEvent(document,"mousemove","OnMouseMove");
};
RadGridNamespace.RadGridGroupPanelItem.prototype.OnMouseMove=function(e){
this.MoveDragDrop(e);
};
RadGridNamespace.RadGridGroupPanelItem.prototype.FireDropAction=function(e){
var _2b6=RadGridNamespace.GetCurrentElement(e);
if(_2b6!=null){
if(!RadGridNamespace.IsChildOf(_2b6,this.Owner.Control)){
this.Owner.Owner.SavePostData("UnGroupByExpression",this.HierarchicalIndex);
eval(this.Owner.Owner.ClientSettings.PostBackReferences.PostBackUnGroupByExpression);
}else{
var item=this.Owner.IsItem(_2b6);
if((_2b6!=this.Control)&&(item!=null)&&(_2b6.parentNode==this.Control.parentNode)){
this.Owner.Owner.SavePostData("ReorderGroupByExpression",this.HierarchicalIndex,item.HierarchicalIndex);
eval(this.Owner.Owner.ClientSettings.PostBackReferences.PostBackReorderGroupByExpression);
}
if(window.netscape){
this.Control.style.MozOpacity=4/4;
}else{
this.Control.style.filter="alpha(opacity=100);";
}
}
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.CreateDragDrop=function(e){
this.MoveHeaderDiv=document.createElement("div");
var _2b9=document.createElement("table");
if(this.MoveHeaderDiv.mergeAttributes){
this.MoveHeaderDiv.mergeAttributes(this.Owner.Owner.Control);
}else{
RadGridNamespace.CopyAttributes(this.MoveHeaderDiv,this.Control);
}
if(_2b9.mergeAttributes){
_2b9.mergeAttributes(this.Owner.Control);
}else{
RadGridNamespace.CopyAttributes(_2b9,this.Owner.Control);
}
_2b9.style.margin="0px";
_2b9.style.height=this.Control.offsetHeight+"px";
_2b9.style.width=this.Control.offsetWidth+"px";
_2b9.style.border="0px";
_2b9.style.borderCollapse="collapse";
_2b9.style.padding="0px";
var _2ba=document.createElement("thead");
var tr=document.createElement("tr");
_2b9.appendChild(_2ba);
_2ba.appendChild(tr);
tr.appendChild(this.Control.cloneNode(true));
this.MoveHeaderDiv.appendChild(_2b9);
document.body.appendChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.style.height=_2b9.style.height;
this.MoveHeaderDiv.style.width=_2b9.style.width;
this.MoveHeaderDiv.style.position="absolute";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
if(window.netscape){
this.MoveHeaderDiv.style.MozOpacity=3/4;
}else{
this.MoveHeaderDiv.style.filter="alpha(opacity=75);";
}
this.MoveHeaderDiv.style.cursor="move";
this.MoveHeaderDiv.style.display="none";
this.MoveHeaderDiv.onmousedown=null;
RadGridNamespace.ClearDocumentEvents();
};
RadGridNamespace.RadGridGroupPanelItem.prototype.DestroyDragDrop=function(e){
if(this.MoveHeaderDiv!=null){
var _2bd=this.MoveHeaderDiv.parentNode;
_2bd.removeChild(this.MoveHeaderDiv);
this.MoveHeaderDiv.onmouseup=null;
this.MoveHeaderDiv.onmousemove=null;
this.MoveHeaderDiv=null;
RadGridNamespace.RestoreDocumentEvents();
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.MoveDragDrop=function(e){
if(this.MoveHeaderDiv!=null){
if(window.netscape){
this.Control.style.MozOpacity=1/4;
}else{
this.Control.style.filter="alpha(opacity=25);";
}
this.MoveHeaderDiv.style.visibility="";
this.MoveHeaderDiv.style.display="";
RadGridNamespace.RadGrid.PositionDragElement(this.MoveHeaderDiv,e);
var _2bf=RadGridNamespace.GetCurrentElement(e);
if(_2bf!=null){
if(RadGridNamespace.IsChildOf(_2bf,this.Owner.Control)){
var item=this.Owner.IsItem(_2bf);
if((_2bf!=this.Control)&&(item!=null)&&(_2bf.parentNode==this.Control.parentNode)){
this.MoveReorderIndicators(e,_2bf);
}else{
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
}
}
}
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.CreateReorderIndicators=function(_2c1){
if((this.ReorderIndicator1==null)&&(this.ReorderIndicator2==null)){
this.ReorderIndicator1=document.createElement("span");
this.ReorderIndicator2=document.createElement("span");
if(this.Owner.Owner.Skin==""||this.Owner.Owner.Skin=="None"){
this.ReorderIndicator1.innerHTML="&darr;";
this.ReorderIndicator2.innerHTML="&uarr;";
}else{
this.ReorderIndicator1.className="TopReorderIndicator_"+this.Owner.Owner.Skin;
this.ReorderIndicator2.className="BottomReorderIndicator_"+this.Owner.Owner.Skin;
this.ReorderIndicator1.style.width=this.ReorderIndicator1.style.height=this.ReorderIndicator2.style.width=this.ReorderIndicator2.style.height="10px";
}
this.ReorderIndicator1.style.backgroundColor="transparent";
this.ReorderIndicator1.style.color="darkblue";
this.ReorderIndicator1.style.font="bold 18px Arial";
this.ReorderIndicator2.style.backgroundColor=this.ReorderIndicator1.style.backgroundColor;
this.ReorderIndicator2.style.color=this.ReorderIndicator1.style.color;
this.ReorderIndicator2.style.font=this.ReorderIndicator1.style.font;
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_2c1)-this.ReorderIndicator1.offsetHeight+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_2c1)+"px";
this.ReorderIndicator2.style.top=RadGridNamespace.FindPosY(_2c1)+_2c1.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
this.ReorderIndicator1.style.visibility="hidden";
this.ReorderIndicator1.style.display="none";
this.ReorderIndicator1.style.position="absolute";
this.ReorderIndicator2.style.visibility=this.ReorderIndicator1.style.visibility;
this.ReorderIndicator2.style.display=this.ReorderIndicator1.style.display;
this.ReorderIndicator2.style.position=this.ReorderIndicator1.style.position;
document.body.appendChild(this.ReorderIndicator1);
document.body.appendChild(this.ReorderIndicator2);
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.DestroyReorderIndicators=function(){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
document.body.removeChild(this.ReorderIndicator1);
document.body.removeChild(this.ReorderIndicator2);
this.ReorderIndicator1=null;
this.ReorderIndicator2=null;
}
};
RadGridNamespace.RadGridGroupPanelItem.prototype.MoveReorderIndicators=function(e,_2c3){
if((this.ReorderIndicator1!=null)&&(this.ReorderIndicator2!=null)){
this.ReorderIndicator1.style.visibility="visible";
this.ReorderIndicator1.style.display="";
this.ReorderIndicator2.style.visibility="visible";
this.ReorderIndicator2.style.display="";
this.ReorderIndicator1.style.top=RadGridNamespace.FindPosY(_2c3)-this.ReorderIndicator1.offsetHeight+"px";
this.ReorderIndicator1.style.left=RadGridNamespace.FindPosX(_2c3)+"px";
this.ReorderIndicator2.style.top=RadGridNamespace.FindPosY(_2c3)+_2c3.offsetHeight+"px";
this.ReorderIndicator2.style.left=this.ReorderIndicator1.style.left;
}
};
RadGridNamespace.RadGridMenu=function(_2c4,_2c5,_2c6){
if(!_2c4||!_2c5){
return;
}
RadControlsNamespace.DomEventMixin.Initialize(this);
for(var _2c7 in _2c4){
this[_2c7]=_2c4[_2c7];
}
this.Owner=_2c5;
this.ItemData=_2c4.Items;
this.Items=[];
};
RadGridNamespace.RadGridMenu.prototype.Initialize=function(){
if(this.Control!=null){
return;
}
this.Control=document.createElement("table");
this.Control.style.backgroundColor=this.SelectColumnBackColor;
this.Control.style.border="outset 1px";
this.Control.style.fontSize="small";
this.Control.style.textAlign="left";
this.Control.cellPadding="0";
this.Control.style.borderCollapse="collapse";
this.Control.style.zIndex=998;
this.Skin=(this.Owner&&this.Owner.Owner&&this.Owner.Owner.Skin)||"None";
var _2c8=RadGridNamespace.IsRightToLeft(this.Owner.Control);
if(_2c8){
this.Control.style.direction="rtl";
RadGridNamespace.addClassName(this.Control,"RadGridRTL_"+this.Skin);
}
RadGridNamespace.addClassName(this.Control,"GridFilterMenu_"+this.Skin);
RadGridNamespace.addClassName(this.Control,this.CssClass);
this.Items=this.CreateItems(this.ItemData);
this.Control.style.position="absolute";
this.Control.style.display="none";
document.body.appendChild(this.Control);
var _2c9=document.createElement("img");
_2c9.src=this.SelectedImageUrl;
_2c9.src=this.NotSelectedImageUrl;
this.Control.style.zIndex=100000;
};
RadGridNamespace.RadGridMenu.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
this.DisposeItems();
this.ItemData=null;
this.Owner=null;
this.Control=null;
};
RadGridNamespace.RadGridMenu.prototype.CreateItems=function(_2ca){
var _2cb=[];
for(var i=0;i<_2ca.length;i++){
_2cb[_2cb.length]=new RadGridNamespace.RadGridMenuItem(_2ca[i],this);
}
return _2cb;
};
RadGridNamespace.RadGridMenu.prototype.DisposeItems=function(){
for(var i=0;i<this.Items.length;i++){
var item=this.Items[i];
item.Dispose();
}
this.Items=null;
};
RadGridNamespace.RadGridMenu.prototype.HideItem=function(_2cf){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Value==_2cf){
this.Items[i].Control.style.display="none";
}
}
};
RadGridNamespace.RadGridMenu.prototype.ShowItem=function(_2d1){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Value==_2d1){
this.Items[i].Control.style.display="";
}
}
};
RadGridNamespace.RadGridMenu.prototype.SelectItem=function(_2d3){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Value==_2d3){
this.Items[i].Selected=true;
this.Items[i].SelectImage.src=this.SelectedImageUrl;
}else{
this.Items[i].Selected=false;
this.Items[i].SelectImage.src=this.NotSelectedImageUrl;
}
}
};
RadGridNamespace.RadGridMenu.prototype.Show=function(_2d5,_2d6,e){
this.Initialize();
this.Control.style.display="";
this.Control.style.top=e.clientY+document.documentElement.scrollTop+document.body.scrollTop+5+"px";
this.Control.style.left=e.clientX+document.documentElement.scrollLeft+document.body.scrollLeft+5+"px";
this.AttachHideEvents();
};
RadGridNamespace.RadGridMenu.prototype.OnKeyPress=function(e){
if(e.keyCode==27){
this.DetachHideEvents();
this.Hide();
}
};
RadGridNamespace.RadGridMenu.prototype.OnClick=function(e){
if(!e.cancelBubble){
this.DetachHideEvents();
this.Hide();
}
};
RadGridNamespace.RadGridMenu.prototype.AttachHideEvents=function(){
this.AttachDomEvent(document,"keypress","OnKeyPress");
this.AttachDomEvent(document,"click","OnClick");
};
RadGridNamespace.RadGridMenu.prototype.DetachHideEvents=function(){
this.DetachDomEvent(document,"keypress","OnKeyPress");
this.DetachDomEvent(document,"click","OnClick");
};
RadGridNamespace.RadGridMenu.prototype.Hide=function(){
if(this.Control.style.display==""){
this.Control.style.display="none";
}
};
RadGridNamespace.RadGridMenuItem=function(_2da,_2db){
for(var _2dc in _2da){
this[_2dc]=_2da[_2dc];
}
this.Owner=_2db;
this.Skin=this.Owner.Skin;
this.Control=this.Owner.Control.insertRow(-1);
this.Control.insertCell(-1);
var _2dd=document.createElement("table");
_2dd.style.width="100%";
_2dd.cellPadding="0";
_2dd.cellSpacing="0";
_2dd.insertRow(-1);
var td1=_2dd.rows[0].insertCell(-1);
var td2=_2dd.rows[0].insertCell(-1);
if(this.Skin=="None"){
td1.style.borderTop="solid 1px "+this.Owner.SelectColumnBackColor;
td1.style.borderLeft="solid 1px "+this.Owner.SelectColumnBackColor;
td1.style.borderRight="none 0px";
td1.style.borderBottom="solid 1px "+this.Owner.SelectColumnBackColor;
td1.style.padding="2px";
td1.style.textAlign="center";
}else{
RadGridNamespace.addClassName(td1,"GridFilterMenuSelectColumn_"+this.Skin);
}
td1.style.width="16px";
td1.appendChild(document.createElement("img"));
td1.childNodes[0].src=this.Owner.NotSelectedImageUrl;
this.SelectImage=td1.childNodes[0];
if(this.Skin=="None"){
td2.style.borderTop="solid 1px "+this.Owner.TextColumnBackColor;
td2.style.borderLeft="none 0px";
td2.style.borderRight="solid 1px "+this.Owner.TextColumnBackColor;
td2.style.borderBottom="solid 1px "+this.Owner.TextColumnBackColor;
td2.style.padding="2px";
td2.style.backgroundColor=this.Owner.TextColumnBackColor;
td2.style.cursor="pointer";
}else{
RadGridNamespace.addClassName(td2,"GridFilterMenuTextColumn_"+this.Skin);
}
td2.innerHTML=this.Text;
this.Control.cells[0].appendChild(_2dd);
var _2e0=this;
this.Control.onclick=function(){
if(_2e0.Owner.Owner.Owner.EnableAJAX){
if(_2e0.Owner.Owner==_2e0.Owner.Owner.Owner.MasterTableViewHeader){
RadGridNamespace.AsyncRequest(_2e0.UID,_2e0.Owner.Owner.Owner.MasterTableView.UID+"!"+_2e0.Owner.Column.UniqueName,_2e0.Owner.Owner.Owner.ClientID);
}else{
RadGridNamespace.AsyncRequest(_2e0.UID,_2e0.Owner.Owner.UID+"!"+_2e0.Owner.Column.UniqueName,_2e0.Owner.Owner.Owner.ClientID);
}
}else{
var _2e1=_2e0.Owner.Owner.Owner.ClientSettings.PostBackFunction;
if(_2e0.Owner.Owner==_2e0.Owner.Owner.Owner.MasterTableViewHeader){
_2e1=_2e1.replace("{0}",_2e0.UID).replace("{1}",_2e0.Owner.Owner.Owner.MasterTableView.UID+"!"+_2e0.Owner.Column.UniqueName);
}else{
_2e1=_2e1.replace("{0}",_2e0.UID).replace("{1}",_2e0.Owner.Owner.UID+"!"+_2e0.Owner.Column.UniqueName);
}
eval(_2e1);
}
};
var _2e0=this;
this.Control.onmouseover=function(e){
if(_2e0.Skin=="None"){
this.cells[0].childNodes[0].rows[0].cells[0].style.backgroundColor=_2e0.Owner.HoverBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderTop="solid 1px "+_2e0.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderLeft="solid 1px "+_2e0.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderBottom="solid 1px "+_2e0.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.backgroundColor=_2e0.Owner.HoverBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderTop="solid 1px "+_2e0.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderRight="solid 1px "+_2e0.Owner.HoverBorderColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderBottom="solid 1px "+_2e0.Owner.HoverBorderColor;
}else{
RadGridNamespace.addClassName(this.cells[0].childNodes[0].rows[0].cells[0],"GridFilterMenuHover_"+_2e0.Skin);
RadGridNamespace.addClassName(this.cells[0].childNodes[0].rows[0].cells[1],"GridFilterMenuHover_"+_2e0.Skin);
}
};
this.Control.onmouseout=function(e){
if(_2e0.Skin=="None"){
this.cells[0].childNodes[0].rows[0].cells[0].style.borderTop="solid 1px "+_2e0.Owner.SelectColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderLeft="solid 1px "+_2e0.Owner.SelectColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.borderBottom="solid 1px "+_2e0.Owner.SelectColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[0].style.backgroundColor="";
this.cells[0].childNodes[0].rows[0].cells[1].style.borderTop="solid 1px "+_2e0.Owner.TextColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderRight="solid 1px "+_2e0.Owner.TextColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.borderBottom="solid 1px "+_2e0.Owner.TextColumnBackColor;
this.cells[0].childNodes[0].rows[0].cells[1].style.backgroundColor=_2e0.Owner.TextColumnBackColor;
}else{
RadGridNamespace.removeClassName(this.cells[0].childNodes[0].rows[0].cells[0],"GridFilterMenuHover_"+_2e0.Skin);
RadGridNamespace.removeClassName(this.cells[0].childNodes[0].rows[0].cells[1],"GridFilterMenuHover_"+_2e0.Skin);
}
};
};
RadGridNamespace.RadGridMenuItem.prototype.Dispose=function(){
this.Control.onclick=null;
this.Control.onmouseover=null;
this.Control.onmouseout=null;
var _2e4=this.Control.getElementsByTagName("table");
while(_2e4.length>0){
var _2e5=_2e4[0];
if(_2e5.parentNode!=null){
_2e5.parentNode.removeChild(_2e5);
}
}
this.Control=null;
this.Owner=null;
};
RadGridNamespace.RadGridFilterMenu=function(_2e6,_2e7){
RadGridNamespace.RadGridMenu.call(this,_2e6,_2e7);
};
RadGridNamespace.RadGridFilterMenu.prototype=new RadGridNamespace.RadGridMenu;
RadGridNamespace.RadGridFilterMenu.prototype.Show=function(_2e8,e){
this.Initialize();
if(!_2e8){
return;
}
this.Owner=_2e8.Owner;
this.Column=_2e8;
for(var i=0;i<this.Items.length;i++){
if(_2e8.DataTypeName=="System.Boolean"){
if((this.Items[i].Value=="GreaterThan")||(this.Items[i].Value=="LessThan")||(this.Items[i].Value=="GreaterThanOrEqualTo")||(this.Items[i].Value=="LessThanOrEqualTo")||(this.Items[i].Value=="Between")||(this.Items[i].Value=="NotBetween")){
this.Items[i].Control.style.display="none";
continue;
}
}
if(_2e8.DataTypeName!="System.String"){
if((this.Items[i].Value=="StartsWith")||(this.Items[i].Value=="EndsWith")||(this.Items[i].Value=="Contains")||(this.Items[i].Value=="DoesNotContain")||(this.Items[i].Value=="IsEmpty")||(this.Items[i].Value=="NotIsEmpty")){
this.Items[i].Control.style.display="none";
continue;
}
}
if(_2e8.FilterListOptions=="VaryByDataType"){
if(this.Items[i].Value=="Custom"){
this.Items[i].Control.style.display="none";
continue;
}
}
this.Items[i].Control.style.display="";
}
this.SelectItem(_2e8.CurrentFilterFunction);
var args={Menu:this,TableView:this.Owner,Column:this.Column,Event:e};
if(!RadGridNamespace.FireEvent(this.Owner,"OnFilterMenuShowing",[this.Owner,args])){
return;
}
this.Control.style.display="";
this.Control.style.top=e.clientY+document.documentElement.scrollTop+document.body.scrollTop+5+"px";
this.Control.style.left=e.clientX+document.documentElement.scrollLeft+document.body.scrollLeft+5+"px";
this.AttachHideEvents();
};
RadGridNamespace.RadGrid.prototype.InitializeFilterMenu=function(_2ec){
if(this.AllowFilteringByColumn||_2ec.AllowFilteringByColumn){
if(!_2ec||!_2ec.Control){
return;
}
if(!_2ec.Control.tHead){
return;
}
if(!_2ec.IsItemInserted){
var _2ed=_2ec.Control.tHead.rows[_2ec.Control.tHead.rows.length-1];
}else{
var _2ed=_2ec.Control.tHead.rows[_2ec.Control.tHead.rows.length-2];
}
if(!_2ed){
return;
}
var _2ee=_2ed.getElementsByTagName("img");
var _2ef=this;
if(!_2ec.Columns){
return;
}
if(!_2ec.Columns[0]){
return;
}
var _2f0=_2ec.Columns[0].FilterImageUrl;
for(var i=0;i<_2ee.length;i++){
var _2f2=RadGridNamespace.EncodeURI(_2f0);
if(_2ee[i].getAttribute("src").indexOf(_2f2)==-1){
continue;
}
_2ee[i].onclick=function(e){
if(!e){
var e=window.event;
}
e.cancelBubble=true;
var _2f4=this.parentNode.cellIndex;
if(window.attachEvent&&!window.opera&&!window.netscape){
_2f4=RadGridNamespace.GetRealCellIndexFormCells(this.parentNode.parentNode.cells,this.parentNode);
}
_2ef.FilteringMenu.Show(_2ec.Columns[_2f4],e);
if(e.preventDefault){
e.preventDefault();
}else{
e.returnValue=false;
return false;
}
};
}
this.FilteringMenu=new RadGridNamespace.RadGridFilterMenu(this.FilterMenu,_2ec);
}
};
RadGridNamespace.RadGrid.prototype.DisposeFilterMenu=function(_2f5){
if(this.FilteringMenu!=null){
this.FilteringMenu.Dispose();
this.FilteringMenu=null;
}
};
RadGridNamespace.GetRealCellIndexFormCells=function(_2f6,cell){
for(var i=0;i<_2f6.length;i++){
if(_2f6[i]==cell){
return i;
}
}
};
if(typeof (window.RadGridNamespace)=="undefined"){
window.RadGridNamespace=new Object();
}
RadGridNamespace.Slider=function(_2f9){
RadControlsNamespace.DomEventMixin.Initialize(this);
if(!document.readyState||document.readyState=="complete"||window.opera){
this._constructor(_2f9);
}else{
this.objectData=_2f9;
this.AttachDomEvent(window,"load","OnWindowLoad");
}
};
RadGridNamespace.Slider.prototype.OnWindowLoad=function(e){
this.DetachDomEvent(window,"load","OnWindowLoad");
this._constructor(this.objectData);
this.objectData=null;
};
RadGridNamespace.Slider.prototype._constructor=function(_2fb){
var _2fc=this;
for(var _2fd in _2fb){
this[_2fd]=_2fb[_2fd];
}
this.Owner=window[this.OwnerID];
this.OwnerGrid=window[this.OwnerGridID];
this.Control=document.getElementById(this.ClientID);
if(this.Control==null){
return;
}
this.Control.unselectable="on";
this.Control.parentNode.style.padding="10px";
this.ToolTip=document.createElement("div");
this.ToolTip.unselectable="on";
this.ToolTip.style.backgroundColor="#F5F5DC";
this.ToolTip.style.border="1px outset";
this.ToolTip.style.font="icon";
this.ToolTip.style.padding="2px";
this.ToolTip.style.marginTop="5px";
this.ToolTip.style.marginBottom="15px";
this.Control.appendChild(this.ToolTip);
this.Line=document.createElement("hr");
this.Line.unselectable="on";
this.Line.style.width="100%";
this.Line.style.height="2px";
this.Line.style.backgroundColor="buttonface";
this.Line.style.border="1px outset threedshadow";
this.Control.appendChild(this.Line);
this.Thumb=document.createElement("div");
this.Thumb.unselectable="on";
this.Thumb.style.position="relative";
this.Thumb.style.width="8px";
this.Thumb.style.marginTop="-15px";
this.Thumb.style.height="16px";
this.Thumb.style.backgroundColor="buttonface";
this.Thumb.style.border="1px outset threedshadow";
this.Control.appendChild(this.Thumb);
this.Link=document.createElement("a");
this.Link.unselectable="on";
this.Link.style.width="100%";
this.Link.style.height="100%";
this.Link.style.display="block";
this.Link.href="javascript:void(0);";
this.Thumb.appendChild(this.Link);
this.LineX=RadGridNamespace.FindPosX(this.Line);
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
this.AttachDomEvent(this.Link,"keydown","OnKeyDown");
var _2fe=this.OwnerGrid.CurrentPageIndex/this.OwnerGrid.MasterTableView.PageCount;
this.SetPosition(_2fe*this.Line.offsetWidth);
var _2ff=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _300=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_2ff);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,this.OwnerGrid.CurrentPageIndex,this.OwnerGrid.MasterTableView.PageCount);
};
RadGridNamespace.Slider.prototype.Dispose=function(){
this.DisposeDomEventHandlers();
for(var _301 in this){
this[_301]=null;
}
this.Control=null;
this.Line=null;
this.Thumb=null;
this.ToolTip=null;
};
RadGridNamespace.Slider.prototype.OnKeyDown=function(e){
this.AttachDomEvent(this.Link,"keyup","OnKeyUp");
if(e.keyCode==39){
this.SetPosition(parseInt(this.Thumb.style.left)+this.Thumb.offsetWidth);
}
if(e.keyCode==37){
this.SetPosition(parseInt(this.Thumb.style.left)-this.Thumb.offsetWidth);
}
if(e.keyCode==39||e.keyCode==37){
var _303=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _304=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_303);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,_304,this.OwnerGrid.MasterTableView.PageCount);
}
};
RadGridNamespace.Slider.prototype.OnKeyUp=function(e){
this.DetachDomEvent(this.Link,"keyup","OnKeyUp");
if(e.keyCode==39||e.keyCode==37){
var _306=this;
setTimeout(function(){
_306.ChangePage();
},100);
}
};
RadGridNamespace.Slider.prototype.OnMouseDown=function(e){
this.DetachDomEvent(this.Control,"mousedown","OnMouseDown");
if(((window.netscape||window.opera||navigator.userAgent.indexOf("Safari")!=-1))&&(e.button==0)||(e.button==1)){
this.SetPosition(RadGridNamespace.GetEventPosX(e)-this.LineX);
this.AttachDomEvent(document,"mousemove","OnMouseMove");
this.AttachDomEvent(document,"mouseup","OnMouseUp");
}
};
RadGridNamespace.Slider.prototype.OnMouseUp=function(e){
this.DetachDomEvent(document,"mousemove","OnMouseMove");
this.DetachDomEvent(document,"mouseup","OnMouseUp");
var _309=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _30a=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_309);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,_30a,this.OwnerGrid.MasterTableView.PageCount);
var _30b=this;
setTimeout(function(){
_30b.ChangePage();
},100);
};
RadGridNamespace.Slider.prototype.OnMouseMove=function(e){
this.SetPosition(RadGridNamespace.GetEventPosX(e)-this.LineX);
var _30d=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _30e=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_30d);
this.OwnerGrid.ApplyPagerTooltipText(this.ToolTip,_30e,this.OwnerGrid.MasterTableView.PageCount);
};
RadGridNamespace.Slider.prototype.GetPosition=function(e){
this.SetPosition(RadGridNamespace.GetEventPosX(e)-this.LineX);
};
RadGridNamespace.Slider.prototype.SetPosition=function(_310){
if(_310>=0&&_310<=this.Line.offsetWidth){
this.Thumb.style.left=_310+"px";
}
};
RadGridNamespace.Slider.prototype.ChangePage=function(){
var _311=parseInt(this.Thumb.style.left)/this.Line.offsetWidth;
var _312=Math.round((this.OwnerGrid.MasterTableView.PageCount-1)*_311);
if(this.OwnerGrid.CurrentPageIndex==_312){
this.AttachDomEvent(this.Control,"mousedown","OnMouseDown");
return;
}
this.OwnerGrid.SavePostData("AJAXScrolledControl",(this.OwnerGrid.GridDataDiv)?this.OwnerGrid.GridDataDiv.scrollLeft:"",(this.OwnerGrid.GridDataDiv)?this.OwnerGrid.LastScrollTop:"",(this.OwnerGrid.GridDataDiv)?this.OwnerGrid.GridDataDiv.scrollTop:"",_312);
var _313=this.OwnerGrid.ClientSettings.PostBackFunction;
_313=_313.replace("{0}",this.OwnerGrid.UniqueID);
eval(_313);
};

//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
