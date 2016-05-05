﻿DROP TABLE Player_W;
CREATE TABLE Player_W
(
	Id						BIGINT					NOT NULL		PRIMARY KEY	IDENTITY,
	Href					NVARCHAR(1000)			NOT NULL,
	Team					CHAR(2),
	InsertDateTime			DATETIME				DEFAULT			CURRENT_TIMESTAMP
);

DROP TABLE Player;
CREATE TABLE Player
(
	Id						BIGINT			NOT NULL		PRIMARY KEY	IDENTITY,
	PlayerId				INT				NOT NULL			UNIQUE,
	Team					NVARCHAR(10)	NOT NULL,
	BackNumber				INT				NULL,
	Name					NVARCHAR(10)	NOT NULL,
	Height					INT				NOT NULL,
	Weight					INT				NOT NULL,
	Position				NVARCHAR(10)	NOT NULL,
	Hand					NVARCHAR(10)	NOT NULL,
	BirthDate				CHAR(8)			NOT NULL,
	Career					NVARCHAR(50)	NULL,
	Deposit					NVARCHAR(50)	NULL,
	Salary					NVARCHAR(20)	NULL,
	Rank					NVARCHAR(50)	NULL,
	JoinYear				NVARCHAR(20)	NULL,
	SCR						NVARCHAR(70)	NULL,
	InsertDateTime			DATETIME		DEFAULT			CURRENT_TIMESTAMP
);