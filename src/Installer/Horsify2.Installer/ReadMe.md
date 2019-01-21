# Daves Wix install notes

### Running HEAT

Run Wix heat to generate components to be installed.

Remove all "Directory="TARGETDIR" refs from generated file.

Replace the Source attribute:  Source="SourceDir\e_sqlite3.dll" to the build outputs

eg. Source="..\..\Build\$(var.Platform)\e_sqlite3.dll"


If adding more libs/files to Horsify run these commands from the Build directory:

Heat Command

	heat dir ".\x64" -cg Binaries -sreg -gg -out "JukeAndImporter.wxs"

API FILES:

	Service uses different runtime files for each platform
	heat dir ".\x86\service" -cg Binaries -sreg -suid -gg -out "Apix86Service.wxs"
	heat dir ".\x64\service" -cg Binaries -sreg -suid -gg -out "Apix64Service.wxs"