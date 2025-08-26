using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.Collections.Generic;

namespace Map
{
    public class MapManager : Singleton<MapManager>
    {
        public List<MapConfig> configs;
        //public MapView view;

        public Map CurrentMap { get; private set; }

        private void Start()
        {

            GenerateNewMap(0);

        }

        public void GenerateNewMap(int index)
        {
            Map map = MapGenerator.GetMap(Utility.ScriptableObjectCloner.Clone<MapConfig>(configs[index]));
            CurrentMap = map;
            Debug.Log(map.ToJson());
            // view.ShowMap(map);
        }

        public void SaveMap()
        {
            if (CurrentMap == null) return;

            string json = JsonConvert.SerializeObject(CurrentMap, Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            PlayerPrefs.SetString("Map", json);
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
