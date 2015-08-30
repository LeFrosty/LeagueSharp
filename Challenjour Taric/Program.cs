using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = SharpDX.Color;
using Color2 = System.Drawing.Color;

namespace Challenjour_Taric
{
    class Program
    {

        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        private static Spell
            Q, W, E, R;

        private static Menu Menu;
        private static Color Frosty = new Color(255, 153, 51);

        private static Orbwalking.Orbwalker Orbwalker;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Taric")
                return;

            Q = new Spell(SpellSlot.Q, 750);
            W = new Spell(SpellSlot.W, 375);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 375);

            Menu = new Menu("Challenjour Taric", "Challenjour Taric", true);
            Menu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            orbwalkerMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);
            ts.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboR", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            comboMenu.Item("Combo").SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);
            comboMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("harassW", "Use W").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassE", "Use E").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassR", "Use R").SetValue(false));
            harassMenu.AddItem(new MenuItem("Harass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            harassMenu.Item("Harass").SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);
            harassMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu ksMenu = Menu.AddSubMenu(new Menu("KS", "KS"));
            ksMenu.AddItem(new MenuItem("ks", "KS").SetValue(false));
            ksMenu.AddItem(new MenuItem("ksW", "Use W").SetValue(false));
            ksMenu.AddItem(new MenuItem("ksE", "Use E").SetValue(false));
            ksMenu.Item("ks").SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);
            ksMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            fleeMenu.AddItem(new MenuItem("fleeE", "Use E").SetValue(true));
            fleeMenu.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            fleeMenu.Item("Flee").SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);
            fleeMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu healMenu = Menu.AddSubMenu(new Menu("AutoHeal", "Auto Heal"));
            healMenu.AddItem(new MenuItem("healMe", "Auto Heal Self").SetValue(true));
            healMenu.AddItem(new MenuItem("healMeHP", "Heal Self When HP % <=").SetValue(new Slider(35, 1, 100)));
            healMenu.AddItem(new MenuItem("healFriends", "Auto Heal Allies").SetValue(true));
            healMenu.AddItem(new MenuItem("healFriendsHP", "Heal Allies When HP % <=").SetValue(new Slider(65, 1, 100)));
            healMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawW", "Draw W").SetValue(false));
            drawMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(false));
            drawMenu.SetFontStyle(System.Drawing.FontStyle.Regular, Frosty);

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Notifications.AddNotification("Challenjour Taric!", 10000);
            Game.PrintChat("<font color=\"#FF9933\">Challenjour Taric by Frosty</font> - <font color=\"#FF9933\">Loaded!</font>");

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
                comboW();
                comboR();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harassE();
                harassW();
                harassR();
            }

            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                flee();
            }

            if (Menu.Item("ks").GetValue<bool>())
            {
                ks();
            }

            if (Menu.Item("healMe").GetValue<bool>())
            {
                healMe();
            }

            if (Menu.Item("healMe").GetValue<bool>())
            {
                healAlly();
            }
        }

        /// <summary>
        /// Combo
        /// </summary>

        private static void comboE()
        {

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("comboE").GetValue<bool>() && E.IsReady())
            {
                E.Cast(target);
            }
        }

        private static void comboW()
        {

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("comboW").GetValue<bool>() && W.IsReady() && target.IsValidTarget())
            {
                W.Cast();
            }
        }

        private static void comboR()
        {

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("comboR").GetValue<bool>() && R.IsReady() && target.IsValidTarget())
            {
                R.Cast();
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

            if (Menu.Item("harassE").GetValue<bool>() && E.IsReady() && target.IsValidTarget())
            {
                E.Cast(target);
            }
        }

        private static void harassW()
        {

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("harassW").GetValue<bool>() && W.IsReady() && target.IsValidTarget())
            {
                W.Cast();
            }
        }

        private static void harassR()
        {

            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("harassR").GetValue<bool>() && R.IsReady() && target.IsValidTarget())
            {
                R.Cast();
            }
        }

        /// <summary>
        /// Harass End
        /// </summary>
        /// 
        /// <summary>
        /// Flee
        /// </summary>

        private static void flee()
        {

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && Menu.Item("fleeE").GetValue<bool>())
            {
                if (target.IsValidTarget(E.Range))
                    E.Cast(target);

            }
        }

        /// <summary>
        /// Flee End
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
                W.Cast();
            }

            if (Menu.Item("ksE").GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range) && E.IsReady() && E.IsKillable(target))
            {
                E.Cast(target);
            }
        }

        /// <summary>
        /// KS End
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
                Q.Cast(Player);
            }
        }

        private static void healAlly()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var healFriends = Menu.Item("healFriends").GetValue<bool>();
            var healFriendsHP = Menu.Item("healFriendsHP").GetValue<Slider>().Value;

            foreach (var Ally in ObjectManager.Get<Obj_AI_Hero>().Where(Ally => Ally.IsAlly && !Ally.IsMe))

            if (healFriends && (Player.Health / Player.MaxHealth) * 100 <= healFriendsHP && W.IsReady())
            {
                Q.Cast(Ally);
            }
        }

        /// <summary>
        /// Heal End
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
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color2.Goldenrod);
                }

                if (Menu.Item("drawW").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color2.Goldenrod);
                }

                if (Menu.Item("drawE").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color2.Goldenrod);
                }

                if (Menu.Item("drawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color2.Goldenrod);
                }
            }
        }

        /// <summary>
        /// Drawings End
        /// </summary>
    }
}
