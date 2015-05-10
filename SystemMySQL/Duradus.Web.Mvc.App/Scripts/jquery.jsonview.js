/* $.jsonView() 0.1 - jQuery-based Json to html pretty printer
 *
 * Copyright (c) 2010 Francois Lafortune  (quickredfox.at)
 * Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php 
 *

	
	/** folder, un-folder array */
	$('.list-toggle-button').live('click', function(e){
		e.preventDefault();e.stopPropagation();
		var $this = $(this);
		if(!$this.data('li')){
			$this.data('li', $this.parents('li').first());
			$this.data('div', $this.data('li').children('div'));
			var type = 	($this.data('li').find('> .array-wrapper').length > 0 ? 'array' : $this.data('li').find('> .object-wrapper').length > 0 ? 'object' : 'string');				
			$this.data('type', type);
		}
		
		if(!$this.data('div').length){$this.text(''); return;} /** array of number, string does not need folder or unfolder */
		//toggle
		if($this.data('div').is(':visible')){
			$this.text('[+]');	
			$this.data('div').hide(0,function(){
				$this.data('li').addClass('closed-'+$this.data('type'));
			});	
		}else{
			$this.text('[-]');
			$this.data('div').show(0,function(){
				$this.data('li').removeClass('closed-'+$this.data('type'));
			});
		} 
	});
	
	// only register this once... will work everywhere
	$('.property-toggle-button').live('click',function(e){
		e.preventDefault();e.stopPropagation();
		var $this = $(this);
		// first time acces this element, store relationships 			
		if(!$this.data('dt')){
			$this.data('dt',$this.parents('dt'));
			$this.data('dd',$this.data('dt').next('dd'));
			var type = 	($this.data('dd').find('> .array-wrapper').length > 0 ? 'array' : $this.data('dd').find('> .object-wrapper').length > 0 ? 'object' : 'string');				
			$this.data('type', type);
		}
		// toggle
		if($this.data('dd').is(':visible')){
			$this.text('[+]');	
			$this.data('dd').hide(0,function(){
				$this.data('dt').addClass('closed-'+$this.data('type'))									
			});	
		}else{
			$this.text('[-]');
			$this.data('dd').show(0,function(){
				$this.data('dt').removeClass('closed-'+$this.data('type'))										
			});
		} 
	});
})();