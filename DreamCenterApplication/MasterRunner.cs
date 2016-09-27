using CommonLib;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamCenterApplication
{
    public class MasterRunner : IRunner
    {
        private TCP_Server _server;
        private Dictionary<string, MessageUtility.MessageHandlerTCP> _handler;
        private List<Household> _households;
        private object _housesLock;

        public MasterRunner()
        {
            // Initialize vars
            _server = new TCP_Server();
            _handler = new Dictionary<string, MessageUtility.MessageHandlerTCP>();
            _households = new List<Household>();
            _housesLock = new object();

            // Wire server events
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.MessageReceived += OnMessageReceived;

            // Instantiate incoming message handlers
            _handler.Add(typeof(NotifyHouseholdUpdated).ToString(), HandleHouseholdUpdated);
            _handler.Add(typeof(SynchronizeRequest).ToString(), HandleSynchronizeRequest);
        }

        #region Incoming message handlers
        private void HandleHouseholdUpdated(object data, TCP_Message tcpMsg)
        {
            // Translate incoming data
            NotifyHouseholdUpdated message = data as NotifyHouseholdUpdated;

            // Update our own family list
            lock (_housesLock)
            {
                Household house = _households.FirstOrDefault(h => h.ID == message.UpdatedHousehold.ID);
                if (house != null)
                {
                    house = message.UpdatedHousehold;
                }
                else
                {
                    _households.Add(message.UpdatedHousehold);
                }
            }

            // Tell everyone what has happened
            _server.SendAllBut(MessageUtility.BuildMessage(message), tcpMsg.Address, tcpMsg.ID);
        }
        private void HandleSynchronizeRequest(object data, TCP_Message tcpMsg)
        {

        }
        #endregion

        private void OnMessageReceived(object sender, TCP_Message e)
        {
            try
            {
                // Make sure the message is complete
                if (e.Message.Length < 1)
                    return;

                // Translate incoming message
                string className;
                object message = MessageUtility.UnpackMessage(e.Message, out className);

                // Handle incoming message
                MessageUtility.MessageHandlerTCP msgHandler;
                if (_handler.TryGetValue(className, out msgHandler))
                    msgHandler(message);
            }
            catch (Exception exp)
            {
                // TODO: Log the problem and continue
            }
        }

        private void OnClientDisconnected(object sender, TCP_Server_Client e)
        {
            throw new NotImplementedException();
        }

        private void OnClientConnected(object sender, TCP_Server_Client e)
        {
        }

        public void Start()
        {
            _server.Start(Properties.Settings.Default.MainPort);
        }
    }
}
