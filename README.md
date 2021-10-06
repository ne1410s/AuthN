## Test Coverage
```powershell
# obtain coverlet test coverage results
dotnet test --settings coverlet.runsettings

# generate html summary report from coverlet coverage results
# PREREQ> dotnet tool install -g dotnet-reportgenerator-globaltool
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
