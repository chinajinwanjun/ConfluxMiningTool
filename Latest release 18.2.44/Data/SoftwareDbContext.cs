using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Latest_release_18._2._44.Data
{
    public class Student
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }
    [System.ComponentModel.DataAnnotations.Schema.Table("DailyTrustedNode")]
    public class DailyTrustedNode
    {
        public long ID { get; set; }
        public string WalletAddress { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class DailyTrustedNodeDto
    {
        public string WalletAddress { get; set; }
        public string CreatedDate { get; set; }
        public int Value { get; set; }
    }

    public class SoftwareDbContext : DbContext
    {
        public SoftwareDbContext(DbContextOptions<SoftwareDbContext> options = null) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<DailyTrustedNode> DailyTrustedNodes { get; set; }
    }

}
