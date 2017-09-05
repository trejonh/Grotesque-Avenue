namespace Horse.Server.Core
{
    /// <summary>
    /// The type of messages to be sent/received
    /// </summary>
    public sealed class MessageType
    {
        public static readonly string Cmd = "COMM";
        public static readonly string Data = "DATA";
        public static readonly string Info = "INFO";
        public static readonly string Err = "ERRO";
    }
}
