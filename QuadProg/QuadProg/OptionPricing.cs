using System;
using QLNet;

namespace QuadProg
{
    public class OptionPricing
    {

        private VanillaOption europeanOption;

        private double premium;

        public double Premium
        {
            get { return Math.Round(premium, 4); }
        }

        public OptionPricing(Date today,
                             Date expiry,
                             double spot, double strike, double rf,
                             double yield, double volatility,
                             int nbsim, int nbstep, DayCounter dc, Calendar calendar)
        {
            Handle<Quote> underlyingH = new Handle<Quote>(new SimpleQuote(spot));

            YieldTermStructure yts = new FlatForward(today, rf, new ActualActual());
            Handle<YieldTermStructure> flatTermStructure = new Handle<YieldTermStructure>(yts);

            Handle<YieldTermStructure> flatDividendTs = new Handle<YieldTermStructure>(new FlatForward(today, yield, dc));
            Handle<BlackVolTermStructure> flatVolTs = new
                Handle<BlackVolTermStructure>(
                new BlackConstantVol(today, calendar, volatility,dc));

            BlackScholesMertonProcess bsmProcess =
                new BlackScholesMertonProcess(underlyingH, flatDividendTs,
                                              flatTermStructure, flatVolTs);

            StrikedTypePayoff payoff = new PlainVanillaPayoff(Option.Type.Call, strike);
            europeanOption = new VanillaOption(payoff, new EuropeanExercise(expiry));

            IPricingEngine mcengine2 =
                new MakeMCEuropeanEngine<LowDiscrepancy>(bsmProcess)
                    .withSteps(nbstep)
                    .withSamples(nbsim)
                    .value();

            europeanOption.setPricingEngine(mcengine2);
        }

        public void Calculate()
        {
            
            premium = europeanOption.NPV();
            //premium = europeanOption.NPV();
        }
    }
}