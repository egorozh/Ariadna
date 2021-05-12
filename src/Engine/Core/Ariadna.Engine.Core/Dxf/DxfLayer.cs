using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ariadna.Engine.Core
{
    [Serializable]
    public class DxfLayer
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string Color { get; set; }

        [XmlElement(ElementName = "LwPolyline")]
        public List<DxfLwPolyline> LwPolylines { get; set; } = new List<DxfLwPolyline>();

        [XmlElement(ElementName = "Line")] public List<DxfLine> Lines { get; set; } = new List<DxfLine>();

        [XmlElement(ElementName = "Arc")] public List<DxfArc> Arcs { get; set; } = new List<DxfArc>();

        [XmlElement(ElementName = "Circles")] public List<DxfCircle> Circles { get; set; } = new List<DxfCircle>();

        [XmlElement(ElementName = "Ellipses")] public List<DxfEllipse> Ellipses { get; set; } = new List<DxfEllipse>();

        [XmlElement(ElementName = "Hatches")] public List<DxfHatch> Hatches { get; set; } = new List<DxfHatch>();
    }
}