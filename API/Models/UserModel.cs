public class UserModel
{
    [Key]
    public int UserId { get; set; }
    public string Username { get; set; }

    public virtual ICollection<UserFollowing> Followers { get; set; }
    public virtual ICollection<UserFollowing> Following { get; set; }
}

public class UserFollowing
{
    public int Id { get; set; }

    public int FollowerId { get; set; }
    public UserModel Follower { get; set; }

    public int FollowingId { get; set; }
    public UserModel Following { get; set; }
}
