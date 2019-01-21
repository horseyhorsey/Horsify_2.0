# Delete all built source from projects and build
#./DeleteObjBinFolders.ps1

# BUILD X64 Jukebox importer
msbuild "src/ui/horsesoft.music.horsify.wpf.shell/horsesoft.music.horsify.wpf.shell.csproj" /property:configuration=release /property:platform=x64 /property:solutiondir="../../../../../"
msbuild "src/UI/Horsesoft.Music.Horsify.Importer.UI.WPF/Horsesoft.Music.Horsify.Importer.UI.WPF.csproj" /property:Configuration=Release /property:Platform=x64 /property:SolutionDir="../../../../../"

# # # BUILD X86 Jukebox, Importer
msbuild "src/UI/Horsesoft.Music.Horsify.WPF.Shell/Horsesoft.Music.Horsify.WPF.Shell.csproj" /property:Configuration=Release /property:Platform=x86 /property:SolutionDir="../../../../../"
msbuild "src/UI/Horsesoft.Music.Horsify.Importer.UI.WPF/Horsesoft.Music.Horsify.Importer.UI.WPF.csproj" /property:Configuration=Release /property:Platform=x86 /property:SolutionDir="../../../../../"

# Run the API build script (builds x86 and x64) (Copies to Build folder under Service)
cd src/services/horsesoft.music.horsify.api
./build.ps1
cd ../../..


# # BUILD X64 Installer
msbuild "src\Installer\Horsify2.Installer/Horsify2.Installer.wixproj" /property:Configuration=Release /property:Platform=x64 /property:SolutionDir="../../../../../"

# # # BUILD X86 Installer
msbuild "src\installer\horsify2.installer/horsify2.installer.wixproj" /property:configuration=release /property:platform=x86 /property:solutiondir="../../../../../"
