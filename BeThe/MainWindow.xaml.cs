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
using BeThe.Util;
using BeThe.Worker;

namespace BeThe
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 작업실행
        private async void Run(WorkType workType)
        {
            mainGrid.IsEnabled = false;
            try
            {
                Manager mgr = new Manager();
                await mgr.Run(workType);
            }
            finally
            {
                mainGrid.IsEnabled = true;
            }
        }

        // Player_W 가져오기
        private void bt_Player_W_Click(object sender, RoutedEventArgs e)
        {
            Run(WorkType.Player_W);
        }

        // Schedule_W 가져오기
        private void bt_Shedule_W_Click(object sender, RoutedEventArgs e)
        {
            Run(WorkType.Player);
        }
    }
}
