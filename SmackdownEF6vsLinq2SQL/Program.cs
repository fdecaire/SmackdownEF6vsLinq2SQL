using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmackdownEF6vsLinq2SQL
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("EF:");
			var EFTests = new EFPerformanceTests();
			EFTests.RunAllTests();

			Console.WriteLine("Linq-to-SQL:");
			var Linq2SqlTests = new Linq2SqlPerformanceTests();
			Linq2SqlTests.RunnAllTests();

			Console.WriteLine("NHibernate:");
			var NHibernateTests = new NHibernatePerformanceTests();
			NHibernateTests.RunAllTests();

			Console.ReadLine();


		}
	}
}
