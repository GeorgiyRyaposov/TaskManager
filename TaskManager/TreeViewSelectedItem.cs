using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace TaskManager
{
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        #region SelectedItem Property

        private TreeView TreeViewObj
        {
            get
            {
                return AssociatedObject;
            }
        }

        public Tasks SelectedItem
        {
            get { return (Tasks)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Tasks), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null,  OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = sender as BindableSelectedItemBehavior;
            if (behavior != null)
            {
                behavior.SelectedItem = (Tasks)e.NewValue;
            }
        }
        #endregion

        private void SubscribeToTreeViewEvents()
        {
            TreeViewObj.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        private void UnsubscribeFromTreeViewEvents()
        {
            TreeViewObj.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            SubscribeToTreeViewEvents();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (TreeViewObj != null)
            {
                UnsubscribeFromTreeViewEvents();
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = (Tasks)e.NewValue;
        }
    }
}
