using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Easy.Common.Exceptions;

namespace Easy.Common.Transaction
{
    public class ExceptionCallHandlerAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new DefaultExceptionCallHandler();
        }
    }
}
