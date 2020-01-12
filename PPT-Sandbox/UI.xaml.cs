using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

namespace Sandbox {
    public partial class UI {
        private ProcessMemory Game = new ProcessMemory("puyopuyotetris", false);

        private Dictionary<Control, Action<bool>> Scripts;
        private Dictionary<Dial, Action<int>> DialScripts;
        private Dictionary<UniformGrid, Action<int, int>> TableScripts;
        private List<object> EncodingList;

        static readonly int Version = Assembly.GetExecutingAssembly().GetName().Version.Minor;

        private byte[] ConvertByteString(string bytes) =>
            bytes.Split(' ').Select(i => Convert.ToByte(i, 16)).ToArray();

        public UI() {
            InitializeComponent();
            FreezeDial = false;

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue)); //tooltips no longer yeet after 5 seconds

            string appendix = 
            #if PRERELEASE
                "-prerelease"
            #else
                ""
            #endif
            ;

            VersionText.Text = $"PPT-Sandbox-{Version}{appendix} by {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName}";

            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
                case "ko":
                    ScriptsHeader.Header = "      스크립트";
                        Script1.Header = "     페이지 1";
                            General.Text = "보통";
                                PentominoVersus.Content = "펜토미노 대전";
                                    PentominoVersus.ToolTip = "모든 미노(블록)들을 펜토미노로 바꿉니다.";
                                RemoveLineClearDelay.Content = "줄 지울때 딜레이 삭제";
                                UndoHold.Content = "무한 홀드";
                                    UndoHold.ToolTip = "하드 드롭하기 전까지는 몇 번이든 홀드를 할 수 있습니다.";
                                FreezeSwap.Content = "스왑 타이머 정지";
                                ColX.Content = "좌/우 이동범위 제거";
                                    ColX.ToolTip = "미노들을 이동할때 좌우 범위 제한을 제거합니다.";
                                ColM.Content = "미노 겹치기";
                                    ColM.ToolTip = "블록을 놓을 때 겹처서 놓을 수 있게 됩니다.";
                                Lockout.Content = "미노 높이제한 제거";
                                    Lockout.ToolTip = "20라인 이상 위에 놓아도 게임오버 되지 않습니다.";
                                Invisible.Content = "투명 모드";
                                    Invisible.ToolTip = "쓰레기줄을 제외하고 모든 미노가 보이지 않습니다.";
                                Input.Content = "향상된 입력 모드";
                                    Input.ToolTip = "기본적으로, 뿌요뿌요 테트리스는 하드 드롭하지 않으면 [회전-> 이동-> 중력] 순서대로 입력을 처리하고, 그렇지 않으면 [회전-> 하드 드롭] 순서로 입력을 처리합니다.\n" +
                                                    "이 옵션을 키면, 항상 [이동 -> 회전 -> (중력 / 하드드롭] 순으로 처리하여 입력 수를 줄이고 부드러운 입력을 보여줍니다.\n" +
                                                    "홀드는 항상 다른 것보다 먼저 처리됩니다.";

                                AutoLocking.Text = "블록 자동 놓기";
                                    RemoveAutoLock.Content = "블록 자동 놓기 해제";
                                    TGMAutoLock.Content = "아키라 스타일 자동 놓기";
                                        TGMAutoLock.ToolTip = "자동 놓기 타이머는 아래 새 층에 놓았을때만 재설정됩니다.\n" +
                                                      "일반적으로 자동 놓기 타이머는 회전 & 이동시에도 재설정됩니다.";

                    Script2.Header = "페이지 2";
                        Harddrop.Text = "하드드롭 변형";
                            Float.Content = "떠있는 상태로 놓기";
                                Float.ToolTip = "블록을 아래 고스트 위치에 놓지 않고 현재 위치에 놓습니다.";
                            Sonicdrop.Content = "소닉드롭";
                                Sonicdrop.ToolTip = "하드드롭과 유사하나, 소프트 드랍처럼 완전히 놓기 전까지 시간이 있습니다.";
                            Sink.Content = "미노 겹치기";
                                Sink.ToolTip = "아래 있는 블록을 무시하고 블록을 놓습니다.";
                            Up.Content = "하드드랍 -> 위로 올리기";
                                Up.ToolTip = "하드드랍 키를 누를 시 블록이 1칸 위로 이동합니다.";
                            Noghost.Content = "고스트 모양 삭제";

                            PreserveRot.Content = "홀드 시 블록의 회전상태 유지";
                                PreserveRot.ToolTip = "홀드 시 블록 회전 저장";
                            Unhold.Content = "홀드 시 블록의 위치 유지";
                                Unhold.ToolTip = "홀드 시 블록 위치 저장";

                            Lockoutdial.Content = "블록 최대높이";
                                Lockoutdial.ToolTip = "이 높이 위로 블록을 놓을 시 게임오버됩니다.";

                        Script3.Header = "회전 시스템";
                            RotationSystems.Text = "킥 테이블 시스템";
                                Ascension.Content = "Ascension";
                                    Ascension.ToolTip = "Ascension의 회전 시스템입니다.\n" +
                                                        "asc.winternebs.com";
                                Cultris2.Content = "Cultris II";
                                    Cultris2.ToolTip = "Cultris II의 회전 시스템입니다.\n" +
                                                       "gewaltig.net/cultris2.aspx";
                                h.Content = "h";
                                    h.ToolTip = "블록의 맞닿음을 무시하고 회전합니다.";
                                BONKERS.Content = "B.O.N.K.E.R.S.";
                                    BONKERS.ToolTip = "기본 회전에 실패하면 미노가 매트릭스의 맨 아래로 걷어차면서 오른쪽 타일 하나를 확인하고 왼쪽 타일 하나를 끝까지 확인합니다.\n" +
                                                      "자세한 것은 https://tetris.wiki/A_Gnowius%27_Challenge#Biased-On-Nutty-Kick-Enhancement_Rotation_System 을 참고하세요.";
                                jstrismeme.Content = "Jstris 밈 회전 시스템";
                                    jstrismeme.ToolTip = "Jstris의 O-스핀 설정에 사용되는 킥테이블 시스템입니다.\n" +
                                                         "O블록의 변환은 포함되지 않습니다.";
                                thenew.Content = "The New Tetris";
                                    thenew.ToolTip = "닌텐도 64의 \"The New Tetris\" 회전 시스템입니다.\n" +
                                                     "The New Tetris 회전상태 코드와 함께 사용하면 좋습니다.";
                                jstris.Content = "Jstris 180 SRS";
                                    jstris.ToolTip = "180도 회전시 Jstris의 킥 시스템이 적용됩니다.\n" +
                                                     "오프라인 전용 탭에서 180도 회전을 활성화하세요.";
                                nullpo.Content = "Nullpomino 180 SRS";
                                    nullpo.ToolTip = "180도 회전시 Nullpomino의 킥 시스템이 적용됩니다.\n" +
                                                     "오프라인 전용 탭에서 180도 회전을 활성화하세요.";

                            RotationStates.Text = "회전 상태 변환";
                                ars.Content = "아리카 회전 시스템 (ARS)";
                                thenewrs.Content = thenew.Content;

                        Offline.Header = "오프라인 전용";
                            Offline.ToolTip = "퍼즐 리그나 클럽/프리플레이에는 적용되지 않습니다.";

                        Rotate.Text = "추가 설정";
                            Rotate.ToolTip = "'세번째' 오른쪽 회전 키를 입력하면 다음 효과가 발동됩니다.\n" +
                                             "[내 정보 (Options&Data)-> 옵션(Settings)->키보드&마우스 조작 설정에서 확인]";
                            DoubleRotate.Content = "180도 회전";
                                DoubleRotate.ToolTip = "블록을 180도 회전시킵니다.";
                            Cycle.Content = "블록 바꾸기";
                                Cycle.ToolTip = "S -> Z -> J -> L -> T -> O -> I -> 모노미노 [1x1블럭] -> S 순으로 미노가 바뀝니다.\n" +
                                                "이 동작은 회전으로 계산되며 변환하는 동안 블럭의 킥 처리도 이루어집니다. [-스핀류 가능]";
                            Flip.Content = "블럭 뒤집기";
                                Flip.ToolTip = "블럭을 가로 방향으로 뒤집습니다.\n" +
                                               "S <-> Z, J <-> L: 0도 회전으로 계산합니다.\n" +
                                               "T, O,  I는 정상적인 180도 회전으로 될 것입니다.\n" +
                                               "SRS의 특성상 커스텀 회전 시스템이 권장됩니다.";
                            Flip180.Content = "블럭 뒤집기(180)";
                                Flip180.ToolTip = "\"블럭 뒤집기\"에 대한 조금 다른 결과를 제공합니다. 자세한 내용은 \"블럭 뒤집기\" 툴팁 설명을 참조하세요.";
                            bigsoftdrop.Content = "슈퍼 소프트 드랍";
                                bigsoftdrop.ToolTip = "아래 있는 블럭을 무시하고 1줄씩 드랍합니다.";
                            ARR.Content = "즉시 이동";
                                ARR.ToolTip = "DAS가 충전되면 블럭이 수평으로 가능한 한 멀리 이동합니다.";

                    AttackHeader.Header = "공격력";
                        TetrisVsTetris.Header = "     테트리스 VS 테트리스";
                            Attacks.Text = "공격";

                            Single.Text = "싱글";
                            Double.Text = "더블";
                            Triple.Text = "트리플";
                            Tetris.Text = "테트리스";
                            TetrisPlus.Text = "테트리스 +";

                            Clear.Text = "라인 클리어";
                            PerfectClear.Text = "퍼클";
                            TSpin.Text = "T-스핀";

                            Combo.Text = "콤보/렌";

                        TetrisVsPuyo.Header = "테트리스 VS 뿌요뿌요";
                            TvPAttacks.Text = "공격";

                            TvPSingle.Text = "싱글";
                            TvPDouble.Text = "더블";
                            TvPTriple.Text = "트리플";
                            TvPTetris.Text = "테트리스";
                            TvPTetrisPlus.Text = "테트리스 +";

                            TvPClear.Text = "라인 클리어";
                            TvPPerfectClear.Text = "퍼클";
                            TvPTSpin.Text = "T-스핀";

                            TvPCombo.Text = "콤보/렌";

                        Puyo.Header = "뿌요뿌요";
                            GarbageRate.Title = "방해뿌요 배율";
                            AllClearMultiplier.Title = "올 클리어 배율";

                            Chain.Text = "연쇄";

                        MarginTime.Header = "마진 타임";
                            Margin.Text = "값";
                                Margin.ToolTip = "테트리스 VS 테트리스에서는 적용되지 않습니다.";

                    GarbageHeader.Header = "방해 줄 / 방해 뿌요";
                        Detection.Header = "     판정";
                            TetrisB2B.Text = "백투백 테트리스";
                                TetrisB2BDouble.Content = "백투백 테트리스시 2배로 공격";
                                TetrisB2BAdd2.Content = "백투백 테트리스시 2줄 추가 공격";
                                TetrisB2BCum.Content = "백투백 테트리스 스택 적용";
                                    TetrisB2BCum.ToolTip = "백투백을 2번 할 때마다 공격력이 1씩 증가합니다. (끊김없이)";

                            TspinB2B.Text = "백투백 T-스핀";
                                TspinB2BDouble.Content = "백투백 T-스핀시 2배로 공격";
                                TspinB2BAdd2.Content = "백투백 T-스핀시 2줄 추가 공격";
                                TspinB2BCum.Content = "백투백 T-스핀 스택 적용";
                                    TspinB2BCum.ToolTip = "백투백을 2번 할 때마다 공격력이 1씩 증가합니다. (끊김없이)";

                            TspinDetection.Text = "T-스핀 판정";
                                FullTmini.Content = "모든 T-스핀 미니를 일반 T-스핀으로 처리";
                                NoT.Content = "T-스핀 없음";
                                AllT.Content = "모든 스핀 -> T-스핀 판정";

                        Behavior.Header = "적용";
                            GarbageGeneration.Text = "방해 줄 생성";
                            CleanGarbage.Title = "클린한 방해줄 확률";
                                CleanGarbage.ToolTip = "일렬로 방해줄이 생성될 확룰을 퍼센트로 나타냅니다.";
                            GarbageFilled.Title = "방해줄 타일";
                                GarbageFilled.ToolTip = "8로 세팅하면 비워집니다.";
                            GarbageEmpty.Title = "빈 줄 타일";
                                GarbageEmpty.ToolTip = "8로 세팅하면 비워집니다.";
                            ReceiveT.Title = "최대 테트리스 방해줄";
                                ReceiveT.ToolTip = "드랍할때 한번에 방해줄이 올라오는 줄 수.\n" +
                                                   "테트리스 VS 테트리스에서는 다른 스크립트 없이는 적용되지 않습니다.";
                            ReceiveP.Title = "최대 방해뿌요";
                                ReceiveP.ToolTip = "뿌요를 놓을 때 내려오는 최대 방해뿌요 갯수";
                            ReceiveCap.Content = "테트리스 VS 테트리스 방해줄 적용";
                                ReceiveCap.ToolTip = "\"최대 테트리스 방해줄\" 설정을 테트리스 VS 테트리스 게임에서 적용되게 해 줍니다.";
                            GarbageModification.Text = "방해뿌요/줄 계산";
                            SecretGradeGarbage.Content = "지그재그 방해줄";
                                SecretGradeGarbage.ToolTip = "방해줄이 1줄마다 1칸씩 이동하며 생성됩니다.";
                            GarbageBlocking.Content = "방해줄 차단";
                            UnCappedPC.Content = "퍼펙트 클리어 대미지 추가 적용";
                                UnCappedPC.ToolTip = "퍼펙트 클리어 데미지가 고정적용에서 추가적용으로 변경됩니다.\n" +
                                                     "예시: 본 코드가 활성화될시, 테트리스 퍼클은 10(퍼클) + 4(테트리스) = 14줄을 보냅니다.\n" +
                                                     "본 코드가 비활성화될시, 테트리스 퍼클은 10(퍼클) 줄만 보냅니다.\n" +
                                                     "퍼펙트 클리어는 별도로 라인 클리어 딜레이가 적용됩니다.";
                            ColorClear.Content = "컬러 클리어";
                                ColorClear.ToolTip = "방해줄을 제외하고 퍼펙트 클리어를 하면 \"세미-퍼펙트 클리어\" 판정을 내려서 별도로 방해줄 5개를 보냅니다.\n" +
                                                     "별도로 방해줄을 보내는 것데 대한 의미는  \"퍼펙트 클리어 대미지 추가 적용\" 툴팁을 참고하십시오.";
                            AllSpin.Content = "올-스핀";
                                AllSpin.ToolTip = "T-스핀 미니/미동 탐지가 없는 올 스핀 판정을 추가합니다.";

                    OtherHeader.Header = "그 외";
                        Timing.Header = "     타이밍";
                            Delays.Text = "라인 딜레이";
                                DelayBase.Text = "기본";
                                    DelayBase.ToolTip = "줄을 지울때 다음과 같이 프레임 지연이 발생하고, 한번에 몇 줄이 지워졌는지에 따른 추가 지연이 있습니다.\n" +
                                                        "방해줄/방해뿌요를 보낼 때도 프레임 지연이 있습니다. 이 값이 0이면 방해줄/방해뿌요가 줄을 지움과 동시에 전송됩니다.";
                                DelaySingle.Text = "싱글";
                                    DelaySingle.ToolTip = "1줄을 지웠을 때 추가로 적용되는 딜레이입니다.";
                                DelayDouble.Text = "더블";
                                    DelayDouble.ToolTip = "2줄을 지웠을 때 추가로 적용되는 딜레이입니다.";
                                DelayTriple.Text = "트리플";
                                    DelayTriple.ToolTip = "3줄을 지웠을 때 추가로 적용되는 딜레이입니다.";
                                DelayTetris.Text = "테트리스";
                                    DelayTetris.ToolTip = "4줄을 지웠을 때 추가로 적용되는 딜레이입니다.";
                                DelayTetrisPlus.Text = "테트리스 +";
                                    DelayTetrisPlus.ToolTip = "5줄 이상 지웠을 때 추가로 적용되는 딜레이입니다. (테트리스 플러스 등)";
                                DAS.Title = "DAS";
                                    DAS.ToolTip = "ARR이 활성화되기 전의 프레임.";
                                Autolockdial.Title = "자동 드랍 타이머";
                                    Autolockdial.ToolTip = "블록이 아래에 닿은 후 자동 드랍되기까지의 프레임을 나타낸 값입니다.";

                        Gravity.Header = "중력";
                            Level.Text = "레벨";
                            GravityExplanation.Text = "값이 높을수록 보드의 맨 아래에 도달하는 데 걸리는 시간이 줄어듭니다. Sweet (달콤) / Mild (순함) / Normal (보통) / Hot (매콤) / Spicy (강렬) 핸디캡은 특정 레벨에 할당됩니다.\n" +
                                                      "달콤은 레벨 1, 순함은 레벨 2, 보통은 레벨 3, 매콤은 레벨 5, 그리고 강렬은 레벨 7입니다.\n" +
                                                      "스프린트 및 울트라와 같은 도전/챌린지 모드는 레벨 1로 계산됩니다.";
                        Softdrop.Title = "소프트드랍 중력 배율";
                            Softdrop.ToolTip = "기본적으로 소프트 드랍시 중력은 기본 중력 값에 20을 곱합니다. 여기서 이 값을 변경할 수 있습니다.";

                    ResetButton.Content = "초기화";
                    SaveButton.Content = "저장";
                    LoadButton.Content = "열기";

                    ofd = new OpenFileDialog() {
                        Multiselect = false,
                        Filter = "PPT-Sandbox 파일|*.pts|모든 파일|*.*",
                        Title = "PPT-Sandbox 파일 열기"
                    };

                    sfd = new SaveFileDialog() {
                        Filter = "PPT-Sandbox 파일|*.pts|모든 파일|*.*",
                        Title = "PPT-Sandbox 파일 저장하기"
                    };
                    break;

                //case "ja":
                //    break;

                default:
                    ScriptsHeader.Header = "      SCRIPTS";
                        Script1.Header = "     Page 1";
                            General.Text = "General";
                                PentominoVersus.Content = "Pentomino Versus";
                                    PentominoVersus.ToolTip = "Turns Tetrominos into their Pentomino counterparts.";
                                RemoveLineClearDelay.Content = "Remove Line Clear Delay";
                                UndoHold.Content = "Undo Hold";
                                    UndoHold.ToolTip = "Allows you to hold as much as you like, without the need to harddrop inbetween.";
                                FreezeSwap.Content = "Freeze Swap Timer";
                                ColX.Content = "Remove Left/Right Collision";
                                    ColX.ToolTip = "Piece will ignore collision on the X axis.";
                                ColM.Content = "Remove Mino Collision";
                                    ColM.ToolTip = "Piece will ignore all collision with other minos inside the matrix.";
                                Lockout.Content = "Remove Lock-Out Death";
                                    Lockout.ToolTip = "Placing a piece above row 20 will no longer cause a game over.";
                                Invisible.Content = "Invisible Matrix";
                                    Invisible.ToolTip = "All mino types, except garbage, are hidden from view.";
                                Input.Content = "Better Input Handling";
                                    Input.ToolTip = "By default, PPT processes inputs in the order of [Rotation -> Movement -> Gravity] if you don't harddrop, and [Rotation -> Harddrop] if you do.\n" +
                                                    "With this, it now always does [Movement -> Rotate -> (Gravity / Harddrop)], leading to a smoother experience and less \"dropped\" inputs.\n" +
                                                    "Hold is always processed first, before anything else.";

                            AutoLocking.Text = "Piece Auto-Locking";
                                RemoveAutoLock.Content = "Disable Auto-Locking";
                                TGMAutoLock.Content = "Arika Style Auto-Locking";
                                    TGMAutoLock.ToolTip = "Auto-Lock timer is only reset upon reaching a new lowest height.\n" +
                                                          "Normally, Auto-Lock timer is reset upon rotate and movement as well.";

                        Script2.Header = "Page 2";
                            Harddrop.Text = "Hard-Drop Modification";
                                Float.Content = "Floating Lock";
                                    Float.ToolTip = "Piece locks at its current position rather than at the ghost position.";
                                Sonicdrop.Content = "Sonic Drop";
                                    Sonicdrop.ToolTip = "Piece drops as far as possible, but doesn't lock. Soft dropping while on the ground will now lock the piece.";
                                Sink.Content = "Piece Sinking";
                                    Sink.ToolTip = "Piece locks one tile below ghost on hard drop.";
                                Up.Content = "Hard Drop Goes Up";
                                    Up.ToolTip = "Pressing Harddrop now makes your piece move 1 tile up, and can no longer lock your piece.\n" +
                                                 "Auto-Lock is now the only way to place a piece.";
                            Noghost.Content = "Remove Ghost";
                            
                            PreserveRot.Content = "Preserve Rotation on Hold";
                                PreserveRot.ToolTip = "Rotation doesn't reset on hold";
                            Unhold.Content = "Preserve Position on Hold";
                                Unhold.ToolTip = "Position doesn't reset on hold.";
                            Lockoutdial.Title = "Lock Out Height";
                                Lockoutdial.ToolTip = "Height at which locking a piece at will cause a game over.";

                        Script3.Header = "Rotation System";
                            RotationSystems.Text = "Kick Table";
                                Ascension.Content = "Ascension";
                                    Ascension.ToolTip = "Rotation system used by Ascension.\n" +
                                                        "asc.winternebs.com";
                                Cultris2.Content = "Cultris II";
                                    Cultris2.ToolTip = "Rotation system used by Cultris II.\n" +
                                                       "gewaltig.net/cultris2.aspx";
                                h.Content = "h";
                                    h.ToolTip = "Piece always rotates, even if it collides.";
                                BONKERS.Content = "B.O.N.K.E.R.S.";
                                    BONKERS.ToolTip = "If initial rotate fails, piece kicks to the bottom of the matrix and checks right one tile, left one tile, all the way up.";
                                jstrismeme.Content = "Jstris meme RS";
                                    jstrismeme.ToolTip = "Kick table used by the \"O-Spin\" setting on Jstris.\n" +
                                                         "Does not include O piece transformations.";
                                thenew.Content = "The New Tetris";
                                    thenew.ToolTip = "Rotation system used by The New Tetris on N64.\n" +
                                                     "Best used with The New Tetris rotation state code.";
                                jstris.Content = "Jstris 180 SRS";
                                    jstris.ToolTip = "180 degree rotates now use Jstris kicks.\n" +
                                                     "Activate 180 rotations in the Offline Only tab.";
                                nullpo.Content = "Nullpomino 180 SRS";
                                    nullpo.ToolTip = "180 degree rotates now use Nullpomino kicks.\n" +
                                                     "Activate 180 rotations in the Offline Only tab.";

                            RotationStates.Text = "Rotation States";
                                ars.Content = "ARS";
                                thenewrs.Content = thenew.Content;

                        Offline.Header = "Offline Only";
                            Offline.ToolTip = "None of these work in Puzzle League or Free Play";
                            Rotate.Text = "Extra Bind";
                                Rotate.ToolTip = "Use the third Rotate Right key to activate these effects.";
                                DoubleRotate.Content = "180 Rotations";
                                    DoubleRotate.ToolTip = "Piece rotates 180 degrees rather than 90.";
                                Cycle.Content = "Piece Cycling";
                                    Cycle.ToolTip = "Piece will Cycle once, from S -> Z -> J -> L -> T -> O -> I -> monomino -> S.\n" +
                                                    "This action counts as a rotation, and the piece will kick as such during transformation.";
                                Flip.Content = "Piece Flipping";
                                    Flip.ToolTip = "Piece will \"flip\" horizontally.\n" +
                                                   "S <-> Z, J <-> L, counts as 0 degree rotate.\n" +
                                                   "T, O, I will do a normal 180 rotate.\n" +
                                                   "Due to the nature of SRS, Custom RS is recommended along with this code.";
                                Flip180.Content = "Piece Flipping (180)";
                                    Flip180.ToolTip = "A different approach to piece flipping, that provides slightly different results than the basic version.\n" +
                                                      "See " + Flip.Content + " ToolTip for more info.";
                                bigsoftdrop.Content = "Super Soft Drop";
                                    bigsoftdrop.ToolTip = "Moves your piece one tile down, ignoring all collision.";
                        ARR.Content = "Instant ARR";
                            ARR.ToolTip = "Piece travels as far as it can horizontally once DAS is charged.";
                    
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

                        MarginTime.Header = "Margin Time";
                            Margin.Text = "Value";
                                Margin.ToolTip = "Has no effect on Tetris vs Tetris games.";

                    GarbageHeader.Header = "GARBAGE";
                        Detection.Header = "     Detection";
                            TetrisB2B.Text = "Back-to-Back Tetris";
                                TetrisB2BDouble.Content = "Tetris B2B doubles attack";
                                TetrisB2BAdd2.Content = "Tetris B2B adds 2 attack";
                                TetrisB2BCum.Content = "Tetris B2B stacks";
                                    TetrisB2BCum.ToolTip = "Garbage sent is increased by 1 for every two Back to Backs done without breaking.";

                            TspinB2B.Text = "Back-to-Back T-Spin";
                                TspinB2BDouble.Content = "T-Spin B2B doubles attack";
                                TspinB2BAdd2.Content = "T-Spin B2B adds 2 attack";
                                TspinB2BCum.Content = "T-Spin B2B stacks";
                                    TspinB2BCum.ToolTip = "Garbage sent is increased by 1 for every two Back to Backs done without breaking.";

                            TspinDetection.Text = "T-Spin Detection";
                                FullTmini.Content = "All T-Spin Mini's are full";
                                NoT.Content = "No T-Spins";
                                AllT.Content = "Every Spin is a T-Spin";

                        Behavior.Header = "Behavior";
                            GarbageGeneration.Text = "Garbage Generation";
                                CleanGarbage.Title = "Clean Garbage Chance";
                                    CleanGarbage.ToolTip = "Percent chance that garbage will stay in the same column.";
                                GarbageFilled.Title = "Filled Garbage Tile";
                                    GarbageFilled.ToolTip = "Set to 8 for empty.";
                                GarbageEmpty.Title = "Empty Garbage Tile";
                                    GarbageEmpty.ToolTip = GarbageFilled.ToolTip;
                                ReceiveT.Title = "Tetris Max Receival";
                                    ReceiveT.ToolTip = "Max garbage lines your matrix gets at once when your piece locks.\n" +
                                                       "Does not work in Tetris vs Tetris games without another script.";
                                ReceiveP.Title = "Puyo Max Receival";
                                    ReceiveP.ToolTip = "Max nuisance your board gets at once when you place a puyo";
                                ReceiveCap.Content = "Tetris vs Tetris Garbage Capping";
                                    ReceiveCap.ToolTip = "Allows " + ReceiveT.Title + " to work in Tetris vs Tetris games.\n";

                            GarbageModification.Text = "Garbage Modification";
                                SecretGradeGarbage.Content = "Secret Grade Garbage";
                                    SecretGradeGarbage.ToolTip = "Garbage forms a snaking pattern instead of a random one.";
                                GarbageBlocking.Content = "Garbage Blocking";
                                UnCappedPC.Content = "Remove Perfect Clear Damage Cap";
                                    UnCappedPC.ToolTip = "Perfect Clears are no longer capped at 10 garbage.\n" +
                                                         "Example: With code active, a Tetris PC sends 10 (PC) + 4 (Tetris) = 14 garbage lines.\n" +
                                                         "Without the code active, a Tetris PC sends 10 (PC) garbage lines.\n" +
                                                         "Results in Perfect Clears having line delay";
                                ColorClear.Content = "Color Clear";
                                    ColorClear.ToolTip = "If you clear every non-garbage mino from your matrix, you do a pseudo-Perfect Clear, sending 5 garbage with no cap.\n" +
                                                         "See Remove Perfect Clear Damage Cap tooltip for more info on what it means to not be capped.";
                                AllSpin.Content = "All Spins";
                                    AllSpin.ToolTip = "Adds All-Spin ruling with immobile detection and no mini penalty.\n" +
                                                      "Regular T-Spin detection is still present so Fin-TSD still works.";

                    OtherHeader.Header = "OTHER";
                        Timing.Header = "     Timings";
                            Delays.Text = "Line Delay";
                                DelayBase.Text = "Base";
                                    DelayBase.ToolTip = "Every line clear has this much delay in frames, plus the extra delay based on how many were cleared at once.\n" +
                                                        "Also is the garbage send delay, so if this is 0, garbage gets sent instantly upon line clear.";
                                DelaySingle.Text = "Single";
                                    DelaySingle.ToolTip = "Extra line delay added after 1 line is cleared.";
                                DelayDouble.Text = "Double";
                                    DelayDouble.ToolTip = "Extra line delay added after 2 lines are cleared.";
                                DelayTriple.Text = "Triple";
                                    DelayTriple.ToolTip = "Extra line delay added after 3 lines are cleared.";
                                DelayTetris.Text = "Tetris";
                                    DelayTetris.ToolTip = "Extra line delay added after 4 lines are cleared.";
                                DelayTetrisPlus.Text = "Tetris Plus";
                                    DelayTetrisPlus.ToolTip = "Extra line delay added after more than 4 lines are cleared.";
                                DAS.Title = "DAS";
                                    DAS.ToolTip = "Frames before ARR activates.";
                                Autolockdial.Title = "Auto-Lock Timer";
                                    Autolockdial.ToolTip = "Frames before a piece locks after touching the ground.";

                        Gravity.Header = "Gravity";
                            Level.Text = "Level";
                            GravityExplanation.Text = "The higher the value, the less time it takes to reach the bottom of the matrix. Sweet / Mild / Normal / Hot / Spicy handicaps are assigned to certain levels\n" +
                                                      "Sweet is level 1, Mild is level 2, Normal is level 3, Hot is level 5, and Spicy is level 7.\n" +
                                                      "Challenge Modes like Sprint and Ultra count as level 1.";
                            Softdrop.Title = "Softdrop Multiplier";
                                Softdrop.ToolTip = "By default, Softdropping multiplies your gravity value by 20. You can change this value here";

                    
                    ResetButton.Content = "Reset";
                    SaveButton.Content = "Save";
                    LoadButton.Content = "Load";

                    ofd = new OpenFileDialog() {
                        Multiselect = false,
                        Filter = "PPT-Sandbox Files|*.pts|All Files|*.*",
                        Title = "Open PPT-Sandbox File"
                    };
                    
                    sfd = new SaveFileDialog() {
                        Filter = "PPT-Sandbox Files|*.pts|All Files|*.*",
                        Title = "Save PPT-Sandbox File"
                    };
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
                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x1400A6CAA),
                            ConvertByteString("80 3D 52 C3 3B 00 01 75 31 C6 05 49 C3 3B 00 00 57 8B F8 83 E7 20 83 FF 20 5F 75 1E 41 8B 7E 30 8B BF C8 03 00 00 80 7F 18 03 74 07 BF 02 00 00 00 EB 7C BF FE FF FF FF EB 75 A8 10 74 71 FF CF EB 6D")
                        );
                        Game.WriteByteArray(
                            new IntPtr(0x140265F06),
                            ConvertByteString("80 25 F6 D0 1F 00 01 F6 84 19 68 01 00 00 80 0F 84 C3 03 00 00 38 4B 5E 0F 85 B8 03 00 00 C6 05 D8 D0 1F 00 01 E9 AC 03 00 00")
                        );
                    } else {
                        if (jstris.IsChecked == true) {
                            Scripts[jstris].Invoke(false);
                            jstris.IsChecked = false;
                        }
                        if (nullpo.IsChecked == true) {
                            Scripts[nullpo].Invoke(false);
                            nullpo.IsChecked = false;
                        }
                    }
                    Game.WriteByteArray(
                        new IntPtr(0x1400A6D53),
                        x
                            ? ConvertByteString("E9 52 FF FF FF 90")
                            : ConvertByteString("A8 10 74 02 FF CF")
                        );
                    Game.WriteByteArray(
                        new IntPtr(0x1402662D2),
                        x
                            ? ConvertByteString("E9 2F FC FF FF 90 90 90")
                            : ConvertByteString("F6 84 19 68 01 00 00 80")
                        );
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
                    Game.WriteByte(
                        new IntPtr(0x142804C8A),
                        (byte)(x
                            ? 0xEB
                            : 0x7C
                        )
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
                    Game.WriteByte(new IntPtr(0x14280D093), Convert.ToByte(!x));//changes the value being written to the t spin flags to be 0
                    Game.WriteByte(new IntPtr(0x14280D09C), Convert.ToByte(!x));//so it never registers a t spin
                }},                                                             //I wanted to just make the game always skip t spin detection but it would conflict with All Spins
                {AllT, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1400A4200),
                        x
                            ? ConvertByteString("90 90")
                            : ConvertByteString("74 21")
                        )
                },
                {Invisible, x =>
                    Game.WriteByte(
                        new IntPtr(0x140238F4D),
                        (byte)(x
                            ? 0x9
                            : 0x1
                        )
                    )
                },
                {ARR, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1400A7616),
                        x
                            ? ConvertByteString("E9 DB 00 00 00")
                            : ConvertByteString("48 8B 7C 24 58")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x1400A76F6),
                            ConvertByteString("84 C0 75 6F 45 00 BE 00 01 00 00 45 31 C0 44 8B 84 24 10 01 00 00 41 C1 E8 03 41 81 C0 78 03 00 00 45 31 D2 44 8B 15 FF A3 3B 00 47 8B 14 02 45 8B 92 B0 00 00 00 41 83 FF 01 75 09 41 80 7A 3D 00 75 30 EB 07 41 80 7A 3B 00 75 27 C6 05 BC B8 3B 00 01 8B 7E 18 44 8B CF 8B 5E 10 44 8B C3 41 0F B6 86 00 01 00 00 42 8D 14 38 48 8B CE FF 55 70 EB 8D 83 3D 95 B8 3B 00 01 75 1B C6 05 8C B8 3B 00 00 30 C0 45 28 BE 00 01 00 00 41 0F B6 BE 00 01 00 00 89 7E 0C 48 8B 7C 24 58 E9 84 FE FF FF")
                        );
                    }
                }},
                {Unhold, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1428524E0),
                        x
                            ? ConvertByteString("90 90 90")
                            : ConvertByteString("FF 50 10")
                        )
                },
                {Noghost, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1426C8D57),
                        x
                            ? ConvertByteString("90 90")
                            : ConvertByteString("74 e7")
                        )
                },
                {Float, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1426D0839),
                        x
                            ? ConvertByteString("90 90")
                            : ConvertByteString("74 E5")
                        )
                },
                {Sink, x =>
                    Game.WriteByteArray(
                        new IntPtr(0x1426D083B),
                        x
                            ? ConvertByteString("90 90 90")
                            : ConvertByteString("FF 43 10")
                        )
                },
                {Sonicdrop, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1400A70E0),
                        x
                            ? ConvertByteString("E9 84 00 00 00 90 90")
                            : ConvertByteString("74 05 45 84 FF 75 09")
                        );

                    Game.WriteByteArray(
                        new IntPtr(0x142853B25),
                        x
                            ? ConvertByteString("E9 3C B2 7B FD 90 90 90")
                            : ConvertByteString("66 83 B9 10 01 00 00 1E")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x14000ED66),
                            ConvertByteString("48 0F B6 81 24 01 00 00 48 83 E0 08 3C 08 75 09 48 31 C0 B8 01 00 00 00 C3 48 31 C0 66 83 B9 10 01 00 00 1E E9 9E 4D 84 02")
                        );
                    }
                }},
                {Cycle, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1400A6D53),
                        x
                            ? ConvertByteString("E9 52 FF FF FF 90")
                            : ConvertByteString("A8 10 74 02 FF CF")
                        );

                    Game.WriteByteArray(
                        new IntPtr(0x1402662D2),
                        x
                            ? ConvertByteString("E9 2F FC FF FF 90 90 90")
                            : ConvertByteString("F6 84 19 68 01 00 00 80")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140265F06),
                            ConvertByteString("F6 84 19 68 01 00 00 80 0F 84 CA 03 00 00 38 4B 5E 0F 85 BF 03 00 00 C6 05 DF D0 1F 00 01 E9 B3 03 00 00")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x1400A6CAA),
                            ConvertByteString("80 3D 52 C3 3B 00 01 75 2D C6 05 49 C3 3B 00 00 57 8B F8 83 E7 20 83 FF 20 5F 75 1A 41 8B 7E 30 8B BF C8 03 00 00 8B 47 08 FF C0 83 F8 07 0F 4F C5 89 47 08 EB 79 A8 10 74 75 FF CF EB 71")
                        );
                    }
                }},
                {Flip, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1400A6D4B),
                        x
                            ? ConvertByteString("E9 F6 F7 FF FF")
                            : ConvertByteString("BF 00 00 00 00")
                        );

                    Game.WriteByteArray(
                        new IntPtr(0x1402662D2),
                        x
                            ? ConvertByteString("E9 2F FC FF FF 90 90 90")
                            : ConvertByteString("F6 84 19 68 01 00 00 80")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140265F06),
                            ConvertByteString("F6 84 19 68 01 00 00 80 0F 84 CA 03 00 00 38 4B 5E 0F 85 BF 03 00 00 C6 05 DF D0 1F 00 01 E9 B3 03 00 00")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x1400A6546),
                            ConvertByteString("BF 00 00 00 00 0F 45 F9 80 3D AE CA 3B 00 01 0F 85 F8 07 00 00 C6 05 A1 CA 3B 00 00 57 8B F8 83 E7 20 83 FF 20 5F 0F 85 E1 07 00 00 41 8B 7E 30 8B BF C8 03 00 00 8B 47 08 83 F8 02 7D 08 83 F0 01 89 47 08 EB 3B 83 F8 04 7D 0E 83 E8 02 83 F0 01 83 C0 02 89 47 08 EB 28 41 8B 7E 30 8B BF C8 03 00 00 80 7F 18 03 74 07 BF 02 00 00 00 EB 05 BF FE FF FF FF E9 99 07 00 00 31 FF E9 92 07 00 00 31 FF 41 80 BE 25 01 00 00 00 0F 85 94 07 00 00 B8 00 08 00 00 66 41 89 86 1E 01 00 00 E9 AC")
                        );
                    }
                }},
                {Flip180, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1402662D2),
                        x
                            ? ConvertByteString("E9 2F FC FF FF 90 90 90")
                            : ConvertByteString("F6 84 19 68 01 00 00 80")
                        );

                    Game.WriteByteArray(
                        new IntPtr(0x1402662D2),
                        x
                            ? ConvertByteString("E9 2F FC FF FF 90 90 90")
                            : ConvertByteString("F6 84 19 68 01 00 00 80")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140265F06),
                            ConvertByteString("80 25 F6 D0 1F 00 01 F6 84 19 68 01 00 00 80 0F 84 C3 03 00 00 38 4B 5E 0F 85 B8 03 00 00 C6 05 D8 D0 1F 00 01 E9 AC 03 00 00")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x140265F06),
                            ConvertByteString("BF 00 00 00 00 0F 45 F9 80 3D AE CA 3B 00 01 0F 85 F8 07 00 00 C6 05 A1 CA 3B 00 00 57 8B F8 83 E7 20 83 FF 20 5F 0F 85 E1 07 00 00 41 8B 7E 30 8B BF C8 03 00 00 8B 47 08 83 F8 02 7D 08 83 F0 01 89 47 08 EB 11 83 F8 04 7D 0C 83 E8 02 83 F0 01 83 C0 02 89 47 08 41 8B 7E 30 8B BF C8 03 00 00 80 7F 18 03 74 07 BF 02 00 00 00 EB 05 BF FE FF FF FF E9 9B 07 00 00 31 FF E9 94 07 00 00 00 00 31 FF 41 80 BE 25 01 00 00 00 0F 85 94 07 00 00 B8 00 08 00 00 66 41 89 86 1E 01 00 00 E9 AC 07 00 00")
                        );
                    }
                }},
                {jstris, x => {
                    if (DoubleRotate.IsChecked == true || x == false) {
                        Game.WriteByte(
                            new IntPtr(0x1400A6CB9),
                            (byte)(x
                                ? 2
                                : 0
                            )
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x1426CCE8E),
                            x
                                ? ConvertByteString("E9 17 51 93 FD 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90")
                                : ConvertByteString("44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C")
                        );

                        if (x) {
                            Game.WriteByteArray(
                                new IntPtr(0x140462000),
                                ConvertByteString("01 FF")
                            );

                            Game.WriteByteArray(
                                new IntPtr(0x140001FAA),
                                ConvertByteString("83 3D 52 10 46 00 02 75 56 45 31 F6 45 31 FF 48 83 FF 01 0F 85 DF AE 6C 02 48 B9 E2 1F 00 40 01 00 00 00 8B D5 6B D2 0A 48 8D 14 11 48 B9 00 20 46 40 01 00 00 00 FF E2 44 0F BE 79 01 E9 B6 AE 6C 02 44 0F BE 71 01 E9 AC AE 6C 02 44 0F BE 39 90 E9 A2 AE 6C 02 44 0F BE 31 E9 99 AE 6C 02 44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C E9 80 AE 6C 02")
                            );
                        }
                    } else {
                        jstris.IsChecked = false;
                    }
                }},
                {jstrismeme, x => {
                    Game.WriteByte(
                        new IntPtr(0x1426CCEBE),
                        (byte)(x
                            ? 0x10
                            : 0x05
                        )
                    );

                    Game.WriteByteArray(
                        new IntPtr(0x1426CCE8E),
                        x
                            ? ConvertByteString("E9 17 51 93 FD 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90")
                            : ConvertByteString("44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140462000),
                            ConvertByteString("00 00 FF 00 01 00 00 FF FF FF 01 FF FE 00 02 00 00 FE FF FE 01 FE 00 01 FE FE 02 FE FD FD 03 FD")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x140001FAA),
                            ConvertByteString("48 8B D3 48 B9 00 20 46 40 01 00 00 00 44 0F BE 34 51 44 0F BE 7C 51 01 83 7C 24 58 01 0F 84 D5 AE 6C 02 41 F7 DE E9 CD AE 6C 02")
                        );
                    }
                }},
                {ReceiveCap, x =>
                   Game.WriteByteArray(
                       new IntPtr(0x1427F6E11),
                       x
                           ? ConvertByteString("90 90 90")
                           : ConvertByteString("41 89 C6")
                       )
                },
                {Up, x => {
                    Game.WriteByteArray(
                       new IntPtr(0x14285F7F2),
                       x
                           ? ConvertByteString("E9 55 67 A0 FD 90 90 90")
                           : ConvertByteString("66 83 8B 98 00 00 00 04")
                       );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140265F4C),
                            ConvertByteString("50 48 8B 44 24 38 48 8B 80 C8 03 00 00 83 78 10 23 73 03 FE 40 10 58 FE 8B 01 01 00 00 E9 89 98 5F 02")
                        );
                    }
                }},
                {bigsoftdrop, x => {
                    Game.WriteByteArray(
                       new IntPtr(0x14285F826),
                       x
                           ? ConvertByteString("E9 C5 00 00 00 90 90 90")
                           : ConvertByteString("66 83 8B 98 00 00 00 20")
                       );

                    Game.WriteByteArray(
                       new IntPtr(0x140266101),
                       x
                           ? ConvertByteString("E9 0A FF FF FF")
                           : ConvertByteString("3D 1E 00 07 80")
                       );

                    Game.WriteByteArray(
                       new IntPtr(0x1402662D2),
                       x
                           ? ConvertByteString("E9 2F FC FF FF 90 90 90")
                           : ConvertByteString("F6 84 19 68 01 00 00 80")
                       );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140266010),
                            ConvertByteString("C7 05 E9 CF 1F 00 00 00 00 00 3D 1E 00 07 80 E9 E2 00 00 00")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x140265F06),
                            ConvertByteString("F6 84 19 68 01 00 00 80 0F 84 CA 03 00 00 38 4B 5E 0F 85 BF 03 00 00 C6 05 DF D0 1F 00 01 E9 B3 03 00 00")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x14285F8F0),
                            ConvertByteString("80 3D 0C 37 C0 FD 01 75 2B C6 05 03 37 C0 FD 00 48 8B 4C 24 30 48 8B 89 C8 03 00 00 83 79 10 00 0F 84 18 FF FF FF FF 49 10 FF 83 01 01 00 00 E9 0A FF FF FF 66 83 8B 98 00 00 00 20 E9 FD FE FF FF 00")
                        );
                    }
                }},
                {Input, x => {
                    Game.WriteByteArray(
                       new IntPtr(0x14036A590),
                       x
                           ? ConvertByteString("E6 DF 01 40 01")
                           : ConvertByteString("50 75 0A 40 01")
                       );

                    Game.WriteByteArray(
                       new IntPtr(0x14036A598),
                       x
                           ? ConvertByteString("50 75 0A 40 01")
                           : ConvertByteString("10 6D 0A 40 01")
                       );

                    Game.WriteByteArray(
                       new IntPtr(0x1400A763A),
                       x
                           ? ConvertByteString("E9 58 01 00 00 90 90")
                           : ConvertByteString("45 00 BE 00 01 00 00")
                       );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x14001DFE6),
                            ConvertByteString("80 3D 17 50 44 00 00 0F 84 1D 8D 08 00 0F BE 0D 0A 50 44 00 C6 05 03 50 44 00 00 48 8B 44 24 68 48 8B 80 C8 03 00 00 01 48 0C 48 31 C0 48 8B CB E9 F5 8C 08 00")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x1400A7797),
                            ConvertByteString("44 88 3D 66 B8 3B 00 45 00 BE 00 01 00 00 E9 97 FE FF FF")
                        );
                    }
                }},
                {nullpo, x => {
                    if (DoubleRotate.IsChecked == true) {
                        Game.WriteByte(
                            new IntPtr(0x1400A6CB9),
                            (byte)(x
                                ? 2
                                : 0
                            )
                        );

                        Game.WriteByte(
                            new IntPtr(0x1426CCEBE),
                            (byte)(x
                                ? 0x0C
                                : 0x05
                            )
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x1426CCE8E),
                            x
                                ? ConvertByteString("E9 17 51 93 FD 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90")
                                : ConvertByteString("44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C")
                        );

                        if (x) {
                            Game.WriteByteArray(
                                new IntPtr(0x140462000),
                                ConvertByteString("00 00 01 00 02 00 01 FF 02 FF FF 00 FE 00 FF FF FE FF 00 01 03 00 FD 00 00 00 00 FF 00 FE FF FF FF FE 00 01 00 02 FF 01 FF 02 01 00 00 FD 00 03 00 00 FF 00 FE 00 01 00 02 00 00 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 00 FE 00 01 00 02 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 02 00 FF 00 FE 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 00 FE 00 01 00 02 01 00 00 00 00 00 00 00 00 00 00 00 00 00 01 FF FF FF FF 01 01 01")
                            );

                            Game.WriteByteArray(
                                new IntPtr(0x140001FAA),
                                ConvertByteString("83 3D 52 10 46 00 02 75 58 83 7E 08 06 74 74 48 0F B6 4E 18 48 83 F9 02 7C 04 48 83 E9 02 48 6B C9 18 48 BA 00 20 46 40 01 00 00 00 48 01 CA 44 0F BE 34 5A 44 0F BE 7C 5A 01 83 7E 18 02 0F 8C B4 AE 6C 02 41 F7 DE 83 7E 08 06 0F 85 A7 AE 6C 02 48 0F B6 56 18 E9 9D AE 6C 02 45 31 FF 45 31 F6 83 FF 04 0F 8F 8E AE 6C 02 44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C E9 75 AE 6C 02 48 0F B6 4E 18 48 6B C9 18 48 BA 00 20 46 40 01 00 00 00 48 8D 54 11 30 44 0F BE 34 5A 44 0F BE 7C 5A 01 0F B6 4E 18 48 BA 90 20 46 40 01 00 00 00 0F BE 0C 4A 41 01 CE 0F B6 4E 18 0F BE 4C 4A 01 41 01 CF E9 2C AE 6C 02")
                            );
                        }
                    } else {
                        nullpo.IsChecked = false;
                    }
                }},
                {thenew, x => {
                    Game.WriteByte(
                        new IntPtr(0x1426CCEBE),
                        (byte)(x
                            ? 0x04
                            : 0x05
                        )
                    );

                    Game.WriteByteArray(
                        new IntPtr(0x1426CCE8E),
                        x
                            ? ConvertByteString("E9 17 51 93 FD 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90")
                            : ConvertByteString("44 8B 74 C2 2C 44 8B 7C C2 30 44 2B 7C CA 30 44 2B 74 CA 2C")
                        );

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140462000),
                            ConvertByteString("00 00 01 00 00 FF FF 00 00 01")
                        );

                        Game.WriteByteArray(
                            new IntPtr(0x140001FAA),
                            ConvertByteString("48 8B D3 48 B9 00 20 46 40 01 00 00 00 44 0F BE 34 51 44 0F BE 7C 51 01 83 7C 24 58 01 0F 84 D5 AE 6C 02 41 F7 DE E9 CD AE 6C 02")
                        );
                    }
                }},
                {thenewrs, x => {
                    RotationStateBase(x);

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140463E20),
                            ConvertByteString("00 00 00 FF 01 00 FF FF 00 00 00 FF FF 00 01 FF 00 FF 01 FF FF FF FF 00 01 FF 00 FF FF FF 01 00 00 00 00 FF FF 00 01 00 00 00 FF 00 00 FF FF FF 00 00 FF 00 FE 00 01 00 00 00 FF 01 FF 00 00 FF 00 00 00 FF 01 00 01 01 FF 01 FF 00 FF FF 00 01 00 00 00 01 00 FF 01 FF 00 00 00 FF FF 00 00 01 00 00 FF 00 00 FF FF FF 00 00 00 01 00 FF 00 FE 00 00 00 FF 01 00 FF FF 00 00 00 FF FF 00 01 FF 00 00 01 00 FF 00 01 FF 00 00 01 00 FF 00 FF FF 00 00 00 01 FF 00 01 00 00 00 FF 00 00 FF FF FF 00 00 FF 00 FE 00 01 00 00 00 FF 01 FF 00 00 FF 00 00 00 FF 01 00 01 01 00 00 00 FF 00 01 FF FF 01 00 01 01 01 FF 00 01 00 00 00 FF 01 00 00 01 00 00 FF 00 00 FF FF FF 00 00 00 01 00 FF 00 FE")
                        );
                    }
                }},
                {ars, x => {
                    RotationStateBase(x);

                    if (x) {
                        Game.WriteByteArray(
                            new IntPtr(0x140463E20),
                            ConvertByteString("00 00 00 FF 01 00 FF FF 00 00 00 FF FF 00 01 FF 00 00 01 00 FF 00 FF 01 00 00 FF 00 01 00 01 01 00 00 00 01 FF 00 01 00 00 00 FF 00 00 FF FF FF 00 00 01 00 FE 00 FF 00 00 00 00 FF 01 FF 01 FE 00 00 00 FF FF FF FF FE 00 00 00 FF 00 01 01 01 00 00 00 01 00 FF 01 FF 00 00 00 FF 01 00 00 01 00 00 FF 00 00 FF FF FF 00 00 00 FF 00 01 00 02 00 00 00 FF 01 00 FF FF 00 00 00 FF FF 00 01 FF 00 01 01 01 01 00 FF 01 00 01 01 01 FF 00 FF 01 00 01 01 01 FF 01 00 00 00 00 FF 00 00 FF FF FF 00 00 01 00 FE 00 FF 00 00 00 00 FF 01 FF 01 FE 00 00 00 FF FF FF FF FE 00 00 00 FF 00 01 FF FF 00 00 00 01 00 FF FF 01 00 00 00 FF FF 00 00 01 00 00 FF 00 00 FF FF FF 00 00 00 FF 00 01 00 02")
                        );
                    }
                }}
            };

            DialScripts = new Dictionary<Dial, Action<int>>() {
                {GarbageRate, x =>
                    Game.WriteUInt16(new IntPtr(0x14044193C), (ushort)x)
                },
                {AllClearMultiplier, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x1411372F0),
                        (x == 30)
                            ? ConvertByteString("89 8B 84 02 00 00")
                            : ConvertByteString("E9 F6 00 00 00 90")
                        );
                    Game.WriteByteArray(
                        new IntPtr(0x1411373EB),
                        ConvertByteString("C7 83 84 02 00 00 1E 00 00 00 E9 FC FE FF FF")
                    );
                    Game.WriteUInt32(new IntPtr(0x1411373F1), (uint)x);
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
                {ReceiveT, x =>
                    Game.WriteByte(new IntPtr(0x142726178), (byte)x)
                },
                {ReceiveP, x => {
                    Game.WriteByteArray(                                //I overwrite a useless operation to do this, although it /may/ not be useless
                        new IntPtr(0x14113722A),                        //I followed the values around a while ago and came to the conclusion that its redundant
                        (x == 30)                                       //AKA this might have unintended side effects
                            ? ConvertByteString("89 8B E8 01 00 00")    //If setting back to default, also fix residue. PPT is now vanilla again
                            : ConvertByteString("B9 1E 00 00 00 90")
                    );
                    Game.WriteByte(new IntPtr(0x1411371E4), (byte)x);
                }},
                {DAS, x =>
                    Game.WriteByte(new IntPtr(0x1413C8C52), (byte)(x + 1)) //DAS gets decremented the same frame it's set, so I increment x here to counter that
                },
                {Autolockdial, x =>
                    Game.WriteByte(new IntPtr(0x142853B2C), (byte)x)
                },
                {Lockoutdial, x =>
                    Game.WriteByte(new IntPtr(0x142802821), (byte)x)
                },
                {Softdrop, x => {
                    if (x == 20) {                      //PPT uses lea eax,[rax+rax*4] and then shl eax,2 for default operations
                        Game.WriteByteArray(            //and while imul eax,eax,20 is the exact same (but slower) I want to leave PPT in vanilla state if its a vanilla value
                            new IntPtr(0x142892DC1),
                            ConvertByteString("8D 04 80 C1 E0 02")
                        );
                    } else {
                        Game.WriteByteArray(
                            new IntPtr(0x142892DC1),
                            ConvertByteString("69 C0 14 00 00 00")  //turn default operation into a 4 byte imul, defaulting to 20
                        );
                        Game.WriteInt32(new IntPtr(0x142892DC3), x);//change the 20 to what you actually want
                    }
                }}
            };

            TableScripts = new Dictionary<UniformGrid, Action<int, int>>() {
                {TvTAttackTable, (i, x) => {
                    if (i < 4) {
                        i += 0x24;
                    } else if (i < 9) {
                        i += 0x16;
                    } else {
                        i -= 0xA;
                    }

                    Game.WriteByte(new IntPtr(0x1403200B5 + i), (byte)x);
                }},
                {TvTComboTable, (i, x) =>
                    Game.WriteByte(new IntPtr(0x1403200BB + i), (byte)x)
                },
                {TvPAttackTable, (i, x) => {
                    if (i < 5) {
                        i = i*2 + 0x26B;
                    } else if (i == 5) {
                        i = 0x277;
                    } else {
                        i -= 0xA;
                    }

                    Game.WriteByte(new IntPtr(0x1404329C5 + i), (byte)x);
                }},
                {TvPComboTable, (i, x) =>
                    Game.WriteByte(new IntPtr(0x140432C17 + i), (byte)x)
                },
                {PvPChainTable, (i, x) =>
                    Game.WriteUInt16(new IntPtr(0x14031DAC0 + i*2), (ushort)x)
                },
                {DelayTable, (i, x) => {
                    if (i == 0) {
                        Game.WriteByte(new IntPtr(0x142724DFC), (byte)x);
                    } else {
                        Game.WriteByte(new IntPtr(0x1427E453B + i*7), (byte)x);
                    }
                }},
                {MarginTimeTable, (i, x) =>
                    Game.WriteUInt16(new IntPtr(0x1403201A6 + i*2), (ushort)x)
                },
                {GravityTable, (i, x) => {
                    if (i == 0) {                                                   
                        Game.WriteUInt16(new IntPtr(0x1403200E8), (ushort)x);       //in memory the first two values are both 17, but correspond to different things for no good reason
                        Game.WriteUInt16(new IntPtr(0x1403200EA), (ushort)x);       //one is for the "Sweet" setting, and the other is for Challenge Modes / Level 1
                    } else {                                                        //To reduce confusion I'm editing both at the same time since they are, after all, the same value
                        Game.WriteUInt16(new IntPtr(0x1403200EA + i*2), (ushort)x);
                    }
                }}
            };

            EncodingList = new List<object>() {
                PentominoVersus,
                RemoveLineClearDelay,
                UndoHold,
                FreezeSwap,
                ColX,
                ColM,
                Lockout,
                Invisible,
                Input,
                new List<OptionalRadioButton>() {
                    RemoveAutoLock,
                    TGMAutoLock
                },
                new List<OptionalRadioButton>() {
                    Sonicdrop,
                    Float,
                    Sink,
                    Up
                },
                Noghost,
                Unhold,
                PreserveRot,
                Lockoutdial,
                new List<OptionalRadioButton>() {
                    Ascension,
                    Cultris2,
                    h,
                    BONKERS,
                    jstrismeme,
                    thenew,
                    jstris,
                    nullpo
                },
                new List<OptionalRadioButton>() {
                    ars,
                    thenewrs
                },
                new List<OptionalRadioButton>() {
                    DoubleRotate,
                    Cycle,
                    Flip,
                    Flip180,
                    bigsoftdrop
                },
                ARR,
                TvTAttackTable,
                TvTComboTable,
                TvPAttackTable,
                TvPComboTable,
                GarbageRate,
                AllClearMultiplier,
                PvPChainTable,
                MarginTimeTable,
                new List<OptionalRadioButton>() {
                    TetrisB2BDouble,
                    TetrisB2BAdd2,
                    TetrisB2BCum
                },
                new List<OptionalRadioButton>() {
                    TspinB2BDouble,
                    TspinB2BAdd2,
                    TspinB2BCum
                },
                new List<OptionalRadioButton>() {
                    FullTmini,
                    NoT,
                    AllT
                },
                CleanGarbage,
                GarbageFilled,
                GarbageEmpty,
                ReceiveT,
                ReceiveP,
                ReceiveCap, 
                SecretGradeGarbage,
                GarbageBlocking,
                UnCappedPC,
                ColorClear,
                AllSpin,
                DelayTable,
                DAS,
                Autolockdial,
                GravityTable,
                Softdrop,
            };
        }

        private void RotationStateBase(bool x) {    //All custom rotation state codes will call this first, passing its own activation state to it
            if (x) {
                Game.WriteByteArray(
                    new IntPtr(0x140463E00),
                    ConvertByteString("20 3E 46 40 01")
                );

                Game.WriteByteArray(
                    new IntPtr(0x140463E08),
                    ConvertByteString("58 3E 46 40 01")
                );

                Game.WriteByteArray(
                    new IntPtr(0x140463E10),
                    ConvertByteString("90 3E 46 40 01")
                );

                Game.WriteByteArray(
                    new IntPtr(0x140463E18),
                    ConvertByteString("C8 3E 46 40 01")
                );
            }

            Game.WriteByteArray(//x
               new IntPtr(0x1426D5EF4),
               x
                   ? ConvertByteString("48 8B 05 05 DF D8 FD 48 8D 04 50 44 2A 04 C8 41 0F BE C0 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 1E E0 D8 FD 8B 44 C8 04 44 01")
               );

            Game.WriteByteArray(
               new IntPtr(0x1426D5ED7),
               x
                   ? ConvertByteString("48 8B 05 2A DF D8 FD 48 8D 04 50 44 2A 04 C8 41 0F BE C0 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 3B E0 D8 FD 8B 44 C8 08 44 01")
               );

            Game.WriteByteArray(
               new IntPtr(0x1426D5E09),
               x
                   ? ConvertByteString("48 8B 05 00 E0 D8 FD 48 8D 04 50 44 2A 04 C8 41 0F BE C0 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 09 E1 D8 FD 44 2B 44 C8 04 44")
               );

            Game.WriteByteArray(
               new IntPtr(0x1426D5DEB),
               x
                   ? ConvertByteString("48 8B 05 26 E0 D8 FD 48 8D 04 50 44 2A 04 C8 41 0F BE C0 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 27 E1 D8 FD 44 2B 44 C8 08 44")
               );

            Game.WriteByteArray(//y
               new IntPtr(0x14270DBA0),
               x
                   ? ConvertByteString("48 8B 05 59 62 D5 FD 48 8D 04 50 44 2A 4C C8 01 41 0F BE C1 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 72 63 D5 FD 8B 44 C8 08 44 01 C8")
               );

            Game.WriteByteArray(
               new IntPtr(0x14270DB82),
               x
                   ? ConvertByteString("48 8B 05 7F 62 D5 FD 48 8D 04 50 44 2A 4C C8 01 41 0F BE C1 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 90 63 D5 FD 44 2B 4C C8 04 44 89")
               );

            Game.WriteByteArray(
               new IntPtr(0x14270DB60),
               x
                   ? ConvertByteString("48 8B 05 A9 62 D5 FD 48 8D 04 50 44 2A 4C C8 01 41 0F BE C1 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 B2 63 D5 FD 44 2B 4C C8 08 44 89")
               );

            Game.WriteByteArray(
               new IntPtr(0x14270DB43),
               x
                   ? ConvertByteString("48 8B 05 CE 62 D5 FD 48 8D 04 50 44 2A 4C C8 01 41 0F BE C1 C3")
                   : ConvertByteString("48 6B C9 1C 48 01 C1 48 8B 05 CF 63 D5 FD 8B 44 C8 04 44 01 C8")
               );
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

        void Reset(TextBlock textBlock) {} // Required because UniformGrid contains TextBlocks, but we don't care about them

        void Reset(CheckBox checkBox) {
            checkBox.IsChecked = false;
            CheckBoxHandle(checkBox, null);
        }

        void Reset(List<OptionalRadioButton> group) {
            int currIndex = group.FindIndex(i => i.IsChecked == true);

            if (currIndex >= 0) {
                group[currIndex].IsChecked = false;
                RadioButtonHandle(group[currIndex], null);
                OptionalRadioButton.OverrideSelected(group[currIndex].GroupName, null);
            }
        }

        void Reset(Dial dial)
            => dial.RawValue = dial.Default;

        void Reset(UniformGrid grid) {
            foreach (UIElement i in grid.Children)
                Reset((dynamic)i);
        }

        OpenFileDialog ofd;
        SaveFileDialog sfd;
        
        static byte[] CreateHeader() => Encoding.ASCII.GetBytes($"PTSB").Concat(BitConverter.GetBytes(Assembly.GetExecutingAssembly().GetName().Version.Minor)).ToArray();

        static void CheckHeader(BinaryReader reader) {
            if (!reader.ReadBytes(4).Select(i => (char)i).SequenceEqual(new char[] {'P', 'T', 'S', 'B'}))
                throw new InvalidDataException("The selected file is not a PPT-Sandbox file.");

            if (reader.ReadInt32() != Version)
                throw new InvalidDataException("The version of the file doesn't match the version of PPT-Sandbox.");
        }

        void Encode(BinaryWriter writer, TextBlock textBlock) {} // Required because UniformGrid contains TextBlocks, but we don't care about them

        void Encode(BinaryWriter writer, CheckBox checkBox)
            => writer.Write(checkBox.IsChecked == true);

        void Encode(BinaryWriter writer, List<OptionalRadioButton> group)
            => writer.Write(group.FindIndex(i => i.IsChecked == true));

        void Encode(BinaryWriter writer, Dial dial)
            => writer.Write((int)dial.RawValue);

        void Encode(BinaryWriter writer, UniformGrid grid) {
            foreach (UIElement i in grid.Children)
                Encode(writer, (dynamic)i);
        }
        
        void Decode(BinaryReader reader, TextBlock textBlock) {} // Required because UniformGrid contains TextBlocks, but we don't care about them

        void Decode(BinaryReader reader, CheckBox checkBox) {
            checkBox.IsChecked = reader.ReadBoolean();
            CheckBoxHandle(checkBox, null);
        }

        void Decode(BinaryReader reader, List<OptionalRadioButton> group) {
            OptionalRadioButton update = null;

            int currIndex = group.FindIndex(i => i.IsChecked == true);

            if (currIndex >= 0)
                (update = group[currIndex]).IsChecked = false;

            int index = reader.ReadInt32();
            if (index >= 0)
                (update = group[index]).IsChecked = true;

            if (currIndex != index && update != null) {
                RadioButtonHandle(update, null);
                OptionalRadioButton.OverrideSelected(update.GroupName, update);
            }
        }

        void Decode(BinaryReader reader, Dial dial)
            => dial.RawValue = reader.ReadInt32();

        void Decode(BinaryReader reader, UniformGrid grid) {
            foreach (UIElement i in grid.Children)
                Decode(reader, (dynamic)i);
        }

        public void Reset(object sender, RoutedEventArgs e) {
            foreach (object i in EncodingList)
                Reset((dynamic)i);
        }

        public void Encode(object sender, RoutedEventArgs e) {
            if (sfd.ShowDialog() != true) return;

            try {
                using (MemoryStream output = new MemoryStream()) {
                    using (BinaryWriter writer = new BinaryWriter(output)) {
                        writer.Write(CreateHeader());

                        foreach (object i in EncodingList)
                            Encode(writer, (dynamic)i);
                    }

                    File.WriteAllBytes(sfd.FileName, output.ToArray());
                }

            } catch (Exception ex) {
                MessageBox.Show(
                    $"An error occurred while saving the file: \n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        void Decode(object sender, RoutedEventArgs e) {
            if (ofd.ShowDialog() != true) return;
            
            try {
                using (FileStream file = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                    using (BinaryReader reader = new BinaryReader(file)) {
                        CheckHeader(reader);
                
                        foreach (object i in EncodingList)
                            Decode(reader, (dynamic)i);
                    }

            } catch (Exception ex) {
                MessageBox.Show(
                    $"An error occurred while loading the file: \n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
