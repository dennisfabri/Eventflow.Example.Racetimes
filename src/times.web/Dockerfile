FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["times.web/times.web.csproj", "times.web/"]
RUN dotnet restore "times.web/times.web.csproj"
COPY . .
WORKDIR "/src/times.web"
RUN dotnet build "times.web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "times.web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "times.web.dll"]
