using System;
using System.Xml.Serialization;

namespace Ariadna.Engine.Core
{
    [Serializable]
    public class DxfArc : IDxfEntity
    {
        [XmlAttribute] public string Data { get; set; }

        [XmlAttribute] public string Color { get; set; }
    }
}