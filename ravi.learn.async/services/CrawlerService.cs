using ravi.learn.async.services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ravi.learn.async.services
{
    public interface ICrawlerService
    {
        string ReadSite(string url);
        Task<string> ReadSiteAsync(string url);
    }

    public class CrawlerService : ICrawlerService
    {
        private readonly ConcurrentQueue<string> _concurrentQueue;

        private Func<string, string> _crawlerInvoker;

        private readonly ICrawler _crawler;

        public CrawlerService(ICrawler crawler, ConcurrentQueue<string> concurrentQueue)
        {
            _crawler = crawler;
            _crawlerInvoker = ReadSite;
            _concurrentQueue = concurrentQueue;
        }

        public string ReadSite(string url)
        {
            string siteData = null;
            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-Start of ReadSite");
            siteData = _crawler.ReadSite(url);
            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-End of ReadSite");
            return siteData;
        }

        public Task<string> ReadSiteAsync(string url)
        {
            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-TId: {Thread.CurrentThread.ManagedThreadId}-Start of ReadSiteAsync");
            return Task<string>.Factory.FromAsync<string>(BeginGreeting, EndGreeting, url, null);
        }

        private IAsyncResult BeginGreeting(string url, AsyncCallback callback, object state)
        {
            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-TId: {Thread.CurrentThread.ManagedThreadId}-Start of BeginGreeting");
            return _crawlerInvoker.BeginInvoke(url, callback, state);
        }

        private string EndGreeting(IAsyncResult asyncResult)
        {
            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-TId: {Thread.CurrentThread.ManagedThreadId}-Start of EndGreeting");
            return _crawlerInvoker.EndInvoke(asyncResult);
        }
    }
}
