using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;
using Color2 = System.Drawing.Color;

namespace Challenjour_Heimerdinger
{
    class Program
    {

        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        private static Spell
            Q, W, E, R, W1, E1;

        private static Menu Menu;
        private static Color Frosty = new Color(255, 153, 51);

        private static Orbwalking.Orbwalker Orbwalker;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Heimerdinger")
                return;

            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 100);

            W1 = new Spell(SpellSlot.W, 1100);
            E1 = new Spell(SpellSlot.E, 2155);

            Q.SetSkillshot(0.5f, 40f, 1100f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);

            W1.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            E1.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);

            Menu = new Menu("Challenjour Heimerdinger", "Challenjour Heimerdinger", true);
            Menu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            orbwalkerMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);
            ts.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("comboQ", "Use Q").SetValue(false));
            comboMenu.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboR", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            comboMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(false));
            harassMenu.AddItem(new MenuItem("harassW", "Use W").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassE", "Use E").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassR", "Use R").SetValue(false));
            harassMenu.AddItem(new MenuItem("Harass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            harassMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu autoHMenu = Menu.AddSubMenu(new Menu("AutoHarass", "Auto Harass"));
            autoHMenu.AddItem(new MenuItem("autoHW", "Use W").SetValue(true));
            autoHMenu.AddItem(new MenuItem("autoHE", "Use E").SetValue(true));
            autoHMenu.AddItem(new MenuItem("autoH", "Auto Harass").SetValue(true));
            autoHMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu ksMenu = Menu.AddSubMenu(new Menu("KS", "KS"));
            ksMenu.AddItem(new MenuItem("ks", "KS").SetValue(true));
            ksMenu.AddItem(new MenuItem("ksW", "Use W").SetValue(true));
            ksMenu.AddItem(new MenuItem("ksE", "Use E").SetValue(true));
            ksMenu.AddItem(new MenuItem("ksWR", "Use R + W").SetValue(false));
            ksMenu.AddItem(new MenuItem("ksER", "Use E + W").SetValue(false));
            ksMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu clearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "Lane Clear"));
            clearMenu.AddItem(new MenuItem("clearW", "Use W").SetValue(true));
            clearMenu.AddItem(new MenuItem("clearE", "Use E").SetValue(true));
            clearMenu.AddItem(new MenuItem("clearManager", "Use Mana Manager").SetValue(true));
            clearMenu.AddItem(new MenuItem("clearMP", "Lane Clear if Mana % >")).SetValue(new Slider(45, 1, 100));
            clearMenu.AddItem(new MenuItem("LaneClear", "Lane Clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            clearMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            fleeMenu.AddItem(new MenuItem("fleeQ", "Use Q").SetValue(true));
            fleeMenu.AddItem(new MenuItem("fleeW", "Use W").SetValue(false));
            fleeMenu.AddItem(new MenuItem("fleeE", "Use E").SetValue(true));
            fleeMenu.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            fleeMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu rMenu = Menu.AddSubMenu(new Menu("R", "R"));
            rMenu.AddItem(new MenuItem("rR", "On = W | Off = E").SetValue(true));
            rMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawE1", "Draw R + E").SetValue(true));
            drawMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Notifications.AddNotification("Challenjour Heimerdinger!", 10000);
            Game.PrintChat("<font color=\"#00FFFF\">Challenjour Heimerdinger by Frosty</font> - <font color=\"#00FFFF\">Loaded!</font>");

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                rR();
                comboQ();
                comboE();
                comboW();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harassQ();
                harassE();
                harassW();
            }

            if (Menu.Item("autoH").GetValue<bool>())
            {
                autoH();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                clearE();
                clearW();
            }

            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                flee();
            }

            if (Menu.Item("ks").GetValue<bool>())
            {
                ks();
            }
        }

        /// <summary>
        /// R
        /// </summary>

        private static void rR()
        {
            var target = TargetSelector.GetTarget(W1.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("rR").IsActive() && R.IsReady() && W.IsReady())
            {
                R.Cast();
                if (target.IsValidTarget(W1.Range))
                    W1.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
            else
            {
                if (R.IsReady() && E.IsReady())
                {
                    R.Cast();
                    if (target.IsValidTarget(E1.Range))
                        E1.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }
            }
        }

        /// <summary>
        /// R End
        /// </summary>
        ///
        /// <summary>
        /// Combo
        /// </summary>

        private static void comboQ()
        {
            if (Q.IsReady() && Menu.Item("comboQ").GetValue<bool>() && Player.CountEnemiesInRange(600) > 0)
            {
                Q.Cast(Player);
            }
        }

        private static void comboE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("comboE").GetValue<bool>())
                E.CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        private static void comboW()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (W.IsReady() && target.IsValidTarget(W.Range) && Menu.Item("comboW").GetValue<bool>())
                W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        /// <summary>
        /// Combo End
        /// </summary>
        /// 
        /// <summary>
        /// Harass
        /// </summary>

        private static void harassQ()
        {
            if (Q.IsReady() && Menu.Item("harassQ").GetValue<bool>() && Player.CountEnemiesInRange(600) > 0)
            {
                Q.Cast(Player);
            }
        }

        private static void harassE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("harassE").GetValue<bool>())
                E.CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        private static void harassW()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (W.IsReady() && target.IsValidTarget(W.Range) && Menu.Item("harassW").GetValue<bool>())
                W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        /// <summary>
        /// Harass End
        /// </summary>
        /// 
        /// <summary>
        /// Auto Harass
        /// </summary>

        private static void autoH()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("autoHW").GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range) && W.IsReady())
            {
                W.CastIfHitchanceEquals(target, HitChance.High);
            }

            if (Menu.Item("autoHE").GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        /// <summary>
        /// Auto Harass End
        /// </summary>
        ///
        /// <summary>
        /// KS
        /// </summary>

        private static void ks()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("ksW").GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range) && W.IsReady() && W.IsKillable(target))
            {
                W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (Menu.Item("ksE").GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range) && E.IsReady() && E.IsKillable(target))
            {
                E.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (Menu.Item("ksWR").GetValue<bool>() && R.IsReady() && W.IsReady() && target.IsValidTarget(W1.Range) && W1.IsKillable(target))
            {
                R.Cast();
                W1.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (Menu.Item("ksER").GetValue<bool>() && R.IsReady() && E.IsReady() && target.IsValidTarget(E1.Range) && E1.IsKillable(target))
            {
                E1.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
        }


        /// <summary>
        /// Lane Clear
        /// </summary>

        private static void clearE()
        {
            var clearMP = Menu.Item("clearMP").GetValue<Slider>().Value;
            var minion = MinionManager.GetMinions(Player.ServerPosition, E.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    E.Width,
                    E.Range);

            if (Menu.Item("clearE").GetValue<bool>() && minion.IsValidTarget() && E.IsReady() && (Player.Mana / Player.MaxMana) * 100 <= clearMP && (Menu.Item("clearManager").GetValue<bool>()))
            {
                E.Cast(farmLocation.Position);
            }
        }

        private static void clearW()
        {
            var clearMP = Menu.Item("clearMP").GetValue<Slider>().Value;
            var minion = MinionManager.GetMinions(Player.ServerPosition, W.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    W.Width,
                    W.Range);

            if (Menu.Item("clearW").GetValue<bool>() && minion.IsValidTarget() && W.IsReady() && (Player.Mana / Player.MaxMana) * 100 <= clearMP && (Menu.Item("clearManager").GetValue<bool>()))
            {
                W.Cast(farmLocation.Position);
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

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (W.IsReady() && Menu.Item("fleeE").GetValue<bool>())
            {
                if (target.IsValidTarget(E.Range))
                    E.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (Q.IsReady() && Menu.Item("fleeQ").GetValue<bool>() && Player.CountEnemiesInRange(600) > 0)
            {
                Q.Cast(Player);
            }

            if (W.IsReady() && Menu.Item("fleeW").GetValue<bool>())
            {
                if (target.IsValidTarget(W.Range))
                    W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
        }

        /// <summary>
        /// Flee End
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

                if (Menu.Item("drawW").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color2.Aqua);
                }

                if (Menu.Item("drawE").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color2.Aqua);
                }

                if (Menu.Item("drawE1").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, 2155, Color2.Aqua);
                }
            }
        }

        /// <summary>
        /// Drawings End
        /// </summary>
    }
}
