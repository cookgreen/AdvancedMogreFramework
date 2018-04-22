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
using Mogre;
using Mogre_Procedural.MogreBites;

namespace AdvancedMogreFramework
{
    public class CreditScreen : IScreen
    {
        private int time;
        private List<Widget> elements;
        private List<string> elementNames;
        private StringVector strCreditLst;
        private float alpha;

        public event Action OnScreenExit;

        public CreditScreen()
        {
            strCreditLst = new StringVector();
            strCreditLst.Add("Advanced Mogre Framework\r\n\r\nCopyRight 2016-2020 Cook Green");
            strCreditLst.Add("Deign: Cook Green\r\n\r\nProgramming: Cook Green");
        }
        public void Init()
        {
            elements = new List<Widget>();
            elementNames = new List<string>();
        }

        public void Run()
        {
        }

        public void Update(float timeSinceLastFrame)
        {
            if (time >= 0 && time <= 2000)
            {
                if (!elementNames.Contains("lbCredit0"))
                {
                    elements.Add(AdvancedMogreFramework.Singleton.m_pTrayMgr.createLabel(TrayLocation.TL_CENTER, "lbCredit0", strCreditLst[0]));
                    elementNames.Add("lbCredit0");
                }
                ColourValue elementColor = elements[0].getOverlayElement().Colour;
                alpha = elementColor.a;
                if (alpha > 0.0f)
                {
                    alpha -= 0.0005f;
                    elements[0].getOverlayElement().Colour = new ColourValue(
                        elementColor.r,
                        elementColor.g,
                        elementColor.b,
                        float.Parse(alpha.ToString("0.00")));
                }
            }
            else if (time > 2000 && time <= 4000)
            {
                if(elementNames.Contains("lbCredit0"))
                {
                    AdvancedMogreFramework.Singleton.m_pTrayMgr.destroyWidget("lbCredit0");
                    elementNames.Remove("lbCredit0");
                }
                if (!elementNames.Contains("lbCredit1"))
                {
                    elements.Add(AdvancedMogreFramework.Singleton.m_pTrayMgr.createLabel(TrayLocation.TL_CENTER, "lbCredit1", strCreditLst[1]));
                    elementNames.Add("lbCredit1");
                }
                ColourValue elementColor = elements[1].getOverlayElement().Colour;
                alpha = elementColor.a;
                if (alpha > 0.0f)
                {
                    alpha -= 0.0005f;
                    elements[1].getOverlayElement().Colour = new ColourValue(
                        elementColor.r,
                        elementColor.g,
                        elementColor.b,
                        float.Parse(alpha.ToString("0.00")));
                }
            }
            else
            {
                OnScreenExit?.Invoke();
            }
            time++;
        }

        public void Exit()
        {
            AdvancedMogreFramework.Singleton.m_pTrayMgr.destroyAllWidgets();
            elements.Clear();
        }
    }
}
