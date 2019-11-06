using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorMaterial : MonoBehaviour {
    
        public List<Color> colorList;
        public Color randColor;
        public Material mat;

        public void Awake()
        {
            randColor = colorList[Random.Range(0, colorList.Count)];
            mat = GetComponent<MeshRenderer>().material;
            mat.color = randColor;
        }
    }
