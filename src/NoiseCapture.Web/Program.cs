using NoiseCapture.Web.Options;
using NoiseCapture.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.Configure<LocalDataOptions>(builder.Configuration.GetSection(LocalDataOptions.SectionName));
builder.Services.Configure<NoiseStorageOptions>(builder.Configuration.GetSection(NoiseStorageOptions.SectionName));
builder.Services.AddSingleton<INoiseLogStore, NoiseLogStore>();

var app = builder.Build();

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
