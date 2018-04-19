﻿using Elastic.Installer.Domain.Properties;
using Xunit;

namespace Elastic.Installer.Domain.Tests.Elasticsearch.Models.Notice
{
	public class NoticeModelTests : InstallationModelTestBase
	{
		[Fact]
		public void ErrorStates() => Pristine()
			.IsValidOnFirstStep()
			.HasSetupValidationFailures(errors => errors
				.ShouldHaveErrors(
					TextResources.NoticeModelValidator_JavaInstalled
				)
			)
			.HasPrerequisiteErrors(errors => errors
				.ShouldHaveErrors(TextResources.NoticeModelValidator_JavaInstalled)
			);

		[Fact]
		public void CanClickNextWhenValid() => WithValidPreflightChecks()
			.IsValidOnStep(m => m.LocationsModel)
			.CanClickNext();

		[Fact] public void MajorDowngradeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.0.0", upgradeFrom: "6.0.0"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void MajorUpgradeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("6.0.0", upgradeFrom: "5.0.0"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void MinorDowngradeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.0.0", upgradeFrom: "5.1.0"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void MinorUpgradeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.1.0", upgradeFrom: "5.0.0"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void PatchDowngradeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.1.0", upgradeFrom: "5.1.1"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void PatchUpgradeDoesNotShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.1.1", upgradeFrom: "5.1.0"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void PatchUpgradeNextScreenIsConfigurationScreen() => WithValidPreflightChecks(f=>f
				.Wix("5.1.1", upgradeFrom: "5.1.0")
				.ServicePreviouslyInstalled()
			)
			.IsValidOnStep(m => m.NoticeModel)
			.ClickNext()
			.IsValidOnStep(m => m.ConfigurationModel)
			.CanClickNext();

		[Fact] public void PrereleaseShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.0.0-alpha1", upgradeFrom: "5.0.0-alpha2"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void PrereleaseDowngradeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.0.0", upgradeFrom: "5.0.0-alpha2"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void PrereleaseUpgradeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.0.0-rc2", upgradeFrom: "5.0.0"))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();

		[Fact] public void InstallingPrereleaseFirstTimeShowsNotice() => WithValidPreflightChecks(f=>f.Wix("5.0.0-beta2", upgradeFrom: null))
			.IsValidOnStep(m => m.NoticeModel)
			.CanClickNext();
	}
}
