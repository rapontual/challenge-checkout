FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Challenge.API/Challenge.API.csproj", "Challenge.API/"]
RUN dotnet restore "Challenge.API/Challenge.API.csproj"
COPY . .

COPY ./wait-for-it.sh /wait-for-it.sh
RUN chmod +x wait-for-it.sh

WORKDIR "/src/Challenge.API"
RUN dotnet build "Challenge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Challenge.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Challenge.API.dll"]