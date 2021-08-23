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
using System.Diagnostics;

namespace HornetEditor.Controls
{
    /// <summary>
    /// Interaction logic for FileTile.xaml
    /// </summary>
    public partial class FileTile : UserControl
    {
        public String FilePath { get; private set; }
        //public FileTile(String path)
        //{
        //    InitializeComponent();
        //    FilePath = path;
        //}

        public FileTile()
        {
            InitializeComponent();
            FilePath = "notepad.exe";
        }

        public void Open()
        {
            Process p = Process.Start(FilePath);
            if(p == null)
            {
                throw new Exception($"Could not open process to path {FilePath}");
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = HornetEditor.Util.HornetColors.SelectDarkBackground.GetBrush();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = HornetEditor.Util.HornetColors.ControlLightGrayBackground.GetBrush();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Open();
        }
    }
}
