using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Windows;
using System.Drawing;
using System.Windows.Media;

public delegate void DelegateMessage(string Reply);

namespace God_C_Creator
{
    class Receiver
    {
        static public MainWindow mW;
        private static TcpListener tcpListener;
        private static Thread listenThread;

        public Receiver(MainWindow mainWindow)
        {
            mW = mainWindow;
        }
        public void StartListening()
        {
            tcpListener = new TcpListener(IPAddress.Any, 3000);
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
        }
        static void ListenForClients()
        {
            tcpListener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                }
                catch
                {
                    return;
                }
            }
        }
        static void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[1024];
            int bytesRead;
            while (true)
            {
                try
                {
                    bytesRead = clientStream.Read(message, 0, 1024);
                    string acceptedMessage = null;

                    acceptedMessage = Encoding.UTF8.GetString(message, 0, bytesRead);
                    if (acceptedMessage.IndexOf("Error") == 0)
                    {
                        tcpListener.Stop();
                        mW.isDebugError = true;
                        SendMessageToTextBox(acceptedMessage);
                        return;
                    }
                    else
                    {
                        mW.isDebugError = false;
                    }
                    SendMessageToTextBox(acceptedMessage);
                }
                catch
                {
                    tcpListener.Stop();
                    return;
                }
            }
        }
        static void SendMessageToTextBox(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                mW.textBoxDebug.Text += message;
                if (mW.isDebugError)
                {
                    mW.textBoxStatus.Text = "Build error!";
                    mW.textBoxStatus.Background = new SolidColorBrush(Color.FromRgb(196, 0, 5));
                }
            }));
        }
    }
}
