Start-Process -FilePath "powershell.exe" -WorkingDirectory "." -ArgumentList "dotnet", "run", "--project", ".\src\DatabaseService\DatabaseService.csproj";

Start-Sleep -Seconds 10;

Start-Process -FilePath "powershell.exe" -WorkingDirectory "." -ArgumentList "dotnet", "run", "--project", ".\src\ReadOnlyDataService\ReadOnlyDataService.csproj";
Start-Process -FilePath "powershell.exe" -WorkingDirectory "." -ArgumentList "dotnet", "run", "--project", ".\src\MutableDataService\MutableDataService.csproj";

Start-Sleep -Seconds 5;

Start-Process -FilePath "powershell.exe" -WorkingDirectory "." -ArgumentList "dotnet", "run", "--project", ".\src\AppUI\AppUI.csproj";

exit;