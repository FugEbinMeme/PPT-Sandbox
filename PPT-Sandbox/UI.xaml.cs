using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox {
    public partial class UI {
        private ProcessMemory Game = new ProcessMemory("puyopuyotetris", false);
        public UI() {
            InitializeComponent();

            Version.Text = $"PPT-Sandbox-{Assembly.GetExecutingAssembly().GetName().Version.Minor} by {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName}";

            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
                case "ko":
                    break;
                    
                case "ja":
                    break;
                    
                default:
                    ScriptsHeader.Header = "      SCRIPTS";
                        General.Text = "General";
                            PentominoVersus.Content = "Pentomino Versus";
                            RemoveLineClearDelay.Content = "Remove Line Clear Delay";
                            UndoHold.Content = "Undo Hold";
                            SecretGradeGarbage.Content = "Secret Grade Garbage";
                            GarbageBlocking.Content = "Garbage Blocking";
                            UnCappedPC.Content = "Remove Perfect Clear Damage Cap";

                        AutoLocking.Text = "Piece Auto-Locking";
                            RemoveAutoLock.Content = "Disable Auto-Locking";
                            TGMAutoLock.Content = "Arika Style Auto-Locking";

                        TetrisB2B.Text = "Back-to-Back Tetris";
                            TetrisB2BDouble.Content = "Tetris B2B doubles attack";
                            TetrisB2BAdd2.Content = "Tetris B2B adds 2 attack";

                        TspinB2B.Text = "Back-to-Back T-Spin";
                            TspinB2BDouble.Content = "T-Spin B2B doubles attack";
                            TspinB2BAdd2.Content = "T-Spin B2B adds 2 attack";

                        RotationSystems.Text = "Rotation System";
                            Ascension.Content = "Ascension";
                            Cultris2.Content = "Cultris II";

                    AttackHeader.Header = "ATTACK";
                        TetrisVsTetris.Header = "     Tetris vs Tetris";
                            Attacks.Text = "Attacks";

                            Single.Text = "Single";
                            Double.Text = "Double";
                            Triple.Text = "Triple";
                            Tetris.Text = "Tetris";
                            TetrisPlus.Text = "Tetris Plus";

                            Clear.Text = "Clear";
                            PerfectClear.Text = "Perfect Clear";
                            TSpin.Text = "T-Spin";

                            Combo.Text = "Combos";

                        TetrisVsPuyo.Header = "Tetris vs Puyo";
                        Puyo.Header = "Puyo";

                    GarbageHeader.Header = "GARBAGE";
                        GarbageGeneration.Text = "Garbage Generation";
                            CleanGarbage.Title = "Clean Garbage Chance";
                            GarbageFilled.Title = "Filled Garbage Tile";
                            GarbageEmpty.Title = "Empty Garbage Tile";
                    break;
            }

            Scripts = new Dictionary<Control, Action<bool>>() {
                {PentominoVersus, x => {
                    IntPtr addr = (IntPtr)Game.ReadUInt64(new IntPtr(0x140463F20));

                    if (x) {
                        Game.WriteByte(addr, 5);            //S
                        Game.WriteInt32(addr + 0x24, -2);
                        Game.WriteByte(addr + 0xE0, 5);     //Z
                        Game.WriteByteArray(
                            addr + 0x104,
                            ConvertByteString("01 00 00 00 FF FF FF FF")
                        );
                        Game.WriteByte(addr + 0x1C0, 5);    //J
                        Game.WriteInt32(addr + 0x1E8, -1);
                        Game.WriteByte(addr + 0x2A0, 5);    //L
                        Game.WriteByteArray(
                            addr + 0x2C4,
                            ConvertByteString("FF FF FF FF FF FF FF FF")
                        );
                        Game.WriteByte(addr + 0x380, 5);    //T
                        Game.WriteInt32(addr + 0x3A8, 2);
                        Game.WriteByte(addr + 0x460, 5);    //O
                        Game.WriteInt32(addr + 0x484, -1);
                        Game.WriteByte(addr + 0x540, 5);    //I
                        Game.WriteInt32(addr + 0x564, -2);

                    } else {
                        Game.WriteByte(addr, 4);          //S
                        Game.WriteByte(addr + 0xE0, 4);   //Z
                        Game.WriteByte(addr + 0x1C0, 4);  //J
                        Game.WriteByte(addr + 0x2A0, 4);  //L
                        Game.WriteByte(addr + 0x380, 4);  //T
                        Game.WriteByte(addr + 0x460, 4);  //O
                        Game.WriteByte(addr + 0x540, 4);  //I
                    }
                }},
                {RemoveLineClearDelay, x =>
                    Game.WriteByte(
                        new IntPtr(0x1400A26F4),
                        Convert.ToByte(!x)
                    )
                },
                {UndoHold, x => {
                    byte[] write = x
                        ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 } //enable
                        : new byte[] { 0xFF, 0x83, 0x48, 0x01, 0x00, 0x00 };//disable

                    Game.WriteByteArray(new IntPtr(0x142852508), write);
                    Game.WriteByteArray(new IntPtr(0x14285252F), write);
                }},
                {SecretGradeGarbage, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x14009F480),
                        x
                            ? ConvertByteString("8A 1D 7A 3B 3C 00 80 3D 74 3B 3C 00 00 75 08 FF 05 6B 3B 3C 00 EB 06 FF 0D 63 3B 3C 00 80 3D 5D 3B 3C 00 00 75 10 80 3D 53 3B 3C 00 09 7C 27 C6 05 4B 3B 3C 00 01 80 3D 43 3B 3C 00 00 75 17 C6 05 3B 3B 3C 00 00 80 3D 33 3B 3C 00 0A 72 07 C6 05 2A 3B 3C 00 00 E9 85 3B 61 02")
                            : ConvertByteString("E9 DB 3B 61 02")
                    );

                    Game.WriteByteArray(
                        new IntPtr(0x140061010),
                        x
                            ? ConvertByteString("66 C7 05 E7 1F 40 00 00 00 E9 A2 79 91 01")
                            : ConvertByteString("E9 AB 79 91 01")
                    );
                }},
                {GarbageBlocking, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1400A26D1),
                        x
                            ? new byte[] { 0x90, 0x90 }
                            : new byte[] { 0x75, 0x0F }
                    )
                },
                {UnCappedPC, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1427E4539),
                        x
                            ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                            : new byte[] { 0x0F, 0x85, 0x8A, 0x00, 0x00, 0x00 }
                    )
                },
                {RemoveAutoLock, x =>
                    Game.WriteByte(
                        new IntPtr(0x142853B33),
                        Convert.ToByte(!x)
                    )
                },
                {TGMAutoLock, x => {
                    byte[] write = x
                        ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                        : new byte[] { 0x66, 0x41, 0x89, 0x86, 0x10, 0x01, 0x00, 0x00 };

                    Game.WriteByteArray(new IntPtr(0x1400A76CF), write);
                    Game.WriteByteArray(new IntPtr(0x1400A6EE0), write);
                }},
                {TetrisB2BDouble, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF3F),
                        x
                            ? new byte[] { 0x45, 0x84, 0xDB, 0x75, 0x05, 0x01, 0xC8, 0x90, 0x90, 0x90 }
                            : new byte[] { 0x31, 0xC9, 0x45, 0x84, 0xDB, 0x0F, 0x95, 0xD1, 0x01, 0xC8 }
                    )
                },
                {TetrisB2BAdd2, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF3F),
                        x
                            ? new byte[] { 0x45, 0x84, 0xDB, 0x74, 0x05, 0xFF, 0xC0, 0xFF, 0xC0, 0x90 }
                            : new byte[] { 0x31, 0xC9, 0x45, 0x84, 0xDB, 0x0F, 0x95, 0xD1, 0x01, 0xC8 }
                    )
                },
                {TspinB2BDouble, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF74),
                        x
                            ? new byte[] { 0x45, 0x84, 0xDB, 0x75, 0x05, 0x01, 0xC8, 0x90, 0x90, 0x90 }
                            : new byte[] { 0x31, 0xC9, 0x45, 0x84, 0xDB, 0x0F, 0x95, 0xD1, 0x01, 0xC8 }
                    )
                },
                {TspinB2BAdd2, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF74),
                        x
                            ? new byte[] { 0x45, 0x84, 0xDB, 0x74, 0x05, 0xFF, 0xC0, 0xFF, 0xC0, 0x90 }
                            : new byte[] { 0x31, 0xC9, 0x45, 0x84, 0xDB, 0x0F, 0x95, 0xD1, 0x01, 0xC8 }
                    )
                },
                {Ascension, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1426CCE8E),
                        x
                            ? ConvertByteString("E9 17 51 93 FD 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90")
                            : ConvertByteString("44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C")
                        );
                    Game.WriteByte(
                        new IntPtr(0x1426CCEBE),
                        (byte)(x
                            ? 0x16
                            : 0x05)
                    );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140001FAA),
                            ConvertByteString("48 8B D3 48 B9 00 20 46 40 01 00 00 00 44 0F BE 34 51 44 0F BE 7C 51 01 83 7C 24 58 01 0F 84 D5 AE 6C 02 41 F7 DE E9 CD AE 6C 02")
                        );
                        Game.WriteByteArray(
                            new IntPtr(0x140462000),
                            ConvertByteString("00 00 FF 00 00 FF FF FF 00 FE FF FE FE 00 FE FF FE FF 01 00 01 FF 00 01 FF 01 FE 01 01 FE 02 00 00 02 FF 02 FE 02 02 FF 02 FE 01 01")
                        );
                    }
                }},
                {Cultris2, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1426CCE8E),
                        x
                            ? ConvertByteString("E9 17 51 93 FD 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90")
                            : ConvertByteString("44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C")
                        );
                    Game.WriteByte(
                        new IntPtr(0x1426CCEBE),
                        (byte)(x
                            ? 0x08
                            : 0x05)
                    );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140001FAA),
                            ConvertByteString("48 8B D3 48 B9 00 20 46 40 01 00 00 00 44 0F BE 34 51 44 0F BE 7C 51 01 83 7C 24 58 01 0F 84 D5 AE 6C 02 41 F7 DE E9 CD AE 6C 02")
                        );
                        Game.WriteByteArray(
                            new IntPtr(0x140462000),
                            ConvertByteString("00 00 FF 00 01 00 00 FF FF FF 01 FF FE 00 02 00 00 00")
                        );
                    }
                }}
            };
        }

        private byte[] ConvertByteString(string bytes) =>
            bytes.Split(' ').Select(i => Convert.ToByte(i, 16)).ToArray();

        private Dictionary<Control, Action<bool>> Scripts;

        private void CheckBoxHandle(object sender, RoutedEventArgs e) {
            CheckBox source = (CheckBox)sender;
            Scripts[source].Invoke(source.IsChecked == true);
        }

        private void RadioButtonHandle(object sender, RoutedEventArgs e) {
            OptionalRadioButton source = (OptionalRadioButton)sender;
            OptionalRadioButton previous = OptionalRadioButton.GetSelected(source.GroupName);

            if (previous != null) 
                Scripts[previous].Invoke(false);
            
            if (source.IsChecked == true)
                Scripts[source].Invoke(true);
        }
    }
}
