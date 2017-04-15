using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class MeasurementList : Expression {
		private List<Measurement> measurements;

		public MeasurementList(IEnumerable<Measurement> measurements) {
			this.measurements = measurements.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}
	}
}