namespace Bouineur2000.lib
{
    using System;
    using Microsoft.SPOT.Hardware;

    public class RCSwitch
    {
        #region fields

        private OutputPort _pin { get; set; }
        private int _delay;
        private int _repeat;

        #endregion

        #region ctor

        public RCSwitch(Cpu.Pin pin)
            : this(pin, 469, 10)
        { }

        public RCSwitch(Cpu.Pin pin, int delay, int repeat)
        {
            _repeat = repeat;
            _pin = new OutputPort(pin, false);
            _delay = delay;
        }

        #endregion

        #region private

        private void send0()
        {
            _pin.Write(true);
            delayMicroSeconds(_delay * 1);

            _pin.Write(false);
            delayMicroSeconds(_delay * 3);

            _pin.Write(true);
            delayMicroSeconds(_delay * 1);

            _pin.Write(false);
            delayMicroSeconds(_delay * 3);
        }

        private void send1()
        {
            _pin.Write(true);
            delayMicroSeconds(_delay * 3);

            _pin.Write(false);
            delayMicroSeconds(_delay * 1);

            _pin.Write(true);
            delayMicroSeconds(_delay * 3);

            _pin.Write(false);
            delayMicroSeconds(_delay * 1);
        }

        private void sendF()
        {
            _pin.Write(true);
            delayMicroSeconds(_delay * 1);

            _pin.Write(false);
            delayMicroSeconds(_delay * 3);

            _pin.Write(true);
            delayMicroSeconds(_delay * 3);

            _pin.Write(false);
            delayMicroSeconds(_delay * 1);
        }

        private void sendSync()
        {
            _pin.Write(true);
            delayMicroSeconds(_delay * 1);

            _pin.Write(false);
            delayMicroSeconds(_delay * 31);
        }

        private void delayMicroSeconds(int microSeconds)
        {
            var started = DateTime.Now;

            int ticks = microSeconds * 10;

            var diff = DateTime.Now - started;

            while (diff.Ticks < ticks)
            {
                diff = DateTime.Now - started;
            }
        }

        private string GetCodeWord(int addressCode, int channelCode, bool status)
        {
            var code = new string[5] { "FFFF", "0FFF", "F0FF", "FF0F", "FFF0" };

            if (addressCode < 1 || addressCode > 4 || channelCode < 1 || channelCode > 4)
                return string.Empty;

            var statusCode = status == true ? "FF" : "F0";

            return code[addressCode] + code[channelCode] + "FF" + statusCode + "S";
        }

        private string GetCodeWord2(string group, int channelCode, bool status)
        {
            var code = new string[6] { "FFFFF", "0FFFF", "F0FFF", "FF0FF", "FFF0F", "FFFF0" };

            if (group.Length != 5 || channelCode < 1 || channelCode > 5)
            {
                return string.Empty;
            }

            string addressCode = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                if (group[i] == '0')
                    addressCode += 'F';
                else
                    addressCode += '0';
            }

            var statusCode = status == true ? "FF" : "F0";

            return string.Concat(addressCode, code[channelCode], statusCode, "S");
        }

        #endregion

        #region public

        public void SwitchOn(int addressCode, int channelCode)
        {
            var s = GetCodeWord(addressCode, channelCode, true);
            SendTriState(s);
        }

        public void SwitchOn(string group, int channel)
        {
            var s = GetCodeWord2(group, channel, true);
            SendTriState(s);
        }

        public void SwitchOff(int addressCode, int channelCode)
        {
            var s = GetCodeWord(addressCode, channelCode, false);
            SendTriState(s);
        }

        public void SwitchOff(string group, int channel)
        {
            var s = GetCodeWord2(group, channel, false);
            SendTriState(s);
        }

        public void SendTriState(string codeWord)
        {
            for (int r = 0; r < _repeat; r++)
            {
                for (int i = 0; i <= 12; i++)
                {
                    switch (codeWord[i])
                    {
                        case '0':
                            send0();
                            break;
                        case 'F':
                            sendF();
                            break;
                        case '1':
                            send1();
                            break;
                        case 'S':
                            sendSync();
                            break;
                        default:
                            throw new Exception("Invalid code char");
                            break;
                    }
                }
            }
        }      

        #endregion
    }
}
