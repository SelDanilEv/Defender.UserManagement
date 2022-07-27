FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get install -f

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# RUN apt-get update
# RUN apt-get install -y curl
# RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
# RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
# RUN apt-get install -y nodejs

WORKDIR /src

COPY ["src/WebUI/WebUI.csproj", "WebUI/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/WebUI/WebUI.csproj", "WebUI/"]

RUN dotnet restore "src/WebUI/WebUI.csproj"
COPY . .
WORKDIR "/src/WebUI"
RUN dotnet build "WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT  ["dotnet", "WebUI.dll"]
#CMD ["dotnet", "WebUI.dll"]

CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebUI.dll

