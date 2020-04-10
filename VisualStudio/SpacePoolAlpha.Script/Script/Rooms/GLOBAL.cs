// Room_GLOBAL - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_GLOBAL;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_GLOBAL
    {
        // Fields

        // Methods
        public void initialize_control_panel()
        {
            gRestartYN.Centre();
            system.Volume = 100;
            SetGameSpeed(40);
            if (system.SupportsGammaControl)
            {
                system.Gamma = 100;
            }
        }

        public override void game_start()
        {
            initialize_control_panel();
        }

        public override void repeatedly_execute()
        {
            if (IsGamePaused() == 1)
                return;
        }

        public override void repeatedly_execute_always()
        {
        }

        public void show_save_game_dialog()
        {
            gSaveGame.Visible = true;
            lstSaveGamesList.FillSaveGameList();
            if (lstSaveGamesList.ItemCount > 0)
            {
                txtNewSaveName.Text = lstSaveGamesList.Items[0];
            }
            else 
            {
                txtNewSaveName.Text = "";
            }
            mouse.UseModeGraphic(eModePointer);
        }

        public void show_restore_game_dialog()
        {
            gRestoreGame.Visible = true;
            lstRestoreGamesList.FillSaveGameList();
            mouse.UseModeGraphic(eModePointer);
        }

        public void close_save_game_dialog()
        {
            gSaveGame.Visible = false;
        }

        public void close_restore_game_dialog()
        {
            gRestoreGame.Visible = false;
        }

        public override void on_key_press(eKeyCode keycode)
        {
            if ((keycode == eKeyEscape) && gRestartYN.Visible)
            {
                gRestartYN.Visible = false;
                return;
            }
            if ((keycode == eKeyEscape) && (gSaveGame.Visible))
            {
                close_save_game_dialog();
                return;
            }
            if ((keycode == eKeyEscape) && (gRestoreGame.Visible))
            {
                close_restore_game_dialog();
                return;
            }
            if (keycode == eKeyReturn)
            {
                if (gRestartYN.Visible)
                    RestartGame();
            }
            if (IsGamePaused() || (IsInterfaceEnabled() == 0))
            {
                return;
            }
            if (keycode == eKeyCtrlQ)
                QuitGame(1);
            if (keycode == eKeyF5)
                show_save_game_dialog();
            if (keycode == eKeyF7)
                show_restore_game_dialog();
            if (keycode == eKeyF12)
                SaveScreenShot("scrnshot.bmp");
        }

        public override void on_mouse_click(MouseButton button)
        {
        }

        public void interface_click(int interfaceVar, int button)
        {
        }

        public override void cEgo_Look()
        {
            Display("Damn, I'm looking good!");
        }

        public override void cEgo_Interact()
        {
            Display("You rub your hands up and down your clothes.");
        }

        public override void cEgo_Talk()
        {
            Display("Talking to yourself is a sign of madness!");
        }

        public void dialog_request(int param)
        {
        }

        public void btnRestart_OnClick(GUIControl control, MouseButton button)
        {
            gRestartYN.Visible=true;
        }

        public override void btnRestartYes_OnClick(GUIControl control, MouseButton button)
        {
            RestartGame();
        }

        public override void btnRestartNo_OnClick(GUIControl control, MouseButton button)
        {
            gRestartYN.Visible = false;
        }

        public override void btnCancelSave_OnClick(GUIControl control, MouseButton button)
        {
            close_save_game_dialog();
        }

        public override void btnSaveGame_OnClick(GUIControl control, MouseButton button)
        {
            int gameSlotToSaveInto = lstSaveGamesList.ItemCount + 1;
            int i = 0;
            while (i < lstSaveGamesList.ItemCount)
            {
                if (lstSaveGamesList.Items[i] == txtNewSaveName.Text)
                {
                    gameSlotToSaveInto = lstSaveGamesList.SaveGameSlots[i];
                }
                i += 1;
            }
            SaveGameSlot(gameSlotToSaveInto, txtNewSaveName.Text);
            close_save_game_dialog();
        }

        public override void btnCancelRestore_OnClick(GUIControl control, MouseButton button)
        {
            close_restore_game_dialog();
        }

        public override void btnRestoreGame_OnClick(GUIControl control, MouseButton button)
        {
            if (lstRestoreGamesList.SelectedIndex >= 0)
            {
                RestoreGameSlot(lstRestoreGamesList.SaveGameSlots[lstRestoreGamesList.SelectedIndex]);
            }
            close_restore_game_dialog();
        }

        public void lstSaveGamesList_OnSelectionCh(GUIControl control)
        {
            txtNewSaveName.Text = lstSaveGamesList.Items[lstSaveGamesList.SelectedIndex];
        }

        public void txtNewSaveName_OnActivate(GUIControl control)
        {
            btnSaveGame_OnClick(control, eMouseLeft);
        }

        public override void btnDeleteSave_OnClick(GUIControl control, MouseButton button)
        {
            if (lstSaveGamesList.SelectedIndex >= 0)
            {
                DeleteSaveSlot(lstSaveGamesList.SaveGameSlots[lstSaveGamesList.SelectedIndex]);
                lstSaveGamesList.FillSaveGameList();
            }
        }

        // Expose Global Variables

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

}
