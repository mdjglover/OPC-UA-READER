using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Client;

namespace OPC_UA_Reader
{
    class OPCUAClient
    {
        private Session _session;

        public bool Connected
        {
            get
            {
                if (_session != null && _session.Connected)
                {
                    return true;
                }

                return false;
            }
        }

        public OPCUAClient(string ipAddress, string port, ApplicationConfiguration configuration=null)
        {
            if (configuration == null)
            {
                // Most basic configuration setup required to create session
                configuration = new ApplicationConfiguration();
                ClientConfiguration clientConfiguration = new ClientConfiguration();
                configuration.ClientConfiguration = clientConfiguration;
            }

            // Create an endpoint to connect to
            string serverURL = $"opc.tcp://{ipAddress}:{port}";

            try
            {
                // As the KEPServer instance I'm connecting to isn't using security, I've passed false as the second argument here.
                EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(serverURL, false);
                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // Session options
                // Sets whether or not the discovery endpoint is used to update the endpoint description before connecting.
                bool updateBeforeConnect = false;

                // Sets whether or not the domain in the certificate must match the endpoint used
                bool checkDomain = false;

                // The name to assign to the session
                string sessionName = configuration.ApplicationName;

                // The session's timeout interval
                uint sessionTimeout = 30 * 60 * 1000;

                // The identity of the user attempting to connect. This can be anonymous as is used here,
                // or can be specified by a variety of means, including username and password, certificate,
                // or token.
                UserIdentity user = new UserIdentity();

                // List of preferred locales
                List<string> preferredLocales = null;

                // Create the session
                Session session = Session.Create(
                    configuration,
                    endpoint,
                    updateBeforeConnect,
                    checkDomain,
                    sessionName,
                    sessionTimeout,
                    user,
                    preferredLocales
                ).Result;

                // If the session was successfully created, assign it
                if (session != null && session.Connected)
                {
                    _session = session;
                }
            }
            catch
            {
                return;
            }
        }

        public ServerStatusDataType GetServerStatus()
        {
            // Get the current DataValue object for the ServerStatus node
            NodeId nodeId = new NodeId(Variables.Server_ServerStatus);
            DataValue dataValue = _session.ReadValue(nodeId);

            // Unpack the ExtensionObject that the DataValue contains, then return ServerStatusDataType object
            // that represents the current server status
            ExtensionObject extensionObject = (ExtensionObject)dataValue.Value;
            ServerStatusDataType serverStatus = (ServerStatusDataType)extensionObject.Body;

            return serverStatus;
        }

        public DataValue GetValue(string tagAddress, ushort namespaceIndex)
        {
            // To read a value, you require the node's ID. This can be either its unique integer ID, or a string
            // identifier along with the namespace which that identifier belongs to. Integer IDs are most useful
            // for acquiring nodes defined in the OPC UA standard, such as the ServerStatus node. The namespace
            // of a tag may differ depending on the OPC server being used, with KEPServer having a tag namespace
            // of 2. The only namespace that is guaranteed to remain the same is namespace 0, which contains the
            // nodes defined in the OPC UA standard.

            // Here I am using the tag address in the format "Channel.Device.Tag", along with the KEPServer namespace
            // of 2 to read the tag's value. Both of the subsequent ways of creating a NodeId are equivalent.

            NodeId nodeId = new NodeId(tagAddress, namespaceIndex);
           // NodeId nodeId = new NodeId($"ns={namespaceIndex};s={tagAddress}");

            try
            {
                return _session.ReadValue(nodeId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        } 
    }
}
