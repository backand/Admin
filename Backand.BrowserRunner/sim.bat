﻿@echo off
cd /d "bin\Debug"
for /L %%a in (1,1,3) do (
   echo This is iteration %%a
   start "" /w /b "Backand.BrowserRunner.exe"
)
pause