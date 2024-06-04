using Microsoft.EntityFrameworkCore;
using Profunion.Models;

namespace Profunion.Data
{
    public class MessagesContext : DbContext
    {
        public MessagesContext(DbContextOptions<MessagesContext> options) : base(options)
        {

        }

        public DbSet<Messages> Message { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Messages>()
              .HasOne(m => m.Initiator)
              .WithMany()
              .HasForeignKey(m => m.InitiatorID)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Messages>()
              .HasOne(m => m.Recipient)
              .WithMany()
              .HasForeignKey(m => m.RecipientID)
              .OnDelete(DeleteBehavior.Restrict);


        }



    }
}
