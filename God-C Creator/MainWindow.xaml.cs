using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.IO;

namespace God_C_Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //===========DRAGING AND DROPPING OF MAIN WINDOW=========
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg,
                int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        //=======================================================


        private List<TabItem> _tabItems;
        private TabItem _lastTabItem;
        private string _addTabHeader;

        private string _currentProject;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this._addTabHeader = "+";
                // initialize tabItem array
                this._tabItems = new List<TabItem>();

                // add a tabItem with + in header 
                TabItem tabAdd = new TabItem();
                tabAdd.Header = "+";

                this._tabItems.Add(tabAdd);

                // add first tab
                int count = this._tabItems.Count;
                string name = string.Format("Document{0}", count);
                this._currentProject = GenerateStartProgram();
                this.AddTabItemWithName(name);

                // bind tab control
                this.tabControlDocs.DataContext = this._tabItems;

                this.tabControlDocs.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //==================WORKING WITH TAB ITEMS===================
        #region Tabs
        private string GenerateStartProgram()
        {
            return "main \n{\n\x9return 0;\n}";
        }
        private string GetTextFromCurrentTab()
        {
           return new TextRange(((RichTextBox)this._lastTabItem.Content).Document.ContentStart, ((RichTextBox)this._lastTabItem.Content).Document.ContentEnd).Text;
        }
        private void CreateNewTabeWithName(string name)
        {
            TabItem newTab = this.AddTabItemWithName(name);
            this.tabControlDocs.DataContext = null;
            this.tabControlDocs.DataContext = _tabItems;
            this.tabControlDocs.SelectedItem = newTab;
        }
        private bool IsTabWithExistWithName(string name)
        {
            foreach (TabItem tabItem in this._tabItems)
            {
                if (tabItem.Name.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        private TabItem AddTabItemWithName(string name)
        {
            int count = this._tabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();
            tab.Header = name;
            tab.Name = name;
            tab.HeaderTemplate = this.tabControlDocs.FindResource("TabHeader") as DataTemplate;

            // add controls to tab item, this case I added just a textbox
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.AcceptsReturn = true;
            richTextBox.AcceptsTab = true;
            richTextBox.Name = "godc";
            richTextBox.AppendText(this._currentProject);
            richTextBox.Tag = count;
            richTextBox.TextChanged += onRichTextBoxTextChanged;
            Colorizer.Colorize(richTextBox, this);
            ReplaceTabCaharactersOnRichTextBox(richTextBox);
            tab.Content = richTextBox;
            this._lastTabItem = tab;

            // insert tab item right before the last (+) tab item
            this._tabItems.Insert(count - 1, tab);
            return tab;
        }
        private void onTabControlsDocsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = this.tabControlDocs.SelectedItem as TabItem;

            if (tab != null && tab.Header != null)
            {
                if (tab.Header.Equals(this._addTabHeader))
                {
                    // clear tab control binding
                    this.tabControlDocs.DataContext = null;

                    // add new tab
                    int count = this._tabItems.Count;
                    string name = string.Format("Document{0}", count);
                    this._currentProject = GenerateStartProgram();
                    TabItem newTab = this.AddTabItemWithName(name);

                    // bind tab control
                    this.tabControlDocs.DataContext = _tabItems;

                    // select newly added tab item
                    this.tabControlDocs.SelectedItem = newTab;
                }
                else
                {
                    this._lastTabItem = tab;
                    this._currentProject = GetTextFromCurrentTab();
                    // your code here...
                }
            }
        }

        private const string TAB_PLACEHOLDER = "===TAB===";
        private void ReplaceTabCaharactersOnRichTextBox(RichTextBox richTextBox)
        {
          /*  string xaml = richTextBox.;

            xaml = xaml.Replace(TAB, TAB_PLACEHOLDER);

            richTextBox2.Xaml = xaml;

            foreach (Block block in richTextBox2.Blocks)
            {
                foreach (Inline inline in ((Paragraph)block).Inlines)
                {
                    ((Run)inline).Text = ((Run)inline).Text.Replace(TAB_PLACEHOLDER, TAB);
                }
            }*/
        }

        private void CloseTabAndDocument(TabItem tab)
        {
            if (tab != null)
            {
                if (_tabItems.Count < 3)
                {
                    MessageBox.Show("Cannot close last document!");
                }
                else if (MessageBox.Show(string.Format("Are you sure you want to close '{0}'?", tab.Header.ToString()),
                    "Closing document", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // get selected tab
                    TabItem selectedTab = this.tabControlDocs.SelectedItem as TabItem;

                    // clear tab control binding
                    this.tabControlDocs.DataContext = null;

                    _tabItems.Remove(tab);

                    // bind tab control
                    this.tabControlDocs.DataContext = _tabItems;

                    // select previously selected tab. if that is removed then select first tab
                    if (selectedTab == null || selectedTab.Equals(tab))
                    {
                        selectedTab = _tabItems[0];
                    }
                    this.tabControlDocs.SelectedItem = selectedTab;
                }
            }
        }
        #endregion

        //ACTIONS
        #region Actions
        public void onRichTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this._currentProject = GetTextFromCurrentTab();
            Colorizer.Colorize((RichTextBox)this._lastTabItem.Content, this);
        }
        private void onUndoButtonPressed(object sender, RoutedEventArgs e)
        {
            ((RichTextBox)this._lastTabItem.Content).Undo();
        }

        private void onRedoButtonPressed(object sender, RoutedEventArgs e)
        {
            ((RichTextBox)this._lastTabItem.Content).Redo();
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
           // DragMove();
        }

        private void onExitButtonPressed(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to close GOD-C CREATOR?"),
                   "Closing document", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void onNewDocumentButtonPressed(object sender, RoutedEventArgs e)
        {
            int count = this._tabItems.Count;
            string name = string.Format("Document{0}", count);
            this._currentProject = GenerateStartProgram();
            CreateNewTabeWithName(name);
        }

        private void onOpenDocumentButtonPressed(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "God-C files (.godc)|*.godc|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                System.IO.Stream fileStream = openFileDialog1.OpenFile();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    // Read the first line from the file and write it the textbox.
                    this._currentProject = reader.ReadToEnd();
                    string fileName = openFileDialog1.SafeFileName.Substring(0, openFileDialog1.SafeFileName.IndexOf(".", 0));

                    if (!IsTabWithExistWithName(fileName))
                    {
                        CreateNewTabeWithName(fileName);
                        Colorizer.Colorize((RichTextBox)this._lastTabItem.Content, this);
                    }
                    else
                    {
                        MessageBox.Show("File with this name already opened!");
                    }
                }
                fileStream.Close();
            }
        }

        private void onSaveDocumentButtonPressed(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = this._lastTabItem.Name; // Default file name
            dlg.DefaultExt = ".godc"; // Default file extension
            dlg.Filter = "Text documents (.godc)|*.godc"; // Filter files by extension


            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                StreamWriter file = new StreamWriter(dlg.OpenFile());
                file.WriteLine(this._currentProject);
                file.Close();
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void onCloseDocumentButtonPressed(object sender, RoutedEventArgs e)
        {
            CloseTabAndDocument(this._lastTabItem);
        }

        private void onCompileButtonPressed(object sender, RoutedEventArgs e)
        {

        }
        private void onButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = this.tabControlDocs.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

            TabItem tab = item as TabItem;

            CloseTabAndDocument(tab);
        }
        #endregion
    }
}
