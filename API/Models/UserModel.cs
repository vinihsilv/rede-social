using System;
using System.ComponentModel.DataAnnotations;
using NJsonSchema.Infrastructure;

namespace ProjetoSistemas.Models;

public class UserModel
{
    [Key]
    public int UserId { get; set; }
    public string Username { get; set; }
    public List<int>? Followers { get; set; }
    public List<int>? Following { get; set; }

    public UserModel()
    {
        Followers = new List<int>();
        Following = new List<int>();
    }

    public UserModel(string username)
    {
        Followers = new List<int>();
        Following = new List<int>();
        Username = username;
    }

    //Retirar esse método antes de entregar, apenas adicionado para testar api mais facilmente.
    public UserModel(string username, int follower, int follow)
    {
        Followers = new List<int>();
        Following = new List<int>();
        
        Username = username;
        Followers.Add(follower);
        Following.Add(follow);


    }
}
