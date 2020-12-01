using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Disassembler : MonoBehaviour {
    class Chip8Instruction {
        public int instruction;

        public byte I {
            get {
                return (byte)((instruction & 0xF000) >> 12);
            }
        }
        public byte X {
            get {
                return (byte)((instruction & 0x0F00) >> 8);
            }
        }
        public byte Y {
            get {
                return (byte)((instruction & 0x00F0) >> 4);
            }
        }
        public byte N {
            get {
                return (byte)(instruction & 0x000F);
            }
        }
        public byte NN {
            get {
                return (byte)(instruction & 0x00FF);
            }
        }
        public int NNN {
            get {
                return instruction & 0x0FFF;
            }
        }

        public Chip8Instruction (int ins) {
            instruction = ins;
        }

        public Chip8Instruction (byte upper, byte lower) {
            int u = upper;
            int l = lower;
            instruction = u << 8 | l;
        }
    }

    [SerializeField]
    string testRomPath;

    // Start is called before the first frame update
    void Start () {
        DumpROM ();
        DisassembleROM ();
    }

    // Update is called once per frame
    void Update () {
        
    }

    void DisassembleROM () {
        Text outputText = GameObject.Find ("DisassemblerOutputPanelContentText").GetComponent<Text> ();

        byte[] romBytes = File.ReadAllBytes (testRomPath);
        StringBuilder sb = new StringBuilder ();

        for (int i = 0; i < romBytes.Length - 1; i += 2) {
            sb.Append (ParseInstruction (romBytes[i], romBytes[i + 1]));
        }

        outputText.text = sb.ToString ();
    }

    string ParseInstruction (byte upper, byte lower) {
        StringBuilder sb = new StringBuilder ();
        Chip8Instruction instruction = new Chip8Instruction (upper, lower);
             
        switch (instruction.I) {
            case 0x0:
                switch (instruction.NN) {
                    case 0xE0:
                        sb.Append ("CLS");
                        break;
                    case 0xEE:
                        sb.Append ("RET");
                        break;
                    default:
                        sb.Append (string.Format ("SYS  0x{0:X3}", instruction.NNN));
                        break;
                }
                break;
            case 0x1:
                sb.Append (string.Format ("JP   0x{0:X3}", instruction.NNN));
                break;
            case 0x2:
                sb.Append (string.Format ("CALL 0x{0:X3}", instruction.NNN));
                break;
            case 0x3:
                sb.Append (string.Format ("SE   V{0:X1}, 0x{1:X2}", instruction.X, instruction.NN));
                break;
            case 0x4:
                sb.Append (string.Format ("SNE  V{0:X1}, 0x{1:X2}", instruction.X, instruction.NN));
                break;
            case 0x5:
                sb.Append (string.Format ("SE   V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                break;
            case 0x6:
                sb.Append (string.Format ("LD   V{0:X1}, 0x{1:X2}", instruction.X, instruction.NN));
                break;
            case 0x7:
                sb.Append (string.Format ("ADD  V{0:X1}, 0x{1:X2}", instruction.X, instruction.NN));
                break;
            case 0x8:
                switch (instruction.N) {
                    case 0x0:
                        sb.Append (string.Format ("LD   V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    case 0x1:
                        sb.Append (string.Format ("OR   V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    case 0x2:
                        sb.Append (string.Format ("AND  V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    case 0x3:
                        sb.Append (string.Format ("XOR  V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    case 0x4:
                        sb.Append (string.Format ("ADD  V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    case 0x5:
                        sb.Append (string.Format ("SUB  V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    case 0x6:
                        sb.Append (string.Format ("SHR  V{0:X1}{, V{1:X1}}", instruction.X, instruction.Y));
                        break;
                    case 0x7:
                        sb.Append (string.Format ("SUBN V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    case 0xE:
                        sb.Append (string.Format ("SHL  V{0:X1}{, V{1:X1}}", instruction.X, instruction.Y));
                        break;
                    default:
                        sb.Append (string.Format ("!!!!!!!!!!! BAD_INS 0x{0:X4}", instruction.instruction));
                        break;
                }
                break;
            case 0x9:
                switch (instruction.N) {
                    case 0x0:
                        sb.Append (string.Format ("SNE  V{0:X1}, V{1:X1}", instruction.X, instruction.Y));
                        break;
                    default:
                        sb.Append (string.Format ("!!!!!!!!!!! BAD_INS 0x{0:X4}", instruction.instruction));
                        break;
                }
                break;
            case 0xA:
                sb.Append (string.Format ("LD   I, 0x{0:X3}", instruction.NNN));
                break;
            case 0xB:
                sb.Append (string.Format ("JP   V0, 0x{0:X2}", instruction.NN));
                break;
            case 0xC:
                sb.Append (string.Format ("RND  V{0:X1}, 0x{1:X2}", instruction.X, instruction.NN));
                break;
            case 0xD:
                sb.Append (string.Format ("DRW  V{0:X1}, V{1:X1}, {2:X1}", instruction.X, instruction.Y, instruction.N));
                break;
            case 0xE:
                switch (instruction.NN) {
                    case 0x9E:
                        sb.Append (string.Format ("SKP  V{0:X1}", instruction.X));
                        break;
                    case 0xA1:
                        sb.Append (string.Format ("SKNP V{0:X1}", instruction.X));
                        break;
                    default:
                        sb.Append (string.Format ("!!!!!!!!!!! BAD_INS 0x{0:X4}", instruction.instruction));
                        break;
                }
                break;
            case 0xF:
                switch (instruction.NN) {
                    case 0x07:
                        sb.Append (string.Format ("LD   V{0:X1}, DT", instruction.X));
                        break;
                    case 0x0A:
                        sb.Append (string.Format ("LD   V{0:X1}, K", instruction.X));
                        break;
                    case 0x15:
                        sb.Append (string.Format ("LD   DT, V{0:X1}", instruction.X));
                        break;
                    case 0x18:
                        sb.Append (string.Format ("LD   ST, V{0:X1}", instruction.X));
                        break;
                    case 0x1E:
                        sb.Append (string.Format ("ADD  I, V{0:X1}", instruction.X));
                        break;
                    case 0x29:
                        sb.Append (string.Format ("LD   F, V{0:X1}", instruction.X));
                        break;
                    case 0x33:
                        sb.Append (string.Format ("LD   B, V{0:X1}", instruction.X));
                        break;
                    case 0x55:
                        sb.Append (string.Format ("LD   [I], V{0:X1}", instruction.X));
                        break;
                    case 0x65:
                        sb.Append (string.Format ("LD   V{0:X1}, [I]", instruction.X));
                        break;
                    default:
                        sb.Append (string.Format ("!!!!!!!!!!! BAD_INS 0x{0:X4}", instruction.instruction));
                        break;
                }
                break;
            default:
                sb.Append (string.Format ("!!!!!!!!!!! BAD_INS 0x{0:X4}", instruction.instruction));
                break;
        }

        sb.Append ("\n");
        return sb.ToString ();
    }

    void DumpROM () {
        Text outputText = GameObject.Find ("DisassemblerDumpOutputPanelContentText").GetComponent<Text> ();

        byte[] romBytes = File.ReadAllBytes (testRomPath);
        StringBuilder sb = new StringBuilder ();

        for (int i = 0; i < romBytes.Length; i += 8) {
            sb.Append (GetRomDumpMemoryAddress (i));
            sb.Append (GetRomDumpMemoryLine (romBytes, i));
            sb.Append (GetRomDumpAsciiRepr (romBytes, i));
            sb.Append ("\n");
        }

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
