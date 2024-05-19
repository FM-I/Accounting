using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PresentationWPF.Forms.Handbooks
{
    public partial class ClientGroupListForm : Window
    {
        private class Data()
        {
            public Guid Id { get; set; }
            public object Content { get; set; }
            public object Image { get; set; } = new BitmapImage(new Uri("D:\\Politeh\\2024\\Project\\Accounting\\PresentationWPF\\Images\\folder.png"));
            public List<Data> Items { get; set; } = new();
        }

        public Guid SelectedId { get; set; }
        private readonly IHandbookController _controller;
        public ClientGroupListForm()
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            InitializeComponent();
        }

        private void tree_Initialized(object sender, EventArgs e)
        {
            var data = _controller.GetHandbooks<Client>(w => w.IsGroup);

            List<Data> list = new List<Data>();
            AddChildren(data, list, null);

            tree.ItemsSource = list;
        }

        private void AddChildren(List<Client> data, List<Data> list, Client parent)
        {
            foreach (var item in data)
            {
                if (item.Parent?.Id == parent?.Id)
                {
                    list.Add(new Data { Id = item.Id, Content = item.Name });
                    AddChildren(data, list.Last().Items, item);
                }
            }
        }

        private void tree_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                e.Handled = true;

                var item = ((TreeView)sender).SelectedItem as Data;

                if (item == null)
                    return;

                SelectedId = item.Id;
                Close();
            }
        }
    }
}
