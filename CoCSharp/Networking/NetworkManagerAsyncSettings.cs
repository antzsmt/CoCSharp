﻿using System;
using System.Net.Sockets;

namespace CoCSharp.Networking
{
    /// <summary>
    /// Provides settings for the <see cref="NetworkManagerAsync"/> class. It is recommended to use
    /// this for better management of <see cref="SocketAsyncEventArgs"/> objects.
    /// </summary>
    public class NetworkManagerAsyncSettings : IDisposable
    {
        /// <summary>
        /// Initailizes a new instance of the <see cref="NetworkManagerAsync"/> class
        /// with default settings.
        /// </summary>
        public NetworkManagerAsyncSettings() 
            : this(25, 25, 65535)
        {
            // Space
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManagerAsyncSettings"/> class
        /// with the specified number of receive operation <see cref="SocketAsyncEventArgs"/> objects
        /// and the specified number of send operation <see cref="SocketAsyncEventArgs"/> objects.
        /// </summary>
        /// <param name="receiveCount">Number of receive operation <see cref="SocketAsyncEventArgs"/> objects.</param>
        /// <param name="sendCount">Number of send operation <see cref="SocketAsyncEventArgs"/> objects.</param>
        public NetworkManagerAsyncSettings(int receiveCount, int sendCount)
            : this(receiveCount, sendCount, 65535)
        {
            // Space
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManagerAsyncSettings"/> class
        /// with the specified number of receive operation <see cref="SocketAsyncEventArgs"/> objects
        /// and the specified number of send operation <see cref="SocketAsyncEventArgs"/> objects with
        /// the specified buffer size of each <see cref="SocketAsyncEventArgs"/> object.
        /// </summary>
        /// <param name="receiveCount">Number of receive operation <see cref="SocketAsyncEventArgs"/> objects.</param>
        /// <param name="sendCount">Number of send operation <see cref="SocketAsyncEventArgs"/> objects.</param>
        /// <param name="bufferSize">Buffer size of each <see cref="SocketAsyncEventArgs"/> object.</param>
        public NetworkManagerAsyncSettings(int receiveCount, int sendCount, int bufferSize)
        {
            ReceivePool = new SocketAsyncEventArgsPool(receiveCount);
            SendPool = new SocketAsyncEventArgsPool(sendCount);
            _bufferManager = new MessageBufferManager(receiveCount, sendCount, bufferSize);

            for (int i = 0; i < ReceiveCount; i++)
            {
                var args = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(args);
                MessageToken.Create(args);
                ReceivePool.Push(args);
            }

            for (int i = 0; i < SendCount; i++)
            {
                var args = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(args);
                MessageToken.Create(args);
                SendPool.Push(args);
            }
        }

        /// <summary>
        /// Gets a new instance of the default state of the <see cref="NetworkManagerAsyncSettings"/> class.
        /// </summary>
        public static NetworkManagerAsyncSettings DefaultSettings
        {
            get { return new NetworkManagerAsyncSettings(); }
        }

        /// <summary>
        /// Gets the number of receive operation <see cref="SocketAsyncEventArgs"/> objects
        /// being used.
        /// </summary>
        public int ReceiveCount { get { return ReceivePool.Capacity; } }

        /// <summary>
        /// Gets the number of send operation <see cref="SocketAsyncEventArgs"/> objects
        /// being used.
        /// </summary>
        public int SendCount { get { return ReceivePool.Capacity; } }

        private bool _disposed;
        private MessageBufferManager _bufferManager;

        internal SocketAsyncEventArgsPool ReceivePool;
        internal SocketAsyncEventArgsPool SendPool;

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="NetworkManagerAsyncSettings"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases all unmanaged resources and optionally managed resources used by the current instance of the
        /// <see cref="NetworkManagerAsyncSettings"/> class.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ReceivePool.Dispose();
                    SendPool.Dispose();
                    //TODO: Dipose all NetworkManagerAsync instances using this also
                }
                _disposed = true;
            }
        }
    }
}