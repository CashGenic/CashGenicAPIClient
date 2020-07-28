using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace CashGenicClient
{


    public enum SystemResponse
    {
        OK = 0,
        InSession = 10,
        OutOfSrvice = 11,
        InvalidAmount = 12,
        SessionNotActive = 13,
        SessionActive = 14,
        ConnectionError = 100,
        InvalidCredentials = 100,

    }


    public enum SystemError
    {
        no_connection = 100
    }




    public class CashGenicSystem
    {


        Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;


        private bool _runSystem = false;
        private bool _cancelPayment = false;
        public bool valueLoaded { get; set; }
        public bool payLoaded { get; set; }
        public bool _closeSession { get; set; }


        public CashGenicSystem()
        {
            _cancelPayment = false;
            _closeSession = false;
        }


        #region events
        /// New System is detected as connected       
        public class NewSystemConnectedArgs : EventArgs
        {
            public SystemDetail system;
        }
        public EventHandler<NewSystemConnectedArgs> NewSystemConnected; 
        protected virtual void OnNewSystemConnected(NewSystemConnectedArgs e)
        {
            EventHandler<NewSystemConnectedArgs> handler = NewSystemConnected;
            if(handler != null)
            {
                handler(this, e);
            }
        }
        // A system, error has been detected
        public class NewSystemErrorArgs : EventArgs
        {
            public SystemError systemError;
            public string systeErrorData;
        }
        public EventHandler<NewSystemErrorArgs> NewSystemError;
        protected virtual void OnNewSystemError(NewSystemErrorArgs e)
        {
            EventHandler<NewSystemErrorArgs> handler = NewSystemError;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        // System events
        public class NewSystemEventArgs : EventArgs
        {
            public List<Event> systemEvents;
        }
        public EventHandler<NewSystemEventArgs> NewSystemEvents;
        protected virtual void OnNewSystemEvents(NewSystemEventArgs e)
        {
            EventHandler<NewSystemEventArgs> handler = NewSystemEvents;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public async Task<int> StartUp()
        {

            // do we need a new access token
            if (localSettings.Values["access_token"] == null || localSettings.Values["access_token_expire"] == null
                || !IsTokenValid(Convert.ToDateTime(localSettings.Values["access_token_expire"].ToString())))
            {
              if(await GetAPIToken() == null)
                {
                   
                    return 1001;
                }
            }


            // get the device detail
            SystemDetail systemDetail = await CashGenicAPIService.GetSystem(localSettings.Values["access_token"].ToString());
            if(systemDetail == null)
            {
                return 1000;
            }
            else
            {
                // fire new system connected event
                NewSystemConnectedArgs args = new NewSystemConnectedArgs();
                args.system = systemDetail;
                OnNewSystemConnected(args);
                _ =Task.Factory.StartNew(() => RunSystem());

            }



            return 0;



        }


        public void CancelPayment()
        {

            _cancelPayment = true;

        }


        public void CloseSession()
        {

            _closeSession = true;

        }



        private async Task RunSystem()
        {

            _runSystem = true;
            while (_runSystem)
            {
                // check for token refresh required
                if (localSettings.Values["access_token"] == null || localSettings.Values["access_token_expire"] == null
                    || !IsTokenValid(Convert.ToDateTime(localSettings.Values["access_token_expire"].ToString())))
                {
                    if (await GetAPIToken() == null)
                    {
                        _runSystem = false;
                        NewSystemErrorArgs args = new NewSystemErrorArgs();
                        args.systemError = SystemError.no_connection;
                        OnNewSystemError(args);
                        break;
                    }
                }
                // get the status
                SystemStatus systemStatus = await CashGenicAPIService.GetStatus(localSettings.Values["access_token"].ToString());
                if(systemStatus == null)
                {
                    _runSystem = false;
                    NewSystemErrorArgs args = new NewSystemErrorArgs();
                    args.systemError = SystemError.no_connection;
                    OnNewSystemError(args);
                    break;
                }
                ParseStatus(systemStatus.Events);


                // any commands
                if (_cancelPayment)
                {
                    _cancelPayment = false;
                    SystemResponse systemResponse = await CashGenicAPIService.CancelSession(localSettings.Values["access_token"].ToString());
                }
                if (_closeSession)
                {
                    _closeSession = false;
                    SystemResponse systemResponse = await CashGenicAPIService.EndSession(localSettings.Values["access_token"].ToString());
                }


                // poll delay
                Thread.Sleep(500);

            }

        }


        public void StopSystem()
        {
            _runSystem = false;
        }



        private void ParseStatus(List<Event> events)
        {

            NewSystemEventArgs args = new NewSystemEventArgs();
            args.systemEvents = events;
            OnNewSystemEvents(args);

        }





        public async Task<SystemResponse> StartPaymentSession(int requestValue)
        {

            if (localSettings.Values["access_token"] == null || localSettings.Values["access_token_expire"] == null
                || !IsTokenValid(Convert.ToDateTime(localSettings.Values["access_token_expire"].ToString())))
            {
                if (await GetAPIToken() == null)
                {

                    return SystemResponse.InvalidCredentials;
                }
            }

            SystemResponse ret =  await CashGenicAPIService.StartSession(localSettings.Values["access_token"].ToString(), requestValue);
            return ret;

        }





        private async Task<ApiToken> GetAPIToken()
        {

            ApiToken apiToken = await CashGenicAPIService.GetToken(localSettings.Values["apiUserName"].ToString(), localSettings.Values["apiPassword"].ToString());
            if (apiToken != null)
            {

                DateTime d = DateTime.Now.AddMilliseconds(apiToken.expires_in);
                localSettings.Values["access_token_expire"] = d.ToString();
                localSettings.Values["access_token"] = apiToken.access_token;
            }

            return apiToken;

        }









        private bool IsTokenValid(DateTime expire)
        {

            return DateTime.Now < expire;

        }



    }
}
