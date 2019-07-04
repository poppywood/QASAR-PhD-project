using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Qasar.ESB;
using Qasar.ESB.Core;

namespace Qasar.UnitTests
{
    [TestFixture]
    public class ESB
    {
        [Test]
        public void Create()
        {
            IntegrationHub core = new IntegrationHub();
            PolicyRequest req = new PolicyRequest();
            Credentials cred = new Credentials();
            cred.LoginID = "Shelly";
            cred.Password = "Shelly";
            cred.Source = "Shelly";
            cred.User = "Any";
            req.Action = PolicyRequest.PolicyRequestAction.Create;
            req.Credentials = cred;
            req.Policy = "Create";
            req.PolicyRef = "0";
            req.ProductCode = "Any";
            PolicyResponse resp = core.SubmitPolicy(req);
        }
        [Test]
        public void Renew()
        {
            IntegrationHub core = new IntegrationHub();
            PolicyRequest req = new PolicyRequest();
            Credentials cred = new Credentials();
            cred.LoginID = "Shelly";
            cred.Password = "Shelly";
            cred.Source = "Shelly";
            cred.User = "Any";
            req.Action = PolicyRequest.PolicyRequestAction.Renew;
            req.Credentials = cred;
            req.Policy = "Renew";
            req.PolicyRef = "0";
            req.ProductCode = "Any";
            PolicyResponse resp = core.SubmitPolicy(req);
        }
        [Test]
        public void Update()
        {
            IntegrationHub core = new IntegrationHub();
            PolicyRequest req = new PolicyRequest();
            Credentials cred = new Credentials();
            cred.LoginID = "Shelly";
            cred.Password = "Shelly";
            cred.Source = "Shelly";
            cred.User = "Any";
            req.Action = PolicyRequest.PolicyRequestAction.Update;
            req.Credentials = cred;
            req.Policy = "Update";
            req.PolicyRef = "0";
            req.ProductCode = "Any";
            PolicyResponse resp = core.SubmitPolicy(req);
        }
        [Test]
        public void Cancel()
        {
            IntegrationHub core = new IntegrationHub();
            PolicyRequest req = new PolicyRequest();
            Credentials cred = new Credentials();
            cred.LoginID = "Shelly";
            cred.Password = "Shelly";
            cred.Source = "Shelly";
            cred.User = "Any";
            req.Action = PolicyRequest.PolicyRequestAction.Cancel;
            req.Credentials = cred;
            req.Policy = "Cancel";
            req.PolicyRef = "0";
            req.ProductCode = "Any";
            PolicyResponse resp = core.SubmitPolicy(req);
        }
    }
}
