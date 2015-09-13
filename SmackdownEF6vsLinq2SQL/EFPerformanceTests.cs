using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmackdownEF6vsLinq2SQL.EF_DAL;

namespace SmackdownEF6vsLinq2SQL
{
	public class EFPerformanceTests
	{
		public EFPerformanceTests()
		{
			// clean up any data from previous runs
			using (var db = new SampleDataContext())
			{
				// delete any records from previous run
				var deptQuery = (from dept in db.Departments select dept).ToList();
				db.Departments.RemoveRange(deptQuery);
				db.SaveChanges();

				var personQuery = (from pers in db.Persons select pers).ToList();
				db.Persons.RemoveRange(personQuery);
				db.SaveChanges();

				// insert one department
				Department myDepartment = new Department()
				{
					name = "Operations"
				};

				db.Departments.Add(myDepartment);
				db.SaveChanges();
			}
		}

		public void RunAllTests()
		{
			TestInsert();
			TestUpdate();
			TestSelect();
			TestDelete();

			/*
			INSERT:00:00:04.0628629
			SELECT:00:00:02.5471320
			INSERT:00:00:02.4533601
			DELETE:00:00:02.1251820

			INSERT:00:00:03.1098533
			SELECT:00:00:02.8792845
			INSERT:00:00:02.4534528
			DELETE:00:00:01.9378780 *

			INSERT:00:00:03.1252107
			SELECT:00:00:02.5001995 *
			INSERT:00:00:02.4377757
			DELETE:00:00:02.0626860

			INSERT:00:00:03.0939763 *
			SELECT:00:00:02.7505441
			INSERT:00:00:02.4064189 *
			DELETE:00:00:01.9689660

			INSERT:00:00:03.0941976
			SELECT:00:00:02.5008052
			INSERT:00:00:02.4221372
			DELETE:00:00:01.9532792
			*/
		}

		public void TestInsert()
		{
			using (var db = new SampleDataContext())
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

				db.Configuration.AutoDetectChangesEnabled = false;
				db.Configuration.ValidateOnSaveEnabled = false;

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

						db.Persons.Add(personRecord);
					}
				}

				db.SaveChanges();
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("INSERT:" + elapsedTime.ToString());
			}
		}

		public void TestSelect()
		{
			using (var db = new SampleDataContext())
			{
				DateTime startTime = DateTime.Now;
				for (int i = 0; i < 1000; i++)
				{
					var query = (from p in db.Persons
								 join d in db.Departments on p.department equals d.id
								 select p).ToList();
				}
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("INSERT:" + elapsedTime.ToString());
			}
		}

		public void TestUpdate()
		{
			using (var db = new SampleDataContext())
			{
				DateTime startTime = DateTime.Now;
				var query = (from p in db.Persons select p).ToList();
				foreach (var item in query)
				{
					item.last = item.last + "2";
				}
				db.SaveChanges();

				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("SELECT:" + elapsedTime.ToString());
			}
		}

		public void TestDelete()
		{
			using (var db = new SampleDataContext())
			{
				DateTime startTime = DateTime.Now;
				var personQuery = (from pers in db.Persons select pers).ToList();

				db.Persons.RemoveRange(personQuery);
				db.SaveChanges();
				TimeSpan elapsedTime = DateTime.Now - startTime;

				Console.WriteLine("DELETE:" + elapsedTime.ToString());
			}
		}
	}
}
