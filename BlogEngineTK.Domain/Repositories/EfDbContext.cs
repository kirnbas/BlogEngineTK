using BlogEngineTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngineTK.Domain.Repositories
{
    /// <summary>
    /// Entity Framework context for DataBase.
    /// Можно было бы исп-ть Singleton, но это оч не желательно (например http://www.britishdeveloper.co.uk/2011/03/dont-use-singleton-datacontexts-entity.html).
    /// </summary>
    public sealed class EfDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
    }
}
