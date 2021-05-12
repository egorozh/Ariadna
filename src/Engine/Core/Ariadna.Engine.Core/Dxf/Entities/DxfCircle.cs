using System;
using System.Xml.Serialization;

namespace Ariadna.Engine.Core
{
    [Serializable]
    public class DxfCircle : IDxfEntity
    {
        [XmlAttribute] public string Data { get; set; }

        [XmlAttribute] public string Color { get; set; }
    }
}