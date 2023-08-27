using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.StageGeneration.Rooms.RoomTypes;
using Core.StageGeneration.Rooms.Util;
using Core.StageGeneration.Stage;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.StageGeneration.Rooms
{
    public class RoomGenerator : MonoBehaviour
    {
        public static RoomGenerator instance;

        private GameObject _currentRoom;
        private GameObject _previousRoom;

        private int hallwayDoorCount;
        private int startDoorsLeftCount;
        private int startDoorsRightCount;

        private bool initialSpawned = false;

        private bool placeRoomsCoroutineActive;

        //Temp please delete from here and BranchRoomGeneration
        private float startTime;

        public bool coroutineRunning { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public IEnumerator BranchRoomGeneration(List<GameObject> mapHallways, int minWeightRoomsBranch, int maxWeightRoomsBranch, float _startTime)
        {
            startTime = _startTime;

            coroutineRunning = true;

            var weightTotal = StageHelper.GetRooms().Sum(h => h.GetComponent<Room>().GetWeight());

            foreach (var hallway in mapHallways)
            {
                // hallway.GetComponent<Room>().SetDoorCells();

                foreach (var door in hallway.GetComponent<Room>().GetDoors())
                {
                    var branchLength = Random.Range(minWeightRoomsBranch, maxWeightRoomsBranch + 1);

                    StartCoroutine(PlaceRooms(door, branchLength, weightTotal));
                    while (placeRoomsCoroutineActive)
                        yield return null;
                }

                StageHelper.ReplaceAllDoors(hallway.gameObject);

                _currentRoom = null;
                _previousRoom = null;

                yield return null;
            }
            coroutineRunning = false;
            yield return null;
        }

        private IEnumerator PlaceRooms(GameObject spawnDoor, int branchLength, int weightTotal)
        {
            initialSpawned = false;
            placeRoomsCoroutineActive = true;

            // Loop through a single branch
            for (var i = 0; i < branchLength; i++)
            {
                var roomRotation = new Quaternion(0, 0, 0, 0);

                // Get random room based on the total weight
                _currentRoom = RoomUtil.GetRandomRoom(StageHelper.GetRooms(), weightTotal).GetComponent<Room>().PlaceRoom();
                Room currentRoom = _currentRoom.GetComponent<Room>();

                Cell doorCell = null;
                var currentSpawnDoor = spawnDoor;
                StageHelper.RoomDirections placementSide;

                // If the first room of the branch has been spawned
                if (initialSpawned)
                {
                    // Determine placement
                    placementSide = DeterminePlacementSide();

                    // If the placementside is undefined, skip this iteration
                    if (placementSide == StageHelper.RoomDirections.UNDEFINED)
                    {
                        Destroy(_currentRoom);
                        continue;
                    }

                    // Set currentSpawnDoor the door of the previous room with direction of placementside
                    currentSpawnDoor = _previousRoom.GetComponent<Room>()
                        .GetDoors().SingleOrDefault(d => d.GetComponent<Door>().GetDirection() == placementSide);

                    if (currentRoom.GetDoors().Count < 4 &&
                        currentRoom.GetDoors().Where(d => d.GetComponent<Door>().direction != StageHelper.GetOppositeDirection(placementSide)).Any())
                    {
                        roomRotation = currentRoom.RotateRoom(placementSide);
                    }

                    // If the currentSpawnDoor exist, set doorCell of the doors cell
                    if (currentSpawnDoor is not null) doorCell = currentSpawnDoor.GetComponent<Door>().cell;
                }
                else
                {
                    // Set placementside direction of the hallway door to spawn initial room
                    placementSide = currentSpawnDoor.GetComponent<Door>().GetDirection();

                    // Set doorcell of the hallway door
                    doorCell = currentSpawnDoor.GetComponent<Door>().cell;

                    if (currentRoom.GetDoors().Count < 4 &&
                        currentRoom.GetDoors().Where(d => d.GetComponent<Door>().direction != StageHelper.GetOppositeDirection(placementSide)).Any())
                    {
                        roomRotation = currentRoom.RotateRoom(placementSide);
                    }

                    // Set the initial position of the room where it should recide
                    var initialPos = currentRoom.PlacementPos(placementSide, doorCell);

                    // Set the canPlace that saves a boolean to know that room can be placed or not
                    var canPlace = currentRoom.CanPlace((int)initialPos.x, (int)initialPos.z);

                    // If not can be placed, go the next iteration
                    if (!canPlace)
                    {
                        Destroy(_currentRoom);
                        continue;
                    }
                }

                var pos = currentRoom.PlacementPos(placementSide, doorCell);

                currentRoom.SetRoomCells((int)pos.x, (int)pos.z);
                currentRoom.SetDoorCells();

                _previousRoom = currentRoom.SetRoomData((int)pos.x, (int)pos.z, roomRotation, placementSide, currentSpawnDoor);

                initialSpawned = true;
                yield return new WaitForSeconds(0.1f);
            }
            placeRoomsCoroutineActive = false;
            yield return null;
        }

        private void CountHallwayDoors(List<GameObject> mapHallways)
        {
            hallwayDoorCount = (mapHallways.Count) * 6;
            startDoorsLeftCount = hallwayDoorCount / 2 - 6;
            startDoorsRightCount = hallwayDoorCount - 6;
        }

        // Spawn key room
        public void SpawnKeyRoom(List<GameObject> mapHallways)
        {
            CountHallwayDoors(mapHallways);

            var room = StageHelper.GetKeyRoom().GetComponent<Room>().PlaceRoom();
            var keyRoom = room.GetComponent<Room>();

            Cell doorCell = null;
            StageHelper.RoomDirections placementSide;
            Vector3 roomPosition;

            var canPlace = false;
            var roomRotation = new Quaternion(0, 0, 0, 0);
            var hallwayDoorId = 0;

            bool isLeft = (Random.Range(0, 2) == 0);

            if (isLeft)
                hallwayDoorId = Random.Range(startDoorsLeftCount, startDoorsLeftCount + 3);
            else
                hallwayDoorId = Random.Range(startDoorsRightCount, startDoorsRightCount + 3);

            var hallwaysDoors = mapHallways.SelectMany(hallway => hallway.GetComponent<Room>().GetDoors()).ToList();

            var spawnDoor = hallwaysDoors[hallwayDoorId].GetComponent<Door>();

            doorCell = spawnDoor.cell;
            placementSide = spawnDoor.GetDirection();

            if (keyRoom.GetDoors().Where(d => d.GetComponent<Door>().direction != StageHelper.GetOppositeDirection(placementSide)).Any())
            {
                roomRotation = keyRoom.RotateRoom(placementSide);
            }

            roomPosition = keyRoom.PlacementPos(placementSide, doorCell);

            canPlace = keyRoom.CanPlace((int)roomPosition.x, (int)roomPosition.z);

            if (canPlace)
            {
                keyRoom.SetRoomCells((int)roomPosition.x, (int)roomPosition.z);
                keyRoom.SetDoorCells();

                _previousRoom = keyRoom.SetRoomData((int)roomPosition.x, (int)roomPosition.z, roomRotation, placementSide, spawnDoor.gameObject);
            }
        }

        // Determine placement side of room
        private StageHelper.RoomDirections DeterminePlacementSide()
        {
            var openDirections = _previousRoom.GetComponent<Room>().GetDoors()
                .Where(d => d.GetComponent<Door>().GetDirection() != StageHelper.RoomDirections.UNDEFINED)
                .Select(d => d.GetComponent<Door>().GetDirection()).ToList();

            var doorDirection = StageHelper.RandomDirectionFromRoom(_previousRoom);

            openDirections.Remove(doorDirection);

            var canPlace = false;
            GameObject previousDoor = null;

            if (doorDirection != StageHelper.RoomDirections.UNDEFINED)
            {
                previousDoor = _previousRoom.GetComponent<Room>().GetDoors().SingleOrDefault(d =>
                    d.GetComponent<Door>().hasNeighbour == false &&
                    d.GetComponent<Door>().GetDirection() == doorDirection);

                if (previousDoor != null)
                {
                    var pos = _currentRoom.GetComponent<Room>()
                    .PlacementPos(doorDirection, previousDoor.GetComponent<Door>().cell);

                    canPlace = _currentRoom.GetComponent<Room>().CanPlace((int)pos.x, (int)pos.z);
                }
            }

            if (!canPlace)
            {
                var iteration = 0;
                var previousRoomMaxDoors = _previousRoom.GetComponent<Room>().GetDoors().Count;

                while (!canPlace && iteration < previousRoomMaxDoors)
                {
                    doorDirection = StageHelper.RandomDirection(openDirections);

                    openDirections.Remove(doorDirection);

                    previousDoor = _previousRoom.GetComponent<Room>().GetDoors().SingleOrDefault(d =>
                        d.GetComponent<Door>().hasNeighbour == false &&
                        d.GetComponent<Door>().GetDirection() == doorDirection);

                    if (previousDoor != null)
                    {
                        var pos = _currentRoom.GetComponent<Room>()
                        .PlacementPos(doorDirection, previousDoor.GetComponent<Door>().cell);

                        canPlace = _currentRoom.GetComponent<Room>().CanPlace((int)pos.x, (int)pos.z);
                    }

                    iteration++;
                }
            }

            if (!canPlace)
                doorDirection = StageHelper.RoomDirections.UNDEFINED;
            else
                previousDoor.GetComponent<Door>().hasNeighbour = true;

            return doorDirection;
        }
    }
}
