using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CashGenicClient
{
    public class SystemSettings
    {

        public string cgURL { get; set; }
        public string portNumber { get; set; }

        public string APIUsername { get; set; }
        public string APIPassword { get; set; }


        Windows.Storage.ApplicationDataContainer localSettings =
            Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;



        public SystemSettings()
        {

            if(localSettings.Values["cgURL"] == null)
            {
                cgURL = "192.168.0.0";
            }
            else
            {
                cgURL = localSettings.Values["cgURL"].ToString();
            }
            if (localSettings.Values["portNumber"] == null)
            {
                portNumber = "9000";
            }
            else
            {
                portNumber = localSettings.Values["portNumber"].ToString();
            }

            if (localSettings.Values["apiUserName"] == null)
            {
                APIUsername = "APIUserName";
            }
            else
            {
                APIUsername = localSettings.Values["apiUserName"].ToString();
            }



            if (localSettings.Values["apiPassword"] == null)
            {
                APIPassword = "APIPassword";
            }
            else
            {
                APIPassword = localSettings.Values["apiPassword"].ToString();
            }


        }


        public void Save()
        {

            localSettings.Values["cgURL"] = cgURL;
            localSettings.Values["portNumber"] = portNumber;
            localSettings.Values["apiUserName"] = APIUsername;
            localSettings.Values["apiPassword"] = APIPassword;

        }


        public void TokenInvalidate()
        {
            DateTime d = DateTime.Now.AddYears(-1);
            localSettings.Values["access_token_expire"] = d.ToString();
            localSettings.Values["access_token"] = "";
        }



    }
}
