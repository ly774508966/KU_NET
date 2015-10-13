using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
namespace Kubility
{
    public abstract class AbstractNetUnit
    {
        Union<HttpUnit, Socket> m_ConnectUnit;

        protected AbstractNetUnit()
        {
            this.m_ConnectUnit = new Union<HttpUnit, Socket>();
        }

        protected HttpUnit CreateHttpConnect()
        {
            this.m_ConnectUnit.m_FirstValue = new HttpUnit();
            return m_ConnectUnit.m_FirstValue;

        }

        protected Socket CreateTcpConnect(AddressFamily family = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType ptype = ProtocolType.Tcp)
        {
            this.m_ConnectUnit.m_SecondValue = new Socket(family, socketType, ptype);
            return this.m_ConnectUnit.m_SecondValue;
        }

        protected Socket CreateUDPConnect(AddressFamily family = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType ptype = ProtocolType.Udp)
        {
            this.m_ConnectUnit.m_SecondValue = new Socket(family, socketType, ptype);
            return this.m_ConnectUnit.m_SecondValue;
        }

    }
}


