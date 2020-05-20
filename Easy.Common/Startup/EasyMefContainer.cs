using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Startup
{
    public static class EasyMefContainer
    {
        public static CompositionContainer Container { get; private set; }

        public static void InitMefContainer(CompositionContainer container)
        {
            Container = container;
        }
    }
}