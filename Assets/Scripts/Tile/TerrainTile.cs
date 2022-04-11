using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public abstract class TerrainTile : Tile{


    List<int> specialvals;
    List<int> specialvalsCalc;
    public virtual Sprite[] GetSprite(string textureName)
    {
        Sprite[] myTextures = Resources.LoadAll<Sprite>("Textures/"+textureName) ;
        return myTextures;
    }


    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        specialvals = new List<int>();
        specialvals.Add(15);
        specialvals.Add(7);
        specialvals.Add(13);
        specialvals.Add(2);
        specialvalsCalc = new List<int> { 115,1015,2015,3007,3115,3013,3007,3015,3113,3115,4002,4115,5014,5015,5108,5115,6015,6100,6115,8115,9015};


        return true;

    }
    public void UpdateDB(Object myAT , string name)
    {
        AssetDatabase.CreateAsset(myAT, "Assets/Tiles/" + name + "");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }


    //Kann sein, dass die hier oben stehen müssen, weil wir sie sonst bei Awake nicht instanzieren können
    public SpriteSlot[] spriteSlots;


    //Ich denke die Funktion wird aufgerufen, wenn das Tile sich selbst auf die Tilemap zeichnet
	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
        

        //Hier wird gecheckt, ob oben, rechts, links oder unten schon ein Walltile ist
        //Ein Index wird gebildet
        //Probably so that no exception is produced
        //Acid Tile has less than WallTile
        if (spriteSlots.Length < 9) return;
		int mask = (HasNeighbour(position + Vector3Int.up, tilemap) ? 1 : 0)
				+ (HasNeighbour(position + Vector3Int.right, tilemap) ? 2 : 0)
				+ (HasNeighbour(position + Vector3Int.down, tilemap) ? 4 : 0)
				+ (HasNeighbour(position + Vector3Int.left, tilemap) ? 8 : 0);

        if (specialvals.Contains(mask))
        {
            int tmpI= mask;
            tmpI += (HasNeighbour(position + Vector3Int.left + Vector3Int.up, tilemap) ? 100 : 0);
            tmpI += (HasNeighbour(position + Vector3Int.left + Vector3Int.down, tilemap) ? 5000 : 0);
            tmpI += (HasNeighbour(position + Vector3Int.right + Vector3Int.down, tilemap) ? 1000 : 0);
            tmpI += (HasNeighbour(position + Vector3Int.right + Vector3Int.up, tilemap) ? 3000 : 0);
            if (specialvalsCalc.Contains(tmpI))
                {
                    mask = tmpI;
                }
        }


        //Anschließend wird geschaut, welche Tiles daneben kein Tile haben

        //Je nach Index wird das jeweilige Tile ausgewählt, das passt
        //Slot wird aus spriteSlots der mit Prefabs versehen ist gefüllt
        //Nummer vier, falls Random Tiles reinkommen, glaube ich
        //ChangeYT
        //Standard if no sprites around
        SpriteSlot slot = spriteSlots[11];
        	switch (mask)
        	{
            //Andersrum denken: Welche Position der Acht-Tiles gehört zu welcher Summe?

                //YT New
        		case  1: slot = spriteSlots[11]; break;
        		case  4: slot = spriteSlots[1]; break;
        		case  5: slot = spriteSlots[6]; break;

        		case  3: slot = spriteSlots[10]; break;
        		case  6: slot = spriteSlots[0]; break;
        		case  7: slot = spriteSlots[5]; break;
        		case  9: slot = spriteSlots[12]; break;
            //left and right
                case 10:  slot = spriteSlots[6]; break;
                case 11:  slot = spriteSlots[11]; break;
        		case 12: slot = spriteSlots[2]; break;
        		case 13: slot = spriteSlots[7]; break;
        		case 14: slot = spriteSlots[1]; break;
                //Wieso gibt es den nicht für WallTile? Weil das dann Floor ist?
        		case 15: slot = spriteSlots[6]; break;
        		case 115: slot = spriteSlots[9]; break;
        		case 1015: slot = spriteSlots[3]; break;
        		case 1115: slot = spriteSlots[4]; break;
        		case 5015: slot = spriteSlots[4]; break;
        		case 4115: slot = spriteSlots[4]; break;
        		case 5115: slot = spriteSlots[7]; break;
        		case 6015: slot = spriteSlots[6]; break;
        		case 3115: slot = spriteSlots[6]; break;
        		case 3013: slot = spriteSlots[8]; break;
        		case 3113: slot = spriteSlots[8]; break;
        		case 3007: slot = spriteSlots[9]; break;
        		case 3015: slot = spriteSlots[6]; break;
        		case 4002: slot = spriteSlots[5]; break;
        		case 5014: slot = spriteSlots[2]; break;
        		case 5108: slot = spriteSlots[7]; break;
        		case 6100: slot = spriteSlots[8]; break;
        		case 6115: slot = spriteSlots[8]; break;
        		case 8115: slot = spriteSlots[3]; break;
        		case 9015: slot = spriteSlots[9]; break;
        	}


        //Slot kann bis zu vier Sprites vorhalten für Random Auswahl
        tileData.sprite = slot.sprites[0].sprite;
        tileData.flags = TileFlags.LockAll;
        //YT
        //Addition to Acid Brush
        tileData.colliderType = mask != 15 ? Tile.ColliderType.Grid : Tile.ColliderType.None;
	}




    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
	{
        foreach (var p in new BoundsInt(-1,-1, 0, 3, 3, 1).allPositionsWithin)
		{
			tilemap.RefreshTile(position + p);	
		}
	}



    public Sprite[] LoadTexture()
    {
        Sprite[] myTextures = Resources.LoadAll<Sprite>("LavaWalls") ;
        //Das hat funktioniert
        return myTextures;

    }



    public bool HasNeighbour(Vector3Int position, ITilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(position);
        //tiledata check must be same kind of tile type
         
        //Important to check name property because we want tiles to change form depending on same name
        return (tile != null && tile.name== this.name );
    }

    public virtual void InitiateSlots(Sprite[] mySprite)
    {
        spriteSlots = new SpriteSlot[mySprite.Length];


        for (int i = 0; i < mySprite.Length; i++)
        {

        SpriteSlot mySpSlot = new SpriteSlot(mySprite[i]);
        spriteSlots[i] = mySpSlot;

        }

    }


    [System.Serializable]
    public class SpriteSlot {
        [SerializeField]
		public List<SpriteSlotItem> sprites;


        public SpriteSlot(Sprite spSprite)
        {
            sprites = new List<SpriteSlotItem>();
            sprites.Add(new SpriteSlotItem());
            sprites[0].sprite = spSprite;
        }


    }

	[System.Serializable]
	public class SpriteSlotItem
	{
        [SerializeField]
		public Sprite sprite;
        [SerializeField]
		public int probability = 10;
	}

}
