using System;
using System.ComponentModel.DataAnnotations;
using NJsonSchema.Infrastructure;

namespace ProjetoSistemas.Models;

public class UserModel
{
    [Key]
    public int UserId { get; set; }
    public string Username { get; set; }
    public List<UserModel>? Followers { get; set; }

    public UserModel()
    {
        Followers = new List<UserModel>();
    }

    

}
