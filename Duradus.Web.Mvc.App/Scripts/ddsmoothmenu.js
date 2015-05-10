//** Smooth Navigational Menu- By Dynamic Drive DHTML code library: http://www.dynamicdrive.com
//** Script Download/ instructions page: http://www.dynamicdrive.com/dynamicindex1/ddlevelsmenu/
//** Menu created: Nov 12, 2008

//** Dec 12th, 08" (v1.01): Fixed Shadow issue when multiple LIs within the same UL (level) contain sub menus: http://www.dynamicdrive.com/forums/showthread.php?t=39177&highlight=smooth

//** Feb 11th, 09" (v1.02): The currently active main menu item (LI A) now gets a CSS class of ".selected", including sub menu items.

//** May 1st, 09" (v1.3):
//** 1) Now supports vertical (side bar) menu mode- set "orientation" to 'v'
//** 2) In IE6, shadows are now always disabled

//** July 27th, 09" (v1.31): Fixed bug so shadows can be disabled if desired.
//** Feb 2nd, 10" (v1.4): Adds ability to specify delay before sub menus appear and disappear, respectively. See showhidedelay variable below

var ddsmoothmenu = {
    isRTL: false,
    //Specify full URL to down and right arrow images (23 is padding-right added to top level LIs with drop downs):
    arrowimages: { down: ['', gVD + 'Content/Images/down.gif', 22], right: ['', gVD + 'Content/Images/right.gif'] },
    transition: { overtime: 200, outtime: 200 }, //duration of slide in/ out animation, in milliseconds
    shadow: { enable: true, offsetx: 5, offsety: 5 }, //enable shadow?
    showhidedelay: { showdelay: 50, hidedelay: 100 }, //set delay in milliseconds before sub menus appear and disappear, respectively

    ///////Stop configuring beyond here///////////////////////////

    detectwebkit: navigator.userAgent.toLowerCase().indexOf("applewebkit") != -1, //detect WebKit browsers (Safari, Chrome etc)
    detectie6: document.all && !window.XMLHttpRequest,

    getajaxmenu: function($, setting) { //function to fetch external page containing the panel DIVs
        var $menucontainer = $('#' + setting.contentsource[0]) //reference empty div on page that will hold menu
        $menucontainer.html("Loading Menu...")
        $.ajax({
            url: setting.contentsource[1], //path to external menu file
            async: true,
            error: function(ajaxrequest) {
                $menucontainer.html('Error fetching content. Server Response: ' + ajaxrequest.responseText)
            },
            success: function(content) {
                $menucontainer.html(content)
                ddsmoothmenu.buildmenu($, setting)
            }
        })
    },


    buildmenu: function($, setting) {
        var smoothmenu = ddsmoothmenu
        var $mainmenu = $("#" + setting.mainmenuid + ">ul") //reference main menu UL
        $mainmenu.parent().get(0).className = setting.classname || "ddsmoothmenu"
        var $headers = $mainmenu.find("ul").parent()
        $headers.hover(
		function(e) {
		    $(this).children('a:eq(0)').addClass('selected')
		},
		function(e) {
		    $(this).children('a:eq(0)').removeClass('selected')
		}
	)
        $headers.each(function(i) { //loop through each LI header
            var $curobj = $(this).css({ zIndex: 10000 - i }) //reference current LI header
            var $subul = $(this).find('ul:eq(0)').css({ display: 'block' })
            $subul.data('timers', {})
            this._dimensions = { w: this.offsetWidth, h: this.offsetHeight, subulw: $subul.outerWidth(), subulh: $subul.outerHeight() }
            this.istopheader = $curobj.parents("ul").length == 1 ? true : false //is top level header?
            $subul.css({ top: this.istopheader && setting.orientation != 'v' ? this._dimensions.h + "px" : 0 })
            var $mchilds = $curobj.children("a:eq(0)");
            if (this.istopheader) {
                if (!ddsmoothmenu.isRTL) {
                    $mchilds.css({ paddingRight: smoothmenu.arrowimages.down[2] });
                } else {
                    $mchilds.css({ paddingLeft: smoothmenu.arrowimages.down[2] });
                }
            }

            $mchilds.append( //add arrow images
			'<img src="' + (this.istopheader && setting.orientation != 'v' ? smoothmenu.arrowimages.down[1] : smoothmenu.arrowimages.right[1])
			+ '" class="' + (this.istopheader && setting.orientation != 'v' ? setting.classdownarrow : setting.classrightarrow)
			+ '" style="border:0;" />'
		)
            if (smoothmenu.shadow.enable && setting.shadow) {
                this._shadowoffset = { x: (this.istopheader ? $subul.offset().left + smoothmenu.shadow.offsetx : this._dimensions.w), y: (this.istopheader ? $subul.offset().top + smoothmenu.shadow.offsety : $curobj.position().top)} //store this shadow's offsets
                if (this.istopheader)
                    $parentshadow = $(document.body)
                else {
                    var $parentLi = $curobj.parents("li:eq(0)")
                    $parentshadow = $parentLi.get(0).$shadow
                }
                this.$shadow = $('<div class="ddshadow' + (this.istopheader ? ' toplevelshadow' : '') + '"></div>').prependTo($parentshadow).css({ left: this._shadowoffset.x + 'px', top: this._shadowoffset.y + 'px' })  //insert shadow DIV and set it to parent node for the next shadow div
            }
            $curobj.hover(
			function(e) {
			    var $targetul = $subul //reference UL to reveal
			    var header = $curobj.get(0) //reference header LI as DOM object
			    clearTimeout($targetul.data('timers').hidetimer)
			    $targetul.data('timers').showtimer = setTimeout(function() {
			        header._offsets = { left: $curobj.offset().left, top: $curobj.offset().top }
			        var menuleft = header.istopheader && setting.orientation != 'v' ? 0 : header._dimensions.w
			        if (!ddsmoothmenu.isRTL) {
			            menuleft = (header._offsets.left + menuleft + header._dimensions.subulw > $(window).width()) ? (header.istopheader && setting.orientation != 'v' ? -header._dimensions.subulw + header._dimensions.w : -header._dimensions.w) : menuleft //calculate this sub menu's offsets from its parent
			        } else if (navigator.appVersion.indexOf("MSIE 7.") != -1) {
			            menuleft = (header._offsets.left + menuleft - header._dimensions.subulw < 0) ? menuleft : (header.istopheader && setting.orientation != 'v' ? -header._dimensions.subulw + header._dimensions.w + 2 + ddsmoothmenu.arrowimages.down[2] / 2 : -header._dimensions.w/2)
			        } else {
			            menuleft = (header._offsets.left + menuleft - header._dimensions.subulw < 0) ? menuleft : (header.istopheader && setting.orientation != 'v' ? -header._dimensions.subulw + header._dimensions.w + 2 + ddsmoothmenu.arrowimages.down[2] / 2 : -header._dimensions.w)
			        }
			        if ($targetul.queue().length <= 1) { //if 1 or less queued animations
			            $targetul.css({ left: menuleft + "px", width: header._dimensions.subulw + 'px' }).animate({ height: 'show', opacity: 'show' }, ddsmoothmenu.transition.overtime)
			            if (smoothmenu.shadow.enable && setting.shadow) {
			                var shadowleft = menuleft;
			                if (header.istopheader) {
			                    if (!ddsmoothmenu.isRTL)
			                        shadowleft = $targetul.offset().left + ddsmoothmenu.shadow.offsetx;
			                    else
			                        shadowleft = $targetul.offset().left - ddsmoothmenu.shadow.offsetx;
			                }

			                var shadowtop = header.istopheader ? $targetul.offset().top + smoothmenu.shadow.offsety : header._shadowoffset.y
			                if (!header.istopheader && ddsmoothmenu.detectwebkit) { //in WebKit browsers, restore shadow's opacity to full
			                    header.$shadow.css({ opacity: 1 })
			                }
			                header.$shadow.css({ overflow: '', width: header._dimensions.subulw + 'px', left: shadowleft + 'px', top: shadowtop + 'px' }).animate({ height: header._dimensions.subulh + 'px' }, ddsmoothmenu.transition.overtime)
			            }
			        }
			    }, ddsmoothmenu.showhidedelay.showdelay)
			},
			function(e) {
			    var $targetul = $subul
			    var header = $curobj.get(0)
			    clearTimeout($targetul.data('timers').showtimer)
			    $targetul.data('timers').hidetimer = setTimeout(function() {
			        $targetul.animate({ height: 'hide', opacity: 'hide' }, ddsmoothmenu.transition.outtime)
			        if (smoothmenu.shadow.enable && setting.shadow) {
			            if (ddsmoothmenu.detectwebkit) { //in WebKit browsers, set first child shadow's opacity to 0, as "overflow:hidden" doesn't work in them
			                header.$shadow.children('div:eq(0)').css({ opacity: 0 })
			            }
			            header.$shadow.css({ overflow: 'hidden' }).animate({ height: 0 }, ddsmoothmenu.transition.outtime)
			        }
			    }, ddsmoothmenu.showhidedelay.hidedelay)
			}
		) //end hover
        }) //end $headers.each()
        $mainmenu.find("ul").css({ display: 'none', visibility: 'visible' })
    },

    init: function(setting) {
        if (typeof setting.customtheme == "object" && setting.customtheme.length == 2) { //override default menu colors (default/hover) with custom set?
            var mainmenuid = '#' + setting.mainmenuid
            var mainselector = (setting.orientation == "v") ? mainmenuid : mainmenuid + ', ' + mainmenuid
            document.write('<style type="text/css">\n'
			+ mainselector + ' ul li a {background:' + setting.customtheme[0] + ';}\n'
			+ mainmenuid + ' ul li a:hover {background:' + setting.customtheme[1] + ';}\n'
		+ '</style>')
        }
        this.shadow.enable = (document.all && !window.XMLHttpRequest) ? false : this.shadow.enable //in IE6, always disable shadow
        jQuery(document).ready(function($) { //ajax menu?
            ddsmoothmenu.isRTL = $('body').attr('dir') == 'rtl';
            if (ddsmoothmenu.isRTL) {
                //ddsmoothmenu.shadow.enable = false;
                ddsmoothmenu.arrowimages.right[1] = ddsmoothmenu.arrowimages.right[1].replace('/right.gif', '/left.gif');
            }
            if (typeof setting.contentsource == "object") { //if external ajax menu
                ddsmoothmenu.getajaxmenu($, setting)
            }
            else { //else if markup menu
                ddsmoothmenu.buildmenu($, setting)
            }
        })
    }

} //end ddsmoothmenu variable
