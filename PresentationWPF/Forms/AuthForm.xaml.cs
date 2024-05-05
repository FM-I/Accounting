using BL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace PresentationWPF.Forms
{
    public partial class AuthForm : Window
    {
        private readonly IServiceProvider _services;
        private readonly IDbContext _dbContext;

        public bool IsClose { get; private set; }
        public AuthForm(IServiceProvider services)
        {
            _services = services;
            _dbContext = _services.GetRequiredService<IDbContext>();

            if (_dbContext.Users.Count() == 0)
            {
                var form = _services.GetRequiredService<FirstInputDataForm>();
                form.Show();
                IsClose = true;
                Close();
            }
            else 
            {
                InitializeComponent();
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnLogin_ClickAsync(object sender, RoutedEventArgs e)
        {
            var res = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == login.Text && x.Password == password.Password);

            if(res != null)
            {
                var mainForm = _services.GetRequiredService<MainWindow>();
                mainForm.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Логін або пароль не вірний!", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
