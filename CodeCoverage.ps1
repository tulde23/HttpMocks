#execute code coverage for Gozer.Gatekeeper.UnitTests
dotnet test '.\tests\UnitTests\UnitTests.csproj' /p:CollectCoverage=true /p:CoverletOutput='../../results/' /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit*]*%2c*"
reportgenerator "-reports:./results/coverage.cobertura.xml" "-targetdir:./reports"
Push-Location ".\reports"

Invoke-Item '.\index.htm'
Pop-Location