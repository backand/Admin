using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public abstract class Chart
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [DataMember]
        public List<Series> Serieses { get; set; }
        [DataMember]
        public string xTitle { get; set; }
        [DataMember]
        public string yTitle { get; set; }
        
        public Chart()
        {
            this.Serieses = new List<Series>();
        }

        public abstract Dimentions Dimentions
        {
            get;
        }

        protected abstract Series GetNewSeries();

        public Series AddSeries(string name)
        {
            Series series = GetNewSeries();
            series.Name = name;
            Serieses.Add(series);
            return series;
        }

        [DataContract]
        public abstract class Series
        {
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Series()
            {
                this.Points = new List<Point>();
            }
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public ChartType ChartType { get; set; }

            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [DataMember]
            public List<Point> Points { get; set; }

            protected abstract Point GetNewPoint();
        }

        [DataContract]
        public abstract class Point
        {
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Point() { }
            
        }
    }

    [DataContract]
    public class Chart1D : Chart
    {
        [DataMember]
        public List<string> xAxis { get; set; }

        public Chart1D() : base()
        {
        }

        protected override Chart.Series GetNewSeries()
        {
            return new Series1D();
        }

        [DataMember]
        public override Dimentions Dimentions
        {
            get
            {
                return Dimentions.Chart1D;
            }
        }


        
        [DataContract]
        public class Series1D : Chart.Series
        {
            public Series1D() : base()
            {
            }

            public Point1D AddPoint(double y)
            {
                Point1D point = (Point1D)GetNewPoint();
                point.Y = y;
                Points.Add(point);
                return point;
            }

            protected override Point GetNewPoint()
            {
                return new Point1D();
            }
        }

        [DataContract]
        public class Point1D : Chart.Point
        {
            public Point1D() { }
            [DataMember]
            public double Y { get; set; }

        }
    }

    [DataContract]
    public class Chart2D : Chart
    {
        public Chart2D() : base()
        {
        }

        protected override Chart.Series GetNewSeries()
        {
            return new Series2D();
        }

        [DataMember]
        public override Dimentions Dimentions
        {
            get
            {
                return Dimentions.Chart2D;
            }
        }

        [DataContract]
        public class Series2D : Chart.Series
        {
            public Series2D()
                : base()
            {
            }

            public Point2D AddPoint(double x, double y)
            {
                Point2D point = (Point2D)GetNewPoint();
                point.X = x;
                point.Y = y;
                Points.Add(point);

                return point;
            }

            protected override Point GetNewPoint()
            {
                return new Point2D();
            }
        }

        [DataContract]
        public class Point2D : Chart.Point
        {
            public Point2D() { }
            [DataMember]
            public double X { get; set; }
            [DataMember]
            public double Y { get; set; }

        }
    }

    public enum Dimentions
    {
        Chart1D,
        Chart2D
    }
}
