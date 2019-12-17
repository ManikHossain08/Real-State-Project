using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace InitVent.Common.Util
{
    public class WCFHost<TService, TServiceContract, TBinding>
    where TBinding : Binding, new()
    {
        private ServiceHost serviceHost;
        private TBinding binding;
        private EndpointAddress endpointAddress;
        private Uri baseAddress;
        private string serviceName;

        public TBinding Binding
        {
            get { return binding; }
        }

        public EndpointAddress EndpointAddress
        {
            get { return endpointAddress; }
        }

        public Uri BaseAddress
        {
            get { return baseAddress; }
        }

        public string ServiceName
        {
            get { return serviceName; }
        }

        public ServiceHost ServiceHost
        {
            get { return serviceHost; }
        }

        #region constructor

        public WCFHost(string hostBaseAddress)
        {
            baseAddress = new Uri(hostBaseAddress);
            this.serviceName = "default";
            endpointAddress = new EndpointAddress(RemoveEndingSlash(hostBaseAddress) + "/" + serviceName);
            serviceHost = new ServiceHost(typeof(TService), BaseAddress);
            Initialize();
        }

        public WCFHost(string hostBaseAddress, string serviceName)
        {
            baseAddress = new Uri(hostBaseAddress);
            this.serviceName = serviceName;
            endpointAddress = new EndpointAddress(RemoveEndingSlash(hostBaseAddress) + "/" + serviceName);
            serviceHost = new ServiceHost(typeof(TService), BaseAddress);
            Initialize();
        }

        //Singleton Instance overload
        public WCFHost(TService singletonInstance, string hostBaseAddress, string serviceName)
        {
            baseAddress = new Uri(hostBaseAddress);
            this.serviceName = serviceName;
            endpointAddress = new EndpointAddress(RemoveEndingSlash(hostBaseAddress) + "/" + serviceName);
            serviceHost = new ServiceHost(singletonInstance, BaseAddress);
            Initialize();
        }

        public void Initialize()
        {
            CreateServiceEndPoint();
            //var smb = new ServiceMetadataBehavior();
            //smb.HttpGetEnabled = true;
            //_serviceHost.Description.Behaviors.Add(smb);
            //serviceHost.Description.Behaviors.OfType<ServiceMetadataBehavior>().First(S => S.HttpGetEnabled = true);
        }
        #endregion

        // Override if you need to implment different binding logic
        public virtual void CreateServiceEndPoint()
        {
            binding = new TBinding();
            binding.SendTimeout = new TimeSpan(Int32.MaxValue);
            //HttpTransportBindingElement httpBindingElement = new HttpTransportBindingElement();
            //httpBindingElement.MaxBufferSize = Int32.MaxValue;
            //httpBindingElement.MaxReceivedMessageSize = Int32.MaxValue;
            //httpBindingElement.AllowCookies = true;
            //httpBindingElement.UseDefaultWebProxy = true;
            //httpBindingElement.KeepAliveEnabled = true;
            //_binding.CreateBindingElements().Add(httpBindingElement);
            serviceHost.AddServiceEndpoint(typeof(TServiceContract), binding, serviceName);

        }

        private static string RemoveEndingSlash(string path)
        {
            char[] charsToTrim = { '/' };
            string resultPath;
            resultPath = path.EndsWith("/") ? path.TrimEnd(charsToTrim) : path;
            return resultPath;
        }
    }
}