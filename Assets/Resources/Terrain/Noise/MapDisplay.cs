using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Generation
{
    class MapDisplay : MonoBehaviour
    {
        public Renderer textureRenderer;

        public void DrawNoiseMap(ID[,] map, float range)
        {
            int w = map.GetLength(0);
            int h = map.GetLength(1);

            Texture2D texture = new Texture2D(w, h);
            Color[] colors = new Color[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int idArr = y * w + x;
                    switch (map[x, y])
                    {
                        case 0:
                            colors[idArr] = Color.black;
                            break;

                        //---------------------------
                        case ID.StoneHighSolid:
                            colors[idArr] = new Color32(102, 102, 102, 1);
                            break;


                        case ID.Stone:
                            colors[idArr] = Color.gray;
                            break;         
                        case ID.Dirt:
                            colors[idArr] = new Color32(60, 32, 7, 1);
                            break;
                        case ID.GrassGround:
                            colors[idArr] = Color.green;
                            break;
                        case ID.WoodPlanks:
                            colors[idArr] = Color.red;
                            break;
                        case ID.Chest:
                            colors[idArr] = Color.yellow;
                            break;

                        //---------------------------

                        case ID.Sand:
                            colors[idArr] = new Color32(255, 255, 102, 1);
                            break;
                        case ID.SandStone:
                            colors[idArr] = new Color32(255, 153, 51, 1);
                            break;
                        //---------------------------

                        case ID.Snow:
                            colors[idArr] = new Color32(226, 207, 207, 1);
                            break;
                        case ID.SnowStone:
                            colors[idArr] = new Color32(102, 153, 255, 1);
                            break;
                        case ID.SnowChest:
                            colors[idArr] = Color.red;
                            break;
                        case ID.SnowWoodPlanks:
                            colors[idArr] = new Color32(51, 51, 255, 1);
                            break;

                        //---------------------------

                        case ID.JungleGrassGround:
                            colors[idArr] = new Color32(7, 88, 23, 1);
                            break;
                        case ID.JungleStone:
                            colors[idArr] = new Color32(0, 153, 0, 1);
                            break;
                        case ID.JungleBriks:
                            colors[idArr] = new Color32(51, 102, 0, 0);
                            break;
                        case ID.JungleChest:
                            colors[idArr] = Color.red;
                            break;

                        //---------------------------

                        case ID.Iron:
                            colors[idArr] = Color.cyan;
                            break;
                        case ID.Gold:
                            colors[idArr] = Color.yellow;
                            break;
                        case ID.Silver:
                            colors[idArr] = Color.red;
                            break;
                        case ID.Copper:
                            colors[idArr] = Color.gray;
                            break;
                        case ID.Tin:
                            colors[idArr] = Color.magenta;
                            break;
                        case ID.Platina:
                            colors[idArr] = Color.blue;
                            break;

                        //---------------------------
                  
                        case ID.Lava:
                            colors[idArr] = Color.red;
                            break;
                        case ID.AdChest:
                            colors[idArr] = new Color32(38, 0, 77, 0);
                            break;
                        case ID.AdGround:
                            colors[idArr] = new Color32(153, 51, 0, 0);
                            break;
                        case ID.AdBriks:
                            colors[idArr] = new Color32(255, 102, 0, 0);
                            break;

                        //---------------------------

                        case ID.CloudGround:
                            colors[idArr] = Color.white;
                            break;
                        case ID.Water:
                            colors[idArr] = Color.blue;
                            break;

                        //---------------------------
                        case ID.DangeBriks:
                            colors[idArr] = Color.white;
                            break;
                    }                     
                }
            }

            texture.SetPixels(colors);
            texture.Apply();


            textureRenderer.sharedMaterial.mainTexture = texture;
        }
      
    }
}
