﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DragDrop.UI
{
    public partial class MainWindow
    {
        private Point dragEndPosition;

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
            if (listBoxItem != null && ReferenceEquals(listBoxItem, DraggedItem))
            {
                var currentPosition = e.GetPosition(this);
                currentPosition.Offset(-dragStartPosition.X, -dragStartPosition.Y);

                Debug.WriteLine("Dragging " + listBoxItem.Content + " (" + currentPosition + ")");

                DraggedItem.RenderTransform = new TranslateTransform(currentPosition.X, currentPosition.Y);
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
                }
            }
        }

        private void StopDragging()
        {
            DraggedItem.ReleaseMouseCapture();
            DraggedItem.RenderTransform = null;
            
            DragContainer.Children.Remove(DraggedItem);

            InitialListBox.Items.Insert(InitialIndex, DraggedItem);
            InitialListBox = null;

            IsDragging = false;
            DraggedItem = null;
        }

        private void OnListBoxMouseMove(object sender, MouseEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null && IsDragging)
            {
                var random = new Random();
                listBox.Background = new SolidColorBrush(Color.FromRgb((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255)));
            }
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
