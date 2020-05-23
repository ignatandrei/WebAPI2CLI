using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CLIExecute
{
    /// <summary>
    /// First type of commands
    /// </summary>
    /// <seealso cref="CLIExecute.ICLICommand_v1" />
    public class CLICommand_v1 : ICLICommand_v1
    {
        private string[] possibleFullAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="CLICommand_v1"/> class.
        /// </summary>
        public CLICommand_v1()
        {

        }
        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        /// <value>
        /// The name of the command.
        /// </value>
        public string NameCommand { get; set; }
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; set; }
        /// <summary>
        /// Gets or sets the relative request URL.
        /// </summary>
        /// <value>
        /// The relative request URL.
        /// </value>
        public string RelativeRequestUrl { get; set; }
        /// <summary>
        /// Gets or sets the verb.
        /// </summary>
        /// <value>
        /// The verb.
        /// </value>
        public string Verb { get; set; }
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; set; } = "application/json";
        /// <summary>
        /// Gets or sets the data to send.
        /// </summary>
        /// <value>
        /// The data to send.
        /// </value>
        public string DataToSend { get; set; }



        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">for the moment, cannot work with {Verb.ToUpper()}</exception>
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
            Console.WriteLine($"host : {Host}");
            Host = Host.Replace("0.0.0.0", "localhost");
            Host = Host.Replace("[::]", "localhost");
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

        /// <summary>
        /// Sets the possible full hosts .
        /// Usefull if you want just pass the start of the host
        /// e.g. http
        /// </summary>
        /// <param name="adresses">The adresses.</param>
        public void SetPossibleFullHosts(params string[] adresses)
        {
            this.possibleFullAddress = adresses;
        }
    }
}
