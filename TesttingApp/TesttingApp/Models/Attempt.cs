using System;
using System.Collections.Generic;

namespace TesttingApp.Models
{
    public partial class Attempt
    {
        public int AttemptId { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int Result { get; set; }

        public virtual Test Test { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
