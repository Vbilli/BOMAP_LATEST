using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;
using WebApplication1.Helper;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	public class HomeController : Controller
	{
		private static string _xmlPath;
		private static readonly string _queryFkConstraintSql = "SELECT FK_Table = FK.TABLE_NAME, FK_Column = CU.COLUMN_NAME, PK_Table = PK.TABLE_NAME, PK_Column = PT.COLUMN_NAME, Constraint_Name = C.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME INNER JOIN(SELECT i1.TABLE_NAME, i2.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY') PT ON PT.TABLE_NAME = PK.TABLE_NAME ";

		public ActionResult Index()
		{
			string xmlPath = ConfigurationManager.AppSettings["XmlPath"];
			if (!string.IsNullOrEmpty(xmlPath))
			{
				_xmlPath = xmlPath;
			}
			return View(new Entity());
		}

		public ActionResult ShowEntity(string name)
		{
			return View(Entities.EntityList.Entities.Where(o => o.Name == name).FirstOrDefault());
		}

		[HttpGet]
		public string GetAllEntityName()
		{
			IList<string> entityList = new List<string>();
			XmlDocument xml = new XmlDocument();
			xml.Load(_xmlPath);
			XmlNodeList entities = xml.GetElementsByTagName("Entity");
			XmlNodeList entitiesNode = entities;
			EntityCollection entityCollections = new EntityCollection();

			foreach (XmlNode node in entities)
			{
				XmlAttributeCollection collections = node.Attributes;
				//entityList.Add(collections["Name"].Value);
				Entity entity = new Entity
				{
					Name = collections["Name"].Value,
					XmlString = node.OuterXml
				};
				if (node.HasChildNodes)
				{
					XmlNodeList childNodes = node.ChildNodes;
					foreach (XmlNode childNode in childNodes)
					{
						if (childNode.Name == "Field")
						{
							Field field = new Field();

							foreach (XmlAttribute attribute in childNode.Attributes)
							{
								Property property = new Property()
								{
									PropertyName = attribute.Name,
									PropertyValue = attribute.Value,
								};
								field.FieldProperties.Add(property);
							}

							entity.Fields.Add(field);
						}
					}
				}
				entityCollections.Entities.Add(entity);
			}
			Entities.EntityList = entityCollections;
			return JsonConvert.SerializeObject(entityCollections);
		}

		public ActionResult SearchResult(string name)
		{
			IEnumerable<Entity> result = Entities.EntityList.Entities.Where(o => o.Name.ToLower() == name.ToLower() || (o.Fields != null && o.Fields.Count(f => f.Name.ToLower() == name.ToLower()) > 0));
			foreach (Entity entity in result)
			{
				entity.XmlString = entity.XmlString.Replace("><Field", ">\r\n   <Field").Trim();
				entity.XmlString = entity.XmlString.Replace("</Entity>", "\r\n </Entity>").Trim();
				entity.XmlString = entity.XmlString.Replace("<Enumeration", ">\r\n   <Enumeration").Trim();
				entity.XmlString = entity.XmlString.Replace("<LoadOption", ">\r\n   <LoadOption").Trim();
				entity.XmlString = entity.XmlString.Replace("<Value", ">\r\n      <Value").Trim();
				//result.XmlString = result.XmlString.Replace("<AliasProperty", ">\r\n       <AliasProperty").Trim();
				entity.XmlString = entity.XmlString.Replace("<CustomDataShapes", ">\r\n   <CustomDataShapes").Trim();
			}

			return PartialView("SearchResult", result);
		}

		public ActionResult TableSearchResult(string tableName)
		{
			DataSet dsTables = new DataSet();
			DataSet dsColumns = new DataSet();
			SqlHelper.SelectRows(dsTables, string.Format("select * from  INFORMATION_SCHEMA.COLUMNS WHERE INFORMATION_SCHEMA.COLUMNS.TABLE_NAME = '{0}'", tableName));
			SqlHelper.SelectRows(dsColumns, _queryFkConstraintSql + string.Format(" where FK.TABLE_NAME = '{0}'", tableName.Trim()));
			if (dsTables.Tables.Count > 0 && dsTables.Tables[0].Rows.Count > 0)
			{
				Table table = new Table(dsTables.Tables[0].Rows[0].ItemArray[2].ToString());
				foreach (DataRow dr in dsTables.Tables[0].Rows)
				{
					string columnName = dr.ItemArray[3].ToString();
					Column column = new Column()
					{
						Name = dr.ItemArray[3].ToString(),
						DataType = dr.ItemArray[7].ToString(),
						IsNullable = dr.ItemArray[6].ToString() == "YES" ? true : false,
						NumberPrecision = dr.ItemArray[10].ToString()
					};
					if (dsColumns.Tables.Count > 0 && dsColumns.Tables[0].Rows.Count > 0)
					{
						foreach (DataRow FK_column in dsColumns.Tables[0].Rows)
						{
							if (FK_column.ItemArray[1].ToString() == columnName)
							{
								ForeignKey fk = new ForeignKey()
								{
									Name = FK_column.ItemArray[4].ToString(),
									FKTable = FK_column.ItemArray[2].ToString(),
									FKColumn = FK_column.ItemArray[3].ToString()
								};
								column.ForeignKeys = fk;
							}
						}
					}
					table.Columns.Add(column);
				}
				return PartialView("TableSearchResult", table);
			}
			return null;
		}

		[HttpGet]
		public string GetAllTableName()
		{
			DataSet dsTables = new DataSet();
			IList<string> tableList = new List<string>();
			SqlHelper.SelectRows(dsTables, "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
			TableCollection tables = new TableCollection();
			if (dsTables.Tables.Count > 0 && dsTables.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow dr in dsTables.Tables[0].Rows)
				{
					string tableName = dr.ItemArray[2].ToString();
					if (!tableName.StartsWith("x_"))
					{
						Table table = new Table(tableName);
						tables.TableList.Add(table);
					}
				}
			}
			return JsonConvert.SerializeObject(tables);
		}
	}
}