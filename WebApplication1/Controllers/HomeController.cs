﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using Newtonsoft.Json;
using WebApplication1.Helper;
using WebApplication1.Models;
using static WebApplication1.Models.Entity;

namespace WebApplication1.Controllers
{
	public class HomeController : Controller
	{
		private static string _xmlPath;


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
			return this.View(Entities.EntityList.Entities.Where(o => o.Name == name).FirstOrDefault());
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
				Entity entity = new Entity();
				entity.Name = collections["Name"].Value;
				entity.XmlString = node.OuterXml;
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
			var result = Entities.EntityList.Entities.Where(o => o.Name.ToLower() == name.ToLower() || (o.Fields != null && o.Fields.Count(f => f.Name.ToLower() == name.ToLower()) > 0));
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

			return this.PartialView("SearchResult", result);
		}

		public ActionResult TableSearchResult(string tableName)
		{
			DataSet dsTables = new DataSet();
			SqlHelper.SelectRows(dsTables, string.Format("select * from  INFORMATION_SCHEMA.COLUMNS WHERE INFORMATION_SCHEMA.COLUMNS.TABLE_NAME = '{0}'", tableName));
			if (dsTables.Tables.Count > 0 && dsTables.Tables[0].Rows.Count > 0)
			{
				Table table = new Table(dsTables.Tables[0].Rows[0].ItemArray[2].ToString());
				foreach (DataRow dr in dsTables.Tables[0].Rows)
				{
					Column column = new Column()
					{
						Name = dr.ItemArray[3].ToString(),
						DataType = dr.ItemArray[7].ToString(),
						IsNullable = dr.ItemArray[6].ToString() == "YES" ? true : false,
						NumberPrecision = dr.ItemArray[10].ToString()
					};
					table.Columns.Add(column);
				}
				return this.PartialView("TableSearchResult", table);
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