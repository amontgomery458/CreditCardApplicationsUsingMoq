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
			var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
			mockValidator.DefaultValue = DefaultValue.Mock;

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
			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

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
				new Mock<IFrequentFlyerNumberValidator>();

			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

			mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
			
			var application = new CreditCardApplication();

			CreditCardApplicationDecision decision = sut.Evaluate(application);

			Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
		}

		//[Fact]
		//public void DeclineLowIncomeApplicationsOutDemo()
		//{
		//	Mock<IFrequentFlyerNumberValidator> mockValidator =
		//		new Mock<IFrequentFlyerNumberValidator>();

		//	bool isValid = true;
		//	mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

		//	var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

		//	var application = new CreditCardApplication
		//	{
		//		GrossAnnualIncome = 19_999,
		//		Age = 42
		//	};

		//	CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

		//	Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
		//}

		[Fact]
		public void ReferWhenLicenseKeyExpired()
		{
			//Dealing with hierarchies manually
			//var mockLicenseData = new Mock<ILicenseData>();
			//mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED");
			//var mockServiceInfo = new Mock<IServiceInformation>();
			//mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);
			//var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
			//mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

			//Dealing with hierarchies automatically with Moq
			var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");

			mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication { Age = 42 };

			CreditCardApplicationDecision decision = sut.Evaluate(application);

			Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
		}

		string GetLicenseKeyExpiryString()
		{
			//E.g. read from vendor-supplied constants file
			return "EXPIRED";
		}

		[Fact]
		public void UseDetailedLookupForOlderApplications()
		{
			var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

			//mockValidator.SetupProperty(x => x.ValidationMode);
			//If there are numerous methods
			mockValidator.SetupAllProperties();

			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication { Age = 30 };

			sut.Evaluate(application);

			Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
		}

		[Fact]
		public void ValidateFrequentFlyerNumberForLowIncomeApplications()
		{
			var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication();

			sut.Evaluate(application);

			//Did the CreditCardApplicationEvaluator call the IsValid method
			mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), "Frequent Flyer numbers should be validated");
		}

		[Fact]
		public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
		{
			var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

			sut.Evaluate(application);

			//Verifying a method was never called
			mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void CheckLicenseKeyForLowIncomeApplications()
		{
			var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

			sut.Evaluate(application);

			//Verifying a Property Getter was Called
			mockValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
		}

		[Fact]
		public void SetDetailedLookupForOlderApplications()
		{
			var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

			mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

			var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

			var application = new CreditCardApplication { Age = 30 };

			sut.Evaluate(application);

			//Verifying a property setter was called
			mockValidator.VerifySet(x => x.ValidationMode = ValidationMode.Detailed);

			//Verify that the isValid method was called
			//mockValidator.Verify(x => x.IsValid(null), Times.Once);

			//There should be no other calls on the Moq object
			//mockValidator.VerifyNoOtherCalls();

			//However we know that other methods and calls were made
		}
	}
}