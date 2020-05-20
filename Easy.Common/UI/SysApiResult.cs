using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.UI
{
    [Serializable, DataContract]
    public sealed class SysApiResult<TResult>
    {
        [DataMember(Name = "Status"), DefaultValue(SysResultStatus.成功)]
        public SysResultStatus Status { get; set; }

        [DataMember(Name = "Message"), DefaultValue("")]
        public string Message { get; set; }

        [DataMember(Name = "Result")]
        public TResult Result { get; set; }

        public SysApiResult()
            : this(SysResultStatus.成功, default(TResult), string.Empty)
        {
        }

        public SysApiResult(SysResultStatus status, TResult result)
            : this(status, result, string.Empty)
        {
        }

        public SysApiResult(SysResultStatus status, TResult result, string message)
        {
            this.Status = status;
            this.Result = result;
            this.Message = message;
        }

        public void DefaultError(string errorMsg)
        {
            this.Status = SysResultStatus.失败;
            this.Message = errorMsg;
        }
    }
}
