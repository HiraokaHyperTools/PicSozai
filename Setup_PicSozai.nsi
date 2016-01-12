; example1.nsi
;
; This script is perhaps one of the simplest NSIs you can make. All of the
; optional settings are left to their default settings. The installer simply 
; prompts the user asking them where to install, and drops a copy of example1.nsi
; there. 

;--------------------------------

!define APP "PicSozai"

; The name of the installer
Name "${APP}"

; The file to write
OutFile "Setup_${APP}.exe"

; The default installation directory
InstallDir "$APPDATA\${APP}"

; Request application privileges for Windows Vista
RequestExecutionLevel user

;--------------------------------

; Pages

Page directory
Page components
Page instfiles

;--------------------------------

; The stuff to install
Section "" ;No components page, name is not important

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File /x "*.vshost.*" "bin\DEBUG\*.*"
  
SectionEnd ; end the section

Section "起動"
  Exec '"$INSTDIR\${APP}.exe"'
SectionEnd

Section "スタートメニューへ"
  SetOutPath "$SMPROGRAMS\PicSozai"
  CreateShortCut "PicSozai.lnk" "$INSTDIR\${APP}.exe" "" "$INSTDIR\1.ico"
  CreateShortCut "画素材.lnk" "$INSTDIR\${APP}.exe" "" "$INSTDIR\1.ico"
  CreateShortCut "画像素材.lnk" "$INSTDIR\${APP}.exe" "" "$INSTDIR\1.ico"
SectionEnd
