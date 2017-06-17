USE master
GO
CREATE DATABASE PTMessenger
GO
USE PTMessenger
EXEC sp_changedbowner 'sa'

IF OBJECT_ID('TLastLogin','U') IS NOT NULL
	DROP TABLE dbo.TLastLogin;
IF OBJECT_ID('TUsers','U') IS NOT NULL
	DROP TABLE dbo.TUsers;

CREATE TABLE TUsers (
	TUsers_id integer identity(1,1) not null PRIMARY KEY,
	TUsers_login varchar(50) not null,
	TUsers_email varchar(100) not null,
	TUsers_passwd varchar(128) not null,
	TUsers_desc varchar(300)
);

CREATE TABLE TLastLogin (
	TLastLogin_id integer identity(1,1) not null PRIMARY KEY,
	TLastLogin_TUserID integer REFERENCES TUsers,
	TLastLogin_TS timestamp,
	TLastLogin_UserIP varchar(30)
);

INSERT INTO dbo.TUsers (TUsers_login, TUsers_email, TUsers_passwd, TUsers_desc)
VALUES
	('silvio','silvio10@wp.pl','trojan1','Taki sobie typ'),
	('marian','marian10@wp.pl','trojan2','Podejrzany od poczêcia');