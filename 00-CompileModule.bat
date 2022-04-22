@echo off

call MC7D2D RemoteDescription.dll /reference:"%PATH_7D2D_MANAGED%\Assembly-CSharp.dll" Harmony\*.cs Library\*.cs && ^
echo Successfully compiled RemoteDescription.dll

pause