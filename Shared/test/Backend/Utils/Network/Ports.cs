namespace M47.Shared.Tests.Utils.Network;

using System.Net;
using System.Net.Sockets;

public class Ports
{
    public static int GetAvailablePort()
    {
        var defaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, 0);

        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(defaultLoopbackEndpoint);
        var port = ((IPEndPoint)socket.LocalEndPoint!).Port;

        return port;
    }
}