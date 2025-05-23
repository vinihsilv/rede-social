using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoSistemas.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public virtual ICollection<UserFollowing> Followers { get; set; } = new List<UserFollowing>();
        public virtual ICollection<UserFollowing> Following { get; set; } = new List<UserFollowing>();

        public UserModel() {}
        public UserModel(string username, int followersCount, int followingCount)
        {
            Username = username;
        }
    }

    public class UserFollowing
    {
        public int Id { get; set; }

        public int FollowerId { get; set; }
        public UserModel Follower { get; set; } = null!;

        public int FollowingId { get; set; }
        public UserModel Following { get; set; } = null!;
    }
}
