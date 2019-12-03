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
                            DoubleRotate.Content = "180 Rotations";
                            FreezeSwap.Content = "Freeze Swap Timer";

                        AutoLocking.Text = "Piece Auto-Locking";
                            RemoveAutoLock.Content = "Disable Auto-Locking";
                            TGMAutoLock.Content = "Arika Style Auto-Locking";

                        RotationSystems.Text = "Rotation System";
                            Ascension.Content = "Ascension";
                            Cultris2.Content = "Cultris II";
                            h.Content = "h";
                            BONKERS.Content = "B.O.N.K.E.R.S.";

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
                            TvPAttacks.Text = "Attacks";

                            TvPSingle.Text = "Single";
                            TvPDouble.Text = "Double";
                            TvPTriple.Text = "Triple";
                            TvPTetris.Text = "Tetris";
                            TvPTetrisPlus.Text = "Tetris Plus";

                            TvPClear.Text = "Clear";
                            TvPPerfectClear.Text = "Perfect Clear";
                            TvPTSpin.Text = "T-Spin";

                            TvPCombo.Text = "Combos";

                        Puyo.Header = "Puyo";
                            GarbageRate.Title = "Garbage Rate";
                            AllClearMultiplier.Title = "All Clear Multiplier";

                            Chain.Text = "Chain";

                    GarbageHeader.Header = "GARBAGE";
                        GarbageGeneration.Text = "Garbage Generation";
                            CleanGarbage.Title = "Clean Garbage Chance";
                            GarbageFilled.Title = "Filled Garbage Tile";
                            GarbageEmpty.Title = "Empty Garbage Tile";

                        GarbageModification.Text = "Garbage Modification";
                            SecretGradeGarbage.Content = "Secret Grade Garbage";
                            GarbageBlocking.Content = "Garbage Blocking";
                            UnCappedPC.Content = "Remove Perfect Clear Damage Cap";

                        TetrisB2B.Text = "Back-to-Back Tetris";
                            TetrisB2BDouble.Content = "Tetris B2B doubles attack";
                            TetrisB2BAdd2.Content = "Tetris B2B adds 2 attack";
                            TetrisB2BCum.Content = "Tetris B2B stacks";

                        TspinB2B.Text = "Back-to-Back T-Spin";
                            TspinB2BDouble.Content = "T-Spin B2B doubles attack";
                            TspinB2BAdd2.Content = "T-Spin B2B adds 2 attack";
                            TspinB2BCum.Content = "T-Spin B2B stacks";
                    break;
            }

            Scripts = new Dictionary<Control, Action<bool>>() {
                {PentominoVersus, x => {
                    IntPtr addr = (IntPtr)Game.ReadUInt64(new IntPtr(0x140463F20));

                    byte minoCount = (byte)(Convert.ToByte(x) + 4);

                    Game.WriteByte(addr, minoCount);            //S
                    Game.WriteByte(addr + 0xE0, minoCount);     //Z
                    Game.WriteByte(addr + 0x1C0, minoCount);    //J
                    Game.WriteByte(addr + 0x2A0, minoCount);    //L
                    Game.WriteByte(addr + 0x380, minoCount);    //T
                    Game.WriteByte(addr + 0x460, minoCount);    //O
                    Game.WriteByte(addr + 0x540, minoCount);    //I

                    if (x) {
                        Game.WriteInt32(addr + 0x24, -2);
                        Game.WriteByteArray(
                            addr + 0x104,
                            ConvertByteString("01 00 00 00 FF FF FF FF")
                        );
                        Game.WriteInt32(addr + 0x1E8, -1);
                        Game.WriteByteArray(
                            addr + 0x2C4,
                            ConvertByteString("FF FF FF FF FF FF FF FF")
                        );
                        Game.WriteInt32(addr + 0x3A8, 2);
                        Game.WriteInt32(addr + 0x484, -1);
                        Game.WriteInt32(addr + 0x564, -2);
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
                            ConvertByteString("48 8B D3 48 B9 00 20 46 40 01 00 00 00 44 0F BE 34 51 44 0F BE 7C 51 01 E9 DB AE 6C 02")
                        );
                        Game.WriteByteArray(
                            new IntPtr(0x140462000),
                            ConvertByteString("00 00 FF 00 01 00 00 FF FF FF 01 FF FE 00 02 00 00 00")
                        );
                    }
                }},
                {DoubleRotate, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1400A6D53),
                        x
                            ? ConvertByteString("E9 52 FF FF FF 90")
                            : ConvertByteString("A8 10 74 02 FF CF")
                        );
                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x1400A6CAA),
                            ConvertByteString("57 40 8A BB 5C 01 00 00 40 80 E7 30 83 FF 30 5F 75 25 A8 30 74 21 41 8B 7E 30 8B BF C8 03 00 00 80 7F 18 03 74 0A BF 02 00 00 00 E9 7F 00 00 00 BF FE FF FF FF EB 78 A8 10 74 74 FF CF EB 70")
                        );
                    }
                }},
                {FreezeSwap, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1400433A5),
                        x
                            ? ConvertByteString("E8 F6 FE FF FF 90")
                            : ConvertByteString("89 83 24 04 00 00")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x1400432A0),
                            ConvertByteString("89 83 24 04 00 00 51 48 31 C9 48 8B 0D 07 DF 41 00 FF 81 A4 00 00 00 59 C3")
                        );
                    }
                }},
                {TetrisB2BCum, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF44),
                        x
                            ? ConvertByteString("E8 F7 BA 99 FD")
                            : ConvertByteString("0F 95 D1 01 C8")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x1400B9A40),
                            ConvertByteString("74 0C 8A 8E F3 03 00 00 D0 F9 FE C1 01 C8 C3")
                        );
                    }
                }},
                {TspinB2BCum, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF79),
                        x
                            ? ConvertByteString("E8 C2 BA 99 FD")
                            : ConvertByteString("0F 95 D1 01 C8")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x1400B9A40),
                            ConvertByteString("74 0C 8A 8E F3 03 00 00 D0 F9 FE C1 01 C8 C3")
                        );
                    }
                }},
                {h, x =>
                    Game.WriteByte(
                        new IntPtr(0x1426CCEB4),
                        (byte)(x
                            ? 0xEB
                            : 0x74
                        )
                    )
                },
                {BONKERS, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1426CCE8E),
                        x
                            ? ConvertByteString("E9 17 51 93 FD 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90")
                            : ConvertByteString("44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C")
                        );

                    Game.WriteByte(
                        new IntPtr(0x1426CCEB4),
                        (byte)(x
                            ? 0x7F
                            : 0x05
                        )
                    );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140462000),
                            ConvertByteString("00 01 FF")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x140001FAA),
                            ConvertByteString("45 31 F6 45 31 FF 84 DB 0F 84 EA AE 6C 02 50 53 48 31 C0 31 D2 8B C7 BB 03 00 00 00 F7 F3 48 B9 00 20 46 40 01 00 00 00 44 0F BE 34 11 41 BF 16 00 00 00 41 29 C7 41 F7 DF 5B 58 E9 B8 AE 6C 02")
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
