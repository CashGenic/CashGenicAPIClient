using System;
using System.Collections.Generic;
using System.Globalization;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CashGenicClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EnterValue : Page
    {

        private CashGenicSystem cashGenicSystem;
        private string requestValue;

        public EnterValue()
        {
            this.InitializeComponent();
            requestValue = "";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.cashGenicSystem = (CashGenicSystem)e.Parameter;
        }


        private void _1_Click(object sender, RoutedEventArgs e)
        {

            Button bttn = (Button)sender;
            switch (bttn.Name)
            {

                case "_1":
                    AddValue("1");
                    break;
                case "_2":
                    AddValue("2");
                    break;
                case "_3":
                    AddValue("3");
                    break;
                case "_4":
                    AddValue("4");
                    break;
                case "_5":
                    AddValue("5");
                    break;
                case "_6":
                    AddValue("6");
                    break;
                case "_7":
                    AddValue("7");
                    break;
                case "_8":
                    AddValue("8");
                    break;
                case "_9":
                    AddValue("9");
                    break;
                case "_0":
                    AddValue("0");
                    break;
                case "_del":
                    if(requestValue.Length > 0)
                    {
                        requestValue = requestValue.Substring(0, requestValue.Length - 1);
                        if (requestValue == "")
                        {
                            txtValueRequest.Text = "";
                        }
                        else
                        {
                            double v = Convert.ToDouble(requestValue) / 100;
                            txtValueRequest.Text = v.ToString("C", CultureInfo.CurrentCulture);
                        }
                    }
                    break;
                case "_Enter":
                    _ = StartPayment();
                    break;

            }


        }


        private async Task StartPayment()
        {

            txtValueRequest.Foreground = App.Current.Resources["systemTextColor"] as SolidColorBrush;
            SystemResponse rsp = await cashGenicSystem.StartPaymentSession(Convert.ToInt32(requestValue));
            if(rsp == SystemResponse.OK)
            {
                this.Frame.Navigate(typeof(Payment), cashGenicSystem);
            }
            else
            {
                double v = Convert.ToDouble(requestValue) / 100;
                txtValueRequest.Text = rsp.ToString() + " " + v.ToString("C", CultureInfo.CurrentCulture); 
                txtValueRequest.Foreground = App.Current.Resources["errorTextColor"] as SolidColorBrush;

            }


        }



        private void AddValue(string v)
        {

            requestValue = requestValue + v;
            txtValueRequest.Text = requestValue;
            double c = Convert.ToDouble(requestValue)/100;
            txtValueRequest.Text = c.ToString("C", CultureInfo.CurrentCulture);
        }

    }


}

