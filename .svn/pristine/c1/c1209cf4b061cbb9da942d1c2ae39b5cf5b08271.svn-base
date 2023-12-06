using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using UMSV.Schema;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;

namespace UMSV
{
    public delegate void SipPacketTransmitEventHandler(byte[] message, IPEndPoint remoteEndPoint);

    public class SipNet : IDisposable
    {
        #region Fields

        private int ReOpenTime = 0;
        private DateTime LastReOpenTime = DateTime.MinValue;
        private IAsyncResult currentAynchResult;
        const int BufferSize = 50000;
        public event SipPacketTransmitEventHandler OnReceive;
        Socket socket;
        byte[] buffer = new byte[BufferSize];
        EndPoint remoteIP = new IPEndPoint(IPAddress.None, 0);
        IPEndPoint Address;
        bool StopSipNet;

        #endregion

        public bool Start(IPEndPoint address)
        {
            try
            {
                Logger.Write(LogType.Info, "Sip endpoint start at:{0} ", address);
                this.Address = address;

                StopSipNet = false;

                OpenSocket();
                BeginReceive();

                Logger.Write(LogType.Important, "Sip started successfully! endpoint: {0}", address);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public void Stop()
        {
            Logger.WriteInfo("Stoping SipNet...");
            StopSipNet = true;
            CloseSocket();
        }

        public bool Send(string message, IPEndPoint endPoint)
        {
            try
            {
                if (endPoint == null)
                {
                    Logger.WriteWarning("Remote Endpoint on Sent is null, packet: {0}", message);
                    return false;
                }

                if (message == null)
                {
                    Logger.WriteWarning("Sending null message to {0}", endPoint);
                    return false;
                }

                if (!message.StartsWith("CUSTOM") && Config.Default.LogSipMessage)
                    Logger.Write("sip", "->[{0}] {1}", endPoint, message);

                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(message);
                socket.SendTo(buffer, buffer.Length, SocketFlags.None, endPoint);
                return true;
            }
            catch (ObjectDisposedException)
            {
                Logger.WriteWarning("SipNet.Send ObjectDisposedException");
                ReOpen();
                return false;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        void ReOpen()
        {
            try
            {
                ReOpenTime++;
                if (DateTime.Now.Subtract(LastReOpenTime).TotalMinutes > 5)
                {
                    Logger.WriteInfo("LastReOpenTime '{0}' is latest than 5 minutes", LastReOpenTime.TimeOfDay);
                    ReOpenTime = 0;
                }

                if (ReOpenTime > 5)
                {
                    Logger.WriteCritical("SipNet.ReOpen, ReOpenTime is more than 5 ({0}), Killing service", ReOpenTime);
                    Process.GetCurrentProcess().Kill();
                }

                Logger.WriteInfo("SipNet.ReOpen, ReOpenTime: {0}", ReOpenTime);
                lock (typeof(SipNet))
                {
                    Stop();
                    Start(Address);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "ReOpen");
            }
        }

        void CloseSocket()
        {
            try
            {
                Logger.WriteDebug("CloseSocket SipNet ...");

                if (socket != null)
                {
                    try
                    {
                        socket.Disconnect(false);
                    }
                    catch
                    {
                    }
                    socket.Close();
                    socket.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        void OpenSocket()
        {
            Logger.WriteView("OpenSocket on {0} ...", Address);

            for (int index = 0; index < Config.Default.SipNetSocketStartRetryTimes; index++)
            {
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.Bind(Address);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

                    Logger.WriteView("Socket opended on {0}", Address);
                    return;
                }
                catch (SocketException ex)
                {
                    Logger.WriteError("SipNet OpenSocket error, SocketErrorCode:{0}, endPoint:{1}, message:{2}", ex.SocketErrorCode, Address, ex.Message);
                    Thread.Sleep(Config.Default.SipNetSocketStartRetryInterval);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, "SipNet.OpenSocket");
                }
            }

            Logger.WriteCritical("SipNet OpenSocket failed after {0} retry times, Killing myself.", Config.Default.SipNetSocketStartRetryTimes);

            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.UseShellExecute = true;
                info.FileName = string.Format("{0}\\Start.{1}.bat", Folder.Utility.ServiceTempDirectory, Guid.NewGuid());
                System.IO.File.WriteAllText(info.FileName, "ping -n 60 localhost\r\nnet start FolderService");

                Logger.WriteDebug("Starting Process Start.bat to start FolderService service ...");
                Process.Start(info);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        void BeginReceive()
        {
            for (int index = 0; index < 100; index++)
            {
                try
                {
                    if (StopSipNet)
                        return;

                    currentAynchResult = socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteIP, PacketReceive, null);
                    return;
                }
                catch (SocketException ex)
                {
                    Logger.WriteError("SipNet.BeginReceive error, SocketErrorCode:{0}, message:{1}, retry time:{2}", ex.SocketErrorCode, ex.Message, index);
                }
                catch (ObjectDisposedException)
                {
                    Logger.WriteCritical("SipNet.BeginReceive disposed object error, retry time:{0}", index);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, "SipNet.BeginReceive, index:{0}", index);
                }
                Thread.Sleep(50);
            }
        }

        void PacketReceive(IAsyncResult result)
        {
            try
            {
                if (result == currentAynchResult) // if not check, we got error: The IAsyncResult object was not returned from the corresponding asynchronous method on this class
                {
                    int length = socket.EndReceiveFrom(result, ref remoteIP);
                    if (length == 0)
                        return;

                    byte[] packet = new byte[length];
                    Array.Copy(buffer, packet, length);

                    if (OnReceive != null)
                        OnReceive(packet, (IPEndPoint)remoteIP);
                }
            }
            catch (SocketException ex)
            {
                Logger.WriteInfo("SocketError on SIPNet PacketReceive code:{1}, message:{0}, remoteIP: {2}", ex.Message, ex.SocketErrorCode, remoteIP);
            }
            catch (ObjectDisposedException)
            {
                Logger.WriteWarning("SipNet.PacketReceive.ObjectDisposedException");
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                BeginReceive();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
