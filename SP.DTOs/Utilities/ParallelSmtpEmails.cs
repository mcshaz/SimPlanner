using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SP.Dto.Utilities
{
    //idea derived from http://stackoverflow.com/questions/11148586/why-does-parallel-foreach-create-endless-threads
    public class ParallelSmtpEmails : IDisposable
    {
        BlockingCollection<MailTask> _workList;
        List<Task> _tasks;
        int _maxClientCount;

        public ParallelSmtpEmails(int maxSmtpClients = 5)
        {
            //create worklist, filled with initial work
            _workList = new BlockingCollection<MailTask>(new ConcurrentQueue<MailTask>());
            _tasks = new List<Task>(maxSmtpClients);
            _maxClientCount = maxSmtpClients;        
        }

        public void CompletedAdding()
        {
            _workList.CompleteAdding();
        }

        public void Send(MailMessage message)
        {
            Send(message, null);
        }

        public void Send(MailMessage message, Action<SmtpException> onComplete)
        {
            _workList.Add(new MailTask() { Message = message, OnComplete = onComplete });
            if (_tasks.Count < _maxClientCount && _workList.Count > 0)
            {
                _tasks.Add(Task.Factory.StartNew(RunWorker));
            }
        }

        public Task SendingComplete()
        {
            CompletedAdding();
            return Task.WhenAll(_tasks);
        }

        public void WaitSendingCompletion()
        {
            CompletedAdding();
            Task.WaitAll(_tasks.ToArray());
        }

        public void RunWorker()
        {
            using (var client = new SmtpClient())
            {
                do
                {
                    MailTask m;
                    if (_workList.TryTake(out m))
                    {
                        try
                        {
                            client.Send(m.Message);
                            m.OnComplete?.Invoke(null);
                        }
                        catch (SmtpException ex)
                        {
                            m.OnComplete?.Invoke(ex);
                        }
                    }
                } while (!_workList.IsCompleted);
            }
        }

        #region IDisposable
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ParallelSmtpEmails()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                _workList.Dispose();
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        #endregion //IDisposable

        private class MailTask
        {
            public MailMessage Message { get; set; }
            public Action<SmtpException> OnComplete { get; set; }
        }
    }
}
