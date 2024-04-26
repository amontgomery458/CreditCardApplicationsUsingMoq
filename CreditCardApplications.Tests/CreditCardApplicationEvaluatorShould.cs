namespace CreditCardApplications.Tests
{
	public class CreditCardApplicationEvaluatorShould
	{
		[Fact]
		public void AcceptHighIncomeApplications()
		{
			var sut = new CreditCardApplicationEvaluator();

			var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

			CreditCardApplicationDecision decision = sut.Evaluate(application);

			Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
		}
	}
}