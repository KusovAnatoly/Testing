using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesttingApp.Models
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        public int QuestionId { get; set; }
        public int TestId { get; set; }
        public string Text { get; set; } = null!;
        public byte[]? Image { get; set; }

        public virtual Test Test { get; set; } = null!;
        public virtual ICollection<Answer> Answers { get; set; }

        [NotMapped]
        public ObservableCollection<Answer> SpecialAnswers
        {
            get => new ObservableCollection<Answer>(Answers);
            set
            {

            }
        }
    }
}
