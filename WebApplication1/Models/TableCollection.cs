using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
	public class TableCollection
	{
		public TableCollection()
		{
			this.TableList = new List<Table>();
		}
		public IList<Table> TableList { get; set; }
	}
}