using Moq;
namespace CreditCardApplications.Tests
{
	public class CreditCardApplicationEvaluatorShould
	{
		[Fact]
		public void AcceptHighIncomeApplications()
		{
			//Create a mock frequent flyer
			Mock<IFrequentFlyerNumberValidator> mockValidator = 
				new Mock<IFrequentFlyerNumberValidator>();

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

			CreditCardApplicationDecision decision = sut.Evaluate(application);

			Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
		}

		[Fact]
		public void ReferYoungApplications()
		{
			//Create a mock frequent flyer
			Mock<IFrequentFlyerNumberValidator> mockValidator =
				new Mock<IFrequentFlyerNumberValidator>();

			mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication { Age = 19 };

			CreditCardApplicationDecision decision = sut.Evaluate(application);

			Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
		}

		[Fact]
		public void DeclineLowIncomeApplications()
		{

			//Create a mock frequent flyer
			Mock<IFrequentFlyerNumberValidator> mockValidator =
				new Mock<IFrequentFlyerNumberValidator>();

			//We need to set the mock object
			//mockValidator.Setup(x => x.IsValid("x")).Returns(true);

			//Here we want it to be true no matter what string is passed to the isValid method
			mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication
			{
				GrossAnnualIncome = 19_999,
				Age = 42,
				FrequentFlyerNumber = "x"
			};

			CreditCardApplicationDecision decision = sut.Evaluate(application);

			Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
		}

		[Fact]
		public void ReferInvalidFrequentFlyerApplications()
		{
			Mock<IFrequentFlyerNumberValidator> mockValidator =
				new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);

			mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
			
			var application = new CreditCardApplication();

			CreditCardApplicationDecision decision = sut.Evaluate(application);

			Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
		}

		[Fact]
		public void DeclineLowIncomeApplicationsOutDemo()
		{
			Mock<IFrequentFlyerNumberValidator> mockValidator =
				new Mock<IFrequentFlyerNumberValidator>();

			bool isValid = true;
			mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication
			{
				GrossAnnualIncome = 19_999,
				Age = 42
			};

			CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

			Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
		}
	}
}