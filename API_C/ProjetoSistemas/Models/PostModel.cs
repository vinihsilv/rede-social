using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoSistemas.Models
{
    public class PostModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }  

        public UserModel User { get; set; } = null!;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public PostModel() {}

        public PostModel(UserModel user, DateTime createdAt, string content)
        {
            User = user;
            UserId = user.UserId;
            CreatedAt = createdAt;
            Content = content;
        }
    }
}
