using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiftScript : MonoBehaviour
{
    #region variables
    //public variables
    public List<Transform> floors = new List<Transform>();
    public Transform openDoor;
    public Transform closeDoor;
    public GameObject door;
    public Transform raycastPoint;
    public List<Text> actualFloorText = new List<Text>();
    public Text floorRequestsText;
    public Animation doorAnimation;

    //sounds
    public AudioSource elevatorAudioSource;
    public AudioSource musicAudioSource;
    private AudioClip movingClip;
    private AudioClip startMovingClip;
    private AudioClip endMovingClip;
    private AudioClip doorClip;

    //photocell
    private Ray ray;
    private LayerMask mask = 1 << 8;
    private bool hitPlayer = false;

    //door too long on one floor
    private int actualFloor = 0;
    public bool doorIsOpen = true;
    private float timeDoorMustBeOpen = 6f;
    private float timeDoorIsOpen = 0f;

    //coroutines
    private Coroutine movingCoroutine;
    private bool moving = false;

    //character list so that the elevator can interact with more players or npc
    //one variable is enough for one player
    private List<CharacterController> insideMe = new List<CharacterController>();

    //for save more request when lift is moving
    private List<int> floorRequests = new List<int>();
    #endregion

    private void Start()
    {
        //If this is related to the Find methods then references to these clips should be public and added in the inspector
        movingClip = Resources.Load<AudioClip>("Sounds/move");
        startMovingClip = Resources.Load<AudioClip>("Sounds/start");
        endMovingClip = Resources.Load<AudioClip>("Sounds/end");
        doorClip = Resources.Load<AudioClip>("Sounds/door");
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
        insideMe.Add(other.GetComponent<CharacterController>());
        musicAudioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
        insideMe.Remove(other.GetComponent<CharacterController>());
        if (insideMe.Count == 0)
            musicAudioSource.Pause();
    }

    private void Update()
    {
        ray = new Ray(raycastPoint.position, raycastPoint.forward);
        hitPlayer = Physics.Raycast(ray, 3.5f, mask);

        if (!hitPlayer && doorIsOpen && timeDoorIsOpen < timeDoorMustBeOpen)
        {
            timeDoorIsOpen += Time.unscaledDeltaTime;
        }
        else
        {
            timeDoorIsOpen = 0f;
            if (!hitPlayer)
            {
                if (doorIsOpen)
                {
                    CloseTheDoor();
                }
            }
            else
            {
                if (doorAnimation.IsPlaying("doorClose"))
                {
                    doorAnimation.Stop();
                    if(movingCoroutine != null)
                    {
                        StopCoroutine(movingCoroutine);
                        movingCoroutine = null;
                        moving = false;
                    }
                    OpenHalfDoor();
                }
            }
        }

        if (floorRequests.Count > 0 && !moving && doorIsOpen && timeDoorIsOpen > timeDoorMustBeOpen / 2f)
        {
            timeDoorIsOpen = 0f;
            Move(floorRequests[0]);
        }
    }

    public void OpenHalfDoor()
    {
        elevatorAudioSource.clip = doorClip;
        elevatorAudioSource.loop = false;
        elevatorAudioSource.Play();

        doorAnimation.Play("doorHalfOpen");

        doorIsOpen = true;
    }

    public void CloseTheDoor()
    {
        if (doorIsOpen)
        {
            elevatorAudioSource.clip = doorClip;
            elevatorAudioSource.loop = false;
            elevatorAudioSource.Play();

            doorAnimation.Play("doorClose");

            doorIsOpen = false;
        }
    }

    public void OpenTheDoor()
    {
        if (!doorIsOpen)
        {
            elevatorAudioSource.clip = doorClip;
            elevatorAudioSource.loop = false;
            elevatorAudioSource.Play();

            doorAnimation.Play("doorOpen");

            doorIsOpen = true;
        }
    }

    public void Move(int floor)
    {
        if (!moving && floor == actualFloor && !doorIsOpen)
        {
            OpenTheDoor();
        }
        else if (floor != actualFloor)
        {
            if (!floorRequests.Contains(floor)) {
                    floorRequests.Add(floor);
                    floorRequestsText.text += (" " + floor);
            }
            if (!moving)
            {
                if (movingCoroutine != null)
                    StopCoroutine(movingCoroutine);
                movingCoroutine = StartCoroutine(MoveNow(floor));
            }
        }
    }

    IEnumerator MoveNow(int floor)
    {
        CloseTheDoor();

        yield return new WaitForSecondsRealtime(1f);

        moving = true;

        elevatorAudioSource.clip = startMovingClip;
        elevatorAudioSource.loop = false;
        elevatorAudioSource.Play();

        yield return new WaitForSecondsRealtime(2.185f);

        elevatorAudioSource.clip = movingClip;
        elevatorAudioSource.loop = true;
        elevatorAudioSource.Play();

        foreach (CharacterController cc in insideMe)
            cc.enabled = false;

        //Lift moving
        float i = 0;
        while (i < 1f)
        {
            i += 0.005f / (float) Mathf.Abs(actualFloor - floor);
            transform.position = new Vector3(
                Mathf.Lerp(floors[actualFloor].position.x, floors[floor].position.x, i),
                Mathf.Lerp(floors[actualFloor].position.y, floors[floor].position.y, i),
                Mathf.Lerp(floors[actualFloor].position.z, floors[floor].position.z, i));
            yield return new WaitForSecondsRealtime(0.01f);
        }
        actualFloor = floor;

        if (floorRequests.Remove(floor))
            floorRequestsText.text = floorRequestsText.text.Substring(2);

        foreach (Text txt in actualFloorText)
            if(txt)
                txt.text = actualFloor.ToString();

        foreach (CharacterController cc in insideMe)
            cc.enabled = true;

        elevatorAudioSource.clip = endMovingClip;
        elevatorAudioSource.loop = false;
        elevatorAudioSource.Play();

        yield return new WaitForSecondsRealtime(1.511f);

        moving = false;

        OpenTheDoor();
    }
}
