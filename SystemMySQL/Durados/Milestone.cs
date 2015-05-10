using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Durados
{
    public class Milestone
    {
        [Durados.Config.Attributes.ColumnProperty(Description="Identify the milestone in the field dropdown")]
        public string Name { get; set; }
        [Durados.Config.Attributes.ColumnProperty(Description="RGB 3 comma delimited numbers to start the gradient")]
        public string StartColor { get; set; }
        [Durados.Config.Attributes.ColumnProperty(Description = "RGB 3 comma delimited numbers to end the gradient")]
        public string EndColor { get; set; }
        [Durados.Config.Attributes.ColumnProperty(Description="if set to true then the milestone will ignore the gradient and will refer to specific classes")]
        public bool Custom { get; set; }

        public Milestone()
        {
            StartColor = "0,0,0";
            EndColor = "255,255,255";
            
        }

        public Color GetColor(string commaDelimitedString)
        {
            string[] rgb = commaDelimitedString.Split(',');
            // validate
            byte r = 255;
            byte.TryParse(rgb[0], out r);
            byte g = 255;
            byte.TryParse(rgb[1], out g);
            byte b = 255;
            byte.TryParse(rgb[2], out b);

            return Color.FromArgb(r, g, b);
        }

        public Color GetStartColor()
        {
            return GetColor(StartColor);
        }

        public System.Drawing.Color GetEndColor()
        {
            return GetColor(EndColor);
        }

        public IEnumerable<Color> GetGradients(Color start, Color end, int steps)
        {
            Color stepper = Color.FromArgb((byte)((end.A - start.A) / (steps - 1)),
                                           (byte)((end.R - start.R) / (steps - 1)),
                                           (byte)((end.G - start.G) / (steps - 1)),
                                           (byte)((end.B - start.B) / (steps - 1)));

            for (int i = 0; i < steps; i++)
            {
                yield return Color.FromArgb(start.A + (stepper.A * i),
                                            start.R + (stepper.R * i),
                                            start.G + (stepper.G * i),
                                            start.B + (stepper.B * i));
            }
        }

        public IEnumerable<Color> GetGradients(int steps)
        {
            return GetGradients(GetStartColor(), GetEndColor(), steps);
        }
    }
}
