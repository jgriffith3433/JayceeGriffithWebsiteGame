using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTrigger : MonoBehaviour
{
    public BasicBehaviour GameCharacter;
    public PreviousTrigger Previous;

    private bool TriggerDisabled = false;

    public void Awake()
    {
        StartCoroutine(DisableTriggerTimed(1));
    }

    public IEnumerator DisableTriggerTimed(float time)
    {
        TriggerDisabled = true;
        yield return new WaitForSeconds(time);
        TriggerDisabled = false;
    }

    public void DisableTrigger()
    {
        TriggerDisabled = true;
    }

    public void EnableTrigger()
    {
        TriggerDisabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!TriggerDisabled)
        {
            NavigationController.Instance.GoNext();
        }
    }

    public void DisablePreviousAndTeleport(float waitTime = .05f)
    {
        StartCoroutine(Previous.DisableTriggerTimed(waitTime + 1));
        StartCoroutine(Teleport(.5f));
    }

    public IEnumerator Teleport(float waitTime = .05f)
    {
        yield return new WaitForSeconds(waitTime);
        GameCharacter.transform.position = Previous.transform.position + new Vector3(1, 0, 0);
        var rb = GameCharacter.GetRigidBody;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
        GameCharacter.AddExternalInput(new Vector2(0.5f, 0), 0.50f);
    }
}
