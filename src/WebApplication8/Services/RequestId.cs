using System;
using System.Threading;

namespace WebApplication8.Services
{
    public interface IRequestId
    {
        string Id { get; }
    }

    // Scoped (to current request)
    public class RequestId : IRequestId
    {
        private readonly string _requestId;

        public RequestId(IRequestIdFactory fac)
        {
            _requestId = fac.MakeRequestId();
        }

        public string Id => _requestId; // => erstatter metodeblokk med 1 return statement..
    }

    public interface IRequestIdFactory
    {
        string MakeRequestId();
    }

    // Singleton
    public class RequestIdFactory : IRequestIdFactory
    {
        private int _requestId;

        public string MakeRequestId()
        {
            return Interlocked.Increment(ref _requestId).ToString();  // ref sørger for å sende en pointer til, fremfor kopi av tallverdien..
        }
    }
}