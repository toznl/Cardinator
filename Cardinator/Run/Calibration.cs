using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace Cardinator.Run
{
    class Calibration
    {
        public void Run()
        {
            #region Socket Listner
            //Fundamental Variables for Socket
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 5000);
            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sListener.Bind(ipEndPoint);
            sListener.Listen(1024);
            #endregion

            while (true)
            {

            }
        }
    }
}
