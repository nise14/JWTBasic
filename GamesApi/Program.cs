using System.Security.Claims;

Dictionary<string, List<string>> gamesMap = new(){
    {"Player1",new List<string>{"Street Figter II", "Minecraft"}},
    {"Player2",new List<string>{"Forza Horizon 5", "Final Fnatasy XIV","FIA 23"}}
};

Dictionary<string, List<string>> subscriptionMap = new(){
    {"silver",new List<string>{"Street Figter II", "Minecraft"}},
    {"gold",new List<string>{"Forza Horizon 5", "Final Fnatasy XIV","FIA 23"}}
};

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/playergames", () => gamesMap)
    .RequireAuthorization(policy =>
    {
        policy.RequireRole("admin");
    });

app.MapGet("/mygames", (ClaimsPrincipal user) =>
{
    var hasClaim = user.HasClaim(claim => claim.Type == "subscription");

    if (hasClaim)
    {
        var subscription = user.FindFirstValue("subscription") ?? throw new Exception("Claim has no value");
        return Results.Ok(subscriptionMap[subscription]);
    }

    ArgumentNullException.ThrowIfNull(user.Identity?.Name);
    var userName = user.Identity.Name;

    if (!gamesMap.ContainsKey(userName))
    {
        return Results.Empty;
    }

    return Results.Ok(gamesMap[userName]);
}).RequireAuthorization(policy =>
    {
        policy.RequireRole("player");
    });

app.Run();
