using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites;

namespace org.ogre.framework
{
    public class AppStateManager : AppStateListener,IDisposable
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
		        if(OgreFramework.Instance.renderWnd.IsClosed)mShutdown = true;
 
		        WindowEventUtilities.MessagePump();

                if (OgreFramework.Instance.renderWnd.IsActive)
		        {
                    startTime = OgreFramework.Instance.timer.MillisecondsCPU;

                    OgreFramework.Instance.keyboard.Capture();
                    OgreFramework.Instance.mouse.Capture();

                    mActiveStateStack.Last().Update(timeSinceLastFrame * 1.0 / 1000);

                    OgreFramework.Instance.keyboard.Capture();
                    OgreFramework.Instance.mouse.Capture();

                    OgreFramework.Instance.UpdateOgre(timeSinceLastFrame * 1.0 / 1000);

                    if (OgreFramework.Instance.root != null)
                    {
                        OgreFramework.Instance.root.RenderOneFrame();
                    }
                    timeSinceLastFrame = OgreFramework.Instance.timer.MillisecondsCPU - startTime;
		        }
		        else
		        {
                    System.Threading.Thread.Sleep(1000);
		        }
	        }

            OgreFramework.Instance.log.LogMessage("Main loop quit");
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
             OgreFramework.Instance.trayMgr.setListener(state);
             OgreFramework.Instance.renderWnd.ResetStatistics();
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
