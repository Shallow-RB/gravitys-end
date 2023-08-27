using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomEditor;

namespace RoomEditor {
	[Serializable]
	public class RoomData
	{
	    // Room basic data
	    public int Width;
	    public int Height;
	    public int WallHeight;
			public int RoomWeight;

	    // Door data
	    public int[] DoorPositions;
	    public int DoorMaterial;
	    public float FloorRandomSpawnChance;
	    public float WallRandomSpawnChance;

	    // Floor data
	    private TileTypeSerializable[] _floorTileTypes;

	    // Wall data
	    private TileTypeSerializable[] _wallTileTypes;

	    public RoomData(int aWidth, int aHeight, int aWallHeight, int aRoomWeight, int[] aDoorPositions, int aDoorMaterial,
	        List<RoomBuilder.TileType> aFloorTileTypes, float aFloorRandomSpawnChance,
	        List<RoomBuilder.TileType> aWallTileTypes, float aWallRandomSpawnChance)
	    {
	        Width      = aWidth;
	        Height     = aHeight;
	        WallHeight = aWallHeight;
					RoomWeight = aRoomWeight;

	        DoorPositions = aDoorPositions;
	        DoorMaterial  = aDoorMaterial;

	        FloorTileTypes         = aFloorTileTypes;
	        FloorRandomSpawnChance = aFloorRandomSpawnChance;

	        WallTileTypes         = aWallTileTypes;
	        WallRandomSpawnChance = aWallRandomSpawnChance;
	    }

	    public List<RoomBuilder.TileType> FloorTileTypes
	    {
	        get { return _floorTileTypes.Select(t => t.GetTileType()).ToList(); }
	        set
	        {
	            _floorTileTypes = new TileTypeSerializable[value.Count];

	            for (var i = 0; i < value.Count; i++) _floorTileTypes[i] = new TileTypeSerializable(value[i]);
	        }
	    }

	    public List<RoomBuilder.TileType> WallTileTypes
	    {
	        get { return _wallTileTypes.Select(t => t.GetTileType()).ToList(); }
	        set
	        {
	            _wallTileTypes = new TileTypeSerializable[value.Count];

	            for (var i = 0; i < value.Count; i++) _wallTileTypes[i] = new TileTypeSerializable(value[i]);
	        }
	    }

	    [Serializable]
	    private class TileTypeSerializable
	    {
	        public int Model;
	        public List<int> Materials;
	        public float[] Rotation;
	        public float SpawnChanceWeight;

	        public TileTypeSerializable(RoomBuilder.TileType aTileType)
	        {
	            Model = aTileType.Model;

	            Materials = aTileType.Materials;

	            var lVec = aTileType.Rotation.eulerAngles;
	            Rotation = new float[3] { lVec.x, lVec.y, lVec.z };

	            SpawnChanceWeight = aTileType.SpawnChanceWeight;
	        }

	        public RoomBuilder.TileType GetTileType()
	        {
	            var lRot = Quaternion.identity;
	            lRot.eulerAngles = new Vector3(Rotation[0], Rotation[1], Rotation[2]);

	            return new RoomBuilder.TileType(Model, Materials, lRot, SpawnChanceWeight);
	        }
	    }
	}
}
