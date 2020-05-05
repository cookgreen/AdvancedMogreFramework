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
using AdvancedMogreFramework.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedMogreFramework.Core
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

            AppState.create<MenuState>(m_pAppStateManager, "MenuState");
            AppState.create<GameState>(m_pAppStateManager, "GameState");
            AppState.create<SinbadState>(m_pAppStateManager, "SinbadState");
            AppState.create<PauseState>(m_pAppStateManager, "PauseState");
            AppState.create<CreditState>(m_pAppStateManager, "CreditState");
            AppState.create<PhysxBasicCubeState>(m_pAppStateManager, "BasicCubeState");
            AppState.create<PhysxCharacterControllerState>(m_pAppStateManager, "CharacterControllerState");
            AppState.create<PhysxClothState>(m_pAppStateManager, "ClothState");
            AppState.create<PhysxNewtonCradleState>(m_pAppStateManager, "NewtonCradleState");
            AppState.create<DrivingCarState>(m_pAppStateManager, "DrivingCarState");
            AppState.create<InventoryDemoState>(m_pAppStateManager, "InventoryDemoState");

            m_pAppStateManager.start(m_pAppStateManager.findByName("MenuState"));
        }

        private AppStateManager m_pAppStateManager;
    }
}
