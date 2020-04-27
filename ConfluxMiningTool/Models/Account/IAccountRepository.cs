using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public interface IAccountRepository
    {
        public IEnumerable<Account> GetAccounts();
        public bool Add(Account account);
    }
}
