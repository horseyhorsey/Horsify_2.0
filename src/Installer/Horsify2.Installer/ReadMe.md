# Daves Wix install notes

### Run HEAT

Run Wix heat to generate components.

Remove all "Directory="TARGETDIR" refs from generated file.

Replace the Source attribute:  Source="SourceDir\e_sqlite3.dll"   to the build outputs

eg. Source="..\..\Build\$(var.Platform)\e_sqlite3.dll"

	heat dir ".\x64" -cg Binaries -srd -sreg -gg -out "JukeAndImporter.wxs"