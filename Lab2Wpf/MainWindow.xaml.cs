using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32; //savefiledialog
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel.VM vm = new ViewModel.VM(new UI()); //две привязки пришлось делать в коде
            DataContext = vm;//new ViewModel.VM(new UI());
            winFormsHost.Child = vm.Chart; //в xaml эта привязка не работает
            vm.SelectedItems = AllCollectionListBox.SelectedItems; //в xaml нет свойства SelectedItems для привязки

        }
    } 
    public class UI: ViewModel.IServices
    {
        public bool UISaveMessage()
        {
            return MessageBox.Show("Project has been changed. Do you want to save it?", "Saving", MessageBoxButton.YesNo).ToString() == "Yes";
        }
        public bool UIChooseSaveLoc(ref string str)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            bool? save;
            save = saveFileDialog.ShowDialog();
            if (save == true)
            {
                str = saveFileDialog.FileName;
                return true;
            }
            return false;
        }
        public bool UIChooseOpenLoc(ref string str)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? open;
            open = openFileDialog.ShowDialog();
            if (open == true)
            {
                str = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void UICustomMessage(string header, string body)
        {
            MessageBox.Show(body, header);
        }
    }
}
