using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MouseOverUILayerObject
{
    public static bool IsPointerOverUIObject()
    {
        //if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //{
        //}
        //else if (!EventSystem.current.IsPointerOverGameObject())
        //{
        //    Debug.Log("clicked nothing");
        //}
        //else
        //{
        //    Debug.Log("clicked game object");
        //}
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var eventSystemResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, eventSystemResults);

        for (int i = 0; i < eventSystemResults.Count; i++)
        {
            if (eventSystemResults[i].gameObject.layer == 5) //5 = UI layer
            {
                return true;
            }
            if (eventSystemResults[i].gameObject.layer == 8) //8 = Grabber layer
            {
                return true;
            }
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var physicsResults = Physics.RaycastAll(ray);
        for (int i = 0; i < physicsResults.Length; i++)
        {
            if (physicsResults[i].transform.gameObject.layer == 5) //5 = UI layer
            {
                return true;
            }
            if (physicsResults[i].transform.gameObject.layer == 8) //8 = Grabber layer
            {
                return true;
            }
        }

        return false;
    }
}