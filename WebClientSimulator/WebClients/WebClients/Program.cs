using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Timers;

namespace WebClients
{
    public interface IChannel 
    {
        void Request(Action<RequestResult> responseHandler);
        
    }

    public class RequestResult 
    {
        public int RequestId { get; set; }
        public int ClientId { get; set; }
        public TimeSpan RequestTime { get; set; }
        public String HttpStatusCode { get; set; }
    }

    public class RequestState
    {
        public WebRequest Request;
        public int ClientID { get; set; }
        
        public RequestState()
        {

        }
    }

    public class HttpChannel : IChannel 
    {
        HttpWebRequest requestChannel;

        public HttpChannel(string address) 
        {

            requestChannel = (HttpWebRequest)WebRequest.Create(address);

        }

        public void Request(Action<RequestResult> responseHandler)
        {
            Stopwatch requestTime = new Stopwatch();
            requestTime.Start();
            requestChannel.BeginGetResponse(new AsyncCallback((ar)=>
            {               
                RequestState rs = (RequestState)ar.AsyncState;

                // Get the WebRequest from RequestState.
                WebRequest req = rs.Request;

                // Call EndGetResponse, which produces the WebResponse object
                //  that came from the request issued above.
                HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(ar);
                
                requestTime.Stop();
                responseHandler(new RequestResult() { ClientId = rs.ClientID, RequestTime = requestTime.Elapsed, HttpStatusCode = resp.StatusCode.ToString() });

            }),new RequestState() { Request = requestChannel });
        }
    }

    public class SimulateClient 
    {
        public int Id { get; set; }
        public TimeSpan RequestTime { get; set; }
        public string lastHttpStatus {get; set; }
        IChannel Channel;

        Action<RequestResult> RequestCompleted;


        public SimulateClient(string address) 
        {
            Channel = new HttpChannel(address);

        }

        public void DoRequest(Action<RequestResult> requestCompleted) 
        {
            Channel.Request((result) => 
            {
                requestCompleted(result);
            });
        }

    }


    public class ClientsFactory 
    {



        public static BlockingCollection<SimulateClient> GetMeSomeHttpClients(int numberOfClients, string address) 
        {

            BlockingCollection<SimulateClient> clients = new BlockingCollection<SimulateClient>();

            for(int i = 0; i< numberOfClients; ++i) 
            {
                clients.Add(new SimulateClient(address));
            }

            return clients;
        }
         
    }



    public class RequestsScheduler
    {
        BlockingCollection<SimulateClient> clientsToExecute = new BlockingCollection<SimulateClient>();

        Timer t = new Timer();

        int minimumNumberOfclients;
        int MaxNumberOfClients;
        int currentIndex = 0;

        public RequestsScheduler() 
        {
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (ResultsDbContext ctx = new ResultsDbContext())
            {
               var res =  Parallel.For(0, currentIndex, (idx) =>
                {
                    clientsToExecute.ElementAt(idx).DoRequest((reqRes) =>
                    {
                        ctx.RequestResults.Add(reqRes);
                    });
                });

               while (!res.IsCompleted) ;

               ctx.SaveChanges();

            }
        }

        public void StartEnvironment(int minimumNumberOfclients, int MaxNumberOfClients, string requestAddress) 
        {
            currentIndex = minimumNumberOfclients;
            clientsToExecute = ClientsFactory.GetMeSomeHttpClients(MaxNumberOfClients, requestAddress);
            
        }




    }


    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length > 1)
            {
                


            }
        }
    }
}
