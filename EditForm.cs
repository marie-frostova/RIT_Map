using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MapApp
{
    public partial class EditForm : Form
    {
        private SqlConnection Connection;
        private Marker redMarker;
        private Marker blueMarker;
        private MapForm mapForm;

        /// <summary>
        /// Заводим красный маркер, чтобы пользователь видел, какой маркер он редактирует
        /// </summary>
        /// <param name="mapForm"></param>
        public EditForm(MapForm mapForm)
        {
            ControlBox = false;
            this.mapForm = mapForm;
            redMarker = new Marker(new PointLatLng(0, 0), GMarkerGoogleType.red_dot);
            redMarker.IsVisible = false;
            mapForm.MarkersOverlay.Markers.Add(redMarker);
            InitializeComponent();
        }

        private void EditForm_Shown(object sender, EventArgs e)
        {
            ReplaceBlueMarkerWithRed();

            textBoxName.Text = redMarker.ToolTipText;
            textBoxLat.Text = redMarker.Position.Lat.ToString();
            textBoxLng.Text = redMarker.Position.Lng.ToString();
            messageName.Text = string.Empty;
            messageLat.Text = string.Empty;
            messageLng.Text = string.Empty;
        }

        private void ReplaceBlueMarkerWithRed()
        {
            blueMarker = mapForm.CurrentMarker as Marker;
            blueMarker.IsVisible = false;
            redMarker.Position = new PointLatLng(blueMarker.Position.Lat, blueMarker.Position.Lng);
            redMarker.ToolTipText = blueMarker.ToolTipText;
            redMarker.IsVisible = true;
            mapForm.gmap.Refresh();
        }

        public void ReplaceMarkersBack()
        {
            redMarker.IsVisible = false;
            if (blueMarker != null)
            {
                blueMarker.Position = redMarker.Position;
                blueMarker.ToolTipText = redMarker.ToolTipText;
                blueMarker.IsVisible = true;
            }
        }

        /// <summary>
        /// Перед тем как спрятать окошко, меняем красный маркер на синий
        /// </summary>
        internal void PrepareAndClose()
        {
            ReplaceMarkersBack();
            Hide();
        }

        private void UpdateMarkerInDatabase(double lat, double lng, string name)
        {
            ConnectionHelper.EstablishConnection(ref Connection);
            string SqlUpdateQuery = $"UPDATE Markers SET Name = '{name}', Latitude = {lat}, Longitude = {lng} WHERE Id = CAST('{blueMarker.Id}' AS UNIQUEIDENTIFIER);";
            var command = new SqlCommand(SqlUpdateQuery, Connection);
            command.ExecuteNonQuery();
        }

        private void DeleteMarkerFromDatabase()
        {
            ConnectionHelper.EstablishConnection(ref Connection);
            string SqlDeleteQuery = $"DELETE FROM Markers WHERE Id = CAST('{blueMarker.Id}' AS UNIQUEIDENTIFIER);";
            var command = new SqlCommand(SqlDeleteQuery, Connection);
            command.ExecuteNonQuery();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (Checker.CheckName(textBoxName, messageName) && Checker.CheckLat(textBoxLat, messageLat) && Checker.CheckLng(textBoxLng, messageLng))
            {
                var lat = double.Parse(textBoxLat.Text);
                var lng = double.Parse(textBoxLng.Text);
                var name = textBoxName.Text;
                UpdateMarkerInDatabase(lat, lng, name);
                redMarker.Position = new PointLatLng(lat, lng);
                redMarker.ToolTipText = textBoxName.Text;
                PrepareAndClose();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            PrepareAndClose();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeleteMarkerFromDatabase();
            mapForm.MarkersOverlay.Markers.Remove(blueMarker);
            blueMarker = null;
            PrepareAndClose();
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
