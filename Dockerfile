FROM mcr.microsoft.com/dotnet/core/runtime:2.2
# FROM mcr.microsoft.com/dotnet/core/runtime:2.2.5-alpine3.9

WORKDIR /app
COPY .publish/W /app

ENTRYPOINT ["dotnet", "MyApi.dll"]