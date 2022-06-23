using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TesttingApp.Models;

namespace TesttingApp.Views
{

    public class UserAnswer
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public bool IsCorrect { get; set; }
    }

    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public static RoutedCommand MyCommand = new RoutedCommand();

        public Test SelectedTest { get; set; }
        public ObservableCollection<Test> Tests { get; set; }
        public ObservableCollection<Question> Questions { get; set; }
        public Question Question { get; set; }
        public ObservableCollection<Answer> Answers { get; set; }
        public ObservableCollection<UserAnswer> UserAnswers { get; set; }

        private int _questionNumber;
        private bool _isNavigationButtonClicked;

        public TestWindow()
        {
            InitializeComponent();
            MyCommand.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            LoadData();
        }

        private async void LoadData()
        {
            using (var db = new TestingDatabaseContext())
            {
                Tests = new ObservableCollection<Test>(await db.Tests
                    .Where(x => x.IsDeleted != true)
                    .Include(x => x.Questions)
                    .ThenInclude(x => x.Answers)
                    .Include(x => x.Attempts)
                    .ToListAsync());
                ListViewTests.ItemsSource = Tests;
            }
        }

        private void ListViewTests_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedTest = null!;
            Questions = null!;
            Question = null!;
            _questionNumber = 0;
            UncheckAllControls();
            UserAnswers = new();
            GridTest.Visibility = Visibility.Collapsed;
            GridTestField.Visibility = Visibility.Collapsed;
            StackPanelFields.IsEnabled = true;
            TextBoxEmail.Text = String.Empty;
            TextBoxName.Text = String.Empty;
            if (ListViewTests.SelectedItem != null && ListViewTests.SelectedIndex >= 0)
            {
                EditTestMenuItem.Visibility = Visibility.Visible;
                DeleteTestMenuItme.Visibility = Visibility.Visible;
                SelectedTest = ListViewTests.SelectedItem as Test;
                Questions = new(SelectedTest.Questions);
                SetupQuestion();

                GridTest.Visibility = Visibility.Visible;
                StackPanelFields.Visibility = Visibility.Visible;
                _questionNumber = 0;
                UserAnswers = new();
            }
            else
            {
                DeleteTestMenuItme.Visibility = Visibility.Collapsed;
                EditTestMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        private void SetupQuestion()
        {
            Question = Questions[_questionNumber];
            Answers = new(Question.Answers);
            TextBlockQuestionTitle.Text = Question.Text;
            try
            {
                TestImage.Source = (BitmapSource)new ImageSourceConverter().ConvertFrom(Question.Image);
            }
            catch (Exception) { }
            if (Question.Answers.Count(x => x.IsCorrect == true) > 1)
            {
                SetupManyAnswers();
            }
            else
            {
                SetupOneAnswer();
            }
        }

        private void SetupOneAnswer()
        {
            UniformGridOneCorrect.Visibility = Visibility.Visible;
            UniformGridManyCorrect.Visibility = Visibility.Collapsed;

            RadioButtonFirstAnswer.Content = Answers[0].Text;
            RadioButtonFirstAnswer.Tag = Answers[0].AnswerId;

            RadioButtonSecondAnswer.Content = Answers[1].Text;
            RadioButtonSecondAnswer.Tag = Answers[1].AnswerId;

            RadioButtonThirdAnswer.Content = Answers[2].Text;
            RadioButtonThirdAnswer.Tag = Answers[2].AnswerId;

            RadioButtonFourthAnswer.Content = Answers[3].Text;
            RadioButtonFourthAnswer.Tag = Answers[3].AnswerId;
        }

        private void SetupManyAnswers()
        {
            UniformGridOneCorrect.Visibility = Visibility.Collapsed;
            UniformGridManyCorrect.Visibility = Visibility.Visible;

            CheckBoxFirstAnswer.Content = Answers[0].Text;
            CheckBoxFirstAnswer.Tag = Answers[0].AnswerId;

            CheckBoxSecondAnswer.Content = Answers[1].Text;
            CheckBoxSecondAnswer.Tag = Answers[1].AnswerId;

            CheckBoxThirdAnswer.Content = Answers[2].Text;
            CheckBoxThirdAnswer.Tag = Answers[2].AnswerId;

            CheckBoxFourthAnswer.Content = Answers[3].Text;
            CheckBoxFourthAnswer.Tag = Answers[3].AnswerId;
        }


        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBoxEmail.Text) && !string.IsNullOrEmpty(TextBoxName.Text))
            {
                GridTestField.Visibility = Visibility.Visible;
                StackPanelFields.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Поля \"Имя\" и \"E-Email\" должны быть заполнены.", "Ошибка заполнения полей", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonEnd_Click(object sender, RoutedEventArgs e)
        {
            var userCorrectAnswers = UserAnswers.Count(x => x.IsCorrect);
            var numberOfCorrectAnswers = SelectedTest.Questions.SelectMany(x => x.Answers).Count(x => x.IsCorrect);
            var message = @$"Вы завершили тест. Кол-во правильных ответов: {userCorrectAnswers} из {numberOfCorrectAnswers}.";
            SaveResult();
            MessageBox.Show(message);
            ListViewTests.SelectedIndex = -1;
            SelectedTest = null!;
            Questions = null!;
            Question = null!;
            _questionNumber = 0;
            UncheckAllControls();
            UserAnswers = new();
            GridTest.Visibility = Visibility.Collapsed;
            GridTestField.Visibility = Visibility.Collapsed;
            StackPanelFields.IsEnabled = true;
            TextBoxEmail.Text = String.Empty;
            TextBoxName.Text = String.Empty;
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (_questionNumber + 1 < Questions.Count)
            {
                _isNavigationButtonClicked = true;
                _questionNumber++;
                SetupQuestion();
            }
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (_questionNumber - 1 >= 0)
            {
                _isNavigationButtonClicked = true;
                _questionNumber--;
                SetupQuestion();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var answerId = Convert.ToInt32(checkBox.Tag);
            UserAnswers.Add(new UserAnswer { AnswerId = answerId, QuestionId = Question.QuestionId, IsCorrect = Question.Answers.First(x => x.AnswerId == answerId).IsCorrect });
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var answerId = Convert.ToInt32(checkBox.Tag);
            if (!_isNavigationButtonClicked)
            {
                UserAnswers.Remove(UserAnswers.ToList().Find(x => x.AnswerId == answerId));
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var answerId = Convert.ToInt32(radioButton.Tag);
            if (!_isNavigationButtonClicked && UserAnswers.Any(x => x.QuestionId == Question.QuestionId))
            {
                UserAnswers.Remove(UserAnswers.ToList().Find(x => x.AnswerId == answerId));
            }
            UserAnswers.Add(new UserAnswer { AnswerId = answerId, QuestionId = Question.QuestionId, IsCorrect = Question.Answers.First(x => x.AnswerId == answerId).IsCorrect });

        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var answerId = Convert.ToInt32(radioButton.Tag);
            if (!_isNavigationButtonClicked)
            {
                UserAnswers.Remove(UserAnswers.ToList().Find(x => x.AnswerId == answerId));
            }
        }

        private void UncheckAllControls()
        {
            _isNavigationButtonClicked = false;
            RadioButtonFirstAnswer.IsChecked = false;
            RadioButtonSecondAnswer.IsChecked = false;
            RadioButtonThirdAnswer.IsChecked = false;
            RadioButtonFourthAnswer.IsChecked = false;
            CheckBoxFirstAnswer.IsChecked = false;
            CheckBoxSecondAnswer.IsChecked = false;
            CheckBoxThirdAnswer.IsChecked = false;
            CheckBoxFourthAnswer.IsChecked = false;
        }

        private void SaveResult()
        {
            using var db = new TestingDatabaseContext();
            Attempt attempt = new()
            {
                TestId = SelectedTest.TestId,
                Result = UserAnswers.Count(x => x.IsCorrect)
            };
        
            if (db.Users.Any(x => x.Email.ToLower() == TextBoxEmail.Text.ToLower()))
            {
                attempt.User = db.Users.First(x => x.Email.ToLower() == TextBoxEmail.Text.ToLower());
            }
            else
            {
                attempt.User = new User
                {
                    Email = TextBoxEmail.Text,
                    Name = TextBoxName.Text
                };
            }
            db.Attempts.Add(attempt);
            db.SaveChanges();
        }

        private void EditTestMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var test = ListViewTests.SelectedItem as Test;
            new AddEditTestWindow(true, test.TestId).Show();
            LoadData();
        }

        private void MenuItemAddTest_Click(object sender, RoutedEventArgs e)
        {
            new AddEditTestWindow(false).ShowDialog();
            LoadData();
        }

        private void DeleteTestMenuItme_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new TestingDatabaseContext())
            {

                db.Tests.Find(SelectedTest.TestId).IsDeleted = true;
                db.SaveChanges();
                LoadData();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SettingWindow().Show();
            LoadData();
        }

        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            AdminMenu.Visibility = AdminMenu.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
