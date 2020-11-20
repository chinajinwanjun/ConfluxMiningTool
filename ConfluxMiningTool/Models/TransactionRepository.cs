using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class Transaction
    {
        [Key]
        public string transactionHash { get; set; }
        public string from { get; set; }
        public decimal value { get; set; }
        public DateTime createdTime { get; set; }
    }
    public interface ITransactionRepository
    {
        public void Add(Transaction Transaction);
        public List<string> GetAllHash();
        public dynamic GetNFT();
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
            Transaction.createdTime = DateTime.Now;
            db.Transaction.Add(Transaction);
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
    }
}
