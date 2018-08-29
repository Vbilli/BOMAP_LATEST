using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
	}
}