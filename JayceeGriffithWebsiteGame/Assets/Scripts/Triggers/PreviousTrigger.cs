using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousTrigger : MonoBehaviour
{
    public GameObject GameCharacter;
    public NextTrigger Next;

    private bool TriggerDisabled = false;
    public IEnumerator DisableTrigger(float time)
    {
        TriggerDisabled = true;
        yield return new WaitForSeconds(time);
        TriggerDisabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!TriggerDisabled)
        {
            NavigationController.Instance.GoPrevious();
            StartCoroutine(Next.DisableTrigger(1));
            StartCoroutine(Teleport());
        }
    }

    public IEnumerator Teleport()
    {
        yield return new WaitForSeconds(.05f);
        GameCharacter.transform.position = Next.transform.position;
    }
}
