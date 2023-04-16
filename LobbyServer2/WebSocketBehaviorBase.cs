using System;
using log4net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CentralServer
{
    public abstract class WebSocketBehaviorBase : WebSocketBehavior
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebSocketBehaviorBase));
        
        protected sealed override void OnOpen()
        {
            Wrap((object x) => HandleOpen(), null);
        }

        protected virtual void HandleOpen()
        {
        }
        
        protected sealed override void OnClose(CloseEventArgs e)
        {
            Wrap(x => log.Info($"Disconnect: code {x.Code}, reason '{x.Reason}', clean {x.WasClean}"), e);
            Wrap(HandleClose, e);
        }

        protected virtual void HandleClose(CloseEventArgs e)
        {
        }

        protected sealed override void OnError(ErrorEventArgs e)
        {
            Wrap(x =>
            {
                log.Error($"Websocket Error: {x.Message} {x.Exception}");
            }, e);
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

        protected abstract string GetConnContext();

        private void LogContextPush()
        {
            string connContext = GetConnContext();
            LogicalThreadContext.Stacks["conns"].Push(connContext);
            LogicalThreadContext.Properties["conn"] = connContext;
        }

        private void LogContextPop()
        {
            LogicalThreadContext.Properties["conn"] = LogicalThreadContext.Stacks["conns"].Pop();
        }
        
        protected void Wrap<T>(Action<T> handler, T param)
        {
            LogContextPush();
            try
            {
                handler(param);
            }
            finally
            {
                LogContextPop();
            }
        }
        
        // plz forgive me
        protected void Wrap<T1, T2>(Action<T1, T2> handler, T1 param1, T2 param2)
        {
            LogContextPush();
            try
            {
                handler(param1, param2);
            }
            finally
            {
                LogContextPop();
            }
        }

        protected void Wrap<T1, T2, T3>(Action<T1, T2, T3> handler, T1 param1, T2 param2, T3 param3)
        {
            LogContextPush();
            try
            {
                handler(param1, param2, param3);
            }
            finally
            {
                LogContextPop();
            }
        }
    }
}