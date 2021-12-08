using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.IServices;
using XianKaiHua.Models.Entity;
using XianKaiHua.Repository;

namespace XianKaiHua.Services
{
    //public class PersonServices : BaseRepository<Person> , IPersonServices
    public class PersonServices : MultiTenant<Person>, IPersonServices
    {



    }
}
