using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public interface ITarget
    {
        int Id { get; }
        string Name { get; }
        int SuspicionLevel { get; set; }
        bool IsAlive { get; }

        void Move();
    }

}
