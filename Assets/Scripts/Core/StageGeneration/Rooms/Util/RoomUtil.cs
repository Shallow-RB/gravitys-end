using System.Collections.Generic;
using System.Linq;
using Core.StageGeneration.Stage;
using UnityEngine;

namespace Core.StageGeneration.Rooms.Util
{
    public static class RoomUtil
    {
        // Rotate the doors clockwise of the room
        public static void RotateDoors(Room room)
        {
            var doors = room.GetDoors().Select(d => d.GetComponent<Door>()).ToList();

            foreach (Door door in doors)
            {
                door.direction = RotateClockwise(door.direction);
            }
        }

        public static void AssignDoorsPos(Room room)
        {
            var doors = room.GetDoors().Select(d => d.GetComponent<Door>()).ToList();

            foreach (Door door in doors)
            {
                ReassignDoorPos(door, room.sizeX, room.sizeZ);
            }
        }

        private static void ReassignDoorPos(Door door, float sizeX, float sizeZ)
        {
            switch (door.direction)
            {
                case StageHelper.RoomDirections.TOP:
                    door.roomPosXOffset = (int)(sizeX / 2) / 5;
                    door.roomPosZOffset = (int)(sizeZ / 5) - 1;
                    break;
                case StageHelper.RoomDirections.RIGHT:
                    door.roomPosXOffset = (int)(sizeX / 5) - 1;
                    door.roomPosZOffset = (int)(sizeZ / 2) / 5;
                    break;
                case StageHelper.RoomDirections.BOTTOM:
                    door.roomPosXOffset = (int)(sizeX / 2) / 5;
                    door.roomPosZOffset = 0;
                    break;
                case StageHelper.RoomDirections.LEFT:
                    door.roomPosXOffset = 0;
                    door.roomPosZOffset = (int)(sizeZ / 2) / 5;
                    break;
            }
        }

        private static StageHelper.RoomDirections RotateClockwise(StageHelper.RoomDirections direction)
        {
            switch (direction)
            {
                case StageHelper.RoomDirections.TOP:
                    return StageHelper.RoomDirections.RIGHT;
                case StageHelper.RoomDirections.RIGHT:
                    return StageHelper.RoomDirections.BOTTOM;
                case StageHelper.RoomDirections.BOTTOM:
                    return StageHelper.RoomDirections.LEFT;
                case StageHelper.RoomDirections.LEFT:
                    return StageHelper.RoomDirections.TOP;
                default:
                    return direction;
            }
        }

        public static Dictionary<string, float> CalculateSizeBasedOnChildren(GameObject parent)
        {
            var sizes = new Dictionary<string, float>();

            float x = 0;
            float y = 0;
            float z = 0;

            float floorY = 0;

            var childrenFloor = parent.GetComponentsInChildren<Transform>().Where(g => g.CompareTag("Floor")).ToArray();
            var childrenWalls = parent.GetComponentsInChildren<Transform>().Where(g => g.CompareTag("Wall")).ToArray();

            x = childrenFloor.Max(g => g.transform.localScale.x);
            z = childrenFloor.Max(g => g.transform.localScale.z);
            floorY = childrenFloor.Max(g => g.transform.localScale.y);

            foreach (Transform t in parent.transform)
            {
                float tempY = 0;

                if (t.CompareTag("Wall")) tempY = t.localScale.y;

                if (y < tempY) y = tempY;
            }

            sizes.Add("x", x);
            sizes.Add("y", y += floorY);
            sizes.Add("z", z);

            return sizes;
        }

        public static GameObject GetRandomRoom(List<GameObject> rooms, int totalWeight)
        {
            var randomWeightChance = Random.Range(0, totalWeight + 1);

            foreach (var r in rooms)
            {
                var hWeight = r.GetComponent<Room>().GetWeight();

                randomWeightChance -= hWeight;

                if (randomWeightChance <= 0) return r;
            }

            //Should never reach that point
            return null;
        }
    }
}
