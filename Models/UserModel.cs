using System;
using System.ComponentModel.DataAnnotations;
using NJsonSchema.Infrastructure;

namespace TodoApi.Models;

public class UserModel
{
    [Key]
    public int UserId { get; set; }
    public string Username { get; set; }
    public List<UserModel>? Followers { get; set; }
    public List<UserModel>? Following { get; set; }

    public UserModel()
    {
        Following = new List<UserModel>();
        Followers = new List<UserModel>();
    }


}
