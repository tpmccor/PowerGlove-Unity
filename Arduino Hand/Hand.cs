/* Filename:    Hand.cs
 * Written by:  Travis McCormick
 * Course:      ECE 4960 Fall 2020
 */

using UnityEngine;

public class Hand
{
    //Hand hierarchy has to be in this order: Index, Middle, Ring, Pinky, Thumb
    public const int INDEX = 0;
    public const int MIDDLE = 1;
    public const int RING = 2;
    public const int PINKY = 3;
    public const int THUMB = 4;

    public const float delta = 3f;

    private readonly Transform wrist;
    public Transform Wrist => this.wrist;
    public Finger[] fingers;
    public Thumb thumb;

    private float roll;
    private float pitch;

    public Hand()
    {
        this.wrist = null;
        this.fingers = new Finger[4];
        this.thumb = null;

        this.roll = 0;
        this.pitch = 0;
    }

    public Hand(Transform wristNode)
    {
        this.wrist = null;
        this.fingers = new Finger[4];
        this.thumb = null;

        this.roll = 0;
        this.pitch = 0;

        if (wristNode != null)
        {
            //Starting from wrist, recursively traverse through the hand to find fingertips
            this.wrist = wristNode;
            InitFingers(this.wrist, 0);


            //Initilize hand at the origin
            this.wrist.localEulerAngles = Vector3.zero;
            this.wrist.position = Vector3.zero;
        }
    }

    private int InitFingers(Transform node, int fingerCount)
    {
        //Find leaf nodes in hand structure and initilize fingers
        //Assumes the hierarchy is in this order: Index, Middle, Ring, Pinky, Thumb
        if (node.childCount == 0 || node.GetChild(0) == null)
        {
            if(fingerCount < THUMB)
            {
                this.fingers[fingerCount] = new Finger(node, this, fingerCount);
            }
            else if(fingerCount == THUMB)
            {
                this.thumb = new Thumb(node, this);
            }

            return fingerCount + 1;
        }

        for (int i = 0; i < node.childCount; i++)
        {
            fingerCount = InitFingers(node.GetChild(i), fingerCount);
        }

        //Returns how many leaf nodes are found in hierarchy
        return fingerCount;
    }

    public void RotateHand(float roll, float pitch, float yaw)
    {
        //Rotate the hand at the wrist
        if (this.wrist != null)
        {
            
            if (Mathf.Abs(this.roll - roll) > delta)
            {
                roll = (roll + this.roll) / 2f; //Average new and old value for smoothing
                this.wrist.rotation *= Quaternion.Euler(this.roll - roll, 0, 0);
                this.roll = roll;
            }

            if (Mathf.Abs(this.pitch - pitch) > delta)
            {
                pitch = (pitch + this.pitch) / 2f; //Average new and old value for smoothing
                this.wrist.rotation *= Quaternion.Euler(0, 0, this.pitch - pitch);
                this.pitch = pitch;
            }
        }
    }

    public void spreadFingers(float indexLoc, float ringLoc, float pinkyLoc, float thumbLoc)
    {
        //Call finger spreading here because the hand object has access to all the fingers

        if (this.wrist != null) {
            Finger index = this.fingers[INDEX];
            Finger middle = this.fingers[MIDDLE];
            Finger ring = this.fingers[RING];
            Finger pinky = this.fingers[PINKY];

            middle.SpreadFinger(index, indexLoc);
            middle.SpreadFinger(ring, ringLoc);
            ring.SpreadFinger(pinky, -pinkyLoc);                     
        }
    }

    public override string ToString()
    {
        string handData = "Index: " + this.fingers[INDEX].ToString();
        handData += "Middle: " + this.fingers[MIDDLE].ToString();
        handData += "Ring: " + this.fingers[RING].ToString();
        handData += "Pinky: " + this.fingers[PINKY].ToString();
        handData += "Thumb: " + this.thumb.ToString();
        handData += "Hand Pitch: " + this.pitch + "\tHand Roll: " + this.roll+ "\n";
        return handData;
    }
}

