﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using RTSafe.DxfCore;
using RTSafe.DxfCore.DxfCore.Model;
using RTSafe.DxfCore.DxfCore.Tables;
using RTSafe.DxfCore.ElementModel;
using RTSafe.DxfCore.Entities;

using Line = RTSafe.DxfCore.Entities.Line;
using Point = RTSafe.DxfCore.DxfCore.Model.Point;
using Text = RTSafe.DxfCore.Entities.Text;

namespace RtSafe.DxfCore
{

    public class DxfHelperReader
    {

        /// <summary>
        /// dxf底图中左下X值
        /// </summary>
        private double MinX { get; set; }

        /// <summary>
        /// dxf底图右上Y值
        /// </summary>
        private double MaxY { get; set; }

        /// <summary>
        /// 显示加载底图的容器
        /// </summary>
        private Canvas _container;

        public DxfHelperReader(Canvas container)
        {
            _container = container;
        }

        public DxfHelperReader()
        {

        }

        /// <summary>
        /// Dxf可见的图层列表
        /// </summary>
        public List<SvyLayer> Layers { get; set; }

        /// <summary>
        /// 读取加载dxf底图文件
        /// </summary>
        /// <param name="ms">文件流</param>
        /// <param name="type"></param>
        /// <param name="layerName"></param>
        public List<Plist> Read(System.IO.Stream ms, int type,string layerName)
        {
            try
            {
                DxfDocument doc = new DxfDocument();
                doc.Load(ms);

                this.MinX = 50;//doc.ExtMinPoint.X - doc.ExtMaxPoint.X + doc.ExtMinPoint.X;
                this.MaxY = 2 * (doc.ExtMaxPoint.Y - doc.ExtMinPoint.Y) - 700;
                if (MaxY > 10000 || doc.ExtMaxPoint.X - doc.ExtMinPoint.X > 10000)
                {
                    throw new DxfException("Dxf的图形界限长宽不能超过10000");
                }

                Layers = doc.Layers.Where(p => p.IsVisible).Select(p => new SvyLayer()
                {
                    ID = null,
                    IsClose = false,
                    LayerName = p.Name,
                    Type = 0
                }).ToList();

                Dictionary<int, string> dictionary = new Dictionary<int, string>();
                var i = 1;

                List<string> list  = new List<string>();


                Layers.ForEach(c =>
                {
                    //dictionary.Add(i, c.LayerName);

                    list.Add(c.LayerName);
                    //++i;
                });

                if (layerName.Equals("all"))
                {
                    return LoadShaps(doc.Shaps, type, list);
                }

                var dd = dictionary.ToList().Where(c => c.Value == layerName);
                list = list.Where(c => c.Equals(layerName)).ToList();
                //var bb = dictionary;

                return LoadShaps(doc.Shaps, type, list);

            }
            catch (DxfException de)
            {
                throw de;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 加载图形
        /// </summary>
        /// <param name="shaps"></param>
        /// <param name="type"></param>
        /// <param name="layerlist"></param>
        private List<Plist> LoadShaps(ReadOnlyCollection<IEntityObject> shaps, int type, List<string> layerlist)
        {
            var dic = new Dictionary<string, List<ElementList>>();

            List<Plist> list = new List<Plist>();
            List<Plist> plist = new List<Plist>();
            layerlist.ForEach(s =>
            {

                var entityTypes = shaps.Where(c => c.Layer.Name == s).ToList();

                Element e = new Element();
                Plist p = new Plist();
                ElementList elementList = new ElementList();
                List<Element> textElements = new List<Element>();
                List<Element> lineElements = new List<Element>();

                List<Element> pathElements = new List<Element>();
                List<Element> circleElements = new List<Element>();
                List<Element> lightWeightPolylineElements = new List<Element>();
                ELayer layer = new ELayer();
                foreach (var item in entityTypes)
                {

                    switch (item.Type)
                    {

                        case EntityType.Arc:
                            e = LoadArc(item as Arc, type);

                            if (e != null)
                            {
                                pathElements.Add(e);

                            }
                            break;
                        case EntityType.Circle:
                            e = LoadCircle(item as Circle, type);

                            if (e != null)
                            {
                                circleElements.Add(e);
                            }

                            break;
                        case EntityType.LightWeightPolyline:
                            e = LoadWpLine(item as LightWeightPolyline, type);

                            if (e != null)
                            {
                                lightWeightPolylineElements.Add(e);
                            }
                            break;
                        case EntityType.Line:

                            e = LoadLine(item as Line, type);

                            if (e != null)
                            {

                                lineElements.Add(e);
                            }
                            break;
                        case EntityType.Text:
                            e = LoadText(item as Text, type);
                            if (e != null)
                            {

                                textElements.Add(e);

                            }
                            break;
                        case EntityType.Polyline:
                            break;
                        case EntityType.Polyline3d:
                            break;
                        case EntityType.PolyfaceMesh:
                            break;
                        case EntityType.NurbsCurve:
                            break;
                        case EntityType.Ellipse:
                            break;
                        case EntityType.Point:
                            break;
                        case EntityType.Face3D:
                            break;
                        case EntityType.Solid:
                            break;
                        case EntityType.Insert:
                            break;
                        case EntityType.Hatch:
                            break;
                        case EntityType.Attribute:
                            break;
                        case EntityType.AttributeDefinition:
                            break;
                        case EntityType.LightWeightPolylineVertex:
                            break;
                        case EntityType.PolylineVertex:
                            break;
                        case EntityType.Polyline3dVertex:
                            break;
                        case EntityType.PolyfaceMeshVertex:
                            break;
                        case EntityType.PolyfaceMeshFace:
                            break;
                        case EntityType.Dimension:
                            break;
                        case EntityType.Vertex:
                            break;
                        case EntityType.Spline:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                }

                if (textElements.Count != 0)
                {
                    elementList.TextElements = textElements;

                }
                if (lineElements.Count != 0)
                {
                    elementList.LineElements = lineElements;
                   
                }
                if (pathElements.Count != 0)
                {
                    elementList.PathElements = pathElements;
                 
                }

                if (circleElements.Count != 0)
                {
                    elementList.CircleElements = circleElements;
                    
                }

                if (lightWeightPolylineElements.Count != 0)
                {
                    elementList.LightWeightPolylineElements = lightWeightPolylineElements;
                   
                }


                List<ElementList> elementLists = new List<ElementList> {elementList};
                List<ELayer> layers = new List<ELayer>();
                layer.Name = s;
                layers.Add(layer);
                p.PElementLists = elementLists;
                p.LayerLists = layers;
                list.Add(p);
            });

            return list;
        }







        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="aciColor">当前颜色</param>
        /// <param name="layer">图层颜色</param>
        /// <param name="type"></param>
        /// <returns></returns>
        private SolidBrush GetBrush(AciColor aciColor, Layer layer, int type)
        {
            if (aciColor.Index == 256)
            {
                aciColor = layer.Color;
            }

            if (type.Equals((int)Type.One))
            {
                if (aciColor.Index == AciColor.Default.Index)
                {
                    return new SolidBrush(Color.White);
                }
            }
            else
            {
                if (aciColor.Index == AciColor.Default.Index)
                {
                    return new SolidBrush(Color.Black);
                }
            }
            return new SolidBrush(aciColor.ToColor());
        }


        /// <summary>
        /// 加载文本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextElement LoadText(Text text, int type)
        {
            var color = GetBrush(text.Color, text.Layer, type).Color;
            TextElement textElement = new TextElement
            {
                Value = text.Value,
                R = color.R,
                G = color.G,
                B = color.B,
                //Color = GetBrush(text.Color, text.Layer, type),
                Fontfamily = text.Style.Font,
                Size = text.Style.Height,
                Height = text.Height,
                LayerName = text.Layer.Name
            };

            //new SolidBrush(Color.Black);//text.Color.ToColor().Name;////
            if (text.Rotation != 0)
            {
                //RotateTransform r = new RotateTransform();
                //r.Angle = -text.Rotation;
                //t.RenderTransform = r;
                //r.CenterY = text.Height;
            }

            textElement.X = text.BasePoint.X - MinX;
            textElement.Y = MaxY - text.BasePoint.Y + text.Height / 2;


            return textElement;
        }

        /// <summary>
        /// 加载线段
        /// </summary>
        /// <param name="line"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Element LoadLine(RTSafe.DxfCore.Entities.Line line, int type)
        {
            var color = GetBrush(line.Color, line.Layer, type).Color;


            LineElement lineElement = new LineElement
            {
                Tag = line.Layer.Name,
                R = color.R,
                G = color.G,
                B = color.B,
                X = line.StartPoint.X - MinX,
                Y = MaxY - line.StartPoint.Y,
                X2 = line.EndPoint.X - MinX,
                Y2 = MaxY - line.EndPoint.Y,
                LayerName = line.Layer.Name
            };

            //line.Color.ToColor().Name;

            //line1.StrokeDashArray = new DoubleCollection();
            foreach (var item in line.LineType.Segments)
            {
                //line1.StrokeDashArray.Add(item);
            }
            return lineElement;
        }


        /// <summary>
        /// 线
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        //private LineElement LoadLine(RTSafe.DxfCore.Entities.Polyline line)
        //{
        //    Polyline p = new Polyline();


        //    LineElement lineElement = new LineElement() { };
        //    p.Tag = line.Layer.Name;
        //    //var points = new PointCollection();
        //    foreach (var point in line.Vertexes)
        //    {
        //        //points.Add(new Point(point.Location.X - MinX, MaxY - point.Location.Y));

        //        lineElement.List.Add(new EPoint(point.Location.X - MinX, MaxY - point.Location.Y));
        //    }
        //    if (line.Flags == RTSafe.DxfCore.Entities.PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM)
        //    {
        //        //points.Add(points[0]);
        //        lineElement.List.Add(lineElement.List[0]);
        //    }
        //    //p.StrokeThickness = GetThickness(line.Thickness);
        //    //p.Stroke = GetBrush(line.Color, line.Layer);
        //    return lineElement;
        //}





        private void SetLocal(Ellipse item, Vector3f vector3f)
        {
            Canvas.SetLeft(item, vector3f.X - MinX);
            Canvas.SetTop(item, MaxY - vector3f.Y);
        }


        /// <summary>
        /// 添加点对象
        /// </summary>
        /// <param name="item"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        /// <summary>
        /// 多线段
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 圆弧
        /// </summary>
        /// <returns></returns>
        private Element LoadArc(RTSafe.DxfCore.Entities.Arc item, int type)
        {

            var color = GetBrush(item.Color, item.Layer, type).Color;


            PathElement pathElement = new PathElement
            {
                Tag = item.Layer.Name,
                R = color.R,
                G = color.G,
                B = color.B,
                StartAngle = item.StartAngle,
                EndAngle = item.EndAngle,
                LayerName = item.Layer.Name
            };
            if (item.Center.X.Equals(269.001501586602d))
            {
                pathElement.IsLargeArc = true;
            }

            pathElement.X = item.Center.X - MinX;
            pathElement.Y = MaxY - item.Center.Y;
            var c = item.EndAngle - item.StartAngle;
            ////若果结束角度小于开始角度，弧长为差值加2*pi
            if (c < 0)
            {
                c += 360;
            }

            if (c > 180)
            {
                pathElement.IsLargeArc = true;
            }
            pathElement.RotationAngle = c;
            pathElement.Radius = item.Radius;
            return pathElement;
        }
        /// <summary>
        /// 椭圆
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        //private FrameworkElement LoadEllipse(Ellipse item)
        //{
        //    Ellipse e = new Ellipse();
        //    e.Tag = item.Layer.Name;

        //    e.Height = item.MajorAxis;
        //    e.Width = (int) item.MinorAxis;

        //    //e.Height = e.Width = 2 * item.Radius;
        //    e.StrokeThickness = GetThickness(item.Thickness);
        //    if (item.Color.Index == 256)
        //    {
        //        item.Color = item.Layer.Color;
        //    }
        //    e.Stroke = GetBrush(item.Color, item.Layer);
        //    foreach (var i in item.LineType.Segments)
        //    {
        //        e.StrokeDashArray.Add(i);
        //    }
        //    Canvas.SetLeft(e, item.Center.X - item.MajorAxis / 2 - this.MinX);
        //    Canvas.SetTop(e, MaxY - item.Center.Y - item.MinorAxis / 2);
        //    return e;
        //}


        /// <summary>
        /// 加载实线
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        //private FrameworkElement LoadSolid(Solid item)
        //{
        //    Polygon p = new Polygon();
        //    p.Tag = item.Layer.Name;
        //    p.Fill = GetBrush(item.Color, item.Layer);
        //    p.Points = new PointCollection();
        //    p.Points.Add(Vector3fToPoint(item.FirstVertex));
        //    p.Points.Add(Vector3fToPoint(item.SecondVertex));
        //    p.Points.Add(Vector3fToPoint(item.ThirdVertex));
        //    p.Points.Add(Vector3fToPoint(item.FourthVertex));
        //    p.Points.Add(Vector3fToPoint(item.FirstVertex));
        //    return p;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector3f"></param>
        /// <returns></returns>
        private Point Vector3fToPoint(RTSafe.DxfCore.Vector3f vector3f)
        {

            return new Point(vector3f.X - MinX, MaxY - vector3f.Y);
        }
        private Point Vector2fToPoint(Vector2f vector2f)
        {
            return new Point(vector2f.X - MinX, MaxY - vector2f.Y);
        }
        /// <summary>
        /// 块
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        //private Canvas LoadBlock(RTSafe.DxfCore.Blocks.Block block)
        //{
        //    Canvas c = new Canvas();
        //    foreach (var item in block.Entities)
        //    {
        //        switch (item.Type)
        //        {
        //            case RTSafe.DxfCore.Entities.EntityType.Text: c.Children.Add(LoadText(item as RTSafe.DxfCore.Entities.Text)); break;
        //            case RTSafe.DxfCore.Entities.EntityType.Line: this._container.Children.Add(LoadLine(item as RTSafe.DxfCore.Entities.Line)); break;
        //            case RTSafe.DxfCore.Entities.EntityType.LightWeightPolyline: c.Children.Add(LoadWpLine(item as RTSafe.DxfCore.Entities.LightWeightPolyline)); break;

        //        }
        //    }
        //    SetLocal(c, block.BasePoint);
        //    return c;
        //}
        /// <summary>
        /// 圆
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Element LoadCircle(Circle item, int type)
        {
            var color = GetBrush(item.Color, item.Layer, type).Color;
            CircleElement circleElement = new CircleElement
            {
                Tag = item.Layer.Name,
                LayerName = item.Layer.Name,
                R = color.R,
                G = color.G,
                B = color.B,
                Radius = item.Radius
            };
            //circleElement.Height = circleElement.Width = 2 * item.Radius;           
            //e.StrokeThickness = GetThickness(item.Thickness);
            foreach (var i in item.LineType.Segments)
            {
                //e.StrokeDashArray.Add(i);
            }
            //circleElement.EPoint = new EPoint(item.Center.X  - MinX, MaxY - item.Center.Y );
            circleElement.X = item.Center.X - MinX;
            circleElement.Y = MaxY - item.Center.Y;
            return circleElement;
        }

        /// <summary>
        /// 加载多线段
        /// </summary>
        /// <param name="line"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Element LoadWpLine(LightWeightPolyline line, int type)
        {
            var lle = new LightWeightPolylineElement();
            var tag = line.Layer.Name;

            lle.EPoints = new List<EPoint>();
            foreach (var point in line.Vertexes)
            {
                lle.EPoints.Add(new EPoint(point.Location.X - MinX, MaxY - point.Location.Y));
            }

            var color = GetBrush(line.Color, line.Layer, type);
            //lle.Color = color;

            if (line.Flags == RTSafe.DxfCore.Entities.PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM)
            {
                lle.EPoints = lle.EPoints;
                //lle.Color = color;
                //p.StrokeThickness = GetThickness(line.Thickness);
                lle.Tag = tag;
                return lle;
            }
            lle.EPoint = lle.EPoint;
            //lle.Color = color;
            //p.StrokeThickness = GetThickness(line.Thickness);
            lle.Tag = tag;
            return lle;
        }
        /// <summary>
        /// 加载圆弧
        /// </summary>
        /// <param name="startvertexe"></param>
        /// <param name="vertexe"></param>
        /// <returns></returns>
        //private ArcSegment ToBezierSegment(RTSafe.DxfCore.Entities.LightWeightPolylineVertex startvertexe,
        //    RTSafe.DxfCore.Entities.LightWeightPolylineVertex vertexe)
        //{
        //    var u = vertexe.Bulge;
        //    double centerAngle;//包角
        //    centerAngle = 4 * Math.Atan(Math.Abs(u));

        //    double x1, x2, y1, y2;//圆弧起始点和终止点
        //    x1 = startvertexe.Location.X;
        //    x2 = vertexe.Location.X;
        //    y1 = startvertexe.Location.Y;
        //    y2 = vertexe.Location.Y;

        //    double L; //弦长
        //    L = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));

        //    double R;//圆弧半径
        //    R = 0.5 * L / Math.Sin(0.5 * centerAngle);

        //    //已知圆上两点和半径，求圆心坐标
        //    double h;//圆心到弦的距离
        //    h = Math.Sqrt(R * R - L * L / 4);

        //    double k;//起始点和终止点连线的中垂线斜率
        //    double xc, yc;//圆心坐标
        //    double xa, ya; //起始点和终止点连线的中点横纵坐标
        //    xa = 0.5 * (x1 + x2);
        //    ya = 0.5 * (y1 + y2);

        //    //弦的方向角（0-2PI之）

        //    double angle;//起点到终点的弦向量与x正方向之间的倾斜角
        //    angle = Math.Acos((x2 - x1) / Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));

        //    double amass; //弦向量与X轴正向单位向量的叉积
        //    amass = y1 - y2;//由（由(x2-x1)*0-1*(y2-y1)）得到

        //    if (amass < 0)
        //    {
        //        angle = -angle;
        //        angle = 2 * Math.PI + angle;
        //    }

        //    double DirectionAngel = 0;//弦中点到圆心的直线向量的方向角（0-2PI之间）
        //    if ((u > 0 && centerAngle < Math.PI) || (u < 0 && centerAngle > Math.PI))
        //        DirectionAngel = angle + Math.PI / 2;
        //    if ((u < 0 && centerAngle < Math.PI) || (u > 0 && centerAngle > Math.PI))
        //        DirectionAngel = angle - Math.PI / 2;
        //    if (DirectionAngel > 2 * Math.PI)
        //        DirectionAngel = DirectionAngel - 2 * Math.PI;

        //    double d;//圆心到弦的距离
        //    d = Math.Sqrt(R * R - L * L / 4);
        //    if (DirectionAngel == 0)
        //    {
        //        xc = xa + d;
        //        yc = ya;
        //    }
        //    else if (DirectionAngel == Math.PI / 2)
        //    {
        //        xc = xa;
        //        yc = ya + d;
        //    }
        //    else if (DirectionAngel == Math.PI)
        //    {
        //        xc = xa - d;
        //        yc = xa;
        //    }
        //    else if (DirectionAngel == Math.PI + Math.PI / 2)
        //    {
        //        xc = xa;
        //        yc = xa - d;
        //    }
        //    else
        //    {
        //        double nslope, k1;//nslope 为弦的斜率，K为弦中垂线的斜率
        //        double nAngle;//中垂线的倾斜角；
        //        double X, Y; //圆心相对于弦中心点的坐标偏移量

        //        nslope = (y2 - y1) / (x2 - x1);
        //        k1 = -1 / nslope;
        //        nAngle = Math.Atan(k1);
        //        X = Math.Cos(nAngle) * d;
        //        Y = Math.Sin(nAngle) * d;

        //        if (DirectionAngel > Math.PI / 2 && DirectionAngel < Math.PI)
        //        {
        //            X = -X;
        //            Y = -Y;
        //        }
        //        if (DirectionAngel > Math.PI && DirectionAngel < (Math.PI + Math.PI / 2))
        //        {
        //            X = -X;
        //            Y = -Y;
        //        }
        //        xc = xa + X;
        //        yc = ya + Y;
        //    }

        //    Point center = new Point(xc, yc);
        //    ArcSegment bs = new ArcSegment();
        //    bs.SweepDirection = vertexe.Bulge > 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
        //    bs.Size = new Size((int) (2 * R), (int) (2 * R));
        //    bs.RotationAngle = centerAngle;
        //    bs.Point = new Point(vertexe.Location.X - MinX, MaxY - vertexe.Location.Y);
        //    return bs;


        //}





        //public double GetThickness(double thickness)
        //{
        //    if (thickness == 0)
        //    {
        //        thickness = 0.25;
        //    }
        //    if (thickness > 20)
        //    {
        //        thickness = 0.25;
        //    }
        //    return 0.5;
        //}

    }

    public enum Type
    {
        One = 1,
        //Two = 2
    }

    #region MyRegion





    //internal class SweepDirection
    //{
    //    public static object Counterclockwise { get; set; }

    //    public static object Clockwise { get; set; }
    //}

    //internal class FontStretches
    //{
    //    public static object Normal { get; set; }
    //}

    //public class RotateTransform
    //{
    //    public double Angle { get; set; }
    //    public double CenterY { get; set; }
    //}

    //internal class Polygon : FrameworkElement
    //{
    //    public Media.Brush Fill { get; set; }
    //    public double StrokeThickness { get; set; }
    //}

    //public class ArcSegment
    //{
    //    public bool IsLargeArc { get; set; }
    //    public double RotationAngle { get; set; }
    //    public Size Size { get; set; }
    //    public object SweepDirection { get; set; }
    //    public Point Point { get; set; }
    //}

    //public class TextBlock : Ellipse
    //{
    //    public string Text { get; set; }
    //    public double FontSize { get; set; }
    //    public RotateTransform RenderTransform { get; set; }
    //    public FontFamily FontFamily { get; set; }
    //    public int Opacity { get; set; }
    //    public object FontStretch { get; set; }

    //    public Media.Brush Foreground { get; set; }
    //    public double ActualWidth { get; set; }
    //}

    //public class DoubleCollection
    //{
    //    public void Add(double item)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class PathFigure
    //{
    //    public Point StartPoint { get; set; }
    //    public PathSegmentCollection Segments { get; set; }
    //}

    //public class PathFigureCollection
    //{
    //    public void Add(PathFigure pf)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class PathGeometry
    //{
    //    public PathFigureCollection Figures { get; set; }
    //}

    #endregion
}
