FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /src

COPY ./Expenses.slnx ./
COPY ./NuGet.Config ./

COPY ./Expenses.Api ./Expenses.Api
COPY ./Expenses.Application ./Expenses.Application
COPY ./Expenses.Domain ./Expenses.Domain
COPY ./Expenses.Infrastructure ./Expenses.Infrastructure
COPY ./tests ./tests

RUN dotnet restore ./Expenses.slnx

RUN dotnet build ./Expenses.slnx -c Release

RUN dotnet publish ./Expenses.Api/Expenses.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Expenses.Api.dll"]
