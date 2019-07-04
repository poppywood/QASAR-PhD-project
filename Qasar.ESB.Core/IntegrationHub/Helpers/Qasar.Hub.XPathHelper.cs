using System;
using System.Data;
using System.Configuration;
using System.Xml;

namespace Qasar.ESB.Helpers
{
    /// <summary>
    /// This class provides useful methods for managing XPath 
    /// </summary>
    public static class XPathHelper
    {
        /// <summary>
        /// Updates an element
        /// </summary>
        /// <param name="xml">XmlDocument to edit</param>
        /// <param name="value">value to apply</param>
        /// <param name="path">XPath to element</param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static XmlDocument UpdateElement(XmlDocument xml, string value, string path, XmlNamespaceManager mgr)
        {
            //TODO: extend it so that if element doesn't exist it is added
            XmlNode node = xml.SelectSingleNode(path, mgr);
            node.InnerXml = value;
            return xml;
        }

        /// <summary>
        /// Reads an element
        /// </summary>
        /// <param name="xml">XmlDocument to read</param>
        /// <param name="path">Xpath to element</param>
        /// <param name="mgr"></param>
        /// <returns>The inner xml</returns>
        public static string ReadElement(XmlDocument xml, string path, XmlNamespaceManager mgr)
        {
            XmlNode node = xml.SelectSingleNode(path, mgr);
            return node.InnerXml;
        }

        /// <summary>
        /// Remove specified child node 
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="path">Node containing child</param>
        /// <param name="child">Child</param>
        /// <param name="mgr">Namespace mgr</param>
        /// <returns></returns>
        public static XmlDocument RemoveElement(XmlDocument xml, string path, string child, XmlNamespaceManager mgr)
        {
            XmlNode node = xml.SelectSingleNode(path, mgr);
            XmlNode childNode = node.SelectSingleNode(child, mgr);
            node.RemoveChild(childNode);
            return xml;
        }

        /// <summary>
        /// Adds a new element
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="name">Name of new element</param>
        /// <param name="value">Value of element</param>
        /// <param name="path">XPath pointing to node in which to insert Element</param>
        /// <param name="nextpath">Optional XPath pointing to sibling node following new Element. If a null is supplied
        /// the method will just append the new element at the end of any other elements</param>
        /// <param name="mgr">Namespace manager</param>
        /// <returns></returns>
        public static XmlDocument AddElement(XmlDocument xml, string name, string value, string path, string nextpath, XmlNamespaceManager mgr)
        {
            //if the node already exists it is replaced.
            XmlNode node = xml.SelectSingleNode(path, mgr);
            XmlElement elem = xml.CreateElement(name);
            elem.InnerXml = value;
            if (nextpath != null)
            {
                XmlNode next = xml.SelectSingleNode(nextpath, mgr);
                if (next != null)
                {
                    node.InsertBefore(elem, next);
                }
                else
                {
                    node.AppendChild(elem);
                }
            }
            else
                node.AppendChild(elem);
            return xml;
        }

        /// <summary>
        /// Attaches a new element After the node specified
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="name">Name of new element</param>
        /// <param name="value">Value of element</param>
        /// <param name="path">XPath pointing to node in which to insert Element</param>
        /// <param name="nextpath">Optional XPath pointing to  node preceding new Element</param>
        /// <param name="mgr">Namespace manager</param>
        /// <returns></returns>
        public static XmlDocument AttachElement(XmlDocument xml, string name, string value, string path, string prevpath, XmlNamespaceManager mgr)
        {
            //if the node already exists it is replaced.
            XmlNode node = xml.SelectSingleNode(path, mgr);
            XmlElement elem = xml.CreateElement(name);
            elem.InnerXml = value;
            if (prevpath != null)
            {
                XmlNode prev = xml.SelectSingleNode(prevpath, mgr);
                if (prev != null)
                {
                    node.InsertAfter(elem, prev);
                }
                else
                {
                    node.AppendChild(elem);
                }
            }
            else
                node.AppendChild(elem);
            return xml;
        }

        /// <summary>
        /// Adds a new element to each of many existing elements
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="name">Name of new element</param>
        /// <param name="value">Value of element</param>
        /// <param name="path">XPath pointing to nodes in which to insert Element</param>
        /// <param name="nextpath">Optional XPath pointing to sibling node following new Element. If a null is supplied
        /// the method will just append the new element at the end of any other elements</param>
        /// <param name="mgr">Namespace manager</param>
        /// <returns></returns>
        public static XmlDocument AddManyElements(XmlDocument xml, string name, string value, string path, string nextpath, XmlNamespaceManager mgr)
        {
            XmlNodeList nodes = xml.SelectNodes(path, mgr);
            
            if (nextpath != null)
            {
                XmlNode next = xml.SelectSingleNode(nextpath, mgr);
                if (next != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement elem = xml.CreateElement(name);
                        elem.InnerXml = value;
                        node.InsertBefore(elem, next);
                    }
                }
                else //the insert before node doesn't exist so just append
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement elem = xml.CreateElement(name);
                        elem.InnerXml = value;
                        node.AppendChild(elem);
                    }
                }
            }
            else
                foreach (XmlNode node in nodes)
                {
                    XmlElement elem = xml.CreateElement(name);
                    elem.InnerXml = value;
                    node.AppendChild(elem);
                }
            return xml;
        }

        /// <summary>
        /// Attaches a new element to each of many existing elements, after the node specified
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="name">Name of new element</param>
        /// <param name="value">Value of element</param>
        /// <param name="path">XPath pointing to nodes in which to insert Element</param>
        /// <param name="nextpath">Optional XPath pointing to node preceding new Element.</param>
        /// <param name="mgr">Namespace manager</param>
        /// <returns></returns>
        public static XmlDocument AttachManyElements(XmlDocument xml, string name, string value, string path, string prevpath, XmlNamespaceManager mgr)
        {
            XmlNodeList nodes = xml.SelectNodes(path, mgr);
            if (prevpath != null)
            {
                XmlNode prev = xml.SelectSingleNode(prevpath, mgr);
                if (prev != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement elem = xml.CreateElement(name);
                        elem.InnerXml = value;
                        node.InsertBefore(elem, prev);
                    }
                }
                else //the insert prev node doesn't exist so just append
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement elem = xml.CreateElement(name);
                        elem.InnerXml = value;
                        node.AppendChild(elem);
                    }
                }
            }
            else
                foreach (XmlNode node in nodes)
                {
                    XmlElement elem = xml.CreateElement(name);
                    elem.InnerXml = value;
                    node.AppendChild(elem);
                }
            return xml;
        }
                
        /// <summary>
        /// Adds a new attribute
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="name">Name of new attribute</param>
        /// <param name="value">Value of attribute</param>
        /// <param name="path">XPath pointing to node in which to insert Attribute</param>
        /// the method will just append the new element at the end of any other elements</param>
        /// <param name="mgr">Namespace manager</param>
        /// <returns></returns>
        public static XmlDocument AddAttribute(XmlDocument xml, string name, string value, string path, XmlNamespaceManager mgr)
        {
            XmlNode node = xml.SelectSingleNode(path, mgr);
            XmlAttribute att = xml.CreateAttribute(name);
            att.InnerXml = value;
            node.Attributes.Append(att);
            return xml;
        }

        /// <summary>
        /// Adds a new attribute of type xsi
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="name">Name of new attribute</param>
        /// <param name="value">Value of attribute</param>
        /// <param name="path">XPath pointing to node in which to insert Attribute</param>
        /// the method will just append the new element at the end of any other elements</param>
        /// <param name="mgr">Namespace manager</param>
        /// <returns></returns>
        public static XmlDocument AddXsiAttribute(XmlDocument xml, string name, string value, string path, XmlNamespaceManager mgr)
        {
            XmlNode node = xml.SelectSingleNode(path, mgr);
            XmlAttribute att = xml.CreateAttribute("xsi", name, "http://www.w3.org/2001/XMLSchema-instance");
            att.InnerXml = value;
            node.Attributes.Append(att);
            return xml;
        }

        /// <summary>
        /// Adds a new attribute to each of many nodes
        /// </summary>
        /// <param name="xml">XmlDocument</param>
        /// <param name="name">Name of new attribute</param>
        /// <param name="value">Value of attribute</param>
        /// <param name="path">XPath pointing to node in which to insert Attribute</param>
        /// the method will just append the new element at the end of any other elements</param>
        /// <param name="mgr">Namespace manager</param>
        /// <returns></returns>
        public static XmlDocument AddManyAttributes(XmlDocument xml, string name, string value, string path, XmlNamespaceManager mgr)
        {
            XmlNodeList nodes = xml.SelectNodes(path, mgr);
            
            foreach (XmlNode node in nodes)
            {
                XmlAttribute att = xml.CreateAttribute(name);
                att.InnerXml = value;
                node.Attributes.Append(att);
            }
            return xml;
        }
    }
}
