using UnityEngine;
 
public class ShowTimw : MonoBehaviour
{
 public class Example : MonoBehaviour
 {
     private int width, height;
     private Rect rect;
     private GUIStyle labelStyle;
     private string currentTime;
 
     void Awake()
     {
         width = Screen.width;
         height = Screen.height;
         rect = new Rect(10, 10, width - 20, height - 20);
     }
 
     void OnGUI()
     {
         // Display the label at the center of the window.
         labelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
         labelStyle.alignment = TextAnchor.MiddleCenter;
 
         // Modify the size of the font based on the window.
         labelStyle.fontSize = 12 * (width / 200);
 
         // Obtain the current time.
         currentTime = Time.time.ToString("f6");
         currentTime = "Time is: " + currentTime + " sec.";
 
         // Display the current time.
         GUI.Label(rect, currentTime, labelStyle);
     }
 }
}
