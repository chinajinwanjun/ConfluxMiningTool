using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class MinerBlock
    {
        public int ID { get; set; }
        public string Wallet { get; set; }
        public int Block { get; set; }
        public string CreatedTime { get; set; }
    }
    public interface IMinerBlockRepository
    {
        public void Add(MinerBlock  minerBlock);
    }
    public class MinerBlockRepository : IMinerBlockRepository
    {
        private readonly AppDbContext db;
        public MinerBlockRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Add(MinerBlock minerBlock)
        {
            db.MinerBlock.Add(minerBlock);
            db.SaveChanges();
        }
  }
}
