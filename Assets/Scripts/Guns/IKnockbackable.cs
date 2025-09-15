using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public interface IKnockbackable
{
    public float StillThreshold { get; set; }
    void GetKnockedBack(Vector3 force, float maxMoveTime);
}
