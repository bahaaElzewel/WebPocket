using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using WebPocket;
using WebPocket.Data;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WebPocketDbContext>(Options => {
    Options.UseSqlServer(builder.Configuration.GetConnectionString("WebPocket"));
});

builder.Services.AddDbContext<WebPocketDbContext>((serviceProvider, dbContextBuilder) => {
    var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    if (httpContext.HttpContext is not null)
    {
        var context = serviceProvider.GetRequiredService<WebPocketDbContext>();
        
    }
});

builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddControllers().AddDataAnnotationsLocalization(options => {
    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(JsonStringLocalizerFactory));
});

builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("x-version")
    );
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
