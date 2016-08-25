using System;
using System.Collections.Generic;
using System.Text;

namespace Mogre_Advanced_Framework
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
            PauseState.create<PauseState>(m_pAppStateManager, "PauseState");
 
	        m_pAppStateManager.start(m_pAppStateManager.findByName("MenuState"));
        }

        private AppStateManager m_pAppStateManager;
    }
}
