using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ExtensaoLog
{
    class Program
    {
static void Main(string[] args)
{
    using (var db = new SampleContext())
    { 

        db.Database.EnsureCreated();
        db.Set<Blog>().Add(new Blog
        {
            Name = "Rafael Almeida",
            Date = DateTime.Now
        });
        db.SaveChanges();

        //db.Set<Blog>().Where(p => p.Id > 0).ToList();

        var strSQL = db.Set<Blog>().Where(p => p.Id > 0).ToSql();
    }

    //foreach (var log in SampleContext.Logs)
    //{
    //    Console.WriteLine(log);
    //}

    Console.ReadKey();
}
    }
}
