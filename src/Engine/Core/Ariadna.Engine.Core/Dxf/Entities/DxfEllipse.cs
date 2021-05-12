using System;
using System.Xml.Serialization;

namespace Ariadna.Engine.Core
{
    [Serializable]
    public class DxfEllipse : IDxfEntity
    {
        [XmlAttribute] public string Color { get; set; }

        [XmlAttribute] public string Data { get; set; }
    }
}