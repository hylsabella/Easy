using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Domain
{
    public class DomainModelBase
    {
        public int Id { get; set; }

        public string Creater { get; set; }

        public DateTime CreateDate { get; set; }

        public string Editor { get; set; }

        public DateTime? EditDate { get; set; }

        public bool IsDel { get; set; }

        public int Version { get; set; }

        public string TableIndex { get; set; }
    }
}
