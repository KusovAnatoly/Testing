using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TesttingApp.Models;

namespace TesttingApp.Views
{
    /// <summary>
    /// Interaction logic for AddEditTestWindow.xaml
    /// </summary>
    public partial class AddEditTestWindow : Window
    {
        public static List<Question> Questions = new List<Question>();
        public int TestId { get; }
        public Test Test { get; set; }

        public AddEditTestWindow(int testId)
        {
            InitializeComponent();
            Title = "Редактирование теста";
            TestId = testId;
            LoadData();
        }

        public AddEditTestWindow()
        {
            InitializeComponent();
            Test = new Test();
        }

        private void LoadData()
        {

            using (var db = new TestingDatabaseContext())
            {

                ListViewQuestions.ItemsSource = db.Questions.Include(x => x.Answers).Where(x => x.TestId == TestId).ToList();
            }
        }

        private void ButtonDeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewQuestions.SelectedIndex != -1)
            {
                var question = ListViewQuestions.SelectedItem as Question;

            }
        }

        private void ButtonAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            new AddEditQuestionWindow(TestId).Show();
            LoadData();
            Test.Questions = Questions;
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Test.Title = TextBoxTestName.Text;
            using (var db = new TestingDatabaseContext())
            {
                db.Add(Test);
                db.SaveChanges();
                Close();
            }
        }
    }
}
