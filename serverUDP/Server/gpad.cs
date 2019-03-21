using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;
namespace Server
{
    class gpad
    {
        public vJoy joystick;
        public vJoy.JoystickState iReport;
        public gpad()
        {
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
        }
        public void set_cntr(uint id, ushort btns)
        {
            uint nbut = 16;
            var array = Convert.ToString(btns, 2).Select(s => s.Equals('1')).ToArray();
            for (int ind = 0; ind < nbut; ind++) {
                var set = (array.Length - ind - 1) < 0 ?false:array[array.Length - ind - 1];
                joystick.SetBtn(set, id, nbut - (uint)ind);
            }
                
        }
        public void aquire(uint id)
        {
            VjdStat status = joystick.GetVJDStatus(id);
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id)))) ;
            else;
        }

    }
}
