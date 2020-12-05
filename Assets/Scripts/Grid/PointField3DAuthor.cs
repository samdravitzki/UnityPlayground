using System.Linq;
using UnityEngine;

namespace Grid
{    
    // No longer used for anything (DEPRECATED)
    public class PointField3DAuthor : MonoBehaviour
    {
        public PointField3D pointField;
        public Vector3Int size;
        public float cellSize = 1f;
        public bool drawGizmos;

        public void Awake()
        {
            pointField = new PointField3D(
                size.x, size.y, size.z,
                (x, y, z) => (x % 2 == 0 || z % 2 == 1));
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos || pointField == null)
            {
                return;
            }
            
            
            foreach (var x in Enumerable.Range(0, pointField.Width))
            {
                foreach (var y in Enumerable.Range(0, pointField.Height))
                {
                    foreach (var z in Enumerable.Range(0, pointField.Depth))
                    {
                        Vector3 gizmoPosition = new Vector3(x * cellSize, y * cellSize, z * cellSize);
                        Gizmos.color = pointField.GetCell(x, y, z) == 0 ? Color.black : Color.white;
                        Gizmos.DrawSphere(gizmoPosition, 0.1f);
                    }
                }
            }
        }
    }
}