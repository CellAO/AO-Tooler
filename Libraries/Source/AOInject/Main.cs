#region License

// Copyright (c) 2005-2014, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

namespace AOInject
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    using AOTooler.Hook;

    using EasyHook;

    #endregion

    /// <summary>
    /// </summary>
    public class Main : IEntryPoint
    {
        #region Fields

        /// <summary>
        /// </summary>
        private HookInterface Interface;

        /// <summary>
        /// </summary>
        private Stack<byte[]> Queue = new Stack<byte[]>();

        /// <summary>
        /// </summary>
        private LocalHook ReceiveMessageHook;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="inContext">
        /// </param>
        /// <param name="inChannelName">
        /// </param>
        public Main(RemoteHooking.IContext inContext, string inChannelName)
        {
            this.Interface = RemoteHooking.IpcConnectClient<HookInterface>(inChannelName);
            this.Interface.Ping();
        }

        #endregion

        #region Delegates

        /// <summary>
        /// </summary>
        /// <param name="_this">
        /// </param>
        /// <param name="size">
        /// </param>
        /// <param name="dataBlock">
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int DDataBlockToMessage(IntPtr _this, uint size, IntPtr dataBlock);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="inContext">
        /// </param>
        /// <param name="inChannelName">
        /// </param>
        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                this.ReceiveMessageHook =
                    LocalHook.Create(
                        LocalHook.GetProcAddress("MessageProtocol.dll", "?DataBlockToMessage@@YAPAVMessage_t@@IPAX@Z"), 
                        new DDataBlockToMessage(DataBlockToMessageHooked), 
                        this);
                this.ReceiveMessageHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            }
            catch (Exception e)
            {
                this.Interface.ReportException(e);
                return;
            }

            this.Interface.IsInstalled(RemoteHooking.GetCurrentProcessId());
            RemoteHooking.WakeUpProcess();

            try
            {
                while (true)
                {
                    Thread.Sleep(40);
                    if (this.Queue.Count > 0)
                    {
                        byte[][] package = null;
                        lock (this.Queue)
                        {
                            package = this.Queue.ToArray();
                            this.Queue.Clear();
                        }

                        this.Interface.Message(RemoteHooking.GetCurrentProcessId(), package);
                    }
                    else
                    {
                        this.Interface.Ping();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="_this">
        /// </param>
        /// <param name="size">
        /// </param>
        /// <param name="dataBlock">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("MessageProtocol.dll", CharSet = CharSet.Unicode, SetLastError = true, 
            CallingConvention = CallingConvention.ThisCall, EntryPoint = "?DataBlockToMessage@@YAPAVMessage_t@@IPAX@Z")]
        private static extern int DataBlockToMessage(IntPtr _this, uint size, IntPtr dataBlock);

        /// <summary>
        /// </summary>
        /// <param name="_this">
        /// </param>
        /// <param name="size">
        /// </param>
        /// <param name="dataBlock">
        /// </param>
        /// <returns>
        /// </returns>
        private static int DataBlockToMessageHooked(IntPtr _this, uint size, IntPtr dataBlock)
        {
            try
            {
                Main This = (Main)HookRuntimeInfo.Callback;
                byte[] temp = new byte[size];
                uint size2 = size;
                int size3 = (Int32)size2;
                Marshal.Copy(dataBlock, temp, 0, size3);
                lock (This.Queue)
                {
                    This.Queue.Push(temp);
                }
            }
            catch (Exception)
            {
            }

            return DataBlockToMessage(_this, size, dataBlock);
        }

        #endregion
    }
}