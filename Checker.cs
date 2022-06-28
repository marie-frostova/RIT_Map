using System;
using System.Windows.Forms;

namespace MapApp
{
    public static class Checker
    {
        public static bool CheckLat(TextBox textBoxLat, Label messageLat)
        {
            double lat;
            try
            {
                lat = double.Parse(textBoxLat.Text);
                messageLat.Text = "";
                if (-90 > lat || lat > 90)
                    throw new Exception();
                return true;
            }
            catch (Exception)
            {
                messageLat.Text = "Wrong latitude format";
                return false;
            }
        }

        public static bool CheckLng(TextBox textBoxLng, Label messageLng)
        {
            double lng;
            try
            {
                lng = double.Parse(textBoxLng.Text);
                messageLng.Text = "";
                if (-180 > lng || lng > 180)
                    throw new Exception();
                return true;
            }
            catch (Exception)
            {
                messageLng.Text = "Wrong longitude format";
                return false;
            }
        }

        public static bool CheckName(TextBox textBoxName, Label messageName)
        {
            var name = textBoxName.Text;
            if (0 == name.Length || name.Length > 100 || ContainsForbidden(name))
            {
                messageName.Text = "Wrong name format";
                return false;
            }
            messageName.Text = "";
            return true;
        }

        /// <summary>
        /// Из-за риска инъекции sql сервера было принято решение запретить следующие подстроки
        /// </summary>
        public static string[] Forbidden = new string[] { ";", "'", "--", "/*", "*/", "xp_" };
 
        public static bool ContainsForbidden(string name)
        {
            foreach (var s in Forbidden)
            {
                if (name.Contains(s))
                    return true;
            }
            return false;
        }
    }
}
