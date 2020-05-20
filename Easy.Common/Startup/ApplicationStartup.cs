using Easy.Common.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Startup
{
    public class ApplicationStartup
    {
        public void Start()
        {
            if (EasyIocContainer.Container == null)
            {
                throw new Exception("请先加载Ioc容器");
            }
        }
    }
}
