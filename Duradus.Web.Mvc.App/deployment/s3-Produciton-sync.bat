
aws s3 sync G:\Publish\angularbknd s3://backand.net --dryrun --delete --Cache-Control: max-age=3600 --acl public-read
@echo off
setlocal
:PROMPT
SET /P AREYOUSURE=Run Production Sync (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
@echo on
aws s3 sync G:\Publish\angularbknd s3://backand.net --delete --Cache-Control: max-age=3600 --acl public-read
pause
:END
endlocal