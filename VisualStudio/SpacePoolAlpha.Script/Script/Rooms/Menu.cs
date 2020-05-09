// Room_Menu - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_Menu;
using static SpacePoolAlpha.MenuStaticRef;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;
using Microsoft.Xna.Framework.Input;

namespace SpacePoolAlpha
{
    public partial class Room_Menu // 2
    {
        // Fields
        public DynamicSprite bg;
        public int currentMenuItem;
        public int selectedMenuItem = -1;
        public int countdown;
        public bool replaying;
        public SMenuItem[] items = CreateAndInstantiateArray<SMenuItem>(4);

        public float get_menu_item_size(int i)
        {
            float x = items[i].timer;
            return 5.0f + 2.0f * x * x * (3.0f - 2.0f * x);
        }

        // Methods
        public override void room_Load()
        {
            bg = DynamicSprite.CreateFromBackground();
            items[0].text = "TUTORIAL]";
            items[0].tip = "CONTROL AND GAMEPLAY ORIENTATION]";
            items[0].timer = 1.0f;
            items[0].room = 1;
            items[1].text = "TIME ATTACK]";
            items[1].tip = "PLAY A GAME AGAINST THE CLOCK]";
            items[1].timer = 0.0f;
            items[1].room = 3;
            items[2].text = "TOURNAMENT]";
            items[2].tip = "PLAY A TOURNAMENT AGAINST AI OPPONENTS]";
            items[2].timer = 0.0f;
            items[2].room = 5;
            items[3].text = "TWO PLAYER]";
            items[3].tip = "PLAY WITH A FRIEND ON THIS COMPUTER]";
            items[3].timer = 0.0f;
            items[3].room = 4;
            if (replay_exists())
            {
                pool_set_num_players(1);
                pool_set_num_ships(1);
                pool_setup();
                pool_rack_em(0);
                replay_start_replay();
                ships[0].disableControls = 0;
                replaying = true;
            }
            else 
            {
                pool_set_num_players(0);
                pool_set_num_ships(2);
                pool_setup();
                pool_rack_em(0);
                ship_load(5);
                replaying = false;
                ships[0].ballSelectionMethod = 1;
                ships[0].astronavigationSkill = 1;
                ships[0].maxSpeed = 1.0f;
                ships[1].ballSelectionMethod = 1;
                ships[1].astronavigationSkill = 1;
                ships[1].maxSpeed = 1.0f;
            }
            pool_set_muted(true);
            selectedMenuItem = -1;
            countdown = 0;
            aSPACPOL2.Play();
        }

        public override void repeatedly_execute_always()
        {
            int mix = Random(314);
            pool_update();
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            if (replaying)
            {
                String timeString = get_time_string(replay_get_time());
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                surf.DrawVectorText(160.0f, 10.0f, 4.0f, timeString);
            }
            int i = 0;
            while (i < 4)
            {
                if (currentMenuItem == i)
                {
                    items[i].timer += 0.2f;
                    if (items[i].timer > 1.0f)
                        items[i].timer = 1.0f;
                }
                else 
                {
                    items[i].timer -= 0.1f;
                    if (items[i].timer < 0.0f)
                        items[i].timer = 0.0f;
                }
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, FloatToInt(255.0f *items[i].timer));
                surf.WriteAliasedTextMessage(160.0f, 40.0f + 40.0f *IntToFloat(i), get_menu_item_size(i), items[i].text);
                if (currentMenuItem == i)
                {
                    surf.DrawVectorText(160.0f, 190.0f, 3.0f, items[i].tip);
                }
                i += 1;
            }
            if (countdown > 0)
            {
                if (ds == null)
                    ds = DynamicSprite.Create(1, 1);
                DrawingSurface spriteSurf = ds.GetDrawingSurface();
                spriteSurf.Clear(Game.GetColorFromRGB(0, 0, 0));
                spriteSurf.Release();
                surf.DrawImage(0, 0, ds.Graphic, 2*countdown, 320, 200);
                //ds.Delete();
            }
            surf.Release();
        }

        DynamicSprite ds;

        public override void on_key_press(eKeyCode key)
        {
            if (selectedMenuItem == -1)
            {
                if (key == eKeyDownArrow && currentMenuItem < 3)
                {
                    MoveDown();
                }
                else if (key == eKeyUpArrow && currentMenuItem > 0)
                {
                    MoveUp();
                }
                else if (key == eKeyReturn)
                {
                    Select();
                }
            }
        }

        public override void ButtonPress(Buttons button)
        {
            if (selectedMenuItem == -1)
            {
                if ((button == Buttons.DPadDown || button == Buttons.LeftThumbstickDown) && currentMenuItem < 3)
                {
                    MoveDown();
                }
                else if ((button == Buttons.DPadUp || button == Buttons.LeftThumbstickUp) && currentMenuItem > 0)
                {
                    MoveUp();
                }
                else if (button == Buttons.A)
                {
                    Select();
                }
            }
        }

        private void MoveDown()
        {
            currentMenuItem += 1;
            aFloop.Play();
        }

        private void MoveUp()
        {
            currentMenuItem -= 1;
            aFloop.Play();
        }

        private void Select()
        {
            selectedMenuItem = currentMenuItem;
            int i = 0;
            while (i < 10)
            {
                float x = IntToFloat(110 + Random(100));
                float y = IntToFloat(35 + 40 * selectedMenuItem + Random(10));
                pool_create_explosion(x, y, 0.0f, 0.0f, 8.0f, 10, Game.GetColorFromRGB(255, 255, 255));
                aPocket.Play();
                i += 1;
            }
            countdown = 50;
        }

        public override void room_RepExec()
        {
            if (countdown > 0)
            {
                countdown -= 1;
                if (countdown == 0)
                {
                    replay_Playback = false;
                    cEgo.ChangeRoom(items[selectedMenuItem].room);
                }
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

    #region SMenuItem (AGS struct from .asc converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class SMenuItem
    {
        // Fields
        public float timer;
        public String text;
        public String tip;
        public int room;

    }

    #endregion

    #region Static class for referencing parent class without prefixing with instance (AGS struct workaround)

    public static class MenuStaticRef
    {
        // Static Fields
        public static DynamicSprite bg { get { return GlobalBase.Menu.bg; } set { GlobalBase.Menu.bg = value; } }
        public static int currentMenuItem { get { return GlobalBase.Menu.currentMenuItem; } set { GlobalBase.Menu.currentMenuItem = value; } }
        public static int selectedMenuItem { get { return GlobalBase.Menu.selectedMenuItem; } set { GlobalBase.Menu.selectedMenuItem = value; } }
        public static int countdown { get { return GlobalBase.Menu.countdown; } set { GlobalBase.Menu.countdown = value; } }
        public static bool replaying { get { return GlobalBase.Menu.replaying; } set { GlobalBase.Menu.replaying = value; } }
        public static SMenuItem[] items { get { return GlobalBase.Menu.items; } set { GlobalBase.Menu.items = value; } }

        // Static Methods
        public static void room_Load()
        {
            GlobalBase.Menu.room_Load();
        }

        public static void room_RepExec()
        {
            GlobalBase.Menu.room_RepExec();
        }

    }

    #endregion
    
}
