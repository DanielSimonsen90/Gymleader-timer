using System;
using System.Timers;
using System.Windows;

namespace GymLeaderTimer
{
    internal class Challenger
    {
        /// <summary>
        /// Name of Challenger
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Whether Challenger is in Reg. Gyms or AGyms
        /// </summary>
        public string GymType { get; set; }
        /// <summary>
        /// Gym on cooldown
        /// </summary>
        public string Gym { get; set; }
        /// <summary>
        /// Timestamp when cooldown is over
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Name: Gym (GymType)\n(Time left | Finished)
        /// </summary>
        /// <returns></returns>
        public string GetToString() => DateTime.Now < EndTime ? ToString() : $"{Name}: {Gym} ({GymType})\nFinished";
        public override string ToString() => $"{Name}: {Gym} ({GymType})\n {(EndTime - DateTime.Now).ToString().Substring(3, 5)}";
    }
}
