using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CLIExecute
{
    public class CLICommand_v1 : ICLICommand_v1
    {
        private string[] possibleFullAddress;

        public CLICommand_v1()
        {

        }
        public string NameCommand { get; set; }
        public string Host { get; set; }
        public string RelativeRequestUrl { get; set; }
        public string Verb { get; set; }
        public string ContentType { get; set; } = "application/json";
        public string DataToSend { get; set; }

        

        public async Task<ReturnValue_v1> Execute()
        {
            if (!Uri.TryCreate(Host,UriKind.Absolute, out Uri newUri))
            {
                if(possibleFullAddress.Length == 1)
                {
                    Host = possibleFullAddress[0];
                }
                else
                {
                    foreach (var item in possibleFullAddress)
                    {
                        if (item.StartsWith(Host))
                        {
                            Host = item;
                            continue;
                        }
                    }
                }
            }
            var h = new HttpClient();

            h.BaseAddress = new Uri(Host);
            
            StringContent sc=null;
            if (!string.IsNullOrWhiteSpace(DataToSend))
            {
                sc = new StringContent(DataToSend,Encoding.UTF8,ContentType);
            }
            ;
            HttpResponseMessage responseMessage;
            switch (Verb.ToUpper())
            {
                case "POST":
                    responseMessage = await h.PostAsync(RelativeRequestUrl, sc);
                    break;
                case "GET":
                    responseMessage = await h.GetAsync(RelativeRequestUrl);
                    break;
                case "DELETE":
                    responseMessage = await h.DeleteAsync(RelativeRequestUrl);
                    break;
                case "PUT":
                    responseMessage = await h.PutAsync(RelativeRequestUrl, sc);
                    break;
                default:
                    throw new ArgumentException($"for the moment, cannot work with {Verb.ToUpper()}");
            }

            var data = await responseMessage.Content.ReadAsStringAsync();
            return new ReturnValue_v1(responseMessage.StatusCode, data).FromCommand(this);

        }

        public void SetPossibleFullHosts(params string[] adresses)
        {
            this.possibleFullAddress = adresses;
        }
    }
}
