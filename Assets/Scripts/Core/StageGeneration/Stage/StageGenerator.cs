using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Chest;
using Core.StageGeneration.Rooms;
using Core.StageGeneration.Rooms.RoomTypes;
using Core.StageGeneration.Rooms.Util;
using UnityEngine;
using Utils;

namespace Core.StageGeneration.Stage
{
    public class StageGenerator : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField]
        private int gridX;

        [SerializeField]
        private int gridZ;

        [SerializeField]
        private int offset;

        [SerializeField]
        private GameObject gridCell;

        [SerializeField]
        private bool debugMode;

        [Header("Hallway settings")]
        [SerializeField]
        private List<GameObject> hallways = new();

        [SerializeField]
        private GameObject hallwayEndLeft;

        [SerializeField]
        private GameObject hallwayEndRight;

        [SerializeField]
        private GameObject bossHallway;

        [Header("Room Settings")]
        [SerializeField]
        private int minWeightRoomsBranch;

        [SerializeField]
        private int maxWeightRoomsBranch;

        [SerializeField]
        private GameObject spawnRoom;

        [SerializeField]
        private GameObject bossRoom;

        [SerializeField]
        private GameObject keyRoom;

        [SerializeField]
        private List<GameObject> rooms;

        [HideInInspector]
        public List<Cell> cells;

        [SerializeField]
        public float maxYieldTime;

        public readonly List<GameObject> mapHallways = new();

        public readonly List<GameObject> mapRooms = new();

        public static StageGenerator instance;

        private RoomGenerator _roomGenerator;

        private GameObject _stageParent;

        private float startTime;

        private void Start()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            startTime = Time.time;

            //Create empty game object called Stage to store the cells in
            _stageParent = new GameObject("Stage");

            //Loop through the X and Z to create the grid
            for (var i = 0; i < gridX; i++)
                for (var j = 0; j < gridZ; j++)
                    SpawnCell(i, j);

            StageHelper.SetGridX(gridX);
            StageHelper.SetGridZ(gridZ);
            StageHelper.SetOffset(offset);
            StageHelper.SetCells(cells);
            StageHelper.SetRooms(rooms);
            StageHelper.SetKeyRoom(keyRoom);

            InitializeRoomSizes();

            _roomGenerator = RoomGenerator.instance;

            //Start spawning bossroom
            StartCoroutine(BossRoomGenerator());
        }

        public Vector2Int GetXZ()
        {
            return new Vector2Int(gridX, gridZ);
        }

        private IEnumerator BossRoomGenerator()
        {
            var _spawnRoomObject = SpawnRoom();
            var hallway = bossHallway;
            var _spawnRoom = _spawnRoomObject.GetComponent<Room>();

            float posX = _spawnRoom.transform.position.x;
            var posZ = Mathf.RoundToInt((_spawnRoom.transform.position.z + _spawnRoom.sizeZ / 2) + (hallway.GetComponent<Room>().sizeZ / 2));

            hallway = Instantiate(hallway, new Vector3(posX, 0, posZ), Quaternion.identity);

            var roomCells = SetRoomCells(hallway, posX, posZ);
            hallway.GetComponent<Room>().cells = roomCells;

            // mapHallways.Add(hallway);

            var _bossRoom = SpawnBossRoom(hallway);

            yield return StartCoroutine(HallwayGenerator(_spawnRoom));
        }

        //Generate the finite hallway in the game
        private IEnumerator HallwayGenerator(Room initialRoom)
        {
            var _spawnRoom = initialRoom;

            //Counts for each side and not total
            var hallwayXlengthLeft = (Mathf.RoundToInt(gridX / 2)) - (int)(spawnRoom.GetComponent<Room>().sizeX / offset) -
                                ((int)(hallwayEndLeft.GetComponent<Room>().sizeX / offset));

            var hallwayXlengthRight = (Mathf.RoundToInt(gridX / 2)) - (int)(spawnRoom.GetComponent<Room>().sizeX / offset) -
                                ((int)(hallwayEndRight.GetComponent<Room>().sizeX / offset));

            Room latestHallway = null;
            var generateHallway = true;

            var currentHallwayXlength = hallwayXlengthLeft;
            var isLeft = true;

            while (generateHallway)
            {
                var weightTotal = hallways.Sum(h => h.GetComponent<Room>().GetWeight());

                var chosenHallway = RoomUtil.GetRandomRoom(hallways, weightTotal);

                var hallwayX = (int)chosenHallway.GetComponent<Room>().sizeX;
                var hallwayZ = (int)chosenHallway.GetComponent<Room>().sizeZ;

                currentHallwayXlength -= hallwayX / offset;

                float posX = 0;
                float posZ = _spawnRoom.transform.position.z;

                if (latestHallway is null)
                {
                    if (isLeft)
                    {
                        posX = Mathf.RoundToInt((_spawnRoom.transform.position.x - _spawnRoom.sizeX / 2) - (hallwayX / 2));
                    }
                    else
                    {
                        posX = Mathf.RoundToInt((_spawnRoom.transform.position.x + _spawnRoom.sizeX / 2) + (hallwayX / 2));
                    }
                }
                else
                {
                    if (isLeft)
                    {
                        posX = latestHallway.transform.position.x - Mathf.RoundToInt(latestHallway.sizeX / 2) -
                           Mathf.RoundToInt(hallwayX / 2);
                    }
                    else
                    {
                        posX = latestHallway.transform.position.x + Mathf.RoundToInt(latestHallway.sizeX / 2) +
                           Mathf.RoundToInt(hallwayX / 2);
                    }
                }

                if (currentHallwayXlength <= 0)
                {
                    if (isLeft)
                    {
                        isLeft = false;
                        latestHallway = null;
                        currentHallwayXlength = hallwayXlengthRight;

                        CreateHallwayEnd(posX, posZ, hallwayEndLeft);

                        continue;
                    }
                    else
                    {
                        generateHallway = false;

                        CreateHallwayEnd(posX, posZ, hallwayEndRight);

                        break;
                    }
                }

                latestHallway = CreateHallway(posX, posZ, chosenHallway);
                yield return null;
            }

            yield return StartCoroutine(SpawnKeyRoom());
        }

        private IEnumerator SpawnKeyRoom()
        {
            _roomGenerator.SpawnKeyRoom(mapHallways);
            yield return StartCoroutine(GenerateRooms());
        }

        private IEnumerator GenerateRooms()
        {
            
            StartCoroutine(_roomGenerator.BranchRoomGeneration(mapHallways, minWeightRoomsBranch, maxWeightRoomsBranch, startTime));
            while (_roomGenerator.coroutineRunning == true)
                yield return null;

            yield return StartCoroutine(ReplaceDoors());
        }

        private IEnumerator ReplaceDoors()
        {
            foreach (var room in mapRooms)
            {
                StageHelper.ReplaceAllDoors(room);
                yield return null;
            } 
            yield return StartCoroutine(StageNavMeshBaker());
        }

        private IEnumerator StageNavMeshBaker()
        {
            StageHelper.NavMeshBaker();
            yield return StartCoroutine(StageEnemyGeneration());
        }

        private IEnumerator StageEnemyGeneration()
        {
            foreach (var room in mapRooms.Where(room => room.GetComponent<EnemyGeneration>() is not null))
            {
                room.GetComponent<EnemyGeneration>().SpawnEnemy();
                yield return null;
            }  
            yield return StartCoroutine(StageChestGeneration());
        }

        private IEnumerator StageChestGeneration()
        {
            foreach (var room in mapRooms.Where(room => room.GetComponent<ChestSpawner>() is not null))
            {
                room.GetComponent<ChestSpawner>().SpawnChest();
                yield return null;
            }

            Navigation.instance.StageGenComplete = true;
            yield return null;
        }

        private GameObject SpawnRoom()
        {
            var spawnRoomX = (int)spawnRoom.GetComponent<Room>().sizeX;
            var spawnRoomZ = (int)spawnRoom.GetComponent<Room>().sizeZ;

            float posX = Mathf.RoundToInt(gridX * offset / 2);
            float posZ = Mathf.RoundToInt(gridZ * offset / 2);

            var room = Instantiate(spawnRoom, new Vector3(posX, 0, posZ), Quaternion.identity);
            var roomCells = SetRoomCells(room, posX, posZ);

            room.GetComponent<Room>().cells = roomCells;
            room.name = "Spawn Room";

            room.GetComponent<Room>().GetDoors().Where(d => d != room.GetComponent<SpawnRoom>().bossHallwayDoor).ToList().ForEach(d => d.SetActive(false));

            StageHelper.Instance.spawnRoom = room.GetComponent<SpawnRoom>();

            mapRooms.Add(room);

            return room;
        }

        private GameObject SpawnBossRoom(GameObject lastHallway)
        {
            var spawnBossRoomX = (int)bossRoom.GetComponent<Room>().sizeX;
            var spawnBossRoomZ = (int)bossRoom.GetComponent<Room>().sizeZ;

            float posX = gridX * offset / 2;
            var posZ = lastHallway.transform.position.z + Mathf.RoundToInt(lastHallway.GetComponent<Room>().sizeZ / 2)
                                                        + spawnBossRoomZ / 2;

            var room = Instantiate(bossRoom, new Vector3(posX, 0, posZ), Quaternion.identity);
            var roomCells = SetRoomCells(room, posX, posZ);

            room.GetComponent<Room>().cells = roomCells;
            room.name = "Boss Room";

            room.GetComponent<Room>().GetDoors()[0].SetActive(false);

            return room;
        }

        //Set the room cells that are occupied by it
        private List<Cell> SetRoomCells(GameObject room, float posX, float posZ)
        {
            //Create a new list to store the cells in
            var roomCells = new List<Cell>();

            var roomScript = room.GetComponent<Room>();

            //Get the room X and Z length
            var roomX = (int)roomScript.sizeX / offset;
            var roomZ = (int)roomScript.sizeZ / offset;

            var cellPosX = (int)(posX % offset == offset / 2 ? posX - offset / 2 : posX) / offset;
            var cellPosZ = (int)(posZ % offset == offset / 2 ? posZ - offset / 2 : posZ) / offset;

            // cellPosX = roomX % 2 != 0 ? cellPosX : cellPosX + 1;
            // cellPosZ = roomZ % 2 != 0 ? cellPosZ : cellPosZ + 1;

            //Get the position X and Z of the cell in the scene
            var startPosX = cellPosX - roomX / 2;
            var startPosZ = cellPosZ - roomZ / 2;

            //Get the starting width and height of the cells you need for the room
            var endPosX = cellPosX + roomX / 2;
            var endPosZ = cellPosZ + roomZ / 2;

            //Loop through cells based on start position and length of room
            for (var i = startPosX; i <= endPosX; i++)
                for (var j = startPosZ; j <= endPosZ; j++)
                {
                    var cell = cells.SingleOrDefault(c => c.x == i && c.z == j && !c.isOccupied);

                    if (cell is null)
                    {
                        //Debug.Log("No cell x" + i + " z" + j);
                    }

                    else
                    {
                        if (roomScript.GetComponent<StandardRoom>())
                            cell.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                        else if (roomScript.GetComponent<SpawnRoom>())
                            cell.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                        else
                            cell.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                        cell.isOccupied = true;

                        roomCells.Add(cell);
                    }
                }

            return roomCells;
        }

        private Room CreateHallway(float posX, float posZ, GameObject chosenHallway)
        {
            var _hallway = Instantiate(chosenHallway, new Vector3(posX, 0, posZ), Quaternion.identity)
                .GetComponent<Room>();

            var roomCells = SetRoomCells(chosenHallway, posX, posZ);

            _hallway.GetComponent<Room>().cells = roomCells;
            _hallway.GetComponent<Room>().SetDoorCells();

            mapHallways.Add(_hallway.gameObject);

            return _hallway;
        }

        private void CreateHallwayEnd(float posX, float posZ, GameObject hallwayEnd)
        {
            var _hallway = Instantiate(hallwayEnd, new Vector3(posX, 0, posZ), Quaternion.identity)
                .GetComponent<Room>();

            var roomCells = SetRoomCells(hallwayEnd, posX, posZ);

            _hallway.GetComponent<Room>().cells = roomCells;
            _hallway.GetComponent<Room>().SetDoorCells();

            mapHallways.Add(_hallway.gameObject);
        }

        //Spawn cell in the grid on the calculated location
        private void SpawnCell(int x, int z)
        {
            //Create the position of the cell to place in scene
            var cellPos = new Vector3(x * offset, -4f, z * offset);

            //Place the game object in the scene
            var newCell = Instantiate(gridCell, cellPos, Quaternion.identity);

            //Set the cell X and Z position of the grid
            newCell.GetComponent<Cell>().x = x;
            newCell.GetComponent<Cell>().z = z;

            //Set the cell in the Stage gameobject and rename the cell to its position in the grid
            newCell.transform.parent = _stageParent.transform;
            newCell.name = "Cell: X = " + x + ", Z = " + z;

            if (!debugMode) newCell.GetComponent<Renderer>().enabled = false;

            //Add the cell to the cells list
            cells.Add(newCell.GetComponent<Cell>());
        }

        private void InitializeRoomSizes()
        {
            var allRooms = new List<GameObject>
            {
                spawnRoom,
                bossHallway,
                bossRoom,
                hallwayEndLeft,
                hallwayEndRight
            };

            allRooms.AddRange(rooms);
            allRooms.AddRange(hallways);

            foreach (var room in allRooms)
            {
                var newSizes = RoomUtil.CalculateSizeBasedOnChildren(room);
                var roomComp = room.GetComponent<Room>();

                roomComp.sizeX = newSizes["x"];
                roomComp.sizeY = newSizes["y"];
                roomComp.sizeZ = newSizes["z"];
            }
        }


        public void AddRoomToMap(GameObject room)
        {
            mapRooms.Add(room);
        }
    }
}
