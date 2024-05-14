using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Data;

namespace PresentationWPF.Forms
{
    public partial class BankListForm : Window
    {
        private readonly IHandbookController _controller;
        private readonly IDbContext _context;
        private readonly bool _select;
        public Guid SelectedId { get; set; } = Guid.Empty;

        public BankListForm(bool select = false)
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;
            _select = select;

            InitializeComponent();

            context_SavedChanges(null, null);

            var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
            view.Filter = ListFilter;
        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            var list = _controller.GetHandbooks<Bank>();
            List<ListItem> items = new List<ListItem>();
            foreach (var item in list)
            {
                items.Add(new ListItem(item.Id, item.Code, item.Name, item.DeleteMark));
            }
            dataList.ItemsSource = items;
        }

        private void dataList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListItem item = (ListItem)dataList.SelectedItem;

            if (_select)
            {
                if (item != null)
                    SelectedId = item.Id;
                Close();
                return;
            }

            var elementForm = new BankElementForm(item.Id);
            elementForm.Show();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new BankElementForm();
            elementForm.Show();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Bank>(item.Id);

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
            var data = _controller.GetHandbook<Bank>(item.Id);
            if (data != null)
            {
                var elementForm = new BankElementForm((Bank)data.DeepCopy());
                elementForm.Show();
            }
        }

        private record ListItem(Guid Id, string Code, string DataName, bool DeleteMark);
    }
}
