using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.framework.app
{
    public class OgreFrameworkApp
    {
        private AppStateManager appStateManager;

        public void Start()
        {
            if (!OgreFramework.Instance.InitOgre("OgreFramework", null, "OF_log", "resources.cfg"))
                return;

            appStateManager = new AppStateManager();

            AppState.Create<MainMenuState>(appStateManager, "MainMenu");

            appStateManager.Start(appStateManager.FindByName("MainMenu"));
        }
    }
}
