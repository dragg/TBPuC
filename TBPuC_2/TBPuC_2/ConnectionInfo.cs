using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Service
{
    class ConnectionInfo
    {
        public Brush Color { get { return new SolidColorBrush(connection ? Colors.Blue : Colors.Red); } }

        public void ToConnect()
        {
            connection = !connection;
        }

        private bool connection = false;
        public string Connection
        {
            get { return !connection ? "Закрыто" : "Открыто"; }
        }

        private IPAddress ipAddress = null;
        public string IP { get { return connection ? ipAddress.ToString() : ""; } }

        private int port;
        public int Port 
        {
            get { return port; }
            set { port = value; }
        }

        public ConnectionInfo(bool con, IPAddress address, int port)
        {
            connection = con;
            ipAddress = address;
            this.port = port;
        }
    }
}
