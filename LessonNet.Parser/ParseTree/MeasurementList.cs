using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class MeasurementList : Expression {
		private readonly List<Measurement> measurements;

		public MeasurementList(IEnumerable<Measurement> measurements) {
			this.measurements = measurements.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new MeasurementList(this.measurements.Select(m => m.EvaluateSingle<Measurement>(context)));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(measurements, " ");
		}

		protected bool Equals(MeasurementList other) {
			return measurements.SequenceEqual(other.measurements);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MeasurementList) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return measurements.Aggregate(397, (h, m) => (h * 397) ^ m.GetHashCode());
			}
		}
	}
}