/*
// jQuery smallColorPicker
//
// Version 1.0.0 beta
//
// Created Yury Gugnin 20.04.2010
//	
// Usage:
// $('.color_input').smallColorPicker({options});
//
// Options:
//  defaultColor - default color value
//  colorRows - num rows in color popup
//  colorValues - List colors, default Array("#000202","#953503","#35381D","#003906","#03316D","#020274","#282AA1","#373737","#7C0200","#F76905","#848000","#037B0D","#008589","#0001FE","#63649D","#7E7E7E","#FE0000","#F7981A","#93CD00","#2D9C69","#21D4CE","#3860FF","#700788","#909090","#F60EE0","#FFC500","#FFFC01","#00FF00","#0CFFFD","#03CBFF","#AB245F","#B9B9B9","#FF8CCE","#FFCB90","#FFFF94","#BFFFC5","#C4FFFF","#92CDFF","#D996FF","#FFFFFF")  
//  buttonBackClassName - button class name, default ""
//  buttonBackColorClassName - button color shower class name, default ""
//  buttonOnPopupClassName - button class name on popup, default ""
//  popupClassName - popup class name, default ""
//  popupColorClassName - popup color element class name, default ""
//  popupHeader - popup header class name, default ""
*/
var show_panel = false;
if(jQuery) (function($){  
	$.extend($.fn, {
	  smallColorPicker:	  function(opt) {	    	    	    	    	    
	    $(this).each( function() {	      
	      var o = opt;	      
	      //DEFAULTS	      
	    if( !o ) var o = {};	    
	    if( o.colorValues == undefined ) o.colorValues = Array("#000202","#953503","#35381D","#003906","#03316D","#020274","#282AA1","#373737","#7C0200","#F76905","#848000","#037B0D","#008589","#0001FE","#63649D","#7E7E7E","#FE0000","#F7981A","#93CD00","#2D9C69","#21D4CE","#3860FF","#700788","#909090","#F60EE0","#FFC500","#FFFC01","#00FF00","#0CFFFD","#03CBFF","#AB245F","#B9B9B9","#FF8CCE","#FFCB90","#FFFF94","#BFFFC5","#C4FFFF","#92CDFF","#D996FF","#FFFFFF");	    
	    if( o.buttonBackClassName == undefined )o.buttonBackClassName = "smallColorPicker_button_back";
	    if( o.buttonBackColorClassName == undefined )o.buttonBackColorClassName = "smallColorPicker_button_color";
	    if( o.buttonOnPopupClassName == undefined )o.buttonOnPopupClassName = "smallColorPicker_button_back_popup";
	    if( o.popupClassName == undefined )o.popupClassName = "color_parent";
	    if( o.clearClassName == undefined )o.clearClassName = "clear";
	    if( o.popupColorClassName == undefined )o.popupColorClassName = "picker_panel";
	    if( o.popupHeader == undefined )o.popupHeader = "picker_panel_header";
	    
	    if( o.defaultColor == undefined )o.defaultColor = "#000202";
	    if( o.colorRows == undefined )o.colorRows = 8;
	    o.input = this;
      $(this).hide();
      colors_html = "";
	    p = 0;
	    cval = '';
	    for(i in o.colorValues){
	      p++;
	      colors_html += '<div class="'+o.popupClassName+'"><a href="" color='+o.colorValues[i]+'><div style="background-color:'+o.colorValues[i]+'"></div></a></div>';	      
	      if(o.colorValues[i] == $(this).val()){
	        cval = o.colorValues[i]; 
	      }  	      
	      if(p==o.colorRows){
	        p=0;
	        colors_html += '<div class="'+o.clearClassName+'"></div>';
	      }
	    }
	    
	    if(cval.length < 1){
	      cval = o.defaultColor;
	    }
	    $(this).val(cval);
	    $(this).after('<div id="picker_button_'+$(this).attr('id')+'" class="'+o.buttonBackClassName +'"><div id="picker_shower_'+$(this).attr('id')+'" class="'+o.buttonBackColorClassName +'" style="background-color:'+cval+'"></div></div><div class="'+o.popupColorClassName+'" id="picker_panel_'+$(this).attr('id')+'"><div class="'+o.popupHeader +'"></div>'+colors_html+'</div>');
	    o.button = $("#picker_button_"+$(this).attr('id'));
	    $(o.button).click(function(){
	      showPanel();	      
	    });
	    o.panel = $("#picker_panel_"+$(this).attr('id'));	    
  
     $(o.panel).mouseout(function(){   
       show_panel = false;
       setTimeout('if(!show_panel){hidePanelIs("'+$(o.panel).attr('id')+'","'+$(o.button).attr('id')+'","'+o.buttonOnPopupClassName+'");}',200);
     });     
     $(o.button).mouseout(function(){   
       show_panel = false;
       setTimeout('if(!show_panel){hidePanelIs("'+$(o.panel).attr('id')+'","'+$(o.button).attr('id')+'","'+o.buttonOnPopupClassName+'");}',200);
     });
     $(o.panel).mouseover(function(){       
       show_panel = true;       
     });
     $(o.button).mouseover(function(){
       show_panel = true;   
     });
  	    
	    $("#picker_panel_"+$(this).attr('id')+" a").each(function(){
	      $(this).data('color_value', $(this).attr('color'));
	      $(this).removeAttr('color');
	      $(this).click(function(){	  	        
	        $("#picker_shower_"+$(o.input).attr('id')).css('background-color',$(this).data('color_value'));	        
	        $(o.input).val($(this).data('color_value'));
	        hidePanel();
	        return false;	        
	      });
	    });  	    
	    function hidePanel(){
	     $(o.panel).hide();
	     show_panel = false;
	     $(o.button).removeClass(o.buttonOnPopupClassName);	     	     
	    }	    
	    function showPanel(){
	     $(o.button).addClass(o.buttonOnPopupClassName);
	     show_panel = true;
	     $(o.panel).show();	     
	    }
	    
	    
	   });	    	   
	  }
	  
	});	
}

)(jQuery);
function hidePanelIs(d1,d2,cl){
  $('#'+d1).hide();
  show_panel = false;
  $('#'+d2).removeClass(cl);	
}