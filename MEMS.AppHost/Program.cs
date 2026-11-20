using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);
var elections = builder.AddSqlServer("sql").WithDataVolume().AddDatabase("elections");
var api = builder.AddProject<Projects.MEMS_Api>("api").WithReference(elections)
    .WaitFor(elections).WithHttpHealthCheck("/health");
var site = builder.AddDockerfile("public", "..", "MEMS.Public/Dockerfile")
    .WithHttpEndpoint(port: 5180, targetPort: 8080).WithHttpHealthCheck("/health");
var admin = builder.AddProject<Projects.MEMS_Admin>("admin").WithReference(api).WithReference(site.GetEndpoint("http"))
    .WaitFor(api).WaitFor(site).WithHttpHealthCheck("/health");
site.WithEnvironment("SITE_HTTPS_PORT", admin.GetEndpoint("https").Property(EndpointProperty.Port));
builder.Build().Run();
