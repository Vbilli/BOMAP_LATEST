using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using Newtonsoft.Json;
using WebApplication1.Models;
using static WebApplication1.Models.Entity;

namespace WebApplication1.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
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
			xml.Load(@"D:\MyDownloads\AT_BOL_Map.xml");
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
			var result = Entities.EntityList.Entities.Where(o => o.Name == name).FirstOrDefault();
            result.XmlString = result.XmlString.Replace("><", ">\r\n<");

            return this.PartialView("SearchResult", result);
			//return RedirectToAction("ShowEntity");
		}

        public string returnXmlString(string name)
        {
            var result = Entities.EntityList.Entities.Where(o => o.Name == name).FirstOrDefault();
            string resultStringWithoutNewLine = result.XmlString;
            return resultStringWithoutNewLine.Replace("</","/br</");
            //return RedirectToAction("ShowEntity");
        }
    }
}