using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CG2
{
    public partial class MainWindow : Window
    {
        private void BufferStockCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DoubleBufferingCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void LightingMaterialsCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ObjectFrameCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void NormalsCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TexturesCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OrthographicProjectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void PerspectiveProjectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void NormalsSmoothingToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void BufferStockCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void DoubleBufferingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void LightingMaterialsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ObjectFrameCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void NormalsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void TexturesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void NormalsSmoothingToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                FileNameText.Text = filename;
            }
            
        }
    }
}
