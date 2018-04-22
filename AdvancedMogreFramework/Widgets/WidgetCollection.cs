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
using Mogre;
using Mogre_Procedural.MogreBites;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedMogreFramework.Widgets
{
    public class WidgetCollection : IList<Widget>, IEnumerable
    {
        private List<Widget> widgets;
        private Widget owner;
        public WidgetCollection(Widget owner)
        {
            this.owner = owner;
            widgets = new List<Widget>();
        }
        public Widget this[int index]
        {
            get
            {
                return widgets[index];
            }

            set
            {
                widgets[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return widgets.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(Widget item)
        {
            ((OverlayContainer)owner.getOverlayElement()).AddChild(item.getOverlayElement());
            widgets.Add(item);
        }

        public void Clear()
        {
            widgets.Clear();
        }

        public bool Contains(Widget item)
        {
            return widgets.Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            widgets.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Widget> GetEnumerator()
        {
            return widgets.GetEnumerator();
        }

        public int IndexOf(Widget item)
        {
            return widgets.IndexOf(item);
        }

        public void Insert(int index, Widget item)
        {
            OverlayContainer container = (OverlayContainer)owner.getOverlayElement();
            container.AddChild(item.getOverlayElement());
            widgets.Insert(index, item);
        }

        public bool Remove(Widget item)
        {
            ((OverlayContainer)owner.getOverlayElement()).RemoveChild(item.getOverlayElement().Name);
            return widgets.Remove(item);
        }

        public void RemoveAt(int index)
        {
            widgets.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return widgets.GetEnumerator();
        }
    }
}
