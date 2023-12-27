using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using CutTheRope.game;
using Microsoft.Xna.Framework;

namespace CutTheRope.ios
{
	internal class XMLNode
	{
		private int depth;

		private XMLNode parent;

		private List<XMLNode> childs_;

		private string name;

		private NSString value;

		private Dictionary<string, string> attributes_;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public NSString data
		{
			get
			{
				return value;
			}
		}

		public NSString this[string key]
		{
			get
			{
				string rhs = null;
				if (!attributes_.TryGetValue(key, out rhs))
				{
					return new NSString("");
				}
				return new NSString(rhs);
			}
		}

		public XMLNode()
		{
			parent = null;
			childs_ = new List<XMLNode>();
			attributes_ = new Dictionary<string, string>();
		}

		public bool attributes()
		{
			if (attributes_ != null)
			{
				return attributes_.Count > 0;
			}
			return false;
		}

		public List<XMLNode> childs()
		{
			return childs_;
		}

		public XMLNode findChildWithTagNameAndAttributeNameValueRecursively(string tag, string attrName, string attrVal, bool recursively)
		{
			if (childs() == null)
			{
				return null;
			}
			foreach (XMLNode item in childs_)
			{
				string text;
				if (item.name == tag && item.attributes() && item.attributes_.TryGetValue(attrName, out text) && text == attrVal)
				{
					return item;
				}
				if (recursively && item.childs() != null)
				{
					XMLNode xMLNode = item.findChildWithTagNameRecursively(tag, recursively);
					if (xMLNode != null)
					{
						return xMLNode;
					}
				}
			}
			return null;
		}

		public XMLNode findChildWithTagNameRecursively(NSString tag, bool recursively)
		{
			return findChildWithTagNameRecursively(tag.ToString(), recursively);
		}

		public XMLNode findChildWithTagNameRecursively(string tag, bool recursively)
		{
			if (childs() == null)
			{
				return null;
			}
			foreach (XMLNode item in childs_)
			{
				if (item.name == tag)
				{
					return item;
				}
				if (recursively && item.childs() != null)
				{
					XMLNode xMLNode = item.findChildWithTagNameRecursively(tag, recursively);
					if (xMLNode != null)
					{
						return xMLNode;
					}
				}
			}
			return null;
		}

		private static XMLNode ReadNode(XmlReader textReader, XMLNode parent)
		{
			while (textReader.NodeType != XmlNodeType.Element && textReader.Read())
			{
			}
			if (textReader.NodeType == XmlNodeType.Element)
			{
				XMLNode xMLNode = new XMLNode();
				if (parent != null)
				{
					xMLNode.parent = parent;
					parent.childs_.Add(xMLNode);
				}
				xMLNode.name = textReader.Name;
				xMLNode.depth = textReader.Depth;
				if (textReader.HasAttributes)
				{
					while (textReader.MoveToNextAttribute())
					{
						xMLNode.attributes_.Add(textReader.Name, textReader.Value);
					}
					textReader.MoveToElement();
				}
				bool flag = false;
				try
				{
					xMLNode.value = new NSString(textReader.ReadElementContentAsString());
				}
				catch (Exception)
				{
					flag = true;
				}
				while ((flag || textReader.Read()) && textReader.Depth > xMLNode.depth)
				{
					ReadNode(textReader, xMLNode);
				}
				return xMLNode;
			}
			return null;
		}

		public static XMLNode parseXML(string fileName)
		{
			return ParseLINQ(fileName);
		}

		private static XMLNode ReadNodeLINQ(XElement nodeLinq, XMLNode parent)
		{
			XMLNode xMLNode = new XMLNode();
			if (parent != null)
			{
				xMLNode.parent = parent;
				parent.childs_.Add(xMLNode);
			}
			xMLNode.name = nodeLinq.Name.ToString();
			string text = (string)nodeLinq;
			if (text != null)
			{
				xMLNode.value = new NSString(text);
			}
			IEnumerable<XAttribute> enumerable = nodeLinq.Attributes();
			foreach (XAttribute item in enumerable)
			{
				xMLNode.attributes_.Add(item.Name.ToString(), item.Value);
			}
			IEnumerable<XElement> enumerable2 = nodeLinq.Elements();
			foreach (XElement item2 in enumerable2)
			{
				ReadNodeLINQ(item2, xMLNode);
			}
			return xMLNode;
		}

		private static XMLNode ParseLINQ(string fileName)
		{
			XDocument xDocument = null;
			try
			{
				Stream stream = TitleContainer.OpenStream("content/" + ResDataPhoneFull.ContentFolder + fileName);
				xDocument = XDocument.Load(stream);
				stream.Dispose();
			}
			catch (Exception)
			{
			}
			if (xDocument == null)
			{
				xDocument = XDocument.Parse(ResDataPhoneFull.GetXml(fileName));
			}
			IEnumerable<XElement> source = xDocument.Elements();
			return ReadNodeLINQ(source.First(), null);
		}
	}
}
