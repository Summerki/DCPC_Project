@echo off
set "exe=DirectConnectionPredictControl.exe"
set "lnk=ֱͨԤ��ά�����-V0.7.4"
mshta VBScript:Execute("Set a=CreateObject(""WScript.Shell""):Set b=a.CreateShortcut(a.SpecialFolders(""Desktop"") & ""\%lnk%.lnk""):b.TargetPath=""%~dp0%exe%"":b.WorkingDirectory=""%~dp0"":b.Save:close")
echo ���&pause