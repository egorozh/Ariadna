using System;
using System.Windows;
using System.Xml.Serialization;

namespace Ariadna.Engine.Core
{
    [Serializable]
    public class DxfLine : IDxfEntity
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        [XmlAttribute] public string Color { get; set; }
    }
}