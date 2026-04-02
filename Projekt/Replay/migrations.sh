rm -rf Migrations
rm -rf persistence/*

dotnet ef migrations add Initial --context ApplicationDbContext
dotnet ef migrations add Initial --context OperationDBContext
dotnet ef migrations add Initial --context ProcessDBContext
dotnet ef migrations add Initial --context SecurityDbContext

dotnet ef database update --context ApplicationDbContext
dotnet ef database update --context OperationDBContext
dotnet ef database update --context ProcessDBContext
dotnet ef database update --context SecurityDbContext