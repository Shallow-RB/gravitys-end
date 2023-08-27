using System;
using System.Collections.Generic;
using System.Linq;
using Core.StageGeneration.Rooms.RoomTypes;
using Core.StageGeneration.Rooms.Util;
using Core.StageGeneration.Stage;
using UnityEngine;

namespace Core.StageGeneration.Rooms
{
    public abstract class Room : MonoBehaviour
    {
        public float sizeX;
        public float sizeY;
        public float sizeZ;

        public List<GameObject> doors;

        public int weight;

        public GameObject doorReplacement;

        [HideInInspector]
        public List<Cell> cells;

        public int GetWeight()
        {
            return weight;
        }

        public List<GameObject> GetDoors()
        {
            return doors;
        }

        public GameObject GetDoorReplacement()
        {
            return doorReplacement;
        }

        public GameObject PlaceRoom()
        {
            return Instantiate(gameObject, new Vector3(0, 20, 0), Quaternion.identity);
        }

        public GameObject SetRoomData(int posX, int posZ, Quaternion rotation, StageHelper.RoomDirections direction, GameObject spawnDoor)
        {
            transform.position = new Vector3(posX, 0, posZ);
            transform.Rotate(new Vector3(0, rotation.y, 0));

            spawnDoor.SetActive(false);

            doors.SingleOrDefault(d =>
                    d.GetComponent<Door>().GetDirection() == StageHelper.GetOppositeDirection(direction))
                ?.SetActive(false);
            
            StageGenerator.instance.AddRoomToMap(gameObject);

            return gameObject;
        }

        public Quaternion RotateRoom(StageHelper.RoomDirections placementDirection)
        {
            var rotation = NewRotationData(placementDirection);

            return rotation;
        }

        public Quaternion NewRotationData(StageHelper.RoomDirections placementDirection)
        {
            var rotation = new Quaternion(0, 0, 0, 0);

            var storedSizeX = sizeX;
            var storedSizeZ = sizeZ;

            var roomDoors = doors.Select(d => d.GetComponent<Door>()).ToList();
            int iterateCount = 0;

            while (!roomDoors.Any(d => d.direction == StageHelper.GetOppositeDirection(placementDirection)))
            {
                if (iterateCount > 2) break;

                RoomUtil.RotateDoors(this);

                rotation.y += 90;               

                iterateCount++;
            }

            if (rotation.y > 270)
            {
                rotation.y = 0;
            }

            if (rotation.y == 90 ||rotation.y == 270)
            {
                sizeX = storedSizeZ;
                sizeZ = storedSizeX;
            }

            RoomUtil.AssignDoorsPos(this);

            return rotation;
        }

        public Vector3 PlacementPos(StageHelper.RoomDirections roomDirection, Cell doorCell)
        {
            var pos = new Vector3();

            float roomX = 0;
            float roomZ = 0;

            var resizeX = Mathf.RoundToInt(sizeX / 2);
            var resizeZ = Mathf.RoundToInt(sizeZ / 2);

            var position = doorCell.transform.position;

            var doorCellX = position.x;
            var doorCellZ = position.z;
            var offset = StageHelper.GetOffset();
            var divOffset = offset / 2;

            switch (roomDirection)
            {
                case StageHelper.RoomDirections.TOP:
                    roomX = doorCellX;
                    roomZ = doorCellZ +
                            (resizeZ - divOffset) + offset;
                    break;
                case StageHelper.RoomDirections.RIGHT:
                    roomX = doorCellX +
                            (resizeX - divOffset) + offset;
                    roomZ = doorCellZ;
                    break;
                case StageHelper.RoomDirections.BOTTOM:
                    roomX = doorCellX;
                    roomZ = doorCellZ -
                            (resizeZ - divOffset + offset);
                    break;
                case StageHelper.RoomDirections.LEFT:
                    roomX = doorCellX -
                            (resizeX - divOffset + offset);
                    roomZ = doorCellZ;
                    break;
                case StageHelper.RoomDirections.UNDEFINED:
                    Debug.Log("Is Undefined");
                    roomX = 0;
                    roomZ = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(roomDirection), roomDirection, null);
            }

            pos.x = roomX;
            pos.z = roomZ;

            return pos;
        }

        public void SetDoorCells()
        {
            var room = gameObject.GetComponent<Room>();

            foreach (var door in room.doors)
            {
                var doorCellX = room.cells.Select(mh => mh.x).Distinct()
                    .ToArray()[door.GetComponent<Door>().roomPosXOffset];
                var doorCellZ = room.cells.Select(mh => mh.z).Distinct()
                    .ToArray()[door.GetComponent<Door>().roomPosZOffset];

                var doorCellRoom = StageHelper.GetCells()
                    .SingleOrDefault(c => c.x == doorCellX && c.z == doorCellZ);

                if (doorCellRoom is null) continue;
                doorCellRoom.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

                door.GetComponent<Door>().cell = doorCellRoom;
            }
        }

        public bool CanPlace(int posX, int posZ)
        {
            var canPlace = true;
            var offset = StageHelper.GetOffset();

            //Get the room X and Z length
            var roomX = (int)sizeX / offset;
            var roomZ = (int)sizeZ / offset;

            var cellPosX = (posX % offset == offset / 2 ? posX - offset / 2 : posX) / offset;
            var cellPosZ = (posZ % offset == offset / 2 ? posZ - offset / 2 : posZ) / offset;

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
                    var cell = StageHelper.GetCells()
                        .SingleOrDefault(c => c.x == i && c.z == j && c.isOccupied == false);

                    if (cell is null) canPlace = false;
                }

            return canPlace;
        }

        //Set the room cells that are occupied by it
        public void SetRoomCells(int posX, int posZ)
        {
            var offset = StageHelper.GetOffset();

            //Create a new list to store the cells in
            var roomCells = new List<Cell>();

            var room = gameObject.GetComponent<Room>();

            //Get the room X and Z length
            var roomX = (int)sizeX / offset;
            var roomZ = (int)sizeZ / offset;

            var cellPosX = (posX % offset == offset / 2 ? posX - offset / 2 : posX) / offset;
            var cellPosZ = (posZ % offset == offset / 2 ? posZ - offset / 2 : posZ) / offset;

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
                    var cell = StageHelper.GetCells()
                        .SingleOrDefault(c => c.x == i && c.z == j && c.isOccupied == false);

                    if (cell is null)
                        Debug.Log("No cell x" + i + " z" + j);
                    else
                    {
                        cell.gameObject.GetComponent<MeshRenderer>().material.color =
                            room.GetComponent<StandardRoom>() ? Color.cyan : Color.red;
                        cell.isOccupied = true;

                        roomCells.Add(cell);
                    }
                }

            cells = roomCells;
        }

        public void SelfRemove()
        {
            Destroy(gameObject);
        }
    }
}
