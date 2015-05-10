DEL /Q G:\Publish\angularbknd-demo
xcopy G:\Dev\Itay\angularbknd\app G:\Publish\angularbknd-demo\app /E /Q /Y
xcopy G:\Dev\Itay\angularbknd\misc G:\Publish\angularbknd-demo\misc /E /Q /Y

pause
aws s3 sync G:\Publish\angularbknd-demo s3://demo.backand.net --dryrun --Cache-Control: max-age=3600 --acl public-read
@echo off
setlocal
:PROMPT
SET /P AREYOUSURE=Run Demo Sync (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
@echo on
aws s3 sync G:\Publish\angularbknd-demo s3://demo.backand.net --Cache-Control: max-age=3600 --acl public-read
pause
:END
endlocal