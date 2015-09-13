using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmackdownEF6vsLinq2SQL.Linq2SQL_DAL;
using System.IO;

//http://ridilabs.net/post/2014/11/03/Unable-to-Add-Data-Connection-on-Visual-Studio-2013-after-SQL-Server-2014-Upgrade.aspx#.VfXatvT_WxA
namespace SmackdownEF6vsLinq2SQL
{
	public class Linq2SqlPerformanceTests
	{
		public Linq2SqlPerformanceTests()
		{
			using (var db = new DataClasses1DataContext())
			{
				// delete any records from previous run
				var deptQuery = (from dept in db.Departments select dept).ToList();
				foreach (var item in deptQuery)
				{
					db.Departments.DeleteOnSubmit(item);
				}
				db.SubmitChanges();

				var personQuery = (from pers in db.Persons select pers).ToList();
				foreach (var item in personQuery)
				{
					db.Persons.DeleteOnSubmit(item);
				}
				db.SubmitChanges();


				Department myDepartment = new Department()
				{
					name = "Operations"
				};

				db.Departments.InsertOnSubmit(myDepartment);
				db.SubmitChanges();
			}
		}

		public void RunnAllTests()
		{
			TestInsert();
			TestUpdate();
			TestSelect();
			TestDelete();

			/*
			INSERT:00:00:03.9266924
			UPDATE:00:00:05.5788331
			SELECT:00:00:02.4695811
			DELETE:00:00:05.0940896
			
			INSERT:00:00:03.6418322
			UPDATE:00:00:05.5173092
			SELECT:00:00:02.5170413
			DELETE:00:00:05.0632424 *
			 
			INSERT:00:00:03.6277620
			UPDATE:00:00:05.4854174
			SELECT:00:00:02.4539101 *
			DELETE:00:00:05.0786909
			 
			INSERT:00:00:03.6098774 *
			UPDATE:00:00:05.4703982 *
			SELECT:00:00:02.4693327
			DELETE:00:00:05.0642351

			INSERT:00:00:03.6774759
			UPDATE:00:00:05.5329884
			SELECT:00:00:02.4700940
			DELETE:00:00:05.1274163
			*/
		}

		public void TestInsert()
		{
			using (var db = new DataClasses1DataContext())
			{
				// read first and last names
				List<string> firstnames = new List<string>();
				using (StreamReader sr = new StreamReader(@"..\..\Data\firstnames.txt"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
						firstnames.Add(line);
				}

				List<string> lastnames = new List<string>();
				using (StreamReader sr = new StreamReader(@"..\..\Data\lastnames.txt"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
						lastnames.Add(line);
				}

				//test inserting 1000 records
				DateTime startTime = DateTime.Now;
				for (int j = 0; j < 10; j++)
				{
					for (int i = 0; i < 1000; i++)
					{
						Person personRecord = new Person()
						{
							first = firstnames[i],
							last = lastnames[i],
							department = 1
						};

						db.Persons.InsertOnSubmit(personRecord);
					}
				}

				db.SubmitChanges();
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("INSERT:" + elapsedTime.ToString());
				//3.49 seconds
			}
		}

		public void TestSelect()
		{
			using (var db = new DataClasses1DataContext())
			{
				// select records from the person joined by department table
				DateTime startTime = DateTime.Now;
				for (int i = 0; i < 1000; i++)
				{
					var query = (from p in db.Persons
								 join d in db.Departments on p.department equals d.id
								 select p).ToList();
				}
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("SELECT:" + elapsedTime.ToString());
			}
		}

		public void TestUpdate()
		{
			using (var db = new DataClasses1DataContext())
			{
				// update all records in the person table
				DateTime startTime = DateTime.Now;
				var query = (from p in db.Persons select p).ToList();
				foreach (var item in query)
				{
					item.last = item.last + "2";
				}
				db.SubmitChanges();
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("UPDATE:" + elapsedTime.ToString());
			}
		}

		public void TestDelete()
		{
			using (var db = new DataClasses1DataContext())
			{
				// delete all records in the person table
				DateTime startTime = DateTime.Now;
				var personQuery = (from pers in db.Persons select pers).ToList();
				foreach (var item in personQuery)
				{
					db.Persons.DeleteOnSubmit(item);
				}
				db.SubmitChanges();
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("DELETE:" + elapsedTime.ToString());
			}
		}
	}
}
