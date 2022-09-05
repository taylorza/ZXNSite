namespace ZXNSite.Pages
{
    public partial class Uart
    {
        private readonly uint[] _videoTimings = {28000000,28571429,29464286,30000000,31000000,32000000,33000000,27000000};
        private readonly uint[] _bauds = { 115200, 57600, 38400, 31250, 28800, 19200, 9600, 4800, 2400 };
                                
        private uint _targetBaud = 115200;
        uint VideoTiming { get; set; } = 28000000;
        uint TargetBaud
        {
            get { return _targetBaud; }
            set
            {
                if (value != _targetBaud)
                {
                    _targetBaud = value;
                    if (_targetBaud < 2400)
                    {
                        _targetBaud = 2400;
                        StateHasChanged();
                    }
                }
            }
        }
    }
}
