﻿using BL.Controllers;
using BL.Interfaces;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms;
using System.IO;
using System.Text;
using System.Windows;

namespace PresentationWPF
{
    public partial class App : Application
    {
        public IConfiguration Configuration { get; set; }

        protected override void OnStartup(StartupEventArgs eventArgs)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            DIContainer.ServiceProvider = serviceCollection.BuildServiceProvider();
            var mainWindow = new AuthForm();
            if(!mainWindow.IsClose)
                mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IDbContext, AppDbContext>();
            services.AddSingleton<IHandbookController, HandbookController>();
            services.AddSingleton<IDocumentController, DocumentController>();
            services.AddSingleton<IInformationRegisterController, InformationRegisterController>();
            services.AddSingleton<IAccumulationRegisterController, AccumulationRegisterController>();
            services.AddSingleton<IExchangeRateService, PrivatBankExchangeRateService>();
        }
    }

}
