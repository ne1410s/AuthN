![dotnet build](https://github.com/ne1410s/AuthN/actions/workflows/dotnet.yml/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/ne1410s/AuthN/badge.svg?branch=master)](https://coveralls.io/github/ne1410s/AuthN?branch=master)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/89c4bbbbcf4b405a93d2e44bb0c40f10)](https://www.codacy.com/gh/ne1410s/AuthN/dashboard)

```powershell
# run this to pick up the accompanying tools
dotnet tool restore
```

## Test Coverage
```powershell
# obtain fresh coverlet test coverage results
gci **/TestResults/ | ri -r; gci *-test.db -r | ri; dotnet test --settings coverlet.runsettings

# obtain fresh report(s) from coverlet coverage results
gci coveragerep* | ri -r; dotnet reportgenerator -targetdir:coveragereport -reports:**/coverage.cobertura.xml -reporttypes:"html" 
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
