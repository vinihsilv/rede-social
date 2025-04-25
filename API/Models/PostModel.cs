using System;
using System.ComponentModel.DataAnnotations;
using NJsonSchema.Infrastructure;
using ProjetoSistemas.Models;

namespace TodoApi.Models

{
    public class PostModel
    {
        [Key]
        public int IdPost { get; set; }
        public UserModel User { get; set; }
        public DateTime PostTime { get; set; }
        public string PostText { get; set; }

        public PostModel()
        {
        }

        public PostModel(UserModel user, DateTime postTime, string postText)
        {
            User = user;
            PostTime = postTime;
            PostText = postText;
        }
    }
}
