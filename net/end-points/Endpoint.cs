namespace Jaket.Net.EndPoints;

using Steamworks;
using System.Collections.Generic;

using Jaket.Content;
using Jaket.IO;

/// <summary> Network connection endpoint that contains listeners for different packet types. </summary>
public abstract class Endpoint
{
    /// <summary> Dictionary of packet types to their listeners. </summary>
    public Dictionary<PacketType, PacketListener> listeners = new();

    /// <summary> Loads endpoint listeners and other stuff. </summary>
    public abstract void Load();

    /// <summary> Adds a new listener to the endpoint. </summary>
    public void Listen(PacketType type, PacketListener listener) => listeners.Add(type, listener);

    /// <summary> Reads available packets and pass them to listeners. </summary>
    public void Update()
    {
        // go through the keys of the dictionary, because listeners may be missing for some types of packets
        foreach (var packetType in listeners.Keys)
        {
            // read each available packet
            while (SteamNetworking.IsP2PPacketAvailable((int)packetType))
            {
                var packet = SteamNetworking.ReadP2PPacket((int)packetType);
                if (packet.HasValue) Reader.Read(packet.Value.Data, r => listeners[packetType](packet.Value.SteamId, packetType, r));
            }
        }
    }

    /// <summary> Packets listener that accept the sender, the packet type and the packet itself. </summary>
    public delegate void PacketListener(SteamId sender, PacketType type, Reader r);
}