using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.IRepository;
using XianKaiHua.Models.Entity;

namespace XianKaiHua.IServices
{
    //public interface IPersonServices: IBaseRepository<Person>
    public interface IPersonServices : IMulti<person>
    //public interface IPersonServices : IBaseRepositoryMultlTenant<person>
    {

        public List<person> TestGetList(int pageSize, int pageIndex, string titles, out int total);
    }
}
