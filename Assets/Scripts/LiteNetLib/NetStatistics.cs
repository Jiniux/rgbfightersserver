﻿namespace LiteNetLib
{
    public sealed class NetStatistics
    {
        public ulong BytesReceived;
        public ulong BytesSent;
        public ulong PacketLoss;
        public ulong PacketsReceived;
        public ulong PacketsSent;

        public ulong SequencedPacketLoss;

        public ulong PacketLossPercent => PacketsSent == 0 ? 0 : PacketLoss * 100 / PacketsSent;

        public void Reset()
        {
            PacketsSent = 0;
            PacketsReceived = 0;
            BytesSent = 0;
            BytesReceived = 0;
            PacketLoss = 0;
        }

        public override string ToString()
        {
            return
                string.Format(
                    "BytesReceived: {0}\nPacketsReceived: {1}\nBytesSent: {2}\nPacketsSent: {3}\nPacketLoss: {4}\nPacketLossPercent: {5}\n",
                    BytesReceived,
                    PacketsReceived,
                    BytesSent,
                    PacketsSent,
                    PacketLoss,
                    PacketLossPercent);
        }
    }
}