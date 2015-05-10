/*
Sequence.js (www.sequencejs.com)
Version: 0.6.8 Beta
Author: Ian Lunn @IanLunn
Author URL: http://www.ianlunn.co.uk/
Github: https://github.com/IanLunn/Sequence

This is a FREE script and is dual licensed under the following:
http://www.opensource.org/licenses/mit-license.php | http://www.gnu.org/licenses/gpl.html

Sequence.js and its dependencies are (c) Ian Lunn Design 2012 unless otherwise stated.
Aside from these comments, you may modify and distribute this file as you please. Have fun!
*/(function(e){function t(t,n,r,i){function f(){function r(t){s.containerLeft=s.container.position().left;s.containerRight=s.containerLeft+s.container.width();s.containerTop=s.container.position().top;s.containerBottom=s.containerTop+s.container.height();var n=t.pageX,r=t.pageY;if(n>=s.containerLeft&&n<=s.containerRight&&r>=s.containerTop&&r<=s.containerBottom){s.settings.autoPlay=!1;clearTimeout(s.sequenceTimer);e(s.settings.pauseIcon).show();s.settings.pauseButton!==undefined&&s.settings.pauseButton.addClass("paused");s.settings.paused();s.sequence.unbind("mousemove")}}e(s.settings.preloader).remove();s.settings.nextButton=s.init.uiElements(s.settings.prependNextButton,s.settings.nextButton,".next",s.settings.nextButtonSrc,s.settings.nextButtonAlt);s.settings.prevButton=s.init.uiElements(s.settings.prependPrevButton,s.settings.prevButton,".prev",s.settings.prevButtonSrc,s.settings.prevButtonAlt);s.settings.pauseButton=s.init.uiElements(s.settings.prependPauseButton,s.settings.pauseButton,".pause",s.settings.pauseButtonSrc,s.settings.pauseButtonAlt);s.settings.nextButton!==undefined&&s.settings.nextButton!==!1&&s.settings.showNextButtonOnInit&&s.settings.nextButton.show();s.settings.prevButton!==undefined&&s.settings.prevButton!==!1&&s.settings.showPrevButtonOnInit&&s.settings.prevButton.show();s.settings.pauseButton!==undefined&&s.settings.pauseButton!==!1&&s.settings.pauseButton.show();if(s.settings.pauseIcon!==!1){s.settings.pauseIcon=s.init.uiElements(s.settings.prependPauseIcon,s.settings.pauseIcon,".pause-icon",s.settings.pauseIconSrc);s.settings.pauseIcon!==undefined&&s.settings.pauseIcon.hide()}s.hasTouch&&(s.settings.calculatedSwipeThreshold=s.container.width()*(s.settings.swipeThreshold/100));s.nextFrame=s.sequence.children("li:nth-child("+s.settings.startingFrameID+")");s.nextFrameChildren=s.nextFrame.children();s.nextFrameID=s.settings.startingFrameID;s.direction;s.sequence.css({width:"100%",height:"100%",position:"relative"});s.sequence.children("li").css({width:"100%",height:"100%",position:"absolute"});if(s.transitionsSupported)if(!s.settings.animateStartingFrameIn){s.currentFrame=s.nextFrame.addClass("current-frame");s.currentFrameChildren=s.currentFrame.children();s.currentFrameID=s.settings.startingFrameID;s.modifyElements(s.currentFrameChildren,"0s");s.currentFrameChildren.addClass("animate-in");setTimeout(function(){s.modifyElements(s.currentFrameChildren,"")},100);if(s.settings.autoPlay){var t=function(){s.autoPlaySequence()};clearTimeout(s.sequenceTimer);s.sequenceTimer=setTimeout(t,s.settings.autoPlayDelay,s)}}else if(s.settings.reverseAnimationsWhenNavigatingBackwards&&s.settings.autoPlayDirection-1&&s.settings.animateStartingFrameIn){s.modifyElements(s.nextFrameChildren,"0s");s.nextFrameChildren.addClass("animate-out");s.goTo(1,-1)}else s.goTo(1,1);else{s.container.addClass("sequence-fallback");s.currentFrame=s.nextFrame;s.currentFrame.addClass("current-frame");s.settings.beforeNextFrameAnimatesIn();s.settings.afterNextFrameAnimatesIn();s.currentFrameChildren=s.currentFrame.children();s.currentFrameID=s.settings.startingFrameID;s.sequence.children("li").children().addClass("animate-in");s.sequence.children(":not(li:nth-child("+s.settings.startingFrameID+"))").css({display:"none",opacity:0});if(s.settings.autoPlay){var t=function(){s.autoPlaySequence()};clearTimeout(s.sequenceTimer);s.sequenceTimer=setTimeout(t,s.settings.autoPlayDelay,s)}}s.settings.nextButton!==undefined&&s.settings.nextButton.click(function(){s.next()});s.settings.prevButton!==undefined&&s.settings.prevButton.click(function(){s.prev()});s.settings.pauseButton!==undefined&&s.settings.pauseButton.click(function(){s.pause()});if(s.settings.keyNavigation){var n={left:37,right:39};e(document).keydown(function(e){function t(e,t){var r;for(keyCodes in t){keyCodes==="left"||keyCodes==="right"?r=n[keyCodes]:r=keyCodes;e===r&&s.initCustomKeyEvent(t[keyCodes])}}var r=parseFloat(String.fromCharCode(e.keyCode));if(r>0&&r<=s.numberOfFrames&&s.settings.numericKeysGoToFrames){s.nextFrameID=r;s.goTo(r)}t(e.keyCode,s.settings.keyEvents);t(e.keyCode,s.settings.customKeyEvents)})}if(s.settings.pauseOnHover&&!s.settings.pauseOnElementsOutsideContainer&&s.settings.autoPlay){s.hoverEvent=s.sequence.mousemove(function(e){r(e)});s.sequence.mouseleave(function(t){s.settings.autoPlay=!0;var n=function(){s.autoPlaySequence()};clearTimeout(s.sequenceTimer);s.sequenceTimer=setTimeout(n,s.settings.autoPlayDelay,s);e(s.settings.pauseIcon).hide();s.settings.pauseButton!==undefined&&s.settings.pauseButton.removeClass("paused");s.settings.unpaused();s.sequence.data("events").mousemove===undefined&&s.sequence.mousemove(function(e){r(e)})})}else s.settings.pauseOnHover&&s.settings.autoPlay&&(s.hoverEvent=s.sequence.hover(function(t){s.settings.autoPlay=!1;clearTimeout(s.sequenceTimer);e(s.settings.pauseIcon).show();s.settings.pauseButton!==undefined&&s.settings.pauseButton.addClass("paused");s.settings.paused()},function(){s.settings.autoPlay=!0;var t=function(){s.autoPlaySequence()};clearTimeout(s.sequenceTimer);s.sequenceTimer=setTimeout(t,s.settings.autoPlayDelay,s);e(s.settings.pauseIcon).hide();s.settings.pauseButton!==undefined&&s.settings.pauseButton.removeClass("paused");s.settings.unpaused()}));if(s.settings.swipeNavigation&&s.hasTouch){var i={touchstartX:-1,touchstartY:-1,touchmoveX:-1,touchmoveY:-1};s.sequence.on("touchstart touchmove touchend",function(e){s.settings.swipePreventsDefault&&e.preventDefault();switch(e.originalEvent.type){case"touchmove":case"touchstart":i[e.originalEvent.type+"X"]=e.originalEvent.touches[0].pageX;i[e.originalEvent.type+"Y"]=e.originalEvent.touches[0].pageY;break;case"touchend":if(i.touchmoveX!==-1){var t=i.touchmoveX-i.touchstartX,n=i.touchmoveY-i.touchstartY;Math.abs(t)>Math.abs(n)&&t>s.settings.calculatedSwipeThreshold?s.initCustomKeyEvent(s.settings.swipeEvents.right):Math.abs(t)>Math.abs(n)&&Math.abs(t)>s.settings.calculatedSwipeThreshold?s.initCustomKeyEvent(s.settings.swipeEvents.left):Math.abs(n)>Math.abs(t)&&n>s.settings.calculatedSwipeThreshold?s.initCustomKeyEvent(s.settings.swipeEvents.down):Math.abs(n)>Math.abs(t)&&Math.abs(n)>s.settings.calculatedSwipeThreshold&&s.initCustomKeyEvent(s.settings.swipeEvents.up);i={touchstartX:-1,touchstartY:-1,touchmoveX:-1,touchmoveY:-1}}break;default:}})}e(window).resize(function(){s.settings.calculatedSwipeThreshold=s.container.width()*(s.settings.swipeThreshold/100)})}var s=this;s.container=e(t),s.sequence=s.container.children("ul");try{Modernizr.prefixed;if(Modernizr.prefixed===undefined)throw"undefined"}catch(o){i.modernizr()}var u={WebkitTransition:"-webkit-",MozTransition:"-moz-",OTransition:"-o-",msTransition:"-ms-",transition:""},a={WebkitTransition:"webkitTransitionEnd webkitAnimationEnd",MozTransition:"transitionend animationend",OTransition:"oTransitionEnd oAnimationEnd otransitionend oanimationend",msTransition:"MSTransitionEnd MSAnimationEnd",transition:"transitionend animationend"};s.prefix=u[Modernizr.prefixed("transition")],s.transitionEnd=a[Modernizr.prefixed("transition")],s.transitionProperties={},s.numberOfFrames=s.sequence.children("li").length,s.transitionsSupported=s.prefix!==undefined?!0:!1,s.hasTouch="ontouchstart"in window?!0:!1,s.sequenceTimer,s.paused=!1,s.hoverEvent,s.defaultPreloader,s.init={preloader:function(t){s.prependTo=s.settings.prependPreloader==1?s.container:s.settings.prependPreloader;switch(t){case!0:case undefined:i.defaultPreloader(s.prependTo,s.transitionsSupported,s.prefix);(!s.transitionsSupported||s.prefix==="-o-")&&s.preloaderFallback();return e(".sequence-preloader");case!1:break;default:this.CSSSelectorToHTML(t);return e(t)}},uiElements:function(t,n,r,i,o){switch(n){case!1:return undefined;case!0:t===!0?s.container.prepend("<img "+this.CSSSelectorToHTML(r)+'src="'+i+'" alt="'+o+'" />'):t!==!1&&e(t).prepend("<img "+this.CSSSelectorToHTML(r)+'src="'+i+'" alt="'+o+'" />');return e(r);default:var u=t==1?s.container:t;e(u).prepend("<img "+this.CSSSelectorToHTML(n)+'src="'+i+'" alt="'+o+'" />');return e(n)}},CSSSelectorToHTML:function(e){switch(e.charAt(0)){case".":return'class="'+e.split(".")[1]+'"';case"#":return'id="'+e.split("#")[1]+'"';default:return e}}},s.settings=e.extend({},r,n),s.settings.preloader=s.init.preloader(s.settings.preloader);s.firstFrame=s.settings.animateStartingFrameIn?!0:!1;s.settings.hideFramesUntilPreloaded&&s.settings.preloader&&s.sequence.children("li").hide();if(s.settings.animateStartingFrameIn){s.modifyElements(s.sequence.children("li").children(),"0s");s.sequence.children("li").children().removeClass("animate-in")}e(window).bind("load",function(){s.settings.afterLoaded();s.settings.hideFramesUntilPreloaded&&s.settings.preloader&&s.sequence.children("li").show();if(s.settings.preloader)if(s.settings.hidePreloaderUsingCSS&&s.transitionsSupported&&s.prefix!=="-o-"){s.prependPreloadingCompleteTo=s.settings.prependPreloadingComplete==1?s.settings.preloader:e(s.settings.prependPreloadingComplete);s.prependPreloadingCompleteTo.addClass("preloading-complete");setTimeout(f,s.settings.hidePreloaderDelay)}else s.settings.preloader.fadeOut(s.settings.hidePreloaderDelay,function(){clearInterval(s.defaultPreloader);f()});else f();e(this).unbind("load")})}t.prototype={preloaderFallback:function(){function r(){n=n===1?0:1;e(".sequence-preloader img:nth-child(1)").animate({opacity:n},100);e(".sequence-preloader img:nth-child(2)").animate({opacity:n},350);e(".sequence-preloader img:nth-child(3)").animate({opacity:n},600)}var t=this,n=0;r();t.defaultPreloader=setInterval(function(){r()},600)},initCustomKeyEvent:function(e){var t=this;switch(e){case"next":t.next();break;case"prev":t.prev();break;case"pause":t.pause()}},autoPlaySequence:function(e){var t=this;t.settings.autoPlayDirection===1?t.next():t.prev()},modifyElements:function(e,t){var n=this;e.css(n.prefixCSS(n.prefix,{"transition-duration":t,"transition-delay":t}))},prefixCSS:function(e,t){var n={};for(property in t)n[e+property]=t[property];return n},getStyleBySelector:function(e){var t={},n=document.styleSheets,r,i,s;for(i=n.length-1;i>=0;i--){var o=!1;try{r=n[i].cssRules}catch(u){o=!0}if(!o)for(s=0;s<r.length;s++)if(r[s].type===CSSRule.STYLE_RULE&&r[s].selectorText===e){t["-o-transition-duration"]=r[s].style.OTransitionDuration;t["-o-transition-delay"]=r[s].style.OTransitionDelay;return t}}return null},setTransitionProperties:function(t){var n=this;n.modifyElements(n.frameChildren,"");n.frameChildren.each(function(){if(n.prefix==="-o-"){var t="."+e(this).attr("class").replace(" ","."),r=n.getStyleBySelector(t);n.transitionProperties["transition-duration"]=r["-o-transition-duration"];n.transitionProperties["transition-delay"]=r["-o-transition-delay"];n.transitionProperties["transition-delay"]=n.transitionProperties["transition-delay"]===""?"0s":n.transitionProperties["transition-delay"]}else{n.transitionProperties["transition-duration"]=e(this).css(n.prefix+"transition-duration");n.transitionProperties["transition-delay"]=e(this).css(n.prefix+"transition-delay")}e(this).css(n.prefixCSS(n.prefix,n.transitionProperties))})},pause:function(){var e=this;if(e.settings.autoPlay){e.settings.pauseButton!==undefined&&e.settings.pauseButton.addClass("paused");e.settings.paused();e.stopAutoPlay()}else{e.settings.pauseButton!==undefined&&e.settings.pauseButton.removeClass("paused");e.settings.unpaused();e.startAutoPlay(e.settings.unpauseDelay)}},startAutoPlay:function(t,n){var r=this;t=t===undefined?0:t;r.settings.autoPlayDelay=n===undefined?r.settings.autoPlayDelay:n;r.settings.autoPlay=!0;var i=function(){r.autoPlaySequence()};clearTimeout(r.sequenceTimer);r.sequenceTimer=setTimeout(i,t,r);r.settings.pauseOnHover&&(r.hoverEvent=r.sequence.hover(function(){r.settings.autoPlay=!1;clearTimeout(r.sequenceTimer);e(r.settings.pauseIcon).show();r.settings.pauseButton!==undefined&&r.settings.pauseButton.addClass("paused");r.settings.paused()},function(){r.settings.autoPlay=!0;var t=function(){r.autoPlaySequence()};clearTimeout(r.sequenceTimer);r.sequenceTimer=setTimeout(t,r.settings.autoPlayDelay,r);e(r.settings.pauseIcon).hide();r.settings.pauseButton!==undefined&&r.settings.pauseButton.removeClass("paused");r.settings.unpaused()}))},stopAutoPlay:function(){var e=this;e.settings.autoPlay=!1;clearTimeout(e.sequenceTimer);e.hoverEvent!==undefined&&e.hoverEvent.unbind()},next:function(){var e=this;if(!e.active)if(e.settings.cycle||!e.settings.cycle&&e.currentFrameID!==e.numberOfFrames){if(e.paused){e.paused=!1;e.startAutoPlay()}e.nextFrameID=e.currentFrameID!==e.numberOfFrames?e.currentFrameID+1:1;e.goTo(e.nextFrameID,1)}else if(e.settings.autoPlayDirection===1){e.paused=!0;e.stopAutoPlay()}},prev:function(){var e=this;if(!e.active)if(e.settings.cycle||!e.settings.cycle&&e.currentFrameID!==1){if(e.paused){e.paused=!1;e.startAutoPlay()}e.nextFrameID=e.currentFrameID===1?e.numberOfFrames:e.currentFrameID-1;e.goTo(e.nextFrameID,-1)}else if(e.settings.autoPlayDirection===-1){e.paused=!0;e.stopAutoPlay()}},goTo:function(e,t){var n=this;e===n.numberOfFrames?n.settings.beforeLastFrameAnimatesIn():e===1&&n.settings.beforeFirstFrameAnimatesIn();if(n.currentFrame!==undefined&&e===n.currentFrame.index()+1)return!1;if(!n.active){n.active=!0;n.currentFrame=n.sequence.children(".current-frame");n.nextFrame=n.sequence.children("li:nth-child("+e+")");t===undefined?n.direction=e>n.currentFrameID?1:-1:n.direction=t;n.frameChildren=n.currentFrame.children();n.nextFrameChildren=n.nextFrame.children();if(n.transitionsSupported){if(n.currentFrame.length!==0){n.settings.beforeCurrentFrameAnimatesOut();n.animateOut(n.direction)}var r=function(){n.animateIn(n.direction);n.currentFrameID=e};if(!n.firstFrame)switch(n.settings.transitionThreshold){case!0:n.waitForAnimationsToComplete(n.currentFrame,n.frameChildren,"out");break;case!1:r();break;default:setTimeout(r,n.settings.transitionThreshold)}else{r();n.firstFrame=!1}}else switch(n.settings.fallback.theme){case"slide":var i={},r={},s={};if(n.direction===1){i.left="-100%";r.left="100%"}else{i.left="100%";r.left="-100%"}s.left="0%";s.opacity=1;n.settings.beforeCurrentFrameAnimatesOut();n.currentFrame.removeClass("current-frame").animate(i,n.settings.fallback.speed,function(){});n.settings.beforeNextFrameAnimatesIn();n.nextFrame.addClass("current-frame").show().css(r).animate(s,n.settings.fallback.speed,function(){n.settings.afterNextFrameAnimatesIn();n.currentFrame=n.nextFrame;n.currentFrameID=n.currentFrame.index()+1;n.active=!1;if(n.settings.autoPlay){var e=function(){n.autoPlaySequence()};clearTimeout(n.sequenceTimer);n.sequenceTimer=setTimeout(e,n.settings.autoPlayDelay,n)}});break;case"fade":default:n.sequence.children("li").css({position:"relative"});n.settings.beforeCurrentFrameAnimatesOut();n.currentFrame.animate({opacity:0},n.settings.fallback.speed,function(){n.currentFrame.css({display:"none","z-index":"1"});n.currentFrame.removeClass("current-frame");n.settings.beforeNextFrameAnimatesIn();n.nextFrame.addClass("current-frame").css({display:"block","z-index":n.numberOfFrames}).animate({opacity:1},500,function(){n.settings.afterNextFrameAnimatesIn()});n.currentFrame=n.nextFrame;n.currentFrameID=n.currentFrame.index()+1;n.active=!1;if(n.settings.autoPlay){var e=function(){n.autoPlaySequence()};clearTimeout(n.sequenceTimer);n.sequenceTimer=setTimeout(e,n.settings.autoPlayDelay,n)}});n.sequence.children("li").css({position:"relative"})}}},animateOut:function(e){var t=this;t.currentFrame.removeClass("current-frame");t.nextFrame.addClass("next-frame");if(!t.settings.reverseAnimationsWhenNavigatingBackwards||e===1){t.modifyElements(t.nextFrameChildren,"0s");t.nextFrameChildren.removeClass("animate-out");t.modifyElements(t.frameChildren,"");t.frameChildren.addClass("animate-out").removeClass("animate-in")}if(t.settings.reverseAnimationsWhenNavigatingBackwards&&e===-1){t.modifyElements(t.nextFrameChildren,"0s");t.nextFrameChildren.addClass("animate-out");t.setTransitionProperties(t.frameChildren);t.frameChildren.removeClass("animate-in")}t.settings.transitionThreshold&&t.waitForAnimationsToComplete(t.currentFrame,t.currentFrame.children(),"out",!0)},animateIn:function(e){var t=this;t.currentFrame.unbind(t.transitionEnd);t.currentFrame=t.nextFrame;e===1?t.currentFrameID=t.currentFrameID!==t.numberOfFrames?t.currentFrameID+1:1:t.currentFrameID=t.currentFrameID!==1?t.currentFrameID-1:t.numberOfFrames;t.nextFrameChildren=t.nextFrame.children();t.frameChildren=t.currentFrame.children();t.settings.beforeNextFrameAnimatesIn();!t.settings.reverseAnimationsWhenNavigatingBackwards||e===1?setTimeout(function(){t.modifyElements(t.frameChildren,"");t.frameChildren.addClass("animate-in");t.waitForAnimationsToComplete(t.nextFrame,t.nextFrameChildren,"in");t.settings.transitionThreshold!==!0&&t.settings.afterCurrentFrameAnimatesOut!="function () {}"&&t.waitForAnimationsToComplete(t.currentFrame,t.currentFrame.children(),"out")},50):t.settings.reverseAnimationsWhenNavigatingBackwards&&e===-1&&setTimeout(function(){t.setTransitionProperties(t.frameChildren);t.frameChildren.addClass("animate-in").removeClass("animate-out");t.waitForAnimationsToComplete(t.nextFrame,t.nextFrameChildren,"in");t.settings.transitionThreshold!==!0&&t.settings.afterCurrentFrameAnimatesOut!="function () {}"&&t.waitForAnimationsToComplete(t.currentFrame,t.currentFrame.children(),"out")},50)},waitForAnimationsToComplete:function(t,n,r,i){var s=this;if(r==="out")var o=function(){s.active=!1;t.unbind(s.transitionEnd);s.settings.afterCurrentFrameAnimatesOut();i&&s.animateIn(s.direction)};else if(r==="in")var o=function(){t.unbind(s.transitionEnd);s.settings.afterNextFrameAnimatesIn();s.currentFrameID===s.numberOfFrames?s.settings.afterLastFrameAnimatesIn():s.currentFrameID===1&&s.settings.afterFirstFrameAnimatesIn();s.nextFrame.removeClass("next-frame").addClass("current-frame");s.active=!1;if(s.settings.autoPlay){var e=function(){s.autoPlaySequence()};clearTimeout(s.sequenceTimer);s.sequenceTimer=setTimeout(e,s.settings.autoPlayDelay,s)}};n.each(function(){e(this).data("animationEnded",!1)});s.currentFrame.bind(s.transitionEnd,function(t){e(t.target).data("animationEnded",!0);var r=!0;n.each(function(){e(this).data("animationEnded")===!1&&(r=!1)});r&&o()})}};e.fn.sequence=function(i){return this.each(function(){var s=new t(e(this),i,r,n);e(this).data("sequence",s)})};var n={modernizr:function(){window.Modernizr=function(e,t,n){function r(e){v.cssText=e}function i(e,t){return r(prefixes.join(e+";")+(t||""))}function s(e,t){return typeof e===t}function o(e,t){return!!~(""+e).indexOf(t)}function u(e,t){for(var r in e)if(v[e[r]]!==n)return t=="pfx"?e[r]:!0;return!1}function a(e,t,r){for(var i in e){var o=t[e[i]];if(o!==n)return r===!1?e[i]:s(o,"function")?o.bind(r||t):o}return!1}function f(e,t,n){var r=e.charAt(0).toUpperCase()+e.substr(1),i=(e+" "+b.join(r+" ")+r).split(" ");return s(t,"string")||s(t,"undefined")?u(i,t):(i=(e+" "+w.join(r+" ")+r).split(" "),a(i,t,n))}var l="2.5.3",c={},h=t.documentElement,p="modernizr",d=t.createElement(p),v=d.style,m,g={}.toString,y="Webkit Moz O ms",b=y.split(" "),w=y.toLowerCase().split(" "),E={},S={},x={},T=[],N=T.slice,C,k={}.hasOwnProperty,L;!s(k,"undefined")&&!s(k.call,"undefined")?L=function(e,t){return k.call(e,t)}:L=function(e,t){return t in e&&s(e.constructor.prototype[t],"undefined")},Function.prototype.bind||(Function.prototype.bind=function(e){var t=self;if(typeof t!="function")throw new TypeError;var n=N.call(arguments,1),r=function(){if(self instanceof r){var i=function(){};i.prototype=t.prototype;var s=new i,o=t.apply(s,n.concat(N.call(arguments)));return Object(o)===o?o:s}return t.apply(e,n.concat(N.call(arguments)))};return r});for(var A in E)L(E,A)&&(C=A.toLowerCase(),c[C]=E[A](),T.push((c[C]?"":"no-")+C));return r(""),d=m=null,c._version=l,c._domPrefixes=w,c._cssomPrefixes=b,c.testProp=function(e){return u([e])},c.testAllProps=f,c.prefixed=function(e,t,n){return t?f(e,t,n):f(e,"pfx")},c}(self,self.document)},defaultPreloader:function(t,n,r){var i=n?0:1;e("head").append("<style>.sequence-preloader{height: 100%;position: absolute;width: 100%;z-index: 999999;}@"+r+"keyframes preload{0%{opacity: 0;}50%{opacity: 1;}100%{opacity: 0;}}.sequence-preloader img{background: #ff9933;border-radius: 6px;display: inline-block;height: 12px;opacity: "+i+";position: relative;top: -50%;width: 12px;"+r+"animation: preload 1s infinite; animation: preload 1s infinite;}.preloading{height: 12px;margin: 0 auto;top: 50%;position: relative;width: 48px;}.sequence-preloader img:nth-child(2){"+r+"animation-delay: .15s; animation-delay: .15s;}.sequence-preloader img:nth-child(3){"+r+"animation-delay: .3s; animation-delay: .3s;}.preloading-complete{opacity: 0;visibility: hidden;"+r+"transition-duration: 1s; transition-duration: 1s;}</style>");e(t).prepend('<div class="sequence-preloader"><div class="preloading"><img src="website/images/sequence-preloader.png" alt="Sequence is loading, please wait..." />    <img src="website/images/sequence-preloader.png" alt="Sequence is loading, please wait..." />    <img src="website/images/sequence-preloader.png" alt="Sequence is loading, please wait..." /></div></div>')}},r={startingFrameID:1,cycle:!0,animateStartingFrameIn:!1,transitionThreshold:1e3,reverseAnimationsWhenNavigatingBackwards:!0,autoPlay:!0,autoPlayDirection:1,autoPlayDelay:5e3,nextButton:!1,prependNextButton:!1,nextButtonSrc:"website/images/bt-next.png",nextButtonAlt:"&#gt;",showNextButtonOnInit:!0,prevButton:!1,prependPrevButton:!1,prevButtonSrc:"website/images/bt-prev.png",prevButtonAlt:"&#lt;",showPrevButtonOnInit:!0,pauseButton:!1,prependPauseButton:!1,pauseButtonSrc:"website/images/bt-pause.png",pauseButtonAlt:"&#166;&#166;",unpauseDelay:0,pauseOnHover:!0,pauseIcon:!1,prependPauseIcon:!1,pauseIconSrc:"website/images/pause-icon.png",pauseAlt:"Pause",pauseOnElementsOutsideContainer:!1,preloader:!0,prependPreloader:!0,hideFramesUntilPreloaded:!0,prependPreloadingComplete:!0,hidePreloaderUsingCSS:!0,hidePreloaderDelay:0,keyNavigation:!0,numericKeysGoToFrames:!0,keyEvents:{left:"prev",right:"next"},customKeyEvents:{},swipeNavigation:!0,swipeThreshold:15,swipePreventsDefault:!1,swipeEvents:{left:"prev",right:"next",up:!1,down:!1},fallback:{theme:"slide",mode:"horizontal",speed:500},paused:function(){},unpaused:function(){},beforeNextFrameAnimatesIn:function(){},afterNextFrameAnimatesIn:function(){},beforeCurrentFrameAnimatesOut:function(){},afterCurrentFrameAnimatesOut:function(){},beforeFirstFrameAnimatesIn:function(){},afterFirstFrameAnimatesIn:function(){},beforeLastFrameAnimatesIn:function(){},afterLastFrameAnimatesIn:function(){},afterLoaded:function(){}}})(jQuery);