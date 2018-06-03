/*
-----------------------------------------------------------------------------
This source file is part of AdvancedMogreFramework
For the latest info, see https://github.com/cookgreen/AdvancedMogreFramework
Copyright (c) 2016-2020 Cook Green

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedMogreFramework.Screen
{
    public class ScreenManager
    {
        private IScreen currentScreen;
        private Dictionary<string, IScreen> screens;
        private static ScreenManager instance;
        public event Action OnCurrentScreenExit;
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScreenManager();
                }
                return instance;
            }
        }

        public ScreenManager()
        {
            currentScreen = null;
            screens = new Dictionary<string, IScreen>();
            screens.Add("Credit", new CreditScreen());
        }
        public void ChangeScreen(string screenName)
        {
            if (currentScreen != null)
            {
                currentScreen.Exit();
            }
            if (screens.ContainsKey(screenName))
            {
                currentScreen = screens[screenName];
                currentScreen.OnScreenExit += CurrentScreen_OnScreenExit;
                currentScreen.Init();
                currentScreen.Run();
            }
        }

        private void CurrentScreen_OnScreenExit()
        {
            if (OnCurrentScreenExit != null)
            {
                OnCurrentScreenExit();
            }
        }

        public void Dispose()
        {
            if (currentScreen != null)
            {
                currentScreen.Exit();
            }
        }

        public void UpdateCurrentScreen(float timeSinceLastFrame)
        {
            if (currentScreen != null)
            {
                currentScreen.Update(timeSinceLastFrame);
            }
        }
    }
}
