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
using System.Windows.Shapes;
using NBMFS.ViewModels;
namespace NBMFS.Views
{
    /// <summary>
    /// Interaction logic for ShowAllMessagesViewModels.xaml
    /// </summary>
    public partial class ShowAllMessagesViewModels : Window
    {
        public ShowAllMessagesViewModels()
        {
            InitializeComponent();
            this.DataContext = new ShowMessagesViewModel();
        }
    }
}
