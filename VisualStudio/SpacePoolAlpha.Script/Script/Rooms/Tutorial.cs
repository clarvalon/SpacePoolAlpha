// Room_Tutorial - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_Tutorial;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_Tutorial // 1
    {
        // Fields
        public int tutorialStage;
        public int ballNum = -1;
        public int disableControlsMessage;
        public int tutorialSubStage;
        public int fadeTime = -1;

        // Methods
        public override void room_Load()
        {
            pool_set_num_players(1);
            pool_set_num_ships(1);
            pool_setup();
            ship_load(0);
            ships[1].reactionTime = 5;
            ships[0].disableControls = 0;
            tutorialStage = 0;
            tutorialSubStage = 0;
            fadeTime = -1;
            aSPACTUT.Play();
        }

        public override void repeatedly_execute_always()
        {
            pool_update();
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
            if (tutorialStage == 0)
            {
                surf.WriteAliasedTextMessage(balls[11].x, balls[11].y - 30.0f, 4.0f, "PRESS A AND D]TO ROTATE]");
                surf.WriteAliasedTextMessage(balls[11].x, balls[11].y + 20.0f, 4.0f, "PRESS W]TO THRUST]");
                if (ships[0].haveCue)
                {
                    tutorialStage = 1;
                    tutorialSubStage = 0;
                }
            }
            else if (tutorialStage == 1)
            {
                if (ships[0].haveCue)
                {
                    if (ships[0].charge == 0.0f)
                    {
                        surf.WriteAliasedTextMessage(balls[11].x, balls[11].y - 30.0f, 4.0f, "PRESS AND HOLD S]TO CHARGE A SHOT]");
                    }
                    else 
                    {
                        surf.WriteAliasedTextMessage(balls[11].x, balls[11].y - 20.0f, 4.0f, "ROTATE TO AIM]");
                        surf.WriteAliasedTextMessage(balls[11].x, balls[11].y + 20.0f, 4.0f, "RELEASE S] TO FIRE]");
                        if (ships[0].charge > 10.0f)
                        {
                            if (tutorialSubStage == 0)
                            {
                                tutorialSubStage = 1;
                            }
                            else if (tutorialSubStage == 2)
                            {
                                tutorialSubStage = 3;
                            }
                        }
                    }
                }
                else if (tutorialSubStage == 3)
                {
                    tutorialStage = 2;
                    tutorialSubStage = 0;
                    ballNum = -1;
                }
                else if (settledAfterShot)
                {
                    tutorialSubStage = 2;
                }
            }
            else if (tutorialStage == 2)
            {
                if (settledAfterShot)
                {
                    if (ballNum == -1 || !balls[ballNum].onTable)
                    {
                        ballNum += 1;
                        if (ballNum < 2)
                        {
                            pool_place_ball_random(ballNum);
                            tutorialSubStage = 0;
                        }
                        else 
                        {
                            tutorialStage = 3;
                            tutorialSubStage = 0;
                            balls[10].onTable = false;
                            holdCueBall = true;
                            pool_set_num_ships(2);
                            pool_place_ball_random(12);
                            ships[0].haveCue = false;
                        }
                    }
                    if (ballNum >= 0 && balls[ballNum].onTable && settledAfterShot && ships[0].haveCue)
                    {
                        if (ballNum == 0)
                        {
                            float vx = balls[ballNum].vx;
                            float vy = balls[ballNum].vy;
                            float speed = Maths.Sqrt(vx*vx + vy*vy);
                            if (speed < 0.1f)
                            {
                                pool_calc_best_ball_and_pocket_complex(0);
                                pool_calc_aim(balls[11].x, balls[11].y, ballNum, ships[0].chosenPocket);
                                float dx = aim.sx - balls[11].x;
                                float dy = aim.sy - balls[11].y;
                                float dist = Maths.Sqrt(dx*dx + dy*dy);
                                if (dist > 50.0f)
                                {
                                    surf.WriteAliasedTextMessage(aim.sx, aim.sy - 8.0f, 4.0f, "FLY]HERE]");
                                }
                                if (dist < 100.0f)
                                {
                                    surf.DrawAntialiasedCircle(aim.bx, aim.by, 2.0f);
                                    surf.WriteAliasedTextMessage(aim.bx, aim.by - 20.0f, 4.0f, "AIM HERE]");
                                    surf.DrawAntialiasedLine(aim.bx, aim.by - 10.0f, aim.bx, aim.by);
                                }
                            }
                        }
                        else 
                        {
                            tutorialSubStage += 1;
                            if (tutorialSubStage < 120)
                            {
                                surf.WriteAliasedTextMessage(balls[ballNum].x, balls[ballNum].y + 20.0f, 4.0f, "POT THIS BALL]");
                            }
                        }
                    }
                }
            }
            else if (tutorialStage == 3)
            {
                tutorialSubStage += 1;
                if (tutorialSubStage <= 200)
                {
                    if (tutorialSubStage < 100)
                    {
                        surf.WriteAliasedTextMessage(balls[12].x, balls[12].y - 30.0f, 4.0f, "THIS IS PILOT]KIM KIRK]");
                    }
                    else 
                    {
                        surf.WriteAliasedTextMessage(balls[12].x, balls[12].y + 20.0f, 4.0f, "TRY TO]BEAT HER TO]THE CUE BALL]");
                    }
                    if (tutorialSubStage == 200)
                    {
                        holdCueBall = false;
                    }
                }
                else if (ships[0].haveCue)
                {
                    tutorialStage = 4;
                    pool_place_ball_random(2);
                    tutorialSubStage = 0;
                }
            }
            else if (tutorialStage == 4)
            {
                if (settledAfterShot)
                {
                    if (balls[ballNum].onTable)
                    {
                        if (ships[0].haveCue)
                        {
                            float vx = balls[ballNum].vx;
                            float vy = balls[ballNum].vy;
                            float speed = Maths.Sqrt(vx*vx + vy*vy);
                            if (speed < 0.1f)
                            {
                                tutorialSubStage += 1;
                                if (tutorialSubStage < 120)
                                {
                                    surf.WriteAliasedTextMessage(balls[ballNum].x, balls[ballNum].y + 25.0f, 4.0f, "POT THIS BALL]");
                                }
                            }
                        }
                    }
                    else if (balls[ballNum].shipOwner == 0)
                    {
                        tutorialStage = 5;
                    }
                    else 
                    {
                        ballNum += 1;
                        if (ballNum < 9)
                        {
                            pool_place_ball_random(ballNum);
                            tutorialSubStage = 0;
                        }
                    }
                }
            }
            else if (tutorialStage == 5)
            {
                surf.WriteAliasedTextMessage(160.0f, 100.0f, 4.0f,"CONGRATULATIONS!]]YOU ARE NOW READY TO FACE]THE GREATEST PILOTS IN THE GALAXY.]GOOD LUCK!]");
            }
            if (ships[0].disableControls > 0)
            {
                disableControlsMessage = 80;
            }
            else 
            {
                if (balls[10].onTable && settledAfterShot && tutorialStage < 3)
                {
                    surf.WriteAliasedTextMessage(balls[10].x, balls[10].y + 15.0f, 4.0f, "FLY THE SHIP]OVER THIS BALL]TO COLLECT IT]");
                }
            }
            disableControlsMessage -= 1;
            if (disableControlsMessage > 0)
            {
                surf.WriteAliasedTextMessage(160.0f, 100.0f, 4.0f,"IF YOU COLLIDE WITH ANOTHER BALL]YOUR CONTROLS WILL BE DISABLED]AND YOU WILL DROP THE CUE BALL]");
            }
            else if (!balls[10].onTable && !ships[0].haveCue && !holdCueBall && tutorialStage < 5)
            {
                if (tutorialStage != 3 || tutorialSubStage > 200)
                {
                    surf.WriteAliasedTextMessage(160.0f, 100.0f, 4.0f,"YOU CAN ONLY THRUST]IF THE BALL IS ON THE TABLE]OR IN YOUR SHIP]");
                }
            }
            if (tutorialStage >= 5)
            {
                surf.WriteAliasedTextMessage(160.0f, 190.0f, 4.0f, "PRESS ESC TO RETURN TO THE MENU]");
            }
            if (fadeTime >= 0)
            {
                fadeTime += 1;
                if (fadeTime <= 50)
                {
                    surf.Fade(fadeTime);
                }
            }
            surf.Release();
        }

        public override void on_key_press(eKeyCode key)
        {
            if (key == eKeyEscape && fadeTime == -1)
            {
                fadeTime = 0;
            }
        }

        public override void room_RepExec()
        {
            if (fadeTime >= 50)
            {
                cEgo.ChangeRoom(2);
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

}
