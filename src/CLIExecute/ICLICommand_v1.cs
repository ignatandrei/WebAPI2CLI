using System.Threading.Tasks;

namespace CLIExecute
{
    public interface ICLICommand
    {
        string NameCommand { get; set; }
        string Host { get; set; }
        string RelativeRequestUrl { get; set; }
        string Verb { get; set; }

        void SetPossibleFullHosts(params string[] adresses);

    }
    public interface ICLICommand_v1: ICLICommand
    {
        
        string DataToSend { get; set; }        
        Task<ReturnValue_v1> Execute();
    }
}