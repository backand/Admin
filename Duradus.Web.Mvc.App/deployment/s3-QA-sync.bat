RD /S /Q G:\Publish\angularbknd
MD G:\Publish\angularbknd
xcopy G:\Dev\Itay\angularbknd\app G:\Publish\angularbknd /E /Q /Y

aws s3 sync G:\Publish\angularbknd s3://qa3.backand.net --dryrun --delete --Cache-Control: max-age=3600 --acl public-read
@echo off
setlocal
:PROMPT
SET /P AREYOUSURE=Run QA Sync (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
@echo on
aws s3 sync G:\Publish\angularbknd s3://qa3.backand.net --delete --Cache-Control: max-age=3600 --acl public-read
pause
:END
endlocal