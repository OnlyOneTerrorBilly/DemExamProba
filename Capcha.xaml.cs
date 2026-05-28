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

namespace DemExam
{
    public partial class Capcha : Window
    {
        private readonly Random _random = new Random();
        private readonly int _maxAttempts = 3;
        private string _currentCaptcha = "";
        private int _attemptCount = 0;

        public bool IsCaptchaPassed { get; private set; } = false;
        public bool IsCaptchaFailed { get; private set; } = false;

        public Capcha()
        {
            InitializeComponent();
            GenerateNewCaptcha();
            UpdateAttemptsDisplay();
        }

        private void GenerateNewCaptcha()
        {
            _currentCaptcha = _random.Next(1000, 9999).ToString();
            CaptchaTextBlock.Text = _currentCaptcha;
            CaptchaTextBox.Text = "";
            ErrorTextBlock.Text = "";
            SubmitButton.IsEnabled = false;
        }

        private void UpdateAttemptsDisplay()
        {
            AttemptsTextBlock.Text = $"Попыток осталось: {_maxAttempts - _attemptCount}";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewCaptcha();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = CaptchaTextBox.Text.Trim();

            if (userInput == _currentCaptcha)
            {
                IsCaptchaPassed = true;
                DialogResult = true;
                Close();
            }
            else
            {
                _attemptCount++;
                UpdateAttemptsDisplay();

                if (_attemptCount >= _maxAttempts)
                {
                    IsCaptchaFailed = true;
                    DialogResult = false;
                    Close();
                }
                else
                {
                    ErrorTextBlock.Text = $"Неверный код. Осталось попыток: {_maxAttempts - _attemptCount}";
                    GenerateNewCaptcha();
                }
            }
        }

        private void CaptchaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitButton.IsEnabled = CaptchaTextBox.Text.Trim().Length > 0;
        }
    }
}
