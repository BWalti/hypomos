FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 1039
EXPOSE 44346

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["web/Web.csproj", "web/"]
RUN dotnet restore "web/Web.csproj"
COPY . .
WORKDIR "/src/web"
RUN dotnet build "Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.dll"]