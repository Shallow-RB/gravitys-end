using System;
using System.Collections.Generic;
using System.Linq;
using Core.StageGeneration.Rooms;
using Core.StageGeneration.Rooms.RoomTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Core.StageGeneration.Stage
{
    public class StageHelper : MonoBehaviour
    {
        [HideInInspector]
        public enum RoomDirections
        {
            TOP,
            RIGHT,
            BOTTOM,
            LEFT,
            UNDEFINED
        }

        public static StageHelper Instance;

        private static int _gridX;
        private static int _gridZ;
        private static int _offset;
        private static GameObject _keyRoom;
        private static List<Cell> _cells;
        private static List<GameObject> _rooms;

        [HideInInspector]
        public SpawnRoom spawnRoom;

        private static List<NavMeshBuildSource> sources;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != null) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public static int GetGridX()
        {
            return _gridX;
        }

        public static void SetGridX(int gridX)
        {
            _gridX = gridX;
        }

        public static int GetGridZ()
        {
            return _gridZ;
        }

        public static void SetGridZ(int gridZ)
        {
            _gridZ = gridZ;
        }

        public static int GetOffset()
        {
            return _offset;
        }

        public static void SetOffset(int _offset)
        {
            StageHelper._offset = _offset;
        }

        public static List<Cell> GetCells()
        {
            return _cells;
        }

        public static void SetCells(List<Cell> _cells)
        {
            StageHelper._cells = _cells;
        }

        public static List<GameObject> GetRooms()
        {
            return _rooms;
        }

        public static void SetRooms(List<GameObject> _rooms)
        {
            StageHelper._rooms = _rooms;
        }

        public static GameObject GetKeyRoom()
        {
            return _keyRoom;
        }

        public static void SetKeyRoom(GameObject _keyRoom)
        {
            StageHelper._keyRoom = _keyRoom;
        }

        public static RoomDirections RandomDirection()
        {
            return (RoomDirections)Random.Range(0, Enum.GetValues(typeof(RoomDirections)).Length);
        }

        public static RoomDirections RandomDirectionFromRoom(GameObject room)
        {
            var openDirections = room.GetComponent<Room>().GetDoors().Where(d => d.GetComponent<Door>().hasNeighbour != false)
                .Select(d => d.GetComponent<Door>().GetDirection()).ToList();

            return (RoomDirections)Random.Range(0, openDirections.Count);
        }

        public static RoomDirections RandomDirection(List<RoomDirections> directions)
        {
            directions.Remove(RoomDirections.UNDEFINED);

            if (directions != null && directions.Count > 0)
            {
                if (directions.Count > 1)
                {
                    return directions[Random.Range(0, directions.Count)];
                }
                else
                {
                    return directions[0];
                }
            }
            return RoomDirections.UNDEFINED;
        }

        public static RoomDirections GetOppositeDirection(RoomDirections direction)
        {
            //Cant be null so I gave just a value that will always be changed
            var oppositeDirection = RoomDirections.UNDEFINED;

            switch (direction)
            {
                case RoomDirections.TOP:
                    oppositeDirection = RoomDirections.BOTTOM;
                    break;
                case RoomDirections.RIGHT:
                    oppositeDirection = RoomDirections.LEFT;
                    break;
                case RoomDirections.BOTTOM:
                    oppositeDirection = RoomDirections.TOP;
                    break;
                case RoomDirections.LEFT:
                    oppositeDirection = RoomDirections.RIGHT;
                    break;
            }

            return oppositeDirection;
        }

        public static void ReplaceAllDoors(GameObject room)
        {
            var doors = room.GetComponent<Room>().GetDoors().Where(d => d.activeSelf).ToList();

            foreach (var _door in doors)
            {
                // Tells the door block to replace itself with a wall and open the door model
                // If no door block component was found, the door is destroyed like before
                // Test rooms before the roomeditor implementation wont work anymore
                RoomEditor.DoorBlock lDoorBlock = _door.GetComponent<RoomEditor.DoorBlock>();
                if (lDoorBlock != null)
                    lDoorBlock.CloseDoor();
                else
                    Destroy(_door);
            }
        }

        public static void NavMeshBaker()
        {
            // Find all game objects with the specified tag
            var taggedObjects = GameObject.FindGameObjectsWithTag("Room");

            // Create a NavMeshBuildSettings object
            var settings = NavMesh.GetSettingsByID(0);

            // Create an array to hold the NavMeshBuildSources
            sources = new List<NavMeshBuildSource>();

            // Iterate through all the tagged game objects and their children
            foreach (var obj in taggedObjects) AddSourcesFromObject(obj);

            var centerCell = CalculateCenterStage();

            // Build the NavMesh
            var data = NavMeshBuilder.BuildNavMeshData(settings, sources, new Bounds(
                centerCell.gameObject.transform.position,
                new Vector3(_gridX * _offset, 30f, _gridZ * _offset)), Vector3.zero, Quaternion.identity);
            NavMesh.AddNavMeshData(data);
        }

        private static Cell CalculateCenterStage()
        {
            float calcX = 0;
            float calcZ = 0;

            if (_gridX % 2 == 0)
                calcX = _gridX / 2 - 1;
            else
                calcX = Mathf.RoundToInt(_gridX / 2 - 1);

            if (_gridZ % 2 == 0)
                calcZ = _gridZ / 2 - 1;
            else
                calcZ = Mathf.RoundToInt(_gridZ / 2 - 1);

            return _cells.Where(c => c.x == calcX && c.z == calcZ).SingleOrDefault();
        }

        private static void AddSourcesFromObject(GameObject obj)
        {
            var meshFilters = obj.GetComponentsInChildren<MeshFilter>();

            // Add a NavMeshBuildSource for each mesh filter
            foreach (var filter in meshFilters)
                if (obj.tag == "Floor" || obj.tag == "Wall" || obj.tag == "Door" && obj.activeSelf)
                {
                    var source = new NavMeshBuildSource
                    {
                        transform = filter.transform.localToWorldMatrix,
                        shape = NavMeshBuildSourceShape.Mesh,
                        sourceObject = filter.sharedMesh,
                        area = 0
                    };

                    // #if UNITY_EDITOR
                    // // Add the NavMeshBuildSource to the sources array
                    // ArrayUtility.Add(ref sources, source);
                    // #else
                    ArrayUtilAdd(source);
                    // #endif
                }

            // Recursively add sources from all children
            foreach (Transform child in obj.transform) AddSourcesFromObject(child.gameObject);
        }


        private static void ArrayUtilAdd(NavMeshBuildSource element)
        {
            sources.Add(element);
        }
    }
}
