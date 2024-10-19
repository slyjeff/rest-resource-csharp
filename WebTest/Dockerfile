FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebTest/WebTest.csproj", "WebTest/"]
COPY ["Slysoft.RestResource.AspNetCoreUtils/SlySoft.RestResource.AspNetCoreUtils.csproj", "Slysoft.RestResource.AspNetCoreUtils/"]
COPY ["SlySoft.RestResource.Html/SlySoft.RestResource.Html.csproj", "SlySoft.RestResource.Html/"]
COPY ["Slysoft.RestResource/SlySoft.RestResource.csproj", "Slysoft.RestResource/"]
COPY ["SlySoft.RestResource.Serializers/SlySoft.RestResource.Serializers.csproj", "SlySoft.RestResource.Serializers/"]
RUN dotnet restore "WebTest/WebTest.csproj"
COPY . .
WORKDIR "/src/WebTest"
RUN dotnet build "WebTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebTest.dll"]
