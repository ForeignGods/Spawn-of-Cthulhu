using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowTile : TerrainTile {


#if UNITY_EDITOR
    [MenuItem("Assets/Create/WindowTile",false,1)]
        private static void CreateWindowTile()
    {

        WindowTile myAT = new WindowTile();
//        lavaTile myAT = ScriptableObject.CreateInstance("lavaTile"); 
        Sprite[] myTextures = myAT.GetSprite(); 

        if (myTextures != null)
        { 
            Debug.Log(myTextures.GetType() + "Length: " + myTextures.Length );
            Debug.Log(myTextures[0].name);
            }
        else
        {
            Debug.Log("Texture not loaded");
        }

        string fName2 = "WindowTile";

        myAT.name = fName2 + ".asset";
        //Has to be placed before Create of Asset data will be lost after saving
        myAT.InitiateSlots(myTextures);
        AssetDatabase.CreateAsset(myAT, "Assets/Tiles/" +myAT.name + "");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public Sprite[] GetSprite()
    {
        Sprite[] myTextures = Resources.LoadAll<Sprite>("Textures/basic/window") ;
        return myTextures;
    }
#endif

    //Heritage from TerrainTile could do this with  GetTileData too?
    public override Sprite[] GetSprite(string textName)
    {
        Sprite[] myTextures= base.GetSprite(textName);
        return myTextures;
    }


}

