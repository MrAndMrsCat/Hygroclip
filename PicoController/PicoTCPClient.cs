using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PicoController;

internal class PicoTCPClient
{
    public bool Connected => _stream is not null && _stream.Socket.Connected;
    public int Port { get; set; } = 42440;
    public string ServerIPAddress { get; set; } = "192.168.30.4";
    public bool UseLengthHeader { get; set; } = false;
    public byte? MessageTerminationByte { get; set; }

    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<byte[]>? BytesReceived;

    private NetworkStream? _stream;

    public void Connect()
    {
        Task.Run(() =>
        {
            try
            {
                Logger.Debug($"Creating TCP client at {ServerIPAddress}:{Port}");
                using TcpClient client = new(ServerIPAddress, Port);
                _stream = client.GetStream();
                OnConnectionChanged();

                while (client.Connected)
                {
                    if (UseLengthHeader)
                    {
                        ReadWithUInt32LengthHeader(_stream);
                    }
                    else if (MessageTerminationByte is not null)
                    {
                        ReadWithTerminationByte(_stream, (byte)MessageTerminationByte);
                    }
                    else
                    {
                        ReadAllBuffer(_stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Message send exception: {ex.Message}");
            }
            finally
            {
                OnConnectionChanged();
            }
        });
    }

    private readonly byte[] _readAllBuffer = new byte[2048];

    private void ReadAllBuffer(NetworkStream stream)
    {
        int byteCount = stream.Read(_readAllBuffer, 0, _readAllBuffer.Length);

        if (byteCount > 0)
        {
            OnBytesRecieved(_readAllBuffer[..byteCount]);
        }
        else throw new IOException("No bytes available, likely disconnection");
    }

    private void ReadWithUInt32LengthHeader(NetworkStream stream)
    {
        byte[] header = new byte[4];

        stream.Read(header, 0, 4);

        uint length = BitConverter.ToUInt32(header, 0);

        byte[] payload = new byte[length];

        if (stream.Read(payload, 0, (int)length) == length)
        {
            OnBytesRecieved(payload);
        }
        else
        {
            throw new FormatException("message length did not match header size");
        }
    }

    private void ReadWithTerminationByte(NetworkStream stream, byte terminationByte)
    {
        List<byte> payload = new();
        int readByte;

        while (true)
        {
            readByte = stream.ReadByte();

            if (readByte == terminationByte) // end of message
                break;

            if (readByte == -1) // end of stream
                throw new FormatException($"expected final byte <{terminationByte}> but reached end of stream");

            payload.Add((byte)readByte);
        }

        OnBytesRecieved(payload.ToArray());
    }

    private void OnConnectionChanged()
    {
        try
        {
            Logger.Info(GetType().Name + (Connected ? " connected" : " disconnected"));
            ConnectionChanged?.Invoke(this, Connected);
        }
        catch (Exception ex) { Logger.Error(ex.Message); }
    }

    private void OnBytesRecieved(byte[] bytes)
    {
        try
        {
            Logger.Debug($"Rx {bytes.Length} bytes");
            BytesReceived?.Invoke(this, bytes);
        }
        catch (Exception cmdProcessingEx)
        {
            Logger.Error($"Unhandled exception during command processing - {cmdProcessingEx.Message}");
        }
    }

    public void Send(byte[] bytes)
    {
        try
        {
            if (Connected)
            {
                byte[] sendBytes;

                if (UseLengthHeader)
                {
                    sendBytes = BitConverter
                        .GetBytes((uint)bytes.Length)
                        .Concat(bytes)
                        .ToArray();
                }
                else if (MessageTerminationByte is not null)
                {
                    sendBytes = bytes
                        .Concat(new byte[] { (byte)MessageTerminationByte })
                        .ToArray();
                }
                else
                {
                    sendBytes = bytes;
                }

                Logger.Debug($"Tx {sendBytes.Length} bytes");
                _stream!.Write(sendBytes);
            }
            else
            {
                Logger.Warning("TCP client cannot send message, stream not connected");
            }
        }
        catch (Exception ex) { Logger.Error(ex.Message); }
    }
}
