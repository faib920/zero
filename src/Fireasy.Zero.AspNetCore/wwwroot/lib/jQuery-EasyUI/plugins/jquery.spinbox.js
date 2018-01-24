/**
 * jQuery EasyUI 1.2
 * 
 * Licensed under the GPL:
 *   http://www.gnu.org/licenses/gpl.txt
 *
 * Copyright 2010 stworthy [ stworthy@gmail.com ] 
 * 
 */
(function($){
function _1(_2){
var _3=$("<span class=\"spinbox\">"+"<span class=\"spinbox-arrow\">"+"<span class=\"spinbox-arrow-up\"></span>"+"<span class=\"spinbox-arrow-down\"></span>"+"</span>"+"</span>").insertAfter(_2);
$(_2).addClass("spinbox-text").prependTo(_3);
return _3;
};
function _4(_5,_6){
var _7=$.data(_5,"spinbox").options;
var _8=$.data(_5,"spinbox").box;
if(_6){
_7.width=_6;
}
if(isNaN(_7.width)){
_7.width=$(_5).outerWidth();
}
var _9=_8.find(".spinbox-arrow").outerWidth();
var _6=_7.width-_9;
if($.boxModel==true){
_6-=_8.outerWidth()-_8.width();
}
$(_5).width(_6);
};
function _a(_b){
var _c=$.data(_b,"spinbox").options;
var _d=$.data(_b,"spinbox").box;
_d.find(".spinbox-arrow-up,.spinbox-arrow-down").unbind(".spinbox");
if(!_c.disabled){
_d.find(".spinbox-arrow-up").bind("mouseenter.spinbox",function(){
$(this).addClass("spinbox-arrow-hover");
}).bind("mouseleave.spinbox",function(){
$(this).removeClass("spinbox-arrow-hover");
}).bind("click.spinbox",function(){
_c.spin.call(_b,false);
_c.onSpinUp.call(_b);
});
_d.find(".spinbox-arrow-down").bind("mouseenter.spinbox",function(){
$(this).addClass("spinbox-arrow-hover");
}).bind("mouseleave.spinbox",function(){
$(this).removeClass("spinbox-arrow-hover");
}).bind("click.spinbox",function(){
_c.spin.call(_b,true);
_c.onSpinDown.call(_b);
});
}
};
function _e(_f,_10){
var _11=$.data(_f,"spinbox").options;
var v=parseFloat($(_f).val()||_11.value)||0;
if(_10==true){
v-=_11.increment;
}else{
v+=_11.increment;
}
if(_11.min!=null&&_11.min!=undefined&&v<_11.min){
v=_11.min.toFixed(_11.precision);
}else{
if(_11.max!=null&&_11.max!=undefined&&v>_11.max){
v=_11.max.toFixed(_11.precision);
}else{
v=v.toFixed(_11.precision);
}
}
_11.value=v;
$(_f).val(v).validatebox("validate");
};
function _12(_13,_14){
var _15=$.data(_13,"spinbox").options;
if(_14){
_15.disabled=true;
$(_13).attr("disabled",true);
}else{
_15.disabled=false;
$(_13).removeAttr("disabled");
}
};
$.fn.spinbox=function(_16,_17){
if(typeof _16=="string"){
var _18=$.fn.spinbox.methods[_16];
if(_18){
return _18(this,_17);
}else{
return this.validatebox(_16,_17);
}
}
_16=_16||{};
return this.each(function(){
var _19=$.data(this,"spinbox");
if(_19){
$.extend(_19.options,_16);
}else{
_19=$.data(this,"spinbox",{options:$.extend({},$.fn.spinbox.defaults,$.fn.spinbox.parseOptions(this),_16),box:_1(this)});
$(this).removeAttr("disabled");
}
$(this).val(_19.options.value);
$(this).attr("readonly",!_19.options.editable);
_12(this,_19.options.disabled);
_4(this);
$(this).validatebox(_19.options);
_a(this);
});
};
$.fn.spinbox.methods={options:function(jq){
return $.data(jq[0],"spinbox").options;
},destroy:function(jq){
return jq.each(function(){
var box=$.data(this,"spinbox").box;
$(this).validatebox("destroy");
box.remove();
});
},resize:function(jq,_1a){
return jq.each(function(){
_4(this,_1a);
});
},enable:function(jq){
return jq.each(function(){
_12(this,false);
_a(this);
});
},disable:function(jq){
return jq.each(function(){
_12(this,true);
_a(this);
});
},getValue:function(jq){
return jq.val();
},setValue:function(jq,_1b){
return jq.each(function(){
var _1c=$.data(this,"spinbox").options;
_1c.value=_1b;
$(this).val(_1b);
});
},clear:function(jq){
return jq.each(function(){
var _1d=$.data(this,"spinbox").options;
_1d.value="";
$(this).val("");
});
}};
$.fn.spinbox.parseOptions=function(_1e){
var t=$(_1e);
return $.extend({},$.fn.validatebox.parseOptions(_1e),{width:(parseInt(_1e.style.width)||undefined),value:(t.val()||undefined),min:(t.attr("min")=="0"?0:parseFloat(t.attr("min"))||undefined),max:(t.attr("max")=="0"?0:parseFloat(t.attr("max"))||undefined),increment:(parseInt(t.attr("increment"))||undefined),editable:(t.attr("editable")?t.attr("editable")=="true":undefined),disabled:(t.attr("disabled")?true:undefined)});
};
$.fn.spinbox.defaults=$.extend({},$.fn.validatebox.defaults,{width:"auto",value:"",min:null,max:null,precision:0,increment:1,editable:true,disabled:false,spin:function(_1f){
_e(this,_1f);
},onSpinUp:function(_20){
},onSpinDown:function(_21){
}});
})(jQuery);

