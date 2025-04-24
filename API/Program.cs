using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using Microsoft.OpenApi.Models;
using ProjetoSistemas.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDB>(opt =>
    opt.UseInMemoryDatabase("SocialDB"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Projeto Sistemas API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDB>();

    context.Users.AddRange(
        new UserModel(1,"Caue"),
        new UserModel(2,"CaueJibbers")
    );

    context.SaveChanges();
}


app.Run();

Console.WriteLine("http://localhost:5010/swagger/v1/swagger.json");
