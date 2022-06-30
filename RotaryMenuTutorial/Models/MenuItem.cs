using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaryMenuTutorial.Models
{
    public class MenuItem
    {
        public string Name { get; }
        public string Icon { get; }

        public Point Location { get; }

        public MenuItem(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }
        public MenuItem(string name, string icon, Point location)
        {
            Name = name;
            Icon = icon;
            Location = location;
        }
    }
}
