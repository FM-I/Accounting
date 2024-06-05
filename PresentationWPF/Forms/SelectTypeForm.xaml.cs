using Domain.Entity.Handbooks;
using System.Windows;
using System.Windows.Controls;

namespace PresentationWPF.Forms
{
    public partial class SelectTypeForm : Window
    {
        public class DataItem
        {
            public string TypeName { get; set; }
            public Type Type { get; set; }
        }

        private List<DataItem> _items = new()
        {
            new(){ Type = typeof(CashBox), TypeName = "Каси" },
            new(){ Type = typeof(BankAccount), TypeName = "Банківські рахунки" },
        };

        public List<DataItem> Data { get { return _items; } }

        public Type? SelectedType { get; set; }

        public SelectTypeForm()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataGrid = (DataGrid)sender;

            if (dataGrid == null)
                return;

            var item = dataGrid.SelectedItem as DataItem;

            if (item == null) 
                return;

            SelectedType = item.Type;

            Close();
        }
    }
}
