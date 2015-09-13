using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmackdownEF6vsLinq2SQL.NHibernate_DAL;
using NHibernate;
using NHibernate.Linq;
using System.IO;

namespace SmackdownEF6vsLinq2SQL
{
	public class NHibernatePerformanceTests
	{
		public NHibernatePerformanceTests()
		{
			using (ISession db = NHibernateHelper.OpenSession())
			{
				// delete any records from previous run
				var deptQuery = (from dept in db.Query<Department>() select dept).ToList();
				using (db.BeginTransaction())
				{
					foreach (var item in deptQuery)
					{
						db.Delete(item);
					}
					db.Transaction.Commit();
				}

				var personQuery = (from pers in db.Query<Person>() select pers).ToList();
				using (db.BeginTransaction())
				{
					foreach (var item in personQuery)
					{
						db.Delete(item);
					}
					db.Transaction.Commit();
				}

				Department myDepartment = new Department()
				{
					name = "Operations"
				};

				db.Save(myDepartment);
			}
		}

		public void RunAllTests()
		{
			TestInsert();
			TestUpdate();
			TestSelect();
			TestDelete();
			/*
			INSERT:00:00:01.5000043
			UPDATE:00:00:00.9064364
			SELECT:00:00:02.3595522
			DELETE:00:00:00.9844320

			INSERT:00:00:01.4688648
			UPDATE:00:00:00.8750664
			SELECT:00:00:02.3281309 *
			DELETE:00:00:01.0003117
			 
			INSERT:00:00:01.5316751
			UPDATE:00:00:00.8750013
			SELECT:00:00:02.3648994
			DELETE:00:00:00.9700620
			 
			INSERT:00:00:01.4240745 *
			UPDATE:00:00:01.1406272
			SELECT:00:00:02.3281312
			DELETE:00:00:00.7187527 *
			 
			INSERT:00:00:01.4375479
			UPDATE:00:00:00.8593775 *
			SELECT:00:00:02.3909192
			DELETE:00:00:00.7188431
			*/
		}

		public void TestInsert()
		{
			using (ISession db = NHibernateHelper.OpenSession())
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
				using (db.BeginTransaction())
				{
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

							db.Save(personRecord);
						}
					}
					db.Transaction.Commit();
				}
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("INSERT:" + elapsedTime.ToString());
				//1.24 seconds
			}
		}

		public void TestSelect()
		{
			using (ISession db = NHibernateHelper.OpenSession())
			{
				// select records from the person joined by department table
				DateTime startTime = DateTime.Now;
				for (int i = 0; i < 1000; i++)
				{
					var query = (from p in db.Query<Person>()
								 join d in db.Query<Department>() on p.department equals d.id
								 select p).ToList();
				}
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("SELECT:" + elapsedTime.ToString());
			}

		}


		public void TestUpdate()
		{
			using (ISession db = NHibernateHelper.OpenSession())
			{
				// update all records in the person table
				DateTime startTime = DateTime.Now;
				using (db.BeginTransaction())
				{
					var query = (from p in db.Query<Person>() select p).ToList();
					foreach (var item in query)
					{
						item.last = item.last + "2";
						db.SaveOrUpdate(item);
					}
					db.Transaction.Commit();
				}

				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("UPDATE:" + elapsedTime.ToString());
			}
		}


		public void TestDelete()
		{
			using (ISession db = NHibernateHelper.OpenSession())
			{
				// delete all records in the person table
				DateTime startTime = DateTime.Now;
				var personQuery = (from pers in db.Query<Person>() select pers).ToList();
				using (db.BeginTransaction())
				{
					foreach (var item in personQuery)
					{
						db.Delete(item);
					}
					db.Transaction.Commit();
				}
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("DELETE:" + elapsedTime.ToString());
			}
		}
	}
}
