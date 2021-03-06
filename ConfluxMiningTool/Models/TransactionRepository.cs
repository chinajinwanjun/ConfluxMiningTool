﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class Miner
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string miner { get; set; }
    }
    public class Transaction
    {
        [Key]
        public string transactionHash { get; set; }
        public string from { get; set; }
        public decimal value { get; set; }
        public DateTime createdTime { get; set; }
        public double timestamp { get; set; }

    }
    public class PoolHashRate
    {
        [Key]
        public long ID { get; set; }
        public string Name { get; set; }
        public long Hashrate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public interface ITransactionRepository
    {
        public void Add(Transaction Transaction);
        public void AddMiner(Miner miner);
        public List<string> GetAllHash();
        public dynamic GetNFT();
        public dynamic GetMinerList(DateTime @from, DateTime @to);
        public dynamic GetNFTBurned();
        public void AddPoolHashRate(List<PoolHashRate> poolHashRate);
    }
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext db;
        public TransactionRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Add(Transaction Transaction)
        {
            db.Transaction.Add(Transaction);
            db.SaveChanges();
        }
        public void AddMiner(Miner miner)
        {
            var Minerlist = db.Miner.ToList();
            var minerList = Minerlist.Where(x => x.CreatedDate.ToShortDateString() == DateTime.Now.ToShortDateString()).Select(x => x.miner);
            if (!minerList.Contains(miner.miner))
            {
                db.Miner.Add(new Miner
                {
                    miner = miner.miner,
                    CreatedDate = DateTime.Now,
                });
            }

            db.SaveChanges();
        }
        public void AddPoolHashRate(List<PoolHashRate> poolHashRate)
        {
            db.PoolHashRate.AddRange(poolHashRate);
            db.SaveChanges();
        }
        public List<string> GetAllHash()
        {
            return db.Transaction.Select(x => x.transactionHash).ToList();
        }
        public dynamic GetNFT()
        {
            return (from trans in db.Transaction.ToList()
                    group trans by trans.@from into g
                    select new
                    {
                        From = g.First().@from,
                        Amount = g.Sum(tr => tr.value),
                        g.First().createdTime,
                    }).ToList().Where(x => x.Amount >= 1000).OrderByDescending(x => x.createdTime).ToList();
        }
        public dynamic GetNFTBurned()
        {
            return db.Transaction.Sum(x => x.value);
        }
        public dynamic GetMinerList(DateTime @from, DateTime to)
        {
            var f = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
            var t = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
            return (from miner in db.Miner.Where(x => x.CreatedDate >= f && x.CreatedDate <= t).ToList()
                    group miner by miner.miner into g
                    select new
                    {
                        miner = g.First().miner.Substring(0, 6) + "**" + g.First().miner.Substring(g.First().miner.Length - 4, 4),
                        count = g.Count(),
                        detail = g.OrderBy(x => x.CreatedDate).Select(x => x.CreatedDate.ToShortDateString()),
                    }).ToList();
        }
    }
}
