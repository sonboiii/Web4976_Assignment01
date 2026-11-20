FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props .
COPY MEMS.Shared/ MEMS.Shared/
COPY MEMS.Public/ MEMS.Public/
RUN dotnet publish MEMS.Public -c Release -o /out -p:WasmBuildNative=false

FROM nginx:stable-alpine
COPY --from=build /out/wwwroot /usr/share/nginx/html
COPY MEMS.Public/nginx.conf.template /etc/nginx/templates/default.conf.template
EXPOSE 8080
