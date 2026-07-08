using UnityEngine;

namespace FSCORD.Input.InputSystem
{
    /// <summary>Projects a screen point onto the battlefield ground plane.</summary>
    public static class ScreenToWorld
    {
        public static bool ToGround(Camera cam, Vector2 screen, float groundY, out Vector3 world)
        {
            world = default;
            if (cam == null) return false;
            var ray = cam.ScreenPointToRay(screen);
            var plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
            if (plane.Raycast(ray, out float enter)) { world = ray.GetPoint(enter); return true; }
            return false;
        }
    }
}
