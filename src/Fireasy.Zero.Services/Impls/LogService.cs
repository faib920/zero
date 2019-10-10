using Fireasy.Common;
using Fireasy.Common.Logging;
using Fireasy.Zero.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fireasy.Zero.Services.Impls
{
    public class LogService : ILogService
    {
        private DbContext context;

        public LogService(DbContext context)
        {
            this.context = context;
        }

        public void Debug(object message, Exception exception = null)
        {
            DefaultLogger.Instance.Debug(message, exception);
        }

        public Task DebugAsync(object message, Exception exception = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Error(object message, Exception exception = null)
        {
            if (exception is ClientNotificationException)
            {
                return;
            }

            context.SysLogs.Insert(new SysLog
            {
                LogTime = DateTime.Now,
                LogType = 2,
                Title = message.ToString(),
                Content = exception == null ? string.Empty : exception.Message
            });
        }

        public async Task ErrorAsync(object message, Exception exception = null, CancellationToken cancellationToken = default)
        {
            if (exception is ClientNotificationException)
            {
                return;
            }

            await context.SysLogs.InsertAsync(new SysLog
            {
                LogTime = DateTime.Now,
                LogType = 2,
                Title = message.ToString(),
                Content = exception == null ? string.Empty : exception.Message
            }, cancellationToken);
        }

        public void Fatal(object message, Exception exception = null)
        {
            Error(message, exception);
        }

        public Task FatalAsync(object message, Exception exception = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Info(object message, Exception exception = null)
        {
            context.SysLogs.Insert(new SysLog
            {
                LogTime = DateTime.Now,
                LogType = 1,
                Title = message.ToString(),
                Content = exception == null ? string.Empty : exception.Message
            });
        }

        public async Task InfoAsync(object message, Exception exception = null, CancellationToken cancellationToken = default)
        {
            await context.SysLogs.InsertAsync(new SysLog
            {
                LogTime = DateTime.Now,
                LogType = 1,
                Title = message.ToString(),
                Content = exception == null ? string.Empty : exception.Message
            }, cancellationToken);
        }

        public void Warn(object message, Exception exception = null)
        {
            Error(message, exception);
        }

        public Task WarnAsync(object message, Exception exception = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
