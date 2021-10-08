## Test Coverage
```powershell
# obtain fresh coverlet test coverage results
gci **/TestResults/ | ri -r
dotnet test --settings coverlet.runsettings

# obtain fresh report(s) from coverlet coverage results
# PREREQ> dotnet tool install -g dotnet-reportgenerator-globaltool
gci coveragerep* | ri -r
reportgenerator -targetdir:coveragereport -reports:**/coverage.cobertura.xml -reporttypes:"html;htmlsummary" 
```

## Entity Framework
```powershell
# add a new migration
# PREREQ> dotnet tool install -g dotnet-ef
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
