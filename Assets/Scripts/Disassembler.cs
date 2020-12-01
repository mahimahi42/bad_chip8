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
        DumpROM ();
    }

    // Update is called once per frame
    void Update () {
        
    }

    void DumpROM () {
        Text outputText = GameObject.Find ("DisassemblerDumpOutputPanelContentText").GetComponent<Text> ();

        byte[] romBytes = File.ReadAllBytes (testRomPath);
        StringBuilder sb = new StringBuilder ();
        List<byte> currBytes = new List<byte> ();

        for (int i = 0; i < romBytes.Length; i += 8) {
            sb.Append (GetRomDumpMemoryAddress (i));
            sb.Append (GetRomDumpMemoryLine (romBytes, i));
            sb.Append (GetRomDumpAsciiRepr (romBytes, i));
            sb.Append ("\n");
        }





        //sb.Append (string.Format ("0x{0:X4}  |  ", 0));
        //for (int i = 0; i < romBytes.Length; i += 2) {
        //    if (i < romBytes.Length - 1)
        //        sb.Append (string.Format ("0x{0:X2}{1:X2}", romBytes[i], romBytes[i + 1]) + " ");
        //    else
        //        sb.Append (string.Format ("0x{0:X2}", romBytes[i]) + " ");
        //    currBytes.Add (romBytes[i]);
        //    if (i % 8 == 0 && i != 0 || i == 8) {
        //        sb.Append ("  |  ");
        //        foreach (byte b in currBytes) {
        //            sb.Append (GetAsciiCharOrWhitespace (b));
        //        }
        //        currBytes.Clear ();
        //        sb.Append ("\n" + string.Format ("0x{0:X4}  |  ", i));
        //    }
        //}

        outputText.text = sb.ToString ();
        Debug.Log ("Done");
    }

    string GetRomDumpMemoryAddress (int addr) {
        return string.Format ("0x{0:X4}  |  ", addr);
    }

    string GetRomDumpMemoryLine (byte[] bytes, int addr) {
        StringBuilder sb = new StringBuilder ();

        int currAddr = addr;
        for (int i = 0; i < 4; i++) {
            if (currAddr >= bytes.Length) {
                sb.Append ("       ");
            } else if (currAddr + 1 >= bytes.Length) {
                sb.Append (string.Format ("0x{0:X2}00 ", bytes[currAddr]));
            } else {
                sb.Append (string.Format ("0x{0:X2}{1:X2} ", bytes[currAddr], bytes[currAddr + 1]));
            }
            currAddr += 2;
        }

        return sb.ToString ();
    }

    string GetRomDumpAsciiRepr (byte[] bytes, int addr) {
        StringBuilder sb = new StringBuilder ();
        sb.Append (" |  ");
        for (int i = addr; i < bytes.Length && i < addr + 4; i++) {
            sb.Append (GetAsciiCharOrWhitespace (bytes[i]));
        }
        sb.Append ("\n        |                               |  ");
        for (int i = addr + 4; i < bytes.Length && i < addr + 8; i++) {
            sb.Append (GetAsciiCharOrWhitespace (bytes[i]));
        }
        return sb.ToString ();
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
