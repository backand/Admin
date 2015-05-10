
REM CSS Start...
java -jar yuicompressor-2.4.2.jar ..\ws\css\revolution_settings.css -o 	..\ws\css\revolution_settings.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\css\bootstrap.css -o 		..\ws\css\bootstrap.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\css\font-awesome.css -o 	..\ws\css\font-awesome.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\css\animate.css -o 		..\ws\css\animate.css.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\css\isotop_animation.css -o 	..\ws\css\isotop_animation.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\css\style.css -o 		..\ws\css\style.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\css\responsive.css -o 		..\ws\css\responsive.min.css --type css --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\css\newstyle.css -o 		..\ws\css\newstyle.min.css --type css --charset utf-8

type ..\ws\css\revolution_settings.min.css space.txt ..\ws\css\bootstrap.min.css space.txt  ..\ws\css\font-awesome.min.css space.txt ..\ws\css\animate.css.min.css space.txt ..\ws\css\isotop_animation.min.css space.txt ..\ws\css\style.min.css space.txt ..\ws\css\responsive.min.css > ..\ws\css\backand-style-1.0.0.min.css

REM JAVASCRIPT start...
java -jar yuicompressor-2.4.2.jar ..\ws\js\_jq.js -o 			..\ws\js\_jq.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\_jquery.placeholder.js -o 	..\ws\js\_jquery.placeholder.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\activeaxon_menu.js -o 	..\ws\js\activeaxon_menu.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\animationEnigne.js -o 	..\ws\js\animationEnigne.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\ie-fixes.js -o 		..\ws\js\ie-fixes.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jq.appear.js -o 		..\ws\js\jq.appear.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jquery.base64.js -o 		..\ws\js\jquery.base64.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jquery.validate.js -o 	..\ws\js\jquery.validate.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jquery.isotope.js -o 	..\ws\js\jquery.isotope.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jquery.infinitescroll.js -o 	..\ws\js\jquery.infinitescroll.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jquery.themepunch.revolution.js -o ..\ws\js\jquery.themepunch.revolution.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jquery.easytabs.js -o 	..\ws\js\jquery.easytabs.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\backand_ui.js -o 		..\ws\js\backand_ui.min.js --type js --charset utf-8
java -jar yuicompressor-2.4.2.jar ..\ws\js\jquery.prettyPhoto.js -o 	..\ws\js\jquery.prettyPhoto.min.js --type js --charset utf-8

type ..\ws\js\_jq.min.js space.txt  ..\ws\js\_jquery.placeholder.min.js space.txt  ..\ws\js\activeaxon_menu.min.js space.txt  ..\ws\js\animationEnigne.min.js space.txt  ..\ws\js\bootstrap.min.js  space.txt ..\ws\js\ie-fixes.min.js space.txt ..\ws\js\jq.appear.min.js space.txt ..\ws\js\jquery.base64.min.js space.txt ..\ws\js\jquery.validate.min.js space.txt ..\ws\js\jquery.isotope.min.js space.txt ..\ws\js\jquery.infinitescroll.min.js space.txt ..\ws\js\jquery.themepunch.plugins.min.js space.txt ..\ws\js\jquery.themepunch.revolution.min.js space.txt ..\ws\js\jquery.prettyPhoto.min.js space.txt ..\ws\js\jquery.easytabs.min.js space.txt ..\ws\js\jquery.rssfeed.min.js > ..\ws\js\backand-js-1.0.0.min.js
pause
exit