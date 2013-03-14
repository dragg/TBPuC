using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Library
{
    public class ServerBroadcast
    {
        #region public events

        public event EventHandler<ServerAddedEventArgs> ServerAdded;

        #endregion

        #region public properties

        public Int32 UdpPort
        {
            get
            {
                return _udpPort;
            }
            set
            {
                _udpPort = value;
            }
        }

        public List<IPEndPoint> AvailableEndPoints
        {
            get { return _availableEndPoints; }
        }

        public IPAddress[] ServerAddreses
        {
            get;
            set;
        }

        #endregion

        #region public methods

        public void NotifyClients()
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, UdpPort);

            String infoString = String.Format("I am Service");
            Byte[] bytes = Encoding.Unicode.GetBytes(infoString);
            udpClient.Send(bytes, bytes.Length, endPoint);
            udpClient.Close();
        }

        public void StartListeningServers()
        {
            _listeningServersThread = new Thread(ListeningServersMethod);
            _listeningServersThread.IsBackground = true;
            _listeningServersThread.Name = "Listening servers";
            _listeningServersThread.Start();
        }

        #endregion

        #region private methods

        private void ListeningServersMethod()
        {
            UdpClient udpClient = new UdpClient(UdpPort);
            IPEndPoint endPoint = null;
            while (true)
            {
                Byte[] receiveBytes = udpClient.Receive(ref endPoint);

                String info = Encoding.Unicode.GetString(receiveBytes);
                if (info == "I am Service")
                {
                    AddToList(endPoint);
                }
            }
        }

        private Boolean AddToList(IPEndPoint endPoint)
        {
            foreach (IPEndPoint point in AvailableEndPoints)
                if (point.Address.Equals(endPoint.Address))
                    return false;
            endPoint.Port = 2013;
            AvailableEndPoints.Add(endPoint);
            return true;
        }

        #endregion

        #region private fields

        private Thread _listeningServersThread;
        private readonly List<IPEndPoint> _availableEndPoints = new List<IPEndPoint>();
        private Int32 _udpPort = 2013;

        #endregion
    }

    public class ServerAddedEventArgs : EventArgs
    {
        public ServerAddedEventArgs(IPEndPoint endPoint, String info)
        {
            EndPoint = endPoint;
            Info = info;
        }

        public readonly IPEndPoint EndPoint;
        public readonly String Info;
    }
}
