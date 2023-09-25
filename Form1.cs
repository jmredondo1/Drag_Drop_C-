namespace DragDrop_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.ListViewItemSorter = new ListViewIndexComparer();
            listView1.InsertionMark.Color = Color.Blue;

            // Añadir datos
            listView1.Items.Add("zero");
            listView1.Items.Add("one");
            listView1.Items.Add("two");
            listView1.Items.Add("three");
            listView1.Items.Add("four");
            listView1.Items.Add("five");

            if (OSFeature.Feature.IsPresent(OSFeature.Themes))
            {
                listView1.AllowDrop = true;
                listView1.ItemDrag += new ItemDragEventHandler(listView1_ItemDrag);
                listView1.DragEnter += new DragEventHandler(listView1_DragEnter);
                listView1.DragOver += new DragEventHandler(listView1_DragOver);
                listView1.DragLeave += new EventHandler(listView1_DragLeave);
                listView1.DragDrop += new DragEventHandler(listView1_DragDrop);
            }

        }

        private void listView1_DragDrop(object? sender, DragEventArgs e)
        {
            // Retrieve the index of the insertion mark;
            int targetIndex = listView1.InsertionMark.Index;

            // If the insertion mark is not visible, exit the method.
            if (targetIndex == -1)
            {
                return;
            }

            // If the insertion mark is to the right of the item with
            // the corresponding index, increment the target index.
            if (listView1.InsertionMark.AppearsAfterItem)
            {
                targetIndex++;
            }

            // Retrieve the dragged item.
            ListViewItem draggedItem =
                (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            // Insert a copy of the dragged item at the target index.
            // A copy must be inserted before the original item is removed
            // to preserve item index values. 
            listView1.Items.Insert(
                targetIndex, (ListViewItem)draggedItem.Clone());

            // Remove the original copy of the dragged item.
            listView1.Items.Remove(draggedItem);
        }

        private void listView1_DragLeave(object? sender, EventArgs e)
        {
            listView1.InsertionMark.Index = -1;
        }

        private void listView1_DragOver(object? sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse pointer.
            Point targetPoint = listView1.PointToClient(new Point(e.X, e.Y));

            // Retrieve the index of the item closest to the mouse pointer.
            int targetIndex = listView1.InsertionMark.NearestIndex(targetPoint);

            // Confirm that the mouse pointer is not over the dragged item.
            if (targetIndex > -1)
            {
                // Determine whether the mouse pointer is to the left or
                // the right of the midpoint of the closest item and set
                // the InsertionMark.AppearsAfterItem property accordingly.
                Rectangle itemBounds = listView1.GetItemRect(targetIndex);
                if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2))
                {
                    listView1.InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    listView1.InsertionMark.AppearsAfterItem = false;
                }
            }

            // Set the location of the insertion mark. If the mouse is
            // over the dragged item, the targetIndex value is -1 and
            // the insertion mark disappears.
            listView1.InsertionMark.Index = targetIndex;
        }

        private void listView1_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void listView1_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            listView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private class ListViewIndexComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((ListViewItem)x).Index - ((ListViewItem)y).Index;
            }
        }
    }
}