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

        private bool isAboutToDrag;
        private bool IsAboutToDrag
        {
            get { return isAboutToDrag; }
            set
            {
                isAboutToDrag = value;
                txtIsAboutToDrag.Text = isAboutToDrag.ToString();
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
            if (e.ChangedButton == MouseButton.Left)
            {
                var item = sender as ListBoxItem;
                if (item != null)
                {
                    IsAboutToDrag = true;
                    DragStartPosition = e.GetPosition(item);
                    DraggedItem = item;
                }
            }
        }

        private void StartDragging()
        {
            IsAboutToDrag = false;
            IsDragging = true;
            
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
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isAboutToDrag)
            {
                var mousePosition = e.GetPosition(DraggedItem);
                var distance = (mousePosition - dragStartPosition).Length;
                if (distance > 8)
                {
                    StartDragging();
                }    
            }

            if (isDragging)
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

            if (IsDropping)
            {
                var listBoxItem = sender as ListBoxItem;
                var listBox = sender as ListBox;
                if (listBoxItem != null)
                {
                    CurrentListBox = listBoxItem.Parent as ListBox;
                    var index = CurrentListBox.Items.IndexOf(listBoxItem);
                    DropItem(CurrentListBox, index);
                }
                else if (listBox != null)
                {
                    var position = e.GetPosition(listBox);
                    CurrentListBox = listBox;

                    var index = 0;
                    var heightSoFar = 0d;
                    foreach (ListBoxItem item in CurrentListBox.Items)
                    {
                        heightSoFar += item.ActualHeight;
                        if (position.Y > heightSoFar)
                        {
                            index++;
                        }
                    }
                    
                    DropItem(listBox, index);
                }
                else
                {
                    DropItem(initialListBox, InitialIndex);
                }
            }
        }

        private void OnListBoxItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (isAboutToDrag)
                {
                    isAboutToDrag = false;
                    DraggedItem = null;
                }
                if (isDragging)
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
