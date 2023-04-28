using System;
using System.IO;
using EvoS.Framework.Misc;
using log4net;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace CentralServer
{
    public abstract class WebSocketBehaviorBase : WebSocketBehavior
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebSocketBehaviorBase));

        protected static void LogMessage(string prefix, object message)
        {
            try
            {
                log.Debug($"{prefix} {message.GetType().Name} {DefaultJsonSerializer.Serialize(message)}");
            }
            catch (Exception e)
            {
                log.Debug($"{prefix} {message.GetType().Name} <failed to serialize message>");
            }
        }
        
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
            if (!IsMinorError(e))
            {
                Wrap(x =>
                {
                    log.Error($"Websocket Error: {x.Message} {x.Exception}");
                }, e);
            }
            else
            {
                Wrap(x =>
                {
                    log.Warn($"Websocket Error: {x.Message} {x.Exception}");
                }, e);
            }
            Wrap(HandleError, e);
        }

        private static bool IsMinorError(ErrorEventArgs e)
        {
            return e.Exception is IOException
                   || "The stream has been closed".Equals(e.Exception.Message);
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
            LogicalThreadContext.Stacks["conns"].Pop();
            if (LogicalThreadContext.Stacks["conns"].Count > 0)
            {
                string connContext = LogicalThreadContext.Stacks["conns"].Pop();
                LogicalThreadContext.Stacks["conns"].Push(connContext);
                LogicalThreadContext.Properties["conn"] = connContext;
            }
            else
            {
                LogicalThreadContext.Properties["conn"] = null;
            }
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