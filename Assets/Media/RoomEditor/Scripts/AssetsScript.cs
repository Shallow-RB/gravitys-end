using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomEditor;

namespace RoomEditor {
	public class AssetsScript : MonoBehaviour
	{
		// Floor Assets
		public GameObject FloorFrameModel;
		public GameObject[] FloorTileModels;
		public int[] FloorTileModelScales;

		// Wall Assets
		public GameObject WallFrameModel;
		public GameObject[] WallTileModels;

		// Door Asset
		public GameObject DoorModel;

		// Materials
		public Material[] Materials;

		// Block Objects
		public GameObject FloorBlock;
		public GameObject WallBlock;
		public GameObject DoorBlock;

		// Default Chest Prefab
		public GameObject DefaultChest;

		// Fog of war Prefab
		public GameObject FogOfWarPlane;
		public Material TransparentMaterial;

		public bool AssetsValid()
		{
			if (FloorFrameModel is null) return false;
			if (WallFrameModel is null) return false;
			if (DoorModel is null) return false;
			if (FloorBlock is null) return false;
			if (WallBlock is null) return false;
			if (DoorBlock is null) return false;

			if (FloorTileModelScales.Length != FloorTileModels.Length) return false;
			if (FloorTileModels.Length < 1) return false;
			for (var i = 0; i < FloorTileModels.Length; i++)
			{
				if (FloorTileModels[i] is null) return false;
				if (FloorTileModelScales[i] < 1) return false;
			}

			return WallTileModels.Length >= 1 && WallTileModels.All(t => t is not null);
		}

		public bool FloorTileModelExists(int aModel)
		{
			return aModel < FloorTileModels.Length && aModel >= 0;
		}

		public bool WallTileModelExists(int aModel)
		{
			return aModel < WallTileModels.Length && aModel >= 0;
		}

		public bool MaterialExists(int aMaterial)
		{
			return aMaterial < Materials.Length && aMaterial >= 0;
		}

		public bool MaterialsExist(IEnumerable<int> aMaterials)
		{
			return aMaterials.All(MaterialExists);
		}
	}
}
