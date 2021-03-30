using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace person.Model
{
    public class PersonContext :DbContext
    {

        public PersonContext(DbContextOptions<PersonContext> options) : base(options)
        {
            
        }
        public DbSet<PersonInfomation> PersonInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
