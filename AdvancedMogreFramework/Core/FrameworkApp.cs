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
    class FrameworkApp : IDisposable
    {
        public FrameworkApp()
        {
            mAppStateManager = null;
        }

        public void Start()
        {
            if (!Framework.Instance.InitOgre("AdvancedMogreFramework"))
		        return;

            Framework.Instance.mLog.LogMessage("Framework initialized!");
 
	        mAppStateManager = new AppStateManager();

            AppState.Create<MenuState>(mAppStateManager, "MenuState");
            AppState.Create<GameState>(mAppStateManager, "GameState");
            AppState.Create<SinbadState>(mAppStateManager, "SinbadState");
            AppState.Create<PauseState>(mAppStateManager, "PauseState");
            AppState.Create<CreditState>(mAppStateManager, "CreditState");
            AppState.Create<PhysxBasicCubeState>(mAppStateManager, "BasicCubeState");
            AppState.Create<PhysxCharacterControllerState>(mAppStateManager, "CharacterControllerState");
            AppState.Create<PhysxClothState>(mAppStateManager, "ClothState");
            AppState.Create<PhysxNewtonCradleState>(mAppStateManager, "NewtonCradleState");
            AppState.Create<DrivingCarState>(mAppStateManager, "DrivingCarState");

            mAppStateManager.Start(mAppStateManager.FindByName("MenuState"));
        }

        public void Dispose()
        {
            mAppStateManager = null;
        }

        private AppStateManager mAppStateManager;
    }
}
