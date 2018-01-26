using Fireasy.Common;
using Fireasy.Zero.Models;
using System;

namespace Fireasy.Zero.Services.Impls
{
    public class LogService : ILogService
    {
        public void Debug(object message, Exception exception = null)
        {
        }

        public void Error(object message, Exception exception = null)
        {
            if (exception is ClientNotificationException)
            {
                return;
            }

            using (var context = new DbContext())
            {
                context.SysLogs.Insert(new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = 2,
                    Title = message.ToString(),
                    Content = exception == null ? string.Empty : exception.Message
                });
            }
        }

        public void Fatal(object message, Exception exception = null)
        {
        }

        public void Info(object message, Exception exception = null)
        {
            using (var context = new DbContext())
            {
                context.SysLogs.Insert(new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = 1,
                    Title = message.ToString(),
                    Content = exception == null ? string.Empty : exception.Message
                });
            }
        }

        public void Warn(object message, Exception exception = null)
        {
        }
    }
}
