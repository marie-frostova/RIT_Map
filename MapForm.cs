using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MapApp
{
    public partial class MapForm : Form
    {
        internal GMapOverlay MarkersOverlay;
        private EditForm EditForm;
        private AddForm AddForm;
        private SqlConnection Connection;
        internal GMapMarker CurrentMarker = null;
        private bool isLeftButtonDown = false;

        public MapForm()
        {
            MarkersOverlay = new GMapOverlay("markers");

            EditForm = new EditForm(this);
            AddForm = new AddForm(this);
            EditForm.Hide();
            AddForm.Hide();

            AddMarkersFromDatabase();
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            gmap.Overlays.Add(MarkersOverlay);
            gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gmap.Position = new PointLatLng(55.49, 84.82);

            gmap.Zoom = 11;
            gmap.MinZoom = 2;
            gmap.MaxZoom = 18;
            gmap.ShowCenter = false;
            gmap.DragButton = MouseButtons.Left;

            gmap.OnMarkerClick += new MarkerClick(mapControl_OnMarkerClick);
            gmap.OnMarkerEnter += new MarkerEnter(mapControl_OnMarkerEnter);
            gmap.OnMarkerLeave += new MarkerLeave(mapControl_OnMarkerLeave);
            gmap.MouseDown += new MouseEventHandler(mapControl_MouseDown);
            gmap.MouseUp += new MouseEventHandler(mapControl_MouseUp);
            gmap.MouseMove += new MouseEventHandler(mapControl_MouseMove);
        }

        /// <summary>
        /// Достает имеющиеся маркеры из базы данных и наполняет DataSet
        /// Из датасета можем читать даже если отсутствует подключение к базе данных
        /// </summary>
        private void AddMarkersFromDatabase()
        {
            string SqlGetAllFromMarkersQuery = "SELECT * FROM Markers";
            ConnectionHelper.EstablishConnection(ref Connection);
            using (SqlDataAdapter adapter = new SqlDataAdapter(SqlGetAllFromMarkersQuery, Connection))
            {
                using (DataSet DataSetMarkers = new DataSet())
                {
                    adapter.Fill(DataSetMarkers);
                    foreach (DataRow row in DataSetMarkers.Tables[0].Rows)
                    {
                        try
                        {
                            var marker = ParseMarker(row);
                            MarkersOverlay.Markers.Add(marker);
                        }
                        catch(Exception)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private Marker ParseMarker(DataRow row)
        {
            var id = row["Id"].ToString();
            var name = row["Name"].ToString();
            var latitude = Convert.ToDouble(row["Latitude"]);
            var longitude = Convert.ToDouble(row["Longitude"]);
            var marker = new Marker(new PointLatLng(latitude, longitude), GMarkerGoogleType.blue_dot);
            marker.Id = id;
            marker.ToolTip = new GMapToolTip(marker);
            marker.ToolTipText = name;
            return marker;
        }

        private void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            var lat = gmap.FromLocalToLatLng(e.X, e.Y).Lat;
            var lng = gmap.FromLocalToLatLng(e.X, e.Y).Lng;
            textPos.Text = $"Lat: {lat}, Lng:{lng}";
            if (CurrentMarker != null && isLeftButtonDown && e.Button == MouseButtons.Left)
            {
                PointLatLng point = gmap.FromLocalToLatLng(e.X, e.Y);
                CurrentMarker.Position = point;
            }
        }

        private void mapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isLeftButtonDown = false;
                if(CurrentMarker != null)
                {
                    UpdateMarkerPositionInDatabase();
                    CurrentMarker = null;
                }
            }
        }

        private void mapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isLeftButtonDown = true;
        }

        /// <summary>
        /// Клик правой кнопкой мыши по маркеру показывает форму-редактор
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        private void mapControl_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && CurrentMarker != null)
            {
                EditForm.ShowDialog();
                gmap.Refresh();
            }
        }

        private void mapControl_OnMarkerEnter(GMapMarker item)
        {
            if (CurrentMarker == null)
                CurrentMarker = item;
        }

        private void mapControl_OnMarkerLeave(GMapMarker item)
        {
            CurrentMarker = null;
        }

        /// <summary>
        /// Клик по кнопке 'Добавить маркер' показывает форму создания маркера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddForm.ShowDialog();
            gmap.Refresh();
        }

        /// <summary>
        /// Когда делаем дроп маркера, его координаты обновляются в базе данных
        /// </summary>
        private void UpdateMarkerPositionInDatabase()
        {
            ConnectionHelper.EstablishConnection(ref Connection);
            var marker = CurrentMarker as Marker;
            string SqlUpdatePosQuery = $"UPDATE Markers SET Latitude = {marker.Position.Lat}, Longitude = {marker.Position.Lng} WHERE Id = CAST('{marker.Id}' AS UNIQUEIDENTIFIER);";
            var command = new SqlCommand(SqlUpdatePosQuery, Connection);
            command.ExecuteNonQuery();
        }
    }
}
