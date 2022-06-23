using System;
using System.Collections.Generic;

namespace TesttingApp.Models
{
    public partial class User
    {
        public User()
        {
            Attempts = new HashSet<Attempt>();
        }

        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<Attempt> Attempts { get; set; }
    }
}
