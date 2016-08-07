aws s3 sync G:\Publish\ngback s3://qa3.backand.net --dryrun --Cache-Control: max-age=3600 --acl public-read
@echo off
setlocal
:PROMPT
SET /P AREYOUSURE=Run QA Sync (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
@echo on
aws s3 sync G:\Publish\ngback s3://qa3.backand.net --Cache-Control: max-age=3600 --acl public-read
pause
:END
endlocal