using System;
using Microsoft.EntityFrameworkCore;
using ProjetoSistemas.Models;
using TodoApi.Models;

namespace TodoApi.Data;

public class UserDB : DbContext
{
    public DbSet<UserModel> Users{ get; set; }
    public DbSet<PostModel> Posts{ get; set; }
    public UserDB(DbContextOptions<UserDB> options): base(options){

    }

}
