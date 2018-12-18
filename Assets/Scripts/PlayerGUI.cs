using Procedural.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Procedural.Gamelogic.UI
{
    class PlayerGUI : MonoBehaviour
    {
        public void ReloadWorld_OnClick()
        {
            Debug.Log("Clicked!");
            GameObject terrain = GameObject.Find("Terrain");
            terrain.GetComponent<TerrainController>().GenerateChunks("");
            GameObject player = GameObject.Find("Player");
            player.transform.position = new Vector3(player.transform.position.x, 100f, player.transform.position.z);
        }
    }
}
