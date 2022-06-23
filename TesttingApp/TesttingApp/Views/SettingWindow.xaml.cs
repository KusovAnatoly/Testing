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

namespace TesttingApp.Views
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            TextBoxConnectionString.Text = Config.Default.ConnectionString;
            CheckBoxShowRightAnswer.IsChecked = Config.Default.ShowRightAnswer;
        }

        private void CheckBoxShowRightAnswer_Checked(object sender, RoutedEventArgs e)
        {
            Config.Default.ShowRightAnswer = CheckBoxShowRightAnswer.IsChecked == true;
            Config.Default.Save();
        }

        private void CheckBoxShowRightAnswer_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Default.ShowRightAnswer = CheckBoxShowRightAnswer.IsChecked == true;
            Config.Default.Save();
        }
    }
}
