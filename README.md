### Drag_Drop_C-

https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.insertionmark?view=netframework-4.8.1

## Ejemplo de Drag Drop en C#
En el ejemplo de código siguiente se muestra cómo utilizar la característica de marca de inserción ListView. En este ejemplo se implementa la reordenación de elementos de arrastrar y colocar mediante los eventos de arrastre estándar. La posición de la marca de inserción se actualiza en un controlador para el evento Control.DragOver. En este controlador, la posición del puntero del mouse se compara con el punto medio del elemento más cercano y el resultado se usa para determinar si la marca de inserción aparece a la izquierda o a la derecha del elemento.
** C# **

```
using System;
using System.Drawing;
using System.Windows.Forms;

public class ListViewInsertionMarkExample : Form
{
    private ListView myListView; 

    public ListViewInsertionMarkExample()
    {
        // Initialize myListView.
        myListView = new ListView();
        myListView.Dock = DockStyle.Fill;
        myListView.View = View.LargeIcon;
        myListView.MultiSelect = false;
        myListView.ListViewItemSorter = new ListViewIndexComparer();

        // Initialize the insertion mark.
        myListView.InsertionMark.Color = Color.Green;

        // Add items to myListView.
        myListView.Items.Add("zero");
        myListView.Items.Add("one");
        myListView.Items.Add("two");
        myListView.Items.Add("three");
        myListView.Items.Add("four");
        myListView.Items.Add("five");
        
        // Initialize the drag-and-drop operation when running
        // under Windows XP or a later operating system.
        if (OSFeature.Feature.IsPresent(OSFeature.Themes))
        {
            myListView.AllowDrop = true;
            myListView.ItemDrag += new ItemDragEventHandler(myListView_ItemDrag);
            myListView.DragEnter += new DragEventHandler(myListView_DragEnter);
            myListView.DragOver += new DragEventHandler(myListView_DragOver);
            myListView.DragLeave += new EventHandler(myListView_DragLeave);
            myListView.DragDrop += new DragEventHandler(myListView_DragDrop);
        }

        // Initialize the form.
        this.Text = "ListView Insertion Mark Example";
        this.Controls.Add(myListView);
    }

    [STAThread]
    static void Main() 
    {
        Application.EnableVisualStyles();
        Application.Run(new ListViewInsertionMarkExample());
    }

    // Starts the drag-and-drop operation when an item is dragged.
    private void myListView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        myListView.DoDragDrop(e.Item, DragDropEffects.Move);
    }

    // Sets the target drop effect.
    private void myListView_DragEnter(object sender, DragEventArgs e)
    {
        e.Effect = e.AllowedEffect;
    }

    // Moves the insertion mark as the item is dragged.
    private void myListView_DragOver(object sender, DragEventArgs e)
    {
        // Retrieve the client coordinates of the mouse pointer.
        Point targetPoint = 
            myListView.PointToClient(new Point(e.X, e.Y));

        // Retrieve the index of the item closest to the mouse pointer.
        int targetIndex = myListView.InsertionMark.NearestIndex(targetPoint);

        // Confirm that the mouse pointer is not over the dragged item.
        if (targetIndex > -1) 
        {
            // Determine whether the mouse pointer is to the left or
            // the right of the midpoint of the closest item and set
            // the InsertionMark.AppearsAfterItem property accordingly.
            Rectangle itemBounds = myListView.GetItemRect(targetIndex);
            if ( targetPoint.X > itemBounds.Left + (itemBounds.Width / 2) )
            {
                myListView.InsertionMark.AppearsAfterItem = true;
            }
            else
            {
                myListView.InsertionMark.AppearsAfterItem = false;
            }
        }

        // Set the location of the insertion mark. If the mouse is
        // over the dragged item, the targetIndex value is -1 and
        // the insertion mark disappears.
        myListView.InsertionMark.Index = targetIndex;
    }

    // Removes the insertion mark when the mouse leaves the control.
    private void myListView_DragLeave(object sender, EventArgs e)
    {
        myListView.InsertionMark.Index = -1;
    }

    // Moves the item to the location of the insertion mark.
    private void myListView_DragDrop(object sender, DragEventArgs e)
    {
        // Retrieve the index of the insertion mark;
        int targetIndex = myListView.InsertionMark.Index;

        // If the insertion mark is not visible, exit the method.
        if (targetIndex == -1) 
        {
            return;
        }

        // If the insertion mark is to the right of the item with
        // the corresponding index, increment the target index.
        if (myListView.InsertionMark.AppearsAfterItem) 
        {
            targetIndex++;
        }

        // Retrieve the dragged item.
        ListViewItem draggedItem = 
            (ListViewItem)e.Data.GetData(typeof(ListViewItem));

        // Insert a copy of the dragged item at the target index.
        // A copy must be inserted before the original item is removed
        // to preserve item index values. 
        myListView.Items.Insert(
            targetIndex, (ListViewItem)draggedItem.Clone());

        // Remove the original copy of the dragged item.
        myListView.Items.Remove(draggedItem);
    }

    // Sorts ListViewItem objects by index.
    private class ListViewIndexComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((ListViewItem)x).Index - ((ListViewItem)y).Index;
        }
    }
}
```


# Notas
La característica de marca de inserción ListView permite indicar visualmente la ubicación de colocación esperada en una operación de arrastrar y colocar cuando se arrastra un elemento a una nueva posición. Esta característica sólo funciona cuando la propiedad AutoArrange se establece en true y cuando el control ListView no ordena los elementos automáticamente. Para evitar la ordenación automática, la propiedad Sorting debe establecerse en SortOrder.None y la propiedad View debe establecerse en View.LargeIcon, View.SmallIcon o View.Tile. Además, es posible que la característica de marca de inserción no esté visible con la característica de agrupación ListView porque la característica de agrupación ordena los elementos por pertenencia a grupos.

La clase ListViewInsertionMark se usa normalmente en un controlador para el evento Control.DragOver o Control.MouseMove para actualizar la posición de la marca de inserción a medida que se arrastra un elemento. También se usa en un controlador para el evento Control.DragDrop o Control.MouseUp para insertar un elemento arrastrado en la ubicación correcta. Para obtener más información, vea ListViewInsertionMark y Cómo: Mostrar una marca de inserción en un control ListView de formularios Windows Forms.

# Nota

La característica de marca de inserción sólo está disponible en Windows XP y Windows Server 2003 cuando la aplicación llama al método Application.EnableVisualStyles. En sistemas operativos anteriores, cualquier código relacionado con la marca de inserción no tiene efecto y la marca de inserción no aparecerá. Como resultado, es posible que cualquier código que dependa de la característica de marca de inserción no funcione correctamente. Es posible que desee incluir código que determine si esta característica está disponible y proporcionar funcionalidad alternativa cuando no esté disponible. Por ejemplo, es posible que desee omitir todo el código que implementa el reposicionamiento de elementos de arrastrar y colocar cuando se ejecuta en sistemas operativos que no admiten marcas de inserción.

La característica de marca de inserción es proporcionada por la misma biblioteca que proporciona la característica de temas del sistema operativo. Para comprobar la disponibilidad de esta biblioteca, llame a la sobrecarga del método FeatureSupport.IsPresent(Object) y pase el valor OSFeature.Themes.
