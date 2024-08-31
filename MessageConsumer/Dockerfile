# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Set up environment variables for the Azure DevOps feed
ARG FEED_URL="https://pkgs.dev.azure.com/email2wajidkhan/_packaging/email2wajidkhan/nuget/v3/index.json"
ARG PAT_TOKEN

# Copy the solution and project files

COPY ["MessageConsumer.sln", "."] 
COPY ["MessageConsumer/MessageConsumer.csproj", "MessageConsumer/"]

# Create a NuGet configuration file
RUN dotnet nuget add source $FEED_URL -n AzureDevOps -u email2wajidkhan --store-password-in-clear-text -p $PAT_TOKEN

# Restore dependencies
RUN dotnet restore

# Copy the remaining source files
COPY . .

# Build the application
WORKDIR "/src/MessageConsumer"
RUN dotnet build -c Release -o /app/build

# Stage 2: Run tests
FROM build AS testrunner
WORKDIR /src

# Execute tests
# RUN dotnet test --no-build "Tests/GraphQl.Core.Test/GraphQl.Core.Test.csproj" --verbosity normal 

# Stage 3: Publish the application
FROM build AS publish
WORKDIR "/src/MessageConsumer"
RUN dotnet publish -c Release -o /app/publish

# Stage 4: Final stage - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

EXPOSE 9090

# Set the entry point for the application
ENTRYPOINT ["dotnet", "MessageConsumer.dll"]
