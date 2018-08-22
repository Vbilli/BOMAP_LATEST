using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
	public class EntityCollection
	{
		public EntityCollection()
		{
			this.Entities = new List<Entity>();
		}

		public IList<Entity> Entities { get; set; }
	}
}