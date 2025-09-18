using Duende.IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddTestUsers(new List<Duende.IdentityServer.Test.TestUser>
    {
        new Duende.IdentityServer.Test.TestUser
        {
             IsActive = true,
             Password="123456",
             Username="aliahmadi",
             SubjectId="1"
        }
    })
    .AddInMemoryClients(new List<Client> { 
        new Client
        {
            ClientName="frondend Web",
            ClientId="webfrontend",
            ClientSecrets={new Secret("123456".Sha256())},
            AllowedGrantTypes=GrantTypes.ClientCredentials,
            AllowedScopes={ "orderservice.Fullaccess" }
        },
        new Client
        {
            ClientName="Web Frond code",
            ClientId="webfrontendcode",
            ClientSecrets={new Secret("123456".Sha256())},
            AllowedGrantTypes=GrantTypes.Code,
            RedirectUris={"https://localhost:44327/signin-oidc"},
            PostLogoutRedirectUris={ "https://localhost:44327/signout-oidc" },
            AllowedScopes={ "openid", "profile" }
        }
    })
    .AddInMemoryIdentityResources(new List<IdentityResource> {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    })
    .AddInMemoryApiScopes(new List<ApiScope>
    {
        new ApiScope("orderservice.Fullaccess")
    })
    .AddInMemoryApiResources(new List<ApiResource>
    {
        new ApiResource("orderservice","Order Service Api")
        {
            Scopes={ "orderservice.Fullaccess" }
        }
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
