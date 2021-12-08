using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianKaiHua.IRepository
{
    public interface IDbsContext<T> where T:class,new()
    {

    }
}
