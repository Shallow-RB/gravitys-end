using Core.StageGeneration.Stage;
using UnityEngine;

namespace Core.StageGeneration.Rooms
{
    public class Door : MonoBehaviour
    {
        public int roomPosXOffset;
        public int roomPosZOffset;

        public bool hasNeighbour;

        public Cell cell;

        public StageHelper.RoomDirections direction;

        public StageHelper.RoomDirections GetDirection()
        {
            return direction;
        }
    }
}
