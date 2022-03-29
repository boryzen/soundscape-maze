using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurfaceHeadphonesBridge
{
    public enum SensorCalibrationState
    {
        NotCalibrated,
        Initializing,
        Calibrating,
        Calibrated
    }

    [Serializable]
    public sealed class OrientationUpdatedEventArgs : EventArgs
    {
        public Vector3 EulerAngles { get; set; }
    }

    public class OrientationListener
    {
        private Thread thread;
        private bool isExiting = false;
        private TcpClient client;
        private NetworkStream stream;

        private readonly int port;

        public bool isStarted { get; private set; }

        public bool isConnected { get; private set; }

        public event EventHandler<OrientationUpdatedEventArgs> OrientationUpdated;

        public OrientationListener(int port)
        {
            this.port = port;
            Debug.Log("Orientation Listener created!");

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Ensure we stop listening whenever the user stops play mode
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("Play mode stopping. Stopping orientation listener.");
                Stop();
            }
        }
#endif

        public void Start()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("Orientation listener only started during play mode...");
                return;
            }

            if (thread != null)
            {
                throw new Exception("Listener is already started! Stop() must be called before Start() can be called again.");
            }

            thread = new Thread(Run);
            thread.Start();
            Debug.Log("Orientation Listener started!");
            isStarted = true;
        }

        public void Stop()
        {
            if (thread == null)
            {
                throw new Exception("Listener is already stopped! Start() must be called before Stop() can be called again.");
            }

            // Close the socket and wait for the thread to join
            isExiting = true;
            stream?.Close();
            client?.Close();

            Debug.Log("Waiting for listener thread to join");
            thread.Join();
            thread = null;
            Debug.Log("Orientation Listener stopped!");

            isStarted = false;
        }

        private void Run()
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Loopback, port);

            int packetSize = sizeof(int) + sizeof(double) * 3;
            var data = new byte[packetSize];

            while (!isExiting)
            {
                Debug.Log($"Connecting to headphone server on port: {remoteEndpoint.Address}:{remoteEndpoint.Port}");
                try
                {
                    client = new TcpClient();
                    client.Connect(remoteEndpoint);
                    stream = client.GetStream();

                    Debug.Log($"Connected!");

                    isConnected = true;

                    while (isConnected)
                    {
                        if (!client.Connected)
                        {
                            Debug.Log("Server disconnected...");
                            isConnected = false;
                            continue;
                        }

                        try
                        {
                            //Debug.Log("Listening for server packet...");
                            var res = stream.Read(data, 0, packetSize);
                            //Debug.Log("Received server packet...");

                            if (res != packetSize)
                            {
                                if (res == 0)
                                {
                                    Debug.Log("Server disconnected...");
                                    break;
                                }

                                Debug.Log($"Read read less than the full packet ({res} bytes read, {packetSize} bytes expected)");
                                Thread.Sleep(20);
                                continue;
                            }

                            var frame = BitConverter.ToInt32(data, 0);
                            var yaw = BitConverter.ToDouble(data, sizeof(int));
                            var pitch = BitConverter.ToDouble(data, sizeof(int) + sizeof(double));
                            var roll = BitConverter.ToDouble(data, sizeof(int) + sizeof(double) * 2);

                            OrientationUpdatedEventArgs e = new OrientationUpdatedEventArgs()
                            {
                                EulerAngles = new Vector3((float)pitch, (float)yaw, (float)roll)
                            };

                            OnOrientationUpdated(e);
                        }
                        catch (SocketException ex)
                        {
                            // Hmm... A socket error occurred. Print the error for debugging purposes
                            Debug.Log($"OrientationListener: Socket error occurred ({ex.ErrorCode})");
                            isConnected = false;
                        }
                        catch (ObjectDisposedException)
                        {
                            // The underlying socket has been closed, so the thread should wrap up
                            isExiting = true;
                            isConnected = false;
                        }
                        catch (System.IO.IOException ex)
                        {
                            // Hmm... An IO error occurred. Print the error for debugging purposes
                            Debug.Log(ex.ToString());
                            isConnected = false;
                        }
                        catch (Exception e)
                        {
                            // Hmm... An error occurred. Print the error for debugging purposes
                            Debug.Log(e.ToString());
                            isConnected = false;
                        }
                    }

                    stream.Close();
                    client.Close();
                }
                catch (Exception)
                {
                    Debug.Log($"Unable to connect to headphones. Will try again in 1 second...");

                    //Try connecting again in one second
                    Thread.Sleep(1000);
                }
            }
        }

        protected virtual void OnOrientationUpdated(OrientationUpdatedEventArgs e)
        {
            OrientationUpdated?.Invoke(this, e);
        }
    }
}