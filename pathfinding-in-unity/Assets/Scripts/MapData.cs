using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MapData : MonoBehaviour
    {
        public int width = 10;
        public int height = 5;

        public TextAsset textAsset;
        public Texture2D textureMap;
        public string resourcePath = "Mapdata";

        private void Awake()
        {
            string levelName = SceneManager.GetActiveScene().name;
            if (textureMap == null)
            {
                textureMap = Resources.Load(resourcePath + "/" + levelName) as Texture2D;
            }
            if (textAsset == null)
            {
                textAsset = Resources.Load(resourcePath + "/" + levelName) as TextAsset;
            }
        }

        public List<string> GetMapFromTextFile(TextAsset tAsset)
        {
            List<string> lines = new List<string>();

            if (tAsset != null)
            {
                string textData = tAsset.text;
                lines = textData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
                lines.Reverse();
            }

            return lines;
        }

        public List<string> GetMapFromTextFile()
        {
            return GetMapFromTextFile(textAsset);
        }

        public List<string> GetMapFromTexture(Texture2D texture)
        {
            List<string> lines = new List<string>();

            if (texture != null)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    string newLine = "";

                    for (int x = 0; x < texture.width; x++)
                    {
                        if (texture.GetPixel(x, y) == Color.black)
                        {
                            newLine += '1';
                        }
                        else if (texture.GetPixel(x, y) == Color.white)
                        {
                            newLine += '0';
                        }
                        else
                        {
                            newLine += ' ';
                        }
                    }

                    lines.Add(newLine);
                }
            }

            return lines;
        }

        public void SetDimentions(List<string> textLines)
        {
            height = textLines.Count;

            foreach (string line in textLines)
            {
                if (line.Length > width)
                {
                    width = line.Length;
                }
            }
        }

        public int[,] MakeMap()
        {
            List<string> lines = new List<string>();

            if (textureMap != null)
            {
                lines = GetMapFromTexture(textureMap);
            }
            else
            {
                lines = GetMapFromTextFile(textAsset);
            }

            SetDimentions(lines);

            int[,] map = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (lines[y].Length > x)
                    {
                        map[x, y] = (int)Char.GetNumericValue(lines[y][x]);
                    }
                }
            }

            return map;
        }
    }
}