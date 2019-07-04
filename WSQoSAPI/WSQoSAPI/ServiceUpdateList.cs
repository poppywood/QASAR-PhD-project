using System.Collections;
using System.Threading;

namespace WSQoSAPI
{

    public class ServiceUpdateList
    {

        private System.Threading.Mutex _m;
        private System.Collections.ArrayList _services;

        public ServiceUpdateList()
        {
            _services = new System.Collections.ArrayList();
            _m = new System.Threading.Mutex();
        }

        public void AddService(WSQoSAPI.ServiceModel NewService)
        {
            _m.WaitOne();
            _services.Add(NewService);
            _m.ReleaseMutex();
        }

        public WSQoSAPI.ServiceModel GetNextService()
        {
            // trial
            return null;
        }

    } // class ServiceUpdateList

}

