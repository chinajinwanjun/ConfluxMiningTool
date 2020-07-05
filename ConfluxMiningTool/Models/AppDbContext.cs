using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<BalanceHistory> BalanceHistory { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<TrustNode> TrustNode { get; set; }
        public DbSet<ActiveTrustNode> ActiveTrustNode { get; set; }

    }
}
