

--update  plm set WorkingCopy=1

--insert into durados_Html( [Name],[Text])
--  values ('PLM BE Status Change Message','<div>PLM BE Status Have Been Change on PLM Id:[PLM Master View.Id] To:[PLM Master View.PLM BE Status.Name]  <a href="[AppPath]/AllegroPLM/IndexPage/v_PLM_SD_uSD?guid=v_PLM_SD_uSD_a93_&mainPage=True&pk=[PLM Master View.Id]">[PLM Master View.Project ID]</a></div><br/>')
--insert into [DistributionList](Name) values ('Notification for SSD _ CF PLM BE Status Changed'),('Notification for iNAND  PLM BE Status Changed'),('Notification for SD uSD USB PLM BE Status Changed'),('Notification for PPN  PLM BE Status Changed')

-------------------------------------------------------------------------------
INSERT INTO [UserRole] ([Name] ,[Description]) VALUES('CTO','CTO role')
INSERT INTO [PLMStatus]([Name],[Ordinal]) VALUES('On Hold',	36),('Cancelled',46)
--INSERT INTO [SanDisk_Allegro_Dev].[dbo].[ApprovalProcessTypeProductLine]
--           ([ProductLineId]
--           ,[ApprovalProcessTypeId])
--     VALUES  (4,1),(20,1),(2,1),(19,1),(5,1),(4,2),(20,2),(2,2),(19,2),(5,2),(4,3),(20,3),(2,3),(19,3),(5,3)
--GO
----------------------------------------------------------------------------------------------------
INSERT INTO [PORPhase]
           ([Id]
           ,[Name]
           ,[Ordinal])
     VALUES
           (1,'Phase 0 [Concept]',10)
           ,(2,'Phase 0 Request Cancellation',20)
           ,(3,'Phase 0 [Cancelled]',30)
           ,(4,'Phase 0 Exit',40)
           ,(5,'Phase 1 [Planning]',50)
           ,(6,'Phase 1 Request Cancellation',60)
           ,(7,'Phase 1 [Cancelled]',70)
           ,(8,'Phase 1 To Concepet',80)
           ,(9,'Phase 1 Exit',90)
           ,(10,'Phase 2 [Development]',100)
           ,(11,'Phase 2 Request Cancellation',110)
           ,(12,'Phase 2 [Cancelled]',120)
           ,(13,'Phase 2 [Test]',130)
           ,(14,'Phase 2 [Pre-Qual]',140)
           ,(15,'Phase 2 [Qual]',150)
           ,(16,'Phase 2 [Re-Qual]',160)
           ,(17,'Phase 2 Exit',170)
           ,(18,'Phase 3 Risk ECO in Process',180)
           ,(19,'Phase 3 Released [Risk]',190)
           ,(20,'Phase 3 Released [C-ACT]',200)
           ,(21,'Phase 3 ACT ECO in Process',210)
           ,(22,'Phase 3 Released [ACT]',220)
           
           
           /****** Script for SelectTopNRows command from SSMS  ******/
INSERT INTO [ProjectStatus]
           ([Name]
           ,[Ordinal])
     VALUES
           ('Pending',10),('Pending Cancelation',20),('Canceled',30),('POR',40),('Completed',50)
GO


INSERT INTO durados_ApprovalProcessType([Id], [Name],[Ordinal]) VALUES (1,'Phase 0 Cancel',10),(2,'Phase 0 Exit',20),(3,'Phase 1 Cancel',30),(4,'Phase 1 Exit',40),(5,'Phase 2 Cancel',50)
		,(6,'Phase 2 Exit',60),(7,'Phase 2 To Concept',70),(8,'Required ACT',1000)
	
GO	
INSERT INTO [durados_Html]
           ([Name]
           ,[Text])
     VALUES
     ('PORApprovalProcessCancelMessage','Please Approve Cancel POR request')    
	,('PORApprovalProcessCancelSubject','POR Cancel Approval message')
	,('PORApprovalProcessMessage','Please Approve change phase request')
	,('PORApprovalProcessSubject','POR Phase chage  Approval')
	,('Notify POR phase changed Message','POR phase has changed')
	,('Notify POR phase changed Subject','POR phase has changed')
	
GO	
------------------------------------------------------------------------------------------------------
INSERT INTO [ApprovalProcessTypeProductLine]
           ([ProductLineId]
           ,[ApprovalProcessTypeId])
     VALUES
          (4,1),(20,1),(1,1),(19,1),(5,1),(2,1),(4,2),(20,2),(1,2),(19,2),(5,2),(2,2) ,
          (4,3),(20,3),(1,3),(19,3),(5,3),(2,3),(4,4),(20,4),(1,4),(19,4),(5,4),(2,4)
          , (4,5),(20,5),(1,5),(19,5),(5,5),(2,5),(4,6),(20,6),(1,6),(19,6),(5,6),(2,6),
          (4,7),(20,7),(1,7),(19,7),(5,7),(2,7), (4,8),(20,8),(1,8),(19,8),(5,8),(2,8)
          
GO
-------------------------------------------------------------------------------------------------------
INSERT INTO [ApprovalProcessTypeProductLineDefaultUser]
           ([ApprovalProcessTypeProductLineId]
           ,[DefaultUserId])
     VALUES
(1,	39),(2,	39),(3,	39),(3,	17),(4,	39),(5,	39),(6,	39),(7,	5),(7,	1),(7,	17),(7,	39),(8,	39),(9,	39),(10,	39),(11,	39),(12,	39),(13,	39),(14,	39),
(15,	39),(16,	39),(17,	39),(18,	39),(19,	39),(20,	39),(21,	39),(22,	39),(23,	39),(24,	39),(25,	39),(26,	39),(27,	39),(28,	39),
(29,	39),(30,	39),(31,	39),(32,	39),(33,	39),(34,	39),(35,	39),(36,	39),(37,	39),(38,	39),(39,	39),(40,	39),(41,	39),(42,	39)
,(43,	39),(44,	39),(45,	39),(46,	39),(47,	39),(48,	39)
------------------------------------------------------------------------------------------------------------
	insert into QualProcessLab values ('DVT',10),('Chaz',20),('APS',30),('Q&R',40)
	
	
	-----------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO [DistributionList]
           ([Name]
           ,[Description])
     VALUES
         ('Notification for SSD _ CF PLM BE Status Changed',NULL)
         ,('Notification for iNAND  PLM BE Status Changed',NULL)
         ,('Notification for SD uSD USB PLM BE Status Changed',NULL)
         ,('Notification for PPN  PLM BE Status Changed',NULL)
         ,('Notification CTO on PLM status',NULL)
         ,('Notification for POR phase 1','Notificattt')
         ,('Notification for POR phase 2','Notificattt')
         ,('Notification for POR phase 3','Notificattt')
         
         
         ------------------------------------------------------------------------
 INSERT INTO [ProjectStatus]
           ([Name],[Ordinal])
     VALUES
       ('Pending',10)
       ,('Pending Cancelation',20)
       ,('Canceled',30)
       ,('POR',40)
       ,('Completed',50)




	




