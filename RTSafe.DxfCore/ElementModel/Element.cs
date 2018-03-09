using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSafe.DxfCore.ElementModel
{
    public class Element
    {
        public double X { get; set; }

        public double Y { get; set; }

        public string Tag { get; set; }
        public EPoint Start { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public string LayerName { get; set; }

        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get;set; }
    }

    public class TextElement : Element
    {
        public double Size { get; set; }

        public string Value { get; set; }

        public string Fontfamily { get; set; }

        //public new string  Color { get; set; }
        //public double Height { get; set; }
    }

    public class ElementList
    {
        public List<Element> TextElements { get; set; }

        public List<Element> LineElements { get; set; }

        public List<Element> PathElements { get; set; }

        public List<Element> CircleElements { get; set; }

        public List<Element> LightWeightPolylineElements { get; set; }
    }

    public class ELayer
    {
        public string Name { get; set; }
    }


    public class Plist
    {
        public List<ElementList> PElementLists;
        public List<ELayer> LayerLists;
    }
}
