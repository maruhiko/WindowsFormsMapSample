using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;


namespace WindowsFormsMapSample
{
    public partial class Form1 : Form
    {
        static int n = 100;
        List<double> latitudes = new List<double>();
        List<double> longitudes = new List<double>();


        public Form1()
        {
            InitializeComponent();

            // 緯度・経度のデータを準備する
            CreateGPSData();


            // 現在位置を取得する

            // GMapControlの設定
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMapControl1.SetPositionByKeywords("東京都");
            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 100;
            gMapControl1.Zoom = 13;

            // 中心を最初のデータ位置に
            gMapControl1.Position = new PointLatLng(latitudes.First(), longitudes.First());

            // マーカーを追加する
            var markerOverlay = new GMapOverlay("markers");
            for (int i = 0; i < n; i++)
            {
                var position = new PointLatLng(latitudes[i], longitudes[i]); // 例として東京駅の緯度経度を指定
                var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(position, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);
                markerOverlay.Markers.Add(marker);
            }

            gMapControl1.Overlays.Add(markerOverlay);
        }

        /// <summary>
        /// サンプル用にグラフに表示するためのデータを作成
        /// </summary>

        void CreateGPSData()
        {
            // データを作る。東京駅からスカイツリーまでのn点を採用する
            var start = new PointLatLng(35.681167, 139.767052); // 東京駅
            var end = new PointLatLng(35.710063, 139.810700); // スカイツリー

            // ルート検索を行う
            var route = GMap.NET.MapProviders.OpenStreetMapProvider.Instance.GetRoute(start, end, false, false, 15);

            // ルートをn点で分割する
            var step = (double)route.Points.Count / n;
            for (int i = 0; i < n; i++)
            {
                var point = route.Points[(int)(i * step)];
                latitudes.Add(point.Lat);
                longitudes.Add(point.Lng);
            }
        }


        /// ////////////////////////////////////////////////////////////////////////////
        // 以下、現在位置を表示させるためのプログラム
        /// ////////////////////////////////////////////////////////////////////////////

        // 現在座標を示すindex
        int posIndex = 0;

        // 現在位置表示用マーカー/オーバーレイ
        GMarkerGoogle currentMarker;
        GMapOverlay currentPosOverlay = new GMapOverlay("current");


        /// <summary>
        /// タイマーが動くごとに次のGPS座標に移動する。
        /// 現在の座標は青色マーカーで表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 現在位置の更新
            if (currentMarker != null)
            {
                currentPosOverlay.Markers.Remove(currentMarker);
            }

            PointLatLng currentPosition = new PointLatLng(latitudes[posIndex], longitudes[posIndex]);
            currentMarker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(currentPosition, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_small);
            currentPosOverlay.Markers.Add(currentMarker);
            gMapControl1.Overlays.Add(currentPosOverlay);

            // 表示データの更新
            posIndex++;
            if (posIndex >= n) posIndex = 0;
        }
    }
}
