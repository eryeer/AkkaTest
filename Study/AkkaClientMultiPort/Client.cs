﻿using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;
using System.Net;
using System.Text;

namespace AkkaClient
{
    public class StartClient
    {
        static void Main(string[] args)
        {
            var sys = ActorSystem.Create("ClientSystem");
            //var manager = sys.Tcp();
            var client1 = sys.ActorOf(TcpClient.Props("127.0.0.1", 8088), "tcp-client1");
            var client2 = sys.ActorOf(TcpClient.Props("127.0.0.1", 8089), "tcp-client2");
            var client3 = sys.ActorOf(TcpClient.Props("127.0.0.1", 8090), "tcp-client3");
            var client4 = sys.ActorOf(TcpClient.Props("127.0.0.1", 8091), "tcp-client4");
            string msg = "";
            while (true)
            {
                Console.WriteLine("Enter a command");
                msg = Console.ReadLine();
                if (msg.Equals("exit"))
                {
                    Console.WriteLine("successfully exit");
                    break;
                }
                else if (msg.Equals("start"))
                {
                    Console.WriteLine("start tps test");
                    client1.Tell("start_tps");
                    client2.Tell("start_tps");
                    client3.Tell("start_tps");
                    client4.Tell("start_tps");
                }
                else
                {
                    Console.WriteLine("send a normal msg");
                    client1.Tell(msg);
                    client2.Tell(msg);
                    client3.Tell(msg);
                    client4.Tell(msg);
                }
            }
        }
    }

    public class TcpClient : UntypedActor
    {
        protected ILoggingAdapter Log { get; } = Context.GetLogger();
        public static Props Props(string host, int port) => Akka.Actor.Props.Create(() => new TcpClient(host, port));

        private byte[] msg_1k = new byte[] { 0x94, 0xd2, 0x00, 0x66, 0x08, 0x91, 0x2a, 0xdf, 0x35, 0x09, 0x48, 0x38, 0x2c, 0xba, 0xa5, 0x33, 0xbd, 0xb0, 0x89, 0x94, 0xb0, 0x0a, 0x2f, 0xec, 0x24, 0x3a, 0x98, 0x26, 0xf7, 0x39, 0x56, 0x77, 0xfb, 0x75, 0xce, 0x38, 0x65, 0xd8, 0x8d, 0xdb, 0xc7, 0x80, 0xab, 0x7f, 0xe7, 0xe7, 0xc0, 0x0e, 0x7a, 0x32, 0xbb, 0x5b, 0x1e, 0xce, 0x82, 0xe0, 0x23, 0x44, 0xf0, 0xa4, 0xf1, 0xb6, 0xb3, 0x02, 0x38, 0xe4, 0xb5, 0xd5, 0x5d, 0xf7, 0x99, 0x60, 0x71, 0x75, 0xff, 0xdf, 0x8a, 0x4b, 0x09, 0xf0, 0x89, 0x5f, 0xbf, 0x60, 0xfd, 0x4f, 0x2f, 0xca, 0xa3, 0x2e, 0x56, 0x63, 0x13, 0x7f, 0x72, 0xf2, 0x17, 0xa5, 0x2d, 0x74, 0x42, 0x42, 0x3e, 0x4a, 0xbf, 0xcb, 0x1d, 0x4f, 0xc9, 0x08, 0xfd, 0x3d, 0xc5, 0xda, 0x31, 0x72, 0x54, 0x0b, 0x6f, 0xdb, 0xa1, 0x36, 0xc9, 0x34, 0x75, 0x8b, 0x4c, 0xb6, 0x8d, 0x19, 0x99, 0x77, 0x00, 0x8f, 0x6b, 0x39, 0xeb, 0x9f, 0x5d, 0xc5, 0x19, 0x40, 0x7b, 0x6d, 0xf9, 0x51, 0x79, 0x7b, 0xcc, 0xe8, 0x2e, 0xfc, 0x2a, 0x3c, 0x8a, 0x58, 0x73, 0xf6, 0x64, 0xc5, 0x62, 0x0c, 0xb4, 0x72, 0x26, 0x9f, 0x18, 0xa7, 0x08, 0x4b, 0xc1, 0xb5, 0x49, 0x75, 0x14, 0x56, 0x7c, 0x45, 0x53, 0xff, 0x7b, 0x25, 0xb5, 0xfd, 0x04, 0x75, 0xb1, 0x45, 0xa1, 0x17, 0xfe, 0x3f, 0xdc, 0xbc, 0xfa, 0x86, 0x0e, 0xbb, 0x01, 0x07, 0x2b, 0x53, 0x36, 0x34, 0xb8, 0x3d, 0x5a, 0x1a, 0x89, 0x84, 0xda, 0x32, 0x07, 0x96, 0xde, 0x05, 0x5f, 0x77, 0x0e, 0x61, 0xe0, 0xba, 0xc0, 0x2c, 0x77, 0xfd, 0x61, 0x34, 0xbc, 0x6f, 0xbf, 0x7d, 0x74, 0xb9, 0xfe, 0x41, 0x0f, 0x57, 0x70, 0x96, 0xa6,  0x85, 0x37, 0xde, 0x93, 0xd4, 0x0d, 0x59, 0x8a, 0x39, 0x12, 0xac, 0x11, 0x16, 0xfc, 0xc1, 0x94, 0xcb, 0xec, 0x18, 0x33, 0xdf, 0x07, 0x37, 0x2e, 0xc6, 0xba, 0xe0, 0xd8, 0x81, 0xf8, 0xcb, 0x3a, 0x4c, 0xd6, 0xff,
                0x45, 0xf5, 0x12, 0x06, 0x5d, 0x81, 0xb1, 0xf9, 0x1f, 0x2c, 0xc6, 0xed, 0xf7, 0xe9, 0x7c, 0x39, 0x38, 0x3b, 0xba, 0x66, 0x1b, 0x20, 0x7d, 0x57, 0xff, 0xf2, 0xcc, 0x22, 0x0a, 0x1b, 0x70, 0xc3, 0xd3, 0xf5, 0x6c, 0xe2, 0xd8, 0xa0, 0x9f, 0x32, 0x43, 0x8f, 0xcd, 0x4a, 0x6d, 0x25, 0xd9, 0xef, 0x23, 0xa9, 0x82, 0x32, 0xf2, 0x7f, 0x94, 0x76, 0x03, 0x17, 0x95, 0x30, 0xc7, 0xf4, 0x81, 0x4d, 0xa0, 0xd4, 0x00, 0xab, 0xdd, 0xf4, 0xe5, 0x24, 0xba, 0x8c, 0x1c, 0x39, 0xd8, 0x53, 0xbf, 0x8a, 0x92, 0xd3, 0x0a, 0x51, 0x12, 0x5f, 0xd2, 0xa0, 0xde, 0x8e, 0xb7, 0xfe, 0xd4, 0x99, 0x87, 0x1a, 0x36, 0x70, 0x4a, 0x2c, 0x5d, 0x70, 0xd6, 0xd5, 0x7a, 0x30, 0xe6, 0xa8, 0x72, 0x58, 0xae, 0xfd, 0xad, 0x5c, 0x05, 0x83, 0x0c, 0x70, 0x82, 0x2b, 0xe1, 0xd6, 0x30, 0x88, 0x76, 0x9e, 0xaf, 0x0f, 0x2c, 0xae, 0x1a, 0x91, 0x20, 0xa8, 0x35, 0x78, 0xed, 0xbc, 0x89, 0xf6, 0x23, 0x61, 0x2c, 0xe4, 0x7d, 0xcd, 0xb9, 0xeb, 0x1e, 0x7c, 0x90, 0x81, 0xd6, 0x80, 0x67, 0x6c, 0x38, 0x17, 0x04, 0xf0, 0x3d, 0x31, 0x4c, 0xb0, 0xb9, 0xc5, 0x40, 0xd8, 0x3d, 0x57, 0xb9, 0xb8, 0x6f, 0x6c, 0x6e, 0xf1, 0x27, 0x64, 0xe4, 0xd6, 0x47, 0x06, 0x14, 0xdc, 0x52, 0x6b, 0xae, 0x43, 0x4a, 0x8e, 0x91, 0x60, 0x7d, 0xa6, 0x7f, 0x01, 0xc7, 0xf6, 0x7b, 0xaf, 0x70, 0xa9, 0xbc, 0x3a, 0x4b, 0x7c, 0x6b, 0x64, 0x87, 0xd0, 0xb0, 0x47, 0x92, 0x39, 0xa5, 0xeb, 0xa5, 0x54, 0x0b, 0xea, 0x1b, 0xbc, 0xdd, 0x48, 0x2e, 0x30, 0xe2, 0x44, 0x54, 0x98, 0x17, 0x12, 0x74, 0x26, 0x34, 0x6b, 0x92, 0x14, 0xa9, 0x30, 0xc5, 0x9a, 0x3b, 0x88, 0xbc, 0x17, 0xfd, 0xa6, 0x56, 0x85, 0xcc, 0x28, 0x27, 0xe8, 0x9c, 0x62, 0xc5, 0xe6, 0xd3, 0x85, 0x37, 0xde, 0x93, 0xd4, 0x0d, 0x59, 0x8a, 0x39, 0x12, 0xac, 0x11, 0x16, 0xfc, 0xc1, 0x94, 0xcb, 0xec, 0x18, 0x33, 0xdf, 0x07, 0x37, 0x2e, 0xc6, 0xba, 0xe0, 0xd8, 0x81, 0xf8, 0xcb, 0x3a, 0x4c, 0xd6, 0xff,
                0xc0, 0x24, 0x28, 0x3b, 0x21, 0x57, 0x12, 0x37, 0xbc, 0xd6, 0x2d, 0x3e, 0x73, 0xeb, 0x51, 0x94, 0xdb, 0x4f, 0x53, 0x8c, 0x93, 0x28, 0x07, 0xae, 0x6a, 0x91, 0x75, 0xc1, 0x5d, 0xbf, 0x4d, 0x13, 0xbd, 0xf5, 0x9f, 0x4c, 0x68, 0x25, 0xe8, 0x7e, 0x9f, 0xaa, 0xd8, 0x69, 0x45, 0xa2, 0x5b, 0xdf, 0xbd, 0x23, 0x01, 0xf3, 0x5e, 0xb3, 0x5a, 0x32, 0x00, 0x6b, 0x22, 0xe9, 0x47, 0x44, 0xef, 0x9c, 0xde, 0xbb, 0x2d, 0x37, 0xe7, 0x7a, 0xd3, 0x93, 0x77, 0x29, 0xa1, 0xbd, 0xd6, 0x6d, 0x2d, 0x51, 0x5e, 0xa8, 0x08, 0xd9, 0x5d, 0x0d, 0x11, 0xb1, 0xd5, 0x0b, 0x72, 0xed, 0xea, 0xbe, 0x0a, 0xdf, 0x3c, 0x2b, 0x6e, 0x9d, 0x52, 0x87, 0xc0, 0xba, 0x5e, 0x1a, 0x62, 0x07, 0x3a, 0xfa, 0x41, 0x6a, 0xdc, 0xa8, 0xd0, 0x40, 0xb4, 0x84, 0xe6, 0x14, 0x63, 0x4d, 0x06, 0x23, 0xe1, 0xe3, 0x26, 0x36, 0x16, 0xe1, 0xca, 0x39, 0x4b, 0x62, 0x27, 0xd3, 0x40, 0x0a, 0x49, 0x68, 0xee, 0x72, 0xa6, 0x7c, 0xe7, 0x34, 0x03, 0x91, 0x9c, 0x7e, 0x3a, 0xa2, 0x12, 0x4c, 0x3f, 0xa5, 0x2a, 0x9e, 0x1c, 0xd9, 0xe8, 0x35, 0x32, 0xad, 0x10, 0x28, 0x3e, 0x9f, 0x8b, 0x75, 0xdc, 0xc5, 0x6f, 0x56, 0x6d, 0xcd, 0xbe, 0x22, 0x83, 0x62, 0x7e, 0x88, 0x68, 0x15, 0x4b, 0x4f, 0x0b, 0x34, 0xd2, 0x02, 0x9e, 0x53, 0x8b, 0x5c, 0xfd, 0x31, 0x14, 0x1b, 0x03, 0x06, 0x11, 0xdf, 0xb2, 0xec, 0x1d, 0xc2, 0x52, 0x63, 0x12, 0x82, 0x7e, 0x70, 0x07, 0x6b, 0x9d, 0x16, 0xe7, 0xfb, 0x36, 0x82, 0x88, 0x47, 0x7b, 0x76, 0x5e, 0x2a, 0x56, 0x0d, 0x3b, 0xe2, 0x36, 0x70, 0x73, 0xbc, 0x65, 0xe1, 0x59, 0xf7, 0x1e, 0xea, 0x53, 0xcb, 0xa1, 0x9a, 0x9a, 0x83, 0xf0, 0x73, 0xff,
                0x90, 0x4e, 0xb3, 0x81, 0x3b, 0x9c, 0xa1, 0x77, 0x76, 0x33, 0xb6, 0xc3, 0xb5, 0xa5, 0x4c, 0x18, 0xfd, 0xa6, 0x10, 0x25, 0xee, 0x79, 0x27, 0xb5, 0x5d, 0xe7, 0x5f, 0xae, 0x04, 0xbc, 0x70, 0xae, 0xe3, 0xfe, 0xe2, 0x60, 0xc7, 0xd1, 0x30, 0x8f, 0x7a, 0x83, 0x9b, 0xf3, 0xdb, 0xf1, 0xfe, 0x3f, 0xaa, 0x93, 0xcc, 0x22, 0x01, 0x26, 0xf9, 0xb5, 0xc7, 0xb1, 0x35, 0x78, 0x83, 0x53, 0xaa, 0x37, 0xdf, 0x53, 0xb6, 0xb9, 0x50, 0x03, 0x2b, 0x41, 0x44, 0xc8, 0x94, 0x89, 0xc1, 0xa2, 0x03, 0x99, 0x58, 0x66, 0x78, 0x44, 0xc4, 0xb6, 0x5b, 0xe0, 0x75, 0xd9, 0xfa, 0xea, 0x3f, 0xc4, 0xc3, 0xfb, 0xfb, 0x9d, 0xca, 0xb1, 0xd6, 0x9c, 0xca, 0xdd, 0x79, 0x08, 0x0c, 0xd8, 0x14, 0xfb, 0x90, 0x40, 0x5f, 0x50, 0x6d, 0x60, 0x9f, 0x6c, 0x51, 0x28, 0x2e, 0x2d, 0xab, 0x22, 0x80, 0x2f, 0xc8, 0xf9, 0xa3, 0xb0, 0x53, 0xa8, 0xcf, 0xfa, 0xb6, 0x25, 0xe1, 0x66, 0xf6, 0xae, 0xa1, 0xe4, 0xb8, 0x7c, 0x47, 0x66, 0xb8, 0xfe, 0x5a, 0xff, 0xbb, 0x7d, 0x5f, 0x8c, 0x69, 0x7d, 0x61, 0x6c, 0x5b, 0x9a, 0x18, 0x11, 0xfc, 0x39, 0xe2, 0x6f, 0xe5, 0x25, 0x45, 0xff, 0x77, 0xda, 0x80, 0x68, 0x4d, 0xa8, 0xbc, 0xad, 0xd2, 0x89, 0xbb, 0x37, 0x12, 0x9d, 0xd7, 0x28, 0x31, 0x55, 0x82, 0x07, 0x48, 0xf5, 0x89, 0x80, 0x3c, 0xee, 0x5a, 0xcc, 0x98, 0x63, 0x50, 0x1c, 0xb3, 0x96, 0xff, 0x10, 0x07, 0x6b, 0x58, 0xb3, 0x0f, 0xba, 0x55, 0x64, 0x46, 0xd3, 0xc8, 0x63, 0xf9, 0x66, 0xc5, 0x78, 0xca, 0xc9, 0x54, 0x06, 0x58, 0x19, 0x4c, 0x1d, 0xc0, 0xc1, 0x2c, 0xcb, 0x0c, 0x35, 0xb1, 0x40, 0xa9, 0x88 };

        private string msg_512_str = "94d2006608912adf350948382cbaa533bdb08994b00a2fec243a9826f7395677fb75ce3865d88ddbc780ab7fe7e7c00e7a32bb5b1ece82e02344f0a4f1b6b30238e4b5d55df799607175ffdf8a4b09f0895fbf60fd4f2fcaa32e5663137f72f217a52d7442423e4abfcb1d4fc908fd3dc5da3172540b6fdba136c934758b4cb68d199977008f6b39eb9f5dc519407b6df951797bcce82efc2a3c8a5873f664c5620cb472269f18a7084bc1b5497514567c4553ff7b25b5fd0475b145a117fe3fdcbcfa860ebb01072b533634b83d5a1a8984da320796de055f770e61e0bac02c77fd6134bc6fbf7d74b9fe410f577096a68537de93d40d598a3912ac1116fcc194cbec1833df07372ec6bae0d881f8cb3a4cd6ff45f512065d81b1f91f2cc6edf7e97c39383bba661b207d57fff2cc220a1b70c3d3f56ce2d8a09f32438fcd4a6d25d9ef23a98232f27f947603179530c7f4814da0d400abddf4e524ba8c1c39d853bf8a92d30a51125fd2a0de8eb7fed499871a36704a2c5d70d6d57a30e6a87258aefdad5c05830c70822be1d63088769eaf0f2cae1a9120a83578edbc89f623612ce47dcdb9eb1e7c9081d680676c381704f03d314cb0b9c540d83d57b9b86f6c6ef12764e4d6470614dc526bae434a8e91607da67f01c7f67baf70a9bc3a4b7c6b6487d0b0479239a5eba5540bea1bbcdd482e30e244549817127426346b";

        private byte[] msg_512;

        private byte[] msg_10 = new byte[] {
            0x94, 0xd2, 0x00, 0x66, 0x08, 0x91, 0x2a, 0xdf, 0x35, 0x09

        };

        private long sendCount = 0;

        public TcpClient(string host, int port)
        {
            var endpoint = new DnsEndPoint(host, port);
            Context.System.Tcp().Tell(new Tcp.Connect(endpoint));
            msg_512 = msg_512_str.HexToBytes();
            Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, new TPSTimer(), Self);
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Connected)
            {
                var connected = message as Tcp.Connected;
                Log.Info("Connected to {0}", connected.RemoteAddress);

                // Register self as connection handler
                Sender.Tell(new Tcp.Register(Self));
                Become(Connected(Sender));
            }
            else if (message is Tcp.CommandFailed)
            {
                Log.Error("Connection failed");
            }
            else Unhandled(message);
        }

        private UntypedReceive Connected(IActorRef connection)
        {
            return message =>
            {
                switch (message)
                {
                    case Tcp.Received received:
                        Log.Info("Response from server for {0}", Encoding.ASCII.GetString(received.Data.ToArray()));
                        break;
                    case string msg when msg.Equals("start_tps"):
                        Log.Info("Start to send tps msg");
                        for (long i = 0; i < 1024 * 2 * 1024; i++)
                        {
                            sendCount++;
                            connection.Tell(Tcp.Write.Create(ByteString.FromBytes(msg_512)));
                        }
                        break;
                    case string msg:
                        Log.Info("Start to send normal msg");
                        connection.Tell(Tcp.Write.Create(ByteString.FromString((string)msg + "\n")));
                        break;
                    case Tcp.PeerClosed _:
                        Log.Info("Connection closed");
                        break;
                    case TPSTimer _:
                        Log.Info($"Send tps is {sendCount}/s");
                        sendCount = 0;
                        break;
                    default:
                        Unhandled(message);
                        break;
                }
            };
        }
    }
    internal class TPSTimer { }
}