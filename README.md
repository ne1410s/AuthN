![dotnet build](https://github.com/ne1410s/AuthN/actions/workflows/dotnet.yml/badge.svg)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/1718b0d609a64562b4a6c0164f7aedec)](https://app.codacy.com/gh/ne1410s/AuthN?utm_source=github.com&utm_medium=referral&utm_content=ne1410s/AuthN&utm_campaign=Badge_Grade_Settings)
[![Coverage Status](https://coveralls.io/repos/github/ne1410s/AuthN/badge.svg?branch=master)](https://coveralls.io/github/ne1410s/AuthN?branch=master)

```powershell
# run this to pick up the accompanying tools
dotnet tool restore
```

## Test Coverage
```powershell
# obtain fresh coverlet test coverage results
gci **/TestResults/ | ri -r
dotnet test --settings coverlet.runsettings

# obtain fresh report(s) from coverlet coverage results
gci coveragerep* | ri -r
dotnet reportgenerator -targetdir:coveragereport -reports:**/coverage.info -reporttypes:"html;lcov" 
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
