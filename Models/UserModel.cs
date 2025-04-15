using System;
using NJsonSchema.Infrastructure;

namespace TodoApi.Models;

public class UserModel
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public List<UserModel>? Followers { get; set; }

}
