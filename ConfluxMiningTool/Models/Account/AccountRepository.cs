using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext db;
        public AccountRepository(AppDbContext db)
        {
            this.db = db;
        }
        public IEnumerable<Account> GetAccounts()
        {
            return db.Account.ToList();
        }
        public bool Add(Account account)
        {
            try
            {
                if (db.Account.FirstOrDefault(x=>x.Address== account.Address)==null)
                {
                    db.Account.Add(account);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
