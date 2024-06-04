using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Profunion;
using Profunion.Data;
using Profunion.Interfaces.EventInterface;
using Profunion.Interfaces.UserInterface;
using Profunion.Interfaces.CategoryInterface;
using Profunion.Services.EventServices;
using Profunion.Services.UserServices;
using Profunion.Services.CategoryServices;
using Profunion.Services.BookingServices;
using System.Text;
using Profunion.Interfaces.ApplicationInterface;
using Profunion.Interfaces;
using Profunion.Services.AdditionalServices;
using System.IdentityModel.Tokens.Jwt;
using Profunion.Services.MailServices;
using SendGrid;
using System.Configuration;
using Profunion.Interfaces.FileInterface;
using Profunion.Services.FileServices;
using Microsoft.OpenApi.Models;
using Profunion.Interfaces.NewsInterface;
using Profunion.Services.NewsService;
using Profunion.Interfaces.ReportInterface;
using Profunion.Services.ReportService;

var builder = WebApplication.CreateBuilder(args);

/*builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps(); 
    });
});*/

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddTransient<Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<Helpers>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IRejectedApplicationRepository, RejectedApplicationRepository>();
builder.Services.AddScoped<IReservationList, ReservationListRepository>();
builder.Services.Configure<SendGridClientOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<HashingPassword>();
builder.Services.AddScoped<GenerateMultipleJWT>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy
                .WithOrigins("https://localhost:3000","http://localhost:3000", "http://profunions.ru", "https://profunions.ru")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); 
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var tokenSettings = builder.Configuration.GetSection("JwtOptions");
var secretKey = Encoding.ASCII.GetBytes(tokenSettings.GetValue<string>("SecretKey"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true, // Валидация времени жизни токена
        };
        options.SaveToken = true;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["accessToken"];
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProfUnion V1");
    });
}

app.UseCors("AllowSpecificOrigins");

app.UseWebSockets();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
