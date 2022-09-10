namespace ZXNSite.Pages
{
    public partial class Ctc
    {
        private double _target = 1;
        private CtcUnits _unit = CtcUnits.ms;

        [System.ComponentModel.DataAnnotations.Range(0, int.MaxValue)]
        double Target
        {
            get { return _target; }
            set
            {
                if (value != _target)
                {
                    _target = value;
                    if (_target <= 0)
                    {
                        _target = 1;
                        StateHasChanged();
                    }
                    UpdateResults();
                }
            }
        }

        CtcUnits Unit
        {
            get { return _unit; }
            set 
            {
                if (value != _unit)
                {
                    _unit = value;
                    UpdateResults();
                }
            }
        }

        List<CtcResult> Results { get; set; } = new();


        public Ctc() : base()
        { 
        }

        protected override Task OnInitializedAsync()
        {
            UpdateResults();
            return base.OnInitializedAsync();
        }

        private void UpdateResults()
        {
            Results.Clear();
            var bestPrescalar = 0;
            var bestTimeConst = 0;
            var bestTarget = 0.0;


            var clocks = new double[] { 28000000/*, 28571429, 29464286, 30000000, 31000000, 32000000, 33000000, 27000000*/ };
            var prescalars = new int[]{ 16, 256 };

            var targetHz = 0.0;
            switch (Unit)
            {
                case CtcUnits.ms: targetHz = 1 / (Target / 1000); break;
                case CtcUnits.Hz: targetHz = Target; break;
                case CtcUnits.KHz: targetHz = Target * 1000; break;
                case CtcUnits.MHz: targetHz = Target * 1000000; break;
            }

            foreach (var clock in clocks) {
                var bestDelta = double.MaxValue;
                foreach (var prescalar in prescalars)
                {
                    for (var timeConst = 1; timeConst <= 256; timeConst++)
                    {
                        var hz = clock / prescalar / timeConst;
                        var delta = Math.Abs(hz - targetHz);
                        if (delta < bestDelta)
                        {
                            bestPrescalar = prescalar;
                            bestTimeConst = timeConst == 256 ? 0 : timeConst;
                            bestTarget = hz;
                            bestDelta = delta;
                        }
                    }
                }
                switch(Unit)
                {
                    case CtcUnits.ms: bestTarget = (1 / bestTarget) * 1000; break;
                    case CtcUnits.Hz: /*bestTarget = bestTarget*/; break;
                    case CtcUnits.KHz: bestTarget = bestTarget / 1000; break;
                    case CtcUnits.MHz: bestTarget = bestTarget / 1000000; break;
                }
                Results.Add(new CtcResult(clock, bestPrescalar, bestTimeConst, bestTarget));
            }
        }
    }

    class CtcResult
    {
        public double Clock { get; set; }
        public int Prescaler { get; set; }
        public int TimeConst { get; set; }
        public double Actual { get; set; }
        public bool Best { get; set; }
        public CtcResult(double clock, int prescaler, int timeConst, double actual)
        {
            Clock = clock;
            Prescaler = prescaler;
            TimeConst = timeConst;
            Actual = actual;
            Best = false;
        }
    }

    enum CtcUnits
    {
        ms,
        Hz,
        KHz,
        MHz
    }
}
