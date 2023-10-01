# Drag_Drop_C-

https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.insertionmark?view=netframework-4.8.1

## Ejemplo de Drag Drop, Certificados y firma con Autofirma en C# 
En el ejemplo de código siguiente se muestra cómo utilizar la característica de marca de inserción ListView. En este ejemplo se implementa la reordenación de elementos de arrastrar y colocar mediante los eventos de arrastre estándar. La posición de la marca de inserción se actualiza en un controlador para el evento Control.DragOver. En este controlador, la posición del puntero del mouse se compara con el punto medio del elemento más cercano y el resultado se usa para determinar si la marca de inserción aparece a la izquierda o a la derecha del elemento.

```
using System.Security.Cryptography.X509Certificates;
namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Initialize myListView.
            //listView1.View = View.Details;
            //listView1.MultiSelect = false;
            //listView1.FullRowSelect = true;
            //listView1.GridLines = true;
            //listView1.Columns.Add("Nombre", 50);
            //listView1.Columns.Add("Tamaño", 30);
            //listView1.Columns.Add("Ubicación", 100,HorizontalAlignment.Center);

            listView1.ListViewItemSorter = new ListViewIndexComparer();

            // Initialize the insertion mark.
            listView1.InsertionMark.Color = Color.Black;

            // Add items to myListView.
            //listView1.Items.Add("zero");
            //listView1.Items.Add("one");
            //listView1.Items.Add("two");
            //listView1.Items.Add("three");
            //listView1.Items.Add("four");
            //listView1.Items.Add("five");

            // Initialize the drag-and-drop operation when running
            // under Windows XP or a later operating system.
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


        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                String[] files = openFileDialog1.FileNames;

                foreach (String file in files)
                {
                    //listView1.Items.Add(file);
                    FileInfo fileInfo = new FileInfo(file);
                    listView1.Items
                        .Add(new ListViewItem(new string[] { fileInfo.Name, fileInfo.Length.ToString(), fileInfo.DirectoryName }));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            // almacén personal
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            //X509Extension extension
            //X509EnhancedKeyUsageExtension ext = (X509EnhancedKeyUsageExtension)extension;
            //OidCollection oids = ext.EnhancedKeyUsages;



            X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
            foreach (X509Certificate2 x509 in storecollection)
            {
                if (x509.HasPrivateKey)
                    listView1.Items.Add(new ListViewItem(new string[] { x509.Subject, x509.Issuer, "" }));
                //foreach (X509Extension extension in x509.Extensions) {
                //    X509EnhancedKeyUsageExtension ext = (X509EnhancedKeyUsageExtension)extension;
                //    Console.WriteLine(extension.ToString());
                //} 
            }

            ////Remove a certificate.
            //store.Remove(certificate1);
            //X509Certificate2Collection storecollection2 = (X509Certificate2Collection)store.Certificates;
            //Console.WriteLine("{1}Store name: {0}", store.Name, Environment.NewLine);
            //foreach (X509Certificate2 x509 in storecollection2)
            //{
            //    Console.WriteLine("certificate name: {0}", x509.Subject);
            //}

            ////Remove a range of certificates.
            //store.RemoveRange(collection);
            //X509Certificate2Collection storecollection3 = (X509Certificate2Collection)store.Certificates;
            //Console.WriteLine("{1}Store name: {0}", store.Name, Environment.NewLine);
            //if (storecollection3.Count == 0)
            //{
            //    Console.WriteLine("Store contains no certificates.");
            //}
            //else
            //{
            //    foreach (X509Certificate2 x509 in storecollection3)
            //    {
            //        Console.WriteLine("certificate name: {0}", x509.Subject);
            //}
            //}

            ////Close the store.
            store.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            // FIRMA SELECCIONANDO EL CERTIFICADO
            //ProcessStartInfo startInfo = new ProcessStartInfo("AutofirmaCommandLine.exe");
            //startInfo.CreateNoWindow = true;
            //startInfo.Arguments = "sign -certgui -i c:/temp/prueba.pdf -o c:/temp/prueba_signed.pdf";
            //Process.Start(startInfo);

            // FUNCIONA CON ESTO ARRANCADO AUTOFIRMA
            //ProcessStartInfo startInfo = new ProcessStartInfo("AutofirmaCommandLine.exe");
            //startInfo.CreateNoWindow = true;
            //Process.Start(startInfo);

            //TAMBIÉN FUNCIONA SOLO CON ESTO
            //Process.Start("AutofirmaCommandLine.exe", "sign -certgui -i c:\\temp\\prueba.pdf -o c:\\temp\\prueba_signed.pdf");
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
