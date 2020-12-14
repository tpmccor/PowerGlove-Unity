/* Filename:    Thumb.cs
 * Written by:  Travis McCormick
 * Course:      ECE 4960 Fall 2020
 */

using System.Runtime.CompilerServices;
using UnityEngine;

public class Thumb
{
    public const int IP = 0;
    public const int MCP = 1;
    public const int CMC = 2;
    public const int boneChainLength = 3;

    public const float delta = 2f;

    private float ipAngle;
    private float mcpAngle;
    private float oppAngle;
    private float spreadAngle;
    private readonly Transform[] joints;
    private readonly Hand hand;

    public Thumb()
    {
        this.ipAngle = 0;
        this.mcpAngle = 0;
        this.oppAngle = 0;
        this.spreadAngle = 0;
        this.joints = null;
        this.hand = null;
    }

    public Thumb(Transform thumbTip, Hand handRef)
    {
        this.ipAngle = 0;
        this.mcpAngle = 0;
        this.oppAngle = 0;
        this.spreadAngle = 0;
        this.joints = null;
        this.hand = handRef;

        if(thumbTip != null)
        {
            //Initialize joint transform references
            this.joints = new Transform[boneChainLength];
            Transform currentJoint = thumbTip.parent;

            for(int i = 0; i < boneChainLength && currentJoint != null; i++)
            {
                this.joints[i] = currentJoint.transform;
                currentJoint = currentJoint.parent;
            }

            //Initialize resting position
            this.joints[IP].transform.localEulerAngles = new Vector3(0, 0, -10);
            this.joints[MCP].transform.localEulerAngles = new Vector3(0, 0, -5);
            this.joints[CMC].transform.localEulerAngles = new Vector3(70, -50, 0);
        }
    }

    public void BendJoint(int jointNum, float angle)
    {
        if(this.joints != null)
        {
            if (jointNum == MCP)
            {
                //Rotate MCP joint around local space z-axis
                if (Mathf.Abs(this.mcpAngle - angle) > delta)
                {
                    angle = (this.mcpAngle + angle) / 2f;
                    this.joints[MCP].transform.Rotate(Vector3.forward, this.mcpAngle - angle, Space.Self);
                    this.mcpAngle = angle;
                }

                //Oppose thumb slightly when the MCP is bent, because there is no palm sensor anymore
                this.OpposeThumb(angle / 2.5f);
            }
            else
            {
                //Rotate IP joint around local space z-axis
                if (Mathf.Abs(this.ipAngle - angle) > delta)
                {
                    angle = (this.ipAngle + angle) / 2f;
                    this.joints[IP].transform.Rotate(Vector3.forward, this.ipAngle - angle, Space.Self);
                    this.ipAngle = angle;
                }
            }
        }
    }

    public void SpreadThumb(float angle)
    {
        if (this.joints != null)
        {
            if (this.IsBent())
            {
                angle = 0f;
            }

            //Rotate CMC towards the base of the index finger
            if(Mathf.Abs(this.spreadAngle - angle) > delta)
            {
                angle = (this.spreadAngle + angle) / 2f; 
                Vector3 direction = this.joints[CMC].forward - this.joints[CMC].up;
                this.joints[CMC].transform.Rotate(direction, this.spreadAngle - angle, Space.World);
                this.spreadAngle = angle;
            }
        }
    }

    public void OpposeThumb(float angle)
    {
        if(this.joints != null)
        {
            //Reference orientation of the wrist/palm for opposition movement
            //Pull thumb metacarpol bone across the palm, rotate the soft side of the thumb to face the palm, and pull the metacarpol bone out from the palm a little
            if (Mathf.Abs(this.oppAngle - angle) > delta)
            {
                angle = (this.oppAngle + angle) / 2f;
                Vector3 direction = this.hand.Wrist.up + (this.hand.Wrist.right * 2f) - (this.hand.Wrist.forward / 2.5f);
                this.joints[CMC].transform.Rotate(direction, angle - this.oppAngle, Space.World);
                this.oppAngle = angle;
            }
        }
    }

    public bool IsBent()
    {
        return this.mcpAngle > 45f;
    }

    public override string ToString()
    {
        return "IP:" + this.ipAngle + " MCP:" + this.mcpAngle + " Opposition:" + this.oppAngle + " Spread:" + this.spreadAngle + "\n";
    }
}
