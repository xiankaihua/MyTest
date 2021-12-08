using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianKaiHua.Models.Entity;

namespace XianKaiHua.Extensions.AutoMapper
{
    public class CustomAutoMapperProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public CustomAutoMapperProfile()
        {
            //base.CreateMap<Menu, Log>();映射实体
            //base.CreateMap<UserRole, Person>();
            //base.CreateMap<Model.Menu, Menu>();//映射
        }
    }

}
