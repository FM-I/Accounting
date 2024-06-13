using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;

namespace PresentationWPF.Forms
{
    public partial class FirstInputDataForm : Window
    {
        private readonly IServiceProvider _serviceProvider;
        public FirstInputDataForm()
        {
            _serviceProvider = DIContainer.ServiceProvider;
            InitializeComponent();
        }

        private async void btnSave_ClickAsync(object sender, RoutedEventArgs e)
        {
            string errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(login.Text))
                errorMessage += "Лонгін не заповнений!\n";

            if (string.IsNullOrWhiteSpace(password.Password))
                errorMessage += "Пароль не заповнений!\n";

            if (string.IsNullOrWhiteSpace(organizationName.Text))
                errorMessage += "Організація не незаповнена!\n";

            if (string.IsNullOrWhiteSpace(currencyName.Text))
                errorMessage += "Валюта не незаповнена!";

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            var User = new User()
            {
                Login = login.Text,
                Password = password.Password,
                Name = login.Name
            };

            var organization = new Organization()
            {
                Name = organizationName.Text,
                IsDefault = true
            };

            var currency = new Currency()
            {
                Name = currencyName.Text,
                IsDefault = true
            };

            var handboocCtrl = _serviceProvider.GetRequiredService<IHandbookController>();
            await handboocCtrl.AddOrUpdateAsync(User);
            await handboocCtrl.AddOrUpdateAsync(organization);
            await handboocCtrl.AddOrUpdateAsync(currency);

            var mainForm = new LaunchForm();
            mainForm.Show();
            Close();

        }
    }
}
