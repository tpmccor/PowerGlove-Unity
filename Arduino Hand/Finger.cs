/* Filename:    Finger.cs
 * Written by:  Travis McCormick
 * Course:      ECE 4960 Fall 2020
 */

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Finger
{
    public const int IP = 0;
    public const int DIP = 0;
    public const int PIP = 1;
    public const int MCP = 2;
    public const int boneChainLength = 3;

    public const float delta = 2f;

    private float ipAngle;
    private float mcpAngle;
    private float spreadAngle;
    private readonly int fingerType;
    private readonly Transform[] joints;
    private readonly Hand hand;

    public Finger()
    {
        this.ipAngle = 0;
        this.mcpAngle = 0;
        this.spreadAngle = 0;
        this.fingerType = 0;
        this.joints = null;
        this.hand = null;
    }

    public Finger(Transform fingerTip, Hand handRef, int fingerNum)
    {
        this.ipAngle = 0;
        this.mcpAngle = 0;
        this.spreadAngle = 0;
        this.fingerType = fingerNum;
        this.joints = null;
        this.hand = handRef;

        if (fingerTip != null) { 

            //Initialize joint transform references
            this.joints = new Transform[boneChainLength];
            Transform currentJoint = fingerTip.parent;

            for (int i = 0; i < boneChainLength && currentJoint != null; i++)
            {
                this.joints[i] = currentJoint.transform;
                currentJoint = currentJoint.parent;
            }

            //Rotate pinky, ring, and index finger inline with the palm so they bend straight (for aesthetics)
            float fingerRotation = 0f;
            if (this.fingerType == Hand.INDEX)
            {
                fingerRotation = -10f;
            }
            else if (this.fingerType == Hand.PINKY || this.fingerType == Hand.RING)
            {
                fingerRotation = 5f;
            }

            //Initialize resting position
            this.joints[DIP].transform.localEulerAngles = new Vector3(0, 0, -10);
            this.joints[PIP].transform.localEulerAngles = new Vector3(0, 0, -10);
            this.joints[MCP].transform.localEulerAngles = new Vector3(fingerRotation, 0, -5);
        }
    }

    public void BendJoint(int jointNum, float angle)
    {
        if (this.joints != null)
        {
            if (jointNum == MCP)
            {
                //Rotate MCP around z-axis referenced from the wrist
                //Using the z-axis in its local space has unintended effects due to finger rotation and spread

                if (Mathf.Abs(this.mcpAngle - angle) > delta)
                {
                    angle = (this.mcpAngle + angle) / 2f;
                    this.joints[MCP].transform.Rotate(this.hand.Wrist.forward, this.mcpAngle - angle, Space.World);
                    this.mcpAngle = angle;
                }
            }
            else
            {
                //Rotate both IP joints around their local space z-axis
                if (Mathf.Abs(this.ipAngle - angle) > delta)
                {
                    angle = (this.ipAngle + angle) / 2f;
                    this.joints[DIP].transform.Rotate(Vector3.forward, this.ipAngle - angle, Space.Self);
                    this.joints[PIP].transform.Rotate(Vector3.forward, this.ipAngle - angle, Space.Self);
                    this.ipAngle = angle;
                }
            }
        }
    }

    public void SpreadFinger(float angle)
    {
        if(this.joints != null)
        {
            //Rotate MCP joint around local space y-axis

            if (Mathf.Abs(this.spreadAngle - angle) > delta)
            {
                angle = (this.spreadAngle + angle) / 2f;
                this.joints[MCP].transform.Rotate(Vector3.up, this.spreadAngle - angle, Space.Self);
                this.spreadAngle = angle;
            }
        }
    }

    public void SpreadFinger(Finger otherFinger, float angle)
    {
        //The base object is the finger that the other finger will be referenced from

        if (this.joints != null)
        {
            //If either finger is bent, set both spread angles to zero
            if (this.IsBent() || otherFinger.IsBent())
            {
                this.SpreadFinger(0);
                otherFinger.SpreadFinger(0);
            }
            else
            {
                if (otherFinger.fingerType == Hand.RING)
                {
                    //The ring and middle finger move from each other
                    this.SpreadFinger(angle / 2);
                    otherFinger.SpreadFinger(-angle / 2);
                }
                else
                {
                    //Pinky and Index finger move from the middle or ring finger
                    otherFinger.SpreadFinger(this.spreadAngle + angle);
                }
            }
        }
    }

    public bool IsBent() {
        return this.mcpAngle > 35f;
    }

    public override string ToString()
    {
        return "DIP:" + this.ipAngle + " PIP:" + this.ipAngle + " MCP:" + this.mcpAngle + " Spread:" + this.spreadAngle + "\n";
    }
}
