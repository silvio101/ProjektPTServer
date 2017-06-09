
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/09/2017 18:47:07
-- Generated from EDMX file: D:\=MAKING OF=\PT_MessengerServer\PT_MessengerServer\PTMessengerModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [PTMessenger];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__TLastLogi__TLast__1273C1CD]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TLastLogin] DROP CONSTRAINT [FK__TLastLogi__TLast__1273C1CD];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[TLastLogin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TLastLogin];
GO
IF OBJECT_ID(N'[dbo].[TUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TUsers];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'TLastLogin'
CREATE TABLE [dbo].[TLastLogin] (
    [TLastLogin_id] int IDENTITY(1,1) NOT NULL,
    [TLastLogin_TUserID] int  NULL,
    [TLastLogin_TS] timestamp  NOT NULL,
    [TLastLogin_UserIP] varchar(30)  NULL
);
GO

-- Creating table 'TUsers'
CREATE TABLE [dbo].[TUsers] (
    [TUsers_id] int IDENTITY(1,1) NOT NULL,
    [TUsers_login] varchar(50)  NOT NULL,
    [TUsers_email] varchar(100)  NOT NULL,
    [TUsers_passwd] varchar(128)  NOT NULL,
    [TUsers_desc] varchar(300)  NULL,
    [TUser_imie] nvarchar(max)  NULL,
    [TUser_nazwisko] nvarchar(max)  NULL,
    [TUser_lock] bit NOT NULL DEFAULT 0
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [TLastLogin_id] in table 'TLastLogin'
ALTER TABLE [dbo].[TLastLogin]
ADD CONSTRAINT [PK_TLastLogin]
    PRIMARY KEY CLUSTERED ([TLastLogin_id] ASC);
GO

-- Creating primary key on [TUsers_id] in table 'TUsers'
ALTER TABLE [dbo].[TUsers]
ADD CONSTRAINT [PK_TUsers]
    PRIMARY KEY CLUSTERED ([TUsers_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TLastLogin_TUserID] in table 'TLastLogin'
ALTER TABLE [dbo].[TLastLogin]
ADD CONSTRAINT [FK__TLastLogi__TLast__1273C1CD]
    FOREIGN KEY ([TLastLogin_TUserID])
    REFERENCES [dbo].[TUsers]
        ([TUsers_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__TLastLogi__TLast__1273C1CD'
CREATE INDEX [IX_FK__TLastLogi__TLast__1273C1CD]
ON [dbo].[TLastLogin]
    ([TLastLogin_TUserID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------