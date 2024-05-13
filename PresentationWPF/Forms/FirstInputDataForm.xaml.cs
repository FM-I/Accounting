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
            await handboocCtrl.AddOrUpdateAsync(User, false);
            await handboocCtrl.AddOrUpdateAsync(organization, false);
            await handboocCtrl.AddOrUpdateAsync(currency);

            var mainForm = new MainWindow();
            mainForm.Show();
            Close();

        }
    }
}
