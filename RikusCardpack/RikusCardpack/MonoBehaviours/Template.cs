using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RikusCardpack.MonoBehaviours
{
    class Template : MonoBehaviour
    {
        private bool _ranOnce = false;
        private int _stackCount = 1;
        public void RunAdder()
        {
            if (_ranOnce)
            {
                _stackCount += 1;

                return;
            }
            _ranOnce = true;
        }
        public void RunRemover()
        {
            _stackCount -= 1;
            if (_stackCount < 1)
            {
                Destroy(this);
            }
        }
    }
}