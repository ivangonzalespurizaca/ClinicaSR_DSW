using ClinicaSR.BL.BC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<CitaBC>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

//app.MapGet("/", () => "Hello World!");

app.Run();
