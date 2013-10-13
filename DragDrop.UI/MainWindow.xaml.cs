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
        public MainWindow()
        {
            InitializeComponent();
        }

        private ListBoxItem draggedItem;
        private ListBoxItem DraggedItem
        {
            get { return draggedItem; }
            set
            {
                draggedItem = value;
                txtDraggedItem.Text = draggedItem == null ? "" : draggedItem.Content.ToString();
            }
        }

        private bool isDragging;
        private bool IsDragging
        {
            get { return isDragging; }
            set
            {
                isDragging = value;
                txtIsDragging.Text = IsDragging.ToString();
            }
        }

        private bool isDropping;
        private bool IsDropping
        {
            get { return isDropping; }
            set
            {
                isDropping = value;
                txtIsDropping.Text = IsDropping.ToString();
            }
        }

        private bool isOutsideWindow;
        private bool IsOutsideWindow
        {
            get { return isOutsideWindow; }
            set
            {
                isOutsideWindow = value;
                txtIsOutsideWindow.Text = IsOutsideWindow.ToString();
            }
        }

        private ListBox initialListBox;
        private ListBox InitialListBox
        {
            get { return initialListBox; }
            set
            {
                initialListBox = value;
                txtInitialListBox.Text = initialListBox == null ? "" : initialListBox.Name;
            }
        }

        private int initialIndex;
        private int InitialIndex
        {
            get { return initialIndex; }
            set
            {
                initialIndex = value;
                txtInitialIndex.Text = initialIndex.ToString();
            }
        }

        private Point dragStartPosition;
        private Point DragStartPosition
        {
            get { return dragStartPosition; }
            set
            {
                dragStartPosition = value;
                txtDragStartPosition.Text = dragStartPosition.ToString();
            }
        }

        private Point currentPosition;
        private Point CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                currentPosition = value;
                txtCurrentPosition.Text = currentPosition.ToString();
            }
        }

        private ListBox currentListBox;
        private ListBox CurrentListBox
        {
            get { return currentListBox; }
            set
            {
                currentListBox = value;
                txtCurrentListBox.Text = currentListBox == null ? "" : currentListBox.Name;
            }
        }

        private void OnListBoxItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var item = sender as ListBoxItem;
                if (item != null)
                {
                    StartDragging(e, item);
                }
            }
        }

        private void StartDragging(MouseButtonEventArgs e, ListBoxItem item)
        {
            IsDragging = true;

            DragStartPosition = e.GetPosition(item);

            DraggedItem = item;
            DraggedItem.Width = DraggedItem.ActualWidth;
            DraggedItem.Height = DraggedItem.ActualHeight;
            
            DraggedItem.CaptureMouse();
            
            var listBox = DraggedItem.Parent as ListBox;
            if (listBox != null)
            {
                InitialListBox = listBox;

                InitialIndex = listBox.Items.IndexOf(DraggedItem);
                listBox.Items.Remove(DraggedItem);
                DragContainer.Children.Add(DraggedItem);
            }

            //e.Handled = true;
        }

        private void OnListBoxItemMouseMove(object sender, MouseEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null && isDragging)
            {
                CurrentPosition = e.GetPosition(MainGrid);
                var xPosition = CurrentPosition.X - DragStartPosition.X;
                var yPosition = CurrentPosition.Y - DragStartPosition.Y;
                DraggedItem.RenderTransform = new TranslateTransform(xPosition, yPosition);

                IsOutsideWindow = CurrentPosition.Y < 0 || CurrentPosition.Y > MainGrid.ActualHeight ||
                                  CurrentPosition.X < 0 || CurrentPosition.X > MainGrid.ActualWidth;
            }    
        }


        private void OnListBoxItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var listBoxItem = sender as ListBoxItem;
                if (listBoxItem != null && ReferenceEquals(listBoxItem, DraggedItem))
                {
                    Debug.WriteLine("Dropped " + DraggedItem.Content);

                    StopDragging();
                    if (IsOutsideWindow)
                    {
                        DropItem(initialListBox, initialIndex);
                    }
                }
            }
        }

        private void StopDragging()
        {
            DraggedItem.ReleaseMouseCapture();
            DraggedItem.IsHitTestVisible = false;
            DraggedItem.RenderTransform = null;
            DraggedItem.Width = double.NaN;
            DraggedItem.Height = double.NaN;
            
            DragContainer.Children.Remove(DraggedItem);
            IsDragging = false;
            IsDropping = true;
        }

        private void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            if (IsDropping)
            {
                var listBox = sender as ListBox;
                if (listBox != null)
                {
                    CurrentListBox = listBox;

                    DropItem(listBox, 0);
                }
                else
                {
                    DropItem(initialListBox, InitialIndex);
                }    
            }
        }

        private void DropItem(ListBox listBox, int index)
        {
            listBox.Items.Insert(index, DraggedItem);
            
            InitialListBox = null;

            IsDropping = false;
            DraggedItem.IsHitTestVisible = true;
            DraggedItem = null;
        }

        private void OnListBoxMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var listBox = sender as ListBox;
                if (listBox != null && IsDragging)
                {
                    Debug.WriteLine("Dropped in " + listBox.Name);
                }
            }
        }
    }
}
