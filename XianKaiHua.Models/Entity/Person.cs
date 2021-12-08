using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianKaiHua.Models.Entity
{

    //[TenantAttribute("postgredb")]
    //[TenantAttribute("multidb1")]
    [SugarTable("person", "练习之用户记录表")]
    public class person
    {

        [SugarColumn(ColumnName = "tid", ColumnDescription = "用户ID", ColumnDataType = "varchar(36)")]
        public string tid { get; set; }

        [SugarColumn(ColumnName = "name", ColumnDescription = "用户ID", ColumnDataType = "varchar(50)")]
        public string name { get; set; }
    }
}
