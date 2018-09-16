using ravi.learn.async.services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ravi.learn.async
{
    class Program
    {
        private static readonly ConcurrentQueue<string> _concurrentQueue = new ConcurrentQueue<string>();
        static void Main(string[] args)
        {
            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-Start of application");
            var readTask = ReadWebSite("http://www.google.com");
            _concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-After trigger of task");
            while (readTask.Status != TaskStatus.RanToCompletion) {
                //_concurrentQueue.Enqueue($"{DateTime.Now.ToLongTimeString()}-Status-{readTask.Status.ToString()}");
            }

            Console.WriteLine(readTask.Result);
            Console.ReadKey();

            string textToDisplay = null;
            while(_concurrentQueue.TryDequeue(out textToDisplay))
            {
                Console.WriteLine(textToDisplay);
            }

            Console.ReadKey();
        }

        static async Task<string> ReadWebSite(string url)
        {
            var crawlerService = new CrawlerService(new WebCrawler(_concurrentQueue), _concurrentQueue);
            return await crawlerService.ReadSiteAsync(url);
        }
   
    }
}
