using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace PBL3
{
    class connectSERVER
    {

        public delegate void ServerDel2(object[] obj, string str);
        public ServerDel2 SerVerDel { get; set; }
        IPEndPoint ip;
        Socket server;
        public List<Socket> Listclient = new List<Socket>();
        object[] DV; //= new object[3];
        public bool Check = false;
        public void connect()
        {
            ip = new IPEndPoint(IPAddress.Any, 1308);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(ip);
            Thread Listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        Listclient.Add(client);
                        //client.Send(Serialize(new object[] {-1,client.RemoteEndPoint.ToString() }));
                        Thread Receive = new Thread(receive);
                        Receive.IsBackground = true;
                        Receive.Start(client);
                    }
                }
                catch
                {
                    // MessageBox.Show("loi listen");
                    ip = new IPEndPoint(IPAddress.Any, 1308);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }
            });
            Listen.IsBackground = true;
            Listen.Start();
        }
        public void receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    Byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    DV = DeSerialize(data) as object[];
                    THDV(DV, client.RemoteEndPoint.ToString());
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("loi receive Server  " + e.ToString());
                Listclient.Remove(client);
                client.Close();
            }
        }
        public void send(object[] obj, string ip)
        {
            if (ip == null)
            {
                foreach (Socket client in Listclient)
                {
                    client.Send(Serialize(obj));
                }
            }
            else
            {
                foreach (Socket client in Listclient)
                {
                    if (ip == client.RemoteEndPoint.ToString()) client.Send(Serialize(obj));
                }
            }
        }
        public void close()
        {
            server.Close();
        }

        Byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter format = new BinaryFormatter();
            format.Serialize(stream, obj);
            return stream.ToArray();
        }
        object DeSerialize(Byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter format = new BinaryFormatter();
            //MessageBox.Show(format.Deserialize(stream).ToString());
            return format.Deserialize(stream);
        }
        public void THDV(object[] dv, string ip)
        {
            SerVerDel(dv, ip);
            //switch (dv[0])
            //{
            //    case 1:
            //        del2(dv);
            //        break;
            //}
        }
        public connectSERVER()
        {

        }
    }
}
