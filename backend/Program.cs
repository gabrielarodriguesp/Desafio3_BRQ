using PlaylistRecommenderAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://127.0.0.1:4200", "http://localhost:4200") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()); 
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


builder.Services.AddControllers();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlaylistService>(); 
builder.Services.AddScoped<MusicService>();
builder.Services.AddScoped<SearchHistoryService>();
builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ISearchHistoryService, SearchHistoryService>();
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseCors("AllowAngular");

app.UseAuthorization();
app.MapControllers();
app.Run();
