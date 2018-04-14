
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/14/2018 15:54:45
-- Generated from EDMX file: C:\Users\Hukuma\Documents\Visual Studio 2015\Projects\_home\WpfNodes\WpfNodes\Entity\ModelBase.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
-- CREATE DATABASE Nodes;
GO
USE [Nodes];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_TablesNodes_parent_id]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TablesNodes] DROP CONSTRAINT [FK_TablesNodes_parent_id];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[TablesNodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TablesNodes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'TablesNodes'
CREATE TABLE [dbo].[TablesNodes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NameNode] varchar(50)  NULL,
    [ParentId] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'TablesNodes'
ALTER TABLE [dbo].[TablesNodes]
ADD CONSTRAINT [PK_TablesNodes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ParentId] in table 'TablesNodes'
ALTER TABLE [dbo].[TablesNodes]
ADD CONSTRAINT [FK_TablesNodes_parent_id]
    FOREIGN KEY ([ParentId])
    REFERENCES [dbo].[TablesNodes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TablesNodes_parent_id'
CREATE INDEX [IX_FK_TablesNodes_parent_id]
ON [dbo].[TablesNodes]
    ([ParentId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------