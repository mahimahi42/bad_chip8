using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Disassembler : MonoBehaviour {
    [SerializeField]
    string testRomPath;

    // Start is called before the first frame update
    void Start () {
        DisassembleRom ();
    }

    // Update is called once per frame
    void Update () {
        
    }

    void DisassembleRom () {
        string debugRomOutput = Path.Combine (Path.GetDirectoryName (testRomPath), "output.txt");

        if (File.Exists (debugRomOutput)) {
            File.Delete (debugRomOutput);
        }

        Text outputText = GameObject.Find ("DisassemblerOutputPanelContentText").GetComponent<Text> ();

        //using (StreamWriter outputFile = new StreamWriter (debugRomOutput)) {
            byte[] romBytes = File.ReadAllBytes (testRomPath);
            StringBuilder sb = new StringBuilder ();
            List<byte> currBytes = new List<byte> ();
            sb.Append (string.Format ("0x{0:X4}  |  ", 0));
            for (int i = 0; i < romBytes.Length; i++) {
                sb.Append (string.Format ("0x{0:X2}", romBytes[i]) + " ");
                currBytes.Add (romBytes[i]);
                if ((i + 1) % 4 == 0) {
                    sb.Append ("  |  ");
                    foreach (byte b in currBytes) {
                        sb.Append (GetAsciiCharOrWhitespace (b));
                    }
                    //sb.Clear ();
                    currBytes.Clear ();
                    sb.Append ("\n" + string.Format ("0x{0:X4}  |  ", i));
                }
            }
        //}

        outputText.text = sb.ToString ();
        Debug.Log ("Done");
    }

    char GetAsciiCharOrWhitespace (byte b) {
        if (b < 128) {
            if (char.IsControl ((char) b)) {
                return ' ';
            }
            return (char)b;
        } else return ' ';
    }
}
