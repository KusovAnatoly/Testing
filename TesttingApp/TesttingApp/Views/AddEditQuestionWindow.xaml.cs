using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddEditQuestionWindow.xaml
    /// </summary>
    public partial class AddEditQuestionWindow : Window
    {
        private byte[] _image;

        public AddEditQuestionWindow(int testId)
        {
            InitializeComponent();
            TestId = testId;
        }

        public int TestId { get; }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxFirstAnswer.IsChecked == true || CheckBoxSecondAnswer.IsChecked == true || CheckBoxThirdAnswer.IsChecked == true || CheckBoxFourthAnswer.IsChecked == true
                && !string.IsNullOrEmpty(TextBoxFirstAnswer.Text) || string.IsNullOrEmpty(TextBoxSecondAnswer.Text) || string.IsNullOrEmpty(TextBoxThirdAnswer.Text) || string.IsNullOrEmpty(TextBoxFourthAnswer.Text) || string.IsNullOrEmpty(TextBoxQuestionTitle.Text))
            {
                using (var db = new TestingDatabaseContext())
                {
                    Question question = new Question
                    {
                        Text = TextBoxQuestionTitle.Text,
                        Answers = new List<Answer>()
                        {
                            new Answer
                            {
                                IsCorrect = CheckBoxFirstAnswer.IsChecked == true,
                                Text = TextBoxFirstAnswer.Text
                            },
                            new Answer
                            {
                                IsCorrect = CheckBoxSecondAnswer.IsChecked == true,
                                Text = TextBoxSecondAnswer.Text
                            },
                            new Answer
                            {
                                IsCorrect = CheckBoxThirdAnswer.IsChecked == true,
                                Text = TextBoxThirdAnswer.Text
                            },
                            new Answer
                            {
                                IsCorrect = CheckBoxFourthAnswer.IsChecked == true,
                                Text = TextBoxFourthAnswer.Text
                            }
                        },
                        Image = _image
                    };
                    AddEditTestWindow.Questions.Add(question);
                    Close();
                }
            }
        }

        private byte[] GetImage()
        {
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog().Equals(true))
            {
                FileStream imageStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);

                byte[] iBytes = new byte[imageStream.Length + 1];
                return iBytes;
            }
            return null;
        }

        private void ButtonAddPicture_Click(object sender, RoutedEventArgs e)
        {
            _image = GetImage();
        }
    }
}
