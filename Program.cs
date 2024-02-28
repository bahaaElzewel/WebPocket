using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using WebPocket;
using WebPocket.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WebPocketDbContext>(Options => {
    Options.UseSqlServer(builder.Configuration.GetConnectionString("WebPocket"));
});

builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddControllers().AddDataAnnotationsLocalization(options => {
    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(JsonStringLocalizerFactory));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var supportedCultures = new [] {"en-US", "ar-EG"};
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

app.MapControllers();

app.Run();
