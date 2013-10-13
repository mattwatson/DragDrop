using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DragDrop.UI
{
    public partial class MainWindow
    {
        private bool isDragging;
        private Point dragStartPosition;
        private ListBoxItem draggedItem;
        private int dragCount;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnListBoxItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var item = sender as ListBoxItem;
                if (item != null)
                {
                    Debug.WriteLine("Picked up " + item.Content);

                    StartDragging(e, item);
                }
            }
        }

        private void StartDragging(MouseButtonEventArgs e, ListBoxItem item)
        {
            isDragging = true;
            dragStartPosition = e.GetPosition(this);
            draggedItem = item;
            draggedItem.CaptureMouse();
            var listBox = draggedItem.Parent as ListBox;
            //listBox.
            //DragContainer.Children.Add(draggedItem);

            //e.Handled = true;
        }

        private void OnListBoxItemMouseMove(object sender, MouseEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null && ReferenceEquals(listBoxItem, draggedItem))
            {
                var currentPosition = e.GetPosition(this) - dragStartPosition;

                Debug.WriteLine("Dragging " + listBoxItem.Content + " (" + currentPosition + ")");

                draggedItem.RenderTransform = new TranslateTransform(currentPosition.X, currentPosition.Y);
            }
        }

        private void OnListBoxItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var listBoxItem = sender as ListBoxItem;
                if (listBoxItem != null && ReferenceEquals(listBoxItem, draggedItem))
                {
                    Debug.WriteLine("Dropped " + draggedItem.Content);

                    StopDragging();
                }
            }
        }

        private void StopDragging()
        {
            isDragging = false;
            draggedItem.ReleaseMouseCapture();
            draggedItem.RenderTransform = null;
            draggedItem = null;
        }

        private void OnListBoxMouseMove(object sender, MouseEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null && isDragging)
            {
                Debug.WriteLine("Over " + listBox.Name);
                var random = new Random();
                listBox.Background = new SolidColorBrush(Color.FromRgb((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255)));
            }
        }

        private void OnListBoxMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var listBox = sender as ListBox;
                if (listBox != null && isDragging)
                {
                    Debug.WriteLine("Dropped in " + listBox.Name);
                }
            }
        }
    }
}
