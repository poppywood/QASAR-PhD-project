
namespace Qasar.ESB
{
    using System;
    
    public interface IMyHub
    {
        PolicyResponse SubmitPolicy(PolicyRequest request);
    }
}
