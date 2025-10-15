using RecipeService.Api.Apis;
using RecipeService.Api.Extensions;
using RecipeService.Core.Extensions;
using RecipeService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container using extension methods
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCore();
builder.Services.AddApi();

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.AddRecipeApi();

app.Run();