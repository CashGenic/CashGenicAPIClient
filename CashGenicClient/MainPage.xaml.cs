using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CashGenicClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private CashGenicSystem cashGenicSystem;



        public MainPage()
        {
            this.InitializeComponent();


            cashGenicSystem = new CashGenicSystem();
            cashGenicSystem.NewSystemConnected += sys_NewSystemConnection;
            cashGenicSystem.NewSystemError += sys_NewSystemError;
            cashGenicSystem.NewSystemEvents += sys_NewSystemEvents;

            txtVersion.Text = "";
            bttnStart.Visibility = Visibility.Collapsed;
            txtError.Text = "";
            


            prg1.IsActive = true;
            txtInfo.Text = "Connecting to CASHGENIC...";


        }

        private void sys_NewSystemEvents(object sender, CashGenicSystem.NewSystemEventArgs e)
        {

        }

        private void sys_NewSystemError(object sender, CashGenicSystem.NewSystemErrorArgs e)
        {

            txtError.Text = e.systemError + " " + e.systeErrorData;

        }

        private void sys_NewSystemConnection(object sender, CashGenicSystem.NewSystemConnectedArgs e)
        {
            txtVersion.Text = "CashGenic Control system detected version: "  +  e.system.SystemVersion;
            bttnStart.Visibility = Visibility.Visible;
        }


        private async Task Connect()
        {
            txtError.Text = "";
            bttnStart.Visibility = Visibility.Collapsed;
            int ret = await cashGenicSystem.StartUp();
            if(ret == 0)
            {
                txtInfo.Text = "CASHGENIC connected";

            }
            else
            {
                txtInfo.Text = "Unable to connect to CashGenic System!";
            }


            prg1.IsActive = false;


        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

               _ = Connect();

        }

        private void bttnStart_Click(object sender, RoutedEventArgs e)
        {
            cashGenicSystem.valueLoaded = true;
            this.Frame.Navigate(typeof(EnterValue), cashGenicSystem);
        }
    }
}
