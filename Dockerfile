FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HygroclipBlazorServer/HygroclipBlazorServer.csproj", "HygroclipBlazorServer/"]
COPY ["HygroclipDriver/HygroclipDriver.csproj", "HygroclipDriver/"]
COPY ["PicoController/PicoController.csproj", "PicoController/"]
RUN dotnet restore "HygroclipBlazorServer/HygroclipBlazorServer.csproj"
COPY . .
WORKDIR "/src/HygroclipBlazorServer"
RUN dotnet build "HygroclipBlazorServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HygroclipBlazorServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HygroclipBlazorServer.dll"]