using judo_backend.Services.Interfaces;
using judo_backend.Services;
using judo_backend.Helpers;
using judo_backend.Attributes;
using judo_backend.Helpers.HealtCheck;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, true);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionAttribute>();
});

builder.Services
    .AddHealthChecks()
    .AddCheck<ApiHealthCheck>(nameof(ApiHealthCheck))
    .AddCheck<DbHealthCheck>(nameof(DbHealthCheck))
    .AddCheck<BlobHealthCheck>(nameof(BlobHealthCheck));

builder.Services.AddCors(options =>
{
    // this defines a CORS policy called "default"
    options.AddPolicy("default", policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetSection("CoreOrigin").Value)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILog, Log>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INewService, NewService>();
builder.Services.AddScoped<IAgendaService, AgendaService>();
builder.Services.AddScoped<ICompetitionService, CompetitionService>();
builder.Services.AddScoped<IStageService, StageService>();
builder.Services.AddScoped<IEjtPersonService, EjtPersonService>();
builder.Services.AddScoped<IEjtAdherentService, EjtAdherentService>();
builder.Services.AddScoped<IClotheService, ClotheService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBlob, Blob>();
builder.Services.AddScoped<IExcelGeneratorService, ExcelGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.UseCors("default");

app.Run();
