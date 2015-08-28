using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;
using Color2 = System.Drawing.Color;

namespace Challenjour_Zilean
{
    class Program
    {

        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        private static Spell
            Q, W, E, R;

        private static Menu Menu;
        private static Color Frosty = new Color(0, 255, 255);

        private static Orbwalking.Orbwalker Orbwalker;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Zilean")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 900);

            Q.SetSkillshot(0.30f, 210f, 2000f, false, SkillshotType.SkillshotCircle);

            Menu = new Menu("Challenjour Zilean", "Challenjour Zilean", true);
            Menu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            orbwalkerMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);
            ts.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboEenemy", "Use E To Slow Enemy").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboEself", "Use E On Yourself").SetValue(false));
            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            comboMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassW", "Use W").SetValue(false));
            harassMenu.AddItem(new MenuItem("harassEenemy", "Use E To Slow Enemy").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassEself", "Use E On Yourself").SetValue(false));
            harassMenu.AddItem(new MenuItem("Harass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            harassMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu clearMenu = Menu.AddSubMenu(new Menu("LaneClear", "Lane Clear"));
            clearMenu.AddItem(new MenuItem("clearQ", "Use Q").SetValue(true));
            clearMenu.AddItem(new MenuItem("clearW", "Use W").SetValue(true));
            clearMenu.AddItem(new MenuItem("LaneClear", "Lane Clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            clearMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            fleeMenu.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            fleeMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu rMenu = Menu.AddSubMenu(new Menu("R", "R"));
            rMenu.AddItem(new MenuItem("rSelf", "R Self").SetValue(true));
            rMenu.AddItem(new MenuItem("rSelfhp", "R Self On % HP").SetValue(new Slider(25, 1, 100)));
            rMenu.AddItem(new MenuItem("rAlly", "R Ally").SetValue(true));
            rMenu.AddItem(new MenuItem("rAllyhp", "R Ally On % HP").SetValue(new Slider(20, 1, 100)));
            rMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));
            drawMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Notifications.AddNotification("Challenjour Zilean LOADED!", 10000);

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                comboE();
                comboEself();
                comboQ();
                comboW();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harassE();
                harassQ();
                harassW();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                clearQ();
                clearW();
            }

            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                flee();
            }

            if (Menu.Item("rSelf").GetValue<bool>())
            {
                rSelf();
            }

            if (Menu.Item("rAlly").GetValue<bool>())
            {
                rAlly();
            }
        }

        /// <summary>
        /// Combo
        /// </summary>

        private static void comboE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("comboEenemy").GetValue<bool>())
                E.Cast(target);
        }

        private static void comboEself()
        {
            if (E.IsReady() && Menu.Item("comboEself").GetValue<bool>())
                E.Cast(Player);
        }

        private static void comboQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Menu.Item("comboQ").GetValue<bool>())
                Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        private static void comboW()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target.HasBuff("ZileanQEnemyBomb") && Menu.Item("comboW").GetValue<bool>())
            {
                W.Cast();
            }
        }

        /// <summary>
        /// Combo End
        /// </summary>
        ///
        /// <summary>
        /// Harass
        /// </summary>

        private static void harassE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("harassE").GetValue<bool>())
                E.Cast(target);
        }

        private static void harassQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Menu.Item("harassQ").GetValue<bool>())
                Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        private static void harassW()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target.HasBuff("ZileanQEnemyBomb") && Menu.Item("harassW").GetValue<bool>())
            {
                W.Cast();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Harass End
        /// </summary>
        /// 
        /// <summary>
        /// Lane Clear
        /// </summary>

        private static void clearQ()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    Q.Width,
                    Q.Range);

            if (Menu.Item("clearQ").GetValue<bool>() && minion.IsValidTarget() && Q.IsReady())
            {
                Q.Cast(farmLocation.Position);
            }
        }

        private static void clearW()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (Menu.Item("clearW").GetValue<bool>() && minion.IsValidTarget() && W.IsReady())
            {
                W.Cast(Player);
            }
        }

        /// <summary>
        /// Lane Clear End
        /// </summary>
        /// 
        /// <summary>
        /// Flee
        /// </summary>

        private static void flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (E.IsReady())
            {
                E.Cast(Player);
            }

            if (W.IsReady())
            {
                W.Cast();
            }
        }

        /// <summary>
        /// Flee End
        /// </summary>
        /// 
        /// <summary>
        /// R
        /// </summary>

        private static void rSelf()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var autoRmeHP = Menu.Item("rSelfhp").GetValue<Slider>().Value;
            var autoRme = Menu.Item("rSelf").GetValue<bool>();

            if (autoRme && (Player.Health / Player.MaxHealth) * 100 <= autoRmeHP && R.IsReady()
                && Player.CountEnemiesInRange(600) > 0)
            {
                R.Cast(Player);
            }
        }

        private static void rAlly()
        {
            var autoRally = Menu.Item("rAlly").GetValue<bool>();
            var RallyHP = Menu.Item("rAllyhp").GetValue<Slider>().Value;

            foreach (var Ally in ObjectManager.Get<Obj_AI_Hero>().Where(Ally => Ally.IsAlly && !Ally.IsMe))
            {
                var allys = Menu.Item("autoRally" + Ally.CharData);

                if (Player.InFountain() || Player.IsRecalling())
                    return;

                if (autoRally && ((Ally.Health / Ally.MaxHealth) * 100 <= RallyHP) && R.IsReady() &&
                    Player.CountEnemiesInRange(900) > 0 && (Ally.Distance(Player.Position) <= R.Range))
                {
                    if (allys != null && allys.GetValue<bool>())
                    {
                        R.Cast(Ally);
                    }
                }
            }
        }

        /// <summary>
        /// R End
        /// </summary>
        /// 
        /// <summary>
        /// Drawings
        /// </summary>

        private static void Drawing_OnDraw(EventArgs args)
        {
            {
                if (Player.IsDead)
                    return;

                if (Menu.Item("drawQ").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color2.Aqua);
                }

                if (Menu.Item("drawE").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color2.Aqua);
                }

                if (Menu.Item("drawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color2.Aqua);
                }

            }
        }

        /// <summary>
        /// Drawings End
        /// </summary>
    
    }
}

