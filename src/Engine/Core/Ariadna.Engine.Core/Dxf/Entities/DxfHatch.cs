using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ariadna.Engine.Core
{
    [Serializable]
    public class DxfHatch : IDxfEntity
    {
        [XmlAttribute] public string Color { get; set; }

        [XmlElement(ElementName = "Path")] public List<BoundaryPath> BoundaryPaths { get; set; }
    }

    [Serializable]
    public class BoundaryPath
    {
        [XmlAttribute] public string Edges { get; set; }
    }
}