// PolarisationData.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Heartucate.TUD
{
    public class PolarisationData : MonoBehaviour
    {
        private float[][] data1;

        public int Rows => data1.Length;
        public int Columns => data1[0].Length;

        private Dictionary<int, float> data2Lookup;

        private Vector2Int _actDegrees = new Vector2Int(0, 0);

        [SerializeField] private string _csvfilePath1;
        [SerializeField] private string _csvfilePath2;
        [SerializeField] private LightUI _lightUI;
        [SerializeField] private GameObject _arrow1;
        [SerializeField] private GameObject _arrow2;
        [SerializeField] private GameObject _arrow3;
        [SerializeField] private DegreeDiagram _degDiagram1;
        [SerializeField] private DegreeDiagram _degDiagram2;
        [SerializeField] private DegreeDiagram _degDiagram3;
        [SerializeField] private DegreeDiagram _degDiagram4;

        private void Start()
        {
            LoadCSV1(Path.Combine(Application.streamingAssetsPath, _csvfilePath1));
            LoadCSV2(Path.Combine(Application.streamingAssetsPath, _csvfilePath2));
        }

        public void LoadCSV1(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);


            int rowCount = lines.Length - 1;

            data1 = new float[rowCount][];

            for (int i = 1; i < lines.Length; i++)
            {
                string[] cells = lines[i].Split(',');


                int columnCount = cells.Length - 1;
                data1[i - 1] = new float[columnCount];

                for (int j = 1; j < cells.Length; j++)
                {

                    data1[i - 1][j - 1] = float.Parse(
                        cells[j],
                        CultureInfo.InvariantCulture
                    );
                }
            }

            Debug.Log($"CSV geladen: {Rows} Zeilen, {Columns} Spalten");
        }
        public void LoadCSV2(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length < 2)
            {
                Debug.LogError("CSV2 muss mindestens 2 Zeilen haben");
                return;
            }

            string[] degreeCells = lines[2].Split(',');
            string[] valueCells = lines[3].Split(',');

            if (degreeCells.Length != valueCells.Length)
            {
                Debug.LogError("CSV2: Anzahl Winkel stimmt nicht mit Anzahl Werte überein");
                return;
            }

            data2Lookup = new Dictionary<int, float>();

            for (int i = 1; i < degreeCells.Length; i++)
            {
                int degree = int.Parse(degreeCells[i], CultureInfo.InvariantCulture);
                float value = float.Parse(valueCells[i], CultureInfo.InvariantCulture);

                data2Lookup[degree] = value;
            }

            Debug.Log($"CSV2 geladen: {data2Lookup.Count} Winkel-Wert-Paare");
        }

        public float GetPolarisationData1(int pol1, int pol2)
        {
            if (pol1 < 0 || pol1 >= Rows || pol2 < 0 || pol2 >= Columns)
            {
                throw new IndexOutOfRangeException(
                    $"Ungültiger Index: Zeile {pol1}, Spalte {pol2}"
                );
            }

            return data1[pol1][pol2];
        }
        public float GetPolarisationData2(int deg)
        {
            if (data2Lookup == null)
            {
                Debug.LogError("CSV2 wurde noch nicht geladen");
                return 0f;
            }

            if (data2Lookup.TryGetValue(deg, out float value))
            {
                return value;
            }

            Debug.LogWarning($"Kein Wert für Winkel {deg} in CSV2 gefunden");
            return 0f;
        }

        public void SetPolDegree(int filter, int deg)
        {
            if (GameState.CurrentLevel == GameState.Level.LEVEL1)
            {
                if (filter == 0) _actDegrees = new Vector2Int(deg, _actDegrees.y);
                if (filter == 1) _actDegrees = new Vector2Int(_actDegrees.x, deg);
                _lightUI.SetLight(GetPolarisationData1(_actDegrees.x, _actDegrees.y));
                //_arrow1.transform.localScale = new Vector3(1, 1, GetPolarisationData1(_actDegrees.x, _actDegrees.y));
                _degDiagram1.SetDegreeDiagram(0.0f, _actDegrees.x, 1.0f);
                _degDiagram2.SetDegreeDiagram(_actDegrees.x, _actDegrees.y, GetPolarisationData1(_actDegrees.x, _actDegrees.y));

            }
            else
            {
                _lightUI.SetLight(GetPolarisationData2(deg));
                //_arrow2.transform.localScale = new Vector3(1, 1, GetPolarisationData2(deg));
                _degDiagram3.SetDegreeDiagram(0.0f, deg, GetPolarisationData2(deg));
                _degDiagram4.SetDegreeDiagram(deg, 90.0f, GetPolarisationData2(deg));
                //_arrow3.transform.localScale = new Vector3(3, 3, GetPolarisationData2(deg)*3f);

            }
        }
    }
}