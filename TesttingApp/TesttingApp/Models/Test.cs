using System;
using System.Collections.Generic;

namespace TesttingApp.Models
{
    public partial class Test
    {
        public Test()
        {
            Attempts = new HashSet<Attempt>();
            Questions = new HashSet<Question>();
        }

        public int TestId { get; set; }
        public string Title { get; set; } = null!;
        public bool IsDeleted { get; set; }

        public virtual ICollection<Attempt> Attempts { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
