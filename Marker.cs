using GMap.NET;
using GMap.NET.WindowsForms.Markers;

namespace MapApp
{
    public class Marker : GMarkerGoogle
    {
        public Marker(PointLatLng p, GMarkerGoogleType type) : base(p, type)
        {
        }

        public string Id { get; set; }
    }
}
