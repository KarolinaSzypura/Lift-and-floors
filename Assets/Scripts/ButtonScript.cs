using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public LiftScript lift;
    public int floor;
    public Text myNumber;

    public void ButtonPressed()
    {
        lift.Move(floor);
    }
    public void Highlight(bool b)
    {
        if (myNumber)
        {
            if (b)
                myNumber.color = Color.white;
            else
                myNumber.color = Color.black;
        }
    }
}
