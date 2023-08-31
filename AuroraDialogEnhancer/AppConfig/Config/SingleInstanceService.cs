using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using AuroraDialogEnhancer.AppConfig.Statics;

namespace AuroraDialogEnhancer.AppConfig.Config;

public sealed class SingleInstanceService : IDisposable
{
    private const byte NOTIFY_INSTANCE_MESSAGE_TYPE = 1;

    private readonly string   _applicationId;
    private readonly string   _pipeName;
    private readonly bool     _isStartServer;
    private readonly TimeSpan _clientConnectionTimeout;

    private NamedPipeServerStream? _server;
    private Mutex?                 _mutex;

    public event EventHandler<SingleInstanceEventArgs>? OnNewInstance;

    public SingleInstanceService()
    {
        _isStartServer           = true;
        _applicationId           = Global.AssemblyInfo.Name;
        _pipeName                = "Local\\Pipe" + _applicationId;
        _clientConnectionTimeout = TimeSpan.FromSeconds(3);
    }

    public bool StartIpcServer()
    {
        if (!TryAcquireMutex()) return false;

        StartNamedPipeServer();
        return true;
    }

    private void StartNamedPipeServer()
    {
        if (!_isStartServer) return;

        using (var currentIdentity = WindowsIdentity.GetCurrent())
        {
            var identifier = currentIdentity.Owner;

            // Grant full control to the owner so multiple servers can be opened.
            // Full control is the default per MSDN docs for CreateNamedPipe.
            if (identifier is not null)
            {
                var rule = new PipeAccessRule(identifier, PipeAccessRights.FullControl, AccessControlType.Allow);
                var pipeSecurity = new PipeSecurity();

                pipeSecurity.AddAccessRule(rule);
                pipeSecurity.SetOwner(identifier);

                _server = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.In,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous,
                    0,
                    0,
                    pipeSecurity);
            }
        }

        try
        {
            _server!.BeginWaitForConnection(Listen, state: null);
        }
        catch (ObjectDisposedException)
        {
            // The server was disposed before getting a connection
        }
    }

    private void Listen(IAsyncResult ar)
    {
        var server = _server;
        if (server is null)
            return;

        try
        {
            try
            {
                server.EndWaitForConnection(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            StartNamedPipeServer();

            using var binaryReader = new BinaryReader(server);
            if (binaryReader.ReadByte() != NOTIFY_INSTANCE_MESSAGE_TYPE) return;

            var processId = binaryReader.ReadInt32();
            var argCount = binaryReader.ReadInt32();
            if (argCount < 0) return;

            var args = new string[argCount];
            for (var i = 0; i < argCount; i++)
            {
                args[i] = binaryReader.ReadString();
            }

            OnNewInstance?.Invoke(this, new SingleInstanceEventArgs(processId, args));
        }
        finally
        {
            server.Dispose();
        }
    }

    private bool TryAcquireMutex()
    {
        if (_mutex is null)
        {
            var mutexName = "Local\\Mutex" + _applicationId;
            _mutex = new Mutex(initiallyOwned: false, name: mutexName);
        }

        try
        {
            return _mutex.WaitOne(TimeSpan.Zero);
        }
        catch (AbandonedMutexException)
        {
            return true;
        }
    }

    public bool NotifyFirstInstance(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        using var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
        try
        {
            client.Connect((int) _clientConnectionTimeout.TotalMilliseconds);

            // type, process id, arg length, arg1, arg2, ...
            using var ms = new MemoryStream();
            using (var binaryWriter = new BinaryWriter(ms))
            {
                binaryWriter.Write(NOTIFY_INSTANCE_MESSAGE_TYPE);
                binaryWriter.Write(GetCurrentProcessId());
                binaryWriter.Write(args.Length);
                foreach (var arg in args)
                {
                    binaryWriter.Write(arg);
                }
            }

            var buffer = ms.ToArray();
            client.Write(buffer, 0, buffer.Length);
            client.Flush();

            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    private static int GetCurrentProcessId()
    {
        return System.Diagnostics.Process.GetCurrentProcess().Id;
    }

    public void Dispose()
    {
        _mutex?.Dispose();
        _server?.Dispose();
    }
}
