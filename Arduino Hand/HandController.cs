/* Filename:    CameraController.cs
 * Written by:  Tom Birdsong, Duncan McCain, Travis McCormick, and Dakota Topel
 * Course:      ECE 4960 Fall 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Leap_Motion;

public class HandController : MonoBehaviour
{
    public Transform wrist;
    public string portName = "COM3";
    public int baudRate = 115200;

    public GloveTrainingBuffer buf;
    public TFSharpAgent agent;
    public UI_Manager UI;

    private Hand hand;
    private SerialPort sp;

    void Start() //Called before the first frame
    {
        //Init serial port
        sp = new SerialPort(portName, baudRate);
        sp.Open();
        sp.ReadTimeout = 10000;

        hand = new Hand(wrist);
    }

    void Update() //Called every frame
    {
        //Poll the serial port
        try
        {
            if (!sp.IsOpen)
            {
                sp.Open();
            }

            var JsonString = GetJSONstring();
            if (JsonString == null) //Ignore Json strings that had errors
            {
                return;
            }
            var glove = (PowerGlove)JsonConvert.DeserializeObject(JsonString, typeof(PowerGlove));

            // If there is a training buffer available then add the data string
            if (buf != null && buf.gameObject.activeSelf) buf.AddData(glove);

            // If there is an agent available then infer the ASL gesture
            if (agent != null && agent.gameObject.activeSelf)
            {
                var result = agent.RunInference(glove.FingersToList().ConvertAll(new Converter<int, float>(x => x)));

                // TODO Optionally send label to a canvas in the scene
                if (result.HasValue)
                { 
                    UI.UpdateHandValue((result != 10 ? result.ToString() : "None"));
                }
            }

            //Write data from Json pacakge to the hand
            hand.fingers[Hand.INDEX].BendJoint(Finger.MCP, ScaleBend(glove.index_mcp));
            hand.fingers[Hand.INDEX].BendJoint(Finger.IP, ScaleBend(glove.index_pip));

            hand.fingers[Hand.MIDDLE].BendJoint(Finger.MCP, ScaleBend(glove.middle_mcp));
            hand.fingers[Hand.MIDDLE].BendJoint(Finger.IP, ScaleBend(glove.middle_pip));

            hand.fingers[Hand.RING].BendJoint(Finger.MCP, ScaleBend(glove.ring_mcp));
            hand.fingers[Hand.RING].BendJoint(Finger.IP, ScaleBend(glove.ring_pip));

            hand.fingers[Hand.PINKY].BendJoint(Finger.MCP, ScaleBend(glove.pinky_mcp));
            hand.fingers[Hand.PINKY].BendJoint(Finger.IP, ScaleBend(glove.pinky_pip));

            hand.thumb.BendJoint(Thumb.MCP, ScaleBend(glove.thumb_mcp));
            hand.thumb.BendJoint(Thumb.IP, ScaleBend(glove.thumb_pip));

            hand.spreadFingers(ScaleSpread(glove.index_hes), ScaleSpread(glove.ring_hes), ScaleSpread(glove.pinky_hes), ScaleSpread(glove.thumb_hes));
            hand.thumb.SpreadThumb(ScaleSpread_Thumb(glove.thumb_hes));

            hand.RotateHand(ScaleRotation(glove.roll), ScaleRotation(glove.pitch), ScaleRotation(glove.yaw));
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }

    private string GetJSONstring()
    {
        string serialBuffer = "";
        while (!serialBuffer.Equals("{")) //Look for start of Json string
        {
            serialBuffer = sp.ReadLine();
        }

        int count = 0;
        while (true)
        {
            string value = sp.ReadLine();
            serialBuffer += value;
            if (value.Equals("{")) //If another open bracket is found, return null
            {
                return null;
            }

            if (value.Equals("}")) //Break out of the loop and return the Json string
            {
                break;
            }

            count++;
            if (count > PowerGlove.size) //If the closing bracket is not found when it should be, return null
            {
                return null;
            }
            
        }

        return serialBuffer;
    }

    private float ScaleBend(int num)
    {
        //Scale 1 - 255 to 0 - 90
        int in_min = 255;
        int in_max = 1; 
        int out_min = 0;
        int out_max = 90;

        return (float)((num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }

    private float ScaleSpread(int num)
    {
        // Scale 1 - 255 to 0 - 30
        int in_min = 1;
        int in_max = 255;
        int out_min = 0;
        int out_max = 17;

        return (float)((num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }

    private float ScaleSpread_Thumb(int num)
    {
        // Scale 1 - 255 to 0 - 30
        int in_min = 255;
        int in_max = 1;
        int out_min = 0;
        int out_max = 17;

        return (float)((num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }

    private float ScaleRotation(int num)
    {
        //Scale glove input of [1, 255] to [-180, 180]
        int in_min = 255;
        int in_max = 1;
        int out_min = -180;
        int out_max = 180;
        
        return (float)((num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }
}

