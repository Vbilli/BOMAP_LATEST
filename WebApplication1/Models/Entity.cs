using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
	public class Entity
	{
		public Entity()
		{
			this.Fields = new List<Field>();
		}

		public string Name { get; set; }

		public IList<Field> Fields { get; set; }

		public IList<string> EntityAttributeList { get; set; }

		public IList<string> EntityChildAttributeList
		{
			get
			{
				if (this.Fields.Count > 0)
				{
					IList<string> childAttributeList = new List<string>();
					foreach (Field field in this.Fields)
					{
						foreach (var attribute in field.PropertyNameList)
						{
							childAttributeList.Add(attribute);
						}

					}
					return childAttributeList.Distinct().ToList();
				}
				return new List<string>();
			}
		}

		public string XmlString { get; set; }
	}
	public class Field
	{
		public Field()
		{
			this.FieldProperties = new List<Property>();
		}

		public string Name
		{
			get
			{
				if (this.FieldProperties.Count > 0)
				{
					Property nameProperty = this.FieldProperties.Where(o => o.PropertyName == "Name").FirstOrDefault();
					if (nameProperty != null)
					{
						return nameProperty.PropertyValue;
					}
				}
				return string.Empty;
			}
		}

		public IList<string> PropertyNameList
		{
			get
			{
				if (this.FieldProperties.Count > 0)
				{
					return this.FieldProperties.Select(o => o.PropertyName).Distinct().ToList();
				}
				return new List<string>();
			}
		}

		public IList<Property> FieldProperties { get; set; }
	}

	public class Property
	{
		//public Property()
		//{
		//	this.PropertyDictionary = new Dictionary<string, string>();
		//}

		//public Property(string name, string value)
		//	: this()
		//{
		//	this.PropertyDictionary.Add(name, value);
		//}
		public string PropertyName { get; set; }
		public string PropertyValue { get; set; }
		//public Dictionary<string, string> PropertyDictionary { get; set; }
	}
}