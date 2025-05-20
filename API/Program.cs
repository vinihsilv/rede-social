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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projeto Sistemas API v1");
        c.RoutePrefix = "swagger";
    });
}

// 🔧 Essa linha é importante para acesso externo via Docker
app.Urls.Add("http://0.0.0.0:5010");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDB>();
    context.Users.AddRange(
        new UserModel("Caue", 2, 2),
        new UserModel("CaueJibbers", 1, 1)
    );
    context.SaveChanges();
}

app.Run();
