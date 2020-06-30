using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PestControlMapper.wpf.controls
{
    /// <summary>
    /// Interaction logic for DropDownControl.xaml
    /// </summary>
    public partial class DropDownControl : UserControl
    {
        public DropDownControl()
        {
            InitializeComponent();
        }

        private bool open = false;

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            open = !open;

            if (open)
            {
                ParentPanel.Height = ContentPanel.Height;
                PlusText.Text = "-";
            }
            else
            {
                ParentPanel.Height = 0;
                PlusText.Text = "+";
            }
        }
    }
}
