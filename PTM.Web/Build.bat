echo off
xcopy /Y ..\..\PTM.Web\* Web\*
del Web\PTM.Web.projitems
del Web\PTM.Web.shproj
del Web\Build.bat