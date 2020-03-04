using System.Net;

namespace CLIExecute
{
    public class ReturnValue_v1
    {
        public ReturnValue_v1(HttpStatusCode statusCode):this(statusCode,null)
        {
            
        }
        public ReturnValue_v1(HttpStatusCode statusCode,string result)
        {
            StatusCode = statusCode;
            Result = result;
        }
        public ICLICommand Command { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }
        public string Result { get; private set; }

        public ReturnValue_v1 FromCommand(ICLICommand cmd)
        {
            this.Command = cmd;
            return this;
        }
        
    }
}
