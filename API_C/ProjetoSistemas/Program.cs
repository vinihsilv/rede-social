using Microsoft.EntityFrameworkCore;
using ProjetoSistemas.Data;
using Microsoft.OpenApi.Models;
using ProjetoSistemas.Models;

var builder = WebApplication.CreateBuilder(args);

// Serviços
builder.Services.AddDbContext<UserDB>(opt =>
    opt.UseInMemoryDatabase("SocialDB"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Projeto Sistemas API", Version = "v1" });
});

var app = builder.Build();

app.Urls.Add("http://0.0.0.0:5010");

// Middlewares
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projeto Sistemas API v1");
    c.RoutePrefix = "swagger";
});

// Redireciona raiz "/" para "/swagger"
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Dados iniciais
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
