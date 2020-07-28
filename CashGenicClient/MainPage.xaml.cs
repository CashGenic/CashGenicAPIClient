using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
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
        private string requestValue;
        private UIState uIState;
        private SessionPayment sessionPayment;


        private enum UIState
        {
            Startup,
            Connecting,
            Connected,
            Idle,
            Waiting,
            Cancelling,
            PayoutError,
            PaymentMade,
            RefundMade,
            Complete,
            Restore
        };



        public MainPage()
        {
            this.InitializeComponent();

            sessionPayment = new SessionPayment();
            cashGenicSystem = new CashGenicSystem();
            uIState = UIState.Startup;

            cashGenicSystem.NewSystemConnected += sys_NewSystemConnection;
            //cashGenicSystem.NewSystemError += sys_NewSystemError;
            cashGenicSystem.NewSystemEvents += sys_NewSystemEvents;
            _ = Connect();


        }

        private void sys_NewSystemConnection(object sender, CashGenicSystem.NewSystemConnectedArgs e)
        {

        }


        private void sys_NewSystemEvents(object sender, CashGenicSystem.NewSystemEventArgs e)
        {

            foreach (Event ev in e.systemEvents)
            {
                SetUIEvents(ev);
            }
        }



        private void SetUIEvents(Event ev)
        {

            Debug.WriteLine(ev.EventEvent + " " + ev.Value.ToString() + " " + ev.timestamp);

            switch (ev.EventEvent)
            {
                case "Idle":
                    // show the key pad
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        SetUIState(UIState.Idle);
                    });
                    break;
                case "Waiting":

                    // hide the key pad
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if(sessionPayment == null)
                        {
                            sessionPayment = new SessionPayment();
                        }
                        sessionPayment.PaymentRequest = (int)ev.Value;
                        SetUIState(UIState.Waiting);

                    });

                    break;
                case "CancelPayment":
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        SetUIState(UIState.Cancelling);
                    });
                    break;
                case "PayOutError":
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        SetUIState(UIState.PayoutError);
                    });
                    break;
                case "Payment":
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        sessionPayment.PaymentMade = (int)ev.Value;
                        SetUIState(UIState.PaymentMade);
                    });
                    break;
                case "Refund":
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        sessionPayment.RefundMade = (int)ev.Value;
                        SetUIState(UIState.RefundMade);
                    });
                    break;
                case "PaymentComplete":
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        SetUIState(UIState.Complete);

                    });
                    break;
                case "RestoreTransaction":
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        sessionPayment.PaymentMade = (int)ev.Value;
                        SetUIState(UIState.Restore);

                    });
                    break;
            }


        }







        private void AddValue(string v)
        {

            requestValue = requestValue + v;
            txtValueRequest.Text = requestValue;
            DisplayValue(requestValue,txtValueRequest);

        }


        private void DisplayValue(string dValue, TextBlock txt)
        {

            double c = Convert.ToDouble(dValue) / 100;
            txt.Text = c.ToString("C", CultureInfo.CurrentCulture);

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
                    if (requestValue.Length > 0)
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

            sessionPayment = new SessionPayment();
            sessionPayment.PaymentRequest = Convert.ToInt32(requestValue);


            txtValueRequest.Foreground = App.Current.Resources["systemTextColor"] as SolidColorBrush;
            SystemResponse rsp = await cashGenicSystem.StartPaymentSession(Convert.ToInt32(requestValue));
            if (rsp == SystemResponse.OK)
            {
                
            }
            else
            {
                double v = Convert.ToDouble(requestValue) / 100;
                txtValueRequest.Text = rsp.ToString() + " " + v.ToString("C", CultureInfo.CurrentCulture);
                txtValueRequest.Foreground = App.Current.Resources["errorTextColor"] as SolidColorBrush;

            }


        }




        private void SetUIState(UIState ui)
        {
            // no action if the state is unchanged
            if(ui == uIState && ui != UIState.PaymentMade && ui != UIState.RefundMade)
            {
                return;
            }

            switch (ui)
            {
                case UIState.Connecting:
                    prg1.IsActive = true;
                    txtInfo.Text = "Connecting to CASHGENIC...";
                    requestValue = "";
                    DisplayValue("0", txtValueRequest);
                    gridValue.Visibility = Visibility.Collapsed;
                    txtPayRequested.Text = "";
                    txtPaymentMade.Text = "";
                    txtChangePaid.Text = "";
                    bttnCancel.Visibility = Visibility.Collapsed;
                    bttnDone.Visibility = Visibility.Collapsed;
                    break;
                case UIState.Idle:
                    gridValue.Visibility = Visibility.Visible;
                    prg1.IsActive = false;                    
                    txtPayRequested.Text = "Ready";
                    txtValueRequest.Text = "";
                    txtChangePaid.Text = "";
                    txtPaymentMade.Text = "";                    
                    txtInfo.Text = "";
                    break;
                case UIState.Waiting:
                    requestValue = "";
                    DisplayValue("0", txtValueRequest);
                    gridValue.Visibility = Visibility.Collapsed;
                    bttnCancel.Visibility = Visibility.Visible;
                    bttnDone.Visibility = Visibility.Collapsed;
                    prg1.IsActive = true;
                    txtInfo.Text = "Waiting for payment";
                    double c = Convert.ToDouble(sessionPayment.PaymentRequest) / 100;
                    txtPayRequested.Text = "Requested " + c.ToString("C", CultureInfo.CurrentCulture);
                    break;
                case UIState.Cancelling:
                    bttnCancel.Visibility = Visibility.Collapsed;
                    txtInfo.Text = "Cancelling payment...";
                    prg1.IsActive = true;
                    break;
                case UIState.Complete:
                    prg1.IsActive = false;
                    txtInfo.Text = "Session Complete";
                    bttnDone.Visibility = Visibility.Visible;
                    break;
                case UIState.PayoutError:
                    prg1.IsActive = false;
                    txtInfo.Text = "Unable to payout change. Please cancel";
                    bttnCancel.Visibility = Visibility.Visible;
                    break;
                case UIState.PaymentMade:
                    double c_made = Convert.ToDouble(sessionPayment.PaymentMade) / 100;
                    txtPaymentMade.Text = "Paid " + c_made.ToString("C", CultureInfo.CurrentCulture);
                    break;
                case UIState.RefundMade:
                    double c_refund = Convert.ToDouble(sessionPayment.RefundMade) / 100;
                    txtChangePaid.Text = "Refund Paid " + c_refund.ToString("C", CultureInfo.CurrentCulture);
                    break;
                case UIState.Restore:
                    double c_restore = Convert.ToDouble(sessionPayment.PaymentMade) / 100;
                    txtChangePaid.Text = "Paid " + c_restore.ToString("C", CultureInfo.CurrentCulture);
                    bttnCancel.Visibility = Visibility.Visible;
                    txtInfo.Text = "Control restore. Cancel session";
                    break;


            }


            // set current state
            uIState = ui;

        }



        private async Task Connect()
        {

            SetUIState(UIState.Connecting);            
            int ret = await cashGenicSystem.StartUp();
            if (ret != 0)
            {
                txtInfo.Text = "Unable to connect to CashGenic System!";
            }

            prg1.IsActive = false;


        }

        private void bttnCancel_Click(object sender, RoutedEventArgs e)
        {

            cashGenicSystem.CancelPayment();
                       
        }

        private void bttnDone_Click(object sender, RoutedEventArgs e)
        {
            bttnDone.Visibility = Visibility.Collapsed;
            cashGenicSystem.CloseSession();
        }

        private void txtSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(SettingsPage));

        }
    }
}
