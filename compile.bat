@echo off

@REM This is the compile command
csc /out:Multi.exe /m:Multi /win32icon:icons/multix32.ico .\Rewrite\*.cs

@REM These are unit testing because why not. Compiles, runs, then deletes the exe
csc /out:Testing.exe /m:UnitTests .\Rewrite\*.cs >bin/nul
.\Testing.exe
del .\Testing.exe