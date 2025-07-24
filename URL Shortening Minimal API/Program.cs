using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using URL_Shortening_Minimal_API.DataAccess.Data;
using URL_Shortening_Minimal_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<URLDbContext>(options => 
options.UseNpgsql(builder.Configuration["ConnectionStrings:Database"]));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapUrlEndpoints();

app.UseHttpsRedirection();




app.Run();


