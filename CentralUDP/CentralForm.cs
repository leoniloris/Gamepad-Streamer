using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CentralUDP
{
    public partial class CentralForm : Form
    {
        struct ClientInfo {
            public EndPoint endpoint;
            public byte id;
            public byte hostid;
            public byte[] cntr;
        }
        struct HostInfo
        {
            public EndPoint endpoint;
            public IList<ClientInfo> clientList;
        }
        IList<HostInfo> hostList;
        Socket serverSocket;
        byte[] receivedData = new byte[10];

        public CentralForm()
        {
            InitializeComponent();
            hostList = new List<HostInfo>();
        }

        private void CentralForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1000);
            serverSocket.Bind(ipEndPoint);

            EndPoint epSender =(EndPoint) new IPEndPoint(IPAddress.Any, 1000);

            serverSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
            
        }
        private void OnReceive(IAsyncResult ar)
        {
            EndPoint epSender = (EndPoint) new IPEndPoint(IPAddress.Any, 1000);
            serverSocket.EndReceiveFrom(ar, ref epSender);

            switch ((char)receivedData[0])
            {
                case 'l': // login client
                    if (hostList.Count>0 && hostList.Count < receivedData[3]) goto nao;
                    else foreach (ClientInfo cl in hostList[receivedData[3]].clientList) 
                        if(cl.id == receivedData[2] ) goto nao;


                    ClientInfo clientInfo = new ClientInfo();
                    clientInfo.endpoint = epSender;
                    clientInfo.id = (byte)(hostList[receivedData[3]].clientList.Count-1);
                    clientInfo.hostid = receivedData[3];
                    clientInfo.cntr = new byte[2];
                    hostList[receivedData[3]].clientList.Add(clientInfo);
                    for (int q = 1; q < 1000; q++) serverSocket.SendTo(new byte[3] { (byte)'O', (byte)'K', clientInfo.id }, epSender);
                    nao:;
                    break;
                case 'h': // login host
                    //foreach (HostInfo ht in hostList) // for now, 254 hosts can login
                    if (hostList.Count == 254) goto nao;


                    HostInfo host= new HostInfo();
                    host.endpoint = epSender;
                    host.clientList = new List<ClientInfo>();
                    hostList.Add(host);
                    for (int q = 1; q < 1000; q++) serverSocket.SendTo(new byte[3] { (byte)'O', (byte)'K',(byte) (hostList.Count-1) }, epSender);
                    break;
                case 'q': // cliente saiu
                    int nIndex = 0;
                    foreach (ClientInfo client in hostList[receivedData[3]].clientList)
                    {
                        if (client.id == receivedData[2])
                        {
                            hostList[receivedData[3]].clientList.RemoveAt(nIndex);
                            break;
                        }
                        ++nIndex;
                    }
                    break;
                case 'c': // cntrol packet received, forward it to its host 
                    serverSocket.SendTo(new byte[] { (byte)'c', (byte)'_',receivedData[2], receivedData[4], receivedData[5] },hostList[receivedData[3]].endpoint);
                    break;
            }
            serverSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);

        }
    }
}
