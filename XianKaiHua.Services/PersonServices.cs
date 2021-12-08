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
    public class PersonServices : Multi<person>, IPersonServices
    //public class PersonServices : BaseRepositoryMultlTenant<person>, IPersonServices
    {

        public List<person>  TestGetList(int pageSize, int pageIndex, string titles, out int total)
        {
            total = 0;

            var list = MultiSqlClient.Queryable<person>()
                .Where((multi) => multi.tid != null)
                .WhereIF(!string.IsNullOrEmpty(titles), (multi) =>multi.name.ToLower().Contains(titles))
                .ToPageList(pageIndex, pageSize, ref total);
            return list;
        }


        //public static String fieldToProperty(String field)
        //{
        //    if (null == field)
        //    {
        //        return "";
        //    }
        //    char[] chars = field.ToArray();
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < chars.Length; i++)
        //    {
        //        char c = chars[i];
        //        if (c == '_')
        //        {
        //            int j = i + 1;
        //            if (j < chars.Length)
        //            {
        //                sb.Append( chars[j].ToString().ToUpper());
        //                i++;
        //            }
        //        }
        //        else
        //        {
        //            sb.Append(c);
        //        }
        //    }
        //    return sb.ToString();
        //}
    }
}
