using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;

namespace PresentationWPF.Forms
{
    public partial class BarcodeInput : Window
    {
        private readonly IInformationRegisterController _controller;
        public Nomenclature Nomenclature { get; set; }
        public Unit Unit { get; set; }

        public BarcodeInput()
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var values = _controller.GetListData<Barcode>(selectionFunc: w => w.Value ==  Barcode.Text.Trim());

            if(values != null && values.Count > 0) 
            {
                var value = values[0];
                Nomenclature = value.Nomenclature;
                Unit = value.Unit;
            }

            Close();
        }
    }
}
