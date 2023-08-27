using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using RoomEditor;

namespace RoomEditor {
	public class RoomBuilder
	{
	    public enum WallSide
	    {
	        North = 0,
	        South = 1,
	        East = 2,
	        West = 3
	    }

			private const int CellSize = 5;
			private Quaternion DefaultFloorRotation = Quaternion.identity;
	    private Quaternion DefaultWallRotation = Quaternion.identity;

	    //
	    private RoomScript _room;
			private AssetsScript _assets;

			// Room basic data
	    private int _width = 5;
			private int _height = 5;
	    private int _wallHeight = 5;
			private int _roomWeight = 1;

	    // Door data
	    private int[] _doorPositions;
			private int _doorDefaultMaterial;

	    // Floor data
	    private List<TileType> _floorTileTypes;
			private List<FloorTile> _floorCustomTiles;
			private FloorTile[,] _floorTiles;
			private float _floorRandomSpawnChance;

	    // Wall data
	    private List<TileType> _wallTileTypes;
			private List<WallTile> _wallCustomTiles;
	    private List<WallTile[,]> _wallTiles;
			private float _wallRandomSpawnChance;

	// Constructor
	public RoomBuilder(RoomScript aRoom, AssetsScript aAssets)
	{
		//
		_room = aRoom;
		_assets = aAssets;

		// Initialises door data
		_doorPositions = new int[4] { -1, -1, -1, -1 };

		// Initialises floor data
		DefaultFloorRotation.eulerAngles = new Vector3(-90, 0, 0);

		_floorTileTypes = new List<TileType>();
		_floorTileTypes.Add(new TileType(0, new List<int>(), DefaultFloorRotation, 0));
		_floorTileTypes[0].Materials.Add(0);

		_floorCustomTiles = new List<FloorTile>();

		_floorTiles = new FloorTile[0, 0];

		// Initialises wall data
		DefaultWallRotation.eulerAngles = new Vector3(-90, 0, 0);

		_wallTileTypes = new List<TileType>();
		_wallTileTypes.Add(new TileType(0, new List<int>(), DefaultWallRotation, 0));
		_wallTileTypes[0].Materials.Add(0);
		_wallTileTypes.Add(new TileType(0, new List<int>(), DefaultWallRotation, 0));

		_wallCustomTiles = new List<WallTile>();

		_wallTiles = new List<WallTile[,]>(4);
		_wallTiles.Add(null);
		_wallTiles.Add(null);
		_wallTiles.Add(null);
		_wallTiles.Add(null);
	}

	    /*   ==========================   */


	    public int RoomWidth
	    {
	        get => _width;
	        set
	        {
	            var newWidth = Mathf.Abs(value);
	            if (CompliantRoomDimension(newWidth))
	                _width = newWidth;
	        }
	    }

	    public int RoomHeight
	    {
	        get => _height;
	        set
	        {
	            var newHeight = Mathf.Abs(value);
	            if (CompliantRoomDimension(newHeight))
	                _height = newHeight;
	        }
	    }

	    public int WallHeight
	    {
	        get => _wallHeight;
	        set
	        {
	            var newWallHeight = Mathf.Abs(value);
	            if (newWallHeight / 5 * 5 == newWallHeight)
	            {
	                if (newWallHeight > 0)
	                    _wallHeight = newWallHeight;
	            }
	        }
	    }

	    public int RoomWeight
	    {
	        get => _roomWeight;
	        set
	        {
	            _roomWeight = Mathf.Abs(value);
	        }
	    }

	    // Door Material
	    public int DefaultDoorMaterial
	    {
	        get => _doorDefaultMaterial;
	        set
	        {
	            if (_assets.MaterialExists(value))
	                _doorDefaultMaterial = value;
	        }
	    }

	    // Floor Default Tile Model
	    public int FloorDefaultTileModel
	    {
	        get => _floorTileTypes[0].Model;
	        set
	        {
	            if (_assets.FloorTileModelExists(value))
	            {
	                if (_assets.FloorTileModelScales[value] == 1)
	                    _floorTileTypes[0].Model = value;
	            }
	        }
	    }

	    // Floor Default Material
	    public int FloorDefaultTileMaterial
	    {
	        get => _floorTileTypes[0].Materials[0];
	        set
	        {
	            if (_assets.MaterialExists(value))
	                _floorTileTypes[0].Materials[0] = value;
	        }
	    }

	    // Wall Default Tile Model
	    public int WallDefaultTileModel
	    {
	        get => _wallTileTypes[0].Model;
	        set
	        {
	            if (_assets.WallTileModelExists(value))
	                _wallTileTypes[0].Model = value;
	        }
	    }

	    // Wall Default Material
	    public int WallDefaultTileMaterial
	    {
	        get => _wallTileTypes[0].Materials[0];
	        set
	        {
	            if (_assets.MaterialExists(value))
	                _wallTileTypes[0].Materials[0] = value;
	        }
	    }

	    // Spawn Chances for floor and wall variations
	    public float FloorRandomSpawnChance
	    {
	        get => _floorRandomSpawnChance;
	        set
	        {
	            if (value >= 0.0f && value <= 1.0f)
	                _floorRandomSpawnChance = value;
	        }
	    }

	    public float WallRandomSpawnChance
	    {
	        get => _wallRandomSpawnChance;
	        set
	        {
	            if (value >= 0.0f && value <= 1.0f)
	                _wallRandomSpawnChance = value;
	        }
	    }

	    private bool CompliantRoomDimension(int a)
	    {
	        // Check if a follows the rule y=20x+15, where a is y and x is an integer
	        float y = a;
	        float x = (y - 15.0f) / 20.0f;
	        return x == Mathf.Floor(x);
	    }


	    /*   Building Floors   */

	    //
	    private void PopulateFloorTiles()
	    {
	        _floorTiles = new FloorTile[RoomWidth / CellSize, RoomHeight / CellSize];
	        for (var x = 0; x < _floorTiles.GetLength(0); x++)
	        {
	            for (var y = 0; y < _floorTiles.GetLength(1); y++)
	            {
	                var newTile = new FloorTile(x, y, 0, 0);
	                _floorTiles[x, y] = newTile;
	            }
	        }
	    }

	    // Checks if a tile with the given x, y and scale would overlap any tiles in _floorCustomTiles
	    private bool NewFloorTileOverlapsAnyCustomTiles(int x, int y, int aScale)
	    {
	        var lOverlapsOtherCustomTiles = false;

	        // Coordinates of the proposed tile
	        var lProposedXCoords = new List<int>();
	        var lProposedYCoords = new List<int>();
	        for (var i = 0; i < aScale; i++)
	        {
	            for (var j = 0; j < aScale; j++)
	            {
	                lProposedXCoords.Add(x + i);
	                lProposedYCoords.Add(y + j);
	            }
	        }

	        for (var lCustomTileIndex = 0; lCustomTileIndex < _floorCustomTiles.Count; lCustomTileIndex++)
	        {
	            var lTile = _floorCustomTiles[lCustomTileIndex];
	            var lModel = _floorTileTypes[lTile.TileType].Model;
	            var lScale = _assets.FloorTileModelScales[lModel];
	            for (var lProposedCoordIndex = 0; lProposedCoordIndex < lProposedXCoords.Count; lProposedCoordIndex++)
	            {
	                var lCurrentProposedX = lProposedXCoords[lProposedCoordIndex];
	                var lCurrentProposedY = lProposedYCoords[lProposedCoordIndex];
	                for (var i = 0; i < lScale; i++)
	                {
	                    for (var j = 0; j < lScale; j++)
	                    {
	                        var lTileX = (int)lTile.x + i;
	                        var lTileY = (int)lTile.y + j;
	                        if (lCurrentProposedX == lTileX)
	                        {
	                            if (lCurrentProposedY == lTileY)
	                                lOverlapsOtherCustomTiles = true;
	                        }
	                    }
	                }
	            }
	        }

	        return lOverlapsOtherCustomTiles;
	    }

	    // Randomly creates custom floor tiles
	    private void CreateRandomFloorTiles()
	    {
	        if (_floorTileTypes.Count <= 1) return;

	        // Calculates spawn weight total
	        float lSpawnChanceWeightTotal = 0;
	        for (var i = 0; i < _floorTileTypes.Count; i++)
	            lSpawnChanceWeightTotal += _floorTileTypes[i].SpawnChanceWeight;

	        // Checks each tile and replaces with a tile variation based on random chance
	        for (var x = 0; x < _floorTiles.GetLength(0); x++)
	        {
	            for (var y = 0; y < _floorTiles.GetLength(1); y++)
	            {
	                if (!(Random.value < _floorRandomSpawnChance)) continue;

	                // Chooses which floor tile type to create based on the spawn chance weights
	                var lTypeRandomFloat = Random.Range(0, lSpawnChanceWeightTotal);
	                float lCurrentFloat = 0;
	                var lType = 0;
	                for (var i = 0; i < _floorTileTypes.Count; i++)
	                {
	                    lCurrentFloat += _floorTileTypes[i].SpawnChanceWeight;
	                    if (lCurrentFloat > lTypeRandomFloat)
	                    {
	                        lType = i;
	                        break;
	                    }
	                }

	                // Chooses a random material for the floor tile type with equal weight to all materials
	                var lMaterial = Random.Range(0, _floorTileTypes[lType].Materials.Count);

	                // Checks that the location for placing the tile is possible
	                var lCanPlaceTile = true;
	                var lScale = _assets.FloorTileModelScales[_floorTileTypes[lType].Model];
	                // Ensures that the tile doesn't stick out of the room
	                if (x + lScale - 1 >= _width / CellSize)
	                    lCanPlaceTile = false;
	                if (y + lScale - 1 >= _height / CellSize)
	                    lCanPlaceTile = false;
	                // Ensures that the tile doesn't overlap any other custom tiles
	                if (NewFloorTileOverlapsAnyCustomTiles(x, y, lScale))
	                    lCanPlaceTile = false;

	                // Adds the tile if possible
	                if (lCanPlaceTile)
	                    _floorCustomTiles.Add(new FloorTile(x, y, lType, lMaterial));
	            }
	        }
	    }

	    // Adds the custom floor tiles into the actual array
	    private void PopulateCustomFloorTilesIntoArray()
	    {
	        foreach (var lTile in _floorCustomTiles)
	        {
	            var lTileType = _floorTileTypes[lTile.TileType];

	            // Finds the x,y index within _floorTiles for this custom tile
	            var xIndex = (int)lTile.x;
	            var yIndex = (int)lTile.y;

	            // If the tile is a non-standard size, the x,y coordinates need to be adjusted,
	            // and the other tiles it would overlap need to be removed
	            var lScale = _assets.FloorTileModelScales[lTileType.Model];
	            if (lScale > 1)
	            {
	                // Adjusts x,y coordinates for the large tile
	                var xPos = xIndex + ((float)lScale - 1) / 2;
	                var yPos = yIndex + ((float)lScale - 1) / 2;
	                lTile.x = xPos;
	                lTile.y = yPos;

	                // Removes other tiles that this tile would overlap
	                for (var xOffset = 0; xOffset < lScale; xOffset++)
	                {
	                    for (var yOffset = 0; yOffset < lScale; yOffset++)
	                        _floorTiles[xIndex + xOffset, yIndex + yOffset].IsCovered = true;
	                }
	            }

	            // Puts lTile into _floorTiles
	            _floorTiles[xIndex, yIndex] = lTile;
	        }
	    }

	    //
	    private void BuildFloors()
	    {
	        // Creates frame and tile objects
	        for (var ix = 0; ix < _floorTiles.GetLength(0); ix++)
	        {
	            for (var iy = 0; iy < _floorTiles.GetLength(1); iy++)
	            {
	                // Base position ("origin" for the whole floor)
	                var lBasePos = _room.transform.position;
	                lBasePos -= new Vector3((float)RoomWidth / 2, 0, (float)RoomHeight / 2);
	                lBasePos += new Vector3((float)CellSize / 2, 0, (float)CellSize / 2);
	                float x, y;

	                // Position of the frame relative to lBasePos
	                x = ix;
	                y = iy;
	                var lFramePos = lBasePos + new Vector3(x * CellSize, 0, y * CellSize);

	                // Position of the tile relative to lBasePos
	                var lTile = _floorTiles[ix, iy];
	                x = lTile.x;
	                y = lTile.y;
	                var lPos = lBasePos + new Vector3(x * CellSize, 0, y * CellSize);

	                // Sets rotation and material
	                var lTileType = _floorTileTypes[lTile.TileType];
	                var lRot = lTileType.Rotation;
	                var lMaterial = lTileType.Materials[lTile.Material];

	                // Creates frame object
	                _room.CreateFloorFrame(lFramePos, lRot, lMaterial);

	                // Creates tile object
	                if (!lTile.IsCovered)
	                    _room.CreateFloorTile(lTileType.Model, lPos, lRot, lMaterial);
	            }
	        }
	    }

	    /*   ==========================   */


	    /*  ===  Building Walls  ===  */

	    // Fills _wallTiles with arrays of the correct size based on the room dimensions
	    private void PrepareWallTileArrays()
	    {
	        _wallTiles[(int)WallSide.North] = new WallTile[RoomWidth / CellSize, WallHeight / CellSize];
	        _wallTiles[(int)WallSide.South] = new WallTile[RoomWidth / CellSize, WallHeight / CellSize];
	        _wallTiles[(int)WallSide.West]  = new WallTile[RoomHeight / CellSize, WallHeight / CellSize];
	        _wallTiles[(int)WallSide.East]  = new WallTile[RoomHeight / CellSize, WallHeight / CellSize];
	    }

	    // Fills all of _wallTiles with the deafault wall tile types
	    private void PopulateWallTiles()
	    {
	        for (var i = 0; i < 4; i++)
	        {
	            for (var j = 0; j < _wallTiles[i].GetLength(0); j++)
	            {
	                for (var k = 0; k < _wallTiles[i].GetLength(1); k++)
	                    _wallTiles[i][j, k] = new WallTile(j, k, i, 0, 0, false);
	            }
	        }
	    }

	    // Randomly creates custom wall tiles
	    private void CreateRandomWallTiles()
	    {
	        if (_wallTileTypes.Count <= 2) return;

	        // Calculates spawn weight total
	        var lSpawnChanceWeightTotal = _wallTileTypes.Sum(t => t.SpawnChanceWeight);

	        // Checks each tile and replaces with a tile variation based on random chance
	        for (var s = 0; s < 4; s++)
	        {
	            for (var x = 0; x < _wallTiles[s].GetLength(0); x++)
	            {
	                for (var y = 0; y < _wallTiles[s].GetLength(1); y++)
	                    if (Random.value < _wallRandomSpawnChance)
	                    {
	                        // Chooses which floor tile type to create based on the spawn chance weights
	                        var lTypeRandomFloat = Random.Range(0, lSpawnChanceWeightTotal);
	                        float lCurrentFloat = 0;
	                        var lType = 0;
	                        for (var i = 0; i < _wallTileTypes.Count; i++)
	                        {
	                            lCurrentFloat += _wallTileTypes[i].SpawnChanceWeight;
	                            if (lCurrentFloat > lTypeRandomFloat)
	                            {
	                                lType = i;
	                                break;
	                            }
	                        }

	                        // Chooses a random material for the floor tile type with equal weight to all materials
	                        var lMaterial = Random.Range(0, _wallTileTypes[lType].Materials.Count);

	                        // Adds the tile
	                        _wallCustomTiles.Add(new WallTile(x, y, s, lType, lMaterial, false));
	                    }
	            }
	        }
	    }

	    // Adds the custom wall tiles
	    private void PopulateCustomWallTilesIntoArray()
	    {
	        foreach (var lTile in _wallCustomTiles)
	        {
	            var lTileType = _wallTileTypes[lTile.TileType];

	            // Finds the x,y index within _wallTiles for this custom tile
	            var xIndex = lTile.x;
	            var yIndex = lTile.y;

	            // Puts lTile into _wallTiles
	            _wallTiles[lTile.WallSide][xIndex, yIndex] = lTile;
	        }
	    }

	    // Adds door tiles to _wallTiles in all door positions
	    private void PopulateDoors()
	    {
	        for (var i = 0; i < _doorPositions.Length; i++)
	        {
	            if (_doorPositions[i] < 0) continue;

	            var x = _doorPositions[i] / CellSize;
	            var y = 0;
	            _wallTiles[i][x, y] = new WallTile(x, y, i, 0, -1, true);
	        }
	    }

	    // Builds one side of the wall
	    private void BuildWallSide(int aSide, Vector3 aBasePos, Quaternion aRot, Vector3 aXOffset, Vector3 aYOffset)
	    {
	        for (var x = 0; x < _wallTiles[aSide].GetLength(0); x++)
	        {
	            for (var y = 0; y < _wallTiles[aSide].GetLength(1); y++)
	            {
	                // Gets the position for this slot (frame and tile)
	                Vector3 lPos = aBasePos + x * aXOffset + y * aYOffset;

	                // Gets the tile
	                WallTile lTile = _wallTiles[aSide][x, y];
	                TileType lTileType = _wallTileTypes[lTile.TileType];

									// Gets the material
	                int lMaterial;
									if (lTile.Material >= 0)
									 	lMaterial = lTileType.Materials[lTile.Material];
									else
										lMaterial = -1;

	                // Creates wall frame
	                if (!_wallTiles[aSide][x, y].IsDoor)
	                    _room.CreateWallFrame(lPos, aRot, lMaterial);

	                // Creates wall tile
	                if (lTile.IsDoor)
	                    _room.CreateDoor(lPos, aRot, lMaterial, aSide);
	                else
	                    _room.CreateWallTile(lTileType.Model, lPos, aRot, lMaterial);
	            }
	        }
	    }

	    // Builds wall frames, tiles and doors
	    private void BuildWalls()
	    {
	        // Creates frame and tile objects
	        for (var i = 0; i < 4; i++)
	        {
	            var side = (WallSide)i;

	            /*
	                Sets base position, x offset, y offset and rotation
	                lXOffset is a vector for one wall tile unit in the x direction on this wall
	                lYOffset is a vector for one wall tile unit in the y direction on this wall
	                lBasePos is the origin (0,0) on this wall
	                lRot is the rotation for wall tiles/frames on this wall
	            */
	            Vector3 lBasePos = _room.transform.position;
	            Quaternion lRot = Quaternion.identity;
	            Vector3 lXOffset = Vector3.zero;
	            Vector3 lYOffset = new Vector3(0, CellSize, 0);
	            float w = (float)RoomWidth / 2;
	            float h = (float)RoomHeight / 2;
	            switch (side)
	            {
	                case WallSide.North:
	                    lBasePos.x       -= w;
	                    lBasePos.z       += h;
	                    lRot.eulerAngles =  new Vector3(-90, 180, 0);
	                    lXOffset         =  new Vector3(CellSize, 0, 0);
	                    lBasePos.x       += (float)CellSize / 2;
	                    break;
	                case WallSide.South:
	                    lBasePos.x       -= w;
	                    lBasePos.z       -= h;
	                    lRot.eulerAngles =  new Vector3(-90, 0, 0);
	                    lXOffset         =  new Vector3(CellSize, 0, 0);
	                    lBasePos.x       += (float)CellSize / 2;
	                    break;
	                case WallSide.West:
	                    lBasePos.x       -= w;
	                    lBasePos.z       -= h;
	                    lRot.eulerAngles =  new Vector3(-90, 90, 0);
	                    lXOffset         =  new Vector3(0, 0, CellSize);
	                    lBasePos.z       += (float)CellSize / 2;
	                    break;
	                case WallSide.East:
	                    lBasePos.x       += w;
	                    lBasePos.z       -= h;
	                    lRot.eulerAngles =  new Vector3(-90, 270, 0);
	                    lXOffset         =  new Vector3(0, 0, CellSize);
	                    lBasePos.z       += (float)CellSize / 2;
	                    break;
	            }

	            BuildWallSide(i, lBasePos, lRot, lXOffset, lYOffset);
	        }
	    }

	    /*   ==========================   */


	    /*  ===  Building Room  ===  */

	    // Checks if the doors are valid
	    private bool DoorsAreValid()
	    {
	        if (NumberOfDoors() < 1) return false;

	        return !_doorPositions.Where((t, i) => !DoorIsValid((WallSide)i, t)).Any();
	    }

	    // Checks if a given tile type is a valid floor tile type
	    public bool FloorTileTypeIsValid(TileType aTileType)
	    {
	        return _assets.FloorTileModelExists(aTileType.Model) && _assets.MaterialsExist(aTileType.Materials);
	    }

	    // Checks if a given tile type is a valid wall tile type
	    public bool WallTileTypeIsValid(TileType aTileType)
	    {
	        return _assets.WallTileModelExists(aTileType.Model) && _assets.MaterialsExist(aTileType.Materials);
	    }

	    // Re-Checks that all values are valid (i.e. in case the assets object was modified)
	    public bool CanBuildRoom()
	    {
	        // Checks if doors are valid
	        if (!DoorsAreValid()) return false;

	        // Checks if floor types are valid and checks if wall types are valid
	        return _floorTileTypes.All(FloorTileTypeIsValid) && _wallTileTypes.All(WallTileTypeIsValid);
	    }

	    // Physically builds the objects of the room in the scene
	    public void BuildRoom()
	    {
	        ClearCustomWallTiles();
	        ClearCustomFloorTiles();
	        // Clears the room if necessary
	        _room.ClearAllObjects();
	        _room.CreateParents();

	        // Builds the floor
	        PopulateFloorTiles();
	        CreateRandomFloorTiles();
	        PopulateCustomFloorTilesIntoArray();
	        BuildFloors();

	        // Creates the _wallTiles arrays to the right size, to prepare for the doors and walls
	        PrepareWallTileArrays();
	        // Populates _wallTiles with the walls
	        PopulateWallTiles();
	        // Creates random wall tiles into _wallCustomTiles
	        CreateRandomWallTiles();
	        // Populates _wallCustomTiles into _wallTiles
	        PopulateCustomWallTilesIntoArray();
	        // Populates _wallTiles with the doors
	        PopulateDoors();
	        // Builds the contents of _wallTiles (including doors)
	        BuildWalls();

	        // Builds the objects to be used by the stage generator and AI
	        _room.FinishRoom(_width, _height, _wallHeight, CellSize, _doorPositions, _roomWeight);
	    }

	    /*   ==========================   */


	    /*  ===  Editing Doors  ===  */

	    // Gets the number of doors
	    public int NumberOfDoors()
	    {
	        return _doorPositions.Count(t => t >= 0);
	    }

	    // Gets the door position for the given side
	    public int GetDoor(WallSide side)
	    {
	        return _doorPositions[(int)side];
	    }

	    // Checks if the given door value is within the limits
	    private bool DoorIsValid(WallSide side, int pos)
	    {
	        if (pos == -1)
	            return true;

	        if (pos / 5 * 5 != pos) return false;
	        if (pos < 0) return false;

	        if (side is WallSide.North or WallSide.South)
	        {
	            if (pos <= RoomWidth - 5)
	                return true;
	        }
	        else
	        {
	            if (pos <= RoomHeight - 5)
	                return true;
	        }

	        return false;
	    }

	    // Sets the door for the given side, if valid
	    public void SetDoor(WallSide side, int pos)
	    {
	        if (DoorIsValid(side, pos))
	            _doorPositions[(int)side] = pos;
	    }

	    /*   ==========================   */


	    /*  ===  Editing Custom Floor Tile Types  ===  */

	    //
	    public int AddCustomFloorTileType()
	    {
	        var lNewTile = new TileType(0, new List<int>(), DefaultFloorRotation, 1);
	        lNewTile.Materials.Add(0);
	        _floorTileTypes.Add(lNewTile);
	        return _floorTileTypes.Count - 1;
	    }

	    //
	    public void RemoveCustomFloorTileType(int aTypeID)
	    {
	        if (aTypeID > 0 && aTypeID < _floorTileTypes.Count)
	            _floorTileTypes.RemoveAt(aTypeID);
	    }

	    //
	    public void EditCustomFloorTileType(int aTypeID, int aModel, List<int> aMaterials, float aSpawnChanceWeight)
	    {
	        if (aTypeID > 0 && aTypeID < _floorTileTypes.Count)
	        {
	            // Sets the model if the model exists
	            if (_assets.FloorTileModelExists(aModel))
	                _floorTileTypes[aTypeID].Model = aModel;

	            // Sets the spawn chance if the number is positive
	            if (aSpawnChanceWeight >= 0)
	                _floorTileTypes[aTypeID].SpawnChanceWeight = aSpawnChanceWeight;

	            // Sets the materials if all materials exist
	            if (_assets.MaterialsExist(aMaterials))
	                _floorTileTypes[aTypeID].Materials = aMaterials;
	        }
	        else
	            Debug.Log("Error: Invalid aTypeID for floor custom tile type: " + aTypeID);
	    }

	    //
	    public int GetCustomFloorTileTypeCount()
	    {
	        return _floorTileTypes.Count;
	    }

	    //
	    public TileType GetCustomFloorTileType(int aTypeID)
	    {
	        if (aTypeID > 0 && aTypeID < _floorTileTypes.Count)
	            return new TileType(_floorTileTypes[aTypeID]);
	        Debug.Log("Error: Invalid aTypeID for floor custom tile type: " + aTypeID);
	        return null;
	    }

	    //
	    private bool ClearCustomFloorTiles()
	    {
	        _floorCustomTiles.Clear();
	        var lCleared = _floorCustomTiles.Count > 0;

	        return lCleared;
	    }

	    /*   ==========================   */


	    /*  ===  Editing Custom Wall Tile Types  ===  */

	    //
	    public int AddCustomWallTileType()
	    {
	        var lNewTile = new TileType(0, new List<int>(), DefaultWallRotation, 1);
	        lNewTile.Materials.Add(0);
	        _wallTileTypes.Add(lNewTile);
	        return _wallTileTypes.Count - 1;
	    }

	    //
	    public void RemoveCustomWallTileType(int aTypeID)
	    {
	        if (aTypeID > 0 && aTypeID < _wallTileTypes.Count)
	            _wallTileTypes.RemoveAt(aTypeID);
	    }

	    //
	    public void EditCustomWallTileType(int aTypeID, int aModel, List<int> aMaterials, float aSpawnChanceWeight)
	    {
	        if (aTypeID > 1 && aTypeID < _wallTileTypes.Count)
	        {
	            // Sets the model if the model exists
	            if (_assets.WallTileModelExists(aModel))
	                _wallTileTypes[aTypeID].Model = aModel;

	            // Sets the spawn chance if the number is positive
	            if (aSpawnChanceWeight >= 0)
	                _wallTileTypes[aTypeID].SpawnChanceWeight = aSpawnChanceWeight;

	            // Sets the materials if all materials exist
	            if (_assets.MaterialsExist(aMaterials))
	                _wallTileTypes[aTypeID].Materials = aMaterials;
	        }
	        else
	            Debug.Log("Error: Invalid aTypeID for wall custom tile type: " + aTypeID);
	    }

	    //
	    public int GetCustomWallTileTypeCount()
	    {
	        return _wallTileTypes.Count;
	    }

	    //
	    public TileType GetCustomWallTileType(int aTypeID)
	    {
	        if (aTypeID > 1 && aTypeID < _wallTileTypes.Count)
	            return new TileType(_wallTileTypes[aTypeID]);
	        Debug.Log("Error: Invalid aTypeID for wall custom tile type: " + aTypeID);
	        return null;
	    }

	    //
	    private bool ClearCustomWallTiles()
	    {
	        _wallCustomTiles.Clear();
	        var lCleared = _wallCustomTiles.Count > 0;

	        return lCleared;
	    }

	    /*   ==========================   */


	    /*  ===  Saving and Loading Data to Files  ===  */

	    public void SaveData(string aFilePath)
	    {
	        var lFile = File.Exists(aFilePath) ? File.OpenWrite(aFilePath) : File.Create(aFilePath);
	        var lData = new RoomData(_width, _height, _wallHeight, _roomWeight, _doorPositions, _doorDefaultMaterial, _floorTileTypes,
	            _floorRandomSpawnChance, _wallTileTypes, _wallRandomSpawnChance);

	        var lBinaryFormatter = new BinaryFormatter();
	        lBinaryFormatter.Serialize(lFile, lData);

	        lFile.Close();
	    }

	    public void LoadData(string aFilePath)
	    {
	        FileStream lFile;

	        if (File.Exists(aFilePath))
	            lFile = File.OpenRead(aFilePath);
	        else
	        {
	            Debug.Log("Error: Invalid filename");
	            return;
	        }

	        var lBinaryFormatter = new BinaryFormatter();
	        var lData = (RoomData)lBinaryFormatter.Deserialize(lFile);
	        lFile.Close();

	        _width      = lData.Width;
	        _height     = lData.Height;
	        _wallHeight = lData.WallHeight;
	        _roomWeight = lData.RoomWeight;

	        _doorPositions       = lData.DoorPositions;
	        _doorDefaultMaterial = lData.DoorMaterial;

	        _floorTileTypes         = lData.FloorTileTypes;
	        _floorRandomSpawnChance = lData.FloorRandomSpawnChance;

	        _wallTileTypes         = lData.WallTileTypes;
	        _wallRandomSpawnChance = lData.WallRandomSpawnChance;
	    }

	    public class TileType
	    {
	        public List<int> Materials;
	        public int Model;
	        public Quaternion Rotation;
	        public float SpawnChanceWeight;

	        public TileType(int aModel, List<int> aMaterials, Quaternion aRotation, float aSpawnChanceWeight)
	        {
	            Model             = aModel;
	            Materials         = aMaterials;
	            Rotation          = aRotation;
	            SpawnChanceWeight = aSpawnChanceWeight;
	        }

	        public TileType(TileType aTileType)
	        {
	            Model             = aTileType.Model;
	            Materials         = new List<int>(aTileType.Materials);
	            Rotation          = aTileType.Rotation;
	            SpawnChanceWeight = aTileType.SpawnChanceWeight;
	        }
	    }

	    public class FloorTile
	    {
	        public bool IsCovered;
	        public int Material;
	        public int TileType;
	        public float x, y;

	        public FloorTile(float aX, float aY, int aTileType, int aMaterial)
	        {
	            x         = aX;
	            y         = aY;
	            TileType  = aTileType;
	            Material  = aMaterial;
	            IsCovered = false;
	        }
	    }

	    public class WallTile
	    {
	        public bool IsDoor;
	        public int Material;
	        public int TileType;
	        public int x, y, WallSide;

	        public WallTile(int aX, int aY, int aWallSide, int aTileType, int aMaterial, bool isDoor)
	        {
	            x        = aX;
	            y        = aY;
	            WallSide = aWallSide;
	            TileType = aTileType;
	            Material = aMaterial;
	            IsDoor   = isDoor;
	        }
	    }
	}
}
