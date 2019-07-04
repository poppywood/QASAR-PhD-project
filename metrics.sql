USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Metrics')
BEGIN
CREATE DATABASE [Metrics] ON  PRIMARY 
( NAME = N'Metrics_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\mssql.1\MSSQL\Data\Metrics_Data.MDF' , SIZE = 8320KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)
 LOG ON 
( NAME = N'Metrics_Log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Data\Metrics_Log.LDF' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END

GO
EXEC dbo.sp_dbcmptlevel @dbname=N'Metrics', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Metrics].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Metrics] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Metrics] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Metrics] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Metrics] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Metrics] SET ARITHABORT OFF 
GO
ALTER DATABASE [Metrics] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Metrics] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Metrics] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Metrics] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Metrics] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Metrics] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Metrics] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Metrics] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Metrics] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Metrics] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Metrics] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Metrics] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Metrics] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Metrics] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Metrics] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Metrics] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Metrics] SET  READ_WRITE 
GO
ALTER DATABASE [Metrics] SET RECOVERY FULL 
GO
ALTER DATABASE [Metrics] SET  MULTI_USER 
GO
ALTER DATABASE [Metrics] SET PAGE_VERIFY TORN_PAGE_DETECTION  
GO
ALTER DATABASE [Metrics] SET DB_CHAINING OFF 
GO
USE [Metrics]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'metrics')
CREATE USER [metrics] FOR LOGIN [metrics] WITH DEFAULT_SCHEMA=[dbo]
GO
GRANT CONNECT TO [metrics]
USE [Metrics]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'metrics')
CREATE USER [metrics] FOR LOGIN [metrics] WITH DEFAULT_SCHEMA=[dbo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Action]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Action](
	[ActionCode] [nvarchar](50) NOT NULL,
	[ActionDescription] [nvarchar](100) NULL,
 CONSTRAINT [PK_Action] PRIMARY KEY CLUSTERED 
(
	[ActionCode] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Filter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Filter](
	[FilterPrefix] [nvarchar](3) NOT NULL,
	[FilterName] [nvarchar](50) NULL,
	[FilterDescription] [nvarchar](100) NULL,
 CONSTRAINT [PK_Filter] PRIMARY KEY CLUSTERED 
(
	[FilterPrefix] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PipelineAudit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PipelineAudit](
	[PipelineAuditID] [int] NOT NULL,
	[PipelineID] [int] NULL,
	[Request] [ntext] NOT NULL,
	[Response] [ntext] NULL,
	[Action] [nvarchar](50) NOT NULL,
	[SubAction] [nvarchar](50) NOT NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[Source] [nvarchar](50) NOT NULL,
	[UserId] [nchar](50) NULL,
	[Result] [bit] NOT NULL,
	[Notification] [bit] NOT NULL,
	[StartDateTime] [datetime] NOT NULL,
	[EndDateTime] [datetime] NULL,
	[Elapsed] [int] NULL,
 CONSTRAINT [PK_PipelineAudit] PRIMARY KEY CLUSTERED 
(
	[PipelineAuditID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Product](
	[ProductCode] [nvarchar](50) NOT NULL,
	[ProductDescription] [nvarchar](50) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductCode] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Security]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Security](
	[LoginID] [nvarchar](20) NOT NULL,
	[Password] [nvarchar](20) NOT NULL,
	[Source] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Security] PRIMARY KEY CLUSTERED 
(
	[LoginID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Source]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Source](
	[SourceCode] [nvarchar](50) NOT NULL,
	[SourceDescription] [nvarchar](100) NULL,
 CONSTRAINT [PK_Source] PRIMARY KEY CLUSTERED 
(
	[SourceCode] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TestValues](
	[Key] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](50) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[User](
	[UserCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserCode] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Variable]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Variable](
	[Variable] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Variable] PRIMARY KEY CLUSTERED 
(
	[Variable] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Version]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Version](
	[Stamp] [char](12) NOT NULL,
	[Version] [char](10) NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FilterErrors]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FilterErrors](
	[FilterID] [int] NOT NULL,
	[FilterName] [nvarchar](50) NULL,
	[PipelineAuditID] [int] NOT NULL,
	[Request] [ntext] NOT NULL,
	[Response] [ntext] NULL,
	[Result] [bit] NULL,
	[Notification] [bit] NULL,
	[StartDateTime] [datetime] NOT NULL,
	[EndDateTime] [datetime] NULL,
	[Elapsed] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Class]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Class](
	[ClassId] [int] NOT NULL,
	[ClassName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED 
(
	[ClassId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Client]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Client](
	[ClientID] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblClient] PRIMARY KEY CLUSTERED 
(
	[ClientID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Flags]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Flags](
	[FlaggedID] [int] NOT NULL,
	[Description] [char](10) NOT NULL,
 CONSTRAINT [PK_tblFlags] PRIMARY KEY CLUSTERED 
(
	[FlaggedID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Resource]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Resource](
	[ResourceID] [int] NOT NULL,
	[Resource] [char](50) NOT NULL,
 CONSTRAINT [PK_Resource] PRIMARY KEY CLUSTERED 
(
	[ResourceID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Value]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Value](
	[ID] [int] NOT NULL,
	[PipelineId] [int] NOT NULL,
	[RulePrefix] [nvarchar](3) NOT NULL,
	[RuleSuffix] [nvarchar](3) NOT NULL,
	[Instance] [int] NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[SubActionCode] [nvarchar](50) NOT NULL,
	[ActionCode] [nvarchar](50) NOT NULL,
	[SourceCode] [nvarchar](50) NOT NULL,
	[UserCode] [nvarchar](50) NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Value] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubAction]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SubAction](
	[SubActionCode] [nvarchar](50) NOT NULL,
	[SubActionDescription] [nvarchar](100) NULL,
	[ActionCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_SubAction] PRIMARY KEY CLUSTERED 
(
	[SubActionCode] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PipelineValue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PipelineValue](
	[ID] [int] NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[SubActionCode] [nvarchar](50) NOT NULL,
	[ActionCode] [nvarchar](50) NOT NULL,
	[SourceCode] [nvarchar](50) NOT NULL,
	[UserCode] [nvarchar](50) NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PipelineValue] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rule]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Rule](
	[RulePrefix] [nvarchar](3) NOT NULL,
	[RuleSuffix] [nvarchar](3) NOT NULL,
	[RuleDescription] [nvarchar](200) NULL,
 CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED 
(
	[RulePrefix] ASC,
	[RuleSuffix] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FilterAudit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FilterAudit](
	[FilterID] [int] NOT NULL,
	[FilterName] [nvarchar](50) NULL,
	[PipelineAuditID] [int] NOT NULL,
	[Request] [ntext] NOT NULL,
	[Response] [ntext] NULL,
	[Result] [bit] NULL,
	[Notification] [bit] NULL,
	[StartDateTime] [datetime] NOT NULL,
	[EndDateTime] [datetime] NULL,
	[Elapsed] [int] NULL,
 CONSTRAINT [PK_FilterAudit] PRIMARY KEY CLUSTERED 
(
	[FilterID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Job]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Job](
	[JobID] [int] NOT NULL,
	[JobSubActionCode] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[JobInterval] [datetime] NOT NULL,
	[DateLastJobRan] [datetime] NOT NULL,
	[DateNextJobStart] [datetime] NOT NULL,
	[SOAPBody] [nvarchar](4000) NULL,
 CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Transaction]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Transaction](
	[ResourceID] [int] NULL,
	[TransactionID] [varchar](50) NOT NULL,
	[ClientID] [varchar](50) NOT NULL,
	[StartDateTime] [datetime] NOT NULL,
	[EndDateTime] [datetime] NOT NULL,
	[ClassID] [int] NOT NULL,
	[JobType] [varchar](20) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[ElapsedMS] [int] NOT NULL,
	[FlagID] [int] NOT NULL CONSTRAINT [DF_tblTransaction_Stats]  DEFAULT ((1)),
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[TransactionID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_Transaction] UNIQUE NONCLUSTERED 
(
	[TransactionID] ASC,
	[ClientID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClassResource]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ClassResource](
	[CRID] [int] IDENTITY(1,1) NOT NULL,
	[ResourceId] [int] NOT NULL,
	[ClassId] [int] NOT NULL,
	[EquivalentLoad] [bigint] NOT NULL,
 CONSTRAINT [PK_ClassResource] PRIMARY KEY CLUSTERED 
(
	[CRID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ResourceMeasurement]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ResourceMeasurement](
	[MeasurementId] [int] IDENTITY(1,1) NOT NULL,
	[CRID] [int] NOT NULL,
	[Load] [int] NOT NULL,
	[Interval] [int] NOT NULL,
	[Average] [numeric](12, 6) NOT NULL,
	[CVS] [numeric](18, 6) NULL,
 CONSTRAINT [PK_ResourceMeasurement] PRIMARY KEY CLUSTERED 
(
	[MeasurementId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vClassResource]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW [dbo].[vClassResource]
AS
SELECT     dbo.ClassResource.CRID, dbo.Class.ClassName, dbo.Resource.Resource, dbo.ClassResource.EquivalentLoad
FROM         dbo.Class INNER JOIN
                      dbo.ClassResource ON dbo.Class.ClassId = dbo.ClassResource.ClassId INNER JOIN
                      dbo.Resource ON dbo.ClassResource.ResourceId = dbo.Resource.ResourceID

' 
GO
USE [Metrics]
GO
USE [Metrics]
GO
USE [Metrics]
GO
USE [Metrics]
GO
USE [Metrics]
GO
USE [Metrics]
GO
USE [Metrics]
GO
USE [Metrics]
GO
USE [Metrics]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Value_Action]') AND parent_object_id = OBJECT_ID(N'[dbo].[Value]'))
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_Value_Action] FOREIGN KEY([ActionCode])
REFERENCES [dbo].[Action] ([ActionCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Value_Product]') AND parent_object_id = OBJECT_ID(N'[dbo].[Value]'))
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_Value_Product] FOREIGN KEY([ProductCode])
REFERENCES [dbo].[Product] ([ProductCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Value_Rule]') AND parent_object_id = OBJECT_ID(N'[dbo].[Value]'))
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_Value_Rule] FOREIGN KEY([RulePrefix], [RuleSuffix])
REFERENCES [dbo].[Rule] ([RulePrefix], [RuleSuffix])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Value_Source]') AND parent_object_id = OBJECT_ID(N'[dbo].[Value]'))
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_Value_Source] FOREIGN KEY([SourceCode])
REFERENCES [dbo].[Source] ([SourceCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Value_SubAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[Value]'))
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_Value_SubAction] FOREIGN KEY([SubActionCode])
REFERENCES [dbo].[SubAction] ([SubActionCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Value_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[Value]'))
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_Value_User] FOREIGN KEY([UserCode])
REFERENCES [dbo].[User] ([UserCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SubAction_Action]') AND parent_object_id = OBJECT_ID(N'[dbo].[SubAction]'))
ALTER TABLE [dbo].[SubAction]  WITH CHECK ADD  CONSTRAINT [FK_SubAction_Action] FOREIGN KEY([ActionCode])
REFERENCES [dbo].[Action] ([ActionCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PipelineValue_Action]') AND parent_object_id = OBJECT_ID(N'[dbo].[PipelineValue]'))
ALTER TABLE [dbo].[PipelineValue]  WITH CHECK ADD  CONSTRAINT [FK_PipelineValue_Action] FOREIGN KEY([ActionCode])
REFERENCES [dbo].[Action] ([ActionCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PipelineValue_Product]') AND parent_object_id = OBJECT_ID(N'[dbo].[PipelineValue]'))
ALTER TABLE [dbo].[PipelineValue]  WITH CHECK ADD  CONSTRAINT [FK_PipelineValue_Product] FOREIGN KEY([ProductCode])
REFERENCES [dbo].[Product] ([ProductCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PipelineValue_Source]') AND parent_object_id = OBJECT_ID(N'[dbo].[PipelineValue]'))
ALTER TABLE [dbo].[PipelineValue]  WITH CHECK ADD  CONSTRAINT [FK_PipelineValue_Source] FOREIGN KEY([SourceCode])
REFERENCES [dbo].[Source] ([SourceCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PipelineValue_SubAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[PipelineValue]'))
ALTER TABLE [dbo].[PipelineValue]  WITH CHECK ADD  CONSTRAINT [FK_PipelineValue_SubAction] FOREIGN KEY([SubActionCode])
REFERENCES [dbo].[SubAction] ([SubActionCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PipelineValue_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[PipelineValue]'))
ALTER TABLE [dbo].[PipelineValue]  WITH CHECK ADD  CONSTRAINT [FK_PipelineValue_User] FOREIGN KEY([UserCode])
REFERENCES [dbo].[User] ([UserCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Rule_Filter]') AND parent_object_id = OBJECT_ID(N'[dbo].[Rule]'))
ALTER TABLE [dbo].[Rule]  WITH CHECK ADD  CONSTRAINT [FK_Rule_Filter] FOREIGN KEY([RulePrefix])
REFERENCES [dbo].[Filter] ([FilterPrefix])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FilterAudit_PipelineAudit]') AND parent_object_id = OBJECT_ID(N'[dbo].[FilterAudit]'))
ALTER TABLE [dbo].[FilterAudit]  WITH CHECK ADD  CONSTRAINT [FK_FilterAudit_PipelineAudit] FOREIGN KEY([PipelineAuditID])
REFERENCES [dbo].[PipelineAudit] ([PipelineAuditID])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Job_SubAction]') AND parent_object_id = OBJECT_ID(N'[dbo].[Job]'))
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_SubAction] FOREIGN KEY([JobSubActionCode])
REFERENCES [dbo].[SubAction] ([SubActionCode])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransaction_tblClient]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transaction]'))
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_tblTransaction_tblClient] FOREIGN KEY([ClientID])
REFERENCES [dbo].[Client] ([ClientID])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransaction_tblFlags]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transaction]'))
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_tblTransaction_tblFlags] FOREIGN KEY([FlagID])
REFERENCES [dbo].[Flags] ([FlaggedID])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transaction_Class]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transaction]'))
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Class] FOREIGN KEY([ClassID])
REFERENCES [dbo].[Class] ([ClassId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transaction_Resource]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transaction]'))
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Resource] FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resource] ([ResourceID])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ClassResource_Class]') AND parent_object_id = OBJECT_ID(N'[dbo].[ClassResource]'))
ALTER TABLE [dbo].[ClassResource]  WITH CHECK ADD  CONSTRAINT [FK_ClassResource_Class] FOREIGN KEY([ClassId])
REFERENCES [dbo].[Class] ([ClassId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ClassResource_Resource]') AND parent_object_id = OBJECT_ID(N'[dbo].[ClassResource]'))
ALTER TABLE [dbo].[ClassResource]  WITH CHECK ADD  CONSTRAINT [FK_ClassResource_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceID])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ResourceMeasurement_ClassResource]') AND parent_object_id = OBJECT_ID(N'[dbo].[ResourceMeasurement]'))
ALTER TABLE [dbo].[ResourceMeasurement]  WITH CHECK ADD  CONSTRAINT [FK_ResourceMeasurement_ClassResource] FOREIGN KEY([CRID])
REFERENCES [dbo].[ClassResource] ([CRID])
