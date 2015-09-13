using System;
using System.Data.Entity;

namespace SmackdownEF6vsLinq2SQL.EF_DAL
{
	public class SampleDataContext : DbContext, IDisposable
	{
		public SampleDataContext()
			: base(@"My Connection String")
		{

		}

		public DbSet<Department> Departments { get; set; }
		public DbSet<Person> Persons { get; set; }
	}
}
