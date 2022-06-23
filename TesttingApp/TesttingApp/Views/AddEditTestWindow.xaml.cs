using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TesttingApp.Models;

namespace TesttingApp.Views
{
    /// <summary>
    /// Interaction logic for AddEditTestWindow.xaml
    /// </summary>
    public partial class AddEditTestWindow : Window
    {
        public Test Test { get; set; }
        public bool IsEdit { get; }
        public int? TestId { get; }

        public List<Question> Questions = new List<Question>();

        public AddEditTestWindow(bool isEdit, int? testId = null)
        {
            InitializeComponent();
            IsEdit = isEdit;
            TestId = testId;
            if (isEdit)
            {
                Title = "Редактирование теста";
                LoadTests();
            }
            else
            {
                Title = "Добавление теста";
                Test = new Test();
            }
        }

        private async void LoadTests()
        {
            using var db = new TestingDatabaseContext();
            Test = db.Tests.Include(x => x.Questions).ThenInclude(x => x.Answers).First(x=> x.TestId == TestId);
            Questions = Test.Questions.ToList();
            TextBoxTestTitle.Text = Test.Title;
            ListViewQuestions.ItemsSource = new ObservableCollection<Question>(db.Questions.Include(x => x.Answers).Where(x => x.TestId == TestId));
        }

        private void ButtonDeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewQuestions.SelectedIndex != -1)
            {
                var question = ListViewQuestions.SelectedItem as Question;
                using var db = new TestingDatabaseContext();
                db.Questions.Find(question.QuestionId).IsDeleted = true;
                db.SaveChanges();
                if (IsEdit)
                {
                    LoadTests();
                }
            }
        }

        private void ButtonAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (IsEdit)
            {
                new AddEditQuestionWindow(true, ref Questions,testId:TestId).Show();
            }
            else
            {
                new AddEditQuestionWindow(false, ref Questions).Show();
                ListViewQuestions.ItemsSource = Questions;
            }
        }

        private void ButtonEditQuestion_Click(object sender, RoutedEventArgs e)
        {
            var question = ListViewQuestions.SelectedItem as Question;
            new AddEditQuestionWindow(true, ref Questions, question.QuestionId).Show();
            if (IsEdit)
            {
                LoadTests();
            }
            else
            {

            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            using var db = new TestingDatabaseContext();
            if (IsEdit)
            {
                var test = db.Tests.Find(TestId);
                test.Title = TextBoxTestTitle.Text;
                test.Questions = Questions;
                test.Questions = Test.Questions;
                db.SaveChanges();
                Close();
                LoadTests();
            }
            else
            {
                Test.Title = TextBoxTestTitle.Text;
                Test.Questions = Questions;
                db.Add(Test);
                db.SaveChanges();
            }
        }

        private void ListViewQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewQuestions.SelectedIndex != -1)
            {
                ButtonEditQuestion.IsEnabled = true;
                ButtonDeleteQuestion.IsEnabled = true;
            }
            else
            {
                ButtonEditQuestion.IsEnabled = false;
                ButtonDeleteQuestion.IsEnabled = false;
            }
        }
    }
}
