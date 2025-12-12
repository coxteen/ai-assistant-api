var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Comment out the check so it runs in Azure (Production)
// if (app.Environment.IsDevelopment()) 
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Note: The lab manual implies adding Swagger to production or outside the IsDevelopment block 
// if you want to see it on Azure later, but follow the specific lines in the PDF carefully.
// The manual specifically shows app.UseSwagger() and UI usually inside Development check, 
// but implies we need it accessible.

app.UseAuthorization();

app.MapControllers();

app.Run();