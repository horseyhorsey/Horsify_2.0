
# BUILD X64 Jukebox
msbuild "src/UI/Horsesoft.Music.Horsify.WPF.Shell/Horsesoft.Music.Horsify.WPF.Shell.csproj" /property:Configuration=Release /property:Platform=x64 /property:SolutionDir="../../../../../"

# # BUILD X86 Jukebox
msbuild "src/UI/Horsesoft.Music.Horsify.WPF.Shell/Horsesoft.Music.Horsify.WPF.Shell.csproj" /property:Configuration=Release /property:Platform=x86 /property:SolutionDir="../../../../../"

# # BUILD X64 Song Importer
msbuild "src/UI/Horsesoft.Music.Horsify.Importer.UI.WPF/Horsesoft.Music.Horsify.Importer.UI.WPF.csproj" /property:Configuration=Release /property:Platform=x64 /property:SolutionDir="../../../../../"

# # BUILD X86 Song Importer
msbuild "src/UI/Horsesoft.Music.Horsify.Importer.UI.WPF/Horsesoft.Music.Horsify.Importer.UI.WPF.csproj" /property:Configuration=Release /property:Platform=x86 /property:SolutionDir="../../../../../"

# Run the API build script (builds x86 and x64) (Copies to Build folder under Service)
cd src/Services/Horsesoft.Music.Horsify.Api
./build.ps1



