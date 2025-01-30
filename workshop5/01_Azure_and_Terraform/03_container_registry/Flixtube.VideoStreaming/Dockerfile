FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /src
EXPOSE 80
COPY ./Flixtube.VideoStreaming ./
COPY ./videos ./videos
RUN ["dotnet","restore"]
CMD ["dotnet","watch","run","--no-launch-profile","--project","Flixtube.VideoStreaming.csproj"]