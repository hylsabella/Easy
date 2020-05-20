using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Security
{
    public interface IAuthUserService
    {
        UserModel GetByToken(string token);
    }
}
