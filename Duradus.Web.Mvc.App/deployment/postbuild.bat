
REM BUILD
java -jar yuicompressor-2.4.2.jar ..\Scripts\form.1.0.js -o ..\Scripts\form.1.0.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\Editor.js -o ..\Scripts\Editor.min.js --type js --charset utf-8

java -jar yuicompressor-2.4.2.jar ..\Scripts\dropdowndiv.durados.jquery.js -o ..\Scripts\dropdowndiv.durados.jquery.min.js --type js --charset utf-8

REM java -jar yuicompressor-2.4.2.jar ..\Scripts\spry.js -o ..\Scripts\spry.min.js --type js --charset utf-8
REM java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.cookie.js -o ..\Scripts\jquery.cookie.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\ddsmoothmenu.js -o ..\Scripts\ddsmoothmenu.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\ajaxupload.3.6.js -o ..\Scripts\ajaxupload.3.6.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.contextMenu.js -o ..\Scripts\jquery.contextMenu.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\ui.dropdownchecklist.js -o ..\Scripts\ui.dropdownchecklist-1.1-min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.crayonbox.js -o ..\Scripts\jquery.crayonbox.min.js --type js --charset utf-8
REM tooltip
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.hoverIntent.js -o ..\Scripts\jquery.hoverIntent.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.cluetip.js -o ..\Scripts\jquery.cluetip.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jHtmlArea-0.7.0.js -o ..\Scripts\jHtmlArea-0.7.0.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.jstree.js -o ..\Scripts\jquery.jstree.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.superfish.js -o ..\Scripts\jquery.superfish.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.anythingslider.js -o ..\Scripts\jquery.anythingslider.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.scrollintoview.js -o ..\Scripts\jquery.scrollintoview.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.checkbox.js -o ..\Scripts\jquery.checkbox.min.js --type js --charset utf-8
REM datetime
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery-ui-sliderAccess.js -o ..\Scripts\jquery-ui-sliderAccess.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery-ui-timepicker-addon.js -o ..\Scripts\jquery-ui-timepicker-addon.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\DateJs\globalization\en-US.js -o ..\Scripts\DateJs\globalization\en-US.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\DateJs\date.js -o ..\Scripts\DateJs\date.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\DateJs\time.js -o ..\Scripts\DateJs\time.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\spectrum.js -o ..\Scripts\spectrum.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Scripts\jquery.nestable.js -o ..\Scripts\jquery.nestable.min.js --type js --charset utf-8



REM Javascript
type ..\Scripts\jquery.min.js space.txt ..\Scripts\jquery-ui.min.js space.txt ..\Scripts\MicrosoftAjax.js space.txt ..\Scripts\jquery.crayonbox.min.js space.txt ..\Scripts\MicrosoftMvcAjax.js space.txt ..\Scripts\jquery_autocomplte_min.js space.txt ..\Scripts\jquery.toChecklist.min.js space.txt ..\Scripts\jHtmlArea-0.7.0.min.js space.txt ..\Scripts\jquery_printElement_min__js__0.js space.txt ..\Scripts\ui.dropdownchecklist-1.1-min.js space.txt ..\Scripts\form.1.0.min.js space.txt ..\Scripts\Editor.min.js space.txt ..\Scripts\dropdowndiv.durados.jquery.min.js space.txt ..\Scripts\spry.min.js space.txt ..\Scripts\jquery.cookie.min.js space.txt ..\Scripts\ddsmoothmenu.min.js space.txt ..\Scripts\ajaxupload.3.6.min.js  space.txt ..\Scripts\jquery.contextMenu.min.js space.txt ..\Scripts\jquery.hoverIntent.min.js space.txt ..\Scripts\jquery.bgiframe.min.js space.txt ..\Scripts\jquery.cluetip.min.js space.txt ..\Scripts\jquery.jstree.min.js space.txt ..\Scripts\jquery.superfish.min.js space.txt ..\Scripts\jQueryRotate.js space.txt ..\Scripts\jquery.checkbox.min.js space.txt ..\Scripts\jquery.anythingslider.min.js space.txt ..\Scripts\jquery.scrollintoview.min.js space.txt ..\Scripts\jquery-ui-sliderAccess.min.js space.txt ..\Scripts\jquery-ui-timepicker-addon.min.js space.txt ..\Scripts\DateJs\globalization\en-US.min.js space.txt ..\Scripts\DateJs\date.min.js space.txt ..\Scripts\DateJs\time.min.js space.txt ..\Scripts\spectrum.min.js space.txt ..\Scripts\jquery.nestable.min.js > ..\Scripts\general-min-1.0.0.js

REM Create JS FOR Designer
type ..\Scripts\dbdesigner.oz.js space.txt ..\Scripts\dbdesigner.Config.js space.txt ..\Scripts\dbdesigner.js space.txt > ..\Scripts\dbdesigner.All.js
java -jar yuicompressor-2.4.2.jar ..\Scripts\dbdesigner.All.js -o ..\Scripts\dbdesigner.All.min.js --type js --charset utf-8
type ..\Scripts\jquery-1.4.2.min.js space.txt ..\Scripts\dbdesigner.All.min.js > ..\Scripts\dbdesigner-1.0.0.min.js
REM java -jar yuicompressor-2.4.2.jar ..\Content\ERD\print.css -o ..\Content\ERD\print.min.css --type css --charset utf-8
REM java -jar yuicompressor-2.4.2.jar ..\Content\ERD\style.css -o ..\Content\ERD\style.min.css --type css --charset utf-8
REM type ..\Content\ERD\style.min.css space.txt ..\Content\ERD\print.min.css > ..\Content\ERD\dbdesigner.min.css

REM CSS Start...
java -jar yuicompressor-2.4.2.jar ..\Content\Stylesheet.css -o ..\Content\Stylesheet.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Content\Gray.css -o ..\Content\gray.min.css --type css --charset utf-8

java -jar yuicompressor-2.4.2.jar ..\Content\styles.css -o ..\Content\styles.min.css --type css --charset utf-8


java -jar yuicompressor-2.4.2.jar ..\Content\ddsmoothmenu.css -o ..\Content\ddsmoothmenu.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Content\SpryValidation.css -o ..\Content\SpryValidation.min.css --type css --charset utf-8

java -jar yuicompressor-2.4.2.jar ..\Content\jquery.contextMenu.css -o ..\Content\jquery.contextMenu.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Content\anythingslider.css -o ..\Content\anythingslider.min.css --type css --charset utf-8

java -jar yuicompressor-2.4.2.jar ..\Content\ui.dropdownchecklist.standalone.css -o ..\Content\ui.dropdownchecklist.standalone.min.css --type css --charset utf-8
REM java -jar yuicompressor-2.4.2.jar ..\Content\ui.dropdownchecklist.themeroller.css -o ..\Content\ui.dropdownchecklist.themeroller.min.css --type css --charset utf-8
REM java -jar yuicompressor-2.4.2.jar ..\Content\jHtmlArea\jHtmlArea.css -o ..\Content\jHtmlArea\jHtmlArea.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Content\smoothness\jquery-ui-1.7.1.custom.css -o ..\Content\smoothness\jquery-ui-1.7.1.custom.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\Content\spectrum.css -o ..\Content\spectrum.min.css --type css --charset utf-8

type ..\Content\styles.min.css ..\Content\SpryValidation.min.css space.txt ..\Content\jquery.toChecklist.min.css space.txt ..\Content\Stylesheet.min.css space.txt ..\Content\gray.min.css space.txt ..\Content\ddsmoothmenu.min.css space.txt ..\Content\ui.dropdownchecklist.standalone.min.css space.txt ..\Content\ui.dropdownchecklist.themeroller.min.css space.txt ..\Content\jquery.contextMenu.min.css space.txt ..\Content\anythingslider.min.css space.txt ..\Content\spectrum.min.css > ..\Content\general.min.1.0.0.css


REM create javascript version
Durados.Version.exe version.txt 4

pause
exit