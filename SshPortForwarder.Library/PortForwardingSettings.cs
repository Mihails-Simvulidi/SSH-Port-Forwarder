namespace SshPortForwarder
{
    /// <summary>
    /// Port to be forwarded.
    /// </summary>
    public class PortForwardingSettings
    {
        /// <summary>
        /// Local IP address which will accept connections. Default: 127.0.0.1.
        /// </summary>
        public string LocalHost { get; set; } = "127.0.0.1";

        /// <summary>
        /// Local port which will accept connections.
        /// </summary>
        public uint LocalPort { get; set; }

        /// <summary>
        /// Remote host in SSH server's network to which incoming connections will be forwarded. Default: 127.0.0.1.
        /// </summary>
        public string RemoteHost { get; set; } = "127.0.0.1";

        /// <summary>
        /// Remote port to which incoming connections will be forwarded.
        /// </summary>
        public uint RemotePort { get; set; }

        /// <summary>
        /// Proceed if port is already busy. Default: false.
        /// </summary>
        public bool IgnorePortInUseError { get; set; }
    }
}
