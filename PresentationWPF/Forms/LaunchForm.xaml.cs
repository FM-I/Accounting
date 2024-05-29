using BL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PresentationWPF.Forms
{
    public partial class LaunchForm : Window
    {
        public LaunchForm()
        {
            InitializeComponent();

            Uri resourceUri = new Uri("pack://application:,,,/Images/icon512.png");
            Logo.Source = new BitmapImage(resourceUri);
        }

        private async void Init()
        {
            var h = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            var d = DIContainer.ServiceProvider.GetRequiredService<IDocumentController>();
            var ifr = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            var ia = DIContainer.ServiceProvider.GetRequiredService<IAccumulationRegisterController>();
            var db = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();

            await Task.Delay(1500);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1500);
            Init();

            var form = new MainWindow();
            form.Show();

            Close();
        }
    }
}
