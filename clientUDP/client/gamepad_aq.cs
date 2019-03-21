using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class gamepad_aq
    {
        private JoystickState joystickState;
        private Joystick Rjoystick;

        public gamepad_aq() {

            var directInput = new DirectInput();
            var joystickGuid = Guid.Empty;

            // tenta encontrar um gamepad ou joystick
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;
            if (joystickGuid == Guid.Empty)
                throw new Exception(string.Format("Nao encontrou Gamepad, Pressione qqr tecla para sair."));

            // cria o joystick
            joystickState = new JoystickState();
            Rjoystick = new Joystick(directInput, joystickGuid);
            Rjoystick.Properties.BufferSize = 128;
            Rjoystick.Acquire();
            
        }
        public ushort getState()
        {
            Rjoystick.GetCurrentState(ref joystickState);
            ushort a = 0; int index = 0;
            byte nButtons = 12;
            var estado = new bool[4 + nButtons];
            joystickState.Buttons.Take(nButtons).ToArray().CopyTo(estado,0);

            

            estado[nButtons] = joystickState.Y == ushort.MinValue;//up
            estado[nButtons + 1] = joystickState.X == ushort.MaxValue;//right
            estado[nButtons + 2] = joystickState.Y == ushort.MaxValue;//down
            estado[nButtons + 3] = joystickState.X == ushort.MinValue;//left
            nButtons = 16;
            foreach (bool b in estado)
            {
                if (b)
                    a |= (ushort)(1 << (nButtons - 1 - index));
                index++;
            }
            index = 0;
            return a;
        }
    }
}
