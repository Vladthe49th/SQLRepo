using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    namespace HitmanSimulator.Core
    {
        public interface ITarget
        {
            int Id { get; }
            string Name { get; }
            string Codename { get; }
            int SuspicionLevel { get; set; }
            bool IsAlive { get; }

            void Move();
            void Eliminate();
        }
    }



}
