using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmackdownEF6vsLinq2SQL.EF_DAL
{
	[Table("Person")]
	public class Person
	{
		[Key]
		public int id { get; set; }
		public string first { get; set; }
		public string last { get; set; }
		public int department { get; set; }
	}
}
