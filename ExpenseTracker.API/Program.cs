using ExpenseTracker.API.Database;
using ExpenseTracker.API.Services;
using ExpenseTracker.API.Repositories;

const string CORS_POLICY = "ExpenseApiPolicy";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddDatabase();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddCors(c =>
    c.AddPolicy(CORS_POLICY, policy =>
        policy.AllowAnyHeader()
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .WithOrigins(builder.Configuration.GetValue<string>("Frontend") ?? throw new InvalidOperationException("Frontend url was not provided"))
));

var app = builder.Build();
app.UseCors(CORS_POLICY);
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();
