using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmackdownEF6vsLinq2SQL.EF_DAL
{
	[Table("Department")]
	public class Department
	{
		[Key]
		public int id { get; set; }
		public string name { get; set; }
	}
}
