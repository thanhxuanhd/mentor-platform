var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

builder.AddProject<Projects.MentorPlatformAPI>("api")
    .WithReference(db, "DefaultConnection")
    .WaitFor(db);

builder.Build().Run();