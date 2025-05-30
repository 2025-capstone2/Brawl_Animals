using Fusion;
using UnityEngine;
using System.Collections;

public class WaterSprayController : NetworkBehaviour
{
    public GameObject owner;
    public Transform followTarget;

    public override void FixedUpdateNetwork()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position;
            transform.rotation = followTarget.rotation;
        }
    }

    public override void Spawned()
    {
        StartCoroutine(DestroyAfterSeconds());
    }

    private IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(3f);
        Runner.Despawn(Object);
    }
}