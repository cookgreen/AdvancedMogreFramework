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
using AdvancedMogreFramework.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites;

namespace AdvancedMogreFramework.States
{
    class AppStateManager : AppStateListener,IDisposable
    {
        bool disposed;

        public struct StateInfo
        { 
            public String name;
            public AppState state;
        };
        public AppStateManager()
        {
             mShutdown = false;
        }

         public override void ManageAppState(String stateName, AppState state)
         {
		    StateInfo new_state_info;
		    new_state_info.name = stateName;
		    new_state_info.state = state;
		    mStates.Add(new_state_info);
         }

         public override AppState FindByName(String stateName)
         {
            foreach (StateInfo itr in mStates)
	        {
		        if(itr.name==stateName)
			    return itr.state;
	        }
 
	        return null;
         }

         public void Start(AppState state)
         {
            ChangeAppState(state);
 
	        uint timeSinceLastFrame = 1;
	        uint startTime = 0;
 
	        while(!mShutdown)
	        {
		        if(AdvancedMogreFramework.Instance.mRenderWnd.IsClosed)mShutdown = true;
 
		        WindowEventUtilities.MessagePump();

                if (AdvancedMogreFramework.Instance.mRenderWnd.IsActive)
		        {
                    startTime = AdvancedMogreFramework.Instance.mTimer.MillisecondsCPU;

                    AdvancedMogreFramework.Instance.mKeyboard.Capture();
                    AdvancedMogreFramework.Instance.mMouse.Capture();

                    mActiveStateStack.Last().Update(timeSinceLastFrame * 1.0 / 1000);

                    AdvancedMogreFramework.Instance.mKeyboard.Capture();
                    AdvancedMogreFramework.Instance.mMouse.Capture();

                    AdvancedMogreFramework.Instance.UpdateOgre(timeSinceLastFrame * 1.0 / 1000);

                    if (AdvancedMogreFramework.Instance.mRoot != null)
                    {
                        AdvancedMogreFramework.Instance.mRoot.RenderOneFrame();
                    }
                    timeSinceLastFrame = AdvancedMogreFramework.Instance.mTimer.MillisecondsCPU - startTime;
		        }
		        else
		        {
                    System.Threading.Thread.Sleep(1000);
		        }
	        }

            AdvancedMogreFramework.Instance.mLog.LogMessage("Main loop quit");
         }
         public override void ChangeAppState(AppState state)
         {
             if (mActiveStateStack.Count!=0)
             {
                 mActiveStateStack.Last().Exit();
                 mActiveStateStack.RemoveAt(mActiveStateStack.Count()-1);
             }

             mActiveStateStack.Add(state);
             init(state);
             mActiveStateStack.Last().Enter();
         }
         public override bool PushAppState(AppState state)
         {
             if (mActiveStateStack.Count!=0)
             {
                 if (!mActiveStateStack.Last().Pause())
                     return false;
             }

             mActiveStateStack.Add(state);
             init(state);
             mActiveStateStack.Last().Enter();

             return true;
         }
         public override void PopAppState()
         {
             if (mActiveStateStack.Count != 0)
             {
                 mActiveStateStack.Last().Exit();
                 mActiveStateStack.RemoveAt(mActiveStateStack.Count()-1);
             }

             if (mActiveStateStack.Count != 0)
             {
                 init(mActiveStateStack.Last());
                 mActiveStateStack.Last().Resume();
             }
             else
                 Shutdown();
         }
         public override void PopAllAndPushAppState<T>(AppState state)
        {
            while (mActiveStateStack.Count != 0)
            {
                mActiveStateStack.Last().Exit();
                mActiveStateStack.RemoveAt(mActiveStateStack.Count()-1);
            }

            PushAppState(state);
        }
         public override void PauseAppState()
         {
             if (mActiveStateStack.Count != 0)
             {
                 mActiveStateStack.Last().Pause();
             }

             if (mActiveStateStack.Count() > 2)
             {
                 init(mActiveStateStack.ElementAt(mActiveStateStack.Count() - 2));
                 mActiveStateStack.ElementAt(mActiveStateStack.Count() - 2).Resume();
             }
         }
         public override void Shutdown()
         {
             mShutdown = true;
         }

         protected void init(AppState state)
         {
             AdvancedMogreFramework.Instance.mTrayMgr.setListener(state);
             AdvancedMogreFramework.Instance.mRenderWnd.ResetStatistics();
         }

         protected List<AppState> mActiveStateStack=new List<AppState>();
         protected List<StateInfo> mStates=new List<StateInfo>();
         protected bool mShutdown;

         public void Dispose()
         {
             Dispose(true);
             GC.SuppressFinalize(this);
         }

         protected virtual void Dispose(bool disposing)
         {
             if (disposed)
             {
                 return;
             }
             if (disposing)
             {

                 StateInfo si;

                 while (mActiveStateStack.Count != 0)
                 {
                     mActiveStateStack.Last().Exit();
                     mActiveStateStack.RemoveAt(mActiveStateStack.Count() - 1);
                 }

                 while (mStates.Count != 0)
                 {
                     si = mStates.Last();
                     si.state.Destroy();
                     mStates.RemoveAt(mStates.Count() - 1);
                 }
             }
             disposed = true;
         }
    }
}
