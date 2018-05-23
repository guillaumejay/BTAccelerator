using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTAccelerator.jsonDefinitions
{
    /// <summary>
    /// Json definition for file AudioConstant
    /// 
    /// </summary>
 
        public class V
        {
            public string key { get; set; }
            public string switchValue { get; set; }
            public string voChance { get; set; }
            public int globalCooldown { get; set; }
            public int localCooldown { get; set; }
            public int priority { get; set; }
            public bool blocksSequence { get; set; }
        }

        public class VOEventData
        {
            public string k { get; set; }
            public V v { get; set; }
        }

        public class V2
        {
            public string key { get; set; }
            public string switchValue { get; set; }
            public string voChance { get; set; }
            public int globalCooldown { get; set; }
            public int priority { get; set; }
            public bool blocksSequence { get; set; }
        }

        public class ComputerVOEventData
        {
            public string k { get; set; }
            public V2 v { get; set; }
        }

    /// <summary>
    /// Root object
    /// </summary>
        public class AudioConstants
        {
            public List<VOEventData> VOEventData { get; set; }
            public List<ComputerVOEventData> ComputerVOEventData { get; set; }
            public int VOPlayChance_Always { get; set; }
            public double VOPlayChance_Common { get; set; }
            public double VOPlayChance_Uncommon { get; set; }
            public double VOPlayChance_Rare { get; set; }
            public double QueuedAudioDelay { get; set; }
            public int AttackPreFireDuration { get; set; }
            public double AttackAfterFireDelay { get; set; }
            public int AttackAfterFireDuration { get; set; }
            public int AttackAfterCompletionDuration { get; set; }
            public string audioEmitterPrefab { get; set; }
            public int maxShortAudioEmitters { get; set; }
            public int maxLongAudioEmitters { get; set; }
            public double rallyHealthPercentageThreshold { get; set; }
            public double distanceToTargetRTPCThreshold { get; set; }
            public double audioFadeDuration { get; set; }
            public double ambientVODelay { get; set; }
            public double ambientVOChance { get; set; }
            public bool DEBUG_Always_Play_VO { get; set; }

            public string Accelerated { get; set; }
        }
    }

