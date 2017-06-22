
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/21/2017 20:43:40
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

IF OBJECT_ID(N'[dbo].[FK_TLastLoginTUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TLastLogin] DROP CONSTRAINT [FK_TLastLoginTUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_TUsersTMessage_src]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TMessageSet] DROP CONSTRAINT [FK_TUsersTMessage_src];
GO
IF OBJECT_ID(N'[dbo].[FK_TUsersTMessage_dst]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TMessageSet] DROP CONSTRAINT [FK_TUsersTMessage_dst];
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
IF OBJECT_ID(N'[dbo].[TMessageSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TMessageSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'TLastLogin'
CREATE TABLE [dbo].[TLastLogin] (
    [TLastLogin_id] int IDENTITY(1,1) NOT NULL,
    [TLastLogin_TS] datetime  NOT NULL,
    [TLastLogin_UserIP] varchar(30)  NOT NULL,
    [TLastLogin_TUsers_id] int  NOT NULL,
    [TLastLogin_Success] bit  NOT NULL
);
GO

-- Creating table 'TUsers'
CREATE TABLE [dbo].[TUsers] (
    [TUsers_id] int IDENTITY(1,1) NOT NULL,
    [TUsers_login] varchar(50)  NOT NULL,
    [TUsers_email] varchar(100)  NOT NULL,
    [TUsers_passwd] varchar(128)  NOT NULL,
    [TUsers_desc] varchar(300)  NULL,
    [TUser_imie] nvarchar(max)  NOT NULL,
    [TUser_nazwisko] nvarchar(max)  NOT NULL,
    [TUser_lock] bit  NOT NULL
);
GO

-- Creating table 'TMessageSet'
CREATE TABLE [dbo].[TMessageSet] (
    [TMessage_id] int IDENTITY(1,1) NOT NULL,
    [TMessage_text] nvarchar(max)  NOT NULL,
    [TMessage_ts] datetime  NOT NULL,
    [TMessage_src] int  NOT NULL,
    [TMessage_dst] int  NOT NULL
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

-- Creating primary key on [TMessage_id] in table 'TMessageSet'
ALTER TABLE [dbo].[TMessageSet]
ADD CONSTRAINT [PK_TMessageSet]
    PRIMARY KEY CLUSTERED ([TMessage_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TLastLogin_TUsers_id] in table 'TLastLogin'
ALTER TABLE [dbo].[TLastLogin]
ADD CONSTRAINT [FK_TLastLoginTUsers]
    FOREIGN KEY ([TLastLogin_TUsers_id])
    REFERENCES [dbo].[TUsers]
        ([TUsers_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TLastLoginTUsers'
CREATE INDEX [IX_FK_TLastLoginTUsers]
ON [dbo].[TLastLogin]
    ([TLastLogin_TUsers_id]);
GO

-- Creating foreign key on [TMessage_src] in table 'TMessageSet'
ALTER TABLE [dbo].[TMessageSet]
ADD CONSTRAINT [FK_TUsersTMessage_src]
    FOREIGN KEY ([TMessage_src])
    REFERENCES [dbo].[TUsers]
        ([TUsers_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TUsersTMessage_src'
CREATE INDEX [IX_FK_TUsersTMessage_src]
ON [dbo].[TMessageSet]
    ([TMessage_src]);
GO

-- Creating foreign key on [TMessage_dst] in table 'TMessageSet'
ALTER TABLE [dbo].[TMessageSet]
ADD CONSTRAINT [FK_TUsersTMessage_dst]
    FOREIGN KEY ([TMessage_dst])
    REFERENCES [dbo].[TUsers]
        ([TUsers_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TUsersTMessage_dst'
CREATE INDEX [IX_FK_TUsersTMessage_dst]
ON [dbo].[TMessageSet]
    ([TMessage_dst]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------