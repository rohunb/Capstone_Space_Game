﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_System_Workshop
{
    class BattleRecorder
    {
        private Queue<BattleEvent> eventLog;

        public void OnBattleEvent(BattleEvent be)
        {

        }

        BattleReport GenerateBattleReport()
        {
            return new BattleReport();
        }
    }
}
