using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace WebClients
{
    class ResultsDbContext : DbContext
    {
        public ResultsDbContext()
            : base("SamplesConnString")
        {
            
        }

        public DbSet<WebClients.RequestResult> RequestResults { get; set; }
    }
}
