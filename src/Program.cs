var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Extensions["foo"] = "baar";
        context.ProblemDetails.Extensions["traceId"] = Guid.NewGuid().ToString();
    };
});

var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI();

app.MapGet("/{value:int}", (ILoggerFactory factory, int value) =>
{
    var logger = factory.CreateLogger("Endpoint");

    logger.LogInformation("[GET]: '{Value}", value);

    return Results.Ok(new
    {
        value
    });
}).WithOpenApi();

app.MapGet("/problem", () =>
{
    return Results.Problem(
        title: "Problem",
        detail: "This is a problem",
        statusCode: 400,
        extensions: new Dictionary<string, object>
        {
            ["foo"] = "bar",
            ["userID"] = 124,

        });
}).WithOpenApi();

await app.RunAsync();
