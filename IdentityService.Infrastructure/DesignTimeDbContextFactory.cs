﻿using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdDbContext>
    {
        public IdDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdDbContext>();
            //optionsBuilder.UseSqlServer("Server=LOCALHOST\\SQLEXPRESS;Database=WYYMusic;Trusted_Connection=True;MultipleActiveResultSets=true");
            optionsBuilder.UseMySql("server=localhost;port=3306;database=WYYMusic;user=root;password=123456", new MySqlServerVersion(new Version(8, 0, 32)));
            return new IdDbContext(optionsBuilder.Options); // 这里调用TDbContext的构造函数传入配置 
        } 
    }
}
