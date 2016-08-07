type ..\Content\default\css\header4_setup.css space.txt ..\Content\default\css\layout2_setup.css space.txt ..\Content\default\css\layout2_text.css space.txt ..\Content\default\css\pager.css space.txt ..\Content\default\css\table.css space.txt ..\Content\default\css\form.css space.txt ..\Content\default\css\grid.css
 space.txt ..\Content\default\css\phone.css space.txt ..\Content\default\css\lookup.css > ..\Content\default\css\default.css

type ..\Content\jquery.autocomplete.css space.txt ..\Content\jquery-ui-1.7.1.custom.css space.txt ..\Content\jquery.tooltip.css space.txt  ..\Content\SpryValidation.css > ..\Content\jqueryspry.css

pause
exit

rem Type ..\lib\prototype.js space.txt ..\lib\scriptaculous.js space.txt ..\lib\builder.js space.txt ..\lib\effects.js space.txt ..\lib\dragdrop.js space.txt ..\lib\controls.js space.txt ..\lib\slider.js space.txt ..\lib\portal.js space.txt ..\lib\utils.js space.txt ..\lib\ajaxbox.js space.txt ..\lib\json.js space.txt ..\lib\portalLayout.js space.txt ..\lib\addgadget.js space.txt ..\lib\gadgetlib.js space.txt ..\lib\weather.js space.txt ..\lib\vstab.js space.txt ..\lib\spinmenu2.js space.txt ..\lib\gallery.js space.txt ..\lib\FusionCharts.js space.txt ..\lib\jDB.js space.txt ..\lib\PortalEvents.js space.txt > libmerge.tmp
rem Type ..\css\datagrid.css space.txt ..\css\employeesearch.css space.txt ..\css\vstab.css space.txt ..\css\weather.css space.txt > ..\css\gadgets.css
rem del /f ..\lib\*.js
rem java -jar custom_rhino.jar -c libmerge.tmp > ..\lib\lib.js 2>&1
rem copy portaljs.txt ..\portaljs.txt
rem pause