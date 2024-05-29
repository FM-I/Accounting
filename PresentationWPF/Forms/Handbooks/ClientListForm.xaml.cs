using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PresentationWPF.Forms.Handbooks
{
    public partial class ClientListForm : Window
    {
        private class DataType
        {
            public string Text { get; set; }
            public TypesClient Type { get; set; }

        }
        private readonly IHandbookController _controller;
        private readonly IDbContext _context;
        private bool _select;
        private bool _onlyGroup;

        public Guid SelectedId { get; set; }

        public ClientListForm(bool select = false, bool onlyGroup = false)
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;
            _select = select;
            _onlyGroup = onlyGroup;

            InitializeComponent();

            List<DataType> list = new()
            {
                new(){ Text = "Всі", Type = TypesClient.None },
                new(){ Text = "Покупець", Type = TypesClient.Client },
                new(){ Text = "Поставщик", Type = TypesClient.Provider }
            };

            TypeProduct.ItemsSource = list;
            TypeProduct.SelectedItem = list.First();

            context_SavedChanges(null, null);
        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            var list = _controller.GetHandbooks<Client>(where => !where.IsGroup);
            List<ListItem> items = new List<ListItem>();
            foreach (var item in list)
            {
                if (_onlyGroup && !item.IsGroup)
                    continue;

                items.Add(new ListItem(item.Id, item.Code, item.Name, item.DeleteMark, item.Parent?.Id, item.TypeClient));
            }
            dataList.ItemsSource = items;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
            view.Filter = ListFilter;

            treeGroups_Initialized(null, null);
        }

        private void dataList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

            var elementForm = new ClientElementForm(item.Id);
            elementForm.Show();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new ClientElementForm();
            elementForm.Show();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Client>(item.Id);

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

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
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
            var group = (GroupData)treeGroups.SelectedItem;
            var type = (DataType)TypeProduct.SelectedItem;

            bool findText = false;
            bool findGroup = (item as ListItem).ParentId == group?.Id;
            bool findType = (type.Type == TypesClient.None || type.Type == (item as ListItem).TypeClient);

            if (String.IsNullOrEmpty(Search.Text))
                findText = true;
            else
                findText = ((item as ListItem).DataName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            return findText && findGroup && findType;
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Client>(item.Id);
            if (data != null)
            {
                var elementForm = new ClientElementForm((Client)data.DeepCopy());
                elementForm.Show();
            }
        }

        private void createGroupBtn_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new ClientGroupForm();
            elementForm.Show();
        }

        private void treeGroups_Initialized(object sender, EventArgs e)
        {
            var data = _controller.GetHandbooks<Client>(w => w.IsGroup);

            List<GroupData> list = [new() { Id = null, Content = "Без групи", Image = null }];
            AddChildren(data, list, null);

            treeGroups.ItemsSource = list;
        }

        private void AddChildren(List<Client> data, List<GroupData> list, Client parent)
        {
            foreach (var item in data)
            {
                if (item.Parent?.Id == parent?.Id)
                {
                    list.Add(new GroupData { Id = item.Id, Content = item.Name });
                    AddChildren(data, list.Last().Items, item);
                }
            }
        }

        private void treeGroups_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ((TreeView)sender).SelectedItem as GroupData;

            if (item == null || item.Id == null || item.Id == Guid.Empty)
                return;

            var form = new ClientGroupForm((Guid)item.Id);
            form.ShowDialog();
        }



        private void treeGroups_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                e.Handled = true;
                treeGroups_MouseDoubleClick(sender, e);
            }
        }

        private void treeGroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dataList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(dataList.ItemsSource).Refresh();
        }

        private void TypeProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(dataList.ItemsSource).Refresh();
        }

        private record ListItem(Guid Id, string Code, string DataName, bool DeleteMark, Guid? ParentId, TypesClient TypeClient);

        private class GroupData()
        {
            public object Content { get; set; }
            public Guid? Id { get; set; }
            public object Image { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Images/folder.png"));
            public List<GroupData> Items { get; set; } = new();

        }
    }
}
