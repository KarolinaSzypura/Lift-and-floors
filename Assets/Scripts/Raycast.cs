using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private ButtonScript hittedObject;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 4f))
        {
            if (hittedObject)
                hittedObject.Highlight(false);
            hittedObject = hit.transform.GetComponent<ButtonScript>();
            if (hittedObject)
                hittedObject.Highlight(true);

            if (Input.GetMouseButtonDown(0))
            {
                if(hittedObject)
                    hittedObject.ButtonPressed();
            }
        }
        else
        {
            if (hittedObject)
                hittedObject.Highlight(false);
            hittedObject = null;
        }
    }
}
