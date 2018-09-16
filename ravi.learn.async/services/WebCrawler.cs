using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ravi.learn.async.services
{
    public interface ICrawler
    {
        string ReadSite(string url);
    }

    public class WebCrawler : ICrawler
    {
        private  readonly ConcurrentQueue<string> _concurrentQueue;
        public WebCrawler(ConcurrentQueue<string> concurrentQueue)
        {
            _concurrentQueue = concurrentQueue;
        }
        public string ReadSite(string url)
        {
            string dataFromSite = null;

            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-TId: {Thread.CurrentThread.ManagedThreadId}- Start of ReadSite");

            using (var client = new WebClient())
            {
                _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-Adding headers");

                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-Reading url");
                using (var data = client.OpenRead(url))
                {
                    _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-opening stream");
                    using (var reader = new StreamReader(data))
                    {
                        _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-TId: {Thread.CurrentThread.ManagedThreadId}-reading data from stream");
                        dataFromSite = reader.ReadToEnd();
                        _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-TId: {Thread.CurrentThread.ManagedThreadId}-done reading data from stream");
                    }
                }
            }
            return dataFromSite;
        }
    }
}
