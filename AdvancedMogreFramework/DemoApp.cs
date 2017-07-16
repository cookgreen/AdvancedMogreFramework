﻿/*
-----------------------------------------------------------------------------
This source file is part of AdvancedMogreFramework
For the latest info, see https://github.com/cookgreen/AdvancedMogreFramework
Copyright (c) 2016-2020 Cook Green(randomcode)

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
using System.Text;

namespace Advanced_Mogre_Framework
{
    class DemoApp
    {
        public DemoApp()
        {
            m_pAppStateManager = null;
        }
        ~DemoApp()
        {
            m_pAppStateManager = null;
        }

        public void startDemo()
        {
            AdvancedMogreFramework amf=new AdvancedMogreFramework();
            if (!AdvancedMogreFramework.Singleton.initOgre("AdvancedMogreFramework"))
		        return;

            AdvancedMogreFramework.Singleton.m_pLog.LogMessage("Demo initialized!");
 
	        m_pAppStateManager = new AppStateManager();

            MenuState.create<MenuState>(m_pAppStateManager, "MenuState");
            GameState.create<GameState>(m_pAppStateManager, "GameState");
            SinbadState.create<SinbadState>(m_pAppStateManager, "SinbadState");
            PauseState.create<PauseState>(m_pAppStateManager, "PauseState");
 
	        m_pAppStateManager.start(m_pAppStateManager.findByName("MenuState"));
        }

        private AppStateManager m_pAppStateManager;
    }
}
