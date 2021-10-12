## Test Coverage
```powershell
# obtain fresh coverlet test coverage results
gci **/TestResults/ | ri -r
dotnet test --settings coverlet.runsettings

# obtain fresh report(s) from coverlet coverage results
gci coveragerep* | ri -r
dotnet reportgenerator -targetdir:coveragereport -reports:**/coverage.cobertura.xml -reporttypes:"html" 
```

## Entity Framework
```powershell
# add a new migration
dotnet ef migrations add <MigrationName> -p AuthN.Persistence -s AuthN.Api

# obtains a script for production
$env:ASPNETCORE_ENVIRONMENT="Production";dotnet ef dbcontext script <PrevMigration> -p AuthN.Persistence -s AuthN.Api
```

## IaC
```bash
# create resource group
az group create -l uksouth -n ??? --tags ???

# deploy cloud infrastructure
az deployment group create -g ??? -f ??? -p ???
```
