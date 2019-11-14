using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox {
    public partial class UI {
        public UI() {
            InitializeComponent();

            Version.Text = $"PPT-Sandbox-{Assembly.GetExecutingAssembly().GetName().Version.Minor} by {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName}";

            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
                case "ko":
                    break;
                    
                case "ja":
                    break;
                    
                default:
                    General.Text = "General";
                    PentominoVersus.Content = "Pentomino Versus";
                    RemoveLineClearDelay.Content = "Remove Line Clear Delay";
                    InfiniteHold.Content = "Infinite Hold";
                    SecretGradeGarbage.Content = "Secret Grade Garbage";

                    AutoLocking.Text = "Piece Auto-Locking";
                    RemoveAutoLock.Content = "Disable Auto-Locking";
                    TGMAutoLock.Content = "TGM Auto-Locking";

                    TetrisB2B.Text = "Back-to-Back Tetris";
                    TetrisB2BDouble.Content = "Tetris B2B doubles attack";
                    TetrisB2BAdd2.Content = "Tetris B2B adds 2 attack";

                    TspinB2B.Text = "Back-to-Back T-Spin";
                    TspinB2BDouble.Content = "T-Spin B2B doubles attack";
                    TspinB2BAdd2.Content = "T-Spin B2B adds 2 attack";

                    GarbageGeneration.Text = "Garbage Generation";
                    CleanGarbage.Title = "Clean Garbage Chance";
                    GarbageFilled.Title = "Filled Garbage Tile";
                    GarbageEmpty.Title = "Empty Garbage Tile";
                    break;
            }
        }
    }
}
