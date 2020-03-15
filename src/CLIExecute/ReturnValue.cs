using System.Net;

namespace CLIExecute
{
    /// <summary>
    /// return value for WebAPI
    /// </summary>
    public class ReturnValue_v1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnValue_v1"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public ReturnValue_v1(HttpStatusCode statusCode):this(statusCode,null)
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnValue_v1"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The result.</param>
        public ReturnValue_v1(HttpStatusCode statusCode,string result)
        {
            StatusCode = statusCode;
            Result = result;
        }
        /// <summary>
        /// Gets the command that was executed
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public ICLICommand Command { get; private set; }

        /// <summary>
        /// Gets the status code received
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public string Result { get; private set; }

        
        internal ReturnValue_v1 FromCommand(ICLICommand cmd)
        {
            this.Command = cmd;
            return this;
        }
        
    }
}
