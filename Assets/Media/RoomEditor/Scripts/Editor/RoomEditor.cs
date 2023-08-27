using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RoomEditor;

namespace RoomEditor {
	[CustomEditor(typeof(RoomScript))]
	public class RoomEditor : Editor
	{
	    private const RoomBuilder.WallSide North = RoomBuilder.WallSide.North;
	    private const RoomBuilder.WallSide South = RoomBuilder.WallSide.South;
	    private const RoomBuilder.WallSide West = RoomBuilder.WallSide.West;
	    private const RoomBuilder.WallSide East = RoomBuilder.WallSide.East;
			private bool _doorsMustBeCentred = true;
	    private AssetsScript _assets;

	    private RoomBuilder _builder;
	    private bool _foldoutMenuDoor;
	    private bool _foldoutMenuFloor;
	    private bool _foldoutMenuFloorVariations;

	    // Menu data
	    private bool _foldoutMenuRoomData = true;
	    private bool _foldoutMenuWall;
	    private bool _foldoutMenuWallVariations;
	    private string _newSaveLocation;
	    private RoomScript _room;


	    //
	    public void OnEnable()
	    {
	        var lTarget = target;
	        _room = (RoomScript)lTarget;

	        _assets = _room.transform.Find("Assets").GetComponent<AssetsScript>();
	        if (_assets == null) Debug.Log("Error: Could not find Assets object!");
	        _room.Assets = _assets;

	        _newSaveLocation = _room.SaveLocation;

	        _builder = new RoomBuilder(_room, _assets);
	    }


	    /*  ===  Scene GUI  ===  */

	    // Draws GUI in the scene view
	    public void OnSceneGUI()
	    {
	        _room        = (RoomScript)target;
	        _room.Assets = _assets;
	        DisplayRoomOutline();
	        DisplayDoorOutlines();
	    }

	    // Draws the Inspector GUI
	    public override void OnInspectorGUI()
	    {
	        _newSaveLocation = EditorGUILayout.TextField("Room Data File", _newSaveLocation);
	        if (GUILayout.Button("Set Location"))
	            _room.SaveLocation = _newSaveLocation;

	        _builder.LoadData(Application.dataPath + "/Media/RoomEditor/RoomData/" + _room.SaveLocation);

	        // Displays input menu for basic room data
	        DisplayRoomMenu();

	        // Displays input menu for floors
	        DisplayFloorMenu();

	        // For creating additional floor variations
	        DisplayFloorVariationMenu();

	        // Displays input menu for walls
	        DisplayWallMenu();

	        // Displays input menu for doors
	        DisplayDoorMenu();

	        // For creating additional wall variations
	        DisplayWallVariationMenu();

	        // Lets the user know if the requirements are met
	        EditorGUILayout.Space();
	        if (_assets.AssetsValid())
	        {
	            if (_builder.CanBuildRoom())
	            {
	                if (GUILayout.Button("Build Room")) _builder.BuildRoom();
	            }
	            else
	                GUILayout.Label("Room data is non-compliant. Cannot build room.");
	        }
	        else
	            GUILayout.Label("Assets are missing or non-compliant. Cannot build room.");

	        _builder.SaveData(Application.dataPath + "/Media/RoomEditor/RoomData/" + _room.SaveLocation);
	    }

	    //
	    private void DisplayRoomOutline()
	    {
	        // Data for drawing rects
	        var centre = _room.transform.position;
	        var w = (float)_builder.RoomWidth / 2;
	        var h = (float)_builder.RoomHeight / 2;
	        float wh = _builder.WallHeight;
	        var outlineCol = new Color(0.0f, 1.0f, 0.0f, 1.0f);
	        var faceCol = new Color(0.0f, 1.0f, 0.0f, 0.025f);

	        // Displays floor
	        Vector3[] verts =
	        {
	            new(centre.x + w, centre.y, centre.z + h),
	            new(centre.x + w, centre.y, centre.z - h),
	            new(centre.x - w, centre.y, centre.z - h),
	            new(centre.x - w, centre.y, centre.z + h)
	        };
	        Handles.DrawSolidRectangleWithOutline(verts, faceCol, outlineCol);

	        // Displays walls
	        faceCol = new Color(0.0f, 1.0f, 0.0f, 0.0f);
	        verts = new Vector3[]
	        {
	            new(centre.x + w, centre.y, centre.z + h),
	            new(centre.x + w, centre.y, centre.z - h),
	            new(centre.x + w, centre.y + wh, centre.z - h),
	            new(centre.x + w, centre.y + wh, centre.z + h)
	        };
	        Handles.DrawSolidRectangleWithOutline(verts, faceCol, outlineCol);
	        verts = new Vector3[]
	        {
	            new(centre.x - w, centre.y, centre.z + h),
	            new(centre.x - w, centre.y, centre.z - h),
	            new(centre.x - w, centre.y + wh, centre.z - h),
	            new(centre.x - w, centre.y + wh, centre.z + h)
	        };
	        Handles.DrawSolidRectangleWithOutline(verts, faceCol, outlineCol);
	        verts = new Vector3[]
	        {
	            new(centre.x + w, centre.y, centre.z + h),
	            new(centre.x - w, centre.y, centre.z + h),
	            new(centre.x - w, centre.y + wh, centre.z + h),
	            new(centre.x + w, centre.y + wh, centre.z + h)
	        };
	        Handles.DrawSolidRectangleWithOutline(verts, faceCol, outlineCol);
	        verts = new Vector3[]
	        {
	            new(centre.x + w, centre.y, centre.z - h),
	            new(centre.x - w, centre.y, centre.z - h),
	            new(centre.x - w, centre.y + wh, centre.z - h),
	            new(centre.x + w, centre.y + wh, centre.z - h)
	        };
	        Handles.DrawSolidRectangleWithOutline(verts, faceCol, outlineCol);
	    }

	    //
	    private void DisplayDoorOutlines()
	    {
	        // Draws the outline for each door
	        for (var i = 0; i < 4; i++)
	        {
	            // Checks if there is a door on this wall
	            var side = (RoomBuilder.WallSide)i;
	            var doorPos = _builder.GetDoor(side);
	            if (doorPos < 0) continue;

	            var outlineCol = new Color(0.0f, 0.0f, 1.0f, 1.0f);
	            var faceCol = new Color(0.0f, 0.0f, 1.0f, 0.025f);

	            var northOrSouth = side == North || side == South;
	            var w = (float)_builder.RoomWidth / 2;
	            var h = (float)_builder.RoomHeight / 2;

	            // Works out the "origin" from which the door's position is relative to
	            var wallOrigin = _room.transform.position;
	            switch (side)
	            {
	                case North:
	                    wallOrigin.x -= w;
	                    wallOrigin.z += h;
	                    break;
	                case East:
	                    wallOrigin.x += w;
	                    wallOrigin.z -= h;
	                    break;
	                case RoomBuilder.WallSide.South:
	                case RoomBuilder.WallSide.West:
	                default:
	                    wallOrigin.x -= w;
	                    wallOrigin.z -= h;
	                    break;
	            }

	            // Starts creating the vertices for the current door's rect
	            var verts = new Vector3[4];
	            // Offsets all vertices to be at the starting vertex of the door
	            for (var j = 0; j < 4; j++)
	            {
	                verts[j] = wallOrigin;
	                if (northOrSouth)
	                    verts[j].x += doorPos;
	                else
	                    verts[j].z += doorPos;
	            }

	            // Offsets the vertices horizontally
	            if (northOrSouth)
	            {
	                verts[1].x += 5;
	                verts[2].x += 5;
	            }
	            else
	            {
	                verts[1].z += 5;
	                verts[2].z += 5;
	            }

	            // Offsets the vertices vertically
	            verts[2].y += 5;
	            verts[3].y += 5;

	            // Draws the rect
	            Handles.DrawSolidRectangleWithOutline(verts, faceCol, outlineCol);
	        }
	    }

	    /*   ==========================   */


	    /*  ===  Other Properties  ===  */

	    // Displays input menu for basic room data
	    private void DisplayRoomMenu()
	    {
	        _foldoutMenuRoomData = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMenuRoomData, "Set Room Properties");
	        if (_foldoutMenuRoomData)
	        {
	            _builder.RoomWidth  = EditorGUILayout.IntField("Room Width", _builder.RoomWidth);
	            _builder.RoomHeight = EditorGUILayout.IntField("Room Height", _builder.RoomHeight);
	            GUILayout.Label("Room dimensions must follow y = 20x + 15,\nwhere x is a positive integer.");
	            _builder.WallHeight = EditorGUILayout.IntField("Wall Height", _builder.WallHeight);
	            GUILayout.Label("Wall height must be a positive multiple of 5.");
							_builder.RoomWeight = EditorGUILayout.IntField("Room Weight", _builder.RoomWeight);
	        }

	        EditorGUILayout.EndFoldoutHeaderGroup();
	    }

		//
		private void DisplaySingleDoorMenu(string doorName, RoomBuilder.WallSide side)
		{
			// Finds the centre position of this door
			int lDoorCentre = 0;
			switch (side) {
				case North:
				case South:
					lDoorCentre = (_builder.RoomWidth - 5) / 2;
					break;
				case West:
				case East:
					lDoorCentre = (_builder.RoomHeight - 5) / 2;
					break;
			}

			// Gets the current position of this door
			int doorPos = _builder.GetDoor(side);

			// Heading for this door
			EditorGUILayout.Space();
			if (doorPos >= 0)
				GUILayout.Label(doorName + " Door");

			// Displays the editor for this door
			if (doorPos >= 0)
			{
				// Allows the user to edit the door position if the door doesn't need to be centred
				if (!_doorsMustBeCentred)
					doorPos = EditorGUILayout.IntField("Door Position", doorPos);
				else
					doorPos = lDoorCentre;

				// Sets the door position
				_builder.SetDoor(side, doorPos);

				// Remove button for door
				if (GUILayout.Button("Remove " + doorName + " Door"))
					_builder.SetDoor(side, -1);
			}
			else
			{
				// Add button for door
				if (GUILayout.Button("Add " + doorName + " Door"))
					_builder.SetDoor(side, lDoorCentre);
			}
		}

	    // Displays input menu for doors
	    private void DisplayDoorMenu()
	    {
	        // Door menu header
	        _foldoutMenuDoor = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMenuDoor, "Setup Doors");
	        if (_foldoutMenuDoor)
	        {
	            // Door Material
	            _builder.DefaultDoorMaterial = EditorGUILayout.IntField("Door Material", _builder.DefaultDoorMaterial);

	            // North door
	            DisplaySingleDoorMenu("North", North);
	            // South door
	            DisplaySingleDoorMenu("South", South);
	            // West door
	            DisplaySingleDoorMenu("West", West);
	            // East door
	            DisplaySingleDoorMenu("East", East);
	        }

	        EditorGUILayout.EndFoldoutHeaderGroup();
	    }

	    /*   ==========================   */


	    /*  ===  Floor Properties  ===  */

	    // Displays menu for the floor data
	    private void DisplayFloorMenu()
	    {
	        _foldoutMenuFloor = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMenuFloor, "Setup Floor Data");
	        if (_foldoutMenuFloor)
	        {
	            // Floor Default Tile Object
	            _builder.FloorDefaultTileModel =
	                EditorGUILayout.IntField("Floor Default Tile Model", _builder.FloorDefaultTileModel);

	            // Floor Default Material
	            _builder.FloorDefaultTileMaterial =
	                EditorGUILayout.IntField("Floor Default Material", _builder.FloorDefaultTileMaterial);
	        }

	        EditorGUILayout.EndFoldoutHeaderGroup();
	    }

	    // For editing an array of materials
	    private List<int> DisplayMaterialArrayEditor(List<int> aMaterials)
	    {
	        var lMaterials = new List<int>(aMaterials);

	        for (var i = 0; i < lMaterials.Count; i++)
	            lMaterials[i] = EditorGUILayout.IntField("Material " + i, lMaterials[i]);

	        if (GUILayout.Button("Add New Material"))
	            lMaterials.Add(0);

	        if (lMaterials.Count > 1)
	        {
	            if (GUILayout.Button("Remove Material"))
	                lMaterials.RemoveAt(lMaterials.Count - 1);
	        }

	        return lMaterials;
	    }

	    // For editing custom floor variations
	    private void DisplayCustomFloorTypeEditor(int aCustomTypeIndex)
	    {
	        var lTileType = _builder.GetCustomFloorTileType(aCustomTypeIndex);

	        EditorGUILayout.Space();
	        EditorGUILayout.Space();
	        EditorGUILayout.Space();

	        // Model
	        var lModel = EditorGUILayout.IntField("Model", lTileType.Model);

	        // Spawn Chance Weight
	        var lSpawnChanceWeight = EditorGUILayout.FloatField("Spawn Chance Weight", lTileType.SpawnChanceWeight);

	        // Materials for this custom floor tile
	        var lMaterials = DisplayMaterialArrayEditor(lTileType.Materials);

	        EditorGUILayout.Space();
	        // Allows the user to remove this floor variation
	        if (GUILayout.Button("Remove This Floor Variation"))
	            _builder.RemoveCustomFloorTileType(aCustomTypeIndex);
	        else
	            _builder.EditCustomFloorTileType(aCustomTypeIndex, lModel, lMaterials, lSpawnChanceWeight);

	        EditorGUILayout.Space();
	        EditorGUILayout.Space();
	        EditorGUILayout.Space();
	    }

	    // Displays menu for the floor variation data
	    private void DisplayFloorVariationMenu()
	    {
	        // Heading
	        _foldoutMenuFloorVariations =
	            EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMenuFloorVariations, "Setup Floor Variations");
	        if (_foldoutMenuFloorVariations)
	        {
	            // For setting random spawn chance for variations
	            _builder.FloorRandomSpawnChance =
	                EditorGUILayout.FloatField("Spawn Chance", _builder.FloorRandomSpawnChance);
	            GUILayout.Label(
	                "Set the chance of spawning a floor variation\nfor any given floor tile (0-1).\nSet to 0 to disable randomisation.");
	            EditorGUILayout.Space();

	            // Displays current floor tile types
	            for (var i = 1; i < _builder.GetCustomFloorTileTypeCount(); i++) DisplayCustomFloorTypeEditor(i);

	            // Allows user to add new floor tile types
	            if (GUILayout.Button("Add New Floor Variation")) _builder.AddCustomFloorTileType();
	        }

	        EditorGUILayout.EndFoldoutHeaderGroup();
	    }

	    /*   ==========================   */


	    /*  ===  Wall Properties  ===  */

	    // Displays menu for the wall data
	    private void DisplayWallMenu()
	    {
	        _foldoutMenuWall = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMenuWall, "Setup Wall Data");
	        if (_foldoutMenuWall)
	        {
	            // Wall default tile object
	            _builder.WallDefaultTileModel =
	                EditorGUILayout.IntField("Wall Default Tile Model", _builder.WallDefaultTileModel);

	            // Wall material
	            _builder.WallDefaultTileMaterial =
	                EditorGUILayout.IntField("Wall Material", _builder.WallDefaultTileMaterial);
	        }

	        EditorGUILayout.EndFoldoutHeaderGroup();
	    }

	    // For editing custom wall variations
	    private void DisplayCustomWallTypeEditor(int aCustomTypeIndex)
	    {
	        var lTileType = _builder.GetCustomWallTileType(aCustomTypeIndex);

	        EditorGUILayout.Space();
	        EditorGUILayout.Space();
	        EditorGUILayout.Space();

	        // Model
	        var lModel = EditorGUILayout.IntField("Model", lTileType.Model);

	        // Spawn Chance Weight
	        var lSpawnChanceWeight = EditorGUILayout.FloatField("Spawn Chance Weight", lTileType.SpawnChanceWeight);

	        // Materials for this custom floor tile
	        var lMaterials = DisplayMaterialArrayEditor(lTileType.Materials);

	        EditorGUILayout.Space();
	        // Allows the user to remove this wall variation
	        if (GUILayout.Button("Remove This Wall Variation"))
	            _builder.RemoveCustomWallTileType(aCustomTypeIndex);
	        else
	            _builder.EditCustomWallTileType(aCustomTypeIndex, lModel, lMaterials, lSpawnChanceWeight);

	        EditorGUILayout.Space();
	        EditorGUILayout.Space();
	        EditorGUILayout.Space();
	    }

	    //
	    private void DisplayWallVariationMenu()
	    {
	        // Heading
	        _foldoutMenuWallVariations =
	            EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMenuWallVariations, "Setup Wall Variations");
	        if (_foldoutMenuWallVariations)
	        {
	            // For setting random spawn chance for variations
	            _builder.WallRandomSpawnChance = EditorGUILayout.FloatField("Spawn Chance", _builder.WallRandomSpawnChance);
	            GUILayout.Label(
	                "Set the chance of spawning a wall variation\nfor any given wall tile (0-1).\nSet to 0 to disable randomisation.");
	            EditorGUILayout.Space();

	            // Displays current wall tile types
	            for (var i = 2; i < _builder.GetCustomWallTileTypeCount(); i++) DisplayCustomWallTypeEditor(i);

	            // Allows user to add new wall tile types
	            if (GUILayout.Button("Add New Wall Variation")) _builder.AddCustomWallTileType();
	        }

	        EditorGUILayout.EndFoldoutHeaderGroup();
	    }
	}
}
