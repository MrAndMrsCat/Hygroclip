FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HomeAutomationServer/HomeAutomationServer.csproj", "HomeAutomationServer/"]
COPY ["HygroclipDriver/HygroclipDriver.csproj", "HygroclipDriver/"]
COPY ["PicoController/PicoController.csproj", "PicoController/"]
RUN dotnet restore "HomeAutomationServer/HomeAutomationServer.csproj"
COPY . .
WORKDIR "/src/HomeAutomationServer"
RUN dotnet publish "HomeAutomationServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HomeAutomationServer.dll"]