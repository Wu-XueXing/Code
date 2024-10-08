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

namespace Happynowyear
{
    /// <summary>
    /// Bomber.xaml 的交互逻辑
    /// </summary>
    public partial class Bomber : UserControl
    {
        public Bomber()
        {
            InitializeComponent();
        }
        public bool haveChild { get; set; }
        public double positionToTop { get; set; }
        public double positionToLeft { get; set;}
    }
}
