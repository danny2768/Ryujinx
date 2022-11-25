﻿using Ryujinx.Common.Logging;
using Ryujinx.Common.Memory;
using Ryujinx.HLE.HOS.Services.Ldn.Spacemeowx2Ldn;
using Ryujinx.HLE.HOS.Services.Ldn.Types;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ryujinx.HLE.HOS.Services.Ldn.UserServiceCreator.Spacemeowx2Ldn.Proxy
{
    class LdnProxyUdpServer : NetCoreServer.UdpServer, ILdnUdpSocket
    {
        private LanProtocol _protocol;

        private byte[] _buffer;
        private int _bufferEnd;

        public Dictionary<Array6<byte>, NetworkInfo> scanResults = new Dictionary<Array6<byte>, NetworkInfo>();
        private void LogMsg(string msg)
        {
            Logger.Info?.PrintMsg(LogClass.ServiceLdn, msg);
        }

        public LdnProxyUdpServer(LanProtocol protocol, IPAddress address, int port)
            : base(address, port)
        {
            OptionReuseAddress = true;
            OptionReceiveBufferSize = LanProtocol.BufferSize;
            OptionSendBufferSize = LanProtocol.BufferSize;
            _protocol = protocol;
            _protocol.Scan += HandleScan;
            _protocol.ScanResp += HandleScanResp;

            // TODO: Figure out what's wrong with linux broadcast
            // Linux> wifi adapter also shows up as type Ethernet
            // Linux> wifi does not receive udp broadcast if hosting a lobby

            _buffer = new byte[LanProtocol.BufferSize];
            Start();
        }

        protected override Socket CreateSocket()
        {
            Socket s = new Socket(Endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true
            };
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
            return s;
        }

        protected override void OnStarted()
        {
            ReceiveAsync();
        }


        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            //LogMsg($"LdnProxyUdpServer OnReceived: Endpoint {endpoint}");
            _protocol.Read(ref _buffer, ref _bufferEnd, buffer, (int)offset, (int)size, endpoint);
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Logger.Info?.PrintMsg(LogClass.ServiceLdn, $"LdnProxyUdpServer caught an error with code {error}");
        }

        protected override void Dispose(bool disposingManagedResources)
        {
            _protocol.Scan -= HandleScan;
            _protocol.ScanResp -= HandleScanResp;
            base.Dispose(disposingManagedResources);
        }

        public bool SendPacketAsync(EndPoint endpoint, byte[] data)
        {
            Logger.Info?.PrintMsg(LogClass.ServiceLdn, $"Sending packet to: {endpoint}");
            return SendAsync(endpoint, data);
        }

        private void HandleScan(EndPoint endpoint, LanPacketType type, byte[] data)
        {
            _protocol.SendPacket(this, type, data, endpoint);
        }

        public void HandleScanResp(NetworkInfo info)
        {
            scanResults.Add(info.Common.MacAddress, info);
        }
    }
}