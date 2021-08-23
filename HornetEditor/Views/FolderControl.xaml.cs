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

namespace HornetEditor.Views
{
    /// <summary>
    /// Interaction logic for FolderControl.xaml
    /// </summary>
    public partial class FolderControl : UserControl
    {
        public FolderControl()
        {
            InitializeComponent();
            //this.Background = HornetEditor.Util.HornetColors.AppDarkBackground.GetBrush();
            this.ContentGrid.Background = HornetEditor.Util.HornetColors.ControlMediumGrayBackground.GetBrush();
            this.FilesPanel.Background = HornetEditor.Util.HornetColors.ControlLightGrayBackground.GetBrush();
        }

        

        public void AddFile(string path)
        {
            Controls.FileTile ft = new Controls.FileTile();
            ft.Width = 152;
            ft.Height = 152;
            this.FilesPanel.Children.Add(ft);
        }
    }
}
