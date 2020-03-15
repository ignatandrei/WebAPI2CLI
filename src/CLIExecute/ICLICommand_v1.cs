using System.Threading.Tasks;

namespace CLIExecute
{
    /// <summary>
    /// Describes the command to be executed
    /// </summary>
    public interface ICLICommand
    {
        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        /// <value>
        /// The name of the command.
        /// </value>
        string NameCommand { get; set; }
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        string Host { get; set; }
        /// <summary>
        /// Gets or sets the relative request URL.
        /// </summary>
        /// <value>
        /// The relative request URL.
        /// </value>
        string RelativeRequestUrl { get; set; }
        /// <summary>
        /// Gets or sets the verb.
        /// </summary>
        /// <value>
        /// The verb.
        /// </value>
        string Verb { get; set; }

        /// <summary>
        /// Sets the possible full hosts . 
        /// Usefull if you want just pass the start of the host
        /// e.g. http
        /// </summary>
        /// <param name="adresses">The adresses.</param>
        void SetPossibleFullHosts(params string[] adresses);

    }
    /// <summary>
    /// first type of commands
    /// </summary>
    /// <seealso cref="CLIExecute.ICLICommand" />
    public interface ICLICommand_v1: ICLICommand
    {

        /// <summary>
        /// Gets or sets the data to send.
        /// </summary>
        /// <value>
        /// The data to send.
        /// </value>
        string DataToSend { get; set; }
        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        Task<ReturnValue_v1> Execute();
    }
}