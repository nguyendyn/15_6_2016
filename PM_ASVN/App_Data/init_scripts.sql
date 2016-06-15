CREATE TABLE [dbo].[Estimation] (
    [ID]          INT           NOT NULL,
    [Description] VARCHAR (MAX) NULL,
    [Cost]        FLOAT (53)    NULL,
    [DateCreate]  DATE          NULL, 
    CONSTRAINT [PK_Estimation] PRIMARY KEY ([ID])
);

GO
CREATE TABLE [dbo].[Item] (
    [ID]         INT           IDENTITY (1, 1) NOT NULL,
    [Type]       INT           NOT NULL,
    [Name]       VARCHAR (MAX) NULL,
	[Description] VARCHAR(MAX) NULL, 
    [CreateBy]   VARCHAR (MAX) NULL,
    [CreateDate] DATETIME          DEFAULT (getdate()) NULL,
    [UpdateBy] VARCHAR(MAX) NULL, 
    [UpdateDate] DATETIME NULL DEFAULT (getdate()), 
    CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED ([ID] ASC)
);



GO
CREATE TABLE [dbo].[ItemRelation] (
    [IDChild]     INT            NOT NULL,
    [IDParent]    INT            NOT NULL,
    [Description] NVARCHAR (MAX) NULL, 
    CONSTRAINT [PK_ItemRelation] PRIMARY KEY ([IDChild], [IDParent])
);

GO
CREATE TABLE [dbo].[Project] (
    [ID]                INT           NOT NULL,
    [StartDate]         DATE          NULL,
    [PrimaryBA] VARCHAR(MAX) NULL, 
    [PrimaryWeb] VARCHAR(MAX) NULL, 
    [PrimaryDB] VARCHAR(MAX) NULL, 
	[Comment]  VARCHAR (MAX) NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE TABLE [dbo].[StepInTestCase] (
    [ID]             INT           NOT NULL,
    [ExpectedResult] VARCHAR (MAX) NULL,
	[TestData]       VARCHAR (MAX) NULL,
    CONSTRAINT [PK_StepInTestCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);

GO
CREATE TABLE [dbo].[TestCase] (
    [ID]               INT           NOT NULL,
    [Priority]         VARCHAR (MAX) NULL,
    [Screen]           VARCHAR (MAX) NULL,
    [Summary]          VARCHAR (MAX) NULL,
    [PreCondition]     VARCHAR (MAX) NULL,
    [Assigner]         VARCHAR (MAX) NULL,
    [TestDate]         DATE          NULL,
    [Status]           VARCHAR (MAX) NULL,
    [TCExpectedResult] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_TestCase] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE TABLE [dbo].[TestCaseRelation] (
    [ID_TestCase] INT NOT NULL,
    [ID_User]     INT NOT NULL,
    [ID_Browser]  INT NOT NULL,
    CONSTRAINT [PK_TestCaseRelation] PRIMARY KEY CLUSTERED ([ID_TestCase] ASC, [ID_User] ASC, [ID_Browser] ASC)
);

GO
CREATE TABLE [dbo].[WorkGroup] (
    [ID]           INT        NOT NULL,
    [PercentGroup] FLOAT (53) NULL,
    CONSTRAINT [PK_WorkGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE TABLE [dbo].[WorkItem] (
    [ID]     INT NOT NULL,
    [Status] INT NULL,
    CONSTRAINT [PK_WorkItem] PRIMARY KEY CLUSTERED ([ID] ASC)
);

GO
CREATE TABLE [dbo].[Task]
(
	[ID] INT NOT NULL, 
    [StartDate] DATE NULL, 
    [DueDate] DATE NULL, 
    [EstimatedTime] INT NULL, 
    [%Done] INT NULL,
	[Assignee] VARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED ([ID] ASC)
)

GO
CREATE TABLE [dbo].[Role] (
    [ID]          INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [Description] VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

GO
CREATE TABLE [dbo].[Account] (
    [ID]       INT          IDENTITY (1, 1) NOT NULL,
    [Username] VARCHAR (50) NOT NULL,
    [Password] VARCHAR (50) NOT NULL,
    [IDRole]   INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([IDRole]) REFERENCES [dbo].[Role] ([ID])
);

GO
CREATE TABLE [dbo].[Permission] (
    [ID]          INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [Description] VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

GO
CREATE TABLE [dbo].[RolePermission] (
    [ID_Role]       INT NOT NULL,
    [ID_Permission] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ID_Role] ASC, [ID_Permission] ASC),
    FOREIGN KEY ([ID_Role]) REFERENCES [dbo].[Role] ([ID]),
    FOREIGN KEY ([ID_Permission]) REFERENCES [dbo].[Permission] ([ID])
);

GO
CREATE TABLE [dbo].[Ticket] (
    [ID]              INT           NOT NULL,
    [RequirementDate] VARCHAR (MAX) NULL,
    [ToQA]            DATE          NULL,
    [ToUS]            DATE          NULL,
    [Assigner]        VARCHAR (MAX) NULL,
    [Comment]         VARCHAR (MAX) NULL,
    CONSTRAINT [PK_Ticket] PRIMARY KEY CLUSTERED ([ID] ASC)
);



------------------------------------------------------------
----------------------STORED PROCEDURE----------------------
------------------------------------------------------------

-----Select Children not in @ParentID and in @Filter (In Use)
GO
CREATE PROCEDURE [dbo].[sp_ChildItem]
	@Type int,
	@ParentId int,
	@Filter int
AS
BEGIN
		select ID,Type,Name
	from Item with(nolock)
	where Item.ID in(
		select distinct i.ID
		from Item i with(nolock)
		where i.Type=@Type and i.ID not in (select IR2.IDChild
											from  ItemRelation IR2 with(nolock)
											where IR2.IDParent = @ParentId) 
											and i.ID in (select IR3.IDChild
											from  ItemRelation IR3 with(nolock)
											where IR3.IDParent = @Filter))
	
END


--exec [sp_ChildItem]
--@Type = 4,
--	@ParentId =1680,
--	@Filter =1648


------Select Children in @Type and not in @parentID (In Use but this case not happen)
GO
CREATE PROCEDURE [dbo].[sp_ChildItemNotInParentID]
	@Type int,
	@ParentId int
AS
BEGIN
	select ID,Type,Name
	from Item with(nolock)
	where Item.ID in(
		select distinct i.ID
		from Item i with(nolock)
		where i.Type=@Type and i.ID not in (select ir.IDChild
																from  ItemRelation ir with(nolock)
																where ir.IDParent = @ParentId)) 
END

--exec [sp_ChildItemNotInParentID]
--@Type = 4,
--	@ParentId =1680


------Get Description of Estimate (in use)
GO
CREATE PROCEDURE [dbo].[sp_GetDescriptionEstimation]
@IDEstimation int
As
select distinct ID,Description 
from Estimation with(nolock)
where ID = @IDEstimation
--exec [sp_GetDescriptionEstimation]
--@IDEstimation = 295


------Get Detail of WorkItem and sum WorkGroup by IDEstimate and IDWorkGroup (in use)
GO
--@IDParent1 phải là ID của Estimate, @IDParent2 là ID WorkGroup
CREATE PROCEDURE [dbo].[sp_GetDetailWorkItem]
@IDParent1 int ,
@IDParent2 int,
@ChildType int,
@SumWG float out
AS
Begin
declare @ListWI table ([ID] INT,[Name] VARCHAR (MAX) NULL,[Description] NVARCHAR (MAX) NULL)
insert into @ListWI 
	select I.ID,I.Name,IR.Description 
	from Item I with(nolock) left join ItemRelation IR with(nolock) on I.ID = IR.IDChild
	where IR.IDParent = @IDParent1 and I.Type = @ChildType and IR.IDChild in 
			 (select IR2.IDChild 
			 from WorkItem WI with(nolock) left join ItemRelation IR2 with(nolock) on I.ID = WI.ID
			 where IR2.IDParent = @IDParent2)
select ID, Name, Description AS Cost from @ListWI
set @SumWG = (select sum((cast(ListWI.Description as float)))
							from @ListWI as ListWI)
End

--declare @result nvarchar(max)
--exec [sp_GetDetailWorkItem] 
--@IDParent1 = 295,--Id Estimate
--@IDParent2 = 241, -- ID WG
--@ChildType = 15,
--@SumWG = @result out
--print @result


------Get list WorkGroup (in use)
GO
CREATE PROCEDURE [dbo].[sp_GetListWorkGroup]
 
AS
 select I.ID,I.Name, WG.PercentGroup
 from  WorkGroup WG with(nolock) left join Item I with(nolock)
 on I.ID=WG.ID 

 --exec [sp_GetListWorkGroup]

----Get List WordItem (in use)
GO
CREATE PROCEDURE [dbo].[sp_GetListWorkItem]
 
AS
  select I.ID,I.Name, WI.Status
 from Item I with(nolock) right join WorkItem WI  with(nolock) on I.ID=WI.ID
 where WI.Status = 0

 --exec [sp_GetListWorkItem]

----Get Time of WordGroup by @IDEstimate and @IDWorkGroup (in use)
 GO
 CREATE PROCEDURE [dbo].[sp_GetTimeWorkGroup]
@IDEstimate int ,
@IDWorkGroup int,
@ChildType int
AS
Begin
	select I.Name, Cast (IR.Description as float) as PercentGroup
	from Item I with(nolock) left join ItemRelation IR with(nolock) on I.ID = IR.IDChild
	where IR.IDParent = @IDEstimate and I.Type = @ChildType and IR.IDChild = @IDWorkGroup
END


--exec [sp_GetTimeWorkGroup]
--@IDEstimate = 286 ,
--@IDWorkGroup = 236,
--@ChildType =14


--Search item is child of multiparent, use function SplitParentID (not use)
GO
CREATE PROCEDURE [dbo].[sp_MultiParentID]
(
@string nVARCHAR(max),
@type int
)
as
begin
Select I.ID, I.Name, I2.Name as ParentName from Item I, ItemRelation IR, Item I2 
where I.id=IR.IDChild and I.Type=@type and IR.IDParent  in (select id from [dbo].[SplitParentID](@string,',')) and I2.ID=IR.IDParent
end

--exec [sp_MultiParentID]
--@string = '13,25,26',
--@type =4

----Search Item Child by @Type and @Parent ID using paging (in use)
GO
CREATE PROCEDURE [dbo].[sp_PagingItem]
@PageIndex int ,
@PageSize int,
@Type int,
@ParentId int = 0,
@TotalRow int out
AS
Begin
    Begin
    WITH s AS
    (  
 Select ROW_NUMBER() 
 OVER(ORDER BY Item.ID) AS RowNum,Item.ID,Item.Name,Item.Type From Item with(nolock) where Item.ID in 
  (SELECT distinct I.ID
  FROM Item I with(nolock)
  left join ItemRelation IR on I.ID = IR.IDChild
  WHERE  Item.Type = @Type   and (  IR.IDParent = @ParentId or  @ParentId = 0))
 )
    Select * From s 
    Where RowNum Between 
 (@PageIndex - 1)*@PageSize+1 
    AND @PageIndex*@PageSize
 -- Tính tổng số bản ghi
    
    Select @TotalRow=count (*) From Item where Item.ID in 
  (SELECT distinct I.ID
  FROM Item I with(nolock)
  left join ItemRelation IR on I.ID = IR.IDChild
  WHERE  Item.Type = @Type   and (  IR.IDParent = @ParentId or  @ParentId = 0))
 End
END

--declare @total int
--exec [sp_PagingItem]
--@PageIndex =1,
--@PageSize = 10,
--@Type=4,
--@ParentId = 27,
--@TotalRow = @total out
--print @total

----Delete Item in type Estimate and delete Estimate, delete relation between it and all item. (in use)
GO
CREATE PROCEDURE [dbo].[sp_RemoveFeatureFromTicket]
 @IDFeature int, 
 @IDTicket int
AS
	declare @ListEs table ([IDEs] int NULL)
    insert into @ListEs 
		select IDChild 
		from Item I left join ItemRelation IR on I.ID = IR.IDChild 
		where  IR.IDParent = @IDFeature and  I.Type = 12  and IDChild in
																(select IR.IDChild 
																from Item I2 left join ItemRelation IR2 on I2.ID = IR2.IDChild 
																where I2.Type = 12 and IR2.IDParent = @IDTicket)

delete from ItemRelation where IDChild  in (select IDEs from @ListEs) or IDParent in 
(select IDEs from @ListEs)
delete from Item where  ID  in (select IDEs from @ListEs)
delete from Estimation where ID  in (select IDEs from @ListEs)

--exec [sp_RemoveFeatureFromTicket]
-- @IDFeature =228,
-- @IDTicket = 235


----Search child item by @parentID (not use)
GO
CREATE PROCEDURE [dbo].[sp_SearchChildItemByParentID]
	@parentId int
AS
	select I.type, I.id 
	from Item I with(nolock) left join ItemRelation IR with(nolock) on I.ID = IR.IDChild
	where IR.IDParent = @parentId

	--exec [sp_SearchChildItemByParentID]
	--@parentId = 25

GO

----Search Estimate by @IDTicket (By @IDTicket, search list feature is child of it, then search Estimate, WorkGroup, WorkItem by list feature and @IDTicket) (in use)
CREATE PROCEDURE [dbo].[sp_SearchEstimateDetail]
	@IDTicket int
AS
declare @result TABLE ([ID] int NULL, [Name] NVarchar(50) NULL)
 insert into @result	select distinct  Fea.ID,Fea.Name
								from Item Fea with(nolock) left join ItemRelation IR3 with(nolock) on Fea.ID = IR3.IDChild 
								where IR3.IDParent = @IDTicket and Fea.Type = 2

 
 select IR.IDParent AS ID, ListFea.Name AS Feature, WG.Name AS WorkGroup,IR.Description AS Cost
 from  ItemRelation IR with(nolock) , @result ListFea,Item WG with(nolock)
 where WG.ID= IR.IDChild and WG.Type = 14 and IR.IDParent in(
							  select distinct  Es.ID
							 from Item Es with(nolock) left join  ItemRelation IR2 with(nolock) on Es.ID= IR2.IDChild
							 where  Es.Type = 12 and IR2.IDParent = ListFea.ID and Es.ID in (
														  select Tic.IDChild 
														  from ItemRelation Tic with(nolock)
														 where Tic.IDParent = @IDTicket ))
 --exec [sp_SearchEstimateDetail]
--@IDTicket = 235


----search item by @type and @parentId (not use)
 GO
CREATE PROCEDURE [dbo].[sp_SearchItem]
	@parentId int = 0,
	@type int
AS
BEGIN
	SELECT I.ID,I.Name,I.Type
	FROM Item I with (nolock) left join ItemRelation IR with (nolock) on I.ID = IR.IDChild
	WHERE I.Type = @type and IR.IDParent = @parentId 
END

	--exec [sp_SearchItem]
	--@parentId = 1646
	--@type = 2
	

----Search all parent by @ChildID using paging (in use)
GO
CREATE PROCEDURE [dbo].[sp_SearchParentItemByChildID]
 @PageIndex int ,
@PageSize int,
 @ChildID int,
 @TypeParent int,
 @TotalRow int out
AS
 Begin

    WITH s AS
    (  
	 Select ROW_NUMBER() 
	 OVER(ORDER BY Item.ID) AS RowNum,Item.ID,Item.Name,Item.Type From Item with (nolock) where Item.ID in 
		  (SELECT distinct I.ID
			from Item I with (nolock) left join ItemRelation IR with (nolock) on I.ID = IR.IDParent
			where IR.IDChild = @ChildID and I.Type = @TypeParent)
	)
    Select * From s 
    Where RowNum Between 
 (@PageIndex - 1)*@PageSize+1 
    AND @PageIndex*@PageSize
 -- Tính tổng số bản ghi
    
    Select @TotalRow=count (*) From Item with (nolock) where Item.ID in 
			(SELECT distinct I.ID
 			from Item I with (nolock) left join ItemRelation IR with (nolock) on I.ID = IR.IDParent
			where IR.IDChild = @ChildID and I.Type = @TypeParent)

END
 
--declare @total int
--exec [sp_SearchParentItemByChildID] 
--@PageIndex =1,
--@PageSize = 10,
--@ChildID = 1680,
--@TypeParent = 1,
--@TotalRow = @total out
--print @total


-----search item by @ChildType and 1 @IDParent (not use)
GO
CREATE PROCEDURE [dbo].[sp_SelectChildBy1Parent]
	@IDParent int,
	@ChildType int
AS
select I.ID,I.Name, IR.Description
from Item I with(nolock) left join ItemRelation IR with(nolock) on I.ID = IR.IDChild
where IR.IDParent = @IDParent and I.Type = @ChildType

--exec [sp_SelectChildBy1Parent]
--	@IDParent =228,
--	@ChildType =14


----search item by @ChildType and @ParentId (in use)
GO
CREATE PROCEDURE [dbo].[sp_SelectDatabaseInProject]
	@ParentId int,
	@ChildType int
AS
 select distinct  I.ID, I.Type, I.Name
  from Item I with(nolock) left join ItemRelation IR with(nolock) on I.ID = IR.IDChild
 where IR.IDParent = @ParentId and I.Type = @ChildType
 --exec [sp_SelectDatabaseInProject]
 --@ParentID = 1646,
 --@ChildType = 8


----search list WorkItem in WorkGroupID (in use)
GO
CREATE PROCEDURE [dbo].[sp_SelectWorkItemInWorkGroup]
	@IDParent int,
	@ChildType int
AS
 select I.ID,I.Name, IR.Description
from Item I with(nolock) left join ItemRelation IR with(nolock) on I.ID = IR.IDChild right join WorkItem WI with(nolock) on I.ID = WI.ID 
where  IR.IDParent = @IDParent and I.Type = @ChildType and WI.Status = 0

--exec [sp_SelectWorkItemInWorkGroup]
--@IDParent = 295,
--	@ChildType = 15


---------------- remove item (IN USE)---------------------------------------
GO
CREATE PROCEDURE [dbo].[sp_RemoveItem]
 @IDItem int
AS

delete from ItemRelation where IDChild = @IDItem or  IDParent = @IDItem 
delete from Item where  ID = @IDItem


--exec [sp_RemoveItem]
-- @IDItem = 468

---------------Search Project (In USE)
GO
CREATE PROCEDURE [dbo].[sp_SearchProject]
 @SearchString varchar(100),
 @Type int
AS
 select * from Item with(nolock) where cast (Item.ID as varchar)like '%' + @SearchString +'%' and Type = @Type

 -- exec [sp_SearchProject]
 --@SearchString = '6',
 --@Type =1

 -----------------search all step in 1 testcase (IN USE)
 GO
CREATE PROCEDURE [dbo].[sp_StepInTestCase]
	@IDTestCase int
AS
	select Step.ID, I.Name, Step.ExpectedResult, Step.TestData
	From ItemRelation IR with(nolock) right join Item I with(nolock) on I.ID = IR.IDChild 
	right join StepInTestCase Step with(nolock) on Step.ID = IR.IDChild and Step.ID = I.ID
	where  IR.IDParent = @IDTestCase and I.Type =25

	--exec [sp_StepInTestCase]
	--@IDTestCase = 1744

 ---------------------search User bu TC and Browser (IN USE)------------------------
 GO
 CREATE PROCEDURE [dbo].[sp_SearchUserByTC_Browser]
 @IDBrowser int,
 @IDTestCase int
AS
select TCRe.ID_User as ID,Usr.Name
 from Item TC with(nolock) left join TestCaseRelation TCRe with(nolock) on TC.ID = TCRe.ID_TestCase 
 left join Item Usr with(nolock) on Usr.ID = TCRe.ID_User 
 where  TCRe.ID_Browser = @IDBrowser and TC.ID = @IDTestCase

--exec [sp_SearchUserByTC_Browser]
-- @IDBrowser = 1750,
-- @IDTestCase = 1743

 ---------------------search TC by Browser and User (IN USE)-----------------
 GO
 CREATE PROCEDURE [dbo].[sp_SearchTCByUser_Browser]
 @IDBrowser int,
 @IDUser int
AS
select TC.ID,TC.Name
 from Item TC with(nolock) left join TestCaseRelation TCRe with(nolock) on TC.ID = TCRe.ID_TestCase
 where TCRe.ID_Browser = @IDBrowser and TCRe.ID_User = @IDUser

--exec [sp_SearchTCByUser_Browser]
-- @IDBrowser = 1750,
-- @IDUser = 1748

-------------------------search TC by Feature and User (in use)-------------------------
GO
CREATE PROCEDURE [dbo].[sp_SearchTCByFeature]
 @IDFeature int,
 @IDType int,
 @IDUser int
AS
 select Distinct TC.ID, TC.Name
 from Item TC with(nolock) left join ItemRelation IR with(nolock) on TC.ID = IR.IDParent 
 left join TestCaseRelation TCRe with(nolock) on TC.ID = TCRe.ID_TestCase 
 left join Item TCType with(nolock) on TCType.ID = IR.IDChild
 where  TCRe.ID_User = @IDUser
 and  TCType.ID = @IDType and TCType.Type = 23  and TC.Type = 19 
 and TC.ID in (
       select IR2.IDChild 
       from ItemRelation IR2 with(nolock)
       where IR2.IDParent = @IDFeature)

--exec [sp_SearchTCByFeature]
-- @IDFeature = 228,
-- @IDType = 1756,
-- @IDUser = 1749


------------------------Count TC in group (NOT USE)---------------------
GO
CREATE PROCEDURE [dbo].[sp_CountTestCaseInGroup]
	@IDFeature int
AS
	select count(TC.ID) as TotalTC, TCGroup.ID, TCGroup.Name
	from Item TC with(nolock) left join ItemRelation IR with(nolock) on TC.ID = IR.IDChild  
	left join Item TCGroup with(nolock) on TCGroup.ID = IR.IDParent 
	where TCGroup.Type = 24 and TC.Type = 19 
	and TC.ID in (
							select IR2.IDChild 
							from ItemRelation IR2  with(nolock)
							where IR2.IDParent = @IDFeature)
	group by TCGroup.ID, TCGroup.Name

	--exec [sp_CountTestCaseInGroup]
	--@IDFeature = 228

-----------------------------Count TC in type TC (in use)---------------
GO
CREATE PROCEDURE [dbo].[sp_CountTestCaseInType]
@IDFeature int,
 @IDType int
AS
 select TCType.ID, TCType.Name, count(TC.ID) as TotalTC
 from Item TC with(nolock) left join ItemRelation IR with(nolock) on TC.ID = IR.IDParent 
 left join Item TCType with(nolock) on TCType.ID = IR.IDChild
 where TCType.Type = 23 and TCType.ID = @IDType and TC.Type = 19 
 and TC.ID in (
       select IR2.IDChild 
       from ItemRelation IR2  with(nolock)
       where IR2.IDParent = @IDFeature)
 group by TCType.ID, TCType.Name

 --exec [sp_CountTestCaseInType]
 --@IDFeature = 228,
 --@IDType = 1757

 ---------------Search TestCaseRelation by 3 ID (User, Browser, TC) (In USE)
 GO
 CREATE PROCEDURE [dbo].[sp_CheckUserInBrowser]
	@IDUser int,
	@IDBrowser int,
	@IDTestCase int
AS
	SELECT TCRe.ID_User, TCRe.ID_Browser, TCRe.ID_TestCase
	FROM TestCaseRelation TCRe with(nolock)
	WHERE TCRe.ID_TestCase = @IDTestCase and TCRe.ID_Browser = @IDBrowser 
	and TCRe.ID_User = @IDUser
	--exec [sp_CheckUserInBrowser]
--	@IDUser =1748,
--	@IDBrowser =1750,
--	@IDTestCase =1626

---------Get all Item in type (In USE)
GO
CREATE PROCEDURE [dbo].[sp_GetItemByType]
	@type int
AS
	SELECT Item.ID, Item.Name, Item.Type
	FROM Item with(nolock)
	WHERE Item.Type = @type

	--exec [sp_GetItemByType]
	--@type = 2

-------Search TestCase in this Feature and Type TestCase (In USE)
GO
CREATE PROCEDURE [dbo].[sp_GetTestCaseByFeatureAndTypeTest]
 @IDFeature int,
 @IDType int
AS
 select TC.ID, TC.Name
 from Item TC with(nolock) left join ItemRelation IR with(nolock) on TC.ID = IR.IDParent 
 left join Item TCType with(nolock) on TCType.ID = IR.IDChild
 where TCType.Type = 23 
 and TCType.ID = @IDType and TC.Type = 19 
 and TC.ID in (
       select IR2.IDChild 
       from ItemRelation IR2  with(nolock)
       where IR2.IDParent = @IDFeature)


--exec [sp_GetTestCaseByFeatureAndTypeTest]
-- @IDFeature = 228,
-- @IDType = 1756


------Search User not in TestCase (IN USE)
GO
CREATE PROCEDURE [dbo].[sp_GetUserNotInTestCase]
	@IDTestCase int,
	@Type int
AS
	
	SELECT I.ID, I.Name
	FROM Item I with(nolock)
	WHERE I.Type = @Type and I.ID not in (
		SELECT distinct TCRe.ID_User
		FROM TestCaseRelation TCRe with(nolock)
		WHERE TCRe.ID_TestCase = @IDTestCase and I.ID = TCRe.ID_User)

		--exec [sp_GetUserNotInTestCase]
		--@IDTestCase = 1626,
		--@Type = 21

---Search User in TestCase (IN USE)
GO
CREATE PROCEDURE [dbo].[sp_GetUserInTestCase]
	@IDTestCase int,
	@Type int
AS
	SELECT distinct Item.ID , Item.Name
	FROM TestCaseRelation with(nolock) right join Item with(nolock) on Item.ID = TestCaseRelation.ID_User
	WHERE TestCaseRelation.ID_TestCase = @IDTestCase
	
	--exec [sp_GetUserInTestCase]
	--@IDTestCase =1626,
	--@Type =21

-----Check Account Login By Username and Password (in use) -------
GO
CREATE PROCEDURE [dbo].[sp_AccountLogin]
	@name varchar(50),
	@pass varchar(50),
	@result int out
AS
	SET @result=(SELECT COUNT(Username) 
				FROM Account 
				WHERE Username=@name and Password=@pass)

	--exec [sp_AccountLogin]
	--@name='admin',
	--@pass='123'

-----Add New Account (in use) ------
Go
CREATE PROCEDURE [dbo].[sp_AddAccount]
	@name varchar(50),
	@pass varchar(50),
	@idRole int,
	@result int out
AS
	BEGIN
			INSERT INTO Account(Username,Password,IDRole)
			VALUES (@name,@pass,@idRole)
			SET @result=@@IDENTITY
	END

	--exec [sp_AddAccount]
	--@name='account1',
	--@pass='123',
	--@idRole=2

-----Edit Account (in use) ------
GO
CREATE PROCEDURE [dbo].[sp_EditAccount]
	@idAccount int,
	@name varchar(50),
	@idRole int
AS
BEGIN
		UPDATE Account
		SET Username=@name,IDRole=@idRole
		WHERE ID=@idAccount
END

	--exec [sp_EditAccount]
	--@idAccount=2,
	--@name='account1',
	--@idRole=3

-----Delete Account (in use) ------
GO
CREATE PROCEDURE [dbo].[sp_DeleteAccount]
	@idAccount int
AS
	BEGIN
		DELETE FROM Account
		WHERE ID=@idAccount
	END

	--exec [sp_DeleteAccount]
	--@idAccount=2

----Get List Account using paging (in use)-----
Go
CREATE PROCEDURE [dbo].[sp_GetAccount]
   @pageNum INT,  
   @pageSize INT,
   @totalRecords INT OUTPUT
AS
BEGIN
	
    SELECT 
	*
	FROM 
    (
	SELECT A.ID,A.Username,A.IDRole,R.Name,ROW_NUMBER() OVER (ORDER BY A.ID) AS RowNumber
	FROM Account A,Role R
	WHERE A.IDRole=R.ID
	)
	 Paging
       WHERE RowNumber BETWEEN (@pageNum - 1) * @pageSize + 1 
              AND @pageNum * @pageSize  
	SET @totalRecords=(SELECT COUNT(*) FROM Account)
			
END

	--exec [sp_GetAccount]
	--@pageNum=1,
	--@pageSize=10,
	--@totalRecords output

----- Get Detail Account By @username (in use)-----
Go
CREATE PROCEDURE [dbo].[sp_GetAccountByName]
	@name varchar(50)
AS
BEGIN
	SELECT A.ID,A.Username,A.IDRole,R.Name 
	FROM Account A,Role R 
	WHERE A.Username=@name and A.IDRole=R.ID
END
	--exec [sp_GetAccountByName]
	--@name='admin'

----- Get Role in Account (in use)------
Go
CREATE PROCEDURE [dbo].[sp_GetRoleInAccount]
	@Id INT ,
	@result INT OUT
AS

	SET @result=( 
				SELECT r.ID 
				FROM Account A,Role R
				WHERE A.IDRole=R.ID and A.ID=@Id)

	--exec [sp_GetAccountByName]
	--@Id=1,
	--@result output

-----Get Detail Account By @ID (in use)------
Go
CREATE PROCEDURE [dbo].[sp_GetAccountID]
@ID INT	
AS
	SELECT A.ID,A.Username,R.Name 
	FROM Account A,Role R
	WHERE A.IDRole=R.ID and A.ID=@ID

	--exec [sp_GetAccountID]
	--@ID=1

------ Check Account Exists By Username (in use)-------
Go
CREATE PROCEDURE [dbo].[sp_CheckAccount]
	@name varchar(50),
	@result int out
AS
	SET @result=(
				SELECT COUNT(Username) as Result 
				FROM Account
				WHERE Username=@name)

	--exec [sp_GetAccountByName]
	--@name='admin'
	--@result output

----- Get List Role (in use)--------
Go
CREATE PROCEDURE [dbo].[sp_GetListRole]
AS
	SELECT * FROM Role

	--exec [sp_GetListRole]

----- Check Role Exists By RoleName (in use)------
Go
CREATE PROCEDURE [dbo].[sp_CheckRole]
	@name varchar(50),
	@result int out
AS
	SET @result=(
				SELECT COUNT(Name) as Result 
				FROM Role
				WHERE Name=@name)

	--exec [sp_CheckRole]
	--@name='Admin'
	--@result output

------Add New Role (in use) -----------
Go
CREATE PROCEDURE [dbo].[sp_AddRoles]
	@role varchar(50),
	@description varchar(50),
	@result int out
AS
	BEGIN
			INSERT INTO Role(Name,Description)
			VALUES (@role,@description)
			SET @result=@@IDENTITY
	END

	--exec [sp_AddRoles]
	--@role='BA',
	--@description='Role BA',
	--@result output

------ Get Detail By RoleID (in use)--------
Go
CREATE PROCEDURE [dbo].[sp_GetRoleID]
	@ID INT	
AS
	SELECT
	 *
	FROM Role R
	WHERE R.ID=@ID

	--exec [sp_GetRoleID]
	--@ID=1

------ Edit Role By RoleID (in use)-------------
Go
CREATE PROCEDURE [dbo].[sp_EditRole]
	@ID int,
	@name varchar(50),
	@description varchar(50)
AS	
BEGIN
	UPDATE Role
	SET Name=@name, Description=@description
	where ID=@ID
END

	--exec [sp_EditRole]
	--@name='BA',
	--@description='Role Business Analyst'

---------Check Role Exists in Account (in use)---------
Go
CREATE PROCEDURE [dbo].[sp_ExistRoleInAccount]
	@ID int,
	@result int out
AS
	IF EXISTS(
				SELECT 
					* 
				FROM Account 
				WHERE IDRole=@ID)
	SET @result=0
	ELSE 
	SET @result=1

	--exec [sp_ExistRoleInAccount]
	--@ID=1,
	--@result output
	--if exists return 1 else return 0

-------Delete Role (in use)-----------------
Go
CREATE PROCEDURE [dbo].[sp_DeleteRole]
	@ID int
AS
	IF EXISTS( SELECT * FROM RolePermission WHERE ID_Role=@ID)
	BEGIN 
		DELETE FROM RolePermission
		WHERE ID_Role=@ID
		DELETE FROM Role
		WHERE ID=@ID
	END
	ELSE
		DELETE FROM Role
		WHERE ID=@ID
	--exec [sp_DeleteRole]
	--@ID=1

-------- Get List Permission of Role in RolePermission (in use)---------------
Go
CREATE PROCEDURE [dbo].[sp_GetPermissionInRole]
	@ID int
AS
	SELECT P.ID,P.Name,P.Description
	FROM Role R, RolePermission RP, Permission P
	WHERE R.ID=RP.ID_Role and RP.ID_Permission=P.ID and RP.ID_Role=@ID

	--exec [sp_GetPermissionInRole]
	--@ID=1

-------- Get List Permission of Role not in RolePermission (in use)---------------
Go
CREATE PROCEDURE [dbo].[sp_GetPermissionNotInRole]
	@IDRole int
AS
	SELECT * FROM Permission

	EXCEPT

	SELECT P.ID,P.Name,P.Description
	FROM Role R, RolePermission RP, Permission P
	WHERE R.ID=RP.ID_Role and RP.ID_Permission=P.ID and RP.ID_Role=@IDRole

	--exec [sp_GetPermissionNotInRole]
	--@IDRole=1

-------- Add Permission into Role (in use)--------------
Go
CREATE PROCEDURE [dbo].[sp_AddPermissionInRole]
	 @IDRole int,
	 @IDPermission int
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM RolePermission WHERE ID_Role=@IDRole and ID_Permission=@IDPermission)
	BEGIN
		INSERT INTO RolePermission(ID_Role,ID_Permission)
		VALUES(@IDRole,@IDPermission)
	END
END

	--exec [sp_AddPermissionInRole]
	--@IDRole=1
	--@IDPermission=1

--------- Delete Permission In Role (in use)----------
Go
CREATE PROCEDURE [dbo].[sp_DeletePermissionInRole]
	@IDRole int,
	@IDPermission int
AS
	DELETE FROM RolePermission
	WHERE ID_Role=@IDRole and ID_Permission=@IDPermission

	--exec [sp_DeletePermissionInRole]
	--@IDRole=1
	--@IDPermission=1

-------- Get List Permission (in use)--------------
Go
CREATE PROCEDURE [dbo].[sp_GetPermission]
   @pageNum INT,  
   @pageSize INT,
   @totalRecords INT OUTPUT
AS
BEGIN
	
    SELECT 
	*
	FROM 
    (
	SELECT ID,Name,Description,ROW_NUMBER() OVER (ORDER BY ID) AS RowNumber
	FROM Permission 
	)
	 Paging
       WHERE RowNumber BETWEEN (@pageNum - 1) * @pageSize + 1 
              AND @pageNum * @pageSize  
	SET @totalRecords=(SELECT COUNT(*) FROM Permission)
			
END

	--exec [sp_GetPermission]
	--@pageNum=1,
	--@pageSize=10,
	--@totalRecords output

--------Check Permission Exists By PermissionName (in use)-------------
Go
CREATE PROCEDURE [dbo].[sp_CheckPermission]
	@name varchar(50),
	@result int out
AS
	SET @result=(
				SELECT COUNT(Name) as Result 
				FROM Permission
				WHERE Name=@name)

	--exec [sp_CheckPermission]
	--@name='Add_Account',
	-- if exists return 1 else return 0

-----Add New Permission (in use)-------------
Go
CREATE PROCEDURE [dbo].[sp_AddPermission]
	@permission varchar(50),
	@description varchar(50),
	@result int out
AS
	BEGIN
			INSERT INTO Permission(Name,Description)
			VALUES (@permission,@description)
			SET @result=@@IDENTITY
	END

	--exec [sp_CheckPermission]
	--@name='Add_Account',
	-- if exists return 1 else return 0

-------Get Permission By PermissionID (in use)-------
Go
CREATE PROCEDURE [dbo].[sp_GetPermissionID]
@ID INT	
AS
	SELECT
	 *
	FROM Permission R
	WHERE R.ID=@ID

	--exec [sp_GetPermissionID]
	--@ID=1

-------Edit Permission (in use)----------
Go
CREATE PROCEDURE [dbo].[sp_EditPermission]
	@ID int,
	@name varchar(50),
	@description varchar(50)
AS	
BEGIN
	IF EXISTS (SELECT * FROM Permission WHERE ID=@ID)
	BEGIN
	UPDATE Permission
	SET Name=@name, Description=@description
	where ID=@ID
	END
END

	--exec [sp_EditPermission]
	--@ID=1
	--@name='View_System1',
	--@description='Allow View System Page'

----- Delete Permission (in use)----------
Go
CREATE PROCEDURE [dbo].[sp_DeletePermission]
	@ID int
AS
	IF EXISTS( SELECT * FROM RolePermission WHERE ID_Permission=@ID)
		BEGIN 
			DELETE FROM RolePermission
			WHERE ID_Permission=@ID

			DELETE FROM Permission
			WHERE ID=@ID
		END
	ELSE
	    	DELETE FROM Permission
			WHERE ID=@ID

	--exec [sp_DeletePermission]
	--@ID=1
--create date: 14/06/2016
GO
CREATE PROCEDURE [dbo].[sp_SearchCmtByIDParent]
 @IDParent int
AS
 SELECT Cmt.ID, Cmt.Text, Cmt.IDUser, Usr.Name, Cmt.CreateDate
 FROM Comment Cmt with(nolock) left join Item Usr on Cmt.IDUser = Usr.ID
 WHERE Cmt.IDItemParent = @IDParent

--exec [sp_SearchCmtByIDParent]
--@IDParent = 1646

------------------------------------------------------
---------------------FUNCTION-------------------------
------------------------------------------------------
GO
CREATE FUNCTION [dbo].[SplitParentID]
	( @str nVARCHAR (MAX),
	@delimeter nvarchar(2))
RETURNS
	@result TABLE (
	[ID] int NULL)
AS
BEGIN

		DECLARE @x XML
		SET @x = '<t>' + REPLACE(@str, @delimeter, '</t><t>') + '</t>'

		INSERT INTO @result
		SELECT DISTINCT x.i.value('.', 'int') AS valueIDParent
		FROM @x.nodes('//t') x(i)

	RETURN
END
