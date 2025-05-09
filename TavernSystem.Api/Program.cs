using Application;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IAdventurerRepository, AdventurerRepository>();
builder.Services.AddTransient<IPersonDataRepository, PersonDataRepository>();
builder.Services.AddTransient<IRaceRepository, RaceRepository>();
builder.Services.AddTransient<IExperienceLevelRepository, ExperienceLevelRepository>();
builder.Services.AddTransient<IAdventurerService, AdventurerService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();