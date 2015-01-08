﻿using System;
using tradeStrategiesFrame.AverageConstructors;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    class DerivativeDecisionStrategy : DecisionStrategy
    {
        public Derivative[] derivatives { get; set; }

        public DerivativeDecisionStrategy(Machine machine)
            : base(machine)
        {
            initDerivatives();
        }

        protected override Position.Direction determineTradeDirection(int start)
        {
            if (start < 1)
                return Position.Direction.None;

            Derivative derivative = buildDerivative(start);
            
            return crossOperationFor(derivative);
        }

        protected override void addAncillaryRequisites(int start)
        {
            machine.addCandleRequisite("averageDerivative", Math.Round(derivatives[start].averageDerivative * 1000, 4).ToString(), start);
        }

        protected Position.Direction crossOperationFor(Derivative derivative)
        {
            if (derivative == null)
                return Position.Direction.None;

            if (derivative.averageDerivative > 0)
                return Position.Direction.Buy;

            if (derivative.averageDerivative < 0)
                return Position.Direction.Sell;

            return Position.Direction.None;
        }

        protected virtual void addDerivative(int start, int depth)
        {
            AverageConstructor constructor = AverageConstructorFactory.createConstructor();

            derivatives[start].averageValue = constructor.average(machine.getCandles(), start, depth);

            derivatives[start].derivative = (derivatives[start].averageValue - derivatives[start - 1].averageValue) / derivatives[start].averageValue;

            derivatives[start].averageDerivative = constructor.average(derivatives, start, depth);
        }

        protected Derivative buildDerivative(int start)
        {
            addDerivative(start, machine.minDepth);

            return derivatives[start];
        }

        public override void readParamsFrom(string xml)
        {
        }

        public void initDerivatives()
        {
            derivatives = new Derivative[machine.portfolio.candles.Length];

            for (int i = 0; i < derivatives.Length; i++)
                derivatives[i] = new Derivative();
        }
    }
}