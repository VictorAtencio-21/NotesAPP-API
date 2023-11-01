using Microsoft.EntityFrameworkCore;
using NotesAPI.Models.Entities;

namespace NotesAPI.Data

{
    public class NotesDBContext: DbContext
    {
        public NotesDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Note> Notes { get; set; }
    }
}
