using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MapApp
{
    public partial class AddForm : Form
    {
        SqlConnection Connection;
        MapForm mapForm;

        public AddForm(MapForm mapForm)
        {
            this.mapForm = mapForm;
            InitializeComponent();
            ControlBox = false;
        }

        private void AddForm_Shown(object sender, EventArgs e)
        {
            textBoxName.Text = string.Empty;
            textBoxLat.Text = string.Empty;
            textBoxLng.Text = string.Empty;

            messageName.Text = string.Empty;
            messageLat.Text = string.Empty;
            messageLng.Text = string.Empty;
        }

        private void AddMarkerToDatabase(string id, string name, double lat, double lng)
        {
            ConnectionHelper.EstablishConnection(ref Connection);
            string SqlAddQuery = $"INSERT INTO Markers(Id, Name, Latitude, Longitude) VALUES(CAST('{id}' AS UNIQUEIDENTIFIER), '{name}', {lat}, {lng})";
            var command = new SqlCommand(SqlAddQuery, Connection);
            command.ExecuteNonQuery();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (Checker.CheckName(textBoxName, messageName) && Checker.CheckLat(textBoxLat, messageLat) && Checker.CheckLng(textBoxLng, messageLng))
            {
                var lat = double.Parse(textBoxLat.Text);
                var lng = double.Parse(textBoxLng.Text);
                var name = textBoxName.Text;
                var id = Guid.NewGuid().ToString();
                AddMarkerToDatabase(id, name, lat, lng);
                var marker = new Marker(new PointLatLng(lat, lng), GMarkerGoogleType.blue_dot);
                marker.Id = id;
                marker.ToolTipText = name;
                mapForm.MarkersOverlay.Markers.Add(marker);
                Hide();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            Checker.CheckName(textBoxName, messageName);
        }

        private void textBoxLat_TextChanged(object sender, EventArgs e)
        {
            Checker.CheckLat(textBoxLat, messageLat);
        }

        private void textBoxLng_TextChanged(object sender, EventArgs e)
        {
            Checker.CheckLng(textBoxLng, messageLng);
        }
    }
}
