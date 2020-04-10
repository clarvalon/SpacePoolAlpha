// Room_Tournament - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_Tournament;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_Tournament // 5
    {
        // Fields
        public int round;
        public int state = STATE_INTRO;
        public int stateTime;
        public bool wonRound;
        public bool wonTournament;
        public int piloti;
        public String[] roundText = new String[NUMROUNDS];
        public int[] ballSelectionMethods = new int[NUMROUNDS];
        public int[] astronavigationSkills = new int[NUMROUNDS];
        public int[] preparationMethods = new int[NUMROUNDS];
        public float[] aimAngles = new float[NUMROUNDS];
        public bool doCheatDialog;
        public int cheatDialogState;
        public int cheatDialogTime;
        public int cheatDialogWhich;
        public DynamicSprite cheatBackup;

        public void set_opponent_ship_properties()
        {
            ships[1].reactionTime = 60 - 10 * round;
            ships[1].flyToRange = 110.0f - IntToFloat(10 * round);
            ships[1].maxSpeed = 1.0f + 0.1f * IntToFloat(round);

            ballSelectionMethods[0] = 0;
            ballSelectionMethods[1] = 1;
            ballSelectionMethods[2] = 2;
            ballSelectionMethods[3] = 3;
            ballSelectionMethods[4] = 3;
            ballSelectionMethods[5] = 3;
            ballSelectionMethods[6] = 3;

            astronavigationSkills[0] = 0;
            astronavigationSkills[1] = 1;
            astronavigationSkills[2] = 1;
            astronavigationSkills[3] = 2;
            astronavigationSkills[4] = 2;
            astronavigationSkills[5] = 2;
            astronavigationSkills[6] = 2;

            aimAngles[0] = 0.999f;
            aimAngles[1] = 0.9999f;
            aimAngles[2] = 0.99992f;
            aimAngles[3] = 0.99994f;
            aimAngles[4] = 0.99996f;
            aimAngles[5] = 0.99998f;
            aimAngles[6] = 0.99999f;

            preparationMethods[0] = 0;
            preparationMethods[1] = 0;
            preparationMethods[2] = 1;
            preparationMethods[3] = 1;
            preparationMethods[4] = 2;
            preparationMethods[5] = 2;
            preparationMethods[6] = 2;

            ships[1].haveGun = false;
            ships[1].haveTeleport = false;
            ships[1].ballSelectionMethod = ballSelectionMethods[round];
            ships[1].astronavigationSkill = astronavigationSkills[round];
            ships[1].aimAngle = aimAngles[round];
            ships[1].preparationMethod = preparationMethods[round];

            if (round == LASTROUND)
            {
                int shipIndex = ship_get_current_index();
                if (shipIndex == 22)
                {
                    ships[1].haveGun = true;
                }
                else if (shipIndex == 7)
                {
                    ships[1].haveTeleport = true;
                }
                else if (shipIndex == 12)
                {
                    ships[1].recoverTime = 10;
                }
            }
        }

        // Methods
        public override void room_Load()
        {
            pool_set_num_players(1);
            pool_set_num_ships(2);
            pool_setup();
            pool_rack_em(0);
            ships_setup();
            round = 0;
            ship_load(ship_get_for_round(round));
            state = STATE_INTRO;
            stateTime = 0;
            set_opponent_ship_properties();
            balls[10].onTable = false;
            holdCueBall = true;
            wonTournament = false;
            roundText[0] = "ROUND 1";
            roundText[1] = "ROUND 2";
            roundText[2] = "ROUND 3";
            roundText[3] = "ROUND 4";
            roundText[4] = "QUARTER FINAL";
            roundText[5] = "SEMI FINAL";
            roundText[6] = "FINAL";
            aSPACCER.Play();
        }

        public void update_intro(DrawingSurface surf)
        {
            DynamicSprite pixel = DynamicSprite.Create(1, 1);
            DrawingSurface dss = pixel.GetDrawingSurface();
            dss.Clear(0);
            dss.Release();
            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
            surf.DrawBox(39, 64, 21, 21);
            surf.DrawImage(20, 50, pixel.Graphic, 50, 280, 100);
            surf.DrawBox(20, 50, 280, 100);
            surf.WriteTextMessageWrap(80.0f, 70.0f, 200.0f, 6.0f, roundText[round], stateTime);
            surf.WriteTextMessageWrap(80.0f, 80.0f, 200.0f, 4.0f, StringFormatAGS("OPPONENT: %s", ship_get_current_name()), stateTime);
            String msg = ship_get_current_bio();
            surf.WriteTextMessageWrap(40.0f, 100.0f, 240.0f, 4.0f, msg, stateTime);
            if (msg.Length > stateTime)
                aMsg.Play();
            surf.DrawImage(40, 65, 14);
            surf.DrawImage(40, 65, ship_get_current_sprite());
        }

        public void update_results(DrawingSurface surf)
        {
            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
            if (wonRound)
            {
                surf.WriteAliasedTextMessage(160.0f, 100.0f, 6.0f, "PLAYER 1 WINS]");
                if (round >= LASTROUND)
                {
                    wonTournament = true;
                }
            }
            else 
            {
                surf.WriteAliasedTextMessage(160.0f, 100.0f, 6.0f, StringFormatAGS("%s WINS]", ship_get_current_name()));
            }
        }

        public override void repeatedly_execute_always()
        {
            if (teleported >= 0)
            {
                teleported += 1;
                if (teleported == 50)
                {
                    teleported = -2;
                    doCheatDialog = true;
                    cheatDialogState = -1;
                    cheatDialogTime = 99;
                    cheatDialogWhich = 0;
                }
            }
            if (firedBullet >= 0)
            {
                firedBullet += 1;
                if (firedBullet == 100)
                {
                    firedBullet = -2;
                    doCheatDialog = true;
                    cheatDialogState = -1;
                    cheatDialogTime = 99;
                    cheatDialogWhich = 1;
                }
            }
            if (haveRecovered >= 0)
            {
                haveRecovered += 1;
                if (haveRecovered == 80)
                {
                    haveRecovered = -2;
                    doCheatDialog = true;
                    cheatDialogState = -1;
                    cheatDialogTime = 99;
                    cheatDialogWhich = 2;
                }
            }
            if (doCheatDialog)
            {
                cheatDialogTime += 1;
                if (cheatDialogTime == 100)
                {
                    aMsg.Play();
                    cheatDialogTime = 0;
                    cheatDialogState += 1;
                    int yy = 40 + 32*cheatDialogState;
                    float fyy = IntToFloat(yy + 4);
                    float fw = 150.0f;
                    DrawingSurface surf2 = Room.GetDrawingSurfaceForBackground();
                    if (cheatDialogState == 0)
                    {
                        ships[0].chan.Volume = 0;
                        ships[1].chan.Volume = 0;
                        surf2.DrawingColor = Game.GetColorFromRGB(0, 0, 0);
                        surf2.DrawRectangle(50, 20, 270, 180);
                        surf2.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                        surf2.DrawBox(50, 20, 220, 160);
                        surf2.DrawingColor = Game.GetColorFromRGB(127, 127, 127);
                        surf2.DrawBox(69, yy-1, 21, 21);
                        surf2.DrawImage(70, yy, 14);
                        surf2.DrawImage(70, yy, 24);
                        surf2.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                        if (cheatDialogWhich == 1)
                        {
                            surf2.WriteTextMessageWrap(100.0f, fyy, fw, 4.0f, "Hold the phone. What the heck is that?", 100);
                        }
                        else 
                        {
                            surf2.WriteTextMessageWrap(100.0f, fyy, fw, 4.0f, "Hold the phone. What the heck was that?", 100);
                        }
                    }
                    else if (cheatDialogState == 1)
                    {
                        surf2.DrawingColor = Game.GetColorFromRGB(127, 127, 127);
                        surf2.DrawBox(229, yy-1, 21, 21);
                        surf2.DrawImage(230, yy, 14);
                        surf2.DrawImage(230, yy, ship_get_current_sprite());
                        surf2.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                        if (cheatDialogWhich == 0)
                        {
                            surf2.WriteTextMessageWrap(70.0f, fyy, fw, 4.0f, "Oh, that? That was just a teleporter. Don@t you have one?", 100);
                        }
                        else if (cheatDialogWhich == 1)
                        {
                            surf2.WriteTextMessageWrap(70.0f, fyy, fw, 4.0f, "Oh, that? That@s just my laser. Don@t you have one?", 100);
                        }
                        else if (cheatDialogWhich == 2)
                        {
                            surf2.WriteTextMessageWrap(70.0f, fyy, fw, 4.0f, "Oh, that? That was just a shield. Don@t you have one?", 100);
                        }
                    }
                    else if (cheatDialogState == 2)
                    {
                        surf2.DrawingColor = Game.GetColorFromRGB(127, 127, 127);
                        surf2.DrawBox(69, yy-1, 21, 21);
                        surf2.DrawImage(70, yy, 14);
                        surf2.DrawImage(70, yy, 24);
                        surf2.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                        surf2.WriteTextMessageWrap(100.0f, fyy, fw, 4.0f, "No. That can@t be legit!", 100);
                    }
                    else if (cheatDialogState == 3)
                    {
                        surf2.DrawingColor = Game.GetColorFromRGB(127, 127, 127);
                        surf2.DrawBox(229, yy-1, 21, 21);
                        surf2.DrawImage(230, yy, 14);
                        surf2.DrawImage(230, yy, ship_get_current_sprite());
                        surf2.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                        surf2.WriteTextMessageWrap(70.0f, fyy, fw, 4.0f, "Have you read the rules? It@s all perfectly above board.", 100);
                    }
                    else 
                    {
                        doCheatDialog = false;
                        cleanTheTable = true;
                    }
                    surf2.Release();
                }
            }
            else 
            {
                pool_update();
                DrawingSurface surf3 = Room.GetDrawingSurfaceForBackground();
                surf3.DrawImage(294, 0, 14);
                surf3.DrawImage(294, 0, ship_get_current_sprite());
                surf3.DrawImage(6, 0, 14);
                surf3.DrawImage(6, 0, 24);
                surf3.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                if (round < NUMROUNDS)
                {
                    surf3.DrawVectorText(60.0f, 10.0f, 4.0f, roundText[round]);
                    surf3.DrawVectorText(280.0f, 10.0f, 4.0f, ship_get_current_name(), eJustify_Right);
                }
                surf3.Release();
            }
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            stateTime += 1;
            if (state == STATE_INTRO)
            {
                update_intro(surf);
                if (stateTime > 320)
                {
                    stateTime = 0;
                    holdCueBall = false;
                    pool_place_ball_random(10);
                    while (balls[10].x > 160.0f)
                    {
                        pool_place_ball_random(10);
                    }
                    cleanTheTable = true;
                    state += 1;
                }
            }
            else if (state == STATE_PLAYING)
            {
                if (ships[0].numPocketed >= 5 || ships[1].numPocketed >= 5)
                {
                    wonRound = false;
                    if (ships[0].numPocketed > ships[1].numPocketed)
                    {
                        wonRound = true;
                    }
                    state += 1;
                    stateTime = 0;
                }
            }
            else if (state == STATE_RESULTS)
            {
                update_results(surf);
                if (stateTime > 160)
                {
                    if (wonRound && round < LASTROUND)
                    {
                        pool_reset();
                        pool_rack_em(0);
                        round += 1;
                        set_opponent_ship_properties();
                        balls[10].onTable = false;
                        holdCueBall = true;
                        ship_load(ship_get_for_round(round));
                        state = 0;
                    }
                    else 
                    {
                        state = STATE_TOMENU;
                    }
                    stateTime = 0;
                }
            }
            else if (state == STATE_TOMENU)
            {
                surf.Fade(stateTime);
            }
            surf.Release();
        }

        public override void on_key_press(eKeyCode key)
        {
            if (key == eKeyEscape)
            {
                state = STATE_TOMENU;
                stateTime = 0;
            }
            else if (key == eKey1 && piloti < 31 && false)
            {
                piloti += 1;
                ship_load(piloti);
                stateTime = 0;
            }
            else if (key == eKey2 && piloti > 0 && false)
            {
                piloti -= 1;
                ship_load(piloti);
                stateTime = 0;
            }
        }

        public override void room_RepExec()
        {
            if (state == STATE_TOMENU && stateTime >= 50)
            {
                if (wonTournament)
                {
                    cEgo.ChangeRoom(9);
                }
                else 
                {
                    cEgo.ChangeRoom(2);
                }
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose AGS singular #defines as C# constants (or static getters)
        public const int NUMROUNDS = 7;
        public const int LASTROUND = 6;
        public const int STATE_INTRO = 0;
        public const int STATE_PLAYING = 1;
        public const int STATE_RESULTS = 2;
        public const int STATE_TOMENU = 3;


    }

    #endregion

}
