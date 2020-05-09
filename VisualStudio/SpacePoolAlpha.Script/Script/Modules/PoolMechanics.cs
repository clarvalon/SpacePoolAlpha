// Module_PoolMechanics - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_PoolMechanics;
using static SpacePoolAlpha.PoolMechanicsStaticRef;
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
    public partial class Module_PoolMechanics
    {
        // Fields
        public int numShips = 2;
        public int numPlayers = 1;
        public int totalBalls = 10;
        public bool haveSound = true;
        public bool holdCueBall;
        public bool cleanTheTable;
        public int firedBullet = -1;
        public int teleported = -1;
        public int haveRecovered = -1;
        public SBall[] balls = CreateAndInstantiateArray<SBall>(NUMBALLS);
        public pocket[] pockets = CreateAndInstantiateArray<pocket>(6);
        public bank[] banks = CreateAndInstantiateArray<bank>(6);
        public SShip[] ships = CreateAndInstantiateArray<SShip>(2);
        public int[] pocketedBalls0 = new int[NUMBALLS];
        public int[] pocketedBalls1 = new int[NUMBALLS];
        public bullet[] bullets = CreateAndInstantiateArray<bullet>(NUMBULLETS);
        public bool settledAfterShot;
        public SAimAssist aim = new SAimAssist();
        public float radius = 5.6f;
        public float pocketRadius = 10.0f;
        public float cr = 1.0f;
        public DynamicSprite bg;
        public DrawingSurface debugSurf;
        public bool broken;

        // Methods
        public void pool_set_num_players(int pNumPlayers)
        {
            numPlayers = pNumPlayers;
        }

        public void pool_set_num_ships(int pNumShips)
        {
            numShips = pNumShips;
        }

        public void pool_set_muted(bool muted)
        {
            haveSound = !muted;
        }

        public void pool_place_ball(int b, float x, float y)
        {
            balls[b].x = x;
            balls[b].y = y;
            balls[b].vx = 0.0f;
            balls[b].vy = 0.0f;
            balls[b].onTable = true;
        }

        public void pool_place_ship_at_cueball(int s)
        {
            int b = ships[s].ball;
            float bestX = 100.0f;
            float bestY = 100.0f;
            float  maxD = 0f;
            int tries = 0;
            while (maxD < 20.0   && tries < 20)
            {
                tries += 1;
                float x = balls[CUEBALL].x + IntToFloat(Random2(60) - 30);
                float y = balls[CUEBALL].y + IntToFloat(Random2(40) - 20);
                if (x < 20.0f)
                    x = 20.0f;
                else if (x > 300.0f)
                    x = 300.0f;
                if (y < 40.0f)
                    y = 40.0f;
                else if (y > 160.0f)
                    y = 160.0f;
                int b2 = 0;
                float minD = 320.0f;
                while (b2 < NUMBALLS)
                {
                    if (b2 != b && b2 != CUEBALL && balls[b2].onTable)
                    {
                        float dx2 = x - balls[b2].x;
                        float dy2 = y - balls[b2].y;
                        float d = Maths.Sqrt(dx2*dx2 + dy2*dy2);
                        if (d < minD)
                        {
                            minD = d;
                        }
                    }
                    b2 += 1;
                }
                if (minD > maxD)
                {
                    maxD = minD;
                    bestX = x;
                    bestY = y;
                }
            }
            balls[b].x = bestX;
            balls[b].y = bestY;
            float dx = bestX - balls[CUEBALL].x;
            float dy = bestY - balls[CUEBALL].y;
            if (dx*dx + dy*dy < 4.0f *radius*radius)
            {
                balls[CUEBALL].onTable = false;
                ships[s].haveCue = true;
                ships[s].haveCueTime = 0;
                aFloop.Play();
            }
        }

        public void pool_place_ball_random(int b)
        {
            balls[b].vx = 0.0f;
            balls[b].vy = 0.0f;
            balls[b].onTable = true;
            float bestX = 100.0f;
            float bestY = 100.0f;
            float  maxD = 0f;
            int tries = 0;
            while (maxD < 80.0   && tries < 20)
            {
                tries += 1;
                float x = IntToFloat(40 + Random2(240));
                float y = IntToFloat(60 + Random2(100));
                int b2 = 0;
                float minD = 320.0f;
                while (b2 < NUMBALLS)
                {
                    if (b2 != b && balls[b2].onTable)
                    {
                        float dx = x - balls[b2].x;
                        float dy = y - balls[b2].y;
                        float d = Maths.Sqrt(dx*dx + dy*dy);
                        if (d < minD)
                        {
                            minD = d;
                        }
                    }
                    b2 += 1;
                }
                if (minD > maxD)
                {
                    maxD = minD;
                    bestX = x;
                    bestY = y;
                }
            }
            balls[b].x = bestX;
            balls[b].y = bestY;
        }

        public void pool_rack_em(int rackType)
        {
            broken = false;
            int b = 0;
            int row = 0;
            if (rackType == 0)
            {
                totalBalls = 9;
                while (row < 5)
                {
                    int col = 0;
                    int numBallsInRow = row;
                    if (row > 2)
                    {
                        numBallsInRow = 4 - row;
                    }
                    while (col <= numBallsInRow)
                    {
                        balls[b].x = 200.0f + IntToFloat(row)*radius*Maths.Sqrt(3.0f);
                        balls[b].y = 100.0f - IntToFloat(numBallsInRow)*radius + 2.0f *IntToFloat(col)*radius;
                        balls[b].onTable = true;
                        balls[b].shipOwner = -1;
                        b += 1;
                        col += 1;
                    }
                    row += 1;
                }
                balls[9].onTable = false;
            }
            else 
            {
                totalBalls = 10;
                while (row < 4)
                {
                    int col = 0;
                    while (col <= row)
                    {
                        balls[b].x = 200.0f + IntToFloat(row)*radius*Maths.Sqrt(3.0f);
                        balls[b].y = 100.0f - IntToFloat(row)*radius + 2.0f *IntToFloat(col)*radius;
                        balls[b].onTable = true;
                        balls[b].shipOwner = -1;
                        b += 1;
                        col += 1;
                    }
                    row += 1;
                }
            }
        }

        public void pool_reset()
        {
            setup_particles();
            int b = 0;
            while (b < NUMBALLS)
            {
                pocketedBalls0[b] = -1;
                pocketedBalls1[b] = -1;
                balls[b].vx = 0.0f;
                balls[b].vy = 0.0f;
                b += 1;
            }
            b = 0;
            while (b < NUMBULLETS)
            {
                bullets[b].active = false;
                b += 1;
            }
            int s = 0;
            while (s < 2)
            {
                balls[11+s].onTable = true;
                balls[11+s].x = 50.0f;
                balls[11+s].y = 50.0f;
                if (numShips > s)
                {
                    balls[11+s].onTable = true;
                }
                else 
                {
                    balls[11+s].onTable = false;
                }
                ships[s].a = 0.0f;
                ships[s].da = 0.0f;
                ships[s].haveCue = false;
                ships[s].charge = 0.0f;
                ships[s].reactionCount = ships[s].reactionTime;
                if (ships[s].chan != null)
                {
                    ships[s].chan.Volume = 1;
                }
                ships[s].numPocketed = 0;
                ships[s].chosenBall = -1;
                ships[s].disableControls = 80;
                ships[s].pfire = false;
                ships[s].pthrust = false;
                ships[s].pleft = false;
                ships[s].pright = false;
                ships[s].haveGun = false;
                ships[s].timeBetweenShots = 40;
                ships[s].haveTeleport = false;
                ships[s].timeBetweenTeleports = 40;
                ships[s].recoverTime = 80;
                s += 1;
            }
            balls[12].y = 150.0f;
        }

        public void pool_setup()
        {
            mouse.Visible = false;
            haveSound = true;
            holdCueBall = false;
            bg = DynamicSprite.CreateFromBackground();
            int b = 0;
            while (b < NUMBALLS)
            {
                balls[b].damp = 0.98f;
                balls[b].onTable = false;
                balls[b].vx = 0.0f;
                balls[b].vy = 0.0f;
                balls[b].type = eBall;
                balls[b].shipOwner = -1;
                b += 1;
            }
            balls[0].colour = Game.GetColorFromRGB(232, 192, 120);
            balls[1].colour = Game.GetColorFromRGB(206, 228, 109);
            balls[2].colour = Game.GetColorFromRGB(67, 139, 161);
            balls[3].colour = Game.GetColorFromRGB(225, 134, 83);
            balls[4].colour = Game.GetColorFromRGB(237, 207, 172);
            balls[5].colour = Game.GetColorFromRGB(255, 233, 167);
            balls[6].colour = Game.GetColorFromRGB(83, 199, 236);
            balls[7].colour = Game.GetColorFromRGB(115, 163, 250);
            balls[8].colour = Game.GetColorFromRGB(160, 192, 192);
            balls[9].colour = Game.GetColorFromRGB(0, 0, 0);
            balls[CUEBALL].x = 100.0f;
            balls[CUEBALL].y = 100.0f;
            balls[CUEBALL].colour = Game.GetColorFromRGB(255, 255, 255);
            balls[CUEBALL].type = eCue;
            balls[11].damp = 0.94f;
            balls[11].colour = Game.GetColorFromRGB(127, 255, 255);
            balls[11].onTable = true;
            balls[11].type = eShip;
            balls[11].shipOwner = 0;
            balls[12].damp = 0.94f;
            balls[12].colour = Game.GetColorFromRGB(255, 255, 127);
            if (numShips > 1)
            {
                balls[12].onTable = true;
            }
            else 
            {
                balls[12].onTable = false;
            }
            balls[12].type = eShip;
            balls[12].shipOwner = 1;
            float bw = 5.0f;
            pockets[0].x = bw;
            pockets[0].y = 20.0f + bw;
            pockets[1].x = 160.0f;
            pockets[1].y = 20.0f;
            pockets[2].x = 320.0f - bw;
            pockets[2].y = 20.0f + bw;
            pockets[3].x = bw;
            pockets[3].y = 180.0f - bw;
            pockets[4].x = 160.0f;
            pockets[4].y = 180.0f;
            pockets[5].x = 320.0f - bw;
            pockets[5].y = 180.0f - bw;
            banks[0].length = 0.5f *(160.0f - 2.0f *pocketRadius - bw) - bw;
            banks[0].x = 2.0f *bw + pocketRadius + banks[0].length;
            banks[0].y = 20.0f;
            banks[0].radius = bw;
            banks[1].x = 319.0f - banks[0].x;
            banks[1].y = 20.0f;
            banks[1].length = banks[0].length;
            banks[1].radius = bw;
            banks[2].x = banks[0].x;
            banks[2].y = 180.0f;
            banks[2].length = banks[0].length;
            banks[2].radius = bw;
            banks[3].x = 319.0f - banks[0].x;
            banks[3].y = 180.0f;
            banks[3].length = banks[0].length;
            banks[3].radius = bw;
            banks[4].length = 0.5f *(160.0f - 2.0f *pocketRadius - 2.0f *bw) - bw;
            banks[4].x = 0.0f;
            banks[4].y = 100.0f;
            banks[4].radius = bw;
            banks[4].vertical = true;
            banks[5].x = 319.0f;
            banks[5].y = 100.0f;
            banks[5].length = banks[4].length;
            banks[5].radius = bw;
            banks[5].vertical = true;
            ships[0].ball = 11;
            ships[0].left = eKeyA;
            ships[0].right = eKeyD;
            ships[0].thrust = eKeyW;
            ships[0].fire = eKeyS;
            if (ships[0].chan == null)
            {
                ships[0].chan = aThrust1.Play();
            }
            ships[0].isComputer = false;
            if (numPlayers < 1)
            {
                ships[0].isComputer = true;
            }
            ships[0].reactionTime = 10;
            ships[0].flyToRange = 50.0f;
            ships[0].aimAngle = 0.99998f;
            ships[0].maxSpeed = 1.6f;
            ships[0].ballSelectionMethod = 2;
            ships[0].astronavigationSkill = 2;
            ships[0].preparationMethod = 2;
            ships[1].ball = 12;
            ships[1].left = eKeyLeftArrow;
            ships[1].right = eKeyRightArrow;
            ships[1].thrust = eKeyUpArrow;
            ships[1].fire = eKeyDownArrow;
            if (ships[1].chan == null)
            {
                ships[1].chan = aThrust2.Play();
            }
            ships[1].isComputer = false;
            if (numPlayers < 2)
            {
                ships[1].isComputer = true;
            }
            ships[1].reactionTime = 80;
            ships[1].flyToRange = 120.0f;
            ships[1].aimAngle = 0.999f;
            ships[1].maxSpeed = 1.0f;
            ships[1].ballSelectionMethod = 0;
            ships[1].astronavigationSkill = 0;
            ships[1].preparationMethod = 0;
            pool_reset();
        }

        public void DrawShipAt(DrawingSurface surf, float x, float y, float ca, float sa)
        {
            float x1 = x - 4.0f *ca - 4.0f *sa;
            float y1 = y - 4.0f *sa + 4.0f *ca;
            float x2 = x - 4.0f *ca + 4.0f *sa;
            float y2 = y - 4.0f *sa - 4.0f *ca;
            float x3 = x + 6.0f *ca;
            float y3 = y + 6.0f *sa;
            surf.DrawAntialiasedLine(x1, y1, x2, y2);
            surf.DrawAntialiasedLine(x2, y2, x3, y3);
            surf.DrawAntialiasedLine(x3, y3, x1, y1);
        }

        public void pool_create_explosion(float x, float y, float vx, float vy, float maxSpeed, int numParticles, int colour)
        {
            int i = 0;
            while (i < numParticles)
            {
                float scale = maxSpeed/100.0f;
                float speed = scale*IntToFloat(Random(200) - 100);
                float partAngle = Maths.DegreesToRadians(IntToFloat(Random(359)));
                float ox = speed*Maths.Cos(partAngle);
                float oy = speed*Maths.Sin(partAngle);
                add_particle(x + ox, y + oy, vx + ox, vy + oy, IntToFloat(Random(3) + 1), IntToFloat(Random(359)), colour);
                i += 1;
            }
        }

        public void ExplodeShip(int shipIndex)
        {
            int b = ships[shipIndex].ball;
            pool_create_explosion(balls[b].x, balls[b].y, balls[b].vx, balls[b].vy, 4.0f, 32, balls[b].colour);
            aExplosion.Play();
        }

        public void DrawShip(DrawingSurface surf, float x, float y, int shipIndex)
        {
            float ca = ships[shipIndex].ca;
            float sa = ships[shipIndex].sa;
            float charge = ships[shipIndex].charge;
            if (!(shipIndex > 0))
            {
                DrawShipAt(surf, x, y, ca, sa);
            }
            else 
            {
                ship_draw(surf, x, y, -sa, ca);
            }
            if (ships[shipIndex].haveCue)
            {
                if (ships[shipIndex].charge > 0.0f)
                {
                    surf.DrawAntialiasedLine(x, y, x + 3.0f *charge*ca, y + 3.0f *charge*sa);
                }
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                surf.DrawAntialiasedCircle(x - ca, y - sa, 2.0f);
            }
        }

        public void DrawBanks(DrawingSurface surf)
        {
            surf.DrawingColor = Game.GetColorFromRGB(178, 100, 0);
            int b = 0;
            while (b < 6)
            {
                if (banks[b].vertical)
                {
                    surf.DrawRectangle(FloatToInt(banks[b].x - banks[b].radius),FloatToInt(banks[b].y - banks[b].length),FloatToInt(banks[b].x + banks[b].radius),FloatToInt(banks[b].y + banks[b].length));
                    surf.DrawAntialiasedFilledCircle(banks[b].x, banks[b].y - banks[b].length, banks[b].radius);
                    surf.DrawAntialiasedFilledCircle(banks[b].x, banks[b].y + banks[b].length, banks[b].radius);
                }
                else 
                {
                    surf.DrawRectangle(FloatToInt(banks[b].x - banks[b].length),FloatToInt(banks[b].y - banks[b].radius),FloatToInt(banks[b].x + banks[b].length),FloatToInt(banks[b].y + banks[b].radius));
                    surf.DrawAntialiasedFilledCircle(banks[b].x - banks[b].length, banks[b].y, banks[b].radius);
                    surf.DrawAntialiasedFilledCircle(banks[b].x + banks[b].length, banks[b].y, banks[b].radius);
                }
                b += 1;
            }
        }

        public void DrawPockets(DrawingSurface surf)
        {
            surf.DrawingColor = Game.GetColorFromRGB(0, 0, 0);
            int p = 0;
            while (p < 6)
            {
                surf.DrawAntialiasedFilledCircle(pockets[p].x, pockets[p].y, pocketRadius);
                p += 1;
            }
        }

        public float steer_at(int i, float x, float y, float wx, float wy)
        {
            float dx = wx-x;
            float dy = wy-y;
            float distanceToTarget = Maths.Sqrt(dx*dx + dy*dy);
            dx = dx/distanceToTarget;
            dy = dy/distanceToTarget;
            float dot = (ships[i].ca*dx + ships[i].sa*dy);
            float ada = Maths.Sqrt(ships[i].da*ships[i].da);
            if (dot < Maths.Cos(Maths.DegreesToRadians(10.0f *ada)))
            {
                float crs = ships[i].sa*dx - ships[i].ca*dy;
                if (crs > 0.0f)
                {
                    ships[i].pleft = true;
                }
                else 
                {
                    ships[i].pright = true;
                }
            }
            return dot;
        }

        public float steer_avoid_balls(int i, float x, float y, float wx, float wy)
        {
            int lastB = 9;
            if (ships[i].haveCue)
            {
                lastB = 12;
            }
            if (ships[i].astronavigationSkill == 1)
            {
                int b2 = 0;
                while (b2 < lastB)
                {
                    if (b2 != CUEBALL && b2 != ships[i].ball)
                    {
                        if (balls[b2].onTable)
                        {
                            float dx = x - balls[b2].x;
                            float dy = y - balls[b2].y;
                            if (test_ray_circle_2d(dx, dy, wx - x, wy - y, 2.0f *radius))
                            {
                                float len = Maths.Sqrt(dx*dx + dy*dy);
                                dx = dx/len;
                                dy = dy/len;
                                return steer_at(i, x, y, balls[b2].x - 3.0f *radius*dy, balls[b2].y + 3.0f *radius*dx);
                            }
                        }
                    }
                    b2 += 1;
                }
                return steer_at(i, x, y, wx, wy);
            }
            int bb = -1;
            int b = 0;
            while (b < lastB)
            {
                if (b != CUEBALL && b != ships[i].ball)
                {
                    if (balls[b].onTable)
                    {
                        float dx = x - balls[b].x;
                        float dy = y - balls[b].y;
                        if (test_ray_circle_2d(dx, dy, wx - x, wy - y, 2.0f *radius))
                        {
                            wx = res.px + balls[b].x;
                            wy = res.py + balls[b].y;
                            bb = b;
                        }
                    }
                }
                b += 1;
            }
            if (bb != -1)
            {
                float dx = x - balls[bb].x;
                float dy = y - balls[bb].y;
                float len = Maths.Sqrt(dx*dx + dy*dy);
                dx = (4.0f *radius*dx)/len;
                dy = (4.0f *radius*dy)/len;
                if ((dy*(wx - x) - dx*(wy - y)) > 0.0f)
                {
                    dx = -dx;
                    dy = -dy;
                }
                wx = balls[bb].x - dy;
                wy = balls[bb].y + dx;
                if (wx < 5.0 || wx > 314.0 || wy < 25.0 || wy > 174.0f)
                {
                    wx = balls[bb].x + dy;
                    wy = balls[bb].y - dx;
                }
            }
            return steer_at(i, x, y, wx, wy);
        }

        public int pool_calc_best_pocket_simple(float x, float y, int bb)
        {
            float bestDot = -1.0f;
            int p = 0;
            int bestPocket = 0;
            while (p < 6)
            {
                float sbx = balls[bb].x - x;
                float sby = balls[bb].y - y;
                float sbd = Maths.Sqrt(sbx*sbx + sby*sby);
                if (sbd > 0.0f)
                {
                    sbx = sbx/sbd;
                    sby = sby/sbd;
                    float bpx = pockets[p].x - balls[bb].x;
                    float bpy = pockets[p].y - balls[bb].y;
                    float bpd = Maths.Sqrt(bpx*bpx + bpy*bpy);
                    bpx = bpx/bpd;
                    bpy = bpy/bpd;
                    float dot = sbx*bpx + sby*bpy;
                    if (dot > bestDot)
                    {
                        bestDot = dot;
                        bestPocket = p;
                    }
                }
                p += 1;
            }
            return bestPocket;
        }

        public int pool_calc_best_pocket_moderate(float x, float y)
        {
            int bestPocket = 0;
            float bestCosAng = 1.0f;
            int p = 0;
            while (p < 6)
            {
                float lx = pockets[p].x;
                float ly = pockets[p].y;
                if (lx < 4.0f)
                    lx = 4.0f;
                else if (lx > 315.0f)
                    lx = 315.0f;
                if (ly < 24.0f)
                    ly = 24.0f;
                else if (ly > 176.0f)
                    ly = 176.0f;
                float rx = lx;
                float ry = ly;
                if (p == 0 || p == 5)
                {
                    lx -= 0.707f *pocketRadius;
                    ly += 0.707f *pocketRadius;
                    rx += 0.707f *pocketRadius;
                    ry -= 0.707f *pocketRadius;
                }
                else if (p == 2 || p == 3)
                {
                    lx -= 0.707f *pocketRadius;
                    ly -= 0.707f *pocketRadius;
                    rx += 0.707f *pocketRadius;
                    ry += 0.707f *pocketRadius;
                }
                else 
                {
                    lx -= pocketRadius;
                    rx += pocketRadius;
                }
                float tlx = lx - x;
                float tly = ly - y;
                float trx = rx - x;
                float ttry = ry - y;
                float tlm = Maths.Sqrt(tlx*tlx + tly*tly);
                tlx = tlx/tlm;
                tly = tly/tlm;
                float trm = Maths.Sqrt(trx*trx + ttry*ttry);
                trx = trx/trm;
                ttry = ttry/trm;
                float cosAng = tlx*trx + tly*ttry;
                if (cosAng < bestCosAng)
                {
                    bestCosAng = cosAng;
                    bestPocket = p;
                }
                p += 1;
            }
            return bestPocket;
        }

        public void pool_calc_aim(float x, float y, int bb, int bp)
        {
            float px = 0f;
            float  py = 0f;
            px = pockets[bp].x;
            py = pockets[bp].y;
            if (px < 4.0f)
                px = 4.0f;
            else if (px > 315.0f)
                px = 315.0f;
            if (py < 24.0f)
                py = 24.0f;
            else if (py > 176.0f)
                py = 176.0f;
            aim.px = px;
            aim.py = py;
            float twx = balls[bb].x;
            float twy = balls[bb].y;
            float dx = px - twx;
            float dy = py - twy;
            float d = Maths.Sqrt(dx*dx + dy*dy);
            aim.bx = twx - 2.0f *radius*dx/d;
            aim.by = twy - 2.0f *radius*dy/d;
            aim.sx = twx - 75.0f *dx/d;
            aim.sy = twy - 75.0f *dy/d;
            if (aim.sx < 10.0f)
                aim.sx = 10.0f;
            else if (aim.sx > 310.0f)
                aim.sx = 310.0f;
            if (aim.sy < 30.0f)
                aim.sy = 30.0f;
            else if (aim.sy > 170.0f)
                aim.sy = 170.0f;
        }

        public void choose_best_ball_closest(int s)
        {
            int bb = -1;
            int gb = 0;
            float closest = 500.0f;
            while (gb < 9)
            {
                if (balls[gb].onTable)
                {
                    float twx = balls[gb].x;
                    float twy = balls[gb].y;
                    float dx = balls[s+11].x - twx;
                    float dy = balls[s+11].y - twy;
                    float d = Maths.Sqrt(dx*dx + dy*dy);
                    if (d < closest)
                    {
                        closest = d;
                        bb = gb;
                    }
                }
                gb += 1;
            }
            ships[s].chosenBall = bb;
        }

        public void pool_calc_best_ball_and_pocket_simple(int s)
        {
            choose_best_ball_closest(s);
            int bb = ships[s].chosenBall;
            if (bb != -1)
            {
                int sb = ships[s].ball;
                ships[s].chosenPocket = pool_calc_best_pocket_simple(balls[sb].x, balls[sb].y, bb);
            }
        }

        public void pool_calc_best_ball_and_pocket_moderate(int s)
        {
            choose_best_ball_closest(s);
            int bb = ships[s].chosenBall;
            if (bb != -1)
            {
                ships[s].chosenPocket = pool_calc_best_pocket_moderate(balls[bb].x, balls[bb].y);
            }
        }

        public void pool_calc_best_ball_and_pocket_complex(int s)
        {
            float bestScore = 10000.0f;
            int bb = -1;
            int bp = -1;
            int b = 0;
            while (b < 9)
            {
                if (balls[b].onTable)
                {
                    int p = 0;
                    while (p < 6)
                    {
                        float dx = pockets[p].x - balls[b].x;
                        float dy = pockets[p].y - balls[b].y;
                        float dist = Maths.Sqrt(dx*dx + dy*dy);
                        float cost = dist;
                        dx = dx/dist;
                        dy = dy/dist;
                        if (p == 1 || p == 4)
                        {
                            if (dx*dx > dy*dy)
                            {
                                cost += 10000.0f *(dx*dx - dy*dy);
                            }
                        }
                        if (ships[s].ballSelectionMethod == 2)
                        {
                            float sx = balls[b].x - balls[11+s].x;
                            float sy = balls[b].y - balls[11+s].y;
                            float sdist = Maths.Sqrt(sx*sx + sy*sy);
                            sx = sx/sdist;
                            sy = sy/sdist;
                            cost += 300.0f *(1.0f - (sx*dx + sy*dy));
                            float ddist = dist - sdist;
                            if (ddist < 0.0f)
                                ddist = -ddist;
                            cost += ddist;
                        }
                        if (ships[s].ballSelectionMethod == 3)
                        {
                            int ob = 0;
                            while (ob < 9)
                            {
                                if (balls[ob].onTable && ob != b)
                                {
                                    if (test_ray_circle_2d(balls[11+s].x - balls[ob].x, balls[11+s].y - balls[ob].x, balls[b].x - 75.0f *dx - balls[11+s].x, balls[b].y - 75.0f *dy - balls[11+s].y, 2.0f *radius))
                                    {
                                        cost += 500.0f;
                                    }
                                    if (test_ray_circle_2d(balls[b].x - balls[ob].x, balls[b].y - balls[ob].y, dx*dist, dy*dist, 2.0f *radius))
                                    {
                                        cost += 1000.0f;
                                    }
                                }
                                ob += 1;
                            }
                        }
                        if (cost < bestScore)
                        {
                            bestScore = cost;
                            bb = b;
                            bp = p;
                        }
                        p += 1;
                    }
                }
                b += 1;
            }
            ships[s].chosenBall = bb;
            ships[s].chosenPocket = bp;
        }

        public void ReadShipInput(int i)
        {
            if (i == 0 && replay_Playback)
            {
                ships[i].pleft = replay_get_left();
                ships[i].pright = replay_get_right();
                ships[i].pthrust = replay_get_thrust();
                ships[i].pfire = replay_get_fire();
                replay_next_frame();
                return;
            }
            if (!ships[i].isComputer)
            {
                if (CurrentInputType == UserInputType.Controller)
                {
                    ships[i].pleft = IsButtonDown(Buttons.DPadLeft) || IsButtonDown(Buttons.LeftThumbstickLeft);
                    ships[i].pright = IsButtonDown(Buttons.DPadRight) || IsButtonDown(Buttons.LeftThumbstickRight);
                    ships[i].pthrust = IsButtonDown(Buttons.DPadUp) || IsButtonDown(Buttons.RightTrigger);
                    ships[i].pfire = IsButtonDown(Buttons.A);
                }
                else
                {
                    ships[i].pleft = IsKeyPressed(ships[i].left);
                    ships[i].pright = IsKeyPressed(ships[i].right);
                    ships[i].pthrust = IsKeyPressed(ships[i].thrust);
                    ships[i].pfire = IsKeyPressed(ships[i].fire);
                }
            }
            int b = ships[i].ball;
            if (!balls[b].onTable || ships[i].disableControls > 0)
            {
                ships[i].pleft = false;
                ships[i].pright = false;
                ships[i].pthrust = false;
                ships[i].pfire = false;
                ships[i].chosenBall = -1;
            }
            if (i == 0 && replay_Record)
            {
                replay_save_input(ships[i].pleft, ships[i].pright, ships[i].pthrust, ships[i].pfire);
            }
            if (!balls[b].onTable || ships[i].disableControls > 0)
            {
                return;
            }
            if (ships[i].isComputer)
            {
                ships[i].pleft = false;
                ships[i].pright = false;
                ships[i].pthrust = false;
                ships[i].pfire = false;
                float x = balls[b].x + balls[b].vx;
                float y = balls[b].y + balls[b].vy;
                if (ships[i].timeBetweenShots > 0)
                {
                    ships[i].timeBetweenShots -= 1;
                }
                if (ships[i].timeBetweenTeleports > 0)
                {
                    ships[i].timeBetweenTeleports -= 1;
                }
                if (ships[1-i].haveCue || (!ships[i].haveCue && !balls[CUEBALL].onTable))
                {
                    if (ships[1-i].haveCue && ships[i].disableControls == 0)
                    {
                        if (ships[i].haveGun)
                        {
                            float pdot = steer_at(i, x, y, balls[12-i].x, balls[12-i].y);
                            if (pdot > 0.99 && ships[i].timeBetweenShots == 0)
                            {
                                int bul = 0;
                                while (bul < NUMBULLETS)
                                {
                                    if (!bullets[bul].active)
                                    {
                                        bullets[bul].active = true;
                                        bullets[bul].x = x;
                                        bullets[bul].y = y;
                                        bullets[bul].vx = 1.5f *ships[i].ca;
                                        bullets[bul].vy = 1.5f *ships[i].sa;
                                        bullets[bul].life = 200;
                                        bullets[bul].owner = i;
                                        bul = NUMBULLETS;
                                        ships[i].timeBetweenShots = 40;
                                        PlaySoundSafePrio(aShot, 100, (int)eAudioPriorityVeryHigh);
                                        if (firedBullet == -1)
                                        {
                                            firedBullet = 0;
                                        }
                                    }
                                    bul += 1;
                                }
                            }
                        }
                        else 
                        {
                            if (ships[i].preparationMethod == 2)
                            {
                                float cdx = 160.0f - x;
                                float cdy = 100.0f - y;
                                if ((cdx*cdx + cdy*cdy) > (50.0f *50.0f))
                                {
                                    steer_at(i, x, y, 160.0f, 100.0f);
                                }
                                else 
                                {
                                    steer_at(i, x, y, balls[12-i].x, balls[12-i].y);
                                }
                            }
                            else if (ships[i].preparationMethod == 1)
                            {
                                steer_at(i, x, y, balls[12-i].x, balls[12-i].y);
                            }
                        }
                    }
                    ships[i].reactionCount = ships[i].reactionTime;
                    return;
                }
                if (ships[i].reactionCount > 0)
                {
                    ships[i].reactionCount -= 1;
                    return;
                }
                if (ships[i].haveTeleport && ships[i].timeBetweenTeleports == 0 && balls[CUEBALL].onTable)
                {
                    float velx = balls[CUEBALL].vx;
                    float vely = balls[CUEBALL].vy;
                    if (velx*velx + vely*vely < 0.1f)
                    {
                        float telx = balls[b].x - balls[CUEBALL].x;
                        float tely = balls[b].y - balls[CUEBALL].y;
                        if (telx*telx + tely*tely > 120.0f *120.0f)
                        {
                            pool_place_ship_at_cueball(i);
                            ships[i].timeBetweenTeleports = 80;
                            PlaySoundSafePrio(aTeleport, 100, (int)eAudioPriorityVeryHigh);
                            if (teleported == -1)
                            {
                                teleported = 0;
                            }
                        }
                    }
                }
                float speed = Maths.Sqrt(balls[b].vx*balls[b].vx + balls[b].vy*balls[b].vy);
                float wx = 0f;
                float  wy = 0f;
                float tx = 0f;
                float  ty = 0f;
                if (balls[CUEBALL].onTable)
                {
                    wx = balls[CUEBALL].x;
                    wy = balls[CUEBALL].y;
                }
                else if (ships[i].haveCue)
                {
                    if (!broken)
                    {
                        tx = balls[4].x - 2.0f;
                        ty = balls[4].y + 4.0f;
                        wx = 50.0f;
                        wy = 130.0f;
                    }
                    else 
                    {
                        if (ships[i].chosenBall == -1 || !balls[ships[i].chosenBall].onTable)
                        {
                            if (ships[i].ballSelectionMethod == 0)
                            {
                                pool_calc_best_ball_and_pocket_simple(i);
                            }
                            else if (ships[i].ballSelectionMethod == 1)
                            {
                                pool_calc_best_ball_and_pocket_moderate(i);
                            }
                            else 
                            {
                                pool_calc_best_ball_and_pocket_complex(i);
                            }
                        }
                        if (ships[i].chosenBall == -1)
                        {
                            ships[i].chosenBall = ships[1-i].ball;
                            ships[i].chosenPocket = pool_calc_best_pocket_moderate(balls[ships[i].chosenBall].x, balls[ships[i].chosenBall].y);
                        }
                        if (ships[i].chosenBall != -1)
                        {
                            pool_calc_aim(x, y, ships[i].chosenBall, ships[i].chosenPocket);
                            tx = aim.bx + balls[ships[i].chosenBall].vx;
                            ty = aim.by + balls[ships[i].chosenBall].vy;
                            wx = aim.sx;
                            wy = aim.sy;
                        }
                    }
                }
                float dx = wx-x;
                float dy = wy-y;
                float distanceToTarget = Maths.Sqrt(dx*dx + dy*dy);
                if (!ships[i].haveCue || distanceToTarget > ships[i].flyToRange)
                {
                    float dot = 1.0f;
                    if (ships[i].astronavigationSkill == 0)
                    {
                        dot = steer_at(i, x, y, wx, wy);
                    }
                    else 
                    {
                        dot = steer_avoid_balls(i, x, y, wx, wy);
                    }
                    float maxSpeed = ships[i].maxSpeed;
                    if (ships[i].haveCue)
                    {
                        maxSpeed = 1.0f;
                    }
                    if (ships[i].charge > 0.0 || (speed < maxSpeed && distanceToTarget > 8.0f *speed && dot > 0.99f))
                    {
                        ships[i].pthrust = true;
                    }
                }
                else 
                {
                    float tdot = steer_at(i, x, y, tx, ty);
                    if (tdot > 0.95f)
                    {
                        ships[i].pfire = true;
                    }
                    gStatusLineLabel.Text = StringFormatAGS("%f", tdot);
                    if (tdot > ships[i].aimAngle && ships[i].charge > 10.0f)
                    {
                        ships[i].pfire = false;
                        ships[i].chosenBall = -1;
                    }
                }
            }
        }

        public void UpdateShipControl(int i)
        {
            ReadShipInput(i);
            if (ships[i].disableControls > 0)
            {
                ships[i].disableControls -= 1;
            }
            bool thrusting = false;
            int b = ships[i].ball;
            if (!balls[b].onTable)
            {
                ships[i].charge = 0.0f;
                ships[i].da = 0.0f;
                ships[i].fireUp = false;
            }
            else 
            {
                if (ships[i].haveCue)
                {
                    ships[i].haveCueTime += 1;
                }
                float dda = 0.3f;
                if (ships[i].pfire)
                {
                    dda = 0.1f;
                }
                if (ships[i].pleft)
                    ships[i].da = ships[i].da - dda;
                if (ships[i].pright)
                    ships[i].da = ships[i].da + dda;
                ships[i].da = 0.94f *ships[i].da;
                ships[i].a = ships[i].a + ships[i].da;
                float ra = Maths.DegreesToRadians(ships[i].a);
                ships[i].sa = Maths.Sin(ra);
                ships[i].ca = Maths.Cos(ra);
                if (ships[i].pfire)
                {
                    if (ships[i].haveCue)
                    {
                        ships[i].charge += 0.5f;
                        if (ships[i].charge > 15.0f)
                            ships[i].charge = 15.0f;
                    }
                    else 
                    {
                        ships[i].fireUp = true;
                    }
                }
                else 
                {
                    ships[i].fireUp = false;
                    if (ships[i].charge > 0.0f)
                    {
                        if (ships[i].charge < 7.5f)
                            ships[i].charge = 7.5f;
                        balls[CUEBALL].x = balls[b].x;
                        balls[CUEBALL].y = balls[b].y;
                        balls[CUEBALL].vx = ships[i].charge*ships[i].ca;
                        balls[CUEBALL].vy = ships[i].charge*ships[i].sa;
                        balls[CUEBALL].onTable = true;
                        balls[CUEBALL].shipOwner = i;
                        ships[i].charge = 0.0f;
                        ships[i].haveCue = false;
                        settledAfterShot = false;
                    }
                    if (ships[i].pthrust && (ships[i].haveCue || balls[CUEBALL].onTable))
                    {
                        balls[b].vx = balls[b].vx + 0.1f *ships[i].ca;
                        balls[b].vy = balls[b].vy + 0.1f *ships[i].sa;
                        pool_create_explosion(balls[b].x - 3.0f *ships[i].ca, balls[b].y - 3.0f *ships[i].sa, -0.5f *ships[i].ca, -0.5f *ships[i].sa, 1.0f, 1, Game.GetColorFromRGB(255, 255, 255));
                        if (haveSound && ships[i].chan != null)
                        {
                            ships[i].chan.Volume = 100;
                        }
                        thrusting = true;
                    }
                }
            }
            if (!thrusting && haveSound && ships[i].chan != null)
            {
                int v = ships[i].chan.Volume - 10;
                if (v <= 0)
                {
                    v = 1;
                }
                ships[i].chan.Volume = v;
            }
        }

        public void draw_background(DrawingSurface surf)
        {
            surf.DrawingColor = Game.GetColorFromRGB(16, 64, 16);
            surf.DrawRectangle(0, 20, 319, 180);
            DrawPockets(surf);
            DrawBanks(surf);
            surf.DrawingColor = Game.GetColorFromRGB(0, 0, 0);
            surf.DrawRectangle(0, 0, 319, 19);
            surf.DrawRectangle(0, 180, 319, 199);
        }

        public void pool_update()
        {
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            debugSurf = surf;
            if (cleanTheTable)
            {
                surf.DrawImage(0, 0, bg.Graphic);
                cleanTheTable = false;
            }
            else 
            {
                surf.DrawImage(0, 0, bg.Graphic, 50);
            }
            UpdateShipControl(0);
            if (numShips > 1)
            {
                UpdateShipControl(1);
            }
            int b = 0;
            while (b < NUMBULLETS)
            {
                if (bullets[b].active)
                {
                    bullets[b].life -= 1;
                    if (bullets[b].life == 0)
                    {
                        bullets[b].active = false;
                    }
                    else 
                    {
                        bullets[b].x += bullets[b].vx;
                        bullets[b].y += bullets[b].vy;
                        surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                        int bl = 0;
                        while (bl < 5)
                        {
                            int rx = Random2(10) - 5;
                            int ry = Random2(10) - 5;
                            if (rx != 0 || ry != 0)
                            {
                                surf.DrawAntialiasedLine(bullets[b].x,bullets[b].y,bullets[b].x + 0.6f *IntToFloat(rx),bullets[b].y + 0.6f *IntToFloat(ry));
                            }
                            bl += 1;
                        }
                        int nonOwner = 1 - bullets[b].owner;
                        float bdx = bullets[b].x - balls[11 + nonOwner].x;
                        float bdy = bullets[b].y - balls[11 + nonOwner].y;
                        if (bdx*bdx + bdy*bdy < radius*radius)
                        {
                            bullets[b].active = false;
                            ships[nonOwner].disableControls = ships[nonOwner].recoverTime;
                            balls[11 + nonOwner].vx += 0.5f *bullets[b].vx;
                            balls[11 + nonOwner].vy += 0.5f *bullets[b].vy;
                            if (ships[nonOwner].haveCue)
                            {
                                ships[nonOwner].haveCue = false;
                                balls[10].onTable = true;
                                balls[10].x = balls[11 + nonOwner].x;
                                balls[10].y = balls[11 + nonOwner].y;
                                balls[10].vx = 0.0f;
                                balls[10].vy = 0.0f;
                            }
                        }
                    }
                }
                b += 1;
            }
            b = 0;
            int touchedCue = -1;
            bool allStop = true;
            while (b < NUMBALLS)
            {
                if (balls[b].onTable)
                {
                    bool inPocket = false;
                    if (balls[b].x < 0.0 || balls[b].x > 319.0 || balls[b].y < 20.0 || balls[b].y > 179.0f)
                    {
                        inPocket = true;
                    }
                    else 
                    {
                        int p = 0;
                        while (p < 6)
                        {
                            float dx = balls[b].x - pockets[p].x;
                            float dy = balls[b].y - pockets[p].y;
                            float off = Maths.Sqrt(dx*dx + dy*dy);
                            if (off < pocketRadius)
                            {
                                inPocket = true;
                                p = 6;
                            }
                            p += 1;
                        }
                    }
                    if (inPocket)
                    {
                        balls[b].onTable = false;
                        if (b < 10)
                        {
                            int shipIndex = balls[b].shipOwner;
                            ships[shipIndex].numPocketed += 1;
                            if (shipIndex == 0)
                            {
                                int pb1 = 0;
                                while (pocketedBalls0[pb1] >= 0)
                                {
                                    pb1 += 1;
                                }
                                pocketedBalls0[pb1] = b;
                            }
                            else 
                            {
                                int pb2 = 0;
                                while (pocketedBalls1[pb2] >= 0)
                                {
                                    pb2 += 1;
                                }
                                pocketedBalls1[pb2] = b;
                            }
                        }
                        else if (b == ships[0].ball)
                        {
                            ships[0].haveCue = false;
                            ships[0].charge = 0.0f;
                        }
                        else if (b == ships[1].ball)
                        {
                            ships[1].haveCue = false;
                            ships[1].charge = 0.0f;
                        }
                        if (haveSound)
                        {
                            int soundIndex = Random(1);
                            AudioClip clip = null;
                            if (soundIndex == 0)
                            {
                                clip = aPocket;
                            }
                            else 
                            {
                                clip = aPocket2;
                            }
                            PlaySoundSafe(clip, 100);
                        }
                    }
                }
                if (balls[b].onTable)
                {
                    int hitBank = -1;
                    int hitBall = -1;
                    float mvx = balls[b].vx;
                    float mvy = balls[b].vy;
                    float nx = 0f;
                    float  ny = 0f;
                    int bk = 0;
                    while (bk < 6)
                    {
                        if (banks[bk].vertical)
                        {
                            if (test_against_vertical_bank(balls[b].x, balls[b].y, mvx, mvy,banks[bk].x, banks[bk].y, banks[bk].length, radius + banks[bk].radius))
                            {
                                mvx = res.px - balls[b].x;
                                mvy = res.py - balls[b].y;
                                nx = res.nx;
                                ny = res.ny;
                                hitBank = bk;
                            }
                        }
                        else 
                        {
                            if (test_against_horizontal_bank(balls[b].x, balls[b].y, mvx, mvy,banks[bk].x, banks[bk].y, banks[bk].length, radius + banks[bk].radius))
                            {
                                mvx = res.px - balls[b].x;
                                mvy = res.py - balls[b].y;
                                nx = res.nx;
                                ny = res.ny;
                                hitBank = bk;
                            }
                        }
                        bk += 1;
                    }
                    int b2 = 0;
                    while (b2 < NUMBALLS)
                    {
                        if (b2 != b && balls[b2].onTable)
                        {
                            if (test_ray_circle_2d(balls[b].x - balls[b2].x, balls[b].y - balls[b2].y, mvx, mvy, 2.0f *radius))
                            {
                                bool shouldBounce = true;
                                if (balls[b].type == eCue && balls[b2].type == eShip)
                                {
                                    shouldBounce = false;
                                    if (ships[b2-11].disableControls == 0)
                                    {
                                        touchedCue = balls[b2].shipOwner;
                                    }
                                }
                                else if (balls[b].type == eShip && balls[b2].type == eCue)
                                {
                                    shouldBounce = false;
                                    if (ships[b-11].disableControls == 0)
                                    {
                                        touchedCue = balls[b].shipOwner;
                                    }
                                }
                                else if (balls[b].type == eShip && (balls[b2].type != eShip || (ships[b-11].haveCue && ships[b-11].haveCueTime > 40)))
                                {
                                    int shipIndex = balls[b].shipOwner;
                                    ships[shipIndex].disableControls = ships[shipIndex].recoverTime;
                                    if (ships[shipIndex].haveCue)
                                    {
                                        ships[shipIndex].haveCue = false;
                                        balls[CUEBALL].x = balls[b].x;
                                        balls[CUEBALL].y = balls[b].y;
                                        balls[CUEBALL].vx = 0.0f;
                                        balls[CUEBALL].vy = 0.0f;
                                        balls[CUEBALL].onTable = true;
                                        balls[CUEBALL].shipOwner = 1 - shipIndex;
                                        if (balls[b2].type == eShip)
                                        {
                                            ships[shipIndex].disableControls += 40;
                                        }
                                    }
                                    if (ships[shipIndex].recoverTime == 10 && haveRecovered == -1)
                                    {
                                        haveRecovered = 0;
                                    }
                                }
                                else if (balls[b2].type == eShip && (balls[b].type != eShip || (ships[b2-11].haveCue && ships[b2-11].haveCueTime > 40)))
                                {
                                    int shipIndex = balls[b2].shipOwner;
                                    ships[shipIndex].disableControls = ships[shipIndex].recoverTime;
                                    if (ships[shipIndex].haveCue)
                                    {
                                        ships[shipIndex].haveCue = false;
                                        balls[CUEBALL].x = balls[b2].x;
                                        balls[CUEBALL].y = balls[b2].y;
                                        balls[CUEBALL].vx = 0.0f;
                                        balls[CUEBALL].vy = 0.0f;
                                        balls[CUEBALL].onTable = true;
                                        balls[CUEBALL].shipOwner = 1 - shipIndex;
                                        if (balls[b].type == eShip)
                                        {
                                            ships[shipIndex].disableControls += 40;
                                        }
                                    }
                                }
                                else if ((balls[b].type == eBall && balls[b2].type == eCue) || (balls[b2].type == eBall && balls[b].type == eCue))
                                {
                                    broken = true;
                                }
                                if (shouldBounce)
                                {
                                    res.px += balls[b2].x;
                                    res.py += balls[b2].y;
                                    mvx = 0.99f *(res.px - balls[b].x);
                                    mvy = 0.99f *(res.py - balls[b].y);
                                    nx = res.nx;
                                    ny = res.ny;
                                    hitBall = b2;
                                }
                            }
                        }
                        b2 += 1;
                    }
                    if (hitBall >= 0)
                    {
                        float dx = -nx;
                        float dy = -ny;
                        float v1a = balls[b].vx * dx + balls[b].vy * dy;
                        float v2a = balls[hitBall].vx * dx + balls[hitBall].vy * dy;
                        if (v1a - v2a > 0.0f)
                        {
                            if (haveSound)
                            {
                                AudioClip clip = null;
                                int soundIndex = Random(5);
                                if (soundIndex == 0)
                                {
                                    clip = aBallhit;
                                }
                                else if (soundIndex == 1)
                                {
                                    clip = aBallhit2;
                                }
                                else if (soundIndex == 2)
                                {
                                    clip = aBallhit3;
                                }
                                else if (soundIndex == 3)
                                {
                                    clip = aBallhit4;
                                }
                                else if (soundIndex == 4)
                                {
                                    clip = aBallhit5;
                                }
                                else if (soundIndex == 5)
                                {
                                    clip = aBallhit6;
                                }
                                int volume = FloatToInt(5.0f *(v1a - v2a));
                                PlaySoundSafe(clip, volume);
                            }
                            pool_create_explosion(0.5f *(balls[b].x + balls[hitBall].x), 0.5f *(balls[b].y + balls[hitBall].y), 0.0f, 0.0f, v1a-v2a, 16, 7);
                            EBallType type1 = balls[b].type;
                            EBallType type2 = balls[hitBall].type;
                            if (type1 == eShip && type2 != eShip)
                            {
                                balls[hitBall].shipOwner = 1 - balls[b].shipOwner;
                            }
                            else if (type1 != eShip && type2 == eShip)
                            {
                                balls[b].shipOwner = 1 - balls[hitBall].shipOwner;
                            }
                            else if (type1 != eShip && type2 != eShip)
                            {
                                float v1 = balls[b].vx*balls[b].vx + balls[b].vy*balls[b].vy;
                                float v2 = balls[hitBall].vx*balls[hitBall].vx + balls[hitBall].vy*balls[hitBall].vy;
                                if (v1 > v2)
                                {
                                    balls[hitBall].shipOwner = balls[b].shipOwner;
                                }
                                else 
                                {
                                    balls[b].shipOwner = balls[hitBall].shipOwner;
                                }
                            }
                            float v1d = 0.5f *((1.0f + cr)*v2a + (1.0f - cr)*v1a) - v1a;
                            float v2d = 0.5f *((1.0f + cr)*v1a + (1.0f - cr)*v2a) - v2a;
                            balls[b].vx += v1d*dx;
                            balls[b].vy += v1d*dy;
                            balls[hitBall].vx += v2d*dx;
                            balls[hitBall].vy += v2d*dy;
                        }
                    }
                    else if (hitBank >= 0)
                    {
                        float vn = balls[b].vx*nx + balls[b].vy*ny;
                        float vnx = vn*nx;
                        float vny = vn*ny;
                        balls[b].vx -= 1.9f *vnx;
                        balls[b].vy -= 1.9f *vny;
                        if (haveSound)
                        {
                            AudioClip clip = null;
                            int soundIndex = Random(1);
                            if (soundIndex == 0)
                            {
                                clip = aBeat1;
                            }
                            else if (soundIndex == 1)
                            {
                                clip = aBeat2;
                            }
                            PlaySoundSafe(clip, 100);
                        }
                    }
                    balls[b].x += mvx;
                    balls[b].y += mvy;
                    balls[b].vx = 0.98f *balls[b].vx;
                    balls[b].vy = 0.98f *balls[b].vy;
                    surf.DrawingColor = balls[b].colour;
                    float speed = balls[b].vx*balls[b].vx + balls[b].vy*balls[b].vy;
                    if (speed > 0.1f)
                    {
                        if (balls[b].type != eShip)
                        {
                            allStop = false;
                        }
                    }
                    else if (speed < 0.01f)
                    {
                        if (b < 10)
                        {
                            surf.DrawAntialiasedVectorText(balls[b].x, balls[b].y, 3.0f, StringFormatAGS("%c", '1' + b));
                        }
                    }
                    if (balls[b].type != eShip)
                    {
                        surf.DrawAntialiasedCircle(balls[b].x, balls[b].y, radius);
                    }
                    else 
                    {
                        int shipIndex = balls[b].shipOwner;
                        if ((ships[shipIndex].disableControls & 7) > 3)
                        {
                            surf.DrawingColor = Game.GetColorFromRGB(255, 0, 0);
                        }
                        DrawShip(surf, balls[b].x, balls[b].y, shipIndex);
                    }
                }
                b += 1;
            }
            update_particles(surf);
            int pb = 0;
            float pocketx = 10.0f;
            while (pocketedBalls0[pb] >= 0)
            {
                b = pocketedBalls0[pb];
                surf.DrawingColor = balls[b].colour;
                surf.DrawAntialiasedVectorText(pocketx, 190.0f, 3.0f, StringFormatAGS("%c", '1' + b));
                surf.DrawAntialiasedCircle(pocketx, 190.0f, radius);
                pocketx += 15.0f;
                pb += 1;
            }
            pb = 0;
            pocketx = 309.0f;
            while (pocketedBalls1[pb] >= 0)
            {
                b = pocketedBalls1[pb];
                surf.DrawingColor = balls[b].colour;
                surf.DrawAntialiasedVectorText(pocketx, 190.0f, 3.0f, StringFormatAGS("%c", '1' + b));
                surf.DrawAntialiasedCircle(pocketx, 190.0f, radius);
                pocketx -= 15.0f;
                pb += 1;
            }
            if (allStop)
            {
                if (balls[CUEBALL].onTable)
                {
                    if (!settledAfterShot)
                    {
                        settledAfterShot = true;
                    }
                }
                else if (!ships[0].haveCue && !ships[1].haveCue && !holdCueBall)
                {
                    pool_place_ball_random(CUEBALL);
                }
                int shipIndex = 0;
                while (shipIndex < numShips)
                {
                    b = ships[shipIndex].ball;
                    if (!balls[b].onTable)
                    {
                        pool_place_ball_random(b);
                    }
                    shipIndex += 1;
                }
            }
            if (settledAfterShot)
            {
                if (balls[CUEBALL].onTable)
                {
                    if (touchedCue != -1)
                    {
                        balls[CUEBALL].onTable = false;
                        ships[touchedCue].haveCue = true;
                        ships[touchedCue].haveCueTime = 0;
                        if (haveSound)
                        {
                            PlaySoundSafe(aFloop, 100);
                        }
                    }
                    else 
                    {
                        float x = balls[CUEBALL].x;
                        float y = balls[CUEBALL].y;
                        int shippy = 0;
                        bool pickedUpBall = false;
                        while (shippy < 2   && !pickedUpBall)
                        {
                            if (ships[shippy].disableControls == 0)
                            {
                                if (balls[11 + shippy].onTable)
                                {
                                    float dx = x - balls[11 + shippy].x;
                                    float dy = y - balls[11 + shippy].y;
                                    if (dx*dx + dy*dy < 4.0f *radius*radius)
                                    {
                                        balls[CUEBALL].onTable = false;
                                        ships[shippy].haveCue = true;
                                        ships[shippy].haveCueTime = 0;
                                        if (haveSound)
                                        {
                                            PlaySoundSafe(aFloop, 100);
                                        }
                                        pickedUpBall = true;
                                    }
                                }
                            }
                            shippy += 1;
                        }
                        if (!pickedUpBall)
                        {
                            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                            surf.DrawAntialiasedLine(x - 2.0f, y - 2.0f, x + 2.0f, y + 2.0f);
                            surf.DrawAntialiasedLine(x - 2.0f, y + 2.0f, x + 2.0f, y - 2.0f);
                        }
                    }
                }
            }
            surf.Release();
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose AGS singular #defines as C# constants (or static getters)
        public const int NUMBALLS = 13;
        public const int CUEBALL = 10;
        public const int NUMBULLETS = 3;

        // Expose Enums and instances of each
        public enum EBallType
        {
            eBall = 0, 
            eCue = 1, 
            eShip = 2
        }
        public const EBallType eBall = EBallType.eBall;
        public const EBallType eCue = EBallType.eCue;
        public const EBallType eShip = EBallType.eShip;

        // Expose PoolMechanics methods so they can be used without instance prefix
        public static void pool_set_num_ships(int pNumShips)
        {
            PoolMechanics.pool_set_num_ships(pNumShips);
        }

        public static void pool_set_num_players(int pNumPlayers)
        {
            PoolMechanics.pool_set_num_players(pNumPlayers);
        }

        public static void pool_setup()
        {
            PoolMechanics.pool_setup();
        }

        public static void pool_rack_em(int rackType)
        {
            PoolMechanics.pool_rack_em(rackType);
        }

        public static void pool_set_muted(bool muted)
        {
            PoolMechanics.pool_set_muted(muted);
        }

        public static void pool_place_ball(int b, float x, float y)
        {
            PoolMechanics.pool_place_ball(b, x, y);
        }

        public static void pool_place_ball_random(int b)
        {
            PoolMechanics.pool_place_ball_random(b);
        }

        public static void pool_update()
        {
            PoolMechanics.pool_update();
        }

        public static void pool_calc_best_ball_and_pocket_complex(int s)
        {
            PoolMechanics.pool_calc_best_ball_and_pocket_complex(s);
        }

        public static void pool_calc_aim(float x, float y, int bb, int bp)
        {
            PoolMechanics.pool_calc_aim(x, y, bb, bp);
        }

        public static void pool_create_explosion(float x, float y, float vx, float vy, float maxSpeed, int numParticles, int colour)
        {
            PoolMechanics.pool_create_explosion(x, y, vx, vy, maxSpeed, numParticles, colour);
        }

        public static void pool_reset()
        {
            PoolMechanics.pool_reset();
        }

        // Expose PoolMechanics variables so they can be used without instance prefix
        public static int totalBalls { get { return PoolMechanics.totalBalls; } set { PoolMechanics.totalBalls = value; } }
        public static bool holdCueBall { get { return PoolMechanics.holdCueBall; } set { PoolMechanics.holdCueBall = value; } }
        public static bool cleanTheTable { get { return PoolMechanics.cleanTheTable; } set { PoolMechanics.cleanTheTable = value; } }
        public static int firedBullet { get { return PoolMechanics.firedBullet; } set { PoolMechanics.firedBullet = value; } }
        public static int teleported { get { return PoolMechanics.teleported; } set { PoolMechanics.teleported = value; } }
        public static int haveRecovered { get { return PoolMechanics.haveRecovered; } set { PoolMechanics.haveRecovered = value; } }
        public static SBall[] balls { get { return PoolMechanics.balls; } set { PoolMechanics.balls = value; } }
        public static SShip[] ships { get { return PoolMechanics.ships; } set { PoolMechanics.ships = value; } }
        public static bool settledAfterShot { get { return PoolMechanics.settledAfterShot; } set { PoolMechanics.settledAfterShot = value; } }
        public static SAimAssist aim { get { return PoolMechanics.aim; } set { PoolMechanics.aim = value; } }

    }

    #endregion

    #region pocket (AGS struct from .asc converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class pocket
    {
        // Fields
        public float x;
        public float y;

    }

    #endregion

    #region bank (AGS struct from .asc converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class bank
    {
        // Fields
        public float x;
        public float y;
        public bool vertical;
        public float length;
        public float radius;

    }

    #endregion

    #region bullet (AGS struct from .asc converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class bullet
    {
        // Fields
        public float x;
        public float y;
        public float vx;
        public float vy;
        public bool active;
        public int life;
        public int owner;

    }

    #endregion

    #region SBall (AGS struct from .ash converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class SBall
    {
        // Fields
        public float x;
        public float y;
        public float vx;
        public float vy;
        public float damp;
        public int colour;
        public bool onTable;
        public EBallType type = default(EBallType);
        public int shipOwner;

    }

    #endregion

    #region SShip (AGS struct from .ash converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class SShip
    {
        // Fields
        public float a;
        public float da;
        public float sa;
        public float ca;
        public bool haveCue;
        public int haveCueTime;
        public bool fireUp;
        public float charge;
        public int ball;
        public bool isComputer;
        public int left;
        public int right;
        public int thrust;
        public int fire;
        public int disableControls;
        public bool pleft;
        public bool pright;
        public bool pthrust;
        public bool pfire;
        public int chosenBall;
        public int chosenPocket;
        public int reactionTime;
        public float aimAngle;
        public float flyToRange;
        public int ballSelectionMethod;
        public int astronavigationSkill;
        public float maxSpeed;
        public int preparationMethod;
        public int reactionCount;
        public int numPocketed;
        public bool haveGun;
        public int timeBetweenShots;
        public bool haveTeleport;
        public int timeBetweenTeleports;
        public int recoverTime;
        public AudioChannel chan;

    }

    #endregion

    #region SAimAssist (AGS struct from .ash converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class SAimAssist
    {
        // Fields
        public float px;
        public float py;
        public float sx;
        public float sy;
        public float bx;
        public float by;

    }

    #endregion

    #region Static class for referencing parent class without prefixing with instance (AGS struct workaround)

    public static class PoolMechanicsStaticRef
    {
        // Static Fields
        public static int numShips { get { return GlobalBase.PoolMechanics.numShips; } set { GlobalBase.PoolMechanics.numShips = value; } }
        public static int numPlayers { get { return GlobalBase.PoolMechanics.numPlayers; } set { GlobalBase.PoolMechanics.numPlayers = value; } }
        public static bool haveSound { get { return GlobalBase.PoolMechanics.haveSound; } set { GlobalBase.PoolMechanics.haveSound = value; } }
        public static pocket[] pockets { get { return GlobalBase.PoolMechanics.pockets; } set { GlobalBase.PoolMechanics.pockets = value; } }
        public static bank[] banks { get { return GlobalBase.PoolMechanics.banks; } set { GlobalBase.PoolMechanics.banks = value; } }
        public static int[] pocketedBalls0 { get { return GlobalBase.PoolMechanics.pocketedBalls0; } set { GlobalBase.PoolMechanics.pocketedBalls0 = value; } }
        public static int[] pocketedBalls1 { get { return GlobalBase.PoolMechanics.pocketedBalls1; } set { GlobalBase.PoolMechanics.pocketedBalls1 = value; } }
        public static bullet[] bullets { get { return GlobalBase.PoolMechanics.bullets; } set { GlobalBase.PoolMechanics.bullets = value; } }
        public static float radius { get { return GlobalBase.PoolMechanics.radius; } set { GlobalBase.PoolMechanics.radius = value; } }
        public static float pocketRadius { get { return GlobalBase.PoolMechanics.pocketRadius; } set { GlobalBase.PoolMechanics.pocketRadius = value; } }
        public static float cr { get { return GlobalBase.PoolMechanics.cr; } set { GlobalBase.PoolMechanics.cr = value; } }
        public static DynamicSprite bg { get { return GlobalBase.PoolMechanics.bg; } set { GlobalBase.PoolMechanics.bg = value; } }
        public static DrawingSurface debugSurf { get { return GlobalBase.PoolMechanics.debugSurf; } set { GlobalBase.PoolMechanics.debugSurf = value; } }
        public static bool broken { get { return GlobalBase.PoolMechanics.broken; } set { GlobalBase.PoolMechanics.broken = value; } }

        // Static Methods
        public static void pool_place_ship_at_cueball(int s)
        {
            GlobalBase.PoolMechanics.pool_place_ship_at_cueball(s);
        }

        public static void pool_reset()
        {
            GlobalBase.PoolMechanics.pool_reset();
        }

        public static void pool_setup()
        {
            GlobalBase.PoolMechanics.pool_setup();
        }

        public static void DrawShipAt(DrawingSurface surf, float x, float y, float ca, float sa)
        {
            GlobalBase.PoolMechanics.DrawShipAt(surf, x, y, ca, sa);
        }

        public static void ExplodeShip(int shipIndex)
        {
            GlobalBase.PoolMechanics.ExplodeShip(shipIndex);
        }

        public static void DrawShip(DrawingSurface surf, float x, float y, int shipIndex)
        {
            GlobalBase.PoolMechanics.DrawShip(surf, x, y, shipIndex);
        }

        public static void DrawBanks(DrawingSurface surf)
        {
            GlobalBase.PoolMechanics.DrawBanks(surf);
        }

        public static void DrawPockets(DrawingSurface surf)
        {
            GlobalBase.PoolMechanics.DrawPockets(surf);
        }

        public static float steer_at(int i, float x, float y, float wx, float wy)
        {
            return GlobalBase.PoolMechanics.steer_at(i, x, y, wx, wy);
        }

        public static float steer_avoid_balls(int i, float x, float y, float wx, float wy)
        {
            return GlobalBase.PoolMechanics.steer_avoid_balls(i, x, y, wx, wy);
        }

        public static int pool_calc_best_pocket_simple(float x, float y, int bb)
        {
            return GlobalBase.PoolMechanics.pool_calc_best_pocket_simple(x, y, bb);
        }

        public static int pool_calc_best_pocket_moderate(float x, float y)
        {
            return GlobalBase.PoolMechanics.pool_calc_best_pocket_moderate(x, y);
        }

        public static void choose_best_ball_closest(int s)
        {
            GlobalBase.PoolMechanics.choose_best_ball_closest(s);
        }

        public static void pool_calc_best_ball_and_pocket_simple(int s)
        {
            GlobalBase.PoolMechanics.pool_calc_best_ball_and_pocket_simple(s);
        }

        public static void pool_calc_best_ball_and_pocket_moderate(int s)
        {
            GlobalBase.PoolMechanics.pool_calc_best_ball_and_pocket_moderate(s);
        }

        public static void ReadShipInput(int i)
        {
            GlobalBase.PoolMechanics.ReadShipInput(i);
        }

        public static void UpdateShipControl(int i)
        {
            GlobalBase.PoolMechanics.UpdateShipControl(i);
        }

        public static void draw_background(DrawingSurface surf)
        {
            GlobalBase.PoolMechanics.draw_background(surf);
        }

        public static void pool_update()
        {
            GlobalBase.PoolMechanics.pool_update();
        }

    }

    #endregion
    
}
