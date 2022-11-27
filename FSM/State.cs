using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.ai
{
    public class State<T>
    {
        public virtual void Enter(T entityObj) { }
        public virtual void Execute(T entityObj) { }
        public virtual void Exit(T entityObj) { }   
    }
}
