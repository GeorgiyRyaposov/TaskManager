using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using TaskManager.ViewModels;

namespace TaskManager
{
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        #region SelectedItem Property

        public Tasks SelectedItem
        {
            get { return TaskManagerViewModel.SelectedTask; }
            set { TaskManagerViewModel.SelectedTask = value; }
            //get{ return (Tasks)GetValue(SelectedItemProperty); }
            //set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = e.NewValue as TreeViewItem;
            if (item != null)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
                
            }
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            
            this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = (Tasks)e.NewValue;
        }
    }
}
