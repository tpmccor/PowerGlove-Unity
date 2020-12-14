/* Filename:    CameraController.cs
 * Written by:  Dakta Topel
 * Course:      ECE 4960 Fall 2020
 */

using Newtonsoft.Json;
using System.Collections.Generic;

public class PowerGlove
{
    //size includes the open and closing brackets + number of elements in object
    public const int size = 20;

    [JsonProperty("a")] public int index_mcp { get; set; }
    [JsonProperty("b")] public int index_pip { get; set; }
    [JsonProperty("c")] public int middle_mcp { get; set; }
    [JsonProperty("d")] public int middle_pip { get; set; }
    [JsonProperty("e")] public int ring_mcp { get; set; }
    [JsonProperty("f")] public int ring_pip { get; set; }
    [JsonProperty("g")] public int pinky_mcp { get; set; }
    [JsonProperty("h")] public int pinky_pip { get; set; }
    [JsonProperty("i")] public int thumb_mcp { get; set; }
    [JsonProperty("j")] public int thumb_pip { get; set; }
    [JsonProperty("k")] public int thumb_hes { get; set; }
    [JsonProperty("l")] public int index_hes { get; set; }
    [JsonProperty("m")] public int ring_hes { get; set; }
    [JsonProperty("n")] public int pinky_hes { get; set; }
    [JsonProperty("o")] public int x_acc { get; set; }
    [JsonProperty("p")] public int y_acc { get; set; }
    [JsonProperty("q")] public int z_acc { get; set; }
    [JsonProperty("r")] public int roll { get; set; }
    [JsonProperty("s")] public int pitch { get; set; }
    [JsonProperty("t")] public int yaw { get; set; }

    // Returns a full list of glove sensor data (default 20 features)
    public List<int> ToList()
    {
        return new List<int>()
        {
            index_mcp, index_pip, middle_mcp,
            middle_pip, ring_mcp, ring_pip,
            pinky_mcp, pinky_pip, thumb_mcp,
            thumb_pip, thumb_hes, index_hes,
            ring_hes, pinky_hes
            ,x_acc, y_acc, z_acc,
            roll, pitch, yaw
        };
    }

    // Returns a shortened list that excludes IMU data (acceleration and rotation)
    public List<int> FingersToList()
    {
        return new List<int>()
        {
            index_mcp, index_pip, middle_mcp,
            middle_pip, ring_mcp, ring_pip,
            pinky_mcp, pinky_pip, thumb_mcp,
            thumb_pip, thumb_hes, index_hes,
            ring_hes, pinky_hes
        };
    }
}
