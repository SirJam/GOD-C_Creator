using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Pipes;
using System.IO;

public delegate void DelegateMessage(string Reply);

namespace God_C_Creator
{
    class Receiver
    {
        MainWindow mW;
        string _pipeName = "god_c_creator_pipe";
        public event DelegateMessage PipeMessage;

        public Receiver(MainWindow mainWindow)
        {
            this.mW = mainWindow;
        }
        public void StartListening()
        {
            try
            {
                NamedPipeServerStream pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
            catch (Exception oEX)
            {
                this.mW.textBoxDebug.Text += '\n' + oEX.Message;
            }
        }
        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeServerStream pipeServer = (NamedPipeServerStream)iar.AsyncState;
                // End waiting for the connection
                pipeServer.EndWaitForConnection(iar);

                byte[] buffer = new byte[255];

                // Read the incoming message
                pipeServer.Read(buffer, 0, 255);

                // Convert byte buffer to string
                string stringData = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                this.mW.textBoxDebug.Text += stringData;

                // Pass message back to calling form
                PipeMessage.Invoke(stringData);

                // Kill original server and create new wait server
                pipeServer.Close();
                pipeServer = null;
                pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Recursively wait for the connection again and again....
                pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
            catch
            {
                return;
            }
        }

        public void MainFunc(object pipe)
        {
            NamedPipeServerStream m_server = (NamedPipeServerStream)pipe;
            try
            {
                StreamReader reader = new StreamReader(m_server);
                string line;

                while (true)
                {
                    Mutex m = new Mutex();
                    m.WaitOne();
                    line = reader.ReadLine();
                    this.mW.textBoxDebug.Text += line;
                    m.ReleaseMutex();
                }
            }
            catch
            {

            }
        }
    }
}
