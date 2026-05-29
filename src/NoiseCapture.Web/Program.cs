using Microsoft.EntityFrameworkCore;
using NoiseCapture.Web.Data;
using NoiseCapture.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("NoiseCaptureDatabase")
    ?? throw new InvalidOperationException("Connection string 'NoiseCaptureDatabase' was not found.");

builder.Services.AddRazorPages();
builder.Services.AddDbContext<NoiseCaptureDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<INoiseLogStore, NoiseLogStore>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NoiseCaptureDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
