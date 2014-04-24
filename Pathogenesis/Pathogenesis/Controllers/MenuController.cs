﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathogenesis.Models;
using Microsoft.Xna.Framework;

namespace Pathogenesis.Controllers
{
    public class MenuController
    {
        private Dictionary<MenuType, Menu> menus;
        public List<Menu> OpenMenus;

        // Returns the current menu
        public Menu CurMenu
        {
            get
            {
                return OpenMenus.Last();
            }
        }

        public MenuController(ContentFactory factory)
        {
            menus = factory.getMenus();
            OpenMenus = new List<Menu>();
        }

        /*
         * Set the current menu to the specified type
         */
        public void LoadMenu(MenuType type)
        {
            if (OpenMenus.Count > 0)
            {
                Menu current = OpenMenus.Last();
                if (!current.Children.Contains(type))
                {
                    OpenMenus.Clear();
                }
            }
            OpenMenus.Add(menus[type]);
        }

        /*
         * Update menu selection
         */
        public void Update(InputController input_controller)
        {
            if (OpenMenus.Count == 0) return;

            Menu menu = OpenMenus.Last();

        }

        /*
         * Handle all menu selections
         */
        public void HandleMenuInput(GameEngine engine, InputController input_controller)
        {
            if (OpenMenus.Count == 0) return;

            Menu menu = OpenMenus.Last();
            MenuOption option = menu.Options[menu.CurSelection];

            // Handle primary option change
            if (input_controller.DownOnce)
            {
                menu.CurSelection = (int)MathHelper.Clamp(menu.CurSelection + 1, 0, menu.Options.Count - 1);
            }
            if (input_controller.UpOnce)
            {
                menu.CurSelection = (int)MathHelper.Clamp(menu.CurSelection - 1, 0, menu.Options.Count - 1);
            }

            // Handle secondary selection (left right)
            bool secondarySelected = false;
            if (input_controller.LeftOnce)
            {
                option.CurSelection = (int)MathHelper.Clamp(option.CurSelection - 1, 0, option.Options.Count - 1);
                secondarySelected = true;
            }
            if (input_controller.RightOnce)
            {
                option.CurSelection = (int)MathHelper.Clamp(option.CurSelection + 1, 0, option.Options.Count - 1);
                secondarySelected = true;
            }

            if (menu.Type == MenuType.OPTIONS && secondarySelected)
            {
                SoundController sound_controller = engine.getSoundController();
                String selection = option.Options[option.CurSelection].Text;
                switch (option.Text)
                {
                    case "Music":
                        if (selection.Equals("Off"))
                        {
                            sound_controller.MuteSounds(SoundType.MUSIC);
                        }
                        else if (selection.Equals("On"))
                        {
                            sound_controller.UnmuteSounds(SoundType.MUSIC);
                        }
                        break;
                    case "Sound Effects":
                        if (selection.Equals("Off"))
                        {
                            sound_controller.MuteSounds(SoundType.EFFECT);
                        }
                        else if (selection.Equals("On"))
                        {
                            sound_controller.UnmuteSounds(SoundType.EFFECT);
                        }
                        break;
                }
            }

            // Handle back button
            if (input_controller.Back)
            {
                if(menu.Type == MenuType.OPTIONS)
                {
                    OpenMenus.RemoveAt(OpenMenus.Count - 1);
                }
                else if (menu.Type == MenuType.PAUSE)
                {
                    OpenMenus.RemoveAt(OpenMenus.Count - 1);
                    engine.ChangeGameState(GameState.IN_GAME);
                }

            }

            // Handle option selection
            String curSelection = option.Text;
            if (input_controller.Enter)
            {
                switch (menu.Type)
                {
                    case MenuType.MAIN:
                        switch (curSelection)
                        {
                            case "Play":
                                engine.fadeTo(GameState.IN_GAME);
                                break;
                            case "Options":
                                LoadMenu(MenuType.OPTIONS);
                                break;
                            case "Quit":
                                engine.Exit();
                                break;
                        }
                        break;
                    case MenuType.PAUSE:
                        switch (curSelection)
                        {
                            case "Resume":
                                engine.ChangeGameState(GameState.IN_GAME);
                                break;
                            case "Map":
                                break;
                            case "Options":
                                LoadMenu(MenuType.OPTIONS);
                                break;
                            case "Quit to Menu":
                                engine.fadeTo(GameState.MAIN_MENU);
                                break;
                        }
                        break;
                    case MenuType.OPTIONS:
                        switch (curSelection)
                        {
                            case "Back":
                                OpenMenus.Remove(menu);
                                break;
                        }
                        break;
                    case MenuType.WIN:
                        switch (curSelection)
                        {
                            case "Continue":
                                engine.fadeTo(GameState.IN_GAME);
                                break;
                        }
                        break;
                    case MenuType.LOSE:
                        switch (curSelection)
                        {
                            case "Start Over":
                                engine.fadeTo(GameState.IN_GAME);
                                break;
                            case "Quit to Menu":
                                engine.fadeTo(GameState.MAIN_MENU);
                                break;
                        }
                        break;
                }
            }
        }

        public void DrawMenu(GameCanvas canvas, Vector2 center)
        {
            foreach (Menu menu in OpenMenus)
            {
                menu.Draw(canvas, center);
            }
        }
    }
}
