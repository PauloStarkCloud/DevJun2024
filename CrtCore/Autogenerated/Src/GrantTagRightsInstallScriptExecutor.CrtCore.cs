namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;

	#region Class: GrantTagRightsInstallScriptExecutor

	internal class GrantTagRightsInstallScriptExecutor : IInstallScriptExecutor
	{

		#region Constants: Private

		private readonly Guid AllEmployee = new Guid("A29A3BA5-4B0D-DE11-9A51-005056C00008");
		private readonly Guid AllExternalUser = new Guid("720B771C-E7A7-4F31-9CFB-52CD21C3739F");
		private readonly Guid TagAccessSchemaUId = new Guid("1FC1E003-8083-44DA-BA4B-7B77186968E0");
		private readonly Guid TagAccess_MyTagId = new Guid("54C3C9A0-ADBA-423F-990B-673B903E2A38");
		private readonly Guid TagAccess_CorporateTagId = new Guid("5126592F-5E93-42B4-B3A1-10752E6D413E");
		private readonly Guid TagAccess_PublicTagId = new Guid("7BEAB7B3-E895-45FE-96C6-0C662C6DCB16");

		#endregion

		#region Properties: Private

		UserConnection _userConnection
		{
			get; set;
		}

		#endregion

		#region Methods: Private

		private void GrantRights() {
			using (DBExecutor dbExecutor = _userConnection.EnsureDBConnection()) {
				dbExecutor.StartTransaction();
				if (IsNeedInsertSysEntitySchemaRecordDefRightRecord()) {
					foreach (var insertDefRights in GetTagAccessSchemaDefRightInsertCollection()) {
						insertDefRights.Execute(dbExecutor);
					}
				}
				if (IsNeedInsertSysTagAccessRight()) {
					foreach (var insertTagAccessRight in GetTagAccessRightInsertCollection()) {
						insertTagAccessRight.Execute(dbExecutor);
					}
				}
				dbExecutor.CommitTransaction();
			}
		}

		public bool IsNeedInsertSysEntitySchemaRecordDefRightRecord() {
			var select = new Select(_userConnection).Top(1)
					.Column("Id")
				.From("SysEntitySchemaRecordDefRight")
				.Where()
				.OpenBlock("SubjectSchemaUId")
					.IsEqual(Column.Parameter(TagAccessSchemaUId))
					.And("AuthorSysAdminUnitId")
					.IsEqual(Column.Parameter(AllEmployee))
					.And("GranteeSysAdminUnitId")
					.IsEqual(Column.Parameter(AllEmployee))
				.CloseBlock()
					.Or()
				.OpenBlock("SubjectSchemaUId")
					.IsEqual(Column.Parameter(TagAccessSchemaUId))
					.And("AuthorSysAdminUnitId")
					.IsEqual(Column.Parameter(AllExternalUser))
					.And("GranteeSysAdminUnitId")
					.IsEqual(Column.Parameter(AllExternalUser))
				.CloseBlock() as Select;
			var recordId = select.ExecuteScalar<Guid>();
			return recordId.Equals(Guid.Empty);
		}

		private List<Insert> GetTagAccessSchemaDefRightInsertCollection() {
			return new List<Insert> {
				GetTagAccessSchemaDefRightInsert(AllExternalUser, AllExternalUser, 2, 1, 0),
				GetTagAccessSchemaDefRightInsert(AllExternalUser, AllExternalUser, 1, 1, 1),
				GetTagAccessSchemaDefRightInsert(AllExternalUser, AllExternalUser, 0, 1, 2),
				GetTagAccessSchemaDefRightInsert(AllEmployee, AllEmployee, 2, 1, 0),
				GetTagAccessSchemaDefRightInsert(AllEmployee, AllEmployee, 1, 1, 1),
				GetTagAccessSchemaDefRightInsert(AllEmployee, AllEmployee, 0, 1, 2),
			};
		}

		private Insert GetTagAccessSchemaDefRightInsert(Guid authorSysAdminUnitId, Guid granteeSysAdminUnitId,
			int operation, int rightLevel, int position) {
			return new Insert(_userConnection)
					.Into("SysEntitySchemaRecordDefRight")
					.Set("AuthorSysAdminUnitId", Column.Parameter(authorSysAdminUnitId))
					.Set("GranteeSysAdminUnitId", Column.Parameter(granteeSysAdminUnitId))
					.Set("Operation", Column.Const(operation))
					.Set("RightLevel", Column.Const(rightLevel))
					.Set("Position", Column.Const(position))
					.Set("SubjectSchemaUId", Column.Parameter(TagAccessSchemaUId));
		}

		private bool IsNeedInsertSysTagAccessRight() {
			var sysAdminUnits = new List<Guid> { AllEmployee, AllExternalUser };
			var recordIds = new List<Guid> { TagAccess_MyTagId, TagAccess_CorporateTagId, TagAccess_PublicTagId };
			var select = new Select(_userConnection).Top(1)
					.Column("Id")
				.From("SysTagAccessRight")
				.Where("SysAdminUnitId").In(Column.Parameters(sysAdminUnits))
					.And("RecordId").In(Column.Parameters(recordIds)) as Select;
			var recordId = select.ExecuteScalar<Guid>();
			return recordId.Equals(Guid.Empty);
		}

		private List<Insert> GetTagAccessRightInsertCollection() {
			return new List<Insert> {
				GetTagAccessRightInsert(AllExternalUser, TagAccess_MyTagId, 0, 1),
				GetTagAccessRightInsert(AllEmployee, TagAccess_MyTagId, 0, 1),
				GetTagAccessRightInsert(AllEmployee, TagAccess_CorporateTagId, 0, 1),
				GetTagAccessRightInsert(AllEmployee, TagAccess_PublicTagId, 0, 1)
			};
		}

		private Insert GetTagAccessRightInsert(Guid sysAdminUnitId, Guid recordId, int operation, int rightLevel) {
			return new Insert(_userConnection)
					.Into("SysTagAccessRight")
					.Set("SysAdminUnitId", Column.Parameter(sysAdminUnitId))
					.Set("RecordId", Column.Parameter(recordId))
					.Set("Operation", Column.Parameter(operation))
					.Set("RightLevel", Column.Parameter(rightLevel));
		}

		#endregion

		#region Methods: Public

		public void Execute(UserConnection userConnection) {
			_userConnection = userConnection;
			GrantRights();
		}

		#endregion

	}

	#endregion

}

