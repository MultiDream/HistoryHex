using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HistoryHex {
    public class TestMenu : MonoBehaviour {

        public MenuStates.StartTest startTest;
        public MenuStates.MiddleTest middleTest;

        public void TestButton0() {
            startTest.ButtonPressed();
        }

        public void TestButton1() {
            middleTest.GoBackToStart();
        }

        public void TestButton2() {
            middleTest.GoToEndState();
        }
    }
}