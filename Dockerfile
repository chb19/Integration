FROM mcr.microsoft.com/dotnet/framework/runtime:4.8-windowsservercore-ltsc2019

WORKDIR /DesctopApp

EXPOSE 80

COPY E:/DOCUMENTS/SEM_10/Integr/IntegrationК/Integration/Integration/DesctopApp/bin/Debug/net5.0-windows .

ENTRYPOINT [“DesctopApp.exe”]
