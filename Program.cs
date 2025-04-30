//using CitiesManager.Core.Identity;

//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNetCore.Mvc;


//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers(options => {
//    options.Filters.Add(new ProducesAttribute("application/json"));
//    options.Filters.Add(new ConsumesAttribute("application/json"));
//})
// .AddXmlSerializerFormatters();


////Enable versioning in Web API controllers


//builder.Services.AddDbContext<Login.Models.UserContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
//});


////Swagger
//builder.Services.AddEndpointsApiExplorer(); //Generates description for all endpoints


//builder.Services.AddSwaggerGen(options =>
//{
//    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));

//    //options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "1.0" });

//    //options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "2.0" });

//}); //generates OpenAPI specification


////builder.Services.AddVersionedApiExplorer(options =>
////{
////    options.GroupNameFormat = "'v'VVV"; //v1
////    options.SubstituteApiVersionInUrl = true;
////});

////CORS: localhost:4200, localhost:4100
//builder.Services.AddCors(options => {
//    options.AddDefaultPolicy(policyBuilder =>
//    {
//        policyBuilder
//        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
//        .WithHeaders("Authorization", "origin", "accept", "content-type")
//        .WithMethods("GET", "POST", "PUT", "DELETE")
//        ;
//    });

//    options.AddPolicy("4100Client", policyBuilder =>
//    {
//        policyBuilder
//        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins2").Get<string[]>())
//        .WithHeaders("Authorization", "origin", "accept")
//        .WithMethods("GET")
//        ;
//    });
//});


////
// ;


//var app = builder.Build();

//// Configure the HTTP request pipeline.

//app.UseHsts();
//app.UseHttpsRedirection();

//app.UseSwagger(); //creates endpoint for swagger.json
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
//    options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
//}); //creates swagger UI for testing all Web API endpoints / action methods
//app.UseRouting();
//app.UseCors();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Login.Models; // Add this line for accessing JwtService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options => {
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
})
.AddXmlSerializerFormatters();

// Enable versioning in Web API controllers
builder.Services.AddDbContext<Login.Models.UserContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Register JwtService
//builder.Services.AddScoped<JwtService>(); // Add this line

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Uncomment and adjust if you want to include XML comments for your API documentation
    // options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
    // options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "1.0" });
    // options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "2.0" });
});

// CORS configuration
//builder.Services.AddCors(options => {
//    options.AddDefaultPolicy(policyBuilder =>
//    {
//        policyBuilder
//            .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
//            .WithHeaders("Authorization", "origin", "accept", "content-type")
//            .WithMethods("GET", "POST", "PUT", "DELETE");
//    });

//    options.AddPolicy("4100Client", policyBuilder =>
//    {
//        policyBuilder
//            .WithOrigins(builder.Configuration.GetSection("AllowedOrigins2").Get<string[]>())
//            .WithHeaders("Authorization", "origin", "accept")
//            .WithMethods("GET");
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("https://pizzaorderclient-b2ddeaduhfcvereu.canadacentral-01.azurewebsites.net")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


//Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero,
    };
});

//Configure Google Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
});

app.UseRouting();
app.UseCors("AllowAngularClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
