using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Sandbox {
    public partial class UI {
        private ProcessMemory Game = new ProcessMemory("puyopuyotetris", false);

        private Dictionary<Control, Action<bool>> Scripts;
        private Dictionary<Dial, Action<int>> DialScripts;
        private Dictionary<UniformGrid, Action<int, int>> TableScripts;

        private byte[] ConvertByteString(string bytes) =>
            bytes.Split(' ').Select(i => Convert.ToByte(i, 16)).ToArray();

        public UI() {
            InitializeComponent();
            FreezeDial = false;

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
                            ColX.Content = "Remove Left/Right Collision";
                            ColM.Content = "Remove Mino Collision";
                            PreserveRot.Content = "Preserve Rotation on Held Piece";
                            Lockout.Content = "Remove Lock-Out Death";

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
                            RecieveT.Title = "Tetris Max Recieval";
                            RecieveP.Title = "Puyo Max Recieval";

                        GarbageModification.Text = "Garbage Modification";
                            SecretGradeGarbage.Content = "Secret Grade Garbage";
                            GarbageBlocking.Content = "Garbage Blocking";
                            UnCappedPC.Content = "Remove Perfect Clear Damage Cap";
                            ColorClear.Content = "Color Clear";
                            AllSpin.Content = "All Spins";

                        TetrisB2B.Text = "Back-to-Back Tetris";
                            TetrisB2BDouble.Content = "Tetris B2B doubles attack";
                            TetrisB2BAdd2.Content = "Tetris B2B adds 2 attack";
                            TetrisB2BCum.Content = "Tetris B2B stacks";

                        TspinB2B.Text = "Back-to-Back T-Spin";
                            TspinB2BDouble.Content = "T-Spin B2B doubles attack";
                            TspinB2BAdd2.Content = "T-Spin B2B adds 2 attack";
                            TspinB2BCum.Content = "T-Spin B2B stacks";

                    OtherHeader.Header = "OTHER";
                        Delays.Text = "Line Delay";
                            DelayBase.Text = "Base";
                            DelaySingle.Text = "Single";
                            DelayDouble.Text = "Double";
                            DelayTriple.Text = "Triple";
                            DelayTetris.Text = "Tetris";
                            DelayTetrisPlus.Text = "Tetris Plus";

                        DAS.Title = "DAS";
                        //0ARR

                        TspinDetection.Text = "T-Spin Detection";
                            FullTmini.Content = "All T-Mini's are full";
                            NoT.Content = "No T-Spins";
                            AllT.Content = "Every Spin is a T-Spin";
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
                        ? ConvertByteString("90 90 90 90 90 90") //enable
                        : ConvertByteString("FF 83 48 01 00 00");//disable

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
                            ? ConvertByteString("90 90")
                            : ConvertByteString("75 0F")
                    )
                },
                {UnCappedPC, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1427E4539),
                        x
                            ? ConvertByteString("90 90 90 90 90 90")
                            : ConvertByteString("0F 85 8A 00 00 00")
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
                        ? ConvertByteString("90 90 90 90 90 90 90 90")
                        : ConvertByteString("66 41 89 86 10 01 00 00");

                    Game.WriteByteArray(new IntPtr(0x1400A76CF), write);
                    Game.WriteByteArray(new IntPtr(0x1400A6EE0), write);
                }},
                {TetrisB2BDouble, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF3F),
                        x
                            ? ConvertByteString("45 84 DB 75 05 01 C8 90 90 90")
                            : ConvertByteString("31 C9 45 84 DB 0F 95 D1 01 C8")
                    )
                },
                {TetrisB2BAdd2, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF3F),
                        x
                            ? ConvertByteString("45 84 DB 74 05 FF C0 FF C0 90")
                            : ConvertByteString("31 C9 45 84 DB 0F 95 D1 01 C8")
                    )
                },
                {TspinB2BDouble, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF74),
                        x
                            ? ConvertByteString("45 84 DB 75 05 01 C8 90 90 90")
                            : ConvertByteString("31 C9 45 84 DB 0F 95 D1 01 C8")
                    )
                },
                {TspinB2BAdd2, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x14271DF74),
                        x
                            ? ConvertByteString("45 84 DB 74 05 FF C0 FF C0 90")
                            : ConvertByteString("31 C9 45 84 DB 0F 95 D1 01 C8")
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
                        new IntPtr(0x1426CCEBE),
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
                }},
                {ColorClear, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1400A29B0),
                        x
                            ? ConvertByteString("E9 4B 70 01 00 90 90 90")
                            : ConvertByteString("48 8B 08 41 83 3C 08 FF")
                        );

                    Game.WriteByteArray(
                        new IntPtr(0x142795324),
                        x
                            ? ConvertByteString("E9 F5 46 92 FD 90 90 90")
                            : ConvertByteString("0F B6 94 28 D0 00 32 00")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x1400B9A00),
                            ConvertByteString("48 8B 08 42 83 3C 01 09 74 0A 42 83 3C 01 FF E9 A1 8F FE FF C6 86 EC 03 00 00 01 EB F2 CC 0F B6 94 28 D0 00 32 00 80 B9 EC 03 00 00 01 75 09 C6 81 EC 03 00 00 00 D1 FA E9 EC B8 6D 02")
                        );
                    }
                }},
                {AllSpin, x => {
                    Game.WriteByte(
                        new IntPtr(0x14280D093),    //remove mini
                        Convert.ToByte(!x)
                    );

                    Game.WriteByteArray(
                        new IntPtr(0x141A70306),    //jump to sfx patching
                        x
                            ? ConvertByteString("E9 6C FD 59 FE")
                            : ConvertByteString("89 FA 48 89 D9")
                        );

                    Game.WriteByteArray(
                        new IntPtr(0x1426CCECD),    //jump to immobile detection
                        x
                            ? ConvertByteString("E9 17 30 94 FD 90")
                            : ConvertByteString("8D 47 01 89 46 28")
                        );

                    Game.WriteByteArray(
                        new IntPtr(0x1400A7625),    //jump to flag reset on movement
                        x
                            ? ConvertByteString("E9 99 8A F6 FF")
                            : ConvertByteString("48 8B 5C 24 40")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140010077),//sfx patching
                            ConvertByteString("48 83 FF 51 75 3B 41 56 56 41 57 49 8B F7 E8 00 FF FF FF 4A 8D 14 FD 78 03 00 00 41 5F 4C 8B 35 85 1A 45 00 4E 8B 34 32 4D 8B B6 A8 00 00 00 41 80 BE E5 03 00 00 00 74 05 BF 57 00 00 00 5E 41 5E 8B D7 48 8B CB E9 49 02 A6 01")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x1400100C3),//reset flag
                            ConvertByteString("48 8B 5C 24 40 84 C0 0F 85 5C 75 09 00 49 8B C6 49 8B D7 48 8B CE 48 8B 74 24 10 E8 A7 FE FF FF 48 8B F1 4A 8D 0C FD 78 03 00 00 44 8B 35 2B 1A 45 00 46 8B 34 31 45 8B B6 A8 00 00 00 41 C6 86 E5 03 00 00 00 4C 8B FA 4C 8B F0 E9 1B 75 09 00")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x14000FEE9),//immobile detection
                            ConvertByteString("C6 05 12 31 45 00 00 8D 47 01 89 46 28 31 DB 41 BE FF FF FF FF 45 31 FF EB 23 83 FB 01 75 08 41 BE 02 00 00 00 EB 16 83 FB 02 75 08 41 BF 01 00 00 00 EB 06 41 BF FF FF FF FF 45 31 F6 8B 56 0C 44 01 F2 44 8B 46 10 45 01 F8 44 8B CD 4F 8D 24 89 48 8B CE E8 6E 09 09 00 84 C0 74 06 FF 05 B6 30 45 00 FF C3 83 FB 04 7C B0 E8 32 00 00 00 83 3D A3 30 45 00 04 41 0F 94 C0 4A 8D 04 FD 78 03 00 00 44 8B 35 AE 1B 45 00 46 8B 34 30 45 8B B6 A8 00 00 00 45 88 86 E5 03 00 00 E9 49 CF 6B 02")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x14000FF8A),//find player
                            ConvertByteString("45 31 FF 44 8B 35 8C 1B 45 00 45 8B B6 78 03 00 00 45 8B B6 A8 00 00 00 45 8B B6 C8 03 00 00 49 39 F6 75 01 C3 44 8B 35 6A 1B 45 00 45 8B B6 80 03 00 00 45 85 F6 74 17 45 8B B6 A8 00 00 00 45 8B B6 C8 03 00 00 49 39 F6 75 05 41 83 C7 01 C3 44 8B 35 3F 1B 45 00 45 8B B6 88 03 00 00 45 85 F6 74 17 45 8B B6 A8 00 00 00 45 8B B6 C8 03 00 00 49 39 F6 75 05 41 83 C7 02 C3 44 8B 35 14 1B 45 00 45 8B B6 90 03 00 00 45 85 F6 74 04 41 83 C7 03 C3")
                        );
                    }
                }},
                {ColX, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1400A762C),
                        x
                            ? ConvertByteString("90 90")
                            : ConvertByteString("75 49")
                        )
                },
                {ColM, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1426D4A52),
                        x
                            ? ConvertByteString("90 90")
                            : ConvertByteString("75 25")
                        )
                },
                {PreserveRot, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x142730C9E),
                        x
                            ? ConvertByteString("90 90 90 90 90 90")
                            : ConvertByteString("FF 90 80 00 00 00")
                        )
                },
                {Lockout, x =>
                    Game.WriteInt32(
                        new IntPtr(0x142802821),
                        x
                            ? 0x30
                            : 0x14
                        )
                },
                {FullTmini, x =>
                    Game.WriteByte(
                        new IntPtr(0x14280D08F),
                        (byte)(x
                                ? 0xE5  //changes what memory location the t spin flag is being set to
                                : 0xE6  //so instead of setting T Mini to yes it sets T Full to yes
                            )
                        )
                },
                {NoT, x => {
                    Game.WriteByte(new IntPtr(0x14280D093), Convert.ToByte(!x)); //changes the value being written to the t spin flags to be 0
                    Game.WriteByte(new IntPtr(0x14280D09C), Convert.ToByte(!x)); //so it never registers a t spin
                }},     //I wanted to just make the game always skip t spin detection but it would conflict with All Spins
                {AllT, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1400A4200),
                        x
                            ? ConvertByteString("90 90")
                            : ConvertByteString("74 21")
                        )
                }
            };

            DialScripts = new Dictionary<Dial, Action<int>>() {
                {GarbageRate, x =>
                    Game.WriteUInt16(new IntPtr(0x14044193C), (ushort)x)
                },
                {AllClearMultiplier, x => {
                    int? addr = Game.TraverseInt32(
                                    new IntPtr(0x140598A20),
                                    new int[] {0x38}
                               );

                    if (addr > 0x10000) {    //I'm not taking chances writing to bad memory, all pointer chains will do this
                        Game.WriteInt32((IntPtr)addr+0x284, x);
                    }

                }},
                {CleanGarbage, x =>
                    Game.WriteByte(new IntPtr(0x14032010F), (byte)x)
                },
                {GarbageFilled, x => {  //8 => -1
                    if (x == 8) {
                        x = -1;
                    }

                    Game.WriteInt32(new IntPtr(0x1426B71B1), x);
                }},
                {GarbageEmpty, x => {
                    if (x == 8) {
                        x = -1;
                    }

                    Game.WriteInt32(new IntPtr(0x1426B71BB), x);
                }},
                {RecieveT, x => {                               //In games with only one game type (tvt, pvp) these actually point to the same address
                    int? addr = Game.TraverseInt32(             //only in pvt and swap are they distinct
                                    new IntPtr(0x1405989C8),    //but I won't say that on the writeup on this since it doesn't really matter
                                    new int[] {0x18, 0xA8}
                                );

                    if (addr > 0x10000) {
                        Game.WriteByte((IntPtr)addr+0x1AC, (byte)x);
                    }
                }},
                {RecieveP, x => {
                    int? addr = Game.TraverseInt32(
                                    new IntPtr(0x1405989C8),
                                    new int[] {0x18, 0x18, 0xA8}
                                );

                    if (addr > 0x10000) {
                        Game.WriteByte((IntPtr)addr+0x1AC, (byte)x);
                    }
                }},
                {DAS, x =>
                    Game.WriteByte(new IntPtr(0x1413C8C52), (byte)(x + 1)) //DAS gets decremented the same frame it's set, so I increment x here to counter that
                }
            };

            TableScripts = new Dictionary<UniformGrid, Action<int, int>>() {
                {TvTAttackTable, (i, x) => {
                    int offset;

                    if (i < 4) {
                        offset = i + 0x24;
                    } else if (i < 9) {
                        offset = i + 0x16;
                    } else {
                        offset = i - 0xA;
                    }

                    Game.WriteByte(new IntPtr(0x1403200B5 + offset), (byte)x);
                }},
                {TvTComboTable, (i, x) => {
                    Game.WriteByte(new IntPtr(0x1403200BB + i), (byte)x);
                }},
                {TvPAttackTable, (i, x) => {
                    int offset;

                    if (i < 5) {
                        offset = i*2 + 0x26B;
                    } else if (i == 5) {
                        offset = 0x277;
                    } else {
                        offset = i - 0xA;
                    }

                    Game.WriteByte(new IntPtr(0x1404329C5 + offset), (byte)x);
                }},
                {TvPComboTable, (i, x) => {
                    Game.WriteByte(new IntPtr(0x140432C17 + i), (byte)x);
                }},
                {PvPChainTable, (i, x) => {
                    Game.WriteUInt16(new IntPtr(0x14031DAC0 + i*2), (ushort)x);
                }},
                {DelayTable, (i, x) => {
                    if (i == 0) {
                        Game.WriteByte(new IntPtr(0x142724DFC), (byte)x);
                    } else {
                        Game.WriteByte(new IntPtr(0x1427E453B + i*7), (byte)x);
                    }
                }}
            };
        }

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

        private bool FreezeDial = true;

        private void DialHandle(Dial source, double value) {
            if (FreezeDial) return;

            if (source.Parent is UniformGrid grid) {
                int index = grid.Children.IndexOf(source);

                TableScripts[grid].Invoke(
                    (index / grid.Columns - 1) * (grid.Columns - 1) + (index % grid.Columns - 1),
                    (int)value
                );

            } else DialScripts[source].Invoke((int)value);
        }
    }
}
