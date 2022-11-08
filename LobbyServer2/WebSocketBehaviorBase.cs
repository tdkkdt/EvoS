using System;
using System.IO;
using log4net;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace CentralServer
{
    public abstract class WebSocketBehaviorBase : WebSocketBehavior
    {
        protected sealed override void OnOpen()
        {
            Wrap((object x) => HandleOpen(), null);
        }

        protected virtual void HandleOpen()
        {
        }
        
        protected sealed override void OnClose(CloseEventArgs e)
        {
            Wrap(HandleClose, e);
        }

        protected virtual void HandleClose(CloseEventArgs e)
        {
        }

        protected sealed override void OnError(ErrorEventArgs e)
        {
            Wrap(HandleError, e);
        }

        protected virtual void HandleError(ErrorEventArgs e)
        {
        }

        protected sealed override void OnMessage(MessageEventArgs e)
        {
            Wrap(HandleMessage, e);
        }

        protected virtual void HandleMessage(MessageEventArgs e)
        {
        }

        protected override void Send(byte[] data)
        {
            Wrap(base.Send, data);
        }

        protected override void Send(FileInfo file)
        {
            Wrap(base.Send, file);
        }

        protected override void Send(string data)
        {
            Wrap(base.Send, data);
        }

        protected override void SendAsync(byte[] data, Action<bool> completed)
        {
            Wrap(base.SendAsync, data, completed);
        }

        protected override void SendAsync(FileInfo file, Action<bool> completed)
        {
            Wrap(base.SendAsync, file, completed);
        }

        protected override void SendAsync(string data, Action<bool> completed)
        {
            Wrap(base.SendAsync, data, completed);
        }

        protected override void SendAsync(Stream stream, int length, Action<bool> completed)
        {
            Wrap(base.SendAsync, stream, length, completed);
        }

        protected abstract string GetConnContext();
        
        private void Wrap<T>(Action<T> handler, T param)
        {
            LogicalThreadContext.Stacks["conn"].Push(GetConnContext());
            try
            {
                handler(param);
            }
            finally
            {
                LogicalThreadContext.Stacks["conn"].Pop();
            }
        }
        
        // plz forgive me
        private void Wrap<T1, T2>(Action<T1, T2> handler, T1 param1, T2 param2)
        {
            LogicalThreadContext.Stacks["conn"].Push(GetConnContext());
            try
            {
                handler(param1, param2);
            }
            finally
            {
                LogicalThreadContext.Stacks["conn"].Pop();
            }
        }
        
        private void Wrap<T1, T2, T3>(Action<T1, T2, T3> handler, T1 param1, T2 param2, T3 param3)
        {
            LogicalThreadContext.Stacks["conn"].Push(GetConnContext());
            try
            {
                handler(param1, param2, param3);
            }
            finally
            {
                LogicalThreadContext.Stacks["conn"].Pop();
            }
        }
    }
}