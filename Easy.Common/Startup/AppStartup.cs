using Easy.Common.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Startup
{
    public class AppStartup
    {
        private static bool _isStart;

        public void Start()
        {
            if (EasyIocContainer.Container == null) throw new Exception("请先加载Ioc容器");

            _isStart = true;
        }

        public static bool IsStart()
        {
            return _isStart;
        }
    }
}
