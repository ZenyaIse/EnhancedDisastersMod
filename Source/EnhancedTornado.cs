﻿using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

namespace EnhancedDisastersMod
{
    public class EnhancedTornado : EnhancedDisaster
    {
        public class Data : SerializableDataCommon, IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.container.Tornado;
                serializeCommonParameters(s, d);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.container.Tornado;
                deserializeCommonParameters(s, d);
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("Tornado");
            }
        }

        public int MaxProbabilityMonth = 5;

        public EnhancedTornado()
        {
            DType = DisasterType.Tornado;
            BaseOccurrencePerYear = 1.0f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;

            calmDays = 360;
            probabilityWarmupDays = 180;
            intensityWarmupDays = 180;
        }

        protected override float getCurrentOccurrencePerYear_local()
        {
            if (Singleton<WeatherManager>.instance.m_currentFog > 0)
            {
                return 0;
            }

            DateTime dt = Singleton<SimulationManager>.instance.m_currentGameTime;
            int delta_month = Math.Abs(dt.Month - MaxProbabilityMonth);
            if (delta_month > 6) delta_month = 12 - delta_month;

            float occurrence = base.getCurrentOccurrencePerYear_local() * (1f - delta_month / 6f);

            return occurrence;
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as TornadoAI != null;
        }

        public override string GetName()
        {
            return "Tornado";
        }
    }
}
