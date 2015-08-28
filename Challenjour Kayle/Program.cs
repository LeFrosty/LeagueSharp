using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;
using Color2 = System.Drawing.Color;

namespace Challenjour_Kayle
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
            if (Player.ChampionName != "Kayle")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 0);

            Menu = new Menu("Challenjour Kayle", "Challenjour Kayle", true);
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
            comboMenu.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            comboMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassW", "Use W").SetValue(false));
            harassMenu.AddItem(new MenuItem("harassE", "Use E").SetValue(true));
            harassMenu.AddItem(new MenuItem("Harass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            harassMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu clearMenu = Menu.AddSubMenu(new Menu("LaneClear", "Lane Clear"));
            clearMenu.AddItem(new MenuItem("clearQ", "Use Q").SetValue(true));
            clearMenu.AddItem(new MenuItem("clearE", "Use E").SetValue(true));
            clearMenu.AddItem(new MenuItem("LaneClear", "Lane Clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            clearMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu healMenu = Menu.AddSubMenu(new Menu("Auto Heal", "Auto Heal"));
            healMenu.AddItem(new MenuItem("healMe", "Use W On Self").SetValue(true));
            healMenu.AddItem(new MenuItem("healMeHP", "Use W On Self %")).SetValue(new Slider(70, 1, 100));
            healMenu.AddItem(new MenuItem("healAlly", "Use W On Ally").SetValue(true));
            healMenu.AddItem(new MenuItem("healAllyHP", "Use W On Ally %")).SetValue(new Slider(65, 1, 100));
            healMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

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
            drawMenu.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));
            drawMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Notifications.AddNotification("Challenjour Kayle LOADED!", 10000);
            Game.PrintChat("<font color=\"#00FFFF\">Challenjour Kayle by Frosty</font> - <font color=\"#00FFFF\">Loaded!</font>");

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                comboW();
                comboQ();
                comboE();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harassW();
                harassQ();
                harassE();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                clearQ();
                clearE();
            }

            if (Menu.Item("healMe").GetValue<bool>())
            {
                healMe();
            }

            if (Menu.Item("healAlly").GetValue<bool>())
            {
                healAlly();
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

        private static void comboW()
        {

            var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);

            if (Menu.Item("comboW").GetValue<bool>() && W.IsReady() && target.IsValidTarget())
            {
                W.Cast(Player);
            }
        }

        private static void comboQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("comboQ").GetValue<bool>() && Q.IsReady() && target.IsValidTarget())
            {
                Q.Cast(target);
            }
        }

        private static void comboE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("comboE").GetValue<bool>() && E.IsReady() && target.IsValidTarget())
            {
                E.Cast();
            }
        }

        /// <summary>
        /// Combo End
        /// </summary>
        /// 
        /// <summary>
        /// Harass
        /// </summary>

        private static void harassW()
        {
            var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);

            if (Menu.Item("harassW").GetValue<bool>() && W.IsReady() && target.IsValidTarget())
            {
                W.Cast(Player);
            }
        }

        private static void harassQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("harassQ").GetValue<bool>() && Q.IsReady() && target.IsValidTarget())
            {
                Q.Cast(target);
            }
        }

        private static void harassE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("harassE").GetValue<bool>() && E.IsReady() && target.IsValidTarget())
            {
                E.Cast();
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
                Q.Cast(minion);
            }
        }

        private static void clearE()
        {
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

            if (Menu.Item("clearE").GetValue<bool>() && minion.IsValidTarget() && E.IsReady())
            {
                E.Cast();
            }
        }

        /// <summary>
        /// Lane Clear End
        /// </summary>
        /// 
        /// <summary>
        /// Heal
        /// </summary>

        private static void healMe()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var healMe = Menu.Item("healMe").GetValue<bool>();
            var healMeHP = Menu.Item("healMeHP").GetValue<Slider>().Value;

            if (healMe && (Player.Health / Player.MaxHealth) * 100 <= healMeHP && W.IsReady())
            {
                W.Cast(Player);
            }
        }

        private static void healAlly()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var healAlly = Menu.Item("healAlly").GetValue<bool>();
            var healAllyHP = Menu.Item("healAllyHP").GetValue<Slider>().Value;

            foreach (var Ally in ObjectManager.Get<Obj_AI_Hero>().Where(Ally => Ally.IsAlly && !Ally.IsMe))
            {
                var allys = Menu.Item("useRally" + Ally.CharData);

                if (Player.InFountain() || Player.IsRecalling())
                    return;

                if (healAlly && ((Ally.Health / Ally.MaxHealth) * 100 <= healAllyHP) && W.IsReady() &&
                    Ally.Distance(Player.Position) <= W.Range)
                {
                    if (allys != null && allys.GetValue<bool>())
                    {
                        W.Cast(Ally);
                    }
                }
            }
        }

        /// <summary>
        /// Heal End
        /// </summary>
        ///
        /// <summary>
        /// Flee
        /// </summary>

        private static void flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (W.IsReady())
            {
                W.Cast(Player);
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
                var allys = Menu.Item("rAlly" + Ally.CharData);

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

                if (Menu.Item("drawW").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color2.Aqua);
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
        /// Drawing End
        /// </summary>
    }

}
