using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
	public class Table
	{
		public Table()
		{
			this.Columns = new List<Column>();
		}

		public Table(string name) : this()
		{
			this.Name = name;
		}
		public IList<Column> Columns
		{
			get; set;
		}

		public string Name { get; set; }
	}


	public class Column
	{
		public string Name { get; set; }
		public string DataType { get; set; }

		public bool IsNullable { get; set; }

		public string NumberPrecision { get; set; }

		public ForeignKey ForeignKeys { get; set; }
	}

	public class ForeignKey
	{
		public string Name { get; set; }

		public string FKTable { get; set; }

		public string FKColumn { get; set; }
	}
}