using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using TesttingApp.Models;

namespace TesttingApp.Views
{
    /// <summary>
    /// Interaction logic for AddEditQuestionWindow.xaml
    /// </summary>
    public partial class AddEditQuestionWindow : Window
    {
        public Question Question { get; set; }
        public bool IsEdit { get; }
        public List<Question> Questions { get; }
        public int? QuestionId { get; }
        public int? TestId { get; }

        private byte[] _image;

        public AddEditQuestionWindow(bool isEdit, ref List<Question> questions, int? questionId = null, int? testId = null)
        {
            InitializeComponent();
            if (isEdit && questionId != null)
            {
                using var db = new TestingDatabaseContext();
                Question = db.Questions.Include(x => x.Answers).First(x => x.QuestionId == questionId);

                TextBoxQuestionText.Text = Question.Text;

                TextBoxFirstAnswer.Text = Question.SpecialAnswers[0].Text;
                TextBoxFirstAnswer.Tag = Question.SpecialAnswers[0].AnswerId;

                TextBoxSecondAnswer.Text = Question.SpecialAnswers[1].Text;
                TextBoxSecondAnswer.Tag = Question.SpecialAnswers[1].AnswerId;

                TextBoxThirdAnswer.Text = Question.SpecialAnswers[2].Text;
                TextBoxThirdAnswer.Tag = Question.SpecialAnswers[2].AnswerId;

                TextBoxFourthAnswer.Text = Question.SpecialAnswers[3].Text;
                TextBoxFourthAnswer.Tag = Question.SpecialAnswers[3].AnswerId;

                CheckBoxFirstAnswer.IsChecked = Question.SpecialAnswers[0].IsCorrect;
                CheckBoxSecondAnswer.IsChecked = Question.SpecialAnswers[1].IsCorrect;
                CheckBoxThirdAnswer.IsChecked = Question.SpecialAnswers[2].IsCorrect;
                CheckBoxFourthAnswer.IsChecked = Question.SpecialAnswers[3].IsCorrect;
            }
            else
            {
                Question = new();
            }
            IsEdit = isEdit;
            Questions = questions;
            QuestionId = questionId;
            TestId = testId;
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxFirstAnswer.IsChecked == true || CheckBoxSecondAnswer.IsChecked == true || CheckBoxThirdAnswer.IsChecked == true || CheckBoxFourthAnswer.IsChecked == true
                && !string.IsNullOrEmpty(TextBoxFirstAnswer.Text) || string.IsNullOrEmpty(TextBoxSecondAnswer.Text) || string.IsNullOrEmpty(TextBoxThirdAnswer.Text) || string.IsNullOrEmpty(TextBoxFourthAnswer.Text) || string.IsNullOrEmpty(TextBoxQuestionText.Text))
            {
                using (var db = new TestingDatabaseContext())
                {
                    if (IsEdit && QuestionId != null)
                    {
                        var question = db.Questions.Include(x => x.Answers).First(x => x.QuestionId == Question.QuestionId);
                        question.Text = TextBoxQuestionText.Text;
                        question.Image = _image;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxFirstAnswer.Tag)).Text = TextBoxFirstAnswer.Text;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxFirstAnswer.Tag)).IsCorrect = (bool)CheckBoxFirstAnswer.IsChecked;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxSecondAnswer.Tag)).Text = TextBoxSecondAnswer.Text;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxSecondAnswer.Tag)).IsCorrect = (bool)CheckBoxSecondAnswer.IsChecked;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxThirdAnswer.Tag)).Text = TextBoxThirdAnswer.Text;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxThirdAnswer.Tag)).IsCorrect = (bool)CheckBoxThirdAnswer.IsChecked;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxFourthAnswer.Tag)).Text = TextBoxFourthAnswer.Text;
                        question.Answers.First(x => x.AnswerId == Convert.ToInt32(TextBoxFourthAnswer.Tag)).IsCorrect = (bool)CheckBoxFourthAnswer.IsChecked;
                        db.SaveChanges();
                    }
                    else if (IsEdit && TestId != null)
                    {
                        var question = new Question { Text = TextBoxQuestionText.Text};
                        question.Text = TextBoxQuestionText.Text;
                        question.Image = _image;
                        question.Answers.Add(new Answer { IsCorrect = (bool)CheckBoxFirstAnswer.IsChecked, Text = TextBoxFirstAnswer.Text });
                        question.Answers.Add(new Answer { IsCorrect = (bool)CheckBoxSecondAnswer.IsChecked, Text = TextBoxSecondAnswer.Text });
                        question.Answers.Add(new Answer { IsCorrect = (bool)CheckBoxThirdAnswer.IsChecked, Text = TextBoxThirdAnswer.Text });
                        question.Answers.Add(new Answer { IsCorrect = (bool)CheckBoxFourthAnswer.IsChecked, Text = TextBoxFourthAnswer.Text });
                        var test = db.Tests.Find(TestId);
                        test.Questions.Add(question);
                        db.SaveChanges();
                    }
                    else
                    {
                        Question.Text = TextBoxQuestionText.Text;
                        Question.Image = _image;
                        Question.Answers.Add(new Answer { Text = TextBoxFirstAnswer.Text, IsCorrect = (bool)CheckBoxFirstAnswer.IsChecked });
                        Question.Answers.Add(new Answer { Text = TextBoxSecondAnswer.Text, IsCorrect = (bool)CheckBoxSecondAnswer.IsChecked });
                        Question.Answers.Add(new Answer { Text = TextBoxThirdAnswer.Text, IsCorrect = (bool)CheckBoxThirdAnswer.IsChecked });
                        Question.Answers.Add(new Answer { Text = TextBoxFourthAnswer.Text, IsCorrect = (bool)CheckBoxFourthAnswer.IsChecked });
                        Questions.Add(Question);

                    }
                    Close();
                }
            }
        }

        private void ButtonAddPicture_Click(object sender, RoutedEventArgs e)
        {
            _image = GetImage();
        }

        private void ButtonDeletePicture_Click(object sender, RoutedEventArgs e)
        {
            _image = null;
        }

        private byte[] GetImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Equals(true))
            {
                byte[] data;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(new Uri(openFileDialog.FileName)));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                return data;
            }
            return null;
        }
    }
}
