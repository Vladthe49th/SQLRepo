using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public static class MissionPack
    {
        
        public static List<IMission> GetDefaultMissions()
        {
            var missions = new List<IMission>();

           
            var vip = new Target(1, "VIP Politician", 10.ToString());
            var bodyguard = new Target(2, "Bodyguard", 5.ToString());
            var scientist = new Target(3, "Scientist", 2.ToString() );

            // Mission 1
            missions.Add(new Mission(
                "Silent Politics",
                "Eliminate the VIP Politician without raising suspicion above 15.",
                new List<ITarget> { vip },
                (world, agent) => vip.IsAlive == false && vip.SuspicionLevel < 15
            ));

            // Mission 2
            missions.Add(new Mission(
                "Double Kill",
                "Eliminate both VIP Politician and Bodyguard.",
                new List<ITarget> { vip, bodyguard },
                (world, agent) => vip.IsAlive == false && bodyguard.IsAlive == false
            ));

            // Mission 3
            missions.Add(new Mission(
                "Science Sabotage",
                "Take out the Scientist without alerting others.",
                new List<ITarget> { scientist },
                (world, agent) => scientist.IsAlive == false && scientist.SuspicionLevel <= 5
            ));

            return missions;
        }
    }
}
