# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file(s) and restore
COPY *.csproj ./
RUN dotnet restore

# Copy source code and publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish/ ./

# Copy the database from project root into the container
COPY expense.db ./expense.db

# Expose port
EXPOSE 80

# Run the app
ENTRYPOINT ["sh", "-c", "export ASPNETCORE_URLS=http://+:${PORT:-5000} && \
    SQLITE_PATH=${SQLITE_PATH:-/app/expense.db} && \
    echo \"Starting app on $ASPNETCORE_URLS with DB at $SQLITE_PATH\" && \
    dotnet ExpenseTracker.dll"]
