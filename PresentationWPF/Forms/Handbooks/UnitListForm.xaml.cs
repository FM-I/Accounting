using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Data;

namespace PresentationWPF.Forms
{
    /// <summary>
    /// Interaction logic for UnitListForm.xaml
    /// </summary>
    public partial class UnitListForm : Window
    {
        private readonly IHandbookController _controller;
        private readonly IDbContext _context;
        private bool _select = false;
        public Guid SelectedId { get; set; }
        public UnitListForm(bool select = false)
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;
            _select = select;

            InitializeComponent();

            context_SavedChanges(null, null);
        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            var list = _controller.GetHandbooks<Unit>();
            List<ListItem> items = new List<ListItem>();
            foreach (var item in list)
            {
                items.Add(new ListItem(item.Id, item.Code, item.Name, item.DeleteMark, item.Coefficient));
            }
            dataList.ItemsSource = items;
            var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
            view.Filter = ListFilter;
        }

        private void dataList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListItem item = (ListItem)dataList.SelectedItem;

            if (item == null)
                return;

            if (_select)
            {                
                SelectedId = item.Id;
                Close();
                return;
            }
            var elementForm = new UnitElementForm(item.Id);
            elementForm.Show();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new UnitElementForm();
            elementForm.Show();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Unit>(item.Id);

            MessageBoxResult result;
            if (item.DeleteMark)
                result = MessageBox.Show("Обраний об'єкт помічений на видалення. Зняти помітку?", "", MessageBoxButton.YesNo);
            else
                result = MessageBox.Show("Помітити на видалення обраний об'єкт?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes && data != null)
            {
                data.DeleteMark = !data.DeleteMark;
                await _controller.AddOrUpdateAsync(data);
            }
        }

        private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Search.Text))
                Placeholder.Visibility = Visibility.Hidden;
            else
                Placeholder.Visibility = Visibility.Visible;

            if (dataList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(dataList.ItemsSource).Refresh();
        }

        private bool ListFilter(object item)
        {
            if (String.IsNullOrEmpty(Search.Text))
                return true;
            else
                return ((item as ListItem).DataName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Unit>(item.Id);
            if (data != null)
            {
                var elementForm = new UnitElementForm((Unit)data.DeepCopy());
                elementForm.Show();
            }
        }

        private record ListItem(Guid Id, string Code, string DataName, bool DeleteMark, double Coefficient);
    }
}
