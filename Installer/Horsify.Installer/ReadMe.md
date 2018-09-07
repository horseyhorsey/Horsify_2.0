# Daves Wix install notes

## The windows service config:
			
	  <!--WINDOWS SERVICE-->			
      <Component Id="Horsesoft.Music.Horsify.Service.exe" Guid="de8dc4e2-878d-4a40-a8d3-eb7d5f62579c">
        <File Id="Horsesoft.Music.Horsify.Service.exe" Name="Horsesoft.Music.Horsify.Service.exe" Source="$(var.Horsesoft.Music.Horsify.Service_TargetDir)Horsesoft.Music.Horsify.Service.exe" DiskId="1" KeyPath="yes" />
        <ServiceInstall Id="HorsifyService" Type="ownProcess" Name="HorsifyService" DisplayName="Horsify Song Service" Description="Access the database for Horsify" Start="auto" Account="LocalSystem" ErrorControl="normal">
        </ServiceInstall>
        <ServiceControl Id="HorsifyService" Start="install" Stop="both" Remove="both" Name="HorsifyService" Wait="no" />
      </Component>

## Manually include the .net standard files?

	Horsesoft.Music.Data.Sqlite.dll
	x86 & x64 folders


	  <!--Horsesoft.Music.Data.Sqlite.dll-->
      <Component Id="Horsesoft.Music.Data.Sqlite.dll" Guid="cfd8257f-b3ef-4bcb-b130-20365541918d">
        <File Id="Horsesoft.Music.Data.Sqlite.dll" Name="Horsesoft.Music.Data.Sqlite.dll" 
              Source="$(var.Horsesoft.Music.Horsify.Service_TargetDir)Horsesoft.Music.Data.Sqlite.dll" />
      </Component>  


## SWLITE 3 dll

      <!--SQLLITE3 Dll-->
      <Component Id="e_sqlite3.dll" Guid="cfd8257f-b3ef-4bcb-b130-20dd5541918d">
        <File Id="e_sqlite3.dll" Name="e_sqlite3.dll"
              Source="$(var.Horsesoft.Music.Horsify.Importer.UI.WPF_TargetDir)/x86/e_sqlite3.dll" />
      </Component>