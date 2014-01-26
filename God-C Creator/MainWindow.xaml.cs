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

namespace God_C_Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg,
                int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        public MainWindow()
        {
            InitializeComponent();
        }


        //ACTIONS

        private void onUndoButtonPressed(object sender, RoutedEventArgs e)
        {

        }

        private void onRedoButtonPressed(object sender, RoutedEventArgs e)
        {

        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
           // DragMove();
        }

        private void onExitButtonPressed(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void onNewDocumentButtonPressed(object sender, RoutedEventArgs e)
        {

        }

        private void onOpenDocumentButtonPressed(object sender, RoutedEventArgs e)
        {

        }

        private void onSaveDocumentButtonPressed(object sender, RoutedEventArgs e)
        {

        }

        private void onCloseDocumentButtonPressed(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
