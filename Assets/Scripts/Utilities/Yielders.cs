﻿using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Utilities {
    public static class Yielders {
        private static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        private static WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private static Dictionary<float, WaitForSeconds> _waitTimes = new();

        public static WaitForEndOfFrame WaitForEndOfFrame { get { return _waitForEndOfFrame; } }
        public static WaitForFixedUpdate WaitForFixedUpdate { get { return _waitForFixedUpdate; } }
        public static WaitForSeconds WaitForSeconds(float seconds) {
            if (!_waitTimes.ContainsKey(seconds)) {
                _waitTimes.Add(seconds, new WaitForSeconds(seconds));
            }
            return _waitTimes[seconds];
        }
    }
}